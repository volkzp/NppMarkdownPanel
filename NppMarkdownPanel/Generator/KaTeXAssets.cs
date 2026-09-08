using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace NppMarkdownPanel.Generator
{
    internal sealed class KaTeXAssets
    {
        private static readonly Lazy<KaTeXAssets> instance =
            new Lazy<KaTeXAssets>(LoadFromPluginDirectory);

        private KaTeXAssets(string css, string katexScript, string autoRenderScript)
        {
            Css = css;
            KatexScript = katexScript;
            AutoRenderScript = autoRenderScript;
        }

        internal string Css { get; }
        internal string KatexScript { get; }
        internal string AutoRenderScript { get; }

        internal static KaTeXAssets Current => instance.Value;

        private static KaTeXAssets LoadFromPluginDirectory()
        {
            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var katexDirectory = Path.Combine(assemblyDirectory, "katex");

            try
            {
                var css = File.ReadAllText(Path.Combine(katexDirectory, "katex.min.css"));
                css = EmbedWoff2Fonts(css, katexDirectory);
                css += ".katex-display{overflow-x:auto;overflow-y:hidden;padding:.2em 0}";

                return new KaTeXAssets(
                    css,
                    ReadScript(Path.Combine(katexDirectory, "katex.min.js")),
                    ReadScript(Path.Combine(katexDirectory, "contrib", "auto-render.min.js")));
            }
            catch (Exception exception)
            {
                var message = "console.error(" + ToJavaScriptString("NppMarkdownPanel: local KaTeX bundle is missing: " + exception.Message) + ");";
                return new KaTeXAssets(String.Empty, message, String.Empty);
            }
        }

        private static string EmbedWoff2Fonts(string css, string katexDirectory)
        {
            return Regex.Replace(css, "url\\((?:['\\\"])?fonts/(?<name>[^)'\\\"]+\\.woff2)(?:['\\\"])?\\)", match =>
            {
                var fontPath = Path.Combine(katexDirectory, "fonts", match.Groups["name"].Value);
                var base64 = Convert.ToBase64String(File.ReadAllBytes(fontPath));
                return "url(data:font/woff2;base64," + base64 + ")";
            });
        }

        private static string ReadScript(string path)
        {
            return File.ReadAllText(path).Replace("</script", "<\\/script");
        }

        private static string ToJavaScriptString(string value)
        {
            return "'" + value.Replace("\\", "\\\\").Replace("'", "\\'").Replace("\r", " ").Replace("\n", " ") + "'";
        }
    }
}
