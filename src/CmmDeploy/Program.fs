module CmmDeploy.Program

open System.Net.Http
open System.IO
open System.Text
open System.Net.Http.Headers

let uploadUrl = sprintf "%s/s/api/cmm/upload"
let activateUrl = sprintf "%s/api/-default-/private/alfresco/versions/1/cmm/%s?select=status"

type Options = {
    BaseUrl: string
    User: string
    Password: string
    Zip: string
}

let basic user password =
    let base64String = System.Convert.ToBase64String(Encoding.ASCII.GetBytes(sprintf "%s:%s" user password));
    AuthenticationHeaderValue("Basic",base64String);

let uploadModel options  =
    let zip = options.Zip
    let user = options.User
    let password = options.Password
    let baseUrl = options.BaseUrl

    let bytes = File.ReadAllBytes (zip: string)
    use client = new HttpClient()
    use content = new MultipartFormDataContent("--separator--")

    let data: HttpContent = upcast ( new StreamContent(new MemoryStream(bytes)))
    content.Add(data, "file", Path.GetFileName zip)

    use message =
        client.DefaultRequestHeaders.Authorization <- basic user password

        client.PostAsync(uploadUrl baseUrl, content)
        |> Async.AwaitTask
        |> Async.RunSynchronously

    let result =
        message.Content.ReadAsStringAsync()
        |> Async.AwaitTask
        |> Async.RunSynchronously

    (result)

let activateModel options name =
    let user = options.User
    let password = options.Password
    let baseUrl = options.BaseUrl

    use client = new HttpClient()
    client.DefaultRequestHeaders.Authorization <- basic user password
    let content = """{"status": "ACTIVE" }"""

    use request = new HttpRequestMessage(HttpMethod.Put, activateUrl baseUrl name)
    request.Content <- new StringContent(content, Encoding.UTF8, "application/json")

    let rs =
        client.SendAsync(request)
        |> Async.AwaitTask
        |> Async.RunSynchronously

    let result =
        rs.Content.ReadAsStringAsync()
        |> Async.AwaitTask
        |> Async.RunSynchronously
    (result)

let rec parseArgs options args =
    let next args = parseArgs options args

    match args with
    | "--url" :: xs ->
        match xs with
        | value :: xss ->
            let newOptions = { options with BaseUrl = value }
            parseArgs newOptions xss
        | _ -> next xs
    | "--user" :: xs ->
        match xs with
        | value :: xss ->
            let newOptions = { options with User = value }
            parseArgs newOptions xss
        | _ -> next xs
    | "--password"  :: xs ->
        match xs with
        | value :: xss ->
            let newOptions = { options with Password = value}
            parseArgs newOptions xss
        | _ -> next xs
    | [zip] -> { options with Zip = zip }
    | _ -> options

let defaultOptions() =
    { BaseUrl = "http://localhost:8080/alfrsco"
      User = "admin"
      Password = "admin"
      Zip = "" }

[<EntryPoint>]
let main argv =
    printfn "parse command line # %s" (System.String.Join(" ", argv))
    let args = List.ofArray argv
    let options = parseArgs (defaultOptions()) args
    let name = Path.GetFileNameWithoutExtension options.Zip

    uploadModel options
    |> printfn "Upload => %s"

    activateModel options name
    |> printfn "Activate => %s"
    0