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
			pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
			pictureBox.Image = _bitmap;
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
