using System.Reflection;
using Medallion.Shell;
using Xunit.Abstractions;

namespace SvgBuild.MsBuild.IntegrationTests.TestFramework;

public abstract class SdkTestBase
{
    private readonly ITestOutputHelper _output;
    private readonly string _sourceRoot;
    private readonly string _temporaryPath = Path.GetTempFileName();
    private readonly string _nuGetPackages;

    protected SdkTestBase(ITestOutputHelper output)
    {
        _output = output;
        _nuGetPackages = Path.Combine(_temporaryPath, "NuGetPackages");

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

    private async Task EnsureCommandSuccess(string executable, string? workingDirectory, string[] arguments)
    {
        var commandText = $"{executable} {string.Join(" ", arguments)}";
        _output.WriteLine($"# Executing command \"{commandText}\"");

        var command = Command.Run(executable, arguments, options =>
        {
            if (workingDirectory != null)
                options.WorkingDirectory(workingDirectory);
            options.EnvironmentVariable("NUGET_PACKAGES", _nuGetPackages);
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
            "pack",
            "--version-suffix", "test",
            "--output", _temporaryPath
        });
        return Directory.EnumerateFiles(_temporaryPath, "*.nupkg").Single();
    }

    private static string CreateTempRepository()
    {
        var path = Path.GetTempFileName();
        File.Delete(path);
        Directory.CreateDirectory(path);
        return path;
    }

    private async Task BuildProject(string projectFilePath)
    {
        var projectDirectory = Path.GetDirectoryName(projectFilePath)!;
        await EnsureCommandSuccess(
            "dotnet",
            projectDirectory,
            new[] { "build", "--no-restore", "-p:Configuration=Release" });
    }

    private async Task PushPackageToRepository(string nupkg, string repository)
    {
        await EnsureCommandSuccess("dotnet", null, new[]
        {
            "nuget", "push",
            nupkg,
            "--source", repository
        });
    }

    private async Task AddPackageToProject(string projectFilePath, string repository)
    {
        await EnsureCommandSuccess("dotnet", null, new[]
        {
            "add",
            projectFilePath,
            "package",
            "SvgBuild.MsBuild",
            "--source", repository,
            "--prerelease"
        });
    }

    protected async Task AssertBuildSuccess(string projectRelativePath, params string[] outputFileRelativePaths)
    {
        var projectFilePath = GetTestDataPath(projectRelativePath);

        var nupkg = await PackMsBuildPackage();
        var repository = CreateTempRepository();
        await PushPackageToRepository(nupkg, repository);
        await AddPackageToProject(projectFilePath, repository);

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
