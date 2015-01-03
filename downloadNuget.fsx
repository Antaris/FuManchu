// ----------------------------------------------------------------------------
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// ----------------------------------------------------------------------------
// Get any working NuGet.exe
// It's sad that this does'nt work on mono as the don't have a "System.IO.Compression.FileSystem.dll"

open System.Net
open System.IO

//let nugetCommandLinePackage = "https://www.nuget.org/api/v2/package/NuGet.Commandline"
//let nugetPkg = "lib/NuGet.CommandLine/NuGet.CommandLine.nupkg"
let nugetLink = "https://www.nuget.org/nuget.exe"
let nugetExe = ".nuget/Build/NuGet.CommandLine/tools/NuGet.exe"

let nugetToolsPath = Path.GetDirectoryName(nugetExe)
//let nugetPath = Path.GetDirectoryName(nugetToolsPath)

let downloadFile (link:string) (filePath:string) =
    let wc = new WebClient()
    wc.DownloadFile(link, filePath)

Directory.CreateDirectory(nugetToolsPath) |> ignore

downloadFile nugetLink nugetExe
//ZipFile.ExtractToDirectory(nugetPkg, nugetPath)