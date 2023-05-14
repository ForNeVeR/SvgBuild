SvgBuild [![NuGet][badge-nuget]][nuget]
========

This is a .NET based tool set to save SVG to raster images. Includes a
terminal-based program and a MSBuild task. Compatible with .NET 6 and later.

Build
-----

To build the project, run the following script:

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

The image will be rescaled to the new size, if specified. Otherwise, the size
of the original document will be preserved.

### From Shell

```console
$ SvgBuild.Console <path to the input file> <path to the output file> [<width>x<height>]
```

### From MSBuild

Install `SvgBuild.MsBuild` package into your project. After that, you're able to run SVG processing tasks e.g. in the `AfterBuild` target:

```xml
<Target Name="SvgBuildTasks" AfterTargets="AfterBuild">
  <SvgBuildTask InputPath="$(ProjectDir)..\SvgBuild.Tests\Resources\Image.svg"
                OutputPath="$(OutDir)Test.bmp"
                Width="30"
                Height="60" /> <!-- Width and Height are optional -->
</Target>
```

Run Tests
---------

```console
$ dotnet test
```

Pack
----

To pack the project before uploading it to NuGet, use the following commands:

```console
$ dotnet pack
```

[badge-nuget]: https://img.shields.io/nuget/v/SvgBuild.MsBuild.svg
