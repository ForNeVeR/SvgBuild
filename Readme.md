SvgBuild [![Appveyor Build][badge-appveyor]][build-appveyor] [![Travis Build][badge-travis]][build-travis]
========

This is a .NET based tool set to save SVG to raster images. Includes a
terminal-based program.

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

Run
---

On Windows:

```console
$ SvgBuild.Console <path to the input file> <path to the output file>
```

On other operating systems:

```console
$ mono SvgBuild.Console <path to the input file> <path to the output file>
```

Test
----

On Windows:

```console
$ packages\xunit.runner.console.2.3.1\tools\net452\xunit.console.exe SvgBuild.Tests\bin\Release\SvgBuild.Tests.dll
```

On other operating systems:

```console
$ mono packages/xunit.runner.console.2.3.1/tools/net452/xunit.console.exe SvgBuild.Tests/bin/Release/SvgBuild.Tests.dll
```

[build-appveyor]: https://ci.appveyor.com/project/ForNeVeR/svgbuild/branch/master
[build-travis]: https://travis-ci.org/ForNeVeR/SvgBuild
[mono]: http://www.mono-project.com/
[msbuild]: https://github.com/Microsoft/msbuild
[visual-studio]: https://www.visualstudio.com/

[badge-appveyor]: https://ci.appveyor.com/api/projects/status/mwpd81tb2nwku1k6/branch/master?svg=true
[badge-travis]: https://travis-ci.org/ForNeVeR/SvgBuild.svg?branch=master
