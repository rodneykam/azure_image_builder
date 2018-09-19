#tool "nuget:?package=JetBrains.dotCover.CommandLineTools"

//
// By default, we'll do everything
//
var target = Argument("target", "Default");

//
// Versioning
//
var version = EnvironmentVariable("AssemblyVersion") ?? "1.0.0.0";
var fileVersion= EnvironmentVariable("AssemblyFileVersion") ?? "1.0.0.0";
var informationalVersion = EnvironmentVariable("AssemblyInformationalVersion") ?? "1.0.0.0-private";
var nugetPackageVersion = EnvironmentVariable("NuGetPackageVersion") ?? "1.0.0-private";

//
// The configuration, if any
// 
var configuration = Argument("configuration", "Release");

//
// Build platform specific assemblies
//
string[] runtimes = new[] {
    "win-x86",
    "win-x64",
    "win10-x64",
    "win10-x86",
};

Task("Clean")
    .Does(() =>
    {
        foreach(var dir in GetDirectories("**/bin"))
        {
            Information($"Deleting `{dir.FullPath}`");
            DeleteDirectory(dir.FullPath, new DeleteDirectorySettings {
                Recursive = true,
                Force = true
            });
        }
        foreach(var dir in GetDirectories("**/obj"))
        {
            Information($"Deleting `{dir.FullPath}`");
            DeleteDirectory(dir.FullPath, new DeleteDirectorySettings {
                Recursive = true,
                Force = true
            });
        }
        foreach(var dir in GetDirectories("./target"))
        {
            Information($"Deleting `{dir}`");
            DeleteDirectory(dir.FullPath, new DeleteDirectorySettings {
                Recursive = true,
                Force = true
            });
        }
        foreach(var dir in GetDirectories("./packages"))
        {
            Information($"Deleting `{dir}`");
            DeleteDirectory(dir.FullPath, new DeleteDirectorySettings {
                Recursive = true,
                Force = true
            });
        }
    });



Task("Restore")
    .Does(() => 
{
    var restoreSettings = new DotNetCoreRestoreSettings
    {
        Sources = new[] {
            "https://packages.relayhealth.com/api/nuget/www.nuget.org",
            "https://packages.relayhealth.com/api/nuget/nuget", 
        },
        DisableParallel = false,
        NoCache = false,
    };
    DotNetCoreRestore(restoreSettings);
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() => 
{
    {
        var settings = new DotNetCoreMSBuildSettings()
            .SetMaxCpuCount(-1)
            .SetConfiguration(configuration)
            .SuppressVersionRecommendedFormatWarning()
            .WithProperty("PackageVersion", nugetPackageVersion)
            .WithProperty("Version", informationalVersion)
            .SetVersion(version)
            .SetFileVersion(fileVersion)
            .SetInformationalVersion(informationalVersion)
            .ShowDetailedSummary();

        DotNetCoreMSBuild(settings);    
    }

     string[] projects = new[] {
        "./src/azure_image_builder/azure_image_builder.csproj"
    };

    foreach (var projectPath in projects)
    {
        string runtimeIdentifiers = XmlPeek(projectPath, "/Project/PropertyGroup/RuntimeIdentifiers");
        var runtimes = runtimeIdentifiers.Split(';');

        var projectName = System.IO.Path.GetFileNameWithoutExtension(projectPath);
        foreach (var r in runtimes)
        {
            var runtime = r.Trim();
            var settings = new DotNetCoreMSBuildSettings()
                .SetMaxCpuCount(-1)
                .SetConfiguration(configuration)
                .SetRuntime(runtime)
                .SuppressVersionRecommendedFormatWarning()
                .WithProperty("PackageVersion", nugetPackageVersion)
                .WithProperty("Version", informationalVersion)
                .SetVersion(version)
                .SetFileVersion(fileVersion)
                .SetInformationalVersion(informationalVersion)
                .ShowDetailedSummary();

            DotNetCoreMSBuild(settings);    
        }
    }
});

Task("Publish")
    .IsDependentOn("Build")
    .Does(() => 
{
    string[] projects = new[] {
        "./src/azure_image_builder/azure_image_builder.csproj"
    };

    foreach (var projectPath in projects)
    {
        string runtimeIdentifiers = XmlPeek(projectPath, "/Project/PropertyGroup/RuntimeIdentifiers");
        var runtimes = runtimeIdentifiers.Split(';');
        var projectName = System.IO.Path.GetFileNameWithoutExtension(projectPath);
        foreach (var r in runtimes)
        {
            var runtime = r.Trim();
            var publishPath = $"./target/tools/{projectName}/{runtime}";
            Information($"Publishing {projectPath}...");    
            var settings = new DotNetCorePublishSettings
            {
                Configuration = configuration,
                OutputDirectory = publishPath,
                Runtime = runtime,
                MSBuildSettings = new DotNetCoreMSBuildSettings()
                    .SetMaxCpuCount(-1)
                    .SetConfiguration(configuration)
                    .SetRuntime(runtime)
                    .SuppressVersionRecommendedFormatWarning()
                    .WithProperty("PackageVersion", nugetPackageVersion)
                    .WithProperty("Version", informationalVersion)
                    .SetVersion(version)
                    .SetFileVersion(fileVersion)
                    .SetInformationalVersion(informationalVersion)
                    .ShowDetailedSummary(),
            };
            DotNetCorePublish(projectPath, settings);
        }
    }

    //
    // Package Functional Tests. Beware that Rosie will infer paths for restoring tests and running them.
    //
    projects = new[] {
        "./test/functional/azure_image_builder.FunctionalTests/azure_image_builder.FunctionalTests.csproj"
    };

    foreach (var projectPath in projects)
    {
        var projectName = System.IO.Path.GetFileNameWithoutExtension(projectPath);
        var publishPath = $"../../../target/tests/{projectName}";
        var settings = new DotNetCoreMSBuildSettings()
            .SetMaxCpuCount(-1)
            .SetConfiguration(configuration)
            .WithTarget("Publish")
            .SuppressVersionRecommendedFormatWarning()
            .WithProperty("PackageVersion", nugetPackageVersion)
            .WithProperty("PublishDir", publishPath)
            .WithProperty("Version", informationalVersion)
            .SetVersion(version)
            .SetFileVersion(fileVersion)
            .SetInformationalVersion(informationalVersion)
            .ShowDetailedSummary();

        DotNetCoreMSBuild(projectPath, settings);    
    }
});

Task("Test")
    .Does(() =>
{
    var projectFiles = GetFiles("./test/unit/**/*.csproj");
    foreach(var file in projectFiles)
    {
        DotNetCoreTest(file.FullPath);
    }
});

Task("Default")
  .IsDependentOn("Clean")
  .IsDependentOn("Restore")
  .IsDependentOn("Build")
  .IsDependentOn("Test")
  .IsDependentOn("Publish");

 Task("Rebuild")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("Build");  

RunTarget(target);