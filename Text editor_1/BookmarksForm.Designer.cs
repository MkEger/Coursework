namespace Text_editor_1
{
    partial class BookmarksForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.bookmarksListView = new System.Windows.Forms.ListView();
            this.lineColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.descriptionColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.timeColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.goToButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.clearAllButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.refreshButton = new System.Windows.Forms.Button();
            this.statusLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // bookmarksListView
            // 
            this.bookmarksListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.lineColumnHeader,
            this.descriptionColumnHeader,
            this.timeColumnHeader});
            this.bookmarksListView.FullRowSelect = true;
            this.bookmarksListView.GridLines = true;
            this.bookmarksListView.HideSelection = false;
            this.bookmarksListView.Location = new System.Drawing.Point(12, 12);
            this.bookmarksListView.MultiSelect = false;
            this.bookmarksListView.Name = "bookmarksListView";
            this.bookmarksListView.Size = new System.Drawing.Size(460, 280);
            this.bookmarksListView.TabIndex = 0;
            this.bookmarksListView.UseCompatibleStateImageBehavior = false;
            this.bookmarksListView.View = System.Windows.Forms.View.Details;
            this.bookmarksListView.SelectedIndexChanged += new System.EventHandler(this.bookmarksListView_SelectedIndexChanged);
            this.bookmarksListView.DoubleClick += new System.EventHandler(this.bookmarksListView_DoubleClick);
            // 
            // lineColumnHeader
            // 
            this.lineColumnHeader.Text = "Line";
            this.lineColumnHeader.Width = 60;
            // 
            // descriptionColumnHeader
            // 
            this.descriptionColumnHeader.Text = "Description";
            this.descriptionColumnHeader.Width = 280;
            // 
            // timeColumnHeader
            // 
            this.timeColumnHeader.Text = "Created";
            this.timeColumnHeader.Width = 100;
            // 
            // goToButton
            // 
            this.goToButton.Location = new System.Drawing.Point(490, 12);
            this.goToButton.Name = "goToButton";
            this.goToButton.Size = new System.Drawing.Size(100, 30);
            this.goToButton.TabIndex = 1;
            this.goToButton.Text = "&Go To";
            this.goToButton.UseVisualStyleBackColor = true;
            this.goToButton.Click += new System.EventHandler(this.goToButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(490, 50);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(100, 30);
            this.deleteButton.TabIndex = 2;
            this.deleteButton.Text = "&Delete";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // clearAllButton
            // 
            this.clearAllButton.Location = new System.Drawing.Point(490, 88);
            this.clearAllButton.Name = "clearAllButton";
            this.clearAllButton.Size = new System.Drawing.Size(100, 30);
            this.clearAllButton.TabIndex = 3;
            this.clearAllButton.Text = "&Clear All";
            this.clearAllButton.UseVisualStyleBackColor = true;
            this.clearAllButton.Click += new System.EventHandler(this.clearAllButton_Click);
            // 
            // closeButton
            // 
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Location = new System.Drawing.Point(490, 262);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(100, 30);
            this.closeButton.TabIndex = 5;
            this.closeButton.Text = "&Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // refreshButton
            // 
            this.refreshButton.Location = new System.Drawing.Point(490, 126);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(100, 30);
            this.refreshButton.TabIndex = 4;
            this.refreshButton.Text = "&Refresh";
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.statusLabel.Location = new System.Drawing.Point(12, 300);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(86, 17);
            this.statusLabel.TabIndex = 6;
            this.statusLabel.Text = "0 bookmark(s)";
            // 
            // BookmarksForm
            // 
            this.AcceptButton = this.goToButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(604, 330);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.refreshButton);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.clearAllButton);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.goToButton);
            this.Controls.Add(this.bookmarksListView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BookmarksForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Bookmarks";
            this.Load += new System.EventHandler(this.BookmarksForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ListView bookmarksListView;
        private System.Windows.Forms.ColumnHeader lineColumnHeader;
        private System.Windows.Forms.ColumnHeader descriptionColumnHeader;
        private System.Windows.Forms.ColumnHeader timeColumnHeader;
        private System.Windows.Forms.Button goToButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button clearAllButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.Label statusLabel;
    }
}