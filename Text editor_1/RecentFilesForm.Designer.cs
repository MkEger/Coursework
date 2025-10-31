using System;
using System.Windows.Forms;

namespace Text_editor_1
{
    partial class RecentFilesForm
    {
        private System.ComponentModel.IContainer components = null;
        private ListView listViewRecentFiles;
        private Button btnOpen;
        private Button btnCancel;
        private Button btnClearHistory;
        private Label lblTitle;

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
            this.listViewRecentFiles = new System.Windows.Forms.ListView();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnClearHistory = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listViewRecentFiles
            // 
            this.listViewRecentFiles.FullRowSelect = true;
            this.listViewRecentFiles.GridLines = true;
            this.listViewRecentFiles.HideSelection = false;
            this.listViewRecentFiles.Location = new System.Drawing.Point(16, 49);
            this.listViewRecentFiles.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.listViewRecentFiles.Name = "listViewRecentFiles";
            this.listViewRecentFiles.Size = new System.Drawing.Size(745, 368);
            this.listViewRecentFiles.TabIndex = 1;
            this.listViewRecentFiles.UseCompatibleStateImageBehavior = false;
            this.listViewRecentFiles.View = System.Windows.Forms.View.Details;
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(396, 443);
            this.btnOpen.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(120, 37);
            this.btnOpen.TabIndex = 2;
            this.btnOpen.Text = "Open";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(524, 443);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(120, 37);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnClearHistory
            // 
            this.btnClearHistory.Location = new System.Drawing.Point(16, 443);
            this.btnClearHistory.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnClearHistory.Name = "btnClearHistory";
            this.btnClearHistory.Size = new System.Drawing.Size(160, 37);
            this.btnClearHistory.TabIndex = 4;
            this.btnClearHistory.Text = "Clear History";
            this.btnClearHistory.UseVisualStyleBackColor = true;
            this.btnClearHistory.Click += new System.EventHandler(this.btnClearHistory_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(16, 11);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(131, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Recent Files";
            // 
            // RecentFilesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(779, 506);
            this.Controls.Add(this.btnClearHistory);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.listViewRecentFiles);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RecentFilesForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Recent Files";
            this.Load += new System.EventHandler(this.RecentFilesForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}