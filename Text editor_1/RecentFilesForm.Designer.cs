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
            this.listViewRecentFiles = new ListView();
            this.btnOpen = new Button();
            this.btnCancel = new Button();
            this.btnClearHistory = new Button();
            this.lblTitle = new Label();
            this.SuspendLayout();
            
            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(12, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(161, 20);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Нещодавні файли";
            
            // listViewRecentFiles
            this.listViewRecentFiles.View = View.Details;
            this.listViewRecentFiles.FullRowSelect = true;
            this.listViewRecentFiles.GridLines = true;
            this.listViewRecentFiles.Location = new System.Drawing.Point(12, 40);
            this.listViewRecentFiles.Name = "listViewRecentFiles";
            this.listViewRecentFiles.Size = new System.Drawing.Size(560, 300);
            this.listViewRecentFiles.TabIndex = 1;
            this.listViewRecentFiles.UseCompatibleStateImageBehavior = false;
            
            this.listViewRecentFiles.Columns.Add("Ім'я файлу", 150);
            this.listViewRecentFiles.Columns.Add("Шлях", 250);
            this.listViewRecentFiles.Columns.Add("Відкрито", 100);
            this.listViewRecentFiles.Columns.Add("К-сть", 60);
            
            // btnOpen
            this.btnOpen.Location = new System.Drawing.Point(297, 360);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(90, 30);
            this.btnOpen.TabIndex = 2;
            this.btnOpen.Text = "Відкрити";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            
            // btnCancel
            this.btnCancel.Location = new System.Drawing.Point(393, 360);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 30);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Скасувати";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            
            // btnClearHistory
            this.btnClearHistory.Location = new System.Drawing.Point(12, 360);
            this.btnClearHistory.Name = "btnClearHistory";
            this.btnClearHistory.Size = new System.Drawing.Size(120, 30);
            this.btnClearHistory.TabIndex = 4;
            this.btnClearHistory.Text = "Очистити історію";
            this.btnClearHistory.UseVisualStyleBackColor = true;
            this.btnClearHistory.Click += new System.EventHandler(this.btnClearHistory_Click);
            
            // RecentFilesForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 411);
            this.Controls.Add(this.btnClearHistory);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.listViewRecentFiles);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RecentFilesForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Нещодавні файли";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}