cd ./Navigation

dotnet restore

msbuild /t:build,pack /nowarn:MSB4011,VSX1000 /p:NoPackageAnalysis=true /verbosity:minimal