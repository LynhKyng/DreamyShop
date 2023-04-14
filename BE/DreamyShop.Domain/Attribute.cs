﻿using DreamyShop.Domain.Shared.Types;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DreamyShop.Domain
{
    [Table("Attributes")]
    public class Attribute : AuditEntity
    {
        public Attribute() { }
        [Key]
        public Guid Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Code { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        public AttributeType DataType { get; set; }
        public int SortOrder { get; set; }
        public bool IsVisibility { get; set; }
        public bool IsActive { get; set; }
        public bool IsUnique { get; set; }
        public string Note { get; set; }

        [InverseProperty(nameof(ProductAttribute.Attribute))]
        public virtual ICollection<ProductAttribute> ProductAttributes { get; set; }

        [InverseProperty(nameof(ProductAttributeDateTime.Attribute))]
        public virtual ICollection<ProductAttributeDateTime> ProductAttributeDateTimes { get; set; }

        [InverseProperty(nameof(ProductAttributeDecimal.Attribute))]
        public virtual ICollection<ProductAttributeDecimal> ProductAttributeDecimals { get; set; }

        [InverseProperty(nameof(ProductAttributeInt.Attribute))]
        public virtual ICollection<ProductAttributeInt> ProductAttributeInts { get; set; }

        [InverseProperty(nameof(ProductAttributeText.Attribute))]
        public virtual ICollection<ProductAttributeText> ProductAttributeTexts { get; set; }

        [InverseProperty(nameof(ProductAttributeVarchar.Attribute))]
        public virtual ICollection<ProductAttributeVarchar> ProductAttributeVarchars { get; set; }

        [InverseProperty(nameof(ProductVariantValue.Attribute))]
        public virtual ICollection<ProductVariantValue> ProductVariantValues { get; set; }
    }
}