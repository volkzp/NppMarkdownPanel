using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NppMarkdownPanel.Forms
{
    public partial class AboutForm : Form
    {

        private const string AboutDialogText =
                "NppMarkdownPanel for Notepad++\r\n\r\nVersion {0}\r\n\r\nCreated by Mohzy 2019-2026\r\n\r\nGithub: https://github.com/mohzy83/NppMarkdownPanel\r\n\r\nUsed Libs and Resources:\r\n\r\n" +
                "Markdig 1.3.2 by xoofx - https://github.com/xoofx/markdig\r\n\r\n" +
                "NotepadPlusPlusPluginPack.Net 0.95.00 by kbilsted - https://github.com/kbilsted/NotepadPlusPlusPluginPack.Net\t\r\n\r\n" +
                "WebView2 Edge 1.0.3650.58 by Microsoft - https://developer.microsoft.com/de-de/microsoft-edge/webview2?form=MA13LH\r\n\r\n" +
                "ColorCode (Portable) 1.0.3 by Bashir Souid and  Richard Slater\r\nhttps://github.com/RichardSlater/ColorCodePortable\r\n\r\n" +
                "Markdig.SyntaxHighlighting 1.1.7  - Syntax Highlighting extension for Markdig by Richard Slater\r\nhttps://github.com/RichardSlater/Markdig.SyntaxHighlighting\r\n\r\n" +
                "github-markdown-css 3.0.1 by sindresorhus - \r\nhttps://github.com/sindresorhus/github-markdown-css\r\n\r\n" +
                "Markdown icon by dcurtis - https://github.com/dcurtis/markdown-mark\r\n\r\n" +
                "markdown-it-github-alerts 1.0.0 by antfu - \r\nhttps://github.com/antfu/markdown-it-github-alerts\r\n\r\n" +
                "ClipboardHelper (HTML Renderer) 1.5.2 by Arthur Teplitzki - \r\nhttps://github.com/ArthurHub/HTML-Renderer\r\n\r\n" +
                "HtmlSanitizer 9.2.1039 by Michael Ganss - \r\nhttps://github.com/mganss/htmlsanitizer\r\n\r\n" +
                "AngleSharp 1.7.2 by AngleSharp - \r\nhttps://github.com/anglesharp/anglesharp\r\n\r\n" +
                "MathJax 3.2.2 by The MathJax Consortium - https://www.mathjax.org\r\n\r\n" +
                "The plugin uses portions of nea's MarkdownViewerPlusPlus Plugin code - https://github.com/nea/MarkdownViewerPlusPlus";

        private const string AboutDialogTextRussian =
                "NppMarkdownPanel для Notepad++\r\n\r\nВерсия {0}\r\n\r\nСоздан Mohzy и участниками проекта в 2019–2026 годах\r\n\r\nGitHub: https://github.com/mohzy83/NppMarkdownPanel\r\n\r\nИспользованные библиотеки и ресурсы:\r\n\r\n" +
                "Markdig 1.3.2 by xoofx - https://github.com/xoofx/markdig\r\n\r\n" +
                "NotepadPlusPlusPluginPack.Net 0.95.00 by kbilsted - https://github.com/kbilsted/NotepadPlusPlusPluginPack.Net\r\n\r\n" +
                "WebView2 Edge 1.0.3650.58 by Microsoft - https://developer.microsoft.com/microsoft-edge/webview2\r\n\r\n" +
                "MathJax 3.2.2 by The MathJax Consortium - https://www.mathjax.org\r\n\r\n" +
                "Полный перечень компонентов приведён в файле THIRD-PARTY-NOTICES.md.";

        private string versionString;

        public AboutForm()
        {
            InitializeComponent();
            versionString = "0.X";
            try
            {
                Version version = Assembly.GetExecutingAssembly().GetName().Version;
                versionString = version.ToString();
            }
            catch (Exception) { }
            PluginLocalization.LanguageChanged += ApplyLocalization;
            FormClosed += AboutForm_FormClosed;
            ApplyLocalization();
            btnOk.Focus();
            this.ActiveControl = btnOk;
        }

        private void ApplyLocalization()
        {
            Text = PluginLocalization.Text("About", "О программе");
            label1.Text = PluginLocalization.Text("NppMarkdownPanel - About", "NppMarkdownPanel — О программе");
            btnOk.Text = PluginLocalization.Text("OK", "ОК");
            tbAbout.Text = string.Format(
                PluginLocalization.Text(AboutDialogText, AboutDialogTextRussian),
                versionString);
        }

        private void AboutForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            PluginLocalization.LanguageChanged -= ApplyLocalization;
        }

    }
}
