using System;
using System.IO;
using System.Reflection;

namespace NppMarkdownPanel.Generator
{
    internal static class MathJaxAssets
    {
        private static readonly Lazy<string> script = new Lazy<string>(LoadScript);

        public static string Script => script.Value;

        private static string LoadScript()
        {
            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var scriptPath = Path.Combine(assemblyDirectory, "mathjax", "tex-svg-full.js");

            if (!File.Exists(scriptPath))
                return "console.error('NppMarkdownPanel: local MathJax bundle is missing.');";

            return File.ReadAllText(scriptPath);
        }
    }
}
