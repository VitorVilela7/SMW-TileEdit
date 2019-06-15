// WARNING: Unfinished -- It's buggy.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Security.Cryptography;

namespace SuperSMWTextEditor
{
    public partial class OverworldMisc : BaseForm
    {
        // Edits Save Menu, Continue Menu

        #region Init
        public OverworldMisc()
        {
            SetStyle(Program.DoubleBuffered, true);
            InitializeComponent();
            CMenu = contextMenuStrip1;
        }

        private void OverworldMisc_Load(object sender, EventArgs e)
        {
            toolStripComboBox1.SelectedIndex = 0; // palette
            comboBox1.SelectedIndex = 0; // what thing to edit
            currentPalette = 0; // palette

            graphics = Program.LoadL3Graphics();
            LoadTilemap();
            LoadPalette();

            loaded = true;
        }
        #endregion

        #region Loading Code
        private void LoadPalette()
        {
            int[] Palette = new int[256];
            byte[] buffer = Properties.Resources.overworld;

            if (File.Exists("./overworld.pal")) // check for custom palette.
            {
                buffer = File.ReadAllBytes("./overworld.pal");
            }

            for (int i = 0, x = 0; i < 256; ++i, x += 3)
            {
                // Convert to BSNES palette, and put in PC palette.
                // aabbb--- -> aabbbbbb
                Palette[i] = (buffer[x + 2] & ~7) | (buffer[x + 2] >> 3 & 7);
                Palette[i] |= ((buffer[x + 1] & ~7) | (buffer[x + 1] >> 3 & 7)) << 8;
                Palette[i] |= ((buffer[x + 0] & ~7) | (buffer[x + 0] >> 3 & 7)) << 16;
                Palette[i] |= (int)(-((0xff000000 ^ 0xffffffff) + 1)); // (int)0xFFFFFFFF
            }

            for (int i = 0; i < 32; i += 4)
            {
                Palette[i] = Color.AliceBlue.ToArgb(); // transparency
            }

            this.palette = new int[256];

            // Convert Color-4 palette to Color-16 palette.
            // This will destroy non Color-4 palette, but I don't worry!
            for (int i = 0, x = 0; i < 256; i += 16, x += 4)
            {
                this.palette[i] = Palette[x];
                this.palette[i + 1] = Palette[x + 1];
                this.palette[i + 2] = Palette[x + 2];
                this.palette[i + 3] = Palette[x + 3];

                // Reserved color
                // it's used by reserved tile (0x3FF) to say that this is a NULL tile.
                this.palette[i + 15] = Color.DodgerBlue.ToArgb();
            }
        }

