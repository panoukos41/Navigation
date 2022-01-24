[CmdletBinding()]
param (
    [Parameter(Position = 0, Mandatory)]
    [string] $path
)

msbuild `
    /t:"restore,build,pack" `
    /nowarn:"MSB4011,VSX1000" `
    /p:NoPackageAnalysis=true `
    /p:ContinuousIntegrationBuild=true `
    /p:Configuration=Release `
    /verbosity:minimal `
    $path
