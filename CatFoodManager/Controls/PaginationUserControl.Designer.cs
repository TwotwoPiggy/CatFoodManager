namespace CatFoodManager.Controls
{
    partial class PaginationUserControl
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
            tableLayoutPanel = new TableLayoutPanel();
            totalLabel = new Label();
            pageInfoLabel = new Label();
            pageSizeHeaderLabel = new Label();
            pageSizeComboBox = new ComboBox();
            pageSizeLineLabel = new Label();
            firstButton = new Button();
            previousButton = new Button();
            nextButton = new Button();
            lastButton = new Button();
            goToPageButton = new Button();
            jumpPageTextBox = new TextBox();
            tableLayoutPanel.SuspendLayout();
            SuspendLayout();
            
            tableLayoutPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            tableLayoutPanel.ColumnCount = 12;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 103F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 103F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 25F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 15F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 65F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 65F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 65F));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel.Controls.Add(totalLabel, 0, 0);
            tableLayoutPanel.Controls.Add(pageInfoLabel, 1, 0);
            tableLayoutPanel.Controls.Add(pageSizeHeaderLabel, 2, 0);
            tableLayoutPanel.Controls.Add(pageSizeComboBox, 3, 0);
            tableLayoutPanel.Controls.Add(pageSizeLineLabel, 4, 0);
            tableLayoutPanel.Controls.Add(firstButton, 6, 0);
            tableLayoutPanel.Controls.Add(previousButton, 7, 0);
            tableLayoutPanel.Controls.Add(nextButton, 8, 0);
            tableLayoutPanel.Controls.Add(lastButton, 9, 0);
            tableLayoutPanel.Controls.Add(goToPageButton, 10, 0);
            tableLayoutPanel.Controls.Add(jumpPageTextBox, 11, 0);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 1;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel.Size = new System.Drawing.Size(800, 35);
            tableLayoutPanel.TabIndex = 0;
            
            totalLabel.AutoSize = true;
            totalLabel.Dock = DockStyle.Fill;
            totalLabel.Location = new System.Drawing.Point(2, 2);
            totalLabel.Margin = new Padding(1);
            totalLabel.Name = "totalLabel";
            totalLabel.Size = new System.Drawing.Size(101, 30);
            totalLabel.TabIndex = 0;
            totalLabel.Text = "共 0 条记录";
            totalLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            
            pageInfoLabel.AutoSize = true;
            pageInfoLabel.Dock = DockStyle.Fill;
            pageInfoLabel.Location = new System.Drawing.Point(106, 2);
            pageInfoLabel.Margin = new Padding(1);
            pageInfoLabel.Name = "pageInfoLabel";
            pageInfoLabel.Size = new System.Drawing.Size(101, 30);
            pageInfoLabel.TabIndex = 1;
            pageInfoLabel.Text = "当前页 1/1";
            pageInfoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            
            pageSizeHeaderLabel.AutoSize = true;
            pageSizeHeaderLabel.Dock = DockStyle.Fill;
            pageSizeHeaderLabel.Location = new System.Drawing.Point(210, 2);
            pageSizeHeaderLabel.Margin = new Padding(1);
            pageSizeHeaderLabel.Name = "pageSizeHeaderLabel";
            pageSizeHeaderLabel.Size = new System.Drawing.Size(38, 30);
            pageSizeHeaderLabel.TabIndex = 2;
            pageSizeHeaderLabel.Text = "每页";
            pageSizeHeaderLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            
            pageSizeComboBox.Anchor = AnchorStyles.None;
            pageSizeComboBox.FormattingEnabled = true;
            pageSizeComboBox.Items.AddRange(new object[] { "10", "20", "50", "100" });
            pageSizeComboBox.Location = new System.Drawing.Point(251, 5);
            pageSizeComboBox.Margin = new Padding(1);
            pageSizeComboBox.Name = "pageSizeComboBox";
            pageSizeComboBox.Size = new System.Drawing.Size(55, 25);
            pageSizeComboBox.TabIndex = 3;
            pageSizeComboBox.SelectedIndexChanged += pageSizeComboBox_SelectedIndexChanged;
            
            pageSizeLineLabel.AutoSize = true;
            pageSizeLineLabel.Dock = DockStyle.Fill;
            pageSizeLineLabel.Location = new System.Drawing.Point(312, 2);
            pageSizeLineLabel.Margin = new Padding(1);
            pageSizeLineLabel.Name = "pageSizeLineLabel";
            pageSizeLineLabel.Size = new System.Drawing.Size(23, 30);
            pageSizeLineLabel.TabIndex = 4;
            pageSizeLineLabel.Text = "行";
            pageSizeLineLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            
            firstButton.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold);
            firstButton.Location = new System.Drawing.Point(352, 4);
            firstButton.Name = "firstButton";
            firstButton.Size = new System.Drawing.Size(44, 26);
            firstButton.TabIndex = 5;
            firstButton.Text = "首页";
            firstButton.UseVisualStyleBackColor = true;
            firstButton.Click += firstButton_Click;
            
            previousButton.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold);
            previousButton.Location = new System.Drawing.Point(404, 4);
            previousButton.Name = "previousButton";
            previousButton.Size = new System.Drawing.Size(59, 26);
            previousButton.TabIndex = 6;
            previousButton.Text = "上一页";
            previousButton.UseVisualStyleBackColor = true;
            previousButton.Click += previousButton_Click;
            
            nextButton.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold);
            nextButton.Location = new System.Drawing.Point(471, 4);
            nextButton.Name = "nextButton";
            nextButton.Size = new System.Drawing.Size(59, 26);
            nextButton.TabIndex = 7;
            nextButton.Text = "下一页";
            nextButton.UseVisualStyleBackColor = true;
            nextButton.Click += nextButton_Click;
            
            lastButton.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold);
            lastButton.Location = new System.Drawing.Point(538, 4);
            lastButton.Name = "lastButton";
            lastButton.Size = new System.Drawing.Size(44, 26);
            lastButton.TabIndex = 8;
            lastButton.Text = "末页";
            lastButton.UseVisualStyleBackColor = true;
            lastButton.Click += lastButton_Click;
            
            goToPageButton.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold);
            goToPageButton.Location = new System.Drawing.Point(590, 4);
            goToPageButton.Name = "goToPageButton";
            goToPageButton.Size = new System.Drawing.Size(59, 26);
            goToPageButton.TabIndex = 9;
            goToPageButton.Text = "跳转到";
            goToPageButton.UseVisualStyleBackColor = true;
            goToPageButton.Click += goToPageButton_Click;
            
            jumpPageTextBox.Dock = DockStyle.Fill;
            jumpPageTextBox.Location = new System.Drawing.Point(657, 4);
            jumpPageTextBox.Margin = new Padding(1);
            jumpPageTextBox.Name = "jumpPageTextBox";
            jumpPageTextBox.Size = new System.Drawing.Size(138, 26);
            jumpPageTextBox.TabIndex = 10;
            
            Controls.Add(tableLayoutPanel);
            Name = "PaginationUserControl";
            Size = new System.Drawing.Size(800, 35);
            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel.PerformLayout();
            ResumeLayout(false);
        }

        private TableLayoutPanel tableLayoutPanel;
        private Label totalLabel;
        private Label pageInfoLabel;
        private Label pageSizeHeaderLabel;
        private ComboBox pageSizeComboBox;
        private Label pageSizeLineLabel;
        private Button firstButton;
        private Button previousButton;
        private Button nextButton;
        private Button lastButton;
        private Button goToPageButton;
        private TextBox jumpPageTextBox;
    }
}
