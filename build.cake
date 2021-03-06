#addin "wk.StartProcess"
#addin "wk.ProjectParser"

using PS = StartProcess.Processor;
using ProjectParser;

var publishDir = ".publish";
var version = DateTime.Now.ToString("yy.MM.dd.HHmm");

Task("Test").Does(() => {
    PS.StartProcess("dotnet run --project src/CmmDeploy http://admin:admin@localhost:8080/alfresco resources/NBTCcrm.zip");
});

Task("Pack").Does(() => {
    CleanDirectory(publishDir);
    var settings = new DotNetCoreMSBuildSettings();
    settings.Properties["Version"] = new string[] { version };

    DotNetCorePack("src/CmmDeploy", new DotNetCorePackSettings {
        OutputDirectory = publishDir,
        MSBuildSettings = settings
    });
});

Task("Publish-Nuget")
    .IsDependentOn("Pack")
    .Does(() => {
        var npi = EnvironmentVariable("npi");
        var nupkg = new DirectoryInfo(publishDir).GetFiles("*.nupkg").LastOrDefault();
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
        PS.StartProcess($"dotnet tool install -g wk.{name} --add-source {currentDir}/{publishDir} --version {version}");
});

var target = Argument("target", "Default");
RunTarget(target);