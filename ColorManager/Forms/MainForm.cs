using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorManager
{
    public partial class MainForm : Form
    {
        private const string defaultName = "Default";
        private readonly ColorProfileManager _profileManager;

        private ColorProfile _currentProfile;
        private bool _isSelect;
        private bool _pauseItemSelect;

        public MainForm()
        {
            InitializeComponent();
            ApplyConfig();

            Enabled = false;

            _profileManager = new ColorProfileManager();
        }
        
        private bool SelectionState {
            set {
                if (value) {
                    txtDesc.Clear();
                    lensBox.Start();
                }
                else {
                    lensBox.Stop();
                    TrySelectExistingItem(txtColorHex.Text);
                    btnSave.Enabled = true;
                }
            }
        }

        private void ApplyConfig()
        {
            lensBox.LensZoom = Program.Config.LensZoomLevel;
            TopMost = Program.Config.TopMostWindow;
        }

        private void TrySelectExistingItem(string hex)
        {
            _pauseItemSelect = true;
            listView.DeselectAll();
            _pauseItemSelect = false;

            var item = listView.FindColorItem(hex);
            if (item == null) return;
            item.Selected = true;
        }

        private async Task LoadProfilesAsync()
        {
            await _profileManager.LoadAsync();

            if (!_profileManager.Profiles.ContainsKey(defaultName)) {
                await _profileManager.CreateProfileAsync(defaultName);
            }

            var keys = _profileManager.Profiles.Keys.ToArray();
            cmbProfiles.Items.AddRange(keys);
            cmbProfiles.SelectedItem = defaultName;
        }

        private async void OnLoad(object sender, EventArgs e)
        {
            await LoadProfilesAsync();

            Enabled = true;
        }
        
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!(e.Control && e.KeyCode == Keys.G)) return;

            _isSelect = !_isSelect;
            SelectionState = _isSelect;
            btnSelectToggle.Text = $"Toggle Select [{(_isSelect ? "On" : "Off")}]\r\n(Ctrl+G)";
        }

        private async void OnSaveClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDesc.Text) || string.IsNullOrWhiteSpace(txtDesc.Text)) {
                MessageBox.Show(
                    "Description cannot be null, empty, or whitespace",
                    "Invalid Description",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var item = (ColorItem) listView.SelectedItem;
            if (item == null) {
                item = new ColorItem(lblColor.BackColor, txtDesc.Text, lensBox.Image.CropFromCenter(lensBox.Width, lensBox.Height));
                listView.AddColorItem(item);
                _currentProfile.Colors.Add(item);
                item.Selected = true;
            }
            else {
                item.SetDescription(txtDesc.Text);
            }

            await _currentProfile.SaveAsync();
        }

        private void OnCopyClick(object sender, EventArgs e)
        {
            var name = ((Button) sender).Tag.ToString();
            var txt = groupColor.Controls[name].Text;
            if (Program.Config.CopyHexHashtag && name == nameof(txtColorHex))
                txt = "#" + txt;
            Clipboard.SetText(txt);
        }

        private async void OnRemoveClick(object sender, EventArgs e)
        {
            if (!listView.ItemsSelected) return;
            foreach (var item in listView.SelectedItems.Cast<ColorItem>()) {
                listView.Items.Remove(item);
                _currentProfile.Colors.Remove(item);
            }

            await _currentProfile.SaveAsync();
        }

        private void OnListSelectedIndexChanged(object sender, EventArgs e)
        {
            if (_pauseItemSelect) return;

            if (!listView.ItemsSelected) {
                btnSave.Enabled = false;
                lensBox.SetImage(null, false);
                lblColor.BackColor = Color.White;
                txtColorHex.Clear();
                txtColorRgb.Clear();
                txtDesc.Clear();
                return;
            }

            var item = (ColorItem) listView.SelectedItem;

            lensBox.SetImage(item.Thumbnail, false);
            lblColor.BackColor = item.Color;
            txtColorHex.Text = item.Color.ToHex();
            txtColorRgb.Text = item.Color.ToRgbString();
            txtDesc.Text = item.Description;
            btnSave.Enabled = true;
        }

        private void OnLensFocusChanged(Color color)
        {
            lblColor.BackColor = color;
            txtColorHex.Text = color.ToHex();
            txtColorRgb.Text = color.ToRgbString();
        }

        private async void OnSettingsClick(object sender, EventArgs e)
        {
            var oldProfilesDir = Program.Config.ProfilesDirectory;
            if (SettingsForm.ShowDialog(this) != DialogResult.OK) return;

            ApplyConfig();

            if (Program.Config.ProfilesDirectory == oldProfilesDir) return;

            _currentProfile.Dispose();
            _currentProfile = null;

            listView.DeselectAll();
            listView.ClearColorItems();
            cmbProfiles.Items.Clear();

            await LoadProfilesAsync();
        }

        private void OnButtonHoldSelectDown(object sender, MouseEventArgs e) => SelectionState = true;

        private void OnButtonHoldSelectUp(object sender, MouseEventArgs e) => SelectionState = false;

        private async void OnNewProfileClick(object sender, EventArgs e)
        {
            var pf = ProfileForm.ShowDialog(this, "Profile name", "New Profile");
            if (pf.DialogResult != DialogResult.OK) return;

            if (!ValidateProfileName(pf.Value)) {
                btnNew.PerformClick();
                return;
            }

            await _profileManager.CreateProfileAsync(pf.Value);
            cmbProfiles.Items.Add(pf.Value);
            cmbProfiles.SelectedItem = pf.Value;
        }

        private async void OnEditProfileClick(object sender, EventArgs e)
        {
            var profileName = cmbProfiles.SelectedItem.ToString();
            var profileIndex = cmbProfiles.SelectedIndex;
            var pf = ProfileForm.ShowDialog(this, "Profile name", "Edit Profile", profileName);
            if (pf.DialogResult != DialogResult.OK) return;

            if (!ValidateProfileName(pf.Value)) {
                btnEdit.PerformClick();
                return;
            }

            var result = await _profileManager.EditProfileAsync(profileName, pf.Value);
            if (!result) {
                MessageBox.Show("An error occurred while editing profile.", "Edit Profile Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            cmbProfiles.Items[profileIndex] = pf.Value;
        }

        private bool ValidateProfileName(string value)
        {
            if (string.IsNullOrEmpty(value) ||
                string.IsNullOrWhiteSpace(value)) {
                MessageBox.Show(
                    "Profile name cannot be empty or whitespace.",
                    "Invalid Profile Name",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (_profileManager.Profiles.ContainsKey(value)) {
                MessageBox.Show(
                    "A profile with that name already exists.",
                    "Duplicate Profile Name",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private async void OnDeleteProfileClick(object sender, EventArgs e)
        {
            var profileName = cmbProfiles.SelectedItem.ToString();
            var dr = MessageBox.Show(
                $"Are you sure you want to delete '{profileName}'?", 
                "Delete Profile", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dr != DialogResult.Yes) {
                return;
            }

            var result = await _profileManager.DeleteProfileAsync(profileName);

            if (!result) {
                MessageBox.Show("An error occurred while deleting profile.", "Delete Profile Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var fallbackProfile = cmbProfiles.Items[cmbProfiles.Items.IndexOf(cmbProfiles.SelectedItem) - 1];
            cmbProfiles.SelectedItem = fallbackProfile;
            cmbProfiles.Items.Remove(profileName);
        }

        private async void OnProfileSelectedValueChanged(object sender, EventArgs e)
        {
            var name = cmbProfiles.SelectedItem.ToString();
            if (string.IsNullOrEmpty(name)) return;

            var isDefault = name == defaultName;
            btnDelete.Enabled = !isDefault;
            btnEdit.Enabled = !isDefault;
            
            listView.DeselectAll();
            listView.ClearColorItems();

            _currentProfile?.Dispose();
            _currentProfile = null;

            var profile = await _profileManager.Profiles[name]();
            foreach (var item in profile.Colors) {
                listView.AddColorItem(item);
            }
            _currentProfile = profile;
        }
    }
}
