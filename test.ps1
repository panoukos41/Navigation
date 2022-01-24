[CmdletBinding()]
param (
    [Parameter(Position = 0, Mandatory)]
    [string] $path,

    [Parameter(Position = 1)]
    [ValidateSet("Debug", "Release")]
    [string] $configuration = "Debug"
)

dotnet `
    test `
    -c $configuration `
    --no-restore `
    --no-build `
    $path
