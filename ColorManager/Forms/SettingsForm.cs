using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ColorManager.Forms;

namespace ColorManager
{
    public partial class SettingsForm : Form
    {
        private static readonly Dictionary<string, int> ZoomLabelMap = new() {
            {"2x", 4},
            {"4x", 8},
            {"6x", 14},
            {"8x", 23}
        };

        private static readonly Dictionary<int, string> ZoomValueMap = ZoomLabelMap.ToDictionary(p => p.Value, p => p.Key);

        public SettingsForm()
        {
            InitializeComponent();
            
            TopMost = Program.Config.TopMostWindow;

            cmbZoomLevel.Items.AddRange(ZoomLabelMap.Keys.ToArray());
        }

        public new static DialogResult ShowDialog(IWin32Window owner) => new SettingsForm().ShowDialogInternal(owner);

        private DialogResult ShowDialogInternal(IWin32Window owner) => base.ShowDialog(owner);

        private void OnLoad(object sender, EventArgs e)
        {
            txtProfilesDir.Text = Program.Config.ProfilesDirectory;
            cmbZoomLevel.SelectedItem = ZoomValueMap.ContainsKey(Program.Config.LensZoomLevel) ? ZoomValueMap[Program.Config.LensZoomLevel] : "6x";
            chkTopMost.Checked = Program.Config.TopMostWindow;
            chkCopyHex.Checked = Program.Config.CopyHexHashtag;
        }

        private void OnDoneClick(object sender, EventArgs e)
        {
            Program.Config.ProfilesDirectory = txtProfilesDir.Text;
            Program.Config.LensZoomLevel = ZoomLabelMap[cmbZoomLevel.SelectedItem?.ToString() ?? "6x"];
            Program.Config.TopMostWindow = chkTopMost.Checked;
            Program.Config.CopyHexHashtag = chkCopyHex.Checked;

            Program.Config.Save();

            DialogResult = DialogResult.OK;
        }

        private void OnChooseProfilesDirClick(object sender, EventArgs e)
        {
            if (fbd.ShowDialog(this) != DialogResult.OK) return;

            txtProfilesDir.Text = fbd.SelectedPath;
        }

        private void OnAboutClick(object sender, EventArgs e)
        {
            new AboutForm().ShowDialog(this);
        }
    }
}
