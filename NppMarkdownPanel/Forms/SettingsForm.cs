using NppMarkdownPanel.Entities;
using System;
using System.Windows.Forms;

namespace NppMarkdownPanel.Forms
{
    public partial class SettingsForm : Form
    {
        public int ZoomLevel { get; set; }
        public string CssFileName { get; set; }
        public string CssDarkModeFileName { get; set; }
        public string HtmlFileName { get; set; }
        public bool ShowToolbar { get; set; }
        public string SupportedFileExt { get; set; }
        public bool AllowAllExtensions { get; set; }
        public bool SupportFilesWithNoExt { get; set; }
        public bool AutoShowPanel { get; set; }
        public bool ShowStatusbar { get; set; }
        public bool EnableThreeStateToggle { get; set; }
        public string RenderingEngine { get; set; }


        public SettingsForm(Settings settings)
        {
            ZoomLevel = settings.ZoomLevel;
            CssFileName = settings.CssFileName;
            CssDarkModeFileName = settings.CssDarkModeFileName;
            HtmlFileName = settings.HtmlFileName;
            ShowToolbar = settings.ShowToolbar;
            SupportedFileExt = settings.SupportedFileExt;
            AutoShowPanel = settings.AutoShowPanel;
            ShowStatusbar = settings.ShowStatusbar;
            RenderingEngine = settings.RenderingEngine;
            AllowAllExtensions = settings.AllowAllExtensions;
            SupportFilesWithNoExt = settings.SupportFilesWithNoExt;
            EnableThreeStateToggle = settings.EnableThreeStateToggle;

            InitializeComponent();
            PluginLocalization.LanguageChanged += ApplyLocalization;
            FormClosed += SettingsForm_FormClosed;
            ApplyLocalization();

            trackBar1.Value = ZoomLevel;
            lblZoomValue.Text = $"{ZoomLevel}%";
            tbCssFile.Text = CssFileName;
            tbDarkmodeCssFile.Text = CssDarkModeFileName;
            tbHtmlFile.Text = HtmlFileName;
            cbShowToolbar.Checked = ShowToolbar;
            tbFileExt.Text = SupportedFileExt;
            cbAutoShowPanel.Checked = AutoShowPanel;
            cbShowStatusbar.Checked = ShowStatusbar;
            cbAllowAllExtensions.Checked = AllowAllExtensions;
            cbFilesWithNoExt.Checked = SupportFilesWithNoExt;
            cbEnableThreeStateToggle.Checked = EnableThreeStateToggle;

            if (settings.IsRenderingEngineIE11())
            {
                comboRenderingEngine.SelectedIndex = 1;
            }
            else if (settings.IsRenderingEngineEdge())
            {
                comboRenderingEngine.SelectedIndex = 0;
            }
        }

        private void ApplyLocalization()
        {
            Text = PluginLocalization.Text("Settings", "Настройки");
            label1.Text = PluginLocalization.Text("Markdown Panel Settings", "Настройки панели Markdown");
            btnSave.Text = PluginLocalization.Text("Save", "Сохранить");
            btnCancel.Text = PluginLocalization.Text("Cancel", "Отмена");
            label2.Text = PluginLocalization.Text("CSS File:", "Файл CSS:");
            label3.Text = PluginLocalization.Text("Zoom Level:", "Масштаб:");
            label4.Text = PluginLocalization.Text("Darkmode CSS File:", "CSS тёмной темы:");
            label5.Text = PluginLocalization.Text("Supported File Extensions:", "Расширения файлов:");
            label6.Text = PluginLocalization.Text("HTML Rendering Engine:", "Движок HTML:");
            lblHtmlFile.Text = PluginLocalization.Text(
                "Automatically Save\r\nHTML from Current\r\nPreview to this File:",
                "Автоматически сохранять\r\nHTML предпросмотра\r\nв этот файл:");
            btnDefaultCss.Text = PluginLocalization.Text("Default", "Сброс");
            btnDefaultDarkmodeCss.Text = PluginLocalization.Text("Default", "Сброс");
            btnResetHtml.Text = PluginLocalization.Text("Default", "Сброс");
            btnDefaultFileExt.Text = PluginLocalization.Text("Default", "Сброс");
            cbAllowAllExtensions.Text = PluginLocalization.Text("Allow all file extensions", "Разрешить все расширения файлов");
            cbFilesWithNoExt.Text = PluginLocalization.Text("Enable preview for files without extension", "Включить просмотр файлов без расширения");
            cbAutoShowPanel.Text = PluginLocalization.Text("Automatically show panel for supported files", "Автоматически открывать панель для поддерживаемых файлов");
            cbShowToolbar.Text = PluginLocalization.Text("Show Toolbar in Preview Window", "Показывать панель инструментов в окне просмотра");
            cbShowStatusbar.Text = PluginLocalization.Text("Show Statusbar in Preview Window (Preview Links)", "Показывать строку состояния (просмотр ссылок)");
            cbEnableThreeStateToggle.Text = PluginLocalization.Text(
                "Enable three-state toggle (docked → fullscreen → hidden)",
                "Три состояния панели (закреплена → весь экран → скрыта)");
        }

        private void SettingsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            PluginLocalization.LanguageChanged -= ApplyLocalization;
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            ZoomLevel = trackBar1.Value;
            lblZoomValue.Text = $"{ZoomLevel}%";
        }

