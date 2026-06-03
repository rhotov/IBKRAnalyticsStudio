param(
  [ValidateSet("Debug", "Release")]
  [string]$Configuration = "Release",

  [string]$Runtime = "win-x64",

  [switch]$SelfContained
)

$ErrorActionPreference = "Stop"

$publishDir = if ($SelfContained) {
  "publish\$Runtime-self-contained"
} else {
  "publish\$Runtime-framework-dependent"
}

dotnet publish `
  -c $Configuration `
  -r $Runtime `
  --self-contained:$($SelfContained.IsPresent.ToString().ToLowerInvariant()) `
  -o $publishDir

Write-Host "Published to $publishDir"
