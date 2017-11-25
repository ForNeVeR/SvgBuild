SvgBuild
========

This is a .NET based tool set to save SVG to raster images. Includes a
terminal-based program.

Build
-----

This program requires [.NET Core SDK 2.0][net-sdk] to build it.

```console
$ dotnet build
```

On Linux, remember that there's [a bug](dotnet-sdk-335) building projects
targeting .NET Framework using .NET Core SDK, so remember to set the
`FrameworkPathOverride` environment variable properly, e.g.

```console
$ export FrameworkPathOverride=/usr/lib/mono/4.6.1-api
```

After that, build the project as usual.

Run
---

```console
$ dotnet run --project SvgBuild.Console [path to input file] [path to output file]
```

[dotnet-sdk-335]: https://github.com/dotnet/sdk/issues/335
[framework-path-override]: https://github.com/dotnet/sdk/issues/335#issuecomment-322137207
[net-sdk]: https://www.microsoft.com/net/download
