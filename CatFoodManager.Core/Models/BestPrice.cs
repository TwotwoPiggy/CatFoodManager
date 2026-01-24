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



        [Ignore]
        public DateTime CreatedAt { get; set; }
        [Ignore]
        public DateTime? UpdatedAt { get; set; }
        [Ignore]
        public DateTime? PurchasedAt { get; set; }

        [Column("CreatedAt")]
        public string CreatedAtString
        {
            get { return CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"); }
            set { CreatedAt = DateTime.Parse(value); }
        }

        [Column("UpdatedAt")]
        public string? UpdatedAtString
        {
            get { return UpdatedAt.HasValue ? UpdatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null; }
            set { UpdatedAt = string.IsNullOrEmpty(value) ? null : DateTime.Parse(value); }
        }

        [Column("PurchasedAt")]
        public string? PurchasedAtString
        {
            get { return PurchasedAt.HasValue ? PurchasedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null; }
            set { PurchasedAt = string.IsNullOrEmpty(value) ? null : DateTime.Parse(value); }
        }

        public BestPrice()
        {
            CreatedAt = DateTime.Now;
        }

    }
}
