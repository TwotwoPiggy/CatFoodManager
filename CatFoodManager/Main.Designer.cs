namespace CatFoodManager
{
	partial class Main
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
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
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
			DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
			DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
			catFoodBindingSource = new BindingSource(components);
			tabPage3 = new TabPage();
			configLayoutPanel = new TableLayoutPanel();
			tableLayoutPanel3 = new TableLayoutPanel();
			savePicConfigBtn = new Button();
			picConfigText = new TextBox();
			tabPage1 = new TabPage();
			viewLayoutPanel = new TableLayoutPanel();
			searchLayoutPanel = new TableLayoutPanel();
			searchLabel = new Label();
			searchText = new TextBox();
			searchBtn = new Button();
			syncBtn = new Button();
			brandManagerBtn = new Button();
			dataView = new DataGridView();
			tableLayoutPanel1 = new TableLayoutPanel();
			totalLabel = new Label();
			pageInfoLabel = new Label();
			pageSizeHeaderLabel = new Label();
			pageSizeComboBox = new ComboBox();
			pageSizeLineLabel = new Label();
			homeBtn = new Button();
			prePageBtn = new Button();
			nextPageBtn = new Button();
			lastPageBtn = new Button();
			jumpBtn = new Button();
			jumpPageText = new TextBox();
			tabControl1 = new TabControl();
			((System.ComponentModel.ISupportInitialize)catFoodBindingSource).BeginInit();
			tabPage3.SuspendLayout();
			configLayoutPanel.SuspendLayout();
			tableLayoutPanel3.SuspendLayout();
			tabPage1.SuspendLayout();
			viewLayoutPanel.SuspendLayout();
			searchLayoutPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)dataView).BeginInit();
			tableLayoutPanel1.SuspendLayout();
			tabControl1.SuspendLayout();
			SuspendLayout();
			// 
			// catFoodBindingSource
			// 
			catFoodBindingSource.DataSource = typeof(Core.Models.CatFood);
			// 
			// tabPage3
			// 
			tabPage3.Controls.Add(configLayoutPanel);
			tabPage3.Location = new Point(4, 26);
			tabPage3.Name = "tabPage3";
			tabPage3.Padding = new Padding(3);
			tabPage3.Size = new Size(842, 434);
			tabPage3.TabIndex = 1;
			tabPage3.Text = "配置管理";
			tabPage3.UseVisualStyleBackColor = true;
			// 
			// configLayoutPanel
			// 
			configLayoutPanel.ColumnCount = 1;
			configLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			configLayoutPanel.Controls.Add(tableLayoutPanel3, 0, 0);
			configLayoutPanel.Dock = DockStyle.Fill;
			configLayoutPanel.Location = new Point(3, 3);
			configLayoutPanel.Margin = new Padding(1);
			configLayoutPanel.Name = "configLayoutPanel";
			configLayoutPanel.RowCount = 3;
			configLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 39F));
			configLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 279F));
			configLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 8F));
			configLayoutPanel.Size = new Size(836, 428);
			configLayoutPanel.TabIndex = 0;
			// 
			// tableLayoutPanel3
			// 
			tableLayoutPanel3.ColumnCount = 2;
			tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 764F));
			tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 13F));
			tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
			tableLayoutPanel3.Controls.Add(savePicConfigBtn, 2, 0);
			tableLayoutPanel3.Controls.Add(picConfigText, 0, 0);
			tableLayoutPanel3.Dock = DockStyle.Fill;
			tableLayoutPanel3.Location = new Point(1, 1);
			tableLayoutPanel3.Margin = new Padding(1);
			tableLayoutPanel3.Name = "tableLayoutPanel3";
			tableLayoutPanel3.RowCount = 1;
			tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			tableLayoutPanel3.Size = new Size(834, 37);
			tableLayoutPanel3.TabIndex = 0;
			// 
			// savePicConfigBtn
			// 
			savePicConfigBtn.Dock = DockStyle.Fill;
			savePicConfigBtn.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
			savePicConfigBtn.Location = new Point(765, 1);
			savePicConfigBtn.Margin = new Padding(1);
			savePicConfigBtn.Name = "savePicConfigBtn";
			savePicConfigBtn.Size = new Size(68, 35);
			savePicConfigBtn.TabIndex = 1;
			savePicConfigBtn.Text = "保存";
			savePicConfigBtn.UseVisualStyleBackColor = true;
			savePicConfigBtn.Click += savePicConfigBtn_Click;
			// 
			// picConfigText
			// 
			picConfigText.Dock = DockStyle.Fill;
			picConfigText.Location = new Point(3, 3);
			picConfigText.Multiline = true;
			picConfigText.Name = "picConfigText";
			picConfigText.Size = new Size(758, 31);
			picConfigText.TabIndex = 2;
			// 
			// tabPage1
			// 
			tabPage1.BorderStyle = BorderStyle.FixedSingle;
			tabPage1.Controls.Add(viewLayoutPanel);
			tabPage1.Location = new Point(4, 26);
			tabPage1.Margin = new Padding(0);
			tabPage1.Name = "tabPage1";
			tabPage1.Size = new Size(842, 434);
			tabPage1.TabIndex = 0;
			tabPage1.Text = "猫粮管理";
			tabPage1.UseVisualStyleBackColor = true;
			// 
			// viewLayoutPanel
			// 
			viewLayoutPanel.ColumnCount = 1;
			viewLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			viewLayoutPanel.Controls.Add(searchLayoutPanel, 0, 0);
			viewLayoutPanel.Controls.Add(dataView, 0, 1);
			viewLayoutPanel.Controls.Add(tableLayoutPanel1, 0, 2);
			viewLayoutPanel.Dock = DockStyle.Fill;
			viewLayoutPanel.Font = new Font("Microsoft YaHei UI", 9F);
			viewLayoutPanel.Location = new Point(0, 0);
			viewLayoutPanel.Margin = new Padding(1);
			viewLayoutPanel.Name = "viewLayoutPanel";
			viewLayoutPanel.RowCount = 3;
			viewLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 8F));
			viewLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 84F));
			viewLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 8F));
			viewLayoutPanel.Size = new Size(840, 432);
			viewLayoutPanel.TabIndex = 0;
			// 
			// searchLayoutPanel
			// 
			searchLayoutPanel.ColumnCount = 5;
			searchLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 69F));
			searchLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 93F));
			searchLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 63F));
			searchLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 561F));
			searchLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 21F));
			searchLayoutPanel.Controls.Add(searchLabel, 2, 0);
			searchLayoutPanel.Controls.Add(searchText, 3, 0);
			searchLayoutPanel.Controls.Add(searchBtn, 4, 0);
			searchLayoutPanel.Controls.Add(syncBtn, 0, 0);
			searchLayoutPanel.Controls.Add(brandManagerBtn, 1, 0);
			searchLayoutPanel.Dock = DockStyle.Fill;
			searchLayoutPanel.Location = new Point(0, 0);
			searchLayoutPanel.Margin = new Padding(0);
			searchLayoutPanel.Name = "searchLayoutPanel";
			searchLayoutPanel.RowCount = 1;
			searchLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			searchLayoutPanel.Size = new Size(840, 34);
			searchLayoutPanel.TabIndex = 0;
			// 
			// searchLabel
			// 
			searchLabel.Anchor = AnchorStyles.Right;
			searchLabel.AutoSize = true;
			searchLabel.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
			searchLabel.Location = new Point(178, 6);
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
			searchText.Font = new Font("Microsoft YaHei UI", 15F);
			searchText.Location = new Point(226, 1);
			searchText.Margin = new Padding(1);
			searchText.Name = "searchText";
			searchText.Size = new Size(559, 33);
			searchText.TabIndex = 1;
			searchText.TextChanged += searchText_TextChanged;
			searchText.KeyDown += searchText_KeyDown;
			// 
			// searchBtn
			// 
			searchBtn.Dock = DockStyle.Fill;
			searchBtn.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
			searchBtn.Location = new Point(787, 1);
			searchBtn.Margin = new Padding(1);
			searchBtn.Name = "searchBtn";
			searchBtn.Size = new Size(52, 32);
			searchBtn.TabIndex = 2;
			searchBtn.Text = "搜索";
			searchBtn.UseVisualStyleBackColor = true;
			searchBtn.Click += searchBtn_Click;
			// 
			// syncBtn
			// 
			syncBtn.Dock = DockStyle.Fill;
			syncBtn.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
			syncBtn.Location = new Point(3, 3);
			syncBtn.Name = "syncBtn";
			syncBtn.Size = new Size(63, 28);
			syncBtn.TabIndex = 3;
			syncBtn.Text = "同步";
			syncBtn.UseVisualStyleBackColor = true;
			syncBtn.Click += syncBtn_Click;
			// 
			// brandManagerBtn
			// 
			brandManagerBtn.Dock = DockStyle.Fill;
			brandManagerBtn.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
			brandManagerBtn.Location = new Point(72, 3);
			brandManagerBtn.Name = "brandManagerBtn";
			brandManagerBtn.Size = new Size(87, 28);
			brandManagerBtn.TabIndex = 4;
			brandManagerBtn.Text = "品牌管理";
			brandManagerBtn.UseVisualStyleBackColor = true;
			brandManagerBtn.Click += brandManagerBtn_Click;
			// 
			// dataView
			// 
			dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = SystemColors.Control;
			dataGridViewCellStyle1.Font = new Font("Microsoft YaHei UI", 9F);
			dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
			dataView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			dataView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = SystemColors.Window;
			dataGridViewCellStyle2.Font = new Font("Microsoft YaHei UI", 9F);
			dataGridViewCellStyle2.ForeColor = SystemColors.ControlText;
			dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
			dataView.DefaultCellStyle = dataGridViewCellStyle2;
			dataView.Dock = DockStyle.Fill;
			dataView.Location = new Point(3, 37);
			dataView.Name = "dataView";
			dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle3.BackColor = SystemColors.Control;
			dataGridViewCellStyle3.Font = new Font("Microsoft YaHei UI", 9F);
			dataGridViewCellStyle3.ForeColor = SystemColors.WindowText;
			dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
			dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
			dataGridViewCellStyle3.WrapMode = DataGridViewTriState.True;
			dataView.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
			dataView.Size = new Size(834, 356);
			dataView.TabIndex = 1;
			dataView.CellValueChanged += dataView_CellValueChanged;
			dataView.CurrentCellDirtyStateChanged += dataView_CurrentCellDirtyStateChanged;
			// 
			// tableLayoutPanel1
			// 
			tableLayoutPanel1.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
			tableLayoutPanel1.ColumnCount = 17;
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 103F));
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 103F));
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 35F));
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 65F));
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 26F));
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 42F));
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 51F));
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 67F));
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 69F));
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 51F));
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 29F));
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 68F));
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 57F));
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 1669F));
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
			tableLayoutPanel1.Controls.Add(totalLabel, 0, 0);
			tableLayoutPanel1.Controls.Add(pageInfoLabel, 1, 0);
			tableLayoutPanel1.Controls.Add(pageSizeHeaderLabel, 2, 0);
			tableLayoutPanel1.Controls.Add(pageSizeComboBox, 3, 0);
			tableLayoutPanel1.Controls.Add(pageSizeLineLabel, 4, 0);
			tableLayoutPanel1.Controls.Add(homeBtn, 6, 0);
			tableLayoutPanel1.Controls.Add(prePageBtn, 7, 0);
			tableLayoutPanel1.Controls.Add(nextPageBtn, 8, 0);
			tableLayoutPanel1.Controls.Add(lastPageBtn, 9, 0);
			tableLayoutPanel1.Controls.Add(jumpBtn, 11, 0);
			tableLayoutPanel1.Controls.Add(jumpPageText, 12, 0);
			tableLayoutPanel1.Dock = DockStyle.Fill;
			tableLayoutPanel1.Location = new Point(1, 397);
			tableLayoutPanel1.Margin = new Padding(1);
			tableLayoutPanel1.Name = "tableLayoutPanel1";
			tableLayoutPanel1.RowCount = 1;
			tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			tableLayoutPanel1.Size = new Size(838, 34);
			tableLayoutPanel1.TabIndex = 2;
			// 
			// totalLabel
			// 
			totalLabel.AutoSize = true;
			totalLabel.Dock = DockStyle.Fill;
			totalLabel.Location = new Point(2, 2);
			totalLabel.Margin = new Padding(1);
			totalLabel.Name = "totalLabel";
			totalLabel.Size = new Size(101, 30);
			totalLabel.TabIndex = 0;
			totalLabel.Text = "共 10000 条记录";
			totalLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// pageInfoLabel
			// 
			pageInfoLabel.AutoSize = true;
			pageInfoLabel.Dock = DockStyle.Fill;
			pageInfoLabel.Location = new Point(106, 2);
			pageInfoLabel.Margin = new Padding(1);
			pageInfoLabel.Name = "pageInfoLabel";
			pageInfoLabel.Size = new Size(101, 30);
			pageInfoLabel.TabIndex = 1;
			pageInfoLabel.Text = "当前页 100/100";
			pageInfoLabel.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// pageSizeHeaderLabel
			// 
			pageSizeHeaderLabel.AutoSize = true;
			pageSizeHeaderLabel.Dock = DockStyle.Fill;
			pageSizeHeaderLabel.Location = new Point(210, 2);
			pageSizeHeaderLabel.Margin = new Padding(1);
			pageSizeHeaderLabel.Name = "pageSizeHeaderLabel";
			pageSizeHeaderLabel.Size = new Size(33, 30);
			pageSizeHeaderLabel.TabIndex = 2;
			pageSizeHeaderLabel.Text = "每页";
			pageSizeHeaderLabel.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// pageSizeComboBox
			// 
			pageSizeComboBox.Anchor = AnchorStyles.None;
			pageSizeComboBox.FormattingEnabled = true;
			pageSizeComboBox.Items.AddRange(new object[] { "10", "20", "50", "100" });
			pageSizeComboBox.Location = new Point(251, 4);
			pageSizeComboBox.Margin = new Padding(1);
			pageSizeComboBox.Name = "pageSizeComboBox";
			pageSizeComboBox.Size = new Size(53, 25);
			pageSizeComboBox.TabIndex = 3;
			// 
			// pageSizeLineLabel
			// 
			pageSizeLineLabel.AutoSize = true;
			pageSizeLineLabel.Dock = DockStyle.Fill;
			pageSizeLineLabel.Location = new Point(312, 2);
			pageSizeLineLabel.Margin = new Padding(1);
			pageSizeLineLabel.Name = "pageSizeLineLabel";
			pageSizeLineLabel.Size = new Size(24, 30);
			pageSizeLineLabel.TabIndex = 4;
			pageSizeLineLabel.Text = "行";
			pageSizeLineLabel.TextAlign = ContentAlignment.MiddleCenter;
			// 
			// homeBtn
			// 
			homeBtn.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold);
			homeBtn.Location = new Point(384, 4);
			homeBtn.Name = "homeBtn";
			homeBtn.Size = new Size(45, 26);
			homeBtn.TabIndex = 5;
			homeBtn.Text = "首页";
			homeBtn.UseVisualStyleBackColor = true;
			homeBtn.Click += homeBtn_Click;
			// 
			// prePageBtn
			// 
			prePageBtn.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold);
			prePageBtn.Location = new Point(436, 4);
			prePageBtn.Name = "prePageBtn";
			prePageBtn.Size = new Size(60, 26);
			prePageBtn.TabIndex = 6;
			prePageBtn.Text = "上一页";
			prePageBtn.UseVisualStyleBackColor = true;
			prePageBtn.Click += prePageBtn_Click;
			// 
			// nextPageBtn
			// 
			nextPageBtn.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold);
			nextPageBtn.Location = new Point(504, 4);
			nextPageBtn.Name = "nextPageBtn";
			nextPageBtn.Size = new Size(60, 26);
			nextPageBtn.TabIndex = 7;
			nextPageBtn.Text = "下一页";
			nextPageBtn.UseVisualStyleBackColor = true;
			nextPageBtn.Click += nextPageBtn_Click;
			// 
			// lastPageBtn
			// 
			lastPageBtn.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold);
			lastPageBtn.Location = new Point(574, 4);
			lastPageBtn.Name = "lastPageBtn";
			lastPageBtn.Size = new Size(45, 26);
			lastPageBtn.TabIndex = 8;
			lastPageBtn.Text = "末页";
			lastPageBtn.UseVisualStyleBackColor = true;
			lastPageBtn.Click += lastPageBtn_Click;
			// 
			// jumpBtn
			// 
			jumpBtn.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold);
			jumpBtn.Location = new Point(656, 4);
			jumpBtn.Name = "jumpBtn";
			jumpBtn.Size = new Size(60, 26);
			jumpBtn.TabIndex = 9;
			jumpBtn.Text = "跳转到";
			jumpBtn.UseVisualStyleBackColor = true;
			jumpBtn.Click += jumpBtn_Click;
			// 
			// jumpPageText
			// 
			jumpPageText.Dock = DockStyle.Fill;
			jumpPageText.Location = new Point(725, 4);
			jumpPageText.Multiline = true;
			jumpPageText.Name = "jumpPageText";
			jumpPageText.Size = new Size(51, 26);
			jumpPageText.TabIndex = 10;
			// 
			// tabControl1
			// 
			tabControl1.Controls.Add(tabPage1);
			tabControl1.Controls.Add(tabPage3);
			tabControl1.Dock = DockStyle.Fill;
			tabControl1.Location = new Point(0, 0);
			tabControl1.Name = "tabControl1";
			tabControl1.SelectedIndex = 0;
			tabControl1.Size = new Size(850, 464);
			tabControl1.TabIndex = 0;
			// 
			// Main
			// 
			AutoScaleDimensions = new SizeF(7F, 17F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(850, 464);
			Controls.Add(tabControl1);
			Margin = new Padding(2, 3, 2, 3);
			Name = "Main";
			Text = "CatFoodManager";
			Load += Main_Load;
			((System.ComponentModel.ISupportInitialize)catFoodBindingSource).EndInit();
			tabPage3.ResumeLayout(false);
			configLayoutPanel.ResumeLayout(false);
			tableLayoutPanel3.ResumeLayout(false);
			tableLayoutPanel3.PerformLayout();
			tabPage1.ResumeLayout(false);
			viewLayoutPanel.ResumeLayout(false);
			searchLayoutPanel.ResumeLayout(false);
			searchLayoutPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)dataView).EndInit();
			tableLayoutPanel1.ResumeLayout(false);
			tableLayoutPanel1.PerformLayout();
			tabControl1.ResumeLayout(false);
			ResumeLayout(false);
		}

		private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{
			
		}

#endregion
		private BindingSource catFoodBindingSource;
		private TabPage tabPage3;
		private TableLayoutPanel configLayoutPanel;
		private TableLayoutPanel tableLayoutPanel3;
		private Button savePicConfigBtn;
		private TextBox picConfigText;
		private TabPage tabPage1;
		private TableLayoutPanel viewLayoutPanel;
		private TableLayoutPanel searchLayoutPanel;
		private Label searchLabel;
		private TextBox searchText;
		private Button searchBtn;
		private Button syncBtn;
		private DataGridView dataView;
		private TableLayoutPanel tableLayoutPanel1;
		private Label totalLabel;
		private Label pageInfoLabel;
		private Label pageSizeHeaderLabel;
		private ComboBox pageSizeComboBox;
		private Label pageSizeLineLabel;
		private Button homeBtn;
		private Button prePageBtn;
		private Button nextPageBtn;
		private Button lastPageBtn;
		private Button jumpBtn;
		private TextBox jumpPageText;
		private TabControl tabControl1;
		private Button brandManagerBtn;
	}
}
