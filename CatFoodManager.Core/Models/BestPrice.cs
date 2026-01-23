using CatFoodManager.Core.Statics;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace CatFoodManager.Core.Models
{
    public class BestPrice
    {
        [PrimaryKey, AutoIncrement]
        [DataMember]
        public long Id { get; set; }

        public string Name { get; set; }

        public ProductType Type { get; set; }

        public PlatformType Platform { get; set; }

        public decimal LowestPrice { get; set; }

        public bool HasPurchased { get; set; }

        public decimal? FinalPrice { get; set; }

        public string? PicturePath { get; set; }



        public DateTime CreatedAt { get; set; } //todo
        public DateTime UpdatedAt { get; set; }//todo
        public DateTime? PurchasedAt { get; set; }//todo

        public BestPrice()
        {
            CreatedAt = DateTime.Now;
        }

    }
}
 