[CmdletBinding()]
param (
    [Parameter(Position = 0, Mandatory)]
    [string] $path,

    [Parameter(Position = 1)]
    [ValidateSet("Debug", "Release")]
    [string] $configuration = "Debug"
)

msbuild `
    /t:"build" `
    /nowarn:"MSB4011,VSX1000" `
    /p:NoPackageAnalysis=true `
    /p:ContinuousIntegrationBuild=true `
    /p:Configuration=$configuration `
    /verbosity:minimal `
    $path
