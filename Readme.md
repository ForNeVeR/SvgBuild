SvgBuild [![NuGet][badge-nuget]][nuget]
========

This is a .NET based tool set to save SVG to raster images. Includes a
terminal-based program and a MSBuild task. It is compatible with .NET Framework
4.6.1 and Mono.

Build
-----

This program uses [MSBuild][msbuild] (included in [Visual
Studio][visual-studio] distribution on Windows or [Mono][mono] on other
operation systems). To build the program, run the following script:

```console
$ nuget restore
$ msbuild /p:Configuration=Release SvgBuild.sln
```

It will create the `SvgBuild.Console/bin/Release/SvgBuild.Console.exe` binary
file.

Usage
-----

SvgBuild basically takes two required parameters: input and output path. Input
path should lead to a valid SVG file, output path should lead to a valid place
to save the resulting image. Output image format will be determined by the
output file extension. The supported file extensions are:

- `.bmp`
- `.gif`
- `.ico`
- `.jpeg`, `.jpg`
- `.png`
- `.tiff`

The image will be rescaled to the new size, if specified. Otherwise, the size
of the original document will be preserved.

### From Terminal

On Windows:

```console
$ SvgBuild.Console <path to the input file> <path to the output file> [<width>x<height>]
```

On other operating systems:

```console
$ mono SvgBuild.Console.exe <path to the input file> <path to the output file> [<width>x<height>]
```

### From MSBuild

Install `SvgBuild.MsBuild` package into your project. Your NuGet client should
automatically generate the following in your project file:

```xml
<Import Project="..\packages\SvgBuild.MsBuild.1.0.0\build\SvgBuild.MsBuild.props"
        Condition="Exists('..\packages\SvgBuild.MsBuild.1.0.0\build\SvgBuild.MsBuild.props')" />
```

After that, you're able to run SVG processing tasks e.g. in the `AfterBuild`
target:

```xml
<Target Name="SvgBuildTasks" AfterTargets="AfterBuild">
  <SvgBuildTask InputPath="$(ProjectDir)..\SvgBuild.Tests\Resources\Image.svg"
                OutputPath="$(OutDir)Test.bmp"
                Width="30"
                Height="60" /> <!-- Width and Height are optional -->
</Target>
```

Unit Tests
----------

On Windows:

```console
$ dotnet test
```

On other operating systems:

```console
$ mono ~/.nuget/xunit.runner.console.2.3.1/tools/net452/xunit.console.exe SvgBuild.Tests/bin/Release/SvgBuild.Tests.dll
```

Integration Tests
-----------------

```console
$ pwsh ./SvgBuild.Tests.Integration/Run-IntegrationTests.ps1
```

Pack
----

To pack the project before uploading it to NuGet, use the following commands:

```console
$ nuget restore
$ msbuild /p:Configuration=Release SvgBuild.sln
$ nuget pack SvgBuild.MsBuild/SvgBuild.MsBuild.csproj -Tool -Prop Platform=AnyCPU -Prop Configuration=Release
```

[mono]: http://www.mono-project.com/
[msbuild]: https://github.com/Microsoft/msbuild
[nuget]: https://www.nuget.org/packages/SvgBuild.MsBuild
[visual-studio]: https://www.visualstudio.com/

[badge-nuget]: https://img.shields.io/nuget/v/SvgBuild.MsBuild.svg
