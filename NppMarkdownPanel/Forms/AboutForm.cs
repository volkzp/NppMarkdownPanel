using System;
using System.Reflection;
using System.Windows.Forms;

namespace NppMarkdownPanel.Forms
{
    public partial class AboutForm : Form
    {

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
            Text = PluginLocalization.Text("about.title");
            label1.Text = PluginLocalization.Text("about.header");
            btnOk.Text = PluginLocalization.Text("about.ok");
            tbAbout.Text = PluginLocalization.Format("about.body", versionString);
        }

        private void AboutForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            PluginLocalization.LanguageChanged -= ApplyLocalization;
        }

    }
}
