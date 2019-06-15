using System;
using System.Drawing;
using System.Windows.Forms;

namespace SuperSMWTextEditor
{
    public partial class MDIParent1 : Form
    {
        public MDIParent1()
        {
            InitializeComponent();

            // check if the ROM is loaded.
            if (Program.Loaded)
            {
                editToolStripMenuItem.Enabled = true;
            }
        }

        private bool IsAnyOpen()
        {
            return MdiChildren.Length > 0;
        }

        private void CloseAll()
        {
            foreach (BaseForm f in MdiChildren)
            {
                f.Close();
            }

            optionsToolStripMenuItem.DropDown = null;
            optionsToolStripMenuItem.Enabled = false;

            ClientSize = new Size(284, 28);
        }

        private void CreateWindow(BaseForm form)
        {
            foreach (BaseForm f in MdiChildren)
            {
                if (f.Text == form.Text)
                {
                    f.Focus();
                    form.Dispose();
                    return;
                }
                else
                {
                    f.Close();
                }
            }

            if (IsAnyOpen())
            {
                form.Dispose();
                return;
            }

            ClientSize = new Size(form.Size.Width + 5,
                form.Size.Height + menuStrip.Size.Height + 5);

            optionsToolStripMenuItem.Enabled = true;
            optionsToolStripMenuItem.DropDown = form.CMenu;

            form.MdiParent = this;
            form.Show();
            form.Top = 0;
            form.Left = 0;
        }

        private void Protect(System.Threading.ThreadStart del)
        {
            try
            {
                del();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                CloseAll();

                if (IsAnyOpen())
                {
                    return;
                }

                Protect(delegate()
                {
                    Program.LoadROM(openFileDialog1.FileName);
                    editToolStripMenuItem.Enabled = true;
                });
            }
        }

        private void miscTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateWindow(new EditMiscText());
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"SMW TileEdit v0.1 (11/2012)
by Vitor Vilela.

Thanks FuSoYa for Lunar Compress DLL.", "About SMW TileEdit");
        }

        private void overworldMiscTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateWindow(new OverworldMisc());
        }
    }
}
