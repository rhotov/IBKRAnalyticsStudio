param(
  [ValidateSet("Debug", "Release")]
  [string]$Configuration = "Release",

  [string]$Runtime = "win-x64",

  [switch]$SelfContained,

  [switch]$SingleFile
)

$ErrorActionPreference = "Stop"

$effectiveSelfContained = $SelfContained.IsPresent -or $SingleFile.IsPresent

$publishDir = if ($SingleFile) {
  "publish\$Runtime-portable-clean"
} elseif ($effectiveSelfContained) {
  "publish\$Runtime-self-contained"
} else {
  "publish\$Runtime-framework-dependent"
}

$publishArgs = @(
  "publish",
  "-c",
  $Configuration,
  "-r",
  $Runtime,
  "--self-contained:$($effectiveSelfContained.ToString().ToLowerInvariant())",
  "-o",
  $publishDir
)

if ($SingleFile) {
  $publishArgs += @(
    "-p:PublishSingleFile=true",
    "-p:IncludeNativeLibrariesForSelfExtract=true",
    "-p:EnableCompressionInSingleFile=true",
    "-p:DebugType=None",
    "-p:DebugSymbols=false"
  )
}

dotnet @publishArgs

if ($SingleFile) {
  Get-ChildItem -Path $publishDir -File | Where-Object { $_.Extension -in ".xml", ".pdb" } | Remove-Item -Force
}

Write-Host "Published to $publishDir"
