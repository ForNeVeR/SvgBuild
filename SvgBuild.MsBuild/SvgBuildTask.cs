namespace SvgBuild.MsBuild
{
    public class SvgBuildTask : Microsoft.Build.Utilities.Task
    {
        public string InputPath { get; set; }
        public string OutputPath { get; set; }

        public override bool Execute()
        {
            Renderer.Render(InputPath, OutputPath);
            return true;
        }
    }
}
