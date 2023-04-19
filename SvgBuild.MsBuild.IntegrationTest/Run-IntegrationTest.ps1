param (
    $SourceRoot = "$PSScriptRoot/.."
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# Pack the package:
nuget pack $SourceRoot/SvgBuild.MsBuild/SvgBuild.MsBuild.csproj -Tool -Prop Platform=AnyCPU -Prop Configuration=Release
if (!$?) { throw "Error from nuget pack: $LASTEXITCODE." }

# Install the package:
Push-Location $SourceRoot/SvgBuild.MsBuild.IntegrationTest
try
{
    New-Item -Type Directory NuGetRepository
    nuget add ../SvgBuild.MsBuild.1.0.0.nupkg -Source $(Resolve-Path NuGetRepository)
    if (!$?) { throw "Error from nuget add: $LASTEXITCODE." }

    nuget restore -Source $(Resolve-Path NuGetRepository) -PackagesDirectory packages
    if (!$?) { throw "Error from nuget restore: $LASTEXITCODE." }
    # Check if the build works now:
    msbuild /p:Platform=AnyCPU /p:Configuration=Release
    if (!$?) { throw "Error from msbuild: $LASTEXITCODE." }

    if (!(Test-Path -Type Leaf bin/Release/Test.bmp)) { throw "File not found: bin\Release\Test.bmp" }
    if (!(Test-Path -Type Leaf bin/Release/Test.ico)) { throw "File not found: bin\Release\Test.ico" }
}
finally
{
    Pop-Location
}
