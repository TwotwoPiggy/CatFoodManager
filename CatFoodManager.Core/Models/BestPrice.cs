using CatFoodManager.Core.Statics;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

namespace CatFoodManager.Core.Models
{
    public class BestPrice: BaseEntity
    {
        public ProductType Type { get; set; }

        public PlatformType Platform { get; set; }

        public decimal LowestPrice { get; set; }

        public bool HasPurchased { get; set; }

        public decimal? FinalPrice { get; set; }

        public string? PicturePath { get; set; }


        public BestPrice()
        {
            CreatedAt = DateTime.Now;
        }

    }
}
