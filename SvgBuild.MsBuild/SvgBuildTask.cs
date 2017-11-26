using System;

namespace SvgBuild.MsBuild
{
    public class SvgBuildTask : Microsoft.Build.Utilities.Task
    {
        public string InputPath { get; set; }
        public string OutputPath { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }

        public override bool Execute()
        {
            if (Width.HasValue != Height.HasValue)
            {
                throw new ArgumentException("Width and Height should both be defined");
            }

            Renderer.Render(InputPath, OutputPath);
            return true;
        }
    }
}
