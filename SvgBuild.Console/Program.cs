namespace SvgBuild.Console
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var input = args[0];
            var output = args[1];
            Renderer.Render(input, output);
        }
    }
}
