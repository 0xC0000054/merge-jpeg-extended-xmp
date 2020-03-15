namespace MergeJPEGExtendedXMP
{
    partial class Form1
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.mergedXmpTab = new System.Windows.Forms.TabPage();
            this.mainXmpTab = new System.Windows.Forms.TabPage();
            this.extendedXmpTab = new System.Windows.Forms.TabPage();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.mergedXmpTextBox = new MergeJPEGExtendedXMP.XmpTextBox();
            this.mainXmpTextBox = new MergeJPEGExtendedXMP.XmpTextBox();
            this.extendedXmpTextBox = new MergeJPEGExtendedXMP.XmpTextBox();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wrapXMPTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileNameLabel = new System.Windows.Forms.Label();
            this.xmpSizeLabel = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.mergedXmpTab.SuspendLayout();
            this.mainXmpTab.SuspendLayout();
            this.extendedXmpTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1060, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openToolStripMenuItem.Text = "&Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.mergedXmpTab);
            this.tabControl1.Controls.Add(this.mainXmpTab);
            this.tabControl1.Controls.Add(this.extendedXmpTab);
            this.tabControl1.Location = new System.Drawing.Point(13, 39);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1035, 614);
            this.tabControl1.TabIndex = 1;
            // 
            // mergedXmpTab
            // 
            this.mergedXmpTab.Controls.Add(this.mergedXmpTextBox);
            this.mergedXmpTab.Location = new System.Drawing.Point(4, 22);
            this.mergedXmpTab.Name = "mergedXmpTab";
            this.mergedXmpTab.Padding = new System.Windows.Forms.Padding(3);
            this.mergedXmpTab.Size = new System.Drawing.Size(1027, 588);
            this.mergedXmpTab.TabIndex = 0;
            this.mergedXmpTab.Text = "Merged XMP";
            this.mergedXmpTab.UseVisualStyleBackColor = true;
            // 
            // mainXmpTab
            // 
            this.mainXmpTab.Controls.Add(this.mainXmpTextBox);
            this.mainXmpTab.Location = new System.Drawing.Point(4, 22);
            this.mainXmpTab.Name = "mainXmpTab";
            this.mainXmpTab.Padding = new System.Windows.Forms.Padding(3);
            this.mainXmpTab.Size = new System.Drawing.Size(1027, 636);
            this.mainXmpTab.TabIndex = 1;
            this.mainXmpTab.Text = "Main XMP";
            this.mainXmpTab.UseVisualStyleBackColor = true;
            // 
            // extendedXmpTab
            // 
            this.extendedXmpTab.Controls.Add(this.extendedXmpTextBox);
            this.extendedXmpTab.Location = new System.Drawing.Point(4, 22);
            this.extendedXmpTab.Name = "extendedXmpTab";
            this.extendedXmpTab.Padding = new System.Windows.Forms.Padding(3);
            this.extendedXmpTab.Size = new System.Drawing.Size(1027, 636);
            this.extendedXmpTab.TabIndex = 2;
            this.extendedXmpTab.Text = "Extended XMP";
            this.extendedXmpTab.UseVisualStyleBackColor = true;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "JPEG Images (*.jpg, *.jpeg, *.jpe, *.jfif)|*.jpg;*.jpeg;*.jpe;*.jfif;";
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // mergedXmpTextBox
            // 
            this.mergedXmpTextBox.AutomaticFold = ((ScintillaNET.AutomaticFold)(((ScintillaNET.AutomaticFold.Show | ScintillaNET.AutomaticFold.Click) 
            | ScintillaNET.AutomaticFold.Change)));
            this.mergedXmpTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.mergedXmpTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mergedXmpTextBox.Lexer = ScintillaNET.Lexer.Xml;
            this.mergedXmpTextBox.Location = new System.Drawing.Point(3, 3);
            this.mergedXmpTextBox.Name = "mergedXmpTextBox";
            this.mergedXmpTextBox.ReadOnly = true;
            this.mergedXmpTextBox.Size = new System.Drawing.Size(1021, 582);
            this.mergedXmpTextBox.TabIndex = 0;
            // 
            // mainXmpTextBox
            // 
            this.mainXmpTextBox.AutomaticFold = ((ScintillaNET.AutomaticFold)(((ScintillaNET.AutomaticFold.Show | ScintillaNET.AutomaticFold.Click) 
            | ScintillaNET.AutomaticFold.Change)));
            this.mainXmpTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.mainXmpTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainXmpTextBox.Lexer = ScintillaNET.Lexer.Xml;
            this.mainXmpTextBox.Location = new System.Drawing.Point(3, 3);
            this.mainXmpTextBox.Name = "mainXmpTextBox";
            this.mainXmpTextBox.ReadOnly = true;
            this.mainXmpTextBox.Size = new System.Drawing.Size(1021, 630);
            this.mainXmpTextBox.TabIndex = 0;
            // 
            // extendedXmpTextBox
            // 
            this.extendedXmpTextBox.AutomaticFold = ((ScintillaNET.AutomaticFold)(((ScintillaNET.AutomaticFold.Show | ScintillaNET.AutomaticFold.Click) 
            | ScintillaNET.AutomaticFold.Change)));
            this.extendedXmpTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.extendedXmpTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.extendedXmpTextBox.Lexer = ScintillaNET.Lexer.Xml;
            this.extendedXmpTextBox.Location = new System.Drawing.Point(3, 3);
            this.extendedXmpTextBox.Name = "extendedXmpTextBox";
            this.extendedXmpTextBox.ReadOnly = true;
            this.extendedXmpTextBox.Size = new System.Drawing.Size(1021, 630);
            this.extendedXmpTextBox.TabIndex = 0;
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.wrapXMPTextToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // wrapXMPTextToolStripMenuItem
            // 
            this.wrapXMPTextToolStripMenuItem.CheckOnClick = true;
            this.wrapXMPTextToolStripMenuItem.Name = "wrapXMPTextToolStripMenuItem";
            this.wrapXMPTextToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.wrapXMPTextToolStripMenuItem.Text = "Wrap XMP Text";
            this.wrapXMPTextToolStripMenuItem.Click += new System.EventHandler(this.wrapXMPTextToolStripMenuItem_Click);
            // 
            // fileNameLabel
            // 
            this.fileNameLabel.AutoSize = true;
            this.fileNameLabel.Location = new System.Drawing.Point(14, 656);
            this.fileNameLabel.Name = "fileNameLabel";
            this.fileNameLabel.Size = new System.Drawing.Size(74, 13);
            this.fileNameLabel.TabIndex = 2;
            this.fileNameLabel.Text = "fileNameLabel";
            // 
            // xmpSizeLabel
            // 
            this.xmpSizeLabel.AutoSize = true;
            this.xmpSizeLabel.Location = new System.Drawing.Point(14, 679);
            this.xmpSizeLabel.Name = "xmpSizeLabel";
            this.xmpSizeLabel.Size = new System.Drawing.Size(72, 13);
            this.xmpSizeLabel.TabIndex = 3;
            this.xmpSizeLabel.Text = "xmpSizeLabel";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1060, 713);
            this.Controls.Add(this.xmpSizeLabel);
            this.Controls.Add(this.fileNameLabel);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Merge Extended JPEG";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.mergedXmpTab.ResumeLayout(false);
            this.mainXmpTab.ResumeLayout(false);
            this.extendedXmpTab.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage mergedXmpTab;
        private System.Windows.Forms.TabPage mainXmpTab;
        private System.Windows.Forms.TabPage extendedXmpTab;
        private MergeJPEGExtendedXMP.XmpTextBox mergedXmpTextBox;
        private MergeJPEGExtendedXMP.XmpTextBox mainXmpTextBox;
        private MergeJPEGExtendedXMP.XmpTextBox extendedXmpTextBox;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wrapXMPTextToolStripMenuItem;
        private System.Windows.Forms.Label fileNameLabel;
        private System.Windows.Forms.Label xmpSizeLabel;
    }
}