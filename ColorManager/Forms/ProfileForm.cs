using System.Windows.Forms;

namespace ColorManager
{
    public partial class ProfileForm : Form
    {
        public ProfileForm()
        {
            InitializeComponent();
            TopMost = Program.Config.TopMostWindow;
        }

        public string Value => txtValue.Text;

        public static ProfileForm ShowDialog(IWin32Window owner, string message, string caption)
        {
            return ShowDialog(owner, message, caption, string.Empty);
        }

        public static ProfileForm ShowDialog(IWin32Window owner, string message, string caption, string defaultValue)
        {
            var form = new ProfileForm {
                Text = caption,
                lblMessage = {Text = message},
                txtValue = {Text = defaultValue}
            };

            form.ShowDialog(owner);
            return form;
        }
    }
}
