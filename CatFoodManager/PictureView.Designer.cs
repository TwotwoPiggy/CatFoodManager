namespace CatFoodManager
{
	partial class PictureView
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
			pictureLayoutPanel = new TableLayoutPanel();
			pictureBox = new PictureBox();
			pictureLayoutPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
			SuspendLayout();
			// 
			// pictureLayoutPanel
			// 
			pictureLayoutPanel.ColumnCount = 1;
			pictureLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			pictureLayoutPanel.Controls.Add(pictureBox, 0, 0);
			pictureLayoutPanel.Dock = DockStyle.Fill;
			pictureLayoutPanel.Location = new Point(0, 0);
			pictureLayoutPanel.Name = "pictureLayoutPanel";
			pictureLayoutPanel.RowCount = 2;
			pictureLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 619F));
			pictureLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 8F));
			pictureLayoutPanel.Size = new Size(318, 681);
			pictureLayoutPanel.TabIndex = 0;
			// 
			// pictureBox
			// 
			pictureBox.Dock = DockStyle.Fill;
			pictureBox.Location = new Point(3, 3);
			pictureBox.Name = "pictureBox";
			pictureBox.Size = new Size(312, 613);
			pictureBox.TabIndex = 0;
			pictureBox.TabStop = false;
			// 
			// PictureView
			// 
			AutoScaleDimensions = new SizeF(7F, 17F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(318, 681);
			Controls.Add(pictureLayoutPanel);
			Name = "PictureView";
			Text = "PictureView";
			Load += PictureView_Load;
			Leave += PictureView_Leave;
			pictureLayoutPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
			ResumeLayout(false);
		}

		#endregion

		private TableLayoutPanel pictureLayoutPanel;
		private PictureBox pictureBox;
	}
}