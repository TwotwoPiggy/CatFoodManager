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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
			parrotForm1 = new ReaLTaiizor.Forms.ParrotForm();
			parrotButton1 = new ReaLTaiizor.Controls.ParrotButton();
			parrotForm1.SuspendLayout();
			SuspendLayout();
			// 
			// parrotForm1
			// 
			parrotForm1.BackColor = Color.FromArgb(236, 236, 236);
			parrotForm1.Controls.Add(parrotButton1);
			parrotForm1.Dock = DockStyle.Fill;
			parrotForm1.ExitApplication = true;
			parrotForm1.FormStyle = ReaLTaiizor.Forms.ParrotForm.Style.MacOS;
			parrotForm1.Location = new Point(0, 0);
			parrotForm1.MacOSForeColor = Color.FromArgb(40, 40, 40);
			parrotForm1.MacOSLeftBackColor = Color.FromArgb(230, 230, 230);
			parrotForm1.MacOSRightBackColor = Color.FromArgb(210, 210, 210);
			parrotForm1.MacOSSeparatorColor = Color.FromArgb(173, 173, 173);
			parrotForm1.MaterialBackColor = Color.DodgerBlue;
			parrotForm1.MaterialForeColor = Color.White;
			parrotForm1.Name = "parrotForm1";
			parrotForm1.ShowMaximize = true;
			parrotForm1.ShowMinimize = true;
			parrotForm1.Size = new Size(715, 439);
			parrotForm1.SmoothingType = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			parrotForm1.TabIndex = 0;
			parrotForm1.TitleText = "Parrot Form";
			parrotForm1.UbuntuForeColor = Color.FromArgb(220, 220, 210);
			parrotForm1.UbuntuLeftBackColor = Color.FromArgb(90, 85, 80);
			parrotForm1.UbuntuRightBackColor = Color.FromArgb(65, 65, 60);
			// 
			// 
			// 
			parrotForm1.WorkingArea.BackColor = Color.FromArgb(236, 236, 236);
			parrotForm1.WorkingArea.Dock = DockStyle.Fill;
			parrotForm1.WorkingArea.Location = new Point(0, 39);
			parrotForm1.WorkingArea.Name = "";
			parrotForm1.WorkingArea.Size = new Size(715, 400);
			parrotForm1.WorkingArea.TabIndex = 0;
			// 
			// parrotButton1
			// 
			parrotButton1.BackgroundColor = Color.FromArgb(255, 255, 255);
			parrotButton1.ButtonImage = (Image)resources.GetObject("parrotButton1.ButtonImage");
			parrotButton1.ButtonStyle = ReaLTaiizor.Controls.ParrotButton.Style.MaterialRounded;
			parrotButton1.ButtonText = "Button";
			parrotButton1.ClickBackColor = Color.FromArgb(195, 195, 195);
			parrotButton1.ClickTextColor = Color.DodgerBlue;
			parrotButton1.CornerRadius = 5;
			parrotButton1.Horizontal_Alignment = StringAlignment.Center;
			parrotButton1.HoverBackgroundColor = Color.FromArgb(225, 225, 225);
			parrotButton1.HoverTextColor = Color.DodgerBlue;
			parrotButton1.ImagePosition = ReaLTaiizor.Controls.ParrotButton.ImgPosition.Left;
			parrotButton1.Location = new Point(485, 361);
			parrotButton1.Name = "parrotButton1";
			parrotButton1.Size = new Size(200, 50);
			parrotButton1.SmoothingType = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			parrotButton1.TabIndex = 3;
			parrotButton1.TextColor = Color.DodgerBlue;
			parrotButton1.TextRenderingType = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
			parrotButton1.Vertical_Alignment = StringAlignment.Center;
			// 
			// Main
			// 
			AutoScaleDimensions = new SizeF(7F, 17F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(715, 439);
			Controls.Add(parrotForm1);
			FormBorderStyle = FormBorderStyle.None;
			Margin = new Padding(2, 3, 2, 3);
			Name = "Main";
			Text = "CatFoodManager";
			parrotForm1.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion

		private ReaLTaiizor.Forms.ParrotForm parrotForm1;
		private ReaLTaiizor.Controls.ParrotButton parrotButton1;
	}
}