        private void tbCssFile_TextChanged(object sender, EventArgs e)
        {
            CssFileName = tbCssFile.Text;
        }
        private void tbDarkmodeCssFile_TextChanged(object sender, EventArgs e)
        {
            CssDarkModeFileName = tbDarkmodeCssFile.Text;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(tbHtmlFile.Text) && String.IsNullOrEmpty(sblInvalidHtmlPath.Text))
            {
                bool validHtmlPath = Utils.ValidateFileSelection(tbHtmlFile.Text, out string validPath, out string error, "HTML Output");
                if (!validHtmlPath)
                    sblInvalidHtmlPath.Text = error;
                else
                    tbHtmlFile.Text = validPath;
            }

            if (String.IsNullOrEmpty(sblInvalidHtmlPath.Text))
            {
                this.DialogResult = DialogResult.OK;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnChooseCss_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = PluginLocalization.Text(
                    "CSS files (*.css)|*.css|All files (*.*)|*.*",
                    "Файлы CSS (*.css)|*.css|Все файлы (*.*)|*.*");
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if ((sender as Button).Name == "btnChooseCss")
                    {
                        CssFileName = openFileDialog.FileName;
                        tbCssFile.Text = CssFileName;
                    }
                    else if ((sender as Button).Name == "btnChooseDarkmodeCss")
                    {
                        CssDarkModeFileName = openFileDialog.FileName;
                        tbDarkmodeCssFile.Text = CssDarkModeFileName;
                    }

                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tbCssFile.Text = "";
        }
        private void btnDefaultDarkmodeCss_Click(object sender, EventArgs e)
        {
            tbDarkmodeCssFile.Text = "";
        }

        #region Output HTML File
        private void tbHtmlFile_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(tbHtmlFile.Text))
            {
                bool valid = Utils.ValidateFileSelection(tbHtmlFile.Text, out string validPath, out string error, "HTML Output");
                if (valid)
                {
                    HtmlFileName = validPath;
                    if (!String.IsNullOrEmpty(sblInvalidHtmlPath.Text))
                        sblInvalidHtmlPath.Text = String.Empty;
                }
                else
                {
                    sblInvalidHtmlPath.Text = error;
                }
            }
            else
            {
                HtmlFileName = String.Empty;
                if (!String.IsNullOrEmpty(sblInvalidHtmlPath.Text))
                    sblInvalidHtmlPath.Text = String.Empty;
            }
        }

        private void tbHtmlFile_Leave(object sender, EventArgs e)
        {
            tbHtmlFile.Text = HtmlFileName;
        }

        private void btnChooseHtml_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = PluginLocalization.Text(
                    "HTML files (*.html, *.htm)|*.html;*.htm|All files (*.*)|*.*",
                    "Файлы HTML (*.html, *.htm)|*.html;*.htm|Все файлы (*.*)|*.*");
                saveFileDialog.RestoreDirectory = true;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    HtmlFileName = saveFileDialog.FileName;
                    tbHtmlFile.Text = HtmlFileName;
                }
            }
        }

        private void btnResetHtml_Click(object sender, EventArgs e)
        {
            tbHtmlFile.Text = "";
        }
        #endregion

        #region Show Toolbar
        private void cbShowToolbar_Changed(object sender, EventArgs e)
        {
            ShowToolbar = cbShowToolbar.Checked;
        }

        #endregion

        private void btnDefaultFileExt_Click(object sender, EventArgs e)
        {
            tbFileExt.Text = Settings.DEFAULT_SUPPORTED_FILE_EXT;
        }

        private void tbFileExt_TextChanged(object sender, EventArgs e)
        {
            SupportedFileExt = tbFileExt.Text;
        }

        private void cbAutoShowPanel_CheckedChanged(object sender, EventArgs e)
        {
            AutoShowPanel = cbAutoShowPanel.Checked;
        }

        private void cbShowStatusbar_CheckedChanged(object sender, EventArgs e)
        {
            ShowStatusbar = cbShowStatusbar.Checked;
        }

        private void comboRenderingEngine_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboRenderingEngine.SelectedIndex == 0)
            {
                RenderingEngine = Settings.RENDERING_ENGINE_WEBVIEW2_EDGE;
            }
            else if (comboRenderingEngine.SelectedIndex == 1)
            {
                RenderingEngine = Settings.RENDERING_ENGINE_WEBVIEW1_IE11;
            }
            else
            {
                throw new NotSupportedException("Rendering Engine with id " + comboRenderingEngine.SelectedIndex + " not supported!");
            }
        }

        private void cbAllowAllExtensions_CheckedChanged(object sender, EventArgs e)
        {
            AllowAllExtensions = cbAllowAllExtensions.Checked;
            if (AllowAllExtensions)
            {
                tbFileExt.Enabled = false;
                cbFilesWithNoExt.Enabled = false;   
            }
            else
            {
                tbFileExt.Enabled = true;
                cbFilesWithNoExt.Enabled = true;
            }
        }

        private void cbFilesWithNoExt_CheckedChanged(object sender, EventArgs e)
        {
            SupportFilesWithNoExt = cbFilesWithNoExt.Checked;
        }

        private void cbEnableThreeStateToggle_CheckedChanged(object sender, EventArgs e)
        {
            EnableThreeStateToggle = cbEnableThreeStateToggle.Checked;
        }
    }
}
