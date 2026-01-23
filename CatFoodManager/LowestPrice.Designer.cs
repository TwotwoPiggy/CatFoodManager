using CatFoodManager.Core.Statics;
using CommonTools;

namespace CatFoodManager
{
    partial class LowestPrice
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
            BaseLayout = new TableLayoutPanel();
            TopLayoutPanel = new TableLayoutPanel();
            lblType = new Label();
            cbType = new ComboBox();
            lblLowestPrice = new Label();
            txtLowestPrice = new TextBox();
            lblFinalPrice = new Label();
            txtFinalPrice = new TextBox();
            label4 = new Label();
            PurchasedAt = new DateTimePicker();
            lblHasPurchased = new Label();
            tableLayoutPanel2 = new TableLayoutPanel();
            rbtPurchased = new RadioButton();
            rbtNotPurchased = new RadioButton();
            lblPlatform = new Label();
            cbPlatform = new ComboBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            lblName = new Label();
            rtbName = new RichTextBox();
            tableLayoutPanel3 = new TableLayoutPanel();
            btnCancel = new Button();
            PicSelector = new Lemon.UI.Controls.FileSelector();
            btnSave = new Button();
            BaseLayout.SuspendLayout();
            TopLayoutPanel.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            SuspendLayout();
            // 
            // BaseLayout
            // 
            BaseLayout.ColumnCount = 1;
            BaseLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            BaseLayout.Controls.Add(TopLayoutPanel, 0, 1);
            BaseLayout.Controls.Add(tableLayoutPanel1, 0, 0);
            BaseLayout.Controls.Add(tableLayoutPanel3, 0, 2);
            BaseLayout.Dock = DockStyle.Fill;
            BaseLayout.Location = new Point(0, 0);
            BaseLayout.Margin = new Padding(0);
            BaseLayout.Name = "BaseLayout";
            BaseLayout.RowCount = 3;
            BaseLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 61F));
            BaseLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 73.4177246F));
            BaseLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 26.58228F));
            BaseLayout.Size = new Size(620, 219);
            BaseLayout.TabIndex = 0;
            // 
            // TopLayoutPanel
            // 
            TopLayoutPanel.ColumnCount = 4;
            TopLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 89F));
            TopLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 207F));
            TopLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 84F));
            TopLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 107F));
            TopLayoutPanel.Controls.Add(lblType, 0, 0);
            TopLayoutPanel.Controls.Add(cbType, 1, 0);
            TopLayoutPanel.Controls.Add(lblLowestPrice, 0, 1);
            TopLayoutPanel.Controls.Add(txtLowestPrice, 1, 1);
            TopLayoutPanel.Controls.Add(lblFinalPrice, 0, 2);
            TopLayoutPanel.Controls.Add(txtFinalPrice, 1, 2);
            TopLayoutPanel.Controls.Add(label4, 2, 2);
            TopLayoutPanel.Controls.Add(PurchasedAt, 3, 2);
            TopLayoutPanel.Controls.Add(lblHasPurchased, 2, 1);
            TopLayoutPanel.Controls.Add(tableLayoutPanel2, 3, 1);
            TopLayoutPanel.Controls.Add(lblPlatform, 2, 0);
            TopLayoutPanel.Controls.Add(cbPlatform, 3, 0);
            TopLayoutPanel.Dock = DockStyle.Fill;
            TopLayoutPanel.Location = new Point(0, 61);
            TopLayoutPanel.Margin = new Padding(0);
            TopLayoutPanel.Name = "TopLayoutPanel";
            TopLayoutPanel.RowCount = 3;
            TopLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 34.8623848F));
            TopLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 30.2752285F));
            TopLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.9449539F));
            TopLayoutPanel.Size = new Size(620, 116);
            TopLayoutPanel.TabIndex = 0;
            // 
            // lblType
            // 
            lblType.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblType.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold);
            lblType.Location = new Point(3, 0);
            lblType.Name = "lblType";
            lblType.Size = new Size(83, 40);
            lblType.TabIndex = 3;
            lblType.Text = "Type:";
            lblType.TextAlign = ContentAlignment.MiddleRight;
            // 
            // cbType
            // 
            cbType.Dock = DockStyle.Fill;
            cbType.FormattingEnabled = true;
            cbType.Location = new Point(92, 3);
            cbType.Name = "cbType";
            cbType.Size = new Size(201, 25);
            cbType.TabIndex = 6;
            // 
            // lblLowestPrice
            // 
            lblLowestPrice.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblLowestPrice.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold);
            lblLowestPrice.Location = new Point(3, 40);
            lblLowestPrice.Name = "lblLowestPrice";
            lblLowestPrice.Size = new Size(83, 35);
            lblLowestPrice.TabIndex = 7;
            lblLowestPrice.Text = "史低:";
            lblLowestPrice.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtLowestPrice
            // 
            txtLowestPrice.Dock = DockStyle.Fill;
            txtLowestPrice.Location = new Point(92, 43);
            txtLowestPrice.Name = "txtLowestPrice";
            txtLowestPrice.Size = new Size(201, 23);
            txtLowestPrice.TabIndex = 8;
            // 
            // lblFinalPrice
            // 
            lblFinalPrice.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblFinalPrice.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold);
            lblFinalPrice.Location = new Point(3, 75);
            lblFinalPrice.Name = "lblFinalPrice";
            lblFinalPrice.Size = new Size(83, 41);
            lblFinalPrice.TabIndex = 9;
            lblFinalPrice.Text = "购买价格:";
            lblFinalPrice.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtFinalPrice
            // 
            txtFinalPrice.Dock = DockStyle.Fill;
            txtFinalPrice.Location = new Point(92, 78);
            txtFinalPrice.Name = "txtFinalPrice";
            txtFinalPrice.Size = new Size(201, 23);
            txtFinalPrice.TabIndex = 13;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label4.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold);
            label4.Location = new Point(299, 75);
            label4.Name = "label4";
            label4.Size = new Size(78, 41);
            label4.TabIndex = 12;
            label4.Text = "购买日期:";
            label4.TextAlign = ContentAlignment.MiddleRight;
            // 
            // PurchasedAt
            // 
            PurchasedAt.Dock = DockStyle.Fill;
            PurchasedAt.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold);
            PurchasedAt.Location = new Point(383, 78);
            PurchasedAt.Name = "PurchasedAt";
            PurchasedAt.Size = new Size(234, 24);
            PurchasedAt.TabIndex = 14;
            // 
            // lblHasPurchased
            // 
            lblHasPurchased.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblHasPurchased.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold);
            lblHasPurchased.Location = new Point(299, 40);
            lblHasPurchased.Name = "lblHasPurchased";
            lblHasPurchased.Size = new Size(78, 35);
            lblHasPurchased.TabIndex = 11;
            lblHasPurchased.Text = "已购买:";
            lblHasPurchased.TextAlign = ContentAlignment.MiddleRight;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 3;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Controls.Add(rbtPurchased, 1, 0);
            tableLayoutPanel2.Controls.Add(rbtNotPurchased, 2, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(380, 40);
            tableLayoutPanel2.Margin = new Padding(0);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(240, 35);
            tableLayoutPanel2.TabIndex = 17;
            // 
            // rbtPurchased
            // 
            rbtPurchased.AutoSize = true;
            rbtPurchased.Dock = DockStyle.Fill;
            rbtPurchased.Location = new Point(23, 3);
            rbtPurchased.Name = "rbtPurchased";
            rbtPurchased.Size = new Size(104, 29);
            rbtPurchased.TabIndex = 15;
            rbtPurchased.Text = "已购买";
            rbtPurchased.UseVisualStyleBackColor = true;
            // 
            // rbtNotPurchased
            // 
            rbtNotPurchased.AutoSize = true;
            rbtNotPurchased.Checked = true;
            rbtNotPurchased.Dock = DockStyle.Fill;
            rbtNotPurchased.Location = new Point(133, 3);
            rbtNotPurchased.Name = "rbtNotPurchased";
            rbtNotPurchased.Size = new Size(104, 29);
            rbtNotPurchased.TabIndex = 16;
            rbtNotPurchased.TabStop = true;
            rbtNotPurchased.Text = "未购买";
            rbtNotPurchased.UseVisualStyleBackColor = true;
            // 
            // lblPlatform
            // 
            lblPlatform.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblPlatform.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold);
            lblPlatform.Location = new Point(299, 0);
            lblPlatform.Name = "lblPlatform";
            lblPlatform.Size = new Size(78, 40);
            lblPlatform.TabIndex = 18;
            lblPlatform.Text = "平台:";
            lblPlatform.TextAlign = ContentAlignment.MiddleRight;
            // 
            // cbPlatform
            // 
            cbPlatform.Dock = DockStyle.Fill;
            cbPlatform.FormattingEnabled = true;
            cbPlatform.Location = new Point(383, 3);
            cbPlatform.Name = "cbPlatform";
            cbPlatform.Size = new Size(234, 25);
            cbPlatform.TabIndex = 19;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 530F));
            tableLayoutPanel1.Controls.Add(lblName, 0, 0);
            tableLayoutPanel1.Controls.Add(rtbName, 1, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Margin = new Padding(0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(620, 61);
            tableLayoutPanel1.TabIndex = 2;
            // 
            // lblName
            // 
            lblName.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblName.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Bold);
            lblName.Location = new Point(3, 0);
            lblName.Name = "lblName";
            lblName.Size = new Size(84, 61);
            lblName.TabIndex = 4;
            lblName.Text = "Name:";
            lblName.TextAlign = ContentAlignment.MiddleRight;
            // 
            // rtbName
            // 
            rtbName.Dock = DockStyle.Fill;
            rtbName.Location = new Point(93, 3);
            rtbName.Name = "rtbName";
            rtbName.Size = new Size(524, 55);
            rtbName.TabIndex = 5;
            rtbName.Text = "";
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 3;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 82.04633F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 17.9536686F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 101F));
            tableLayoutPanel3.Controls.Add(btnCancel, 1, 0);
            tableLayoutPanel3.Controls.Add(PicSelector, 0, 0);
            tableLayoutPanel3.Controls.Add(btnSave, 2, 0);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(0, 177);
            tableLayoutPanel3.Margin = new Padding(0);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 1;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.Size = new Size(620, 42);
            tableLayoutPanel3.TabIndex = 3;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.Transparent;
            btnCancel.Dock = DockStyle.Fill;
            btnCancel.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
            btnCancel.Location = new Point(426, 1);
            btnCancel.Margin = new Padding(1);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(91, 40);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "取消";
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += btnCancel_Click;
            // 
            // PicSelector
            // 
            PicSelector.ButtonSize = new Size(78, 30);
            PicSelector.ButtonText = "选择文件";
            PicSelector.Dock = DockStyle.Fill;
            PicSelector.Location = new Point(2, 3);
            PicSelector.Margin = new Padding(2, 3, 2, 3);
            PicSelector.Name = "PicSelector";
            PicSelector.Size = new Size(421, 36);
            PicSelector.TabIndex = 1;
            // 
            // btnSave
            // 
            btnSave.BackColor = Color.Transparent;
            btnSave.Dock = DockStyle.Fill;
            btnSave.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold);
            btnSave.Location = new Point(519, 1);
            btnSave.Margin = new Padding(1);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(100, 40);
            btnSave.TabIndex = 2;
            btnSave.Text = "保存";
            btnSave.UseVisualStyleBackColor = false;
            btnSave.Click += btnSave_Click;
            // 
            // LowestPrice
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(620, 219);
            Controls.Add(BaseLayout);
            Name = "LowestPrice";
            Text = "LowestPrice";
            BaseLayout.ResumeLayout(false);
            TopLayoutPanel.ResumeLayout(false);
            TopLayoutPanel.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel BaseLayout;
        private TableLayoutPanel TopLayoutPanel;
        private Lemon.UI.Controls.FileSelector PicSelector;
        private Label lblType;
        private Label lblName;
        private RichTextBox rtbName;
        private ComboBox cbType;
        private TableLayoutPanel tableLayoutPanel1;
        private Label lblLowestPrice;
        private TextBox txtLowestPrice;
        private Label lblFinalPrice;
        private Label lblHasPurchased;
        private Label label4;
        private TextBox txtFinalPrice;
        private DateTimePicker PurchasedAt;
        private TableLayoutPanel tableLayoutPanel2;
        private RadioButton rbtPurchased;
        private RadioButton rbtNotPurchased;
        private TableLayoutPanel tableLayoutPanel3;
        private Button btnCancel;
        private Button btnSave;
        private Label lblPlatform;
        private ComboBox cbPlatform;
    }
}