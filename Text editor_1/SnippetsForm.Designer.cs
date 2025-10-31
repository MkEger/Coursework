namespace Text_editor_1
{
    partial class SnippetsForm
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
            this.snippetsListView = new System.Windows.Forms.ListView();
            this.nameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.triggerColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.languageColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.categoryColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.usageColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.insertButton = new System.Windows.Forms.Button();
            this.newButton = new System.Windows.Forms.Button();
            this.editButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.languageComboBox = new System.Windows.Forms.ComboBox();
            this.languageLabel = new System.Windows.Forms.Label();
            this.previewGroupBox = new System.Windows.Forms.GroupBox();
            this.previewTextBox = new System.Windows.Forms.TextBox();
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.previewGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // snippetsListView
            // 
            this.snippetsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameColumnHeader,
            this.triggerColumnHeader,
            this.languageColumnHeader,
            this.categoryColumnHeader,
            this.usageColumnHeader});
            this.snippetsListView.FullRowSelect = true;
            this.snippetsListView.GridLines = true;
            this.snippetsListView.HideSelection = false;
            this.snippetsListView.Location = new System.Drawing.Point(12, 45);
            this.snippetsListView.MultiSelect = false;
            this.snippetsListView.Name = "snippetsListView";
            this.snippetsListView.Size = new System.Drawing.Size(480, 200);
            this.snippetsListView.TabIndex = 0;
            this.snippetsListView.UseCompatibleStateImageBehavior = false;
            this.snippetsListView.View = System.Windows.Forms.View.Details;
            this.snippetsListView.SelectedIndexChanged += new System.EventHandler(this.snippetsListView_SelectedIndexChanged);
            this.snippetsListView.DoubleClick += new System.EventHandler(this.snippetsListView_DoubleClick);
            // 
            // nameColumnHeader
            // 
            this.nameColumnHeader.Text = "Name";
            this.nameColumnHeader.Width = 120;
            // 
            // triggerColumnHeader
            // 
            this.triggerColumnHeader.Text = "Trigger";
            this.triggerColumnHeader.Width = 80;
            // 
            // languageColumnHeader
            // 
            this.languageColumnHeader.Text = "Language";
            this.languageColumnHeader.Width = 80;
            // 
            // categoryColumnHeader
            // 
            this.categoryColumnHeader.Text = "Category";
            this.categoryColumnHeader.Width = 100;
            // 
            // usageColumnHeader
            // 
            this.usageColumnHeader.Text = "Usage";
            this.usageColumnHeader.Width = 60;
            // 
            // insertButton
            // 
            this.insertButton.Location = new System.Drawing.Point(510, 45);
            this.insertButton.Name = "insertButton";
            this.insertButton.Size = new System.Drawing.Size(100, 30);
            this.insertButton.TabIndex = 1;
            this.insertButton.Text = "&Insert";
            this.insertButton.UseVisualStyleBackColor = true;
            this.insertButton.Click += new System.EventHandler(this.insertButton_Click);
            // 
            // newButton
            // 
            this.newButton.Location = new System.Drawing.Point(510, 83);
            this.newButton.Name = "newButton";
            this.newButton.Size = new System.Drawing.Size(100, 30);
            this.newButton.TabIndex = 2;
            this.newButton.Text = "&New";
            this.newButton.UseVisualStyleBackColor = true;
            this.newButton.Click += new System.EventHandler(this.newButton_Click);
            // 
            // editButton
            // 
            this.editButton.Location = new System.Drawing.Point(510, 121);
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(100, 30);
            this.editButton.TabIndex = 3;
            this.editButton.Text = "&Edit";
            this.editButton.UseVisualStyleBackColor = true;
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(510, 159);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(100, 30);
            this.deleteButton.TabIndex = 4;
            this.deleteButton.Text = "&Delete";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // closeButton
            // 
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Location = new System.Drawing.Point(510, 215);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(100, 30);
            this.closeButton.TabIndex = 5;
            this.closeButton.Text = "&Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // languageComboBox
            // 
            this.languageComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.languageComboBox.Location = new System.Drawing.Point(80, 15);
            this.languageComboBox.Name = "languageComboBox";
            this.languageComboBox.Size = new System.Drawing.Size(150, 24);
            this.languageComboBox.TabIndex = 6;
            this.languageComboBox.SelectedIndexChanged += new System.EventHandler(this.languageComboBox_SelectedIndexChanged);
            // 
            // languageLabel
            // 
            this.languageLabel.AutoSize = true;
            this.languageLabel.Location = new System.Drawing.Point(12, 18);
            this.languageLabel.Name = "languageLabel";
            this.languageLabel.Size = new System.Drawing.Size(77, 17);
            this.languageLabel.TabIndex = 7;
            this.languageLabel.Text = "Language:";
            // 
            // previewGroupBox
            // 
            this.previewGroupBox.Controls.Add(this.previewTextBox);
            this.previewGroupBox.Controls.Add(this.descriptionLabel);
            this.previewGroupBox.Location = new System.Drawing.Point(12, 255);
            this.previewGroupBox.Name = "previewGroupBox";
            this.previewGroupBox.Size = new System.Drawing.Size(598, 160);
            this.previewGroupBox.TabIndex = 8;
            this.previewGroupBox.TabStop = false;
            this.previewGroupBox.Text = "Preview";
            // 
            // previewTextBox
            // 
            this.previewTextBox.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.previewTextBox.Location = new System.Drawing.Point(10, 45);
            this.previewTextBox.Multiline = true;
            this.previewTextBox.Name = "previewTextBox";
            this.previewTextBox.ReadOnly = true;
            this.previewTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.previewTextBox.Size = new System.Drawing.Size(578, 105);
            this.previewTextBox.TabIndex = 1;
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.Location = new System.Drawing.Point(10, 20);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(578, 20);
            this.descriptionLabel.TabIndex = 0;
            this.descriptionLabel.Text = "Select a snippet to see preview";
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.statusLabel.Location = new System.Drawing.Point(12, 425);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(81, 17);
            this.statusLabel.TabIndex = 9;
            this.statusLabel.Text = "0 snippet(s)";
            // 
            // SnippetsForm
            // 
            this.AcceptButton = this.insertButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(620, 450);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.previewGroupBox);
            this.Controls.Add(this.languageLabel);
            this.Controls.Add(this.languageComboBox);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.editButton);
            this.Controls.Add(this.newButton);
            this.Controls.Add(this.insertButton);
            this.Controls.Add(this.snippetsListView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SnippetsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Snippets";
            this.Load += new System.EventHandler(this.SnippetsForm_Load);
            this.previewGroupBox.ResumeLayout(false);
            this.previewGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ListView snippetsListView;
        private System.Windows.Forms.ColumnHeader nameColumnHeader;
        private System.Windows.Forms.ColumnHeader triggerColumnHeader;
        private System.Windows.Forms.ColumnHeader languageColumnHeader;
        private System.Windows.Forms.ColumnHeader categoryColumnHeader;
        private System.Windows.Forms.ColumnHeader usageColumnHeader;
        private System.Windows.Forms.Button insertButton;
        private System.Windows.Forms.Button newButton;
        private System.Windows.Forms.Button editButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.ComboBox languageComboBox;
        private System.Windows.Forms.Label languageLabel;
        private System.Windows.Forms.GroupBox previewGroupBox;
        private System.Windows.Forms.TextBox previewTextBox;
        private System.Windows.Forms.Label descriptionLabel;
        private System.Windows.Forms.Label statusLabel;
    }
}