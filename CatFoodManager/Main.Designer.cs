using CatFoodManager.Controls;

namespace CatFoodManager
{
    partial class Main
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
            components = new System.ComponentModel.Container();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            catFoodBindingSource = new BindingSource(components);
            ConfigManagement = new TabPage();
            configLayoutPanel = new TableLayoutPanel();
            tableLayoutPanel3 = new TableLayoutPanel();
            savePicConfigBtn = new Button();
            picConfigText = new TextBox();
            catFoodView = new TabPage();
            viewLayoutPanel = new TableLayoutPanel();
            searchLayoutPanel = new TableLayoutPanel();
            searchLabel = new Label();
            btnFunction = new Button();
            brandManagerBtn = new Button();
            rbnLowestPrice = new RadioButton();
            rtbInventory = new RadioButton();
            searchUserControl = new Controls.SearchUserControl();
            dataGridUserControl = new Controls.DataGridUserControl();
            paginationUserControl = new Controls.PaginationUserControl();
            tabControl1 = new TabControl();
            ((System.ComponentModel.ISupportInitialize)catFoodBindingSource).BeginInit();
            ConfigManagement.SuspendLayout();
            configLayoutPanel.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            catFoodView.SuspendLayout();
            viewLayoutPanel.SuspendLayout();
            searchLayoutPanel.SuspendLayout();
            tabControl1.SuspendLayout();
            SuspendLayout();

            ConfigManagement.Controls.Add(configLayoutPanel);
            ConfigManagement.Location = new Point(4, 26);
            ConfigManagement.Name = "ConfigManagement";
            ConfigManagement.Padding = new Padding(3);
            ConfigManagement.Size = new Size(842, 434);
            ConfigManagement.TabIndex = 1;
            ConfigManagement.Text = "配置管理";
            ConfigManagement.UseVisualStyleBackColor = true;

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

            picConfigText.Dock = DockStyle.Fill;
            picConfigText.Location = new Point(3, 3);
            picConfigText.Multiline = true;
            picConfigText.Name = "picConfigText";
            picConfigText.Size = new Size(758, 31);
            picConfigText.TabIndex = 2;

            catFoodView.BorderStyle = BorderStyle.FixedSingle;
            catFoodView.Controls.Add(viewLayoutPanel);
            catFoodView.Location = new Point(4, 26);
            catFoodView.Margin = new Padding(0);
            catFoodView.Name = "catFoodView";
            catFoodView.Size = new Size(842, 434);
            catFoodView.TabIndex = 0;
            catFoodView.Text = "猫粮管理";
            catFoodView.UseVisualStyleBackColor = true;

            viewLayoutPanel.ColumnCount = 1;
            viewLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            viewLayoutPanel.Controls.Add(searchLayoutPanel, 0, 0);
            viewLayoutPanel.Controls.Add(dataGridUserControl, 0, 1);
            viewLayoutPanel.Controls.Add(paginationUserControl, 0, 2);
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

