[CmdletBinding()]
param (
    [Parameter(Position = 0)]
    [ValidateSet("build", "test", "pack")]
    [string] $command = $null,

    [Parameter(Position = 1)]
    [ValidateSet("Debug", "Release")]
    [string] $configuration = "Release"
)

$env:PATH += ";C:/Program Files/Microsoft Visual Studio/2022/Enterprise/Msbuild/Current/Bin";

$build = @(
    "./src/Navigation",
    "./tests/Navigation.UnitTests"
)

$test = @(
    "./tests/Navigation.UnitTests"
)

$pack = @(
    "./src/Navigation"
)

function build () {
    foreach ($path in $build) {
        msbuild `
            /t:"restore,build" `
            /nowarn:"MSB4011,VSX1000" `
            /p:NoPackageAnalysis=true `
            /p:ContinuousIntegrationBuild=true `
            /p:Configuration=$configuration `
            /verbosity:minimal `
            $path
    }
}

function test () {
    foreach ($path in $test) {
        dotnet `
            test $path `
            -c $configuration `
            --no-restore `
            --no-build
    }
}

function pack () {
    $output = "./nuget"
    foreach ($path in $pack) {
        msbuild `
            /t:"pack" `
            /nowarn:"MSB4011,VSX1000" `
            /p:NoPackageAnalysis=true `
            /p:ContinuousIntegrationBuild=true `
            /p:Configuration=Release `
            /p:PackageOutputPath=$output `
            /verbosity:minimal `
            $path
    }
}

switch ($command.ToString()) {
    "build" { build; break }
    "test" { test; break }
    "pack" { pack; break }
    Default {
        build;
        test;
        pack;
    }
}
