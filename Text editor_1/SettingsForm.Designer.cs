namespace Text_editor_1
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabAppearance;
        private System.Windows.Forms.TabPage tabBehavior;
        private System.Windows.Forms.TabPage tabAdvanced;
        private System.Windows.Forms.GroupBox groupFont;
        private System.Windows.Forms.Label lblFontFamily;
        private System.Windows.Forms.ComboBox cmbFontFamily;
        private System.Windows.Forms.Label lblFontSize;
        private System.Windows.Forms.NumericUpDown numFontSize;
        private System.Windows.Forms.GroupBox groupTheme;
        private System.Windows.Forms.Label lblTheme;
        private System.Windows.Forms.ComboBox cmbTheme;
        private System.Windows.Forms.TextBox txtPreview;
        private System.Windows.Forms.GroupBox groupTextOptions;
        private System.Windows.Forms.CheckBox chkWordWrap;
        private System.Windows.Forms.CheckBox chkShowLineNumbers;
        private System.Windows.Forms.GroupBox groupAutoSave;
        private System.Windows.Forms.CheckBox chkAutoSave;
        private System.Windows.Forms.Label lblAutoSaveInterval;
        private System.Windows.Forms.NumericUpDown numAutoSaveInterval;
        private System.Windows.Forms.Label lblSeconds;
        private System.Windows.Forms.GroupBox groupEncoding;
        private System.Windows.Forms.Label lblDefaultEncoding;
        private System.Windows.Forms.ComboBox cmbDefaultEncoding;
        private System.Windows.Forms.GroupBox groupRecentFiles;
        private System.Windows.Forms.Label lblMaxRecentFiles;
        private System.Windows.Forms.NumericUpDown numMaxRecentFiles;
        private System.Windows.Forms.GroupBox groupInterface;
        private System.Windows.Forms.CheckBox chkShowStatusBar;
        private System.Windows.Forms.CheckBox chkShowToolbar;
        private System.Windows.Forms.GroupBox groupWindow;
        private System.Windows.Forms.Label lblWindowWidth;
        private System.Windows.Forms.NumericUpDown numWindowWidth;
        private System.Windows.Forms.Label lblWindowHeight;
        private System.Windows.Forms.NumericUpDown numWindowHeight;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnReset;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabAppearance = new System.Windows.Forms.TabPage();
            this.groupFont = new System.Windows.Forms.GroupBox();
            this.lblFontFamily = new System.Windows.Forms.Label();
            this.cmbFontFamily = new System.Windows.Forms.ComboBox();
            this.lblFontSize = new System.Windows.Forms.Label();
            this.numFontSize = new System.Windows.Forms.NumericUpDown();
            this.groupTheme = new System.Windows.Forms.GroupBox();
            this.lblTheme = new System.Windows.Forms.Label();
            this.cmbTheme = new System.Windows.Forms.ComboBox();
            this.txtPreview = new System.Windows.Forms.TextBox();
            this.tabBehavior = new System.Windows.Forms.TabPage();
            this.groupTextOptions = new System.Windows.Forms.GroupBox();
            this.chkWordWrap = new System.Windows.Forms.CheckBox();
            this.chkShowLineNumbers = new System.Windows.Forms.CheckBox();
            this.groupAutoSave = new System.Windows.Forms.GroupBox();
            this.chkAutoSave = new System.Windows.Forms.CheckBox();
            this.lblAutoSaveInterval = new System.Windows.Forms.Label();
            this.numAutoSaveInterval = new System.Windows.Forms.NumericUpDown();
            this.lblSeconds = new System.Windows.Forms.Label();
            this.groupEncoding = new System.Windows.Forms.GroupBox();
            this.lblDefaultEncoding = new System.Windows.Forms.Label();
            this.cmbDefaultEncoding = new System.Windows.Forms.ComboBox();
            this.tabAdvanced = new System.Windows.Forms.TabPage();
            this.groupRecentFiles = new System.Windows.Forms.GroupBox();
            this.lblMaxRecentFiles = new System.Windows.Forms.Label();
            this.numMaxRecentFiles = new System.Windows.Forms.NumericUpDown();
            this.groupInterface = new System.Windows.Forms.GroupBox();
            this.chkShowStatusBar = new System.Windows.Forms.CheckBox();
            this.chkShowToolbar = new System.Windows.Forms.CheckBox();
            this.groupWindow = new System.Windows.Forms.GroupBox();
            this.lblWindowWidth = new System.Windows.Forms.Label();
            this.numWindowWidth = new System.Windows.Forms.NumericUpDown();
            this.lblWindowHeight = new System.Windows.Forms.Label();
            this.numWindowHeight = new System.Windows.Forms.NumericUpDown();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabAppearance.SuspendLayout();
            this.groupFont.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFontSize)).BeginInit();
            this.groupTheme.SuspendLayout();
            this.tabBehavior.SuspendLayout();
            this.groupTextOptions.SuspendLayout();
            this.groupAutoSave.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAutoSaveInterval)).BeginInit();
            this.groupEncoding.SuspendLayout();
            this.tabAdvanced.SuspendLayout();
            this.groupRecentFiles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxRecentFiles)).BeginInit();
            this.groupInterface.SuspendLayout();
            this.groupWindow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numWindowWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWindowHeight)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabAppearance);
            this.tabControl1.Controls.Add(this.tabBehavior);
            this.tabControl1.Controls.Add(this.tabAdvanced);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(560, 400);
            this.tabControl1.TabIndex = 0;
            // 
            // tabAppearance
            // 
            this.tabAppearance.Controls.Add(this.groupFont);
            this.tabAppearance.Controls.Add(this.groupTheme);
            this.tabAppearance.Location = new System.Drawing.Point(4, 25);
            this.tabAppearance.Name = "tabAppearance";
            this.tabAppearance.Padding = new System.Windows.Forms.Padding(3);
            this.tabAppearance.Size = new System.Drawing.Size(552, 371);
            this.tabAppearance.TabIndex = 0;
            this.tabAppearance.Text = "Appearance";
            this.tabAppearance.UseVisualStyleBackColor = true;
            // 
            // groupFont
            // 
            this.groupFont.Controls.Add(this.lblFontFamily);
            this.groupFont.Controls.Add(this.cmbFontFamily);
            this.groupFont.Controls.Add(this.lblFontSize);
            this.groupFont.Controls.Add(this.numFontSize);
            this.groupFont.Location = new System.Drawing.Point(6, 6);
            this.groupFont.Name = "groupFont";
            this.groupFont.Size = new System.Drawing.Size(260, 100);
            this.groupFont.TabIndex = 0;
            this.groupFont.TabStop = false;
            this.groupFont.Text = "Font Settings";
            // 
            // lblFontFamily
            // 
            this.lblFontFamily.AutoSize = true;
            this.lblFontFamily.Location = new System.Drawing.Point(6, 25);
            this.lblFontFamily.Name = "lblFontFamily";
            this.lblFontFamily.Size = new System.Drawing.Size(79, 16);
            this.lblFontFamily.TabIndex = 0;
            this.lblFontFamily.Text = "Font Family:";
            // 
            // cmbFontFamily
            // 
            this.cmbFontFamily.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFontFamily.Location = new System.Drawing.Point(90, 22);
            this.cmbFontFamily.Name = "cmbFontFamily";
            this.cmbFontFamily.Size = new System.Drawing.Size(150, 24);
            this.cmbFontFamily.TabIndex = 1;
            this.cmbFontFamily.SelectedIndexChanged += new System.EventHandler(this.cmbFontFamily_SelectedIndexChanged);
            // 
            // lblFontSize
            // 
            this.lblFontSize.AutoSize = true;
            this.lblFontSize.Location = new System.Drawing.Point(6, 60);
            this.lblFontSize.Name = "lblFontSize";
            this.lblFontSize.Size = new System.Drawing.Size(65, 16);
            this.lblFontSize.TabIndex = 2;
            this.lblFontSize.Text = "Font Size:";
            // 
            // numFontSize
            // 
            this.numFontSize.Location = new System.Drawing.Point(90, 58);
            this.numFontSize.Name = "numFontSize";
            this.numFontSize.Size = new System.Drawing.Size(80, 22);
            this.numFontSize.TabIndex = 3;
            this.numFontSize.ValueChanged += new System.EventHandler(this.numFontSize_ValueChanged);
            // 
            // groupTheme
            // 
            this.groupTheme.Controls.Add(this.lblTheme);
            this.groupTheme.Controls.Add(this.cmbTheme);
            this.groupTheme.Controls.Add(this.txtPreview);
            this.groupTheme.Location = new System.Drawing.Point(280, 6);
            this.groupTheme.Name = "groupTheme";
            this.groupTheme.Size = new System.Drawing.Size(260, 200);
            this.groupTheme.TabIndex = 1;
            this.groupTheme.TabStop = false;
            this.groupTheme.Text = "Theme";
            // 
            // lblTheme
            // 
            this.lblTheme.AutoSize = true;
            this.lblTheme.Location = new System.Drawing.Point(6, 25);
            this.lblTheme.Name = "lblTheme";
            this.lblTheme.Size = new System.Drawing.Size(53, 16);
            this.lblTheme.TabIndex = 0;
            this.lblTheme.Text = "Theme:";
            // 
            // cmbTheme
            // 
            this.cmbTheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTheme.Location = new System.Drawing.Point(70, 22);
            this.cmbTheme.Name = "cmbTheme";
            this.cmbTheme.Size = new System.Drawing.Size(150, 24);
            this.cmbTheme.TabIndex = 1;
            this.cmbTheme.SelectedIndexChanged += new System.EventHandler(this.cmbTheme_SelectedIndexChanged);
            // 
            // txtPreview
            // 
            this.txtPreview.Location = new System.Drawing.Point(9, 60);
            this.txtPreview.Multiline = true;
            this.txtPreview.Name = "txtPreview";
            this.txtPreview.ReadOnly = true;
            this.txtPreview.Size = new System.Drawing.Size(240, 120);
            this.txtPreview.TabIndex = 2;
            this.txtPreview.Text = "Preview text...";
            this.txtPreview.TextChanged += new System.EventHandler(this.txtPreview_TextChanged);
            // 
            // tabBehavior
            // 
            this.tabBehavior.Controls.Add(this.groupTextOptions);
            this.tabBehavior.Controls.Add(this.groupAutoSave);
            this.tabBehavior.Controls.Add(this.groupEncoding);
            this.tabBehavior.Location = new System.Drawing.Point(4, 25);
            this.tabBehavior.Name = "tabBehavior";
            this.tabBehavior.Padding = new System.Windows.Forms.Padding(3);
            this.tabBehavior.Size = new System.Drawing.Size(552, 371);
            this.tabBehavior.TabIndex = 1;
            this.tabBehavior.Text = "Behavior";
            this.tabBehavior.UseVisualStyleBackColor = true;
            // 
            // groupTextOptions
            // 
            this.groupTextOptions.Controls.Add(this.chkWordWrap);
            this.groupTextOptions.Controls.Add(this.chkShowLineNumbers);
            this.groupTextOptions.Location = new System.Drawing.Point(6, 6);
            this.groupTextOptions.Name = "groupTextOptions";
            this.groupTextOptions.Size = new System.Drawing.Size(260, 80);
            this.groupTextOptions.TabIndex = 0;
            this.groupTextOptions.TabStop = false;
            this.groupTextOptions.Text = "Text Options";
            // 
            // chkWordWrap
            // 
            this.chkWordWrap.AutoSize = true;
            this.chkWordWrap.Location = new System.Drawing.Point(15, 25);
            this.chkWordWrap.Name = "chkWordWrap";
            this.chkWordWrap.Size = new System.Drawing.Size(98, 20);
            this.chkWordWrap.TabIndex = 0;
            this.chkWordWrap.Text = "Word Wrap";
            this.chkWordWrap.UseVisualStyleBackColor = true;
            // 
            // chkShowLineNumbers
            // 
            this.chkShowLineNumbers.AutoSize = true;
            this.chkShowLineNumbers.Location = new System.Drawing.Point(15, 50);
            this.chkShowLineNumbers.Name = "chkShowLineNumbers";
            this.chkShowLineNumbers.Size = new System.Drawing.Size(148, 20);
            this.chkShowLineNumbers.TabIndex = 1;
            this.chkShowLineNumbers.Text = "Show Line Numbers";
            this.chkShowLineNumbers.UseVisualStyleBackColor = true;
            // 
            // groupAutoSave
            // 
            this.groupAutoSave.Controls.Add(this.chkAutoSave);
            this.groupAutoSave.Controls.Add(this.lblAutoSaveInterval);
            this.groupAutoSave.Controls.Add(this.numAutoSaveInterval);
            this.groupAutoSave.Controls.Add(this.lblSeconds);
            this.groupAutoSave.Location = new System.Drawing.Point(280, 6);
            this.groupAutoSave.Name = "groupAutoSave";
            this.groupAutoSave.Size = new System.Drawing.Size(260, 80);
            this.groupAutoSave.TabIndex = 1;
            this.groupAutoSave.TabStop = false;
            this.groupAutoSave.Text = "Auto Save";
            // 
            // chkAutoSave
            // 
            this.chkAutoSave.AutoSize = true;
            this.chkAutoSave.Location = new System.Drawing.Point(15, 25);
            this.chkAutoSave.Name = "chkAutoSave";
            this.chkAutoSave.Size = new System.Drawing.Size(134, 20);
            this.chkAutoSave.TabIndex = 0;
            this.chkAutoSave.Text = "Enable AutoSave";
            this.chkAutoSave.UseVisualStyleBackColor = true;
            // 
            // lblAutoSaveInterval
            // 
            this.lblAutoSaveInterval.AutoSize = true;
            this.lblAutoSaveInterval.Location = new System.Drawing.Point(15, 50);
            this.lblAutoSaveInterval.Name = "lblAutoSaveInterval";
            this.lblAutoSaveInterval.Size = new System.Drawing.Size(53, 16);
            this.lblAutoSaveInterval.TabIndex = 1;
            this.lblAutoSaveInterval.Text = "Interval:";
            // 
            // numAutoSaveInterval
            // 
            this.numAutoSaveInterval.Location = new System.Drawing.Point(80, 48);
            this.numAutoSaveInterval.Name = "numAutoSaveInterval";
            this.numAutoSaveInterval.Size = new System.Drawing.Size(80, 22);
            this.numAutoSaveInterval.TabIndex = 2;
            // 
            // lblSeconds
            // 
            this.lblSeconds.AutoSize = true;
            this.lblSeconds.Location = new System.Drawing.Point(170, 50);
            this.lblSeconds.Name = "lblSeconds";
            this.lblSeconds.Size = new System.Drawing.Size(59, 16);
            this.lblSeconds.TabIndex = 3;
            this.lblSeconds.Text = "seconds";
            // 
            // groupEncoding
            // 
            this.groupEncoding.Controls.Add(this.lblDefaultEncoding);
            this.groupEncoding.Controls.Add(this.cmbDefaultEncoding);
            this.groupEncoding.Location = new System.Drawing.Point(6, 100);
            this.groupEncoding.Name = "groupEncoding";
            this.groupEncoding.Size = new System.Drawing.Size(260, 60);
            this.groupEncoding.TabIndex = 2;
            this.groupEncoding.TabStop = false;
            this.groupEncoding.Text = "Default Encoding";
            // 
            // lblDefaultEncoding
            // 
            this.lblDefaultEncoding.AutoSize = true;
            this.lblDefaultEncoding.Location = new System.Drawing.Point(6, 25);
            this.lblDefaultEncoding.Name = "lblDefaultEncoding";
            this.lblDefaultEncoding.Size = new System.Drawing.Size(67, 16);
            this.lblDefaultEncoding.TabIndex = 0;
            this.lblDefaultEncoding.Text = "Encoding:";
            // 
            // cmbDefaultEncoding
            // 
            this.cmbDefaultEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDefaultEncoding.Location = new System.Drawing.Point(90, 22);
            this.cmbDefaultEncoding.Name = "cmbDefaultEncoding";
            this.cmbDefaultEncoding.Size = new System.Drawing.Size(150, 24);
            this.cmbDefaultEncoding.TabIndex = 1;
            // 
            // tabAdvanced
            // 
            this.tabAdvanced.Controls.Add(this.groupRecentFiles);
            this.tabAdvanced.Controls.Add(this.groupInterface);
            this.tabAdvanced.Controls.Add(this.groupWindow);
            this.tabAdvanced.Location = new System.Drawing.Point(4, 25);
            this.tabAdvanced.Name = "tabAdvanced";
            this.tabAdvanced.Size = new System.Drawing.Size(552, 371);
            this.tabAdvanced.TabIndex = 2;
            this.tabAdvanced.Text = "Advanced";
            this.tabAdvanced.UseVisualStyleBackColor = true;
            // 
            // groupRecentFiles
            // 
            this.groupRecentFiles.Controls.Add(this.lblMaxRecentFiles);
            this.groupRecentFiles.Controls.Add(this.numMaxRecentFiles);
            this.groupRecentFiles.Location = new System.Drawing.Point(6, 6);
            this.groupRecentFiles.Name = "groupRecentFiles";
            this.groupRecentFiles.Size = new System.Drawing.Size(260, 60);
            this.groupRecentFiles.TabIndex = 0;
            this.groupRecentFiles.TabStop = false;
            this.groupRecentFiles.Text = "Recent Files";
            // 
            // lblMaxRecentFiles
            // 
            this.lblMaxRecentFiles.AutoSize = true;
            this.lblMaxRecentFiles.Location = new System.Drawing.Point(6, 25);
            this.lblMaxRecentFiles.Name = "lblMaxRecentFiles";
            this.lblMaxRecentFiles.Size = new System.Drawing.Size(113, 16);
            this.lblMaxRecentFiles.TabIndex = 0;
            this.lblMaxRecentFiles.Text = "Max Recent Files:";
            // 
            // numMaxRecentFiles
            // 
            this.numMaxRecentFiles.Location = new System.Drawing.Point(130, 23);
            this.numMaxRecentFiles.Name = "numMaxRecentFiles";
            this.numMaxRecentFiles.Size = new System.Drawing.Size(80, 22);
            this.numMaxRecentFiles.TabIndex = 1;
            // 
            // groupInterface
            // 
            this.groupInterface.Controls.Add(this.chkShowStatusBar);
            this.groupInterface.Controls.Add(this.chkShowToolbar);
            this.groupInterface.Location = new System.Drawing.Point(280, 6);
            this.groupInterface.Name = "groupInterface";
            this.groupInterface.Size = new System.Drawing.Size(260, 80);
            this.groupInterface.TabIndex = 1;
            this.groupInterface.TabStop = false;
            this.groupInterface.Text = "Interface";
            // 
            // chkShowStatusBar
            // 
            this.chkShowStatusBar.AutoSize = true;
            this.chkShowStatusBar.Location = new System.Drawing.Point(15, 25);
            this.chkShowStatusBar.Name = "chkShowStatusBar";
            this.chkShowStatusBar.Size = new System.Drawing.Size(126, 20);
            this.chkShowStatusBar.TabIndex = 0;
            this.chkShowStatusBar.Text = "Show Status Bar";
            this.chkShowStatusBar.UseVisualStyleBackColor = true;
            // 
            // chkShowToolbar
            // 
            this.chkShowToolbar.AutoSize = true;
            this.chkShowToolbar.Location = new System.Drawing.Point(15, 50);
            this.chkShowToolbar.Name = "chkShowToolbar";
            this.chkShowToolbar.Size = new System.Drawing.Size(113, 20);
            this.chkShowToolbar.TabIndex = 1;
            this.chkShowToolbar.Text = "Show Toolbar";
            this.chkShowToolbar.UseVisualStyleBackColor = true;
            // 
            // groupWindow
            // 
            this.groupWindow.Controls.Add(this.lblWindowWidth);
            this.groupWindow.Controls.Add(this.numWindowWidth);
            this.groupWindow.Controls.Add(this.lblWindowHeight);
            this.groupWindow.Controls.Add(this.numWindowHeight);
            this.groupWindow.Location = new System.Drawing.Point(6, 80);
            this.groupWindow.Name = "groupWindow";
            this.groupWindow.Size = new System.Drawing.Size(260, 80);
            this.groupWindow.TabIndex = 2;
            this.groupWindow.TabStop = false;
            this.groupWindow.Text = "Default Window Size";
            // 
            // lblWindowWidth
            // 
            this.lblWindowWidth.AutoSize = true;
            this.lblWindowWidth.Location = new System.Drawing.Point(6, 25);
            this.lblWindowWidth.Name = "lblWindowWidth";
            this.lblWindowWidth.Size = new System.Drawing.Size(44, 16);
            this.lblWindowWidth.TabIndex = 0;
            this.lblWindowWidth.Text = "Width:";
            // 
            // numWindowWidth
            // 
            this.numWindowWidth.Location = new System.Drawing.Point(70, 23);
            this.numWindowWidth.Name = "numWindowWidth";
            this.numWindowWidth.Size = new System.Drawing.Size(80, 22);
            this.numWindowWidth.TabIndex = 1;
            // 
            // lblWindowHeight
            // 
            this.lblWindowHeight.AutoSize = true;
            this.lblWindowHeight.Location = new System.Drawing.Point(6, 50);
            this.lblWindowHeight.Name = "lblWindowHeight";
            this.lblWindowHeight.Size = new System.Drawing.Size(49, 16);
            this.lblWindowHeight.TabIndex = 2;
            this.lblWindowHeight.Text = "Height:";
            // 
            // numWindowHeight
            // 
            this.numWindowHeight.Location = new System.Drawing.Point(70, 48);
            this.numWindowHeight.Name = "numWindowHeight";
            this.numWindowHeight.Size = new System.Drawing.Size(80, 22);
            this.numWindowHeight.TabIndex = 3;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(320, 425);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(80, 30);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(410, 425);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 30);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnReset
            // 
            this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReset.Location = new System.Drawing.Point(500, 425);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(80, 30);
            this.btnReset.TabIndex = 3;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 467);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Editor Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabAppearance.ResumeLayout(false);
            this.groupFont.ResumeLayout(false);
            this.groupFont.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFontSize)).EndInit();
            this.groupTheme.ResumeLayout(false);
            this.groupTheme.PerformLayout();
            this.tabBehavior.ResumeLayout(false);
            this.groupTextOptions.ResumeLayout(false);
            this.groupTextOptions.PerformLayout();
            this.groupAutoSave.ResumeLayout(false);
            this.groupAutoSave.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAutoSaveInterval)).EndInit();
            this.groupEncoding.ResumeLayout(false);
            this.groupEncoding.PerformLayout();
            this.tabAdvanced.ResumeLayout(false);
            this.groupRecentFiles.ResumeLayout(false);
            this.groupRecentFiles.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxRecentFiles)).EndInit();
            this.groupInterface.ResumeLayout(false);
            this.groupInterface.PerformLayout();
            this.groupWindow.ResumeLayout(false);
            this.groupWindow.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numWindowWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWindowHeight)).EndInit();
            this.ResumeLayout(false);

        }
    }
}