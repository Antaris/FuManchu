// Arguments

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");

// Configuration

var folders = new 
{
    build = "./artifacts/",
    solution = "./",
    src = "./src/",
    tests = "./test/",
    testResults = "./artifacts/test-results/"
};

// Clean
Task("Clean")
    .Description("Cleans the working and build output directories")
    .Does(() =>
    {
        CleanDirectories(new DirectoryPath[] {
            folders.build
        });

        CleanDirectories("./src/**/" + configuration);
        CleanDirectories("./test/**/" + configuration);
    });

// Restore

Task("Restore-NuGet")
    .Description("Restores NuGet packages")
    .Does(() =>
    {
        DotNetCoreRestore(folders.solution);
    });

// Build

Task("Build")
    .Description("Builds all projects in the solution")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore-NuGet")
    .Does(() =>
    {
        DotNetCoreBuild(folders.solution, new DotNetCoreBuildSettings
        {
            Configuration = configuration
        });
    });

// Test

Task("Test")
    .Description("Runs unit tests")
    .IsDependentOn("Build")
    .Does(() =>
    {
        CreateDirectory(folders.testResults);

        var tests = GetFiles(folders.tests + "**/*.csproj");
        foreach (var test in tests)
        {
            string folder = System.IO.Path.GetDirectoryName(test.FullPath);
            string project = folder.Substring(folder.LastIndexOf('\\') + 1);
            string resultsFile = folders.testResults + project + ".xml";

            using (var process = StartAndReturnProcess("dotnet", new ProcessSettings 
                {
                    Arguments = "xunit -xml ../../" + resultsFile + " -internaldiagnostics",
                    WorkingDirectory = folder
                }))
            {
                process.WaitForExit();

                if (AppVeyor.IsRunningOnAppVeyor)
                {
                    AppVeyor.UploadTestResults(resultsFile, AppVeyorTestResultsType.XUnit);
                }
            }
        }
    });

// Packaging

Task("Pack")
    .Description("Packs the output of projects")
    .IsDependentOn("Test")
    .Does(() =>
    {
        var projects = GetFiles(folders.src + "**/*.csproj");
        foreach (var project in projects)
        {
            DotNetCorePack(project.FullPath, new DotNetCorePackSettings
            {
                ArgumentCustomization = args =>
                {
                    args.Append("--include-symbols");
                    return args;
                },
                Configuration = configuration,
                OutputDirectory = folders.build,
                NoBuild = true
            });
        }
    });

// Default

Task("Default")
.IsDependentOn("Pack")
;

// Execution

RunTarget(target);