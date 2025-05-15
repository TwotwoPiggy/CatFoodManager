namespace CatFoodManager
{
	partial class BrandManager
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
			tableLayoutPanel1 = new TableLayoutPanel();
			searchLayoutPanel = new TableLayoutPanel();
			searchLabel = new Label();
			searchText = new TextBox();
			searchBtn = new Button();
			dataView = new DataGridView();
			tableLayoutPanel1.SuspendLayout();
			searchLayoutPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)dataView).BeginInit();
			SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			tableLayoutPanel1.ColumnCount = 1;
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
			tableLayoutPanel1.Controls.Add(searchLayoutPanel, 0, 0);
			tableLayoutPanel1.Controls.Add(dataView, 0, 1);
			tableLayoutPanel1.Dock = DockStyle.Fill;
			tableLayoutPanel1.Location = new Point(0, 0);
			tableLayoutPanel1.Name = "tableLayoutPanel1";
			tableLayoutPanel1.RowCount = 2;
			tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 8.439898F));
			tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 91.5601044F));
			tableLayoutPanel1.Size = new Size(348, 393);
			tableLayoutPanel1.TabIndex = 0;
			// 
			// searchLayoutPanel
			// 
			searchLayoutPanel.ColumnCount = 3;
			searchLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
			searchLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 211F));
			searchLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 453F));
			searchLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
			searchLayoutPanel.Controls.Add(searchLabel, 0, 0);
			searchLayoutPanel.Controls.Add(searchText, 1, 0);
			searchLayoutPanel.Controls.Add(searchBtn, 2, 0);
			searchLayoutPanel.Dock = DockStyle.Fill;
			searchLayoutPanel.Location = new Point(0, 0);
			searchLayoutPanel.Margin = new Padding(0);
			searchLayoutPanel.Name = "searchLayoutPanel";
			searchLayoutPanel.RowCount = 1;
			searchLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			searchLayoutPanel.Size = new Size(348, 33);
			searchLayoutPanel.TabIndex = 1;
			// 
			// searchLabel
			// 
			searchLabel.Anchor = AnchorStyles.Right;
			searchLabel.AutoSize = true;
			searchLabel.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
			searchLabel.Location = new Point(23, 5);
			searchLabel.Margin = new Padding(0);
			searchLabel.Name = "searchLabel";
			searchLabel.Size = new Size(47, 22);
			searchLabel.TabIndex = 0;
			searchLabel.Text = "检索:";
			searchLabel.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// searchText
			// 
			searchText.Dock = DockStyle.Fill;
			searchText.Font = new Font("Microsoft YaHei UI", 13F, FontStyle.Bold);
			searchText.Location = new Point(71, 1);
			searchText.Margin = new Padding(1);
			searchText.Name = "searchText";
			searchText.Size = new Size(209, 30);
			searchText.TabIndex = 1;
			searchText.TextChanged += searchText_TextChanged;
			searchText.KeyDown += searchText_KeyDown;
			// 
			// searchBtn
			// 
			searchBtn.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
			searchBtn.Location = new Point(282, 1);
			searchBtn.Margin = new Padding(1);
			searchBtn.Name = "searchBtn";
			searchBtn.Size = new Size(61, 31);
			searchBtn.TabIndex = 2;
			searchBtn.Text = "搜索";
			searchBtn.UseVisualStyleBackColor = true;
			searchBtn.Click += searchBtn_Click;
			// 
			// dataView
			// 
			dataView.AllowUserToDeleteRows = false;
			dataView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dataView.Dock = DockStyle.Fill;
			dataView.Location = new Point(3, 36);
			dataView.Name = "dataView";
			dataView.Size = new Size(342, 354);
			dataView.TabIndex = 2;
			dataView.CellValueChanged += dataView_CellValueChanged;
			// 
			// BrandManager
			// 
			AutoScaleDimensions = new SizeF(7F, 17F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(348, 393);
			Controls.Add(tableLayoutPanel1);
			Name = "BrandManager";
			Text = "BrandManager";
			Load += BrandManager_Load;
			tableLayoutPanel1.ResumeLayout(false);
			searchLayoutPanel.ResumeLayout(false);
			searchLayoutPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)dataView).EndInit();
			ResumeLayout(false);
		}

		#endregion

		private TableLayoutPanel tableLayoutPanel1;
		private TableLayoutPanel searchLayoutPanel;
		private Label searchLabel;
		private TextBox searchText;
		private Button searchBtn;
		private DataGridView dataView;
	}
}