        private void LoadTilemap()
        {
            try
            {
                continueMenu = new int[0x400];
                saveMenu = new int[0x400];

                graphicsTilemap = new int[0x400];

                int cnt = 0;

                for (int i = 0; i < 0x400; ++i)
                {
                    continueMenu[i] = 0x3FF;
                    saveMenu[i] = 0x3FF;

                    if ((i & 0x10) == 0)
                        graphicsTilemap[i] = cnt++;
                }

                if (!Program.Loaded)
                {
                    return;
                }

                byte[] continueStripe = Program.ReadROM(0x05B91C, 31);
                byte[] saveStripe = Program.ReadROM(0x05B8C7, 101);
                Program.CloseHandle();

                Program.StripeImageRt(ref continueStripe, ref continueMenu, 0, false);
                Program.StripeImageRt(ref saveStripe, ref saveMenu, 0, false);

                //int index = Program.StripeImageRt(ref buffer, ref eraseMenu, 0, false);
                //Program.StripeImageRt(ref buffer, ref fileSelect, index, false);
                //Program.StripeImageRt(ref play12, ref playerSelect, 0, false);
                //tilemapChanged();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while creating tilemap: " + e.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Fields
        private int mouseX, mouseY;

        private bool loaded;

        private byte[] graphics;

        private int[] saveMenu, continueMenu;

        private int[] graphicsTilemap;
        private int[] palette;

        private bool flipX, flipY;

        private int currentPalette, selectedTile;

        private bool changed;
        #endregion

        #region Drawing Screen (20 FPS)
        // This renders so fast that I can reach 60 FPS without problem.
        // (I'm talking about you, Program.DrawTilemap!)

        private unsafe void DrawTilemap(object sender, PaintEventArgs e)
        {
            using (Bitmap bmp = new Bitmap(256, 520, PixelFormat.Format24bppRgb))
            {
                fixed (int* palPtr = palette, select = &selectedTile, editPtr = graphicsTilemap,
                    savePtr = saveMenu, continuePtr = continueMenu)
                fixed (byte* gfxPtr = graphics)
                {
                    int* ptr;
                    switch (comboBox1.SelectedIndex)
                    {
                        case 0: ptr = savePtr; break;
                        case 1: ptr = continuePtr; break;
                        default: throw new Exception("Internal Error.");
                    }

                    Program.DrawTilemap(bmp, new Rectangle(0, 0, 256, 256),
                        0x400, palPtr, ptr, gfxPtr, 4, 8);
                    Program.DrawTilemap(bmp, new Rectangle(0, 256, 256, 256),
                        0x400, palPtr, editPtr, gfxPtr, 4, 8);
                    Program.DrawTilemap(bmp, new Rectangle(0, 512, 8, 8),
                        1, palPtr, select, gfxPtr, 4, 3);

                    // y >= 256+ is for graphics, not tilemap.

                    // for x = 248 -> 256
                    // and y = 256 -> 8+256, it's the selected tile.
                }

                e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                e.Graphics.DrawImage(bmp, new Rectangle(0, 0, 192 * 2, 56 * 2), new Rectangle(48-16, 112-16, 192, 56), GraphicsUnit.Pixel);
                
                e.Graphics.DrawImage(bmp, new Rectangle(0, 112 + 16, 256, 256), new Rectangle(0, 256, 128, 128), GraphicsUnit.Pixel);
                e.Graphics.DrawImage(bmp, new Rectangle(256, 112 + 16, 256, 256), new Rectangle(0, 256 + 128, 128, 128), GraphicsUnit.Pixel);

                //64+8
                //192*2+16
                e.Graphics.DrawImage(bmp, new Rectangle(192 * 2 + 40, 64 + 8, 48, 48), new Rectangle(0, 512, 8, 8), GraphicsUnit.Pixel);

                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.BlueViolet)), mouseX, mouseY, 16, 16);
            }
        }

        private void propertiesChanged()
        {
            if (!loaded) return;

            int OR = (currentPalette & 7) << 10;
            int AND = ((7 << 10) | 0xC000) ^ 0xFFFF;

            if (flipX) OR |= 0x4000;
            if (flipY) OR |= 0x8000;

            for (int i = 0; i < 0x400; ++i)
            {
                graphicsTilemap[i] &= AND;
                graphicsTilemap[i] |= OR;
            }

            selectedTile &= AND;
            selectedTile |= OR;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Invalidate();
        }
        #endregion

