using CatFoodManager.Core.Interfaces;
using CatFoodManager.Core.Models;
using CatFoodManager.Core.Services;
using CatFoodManager.Core.Statics;
using CommonTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CatFoodManager
{
    public partial class LowestPrice : Form
    {
        #region services
        private readonly IService<BestPrice> _lowestPriceService;
        #endregion

        public LowestPrice(IService<BestPrice> lowestPriceService)
        {
            InitializeComponent();
            InitializeComponents();
            InitializeContext();
            _lowestPriceService = lowestPriceService;
        }

        #region events
        private void btnCancel_Click(object sender, EventArgs e)
        {
            InitializeComponent();
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //todo: create a new LowestPrice and save it to the database
            var type = this.cbType.SelectedIndex < 0 ? ProductType.Others : (ProductType)this.cbType.SelectedIndex;
            BestPrice bestPrice = new()
            {
                Name = this.rtbName.Text,
                Type = type,
                LowestPrice = decimal.Parse(this.txtLowestPrice.Text),
                FinalPrice = string.IsNullOrWhiteSpace(this.txtFinalPrice.Text) ? default : decimal.Parse(this.txtFinalPrice.Text),
                PurchasedAt = string.IsNullOrWhiteSpace(this.PurchasedAt.Text) ? default : DateTime.Parse(this.PurchasedAt.Text),
                PicturePath = this.PicSelector.Text,
                Platform = (PlatformType)this.cbPlatform.SelectedIndex,
            };
            //todo: when this.txtFinalPrice.Text changed, set rbtPurchased.Checked
            bestPrice.HasPurchased = bestPrice.FinalPrice.HasValue && this.rbtPurchased.Checked;
            _lowestPriceService.Save(bestPrice);
            this.Close();
        }
        #endregion

        #region private methods

        private void InitializeContext()
        {
            Context.FillConnectionString(ConfigManager.GetConnectionString("SQLite"));
        }
        private void InitializeComponents()
        {
            this.cbType.DataSource = Enum.GetValues<ProductType>().Select(type => type.GetEnumDescription()).ToList();
            this.cbType.SelectedIndex = 2;
            this.cbPlatform.DataSource = Enum.GetValues<PlatformType>().ToList();
            this.cbPlatform.SelectedIndex = 1;
        }
        #endregion


    }
}
