using SvgBuild.MsBuild.IntegrationTests.TestFramework;
using Xunit.Abstractions;

namespace SvgBuild.MsBuild.IntegrationTests;

public class SdkTests : SdkTestBase
{
    public SdkTests(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public Task ProjectBuildsCorrectly() => AssertBuildSuccess(
        "Project/Project.csproj",
        "Project/bin/Release/Test.bmp",
        "Project/bin/Release/Test.ico");
}
