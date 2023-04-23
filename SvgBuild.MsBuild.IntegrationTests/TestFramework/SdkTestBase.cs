using System.Reflection;
using System.Runtime.InteropServices;
using Medallion.Shell;
using Xunit.Abstractions;

namespace SvgBuild.MsBuild.IntegrationTests.TestFramework;

public abstract class SdkTestBase
{
    private readonly ITestOutputHelper _output;
    private readonly string _sourceRoot;
    private readonly string _temporaryPath = Path.GetTempFileName();

    protected SdkTestBase(ITestOutputHelper output)
    {
        _output = output;

        File.Delete(_temporaryPath);

        var assemblyPath = Assembly.GetExecutingAssembly().Location;
        var assemblyDirectory = Path.GetDirectoryName(assemblyPath)!;
        var testDataPath = Path.Combine(assemblyDirectory, "TestData");
        CopyDirectoryRecursive(testDataPath, _temporaryPath);

        _sourceRoot = FindSourcePath(assemblyDirectory, "SvgBuild.sln");
    }

    private static string FindSourcePath(string path, string fileName)
    {
        while (true)
        {
            var filePath = Path.Combine(path, fileName);
            if (File.Exists(filePath)) return path;
            path = Path.GetDirectoryName(path)!;
            if (path == null) throw new Exception($"Could not find \"{fileName}\" in any parent directory.");
        }
    }

    private static void CopyDirectoryRecursive(string source, string target)
    {
        Directory.CreateDirectory(target);

        foreach (var subDirPath in Directory.GetDirectories(source))
        {
            var dirName = Path.GetFileName(subDirPath);
            CopyDirectoryRecursive(subDirPath, Path.Combine(target, dirName));
        }

        foreach (var filePath in Directory.GetFiles(source))
        {
            var fileName = Path.GetFileName(filePath);
            File.Copy(filePath, Path.Combine(target, fileName));
        }
    }

    private string GetTestDataPath(string relativePath) => Path.Combine(_temporaryPath, relativePath);

    private async Task EnsureCommandSuccess(string executable, string? workingDirectory, string[] arguments, Dictionary<string, string>? environment = null)
    {
        var commandText = $"{executable} {string.Join(" ", arguments)}";
        _output.WriteLine($"# Executing command \"{commandText}\"");

        var command = Command.Run(executable, arguments, options =>
        {
            if (workingDirectory != null)
                options.WorkingDirectory(workingDirectory);
            if (environment != null)
                options.EnvironmentVariables(environment);
        });
        var result = await command.Task;
        if (!result.Success)
            throw new Exception(
                $"Command \"{commandText}\" failed.\n" +
                $"StdOut: {result.StandardOutput}\nStdErr: {result.StandardError}");

        if (!string.IsNullOrWhiteSpace(result.StandardOutput))
            _output.WriteLine("## Output\n" + result.StandardOutput);
        if (!string.IsNullOrWhiteSpace(result.StandardError))
            _output.WriteLine("## Errors\n" + result.StandardError);
        _output.WriteLine($"## Exit code: {result.ExitCode}\n");
    }

    private async Task<string> PackMsBuildPackage()
    {
        await EnsureCommandSuccess("dotnet", _sourceRoot, new[]
        {
            "build",
            "-c", "Release"
        });
        await EnsureCommandSuccess("nuget", _temporaryPath, new[]
        {
            "pack",
            Path.Combine(_sourceRoot, "SvgBuild.MsBuild/SvgBuild.MsBuild.csproj"),
            "-Tool",
            "-Prop", "Platform=AnyCPU",
            "-Prop", "Configuration=Release"
        }, new Dictionary<string, string>
        {
            ["NUGET_ENABLE_LEGACY_CSPROJ_PACK"] = "true"
        });
        return Path.Combine(_temporaryPath, "SvgBuild.MsBuild.1.0.0.nupkg");
    }

    private static string CreateTempRepository()
    {
        var path = Path.GetTempFileName();
        File.Delete(path);
        Directory.CreateDirectory(path);
        return path;
    }

    private static string FindMsBuildExecutable()
    {
        const string executableName = "msbuild";
        var pathsToSearch = Environment.GetEnvironmentVariable("PATH")!.Split(Path.PathSeparator);
        var extensions = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? Environment.GetEnvironmentVariable("PATHEXT")!.Split(Path.PathSeparator)
            : null;
        foreach (var directory in pathsToSearch)
        {
            foreach (var extension in extensions ?? Array.Empty<string>())
            {
                var path = Path.Combine(directory, executableName + extension);
                if (File.Exists(path)) return path;
            }
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            var msBuildPath = Path.Combine(
                programFiles,
                "Microsoft Visual Studio",
                "2022",
                "Professional",
                "Msbuild",
                "Current",
                "Bin",
                "amd64",
                "MSBuild.exe");
            if (File.Exists(msBuildPath)) return msBuildPath;
        }

        throw new Exception("Cannot find msbuild executable.");
    }

    private async Task BuildProject(string projectFilePath)
    {
        var projectDirectory = Path.GetDirectoryName(projectFilePath)!;
        await EnsureCommandSuccess(
            FindMsBuildExecutable(),
            projectDirectory,
            new[] { "/p:Platform=AnyCPU", "/p:Configuration=Release" });
    }

    private async Task AddPackageToRepository(string package, string repository)
    {
        await EnsureCommandSuccess("nuget", null, new[]
        {
            "add",
            package,
            "-Source", repository
        });
    }

    private Task RestorePackages(string projectFilePath, string repository)
    {
        var projectDirectory = Path.GetDirectoryName(projectFilePath)!;
        return EnsureCommandSuccess(
            "nuget",
            projectDirectory,
            new[]
            {
                "restore", projectFilePath,
                "-Source", repository,
                "-PackagesDirectory", "packages"
            });
    }

    protected async Task AssertBuildSuccess(string projectRelativePath, params string[] outputFileRelativePaths)
    {
        var projectFilePath = GetTestDataPath(projectRelativePath);

        var nupkg = await PackMsBuildPackage();
        var repository = CreateTempRepository();
        await AddPackageToRepository(nupkg, repository);
        await RestorePackages(projectFilePath, repository);

        foreach (var outputFileRelativePath in outputFileRelativePaths)
        {
            var outputFilePath = GetTestDataPath(outputFileRelativePath);
            if (File.Exists(outputFilePath)) throw new Exception($"File \"{outputFilePath}\" should not exist.");
        }

        await BuildProject(projectFilePath);

        foreach (var outputFileRelativePath in outputFileRelativePaths)
        {
            var outputFilePath = GetTestDataPath(outputFileRelativePath);
            Assert.True(File.Exists(outputFilePath), $"File \"{outputFilePath}\" should exist.");
        }
    }
}
