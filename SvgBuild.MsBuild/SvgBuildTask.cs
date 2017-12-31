using System;
using System.Drawing;
using System.Globalization;

namespace SvgBuild.MsBuild
{
    public class SvgBuildTask : Microsoft.Build.Utilities.Task
    {
        public string InputPath { get; set; }
        public string OutputPath { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }

        public override bool Execute()
        {
            if (string.IsNullOrWhiteSpace(Width) != string.IsNullOrWhiteSpace(Height))
            {
                throw new ArgumentException("Width and Height should both be defined");
            }

            var size = ParseSize();

            Renderer.Render(InputPath, OutputPath, size);
            return true;
        }

        private Size? ParseSize()
        {
            if (string.IsNullOrWhiteSpace(Width) || string.IsNullOrWhiteSpace(Height))
            {
                return null;
            }

            var width = int.Parse(Width.Trim(), CultureInfo.InvariantCulture);
            var height = int.Parse(Height.Trim(), CultureInfo.InvariantCulture);
            return new Size(width, height);
        }
    }
}
