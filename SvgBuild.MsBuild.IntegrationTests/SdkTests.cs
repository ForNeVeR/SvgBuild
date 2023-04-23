using SvgBuild.MsBuild.IntegrationTests.TestFramework;
using Xunit.Abstractions;

namespace SvgBuild.MsBuild.IntegrationTests;

public class SdkTests : SdkTestBase
{
    public SdkTests(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public Task FrankenprojBuildsCorrectly() => AssertBuildSuccess(
        "Frankenproj/Frankenproj.csproj",
        "Frankenproj/bin/Release/Test.bmp",
        "Frankenproj/bin/Release/Test.ico");
}
