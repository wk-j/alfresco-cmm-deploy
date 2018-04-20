module CmmDeploy.Program

open System.Net.Http
open System.IO
open System.Text

let uploadUrl = sprintf "%s/s/api/cmm/upload"
let activateUrl = sprintf "%s/api/-default-/private/alfresco/versions/1/cmm/%s?select=status"

let uploadModel baseUrl zip =
    let bytes = File.ReadAllBytes (zip: string)
    use client = new HttpClient()
    use content = new MultipartFormDataContent("--separator--")

    let data: HttpContent = upcast ( new StreamContent(new MemoryStream(bytes)))
    content.Add(data, "file", Path.GetFileName zip)

    use message =
        client.PostAsync(uploadUrl baseUrl, content)
        |> Async.AwaitTask
        |> Async.RunSynchronously

    let result =
        message.Content.ReadAsStringAsync()
        |> Async.AwaitTask
        |> Async.RunSynchronously

    (result)

let activateModel baseUrl name =
    use client = new HttpClient()
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

[<EntryPoint>]
let main argv =
    if argv.Length = 2 then
        let baseUrl = argv.[0]
        let zip = argv.[1]

        let name = Path.GetFileNameWithoutExtension zip

        uploadModel baseUrl zip
        |> printfn "Upload => %A"

        activateModel baseUrl name
        |> printfn "Activate => %A"
    0