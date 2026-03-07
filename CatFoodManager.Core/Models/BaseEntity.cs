using SQLite;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace CatFoodManager.Core.Models
{
    public class BaseEntity
    {
        [PrimaryKey, AutoIncrement]
        [DataMember]
        public long Id { get; set; }

        public string Name { get; set; }


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
    }
}
