using System.Windows.Forms;
using ColorManager.Properties;

namespace ColorManager.Forms
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            TopMost = Program.Config.TopMostWindow;

            lblAbout.Text = Resources.AboutText;
            lblDeveloper.Text = Resources.AboutDeveloper;
            lblVersion.Text = $"Version {Application.ProductVersion}";
        }
    }
}