        #region User Input from Mouse/Keyboard
        private void EditMiscText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode >= Keys.D1 && e.KeyCode <= Keys.D8)
            {
                int number = e.KeyCode - Keys.D1;
                if (currentPalette != number)
                {
                    currentPalette = number;
                    toolStripComboBox1.SelectedIndex = currentPalette;
                    propertiesChanged();
                }
            }
            else if (e.KeyCode >= Keys.NumPad1 && e.KeyCode <= Keys.NumPad8)
            {
                int number = e.KeyCode - Keys.NumPad1;
                if (currentPalette != number)
                {
                    currentPalette = number;
                    toolStripComboBox1.SelectedIndex = currentPalette;
                    propertiesChanged();
                }
            }
            else
            {
                switch (e.KeyCode)
                {
                    case Keys.X:
                        flipX = !flipX;
                        flipXToolStripMenuItem.Checked = flipX;
                        propertiesChanged();
                        break;

                    case Keys.Y:
                        flipY = !flipY;
                        flipYToolStripMenuItem.Checked = flipY;
                        propertiesChanged();
                        break;

                    case Keys.E:
                        ExportOverworldMiscText();
                        break;

                    case Keys.I:
                        ImportOverworldMiscText();
                        break;

                    case Keys.S:
                        {
                            try
                            {
                                SaveROM();
                                MessageBox.Show("Successful saved!", "Information",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            catch (Exception E)
                            {
                                MessageBox.Show(E.Message, "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            break;
                        }
                }
            }
        }

        private void EditMiscText_MouseMove(object sender, MouseEventArgs e)
        {
            mouseX = e.X >> 4 << 4;
            mouseY = e.Y >> 4 << 4;
        }

        private void EditMiscText_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left: // get tile
                    if (e.X >= 0 && e.X <= 192 * 2 && e.Y >= 0 && e.Y <= 56 * 2)
                    {
                        // get from stripe image.
                        int tX = 48-16 + (e.X >> 1);
                        int tY = 112-16 + (e.Y >> 1);

                        int tile = (tX >> 3 & 0x1F) + ((tY >> 3 & 0x1F) << 5);

                        if (comboBox1.SelectedIndex == 0) // save menu
                        {
                            if (saveMenu[tile] != 0x3FF)
                            {
                                selectedTile = saveMenu[tile];
                            }
                        }
                        else // continue menu
                        {
                            if (continueMenu[tile] != 0x3FF)
                            {
                                selectedTile = continueMenu[tile];
                            }
                        }
                    }
                    else if (e.X >= 0 && e.X <= 256 && e.Y >= (112 + 16) && e.Y <= (112 + 16 + 256))
                    {
                        // get from tiles #1
                        int tX = e.X >> 1;
                        int tY = (e.Y - (112 + 16)) >> 1;

                        selectedTile = (tX >> 3 & 0x0F) + ((tY >> 3 & 0x1F) << 4);
                        selectedTile |= currentPalette << 10;
                        if (flipX) selectedTile |= 0x4000;
                        if (flipY) selectedTile |= 0x8000;
                    }
                    else if (e.X >= 256 && e.X <= 512 && e.Y >= (112 + 16) && e.Y <= (112 + 16 + 256))
                    {
                        // get from tiles #2
                        int tX = (e.X - 256) >> 1;
                        int tY = (e.Y - (112 + 16)) >> 1;

                        selectedTile = (tX >> 3 & 0x0F) + ((tY >> 3 & 0x1F) << 4);
                        selectedTile |= currentPalette << 10;
                        selectedTile |= 0x100;
                    }

                    break;

                case MouseButtons.Right: // store tile
                    if (e.X >= 0 && e.X <= 192 * 2 && e.Y >= 0 && e.Y <= 56 * 2)
                    {
                        // get from stripe image.
                        int tX = 48-16 + (e.X >> 1);
                        int tY = 112-16 + (e.Y >> 1);

                        int tile = (tX >> 3 & 0x1F) + ((tY >> 3 & 0x1F) << 5);

                        if (comboBox1.SelectedIndex == 0) // save menu
                        {
                            if (saveMenu[tile] != 0x3FF)
                            {
                                saveMenu[tile] = selectedTile;
                                //tilemapChanged();
                            }
                        }
                        else // continue menu
                        {
                            if (continueMenu[tile] != 0x3FF)
                            {
                                continueMenu[tile] = selectedTile;
                                //tilemapChanged();
                            }
                        }

                        changed = true;
                    }
                    break;
            }
        }
        #endregion

        #region Export/Import/Save ROM
        private void SaveROM()
        {
            byte[] cont, save;

            try
            {
                cont = Program.ReadROM(0x05B91C, 31);
                save = Program.ReadROM(0x05B8C7, 101);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Error while reading ROM: {0}", e.Message));
            }

            Program.StripeImageRt(ref cont, ref continueMenu, 0, false, true);
            Program.StripeImageRt(ref save, ref saveMenu, 0, false, true);

            try
            {
                Program.WriteROM(0x05B91C, cont);
                Program.WriteROM(0x05B8C7, save);
                Program.CloseHandle();
                changed = false;
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Error while writing ROM: {0}", e.Message));
            }
        }

        private void ImportOverworldMiscText()
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ImportFrom(openFileDialog1.FileName);
                }
                catch (Exception E)
                {
                    MessageBox.Show(E.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ExportOverworldMiscText()
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ExportTo(saveFileDialog1.FileName);
                }
                catch (Exception E)
                {
                    MessageBox.Show(E.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        
        private void ImportFrom(string p)
        {
            var data = File.ReadAllBytes(p);

            if (data.Length != (31 + 101 + 16 + 1))
            {
                throw new Exception("File has a invalid size.");
            }

            var md5_new = MD5.Create().ComputeHash(data, 0, 31 + 101);
            var md5_save = new byte[16];
            Array.Copy(data, 31 + 101, md5_save, 0, 16);

            for (var i = 0; i < 16; ++i)
            {
                if (md5_new[i] != md5_save[i])
                {
                    throw new Exception("Invalid checksum.");
                }
            }

            if (data[data.Length - 1] != 0x02)
            {
                throw new Exception("Invalid signature/version/export type.");
            }

            var index = Program.StripeImageRt(ref data, ref saveMenu, 0, false);
            Program.StripeImageRt(ref data, ref continueMenu, index, false);
            //tilemapChanged();

            changed = true;
        }
        private void ExportTo(string p)
        {
            var output = new byte[31 + 101 + 16 + 1];

            try
            {
                byte[] continueStripe = Program.ReadROM(0x05B91C, 31);
                byte[] saveStripe = Program.ReadROM(0x05B8C7, 101);
                Program.CloseHandle();

                saveStripe.CopyTo(output, 0);
                continueStripe.CopyTo(output, 101);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Error while reading ROM: {0}", e.Message));
            }

            var index = Program.StripeImageRt(ref output, ref saveMenu, 0, false, true);
             Program.StripeImageRt(ref output, ref continueMenu, index, false, true);

            var md5_new = MD5.Create().ComputeHash(output, 0, 31 + 101);
            md5_new.CopyTo(output, 31 + 101);

            output[output.Length - 1] = 0x02; // signature
            File.WriteAllBytes(p, output);
        }
        #endregion

        #region Window 2 Communication
        private EditMusicText2 textEdit;

        private void spawnWindow()
        {
            textEdit = new EditMusicText2();
            textEdit.Owner = this;
            textEdit.UpdateTilemapFrom += new EditMusicText2.TilemapUpdate(textEdit_UpdateTilemapFrom);
            tilemapChanged();
            textEdit.Show();
        }

        private void tilemapChanged()
        {
            if (textEdit != null && !textEdit.IsDisposed)
            {
                textEdit.UpdateTilemapTo(saveMenu, continueMenu, null, comboBox1.SelectedIndex, 1);
            }
        }

        private void textEdit_UpdateTilemapFrom(int[] save, int[] cont, int[] unused)
        {
            this.saveMenu = save;
            this.continueMenu = cont;
            changed = true;
        }
        #endregion

        #region User General Interaction
        private void button1_Click(object sender, EventArgs e)
        {
            if (textEdit != null)
            {
                if (textEdit.IsDisposed)
                {
                    textEdit = null;
                    spawnWindow();
                }
                else
                {
                    textEdit.Focus();
                }
            }
            else
            {
                spawnWindow();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            tilemapChanged();
        }

        private void flipXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            flipX = flipXToolStripMenuItem.Checked;
            propertiesChanged();
        }

        private void flipYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            flipY = flipYToolStripMenuItem.Checked;
            propertiesChanged();
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentPalette = toolStripComboBox1.SelectedIndex;
            propertiesChanged();
        }
        #endregion

        private void exportOverworldMiscTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImportOverworldMiscText();
        }

        private void importOverworldMiscTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportOverworldMiscText();
        }

        private void OverworldMisc_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (changed)
            {
                switch (MessageBox.Show("Save changes to your ROM?", "Question",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button3))
                {
                    case System.Windows.Forms.DialogResult.No:
                        return;

                    case System.Windows.Forms.DialogResult.Yes:
                        try
                        {
                            SaveROM();
                        }
                        catch (Exception E)
                        {
                            MessageBox.Show(E.Message, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            e.Cancel = true;
                        }
                        return;

                    default:
                        e.Cancel = true;
                        return;
                }
            }
        }
    }
}
