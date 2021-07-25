using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ColorManager.Controls
{
    public sealed class ListViewEx : ListView
    {
        [DllImport("uxtheme.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hwnd, string pszSubAppName, string pszSubIdList);

        [DefaultValue(-1)]
        public int ColumnLockIndex { get; set; } = -1;

        public bool ItemsSelected => SelectedItems.Count > 0;

        public ListViewItem SelectedItem => ItemsSelected ? SelectedItems[0] : null;

        public void DeselectAll()
        {
            foreach (ListViewItem item in Items) {
                item.Selected = false;
            }
        }
        
        private void ResizeColumLock()
        {
            if (!Visible) return;
            if (!Visible) return;
            if (Columns.Count == 0) return;
            if (ColumnLockIndex == -1) return;
            if (ColumnLockIndex > Columns.Count - 1) {
                ColumnLockIndex = -1;
                return;
            }
            var combinedWidth = Columns.Cast<ColumnHeader>().Select(c => c.Width).Sum() - Columns[ColumnLockIndex].Width;
            Columns[ColumnLockIndex].Width = Width - combinedWidth - 22;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ResizeColumLock();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            SetWindowTheme(Handle, "explorer", null);
            View = View.Details;
            FullRowSelect = true;
        }
    }
}
