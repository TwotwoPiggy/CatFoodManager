namespace CatFoodManager.Controls
{
    partial class SearchUserControl
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

        private void InitializeComponent()
        {
            searchLabel = new Label();
            searchTextBox = new TextBox();
            searchButton = new Button();
            resetButton = new Button();
            tableLayoutPanel = new TableLayoutPanel();
            tableLayoutPanel.SuspendLayout();
            SuspendLayout();
            
            tableLayoutPanel.ColumnCount = 4;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            tableLayoutPanel.Controls.Add(searchLabel, 0, 0);
            tableLayoutPanel.Controls.Add(searchTextBox, 1, 0);
            tableLayoutPanel.Controls.Add(searchButton, 2, 0);
            tableLayoutPanel.Controls.Add(resetButton, 3, 0);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 1;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel.Size = new System.Drawing.Size(400, 35);
            tableLayoutPanel.TabIndex = 0;
            
            searchLabel.Anchor = AnchorStyles.Right;
            searchLabel.AutoSize = true;
            searchLabel.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold);
            searchLabel.Location = new System.Drawing.Point(8, 6);
            searchLabel.Name = "searchLabel";
            searchLabel.Size = new System.Drawing.Size(47, 22);
            searchLabel.TabIndex = 0;
            searchLabel.Text = "检索:";
            searchLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            
            searchTextBox.Dock = DockStyle.Fill;
            searchTextBox.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F);
            searchTextBox.Location = new System.Drawing.Point(62, 1);
            searchTextBox.Margin = new Padding(1);
            searchTextBox.Name = "searchTextBox";
            searchTextBox.Size = new System.Drawing.Size(174, 30);
            searchTextBox.TabIndex = 1;
            searchTextBox.TextChanged += searchTextBox_TextChanged;
            searchTextBox.KeyDown += searchTextBox_KeyDown;
            
            searchButton.Dock = DockStyle.Fill;
            searchButton.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold);
            searchButton.Location = new System.Drawing.Point(240, 1);
            searchButton.Margin = new Padding(1);
            searchButton.Name = "searchButton";
            searchButton.Size = new System.Drawing.Size(76, 33);
            searchButton.TabIndex = 2;
            searchButton.Text = "搜索";
            searchButton.UseVisualStyleBackColor = true;
            searchButton.Click += searchButton_Click;
            
            resetButton.Dock = DockStyle.Fill;
            resetButton.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold);
            resetButton.Location = new System.Drawing.Point(320, 1);
            resetButton.Margin = new Padding(1);
            resetButton.Name = "resetButton";
            resetButton.Size = new System.Drawing.Size(76, 33);
            resetButton.TabIndex = 3;
            resetButton.Text = "重置";
            resetButton.UseVisualStyleBackColor = true;
            resetButton.Click += resetButton_Click;
            
            Controls.Add(tableLayoutPanel);
            Name = "SearchUserControl";
            Size = new System.Drawing.Size(400, 35);
            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            ResumeLayout(false);
        }

        private Label searchLabel;
        private TextBox searchTextBox;
        private Button searchButton;
        private Button resetButton;
        private TableLayoutPanel tableLayoutPanel;
    }
}
