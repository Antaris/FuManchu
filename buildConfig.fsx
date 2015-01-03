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

open System.Collections.Generic
open System.IO

open Fake
open Fake.Git
open Fake.FSharpFormatting
open AssemblyInfoFile

// properties
let projectName = "FuManchu"
let projectSummary = "Handlebars templating for .NET."
let projectDescription = "FuManchu - Handlebars for .NET"
let authors = ["Matthew Abbott"]
let page_author = "Matthew Abbott"
let mail = "matthew.abbott@outlook.com"
let version = "1.0.0.0"
let version_nuget = "1.0.0.0"
let commitHash = Information.getCurrentSHA1(".")

let buildDir = "./build/"
let releaseDir = "./release/"
let outLibDir = "./release/lib/"
let outDocDir = "./release/documentation/"
let docTemplatesDir = "./doc/templates/"
let testDir  = "./test/"
let nugetDir  = "./.nuget/"
let packageDir  = "./.nuget/packages"

let github_user = "Antaris"
let github_project = "FuManchu"
let nuget_url = "https://www.nuget.org/packages/FuManchu/"

let tags = "C# FuManchu Handlebars templating"

let buildMode = "Debug" // if isMono then "Release" else "Debug"

let layoutRoots = [docTemplatesDir; docTemplatesDir @@ "reference" ]

if isMono then monoArguments <- "--runtime-v4.0 --debug"

let github_url = sprintf "http:/github.com/%s/%s" github_user github_project

let nuget = findToolInSubPath "NuGet.exe" "./.nuget/Build/NuGet.CommandLine/tools/NuGet.exe"
System.IO.File.Copy(nuget, "./src/.nuget/NuGet.exe", true)

let release = ReleaseNotesHelper.parseReleaseNotes (File.ReadLines "doc/ReleaseNotes.md")

let MyTarget name body =
  Target name body
  Target (sprintf "%s_single" name) body

type BuildParams =
  {
    CustomBuildName: string
  }

let buildApp (buildParams:BuildParams) =
  let buildDir = buildDir @@ buildParams.CustomBuildName
  CleanDirs [ buildDir ]
  let files = !! "src/FuManchu/FuManchu.csproj"

  files
    |> MSBuild buildDir "Build"
      [ "Configuration", buildMode
        "CustomBuildName", buildParams.CustomBuildName ]
    |> Log "AppBuild-Output: "

let net40Params = { CustomBuildName = "net40" }
let net45Params = { CustomBuildName = "net45" }