            searchLayoutPanel.ColumnCount = 6;
            searchLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            searchLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 73F));
            searchLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 91F));
            searchLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 53F));
            searchLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            searchLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 400F));
            searchLayoutPanel.Controls.Add(rbnLowestPrice, 0, 0);
            searchLayoutPanel.Controls.Add(rtbInventory, 1, 0);
            searchLayoutPanel.Controls.Add(btnFunction, 2, 0);
            searchLayoutPanel.Controls.Add(brandManagerBtn, 3, 0);
            searchLayoutPanel.Controls.Add(searchLabel, 4, 0);
            searchLayoutPanel.Controls.Add(searchUserControl, 5, 0);
            searchLayoutPanel.Dock = DockStyle.Fill;
            searchLayoutPanel.Location = new Point(0, 0);
            searchLayoutPanel.Margin = new Padding(0);
            searchLayoutPanel.Name = "searchLayoutPanel";
            searchLayoutPanel.RowCount = 1;
            searchLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            searchLayoutPanel.Size = new Size(840, 34);
            searchLayoutPanel.TabIndex = 0;

            searchLabel.Anchor = AnchorStyles.Right;
            searchLabel.AutoSize = true;
            searchLabel.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
            searchLabel.Location = new Point(291, 6);
            searchLabel.Margin = new Padding(0);
            searchLabel.Name = "searchLabel";
            searchLabel.Size = new Size(47, 22);
            searchLabel.TabIndex = 0;
            searchLabel.Text = "检索:";
            searchLabel.TextAlign = ContentAlignment.MiddleCenter;

            btnFunction.Dock = DockStyle.Fill;
            btnFunction.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
            btnFunction.Location = new Point(146, 3);
            btnFunction.Name = "btnFunction";
            btnFunction.Size = new Size(55, 28);
            btnFunction.TabIndex = 3;
            btnFunction.Text = "同步";
            btnFunction.UseVisualStyleBackColor = true;
            btnFunction.Click += btnFunction_Click;

            brandManagerBtn.Dock = DockStyle.Fill;
            brandManagerBtn.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
            brandManagerBtn.Location = new Point(207, 3);
            brandManagerBtn.Name = "brandManagerBtn";
            brandManagerBtn.Size = new Size(85, 28);
            brandManagerBtn.TabIndex = 4;
            brandManagerBtn.Text = "品牌管理";
            brandManagerBtn.UseVisualStyleBackColor = true;
            brandManagerBtn.Click += brandManagerBtn_Click;

            rbnLowestPrice.Anchor = AnchorStyles.None;
            rbnLowestPrice.AutoSize = true;
            rbnLowestPrice.Checked = true;
            rbnLowestPrice.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
            rbnLowestPrice.Location = new Point(5, 4);
            rbnLowestPrice.Name = "rbnLowestPrice";
            rbnLowestPrice.Size = new Size(60, 26);
            rbnLowestPrice.TabIndex = 5;
            rbnLowestPrice.TabStop = true;
            rbnLowestPrice.Text = "比价";
            rbnLowestPrice.UseVisualStyleBackColor = true;
            rbnLowestPrice.CheckedChanged += rbnLowestPrice_CheckedChanged;

            rtbInventory.Anchor = AnchorStyles.None;
            rtbInventory.AutoSize = true;
            rtbInventory.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
            rtbInventory.Location = new Point(76, 4);
            rtbInventory.Name = "rtbInventory";
            rtbInventory.Size = new Size(60, 26);
            rtbInventory.TabIndex = 6;
            rtbInventory.TabStop = true;
            rtbInventory.Text = "库存";
            rtbInventory.UseVisualStyleBackColor = true;
            rtbInventory.CheckedChanged += rtbInventory_CheckedChanged;

            searchUserControl.Dock = DockStyle.Fill;
            searchUserControl.Location = new Point(344, 1);
            searchUserControl.Margin = new Padding(1);
            searchUserControl.Name = "searchUserControl";
            searchUserControl.Size = new Size(394, 32);
            searchUserControl.TabIndex = 7;

            dataGridUserControl.Dock = DockStyle.Fill;
            dataGridUserControl.Location = new Point(3, 37);
            dataGridUserControl.Name = "dataGridUserControl";
            dataGridUserControl.Size = new Size(834, 356);
            dataGridUserControl.TabIndex = 1;

            paginationUserControl.Dock = DockStyle.Fill;
            paginationUserControl.Location = new Point(1, 397);
            paginationUserControl.Margin = new Padding(1);
            paginationUserControl.Name = "paginationUserControl";
            paginationUserControl.Size = new Size(838, 34);
            paginationUserControl.TabIndex = 2;

            tabControl1.Controls.Add(catFoodView);
            tabControl1.Controls.Add(ConfigManagement);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(850, 464);
            tabControl1.TabIndex = 0;

            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(850, 464);
            Controls.Add(tabControl1);
            Margin = new Padding(2, 3, 2, 3);
            Name = "Main";
            Text = "CatFoodManager";
            Load += Main_Load;
            ((System.ComponentModel.ISupportInitialize)catFoodBindingSource).EndInit();
            ConfigManagement.ResumeLayout(false);
            configLayoutPanel.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            catFoodView.ResumeLayout(false);
            viewLayoutPanel.ResumeLayout(false);
            searchLayoutPanel.ResumeLayout(false);
            searchLayoutPanel.PerformLayout();
            tabControl1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private BindingSource catFoodBindingSource;
        private TabPage ConfigManagement;
        private TableLayoutPanel configLayoutPanel;
        private TableLayoutPanel tableLayoutPanel3;
        private Button savePicConfigBtn;
        private TextBox picConfigText;
        private TabPage catFoodView;
        private TableLayoutPanel viewLayoutPanel;
        private TableLayoutPanel searchLayoutPanel;
        private Label searchLabel;
        private Button btnFunction;
        private Button brandManagerBtn;
        private TableLayoutPanel tableLayoutPanel1;
        private TabControl tabControl1;
        private RadioButton rbnLowestPrice;
        private RadioButton rtbInventory;
        private Controls.SearchUserControl searchUserControl;
        private Controls.DataGridUserControl dataGridUserControl;
        private Controls.PaginationUserControl paginationUserControl;
    }
}
