#addin "wk.StartProcess"
#addin "wk.ProjectParser"

using PS = StartProcess.Processor;
using ProjectParser;


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
        var name = "CmmDeploy";
        var info = Parser.Parse($"src/{name}/{name}.fsproj");
        var currentDir = new System.IO.DirectoryInfo(".").FullName;
        PS.StartProcess($"dotnet tool install -g BCircle.{name} --add-source {currentDir}/publish --version {info.Version}");
});


var target = Argument("target", "Default");
RunTarget(target);