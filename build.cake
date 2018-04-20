#addin "wk.StartProcess"

using PS = StartProcess.Processor;

Task("Test").Does(() => {
    PS.StartProcess("dotnet run --project src/CmmDeploy http://admin:admin@localhost:8080/alfresco resources/NBTCcrm.zip");
});

Task("Pack").Does(() => {
    CleanDirectory("publish");
    DotNetCorePack("src/CmmDeploy", new DotNetCorePackSettings {
        OutputDirectory = "publish"
    });
});

Task("Publish-Nuget")
    .IsDependentOn("Pack")
    .Does(() => {
        var npi = EnvironmentVariable("npi");
        var nupkg = new DirectoryInfo("publish").GetFiles("*.nupkg").LastOrDefault();
        var package = nupkg.FullName;
        NuGetPush(package, new NuGetPushSettings {
            Source = "https://www.nuget.org/api/v2/package",
            ApiKey = npi
        });
});

Task("Install")
    .IsDependentOn("Pack")
    .Does(() => {
        PS.StartProcess("rm /Users/wk/.dotnet/tools/bc-cmm-deploy");
        PS.StartProcess("dotnet install tool -g BCircle.CmmDeploy --source ./publish");
});

var target = Argument("target", "Default");
RunTarget(target);