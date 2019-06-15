namespace SuperSMWTextEditor
{
    partial class OverworldMisc
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editWithTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.exportOverworldMiscTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importOverworldMiscTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.flipXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flipYToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.button1 = new System.Windows.Forms.Button();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editWithTextToolStripMenuItem,
            this.toolStripSeparator3,
            this.exportOverworldMiscTextToolStripMenuItem,
            this.importOverworldMiscTextToolStripMenuItem,
            this.toolStripSeparator2,
            this.flipXToolStripMenuItem,
            this.flipYToolStripMenuItem,
            this.toolStripSeparator1,
            this.toolStripComboBox1});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(234, 159);
            // 
            // editWithTextToolStripMenuItem
            // 
            this.editWithTextToolStripMenuItem.Name = "editWithTextToolStripMenuItem";
            this.editWithTextToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.editWithTextToolStripMenuItem.Text = "Edit with Text...";
            this.editWithTextToolStripMenuItem.Click += new System.EventHandler(this.button1_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(230, 6);
            // 
            // exportOverworldMiscTextToolStripMenuItem
            // 
            this.exportOverworldMiscTextToolStripMenuItem.Name = "exportOverworldMiscTextToolStripMenuItem";
            this.exportOverworldMiscTextToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.exportOverworldMiscTextToolStripMenuItem.Text = "Export Overworld Misc. Text";
            this.exportOverworldMiscTextToolStripMenuItem.Click += new System.EventHandler(this.exportOverworldMiscTextToolStripMenuItem_Click);
            // 
            // importOverworldMiscTextToolStripMenuItem
            // 
            this.importOverworldMiscTextToolStripMenuItem.Name = "importOverworldMiscTextToolStripMenuItem";
            this.importOverworldMiscTextToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.importOverworldMiscTextToolStripMenuItem.Text = "Import Overworld Misc. Text...";
            this.importOverworldMiscTextToolStripMenuItem.Click += new System.EventHandler(this.importOverworldMiscTextToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(230, 6);
            // 
            // flipXToolStripMenuItem
            // 
            this.flipXToolStripMenuItem.CheckOnClick = true;
            this.flipXToolStripMenuItem.Name = "flipXToolStripMenuItem";
            this.flipXToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.flipXToolStripMenuItem.Text = "Flip X";
            this.flipXToolStripMenuItem.Click += new System.EventHandler(this.flipXToolStripMenuItem_Click);
            // 
            // flipYToolStripMenuItem
            // 
            this.flipYToolStripMenuItem.CheckOnClick = true;
            this.flipYToolStripMenuItem.Name = "flipYToolStripMenuItem";
            this.flipYToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.flipYToolStripMenuItem.Text = "Flip Y";
            this.flipYToolStripMenuItem.Click += new System.EventHandler(this.flipYToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(230, 6);
            // 
            // toolStripComboBox1
            // 
            this.toolStripComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBox1.Items.AddRange(new object[] {
            "Palette 0",
            "Palette 1",
            "Palette 2",
            "Palette 3",
            "Palette 4",
            "Palette 5",
            "Palette 6",
            "Palette 7"});
            this.toolStripComboBox1.Name = "toolStripComboBox1";
            this.toolStripComboBox1.Size = new System.Drawing.Size(121, 23);
            this.toolStripComboBox1.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox1_SelectedIndexChanged);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 50;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Edit Save Menu",
            "Edit Continue Menu"});
            this.comboBox1.Location = new System.Drawing.Point(404, 12);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(96, 21);
            this.comboBox1.TabIndex = 1;
            this.comboBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.EditMiscText_KeyDown);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "Binary data|*.bin";
            this.saveFileDialog1.RestoreDirectory = true;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "Binary data|*.bin";
            this.openFileDialog1.RestoreDirectory = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(404, 40);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(96, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Edit with Text";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // OverworldMisc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(512, 384);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.comboBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OverworldMisc";
            this.Text = "Edit Misc. Overworld Text";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OverworldMisc_FormClosing);
            this.Load += new System.EventHandler(this.OverworldMisc_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.DrawTilemap);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.EditMiscText_KeyDown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.EditMiscText_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.EditMiscText_MouseMove);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
        private System.Windows.Forms.ToolStripMenuItem flipXToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem flipYToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem importOverworldMiscTextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportOverworldMiscTextToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolStripMenuItem editWithTextToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    }
}