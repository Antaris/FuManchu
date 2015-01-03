// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------

(*
    This file handles the complete build process of FuManchu

    The first step is handled in build.sh and build.cmd by bootstrapping a NuGet.exe and 
    executing NuGet to resolve all build dependencies (dependencies required for the build to work, for example FAKE)

    The secound step is executing this file which resolves all dependencies, builds the solution and executes all unit tests
*)


// Supended until FAKE supports custom mono parameters
#I @".nuget/Build/FAKE/tools/" // FAKE
#r @"FakeLib.dll"  //FAKE

#load @"buildConfig.fsx"
open BuildConfig

open System.Collections.Generic
open System.IO

open Fake
open Fake.Git
open Fake.FSharpFormatting
open AssemblyInfoFile


let MyTarget name body =
    Target name (fun _ -> body false)
    let single = (sprintf "%s_single" name)
    Target single (fun _ -> body true) 

// Targets
MyTarget "Clean" (fun _ ->
    CleanDirs [ buildDir ] //[ buildDir; testDir; releaseDir ]
)

MyTarget "CleanAll" (fun _ ->
  Directory.EnumerateDirectories BuildConfig.nugetDir
  |> Seq.collect (fun dir ->
      let name = Path.GetFileName dir
      if name = "Build" then
        Directory.EnumerateDirectories dir
        |> Seq.filter (fun buildDepDir ->
            let buildDepName = Path.GetFileName buildDepDir
            buildDepName <> "FAKE")
      else
        Seq.singleton dir)
  |> Seq.iter (fun dir ->
      try
        DeleteDir dir
      with exn ->
        traceError (sprintf "Unable to delete %s: %O" dir exn))
)

MyTarget "RestorePackages" (fun _ -> 
    // will catch src/targetsDependencies
    !! "./src/**/packages.config"
    |> Seq.iter 
        (RestorePackage (fun param ->
            { param with    
                // ToolPath = ""
                OutputPath = BuildConfig.packageDir }))
)

MyTarget "SetVersions" (fun _ -> 
    let info =
        [Attribute.Company "FuManchu"
         Attribute.Product "FuManchu"
         Attribute.Copyright "Copyright © FuManchu Project 2015"
         Attribute.Version version
         Attribute.FileVersion version
         Attribute.InformationalVersion version_nuget]
    CreateCSharpAssemblyInfo "./src/SharedAssemblyInfo.cs" info
)

MyTarget "CopyToRelease" (fun _ ->
    trace "Copying to release because test was OK."
    CleanDirs [ outLibDir ]
    System.IO.Directory.CreateDirectory(outLibDir) |> ignore

    // Copy RazorEngine.dll to release directory
    [ "net40"; "net45" ] 
        |> Seq.map (fun t -> buildDir @@ t, t)
        |> Seq.filter (fun (p, t) -> Directory.Exists p)
        |> Seq.iter (fun (source, target) ->
            let outDir = outLibDir @@ target 
            ensureDirectory outDir
            [ "FuManchu.dll"
              "FuManchu.xml" ]
            |> Seq.filter (fun (file) -> File.Exists (source @@ file))
            |> Seq.iter (fun (file) ->
                let newfile = outDir @@ Path.GetFileName file
                File.Copy(source @@ file, newfile))
        )

    // TODO: Copy documentation
    // Zip?
    // Versioning?
)

MyTarget "BuildApp_45" (fun _ -> buildApp net45Params)
MyTarget "BuildApp_40" (fun _ -> buildApp net40Params)
//MyTarget "TestApp_45" (fun _ -> buildTests net45Params)
//MyTarget "TestApp_40" (fun _ -> buildTests net40Params)

Target "All" (fun _ ->
    trace "All finished!"
)

MyTarget "NuGet" (fun _ ->
  let outDir = releaseDir @@ "nuget"
  ensureDirectory outDir
  NuGet (fun p ->
    { p with
        Authors = authors
        Project = projectName
        Summary = projectSummary
        Description = projectDescription
        Version = version_nuget
        ReleaseNotes = toLines release.Notes
        Tags = tags
        OutputPath = outDir
        AccessKey = getBuildParamOrDefault "nugetkey" ""
        Publish = hasBuildParam "nugetkey"
        DependenciesByFramework = 
          [ { FrameworkVersion = "net40";
              Dependencies = [] } ] })
    "nuget/FuManchu.nuspec"
)

MyTarget "VersionBump" (fun _ ->
    // Build updates the SharedAssemblyInfo.cs files.
    let changedFiles = Fake.Git.FileStatus.getChangedFilesInWorkingCopy "" "HEAD" |> Seq.toList
    if changedFiles |> Seq.isEmpty |> not then
        for (status, file) in changedFiles do
            printfn "File %s changed (%A)" file status
        printf "version bump commit? (y,n): "
        let line = System.Console.ReadLine()
        if line = "y" then
            StageAll ""
            Commit "" (sprintf "Bump version to %s" release.NugetVersion)
        
            printf "create tag? (y,n): "
            let line = System.Console.ReadLine()
            if line = "y" then
                Branches.tag "" release.NugetVersion
                Branches.pushTag "" "origin" release.NugetVersion
            
            printf "push branch? (y,n): "
            let line = System.Console.ReadLine()
            if line = "y" then
                Branches.push ""
)

Target "Release" (fun _ ->
    trace "All released!"
)

"Clean"
  ==> "CleanAll"
"Clean"
  ==> "RestorePackages"
  ==> "SetVersions"
"SetVersions"
  ==> "BuildApp_40"

"All"
  ==> "Clean"
  ==> "VersionBump"
  ==> "NuGet"
//  ==> "GithubDoc"
//  ==> "ReleaseGithubDoc"
  ==> "Release"

RunTargetOrDefault "All"