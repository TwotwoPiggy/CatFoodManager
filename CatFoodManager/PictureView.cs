using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CatFoodManager
{
	public partial class PictureView : Form
	{
		private string _picturePath;
		private Bitmap _bitmap;
		public PictureView(string picturePath)
		{
			InitializeComponent();
			_picturePath = picturePath;
		}

		private void PictureView_Load(object sender, EventArgs e)
		{
			//_picturePath = @"V:\Screenshots\IMG_5816.PNG";
			if (string.IsNullOrWhiteSpace(_picturePath))
			{
				MessageBox.Show("没有指定照片路径, 请检查!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			if (_bitmap != null)
			{
				_bitmap.Dispose();
			}
			_bitmap = new Bitmap(_picturePath);

			// Determine display size: use actual size unless it exceeds screen working area, then scale down
			var imgWidth = _bitmap.Width;
			var imgHeight = _bitmap.Height;
			var workingArea = Screen.FromControl(this).WorkingArea;
			var maxWidth = Math.Max(100, workingArea.Width - 100);
			var maxHeight = Math.Max(100, workingArea.Height - 100);
			float scale = 1f;
			if (imgWidth > maxWidth || imgHeight > maxHeight)
			{
				scale = Math.Min((float)maxWidth / imgWidth, (float)maxHeight / imgHeight);
			}
			var displayWidth = Math.Max(1, (int)(imgWidth * scale));
			var displayHeight = Math.Max(1, (int)(imgHeight * scale));

			// Show image at real size when it fits, otherwise zoom to scaled size
			pictureBox.Dock = DockStyle.None;
			pictureBox.SizeMode = scale < 1f ? PictureBoxSizeMode.Zoom : PictureBoxSizeMode.Normal;
			pictureBox.Size = new Size(displayWidth, displayHeight);
			pictureBox.Image = _bitmap;

			// Adjust layout and form to fit the picture
			pictureLayoutPanel.AutoSize = true;
			pictureLayoutPanel.RowCount = 1;
			pictureLayoutPanel.RowStyles.Clear();
			pictureLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
			// Calculate non-client chrome size so we can set the full window size correctly
			var nonClientWidth = this.Width - this.ClientSize.Width;
			var nonClientHeight = this.Height - this.ClientSize.Height;

			var targetWidth = pictureBox.Width + nonClientWidth;
			var targetHeight = pictureBox.Height + nonClientHeight;

			// Ensure window does not exceed working area
			if (targetWidth > workingArea.Width || targetHeight > workingArea.Height)
			{
				targetWidth = Math.Min(targetWidth, workingArea.Width);
				targetHeight = Math.Min(targetHeight, workingArea.Height);
				this.AutoScroll = true;
			}

			this.Size = new Size(targetWidth, targetHeight);
			this.StartPosition = FormStartPosition.CenterScreen;
		}

		private void PictureView_Leave(object sender, EventArgs e)
		{
			//if (_bitmap != null)
			//{
			//	_bitmap.Dispose();
			//}
		}
	}
}
