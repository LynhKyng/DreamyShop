﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DreamyShop.Domain.Shared.Dtos
{
    public class CreateProductAttributeValueDto
    {
        public int ProductId { get; set; }
        public int AttributeId { get; set; }

        public DateTime? DateTimeValue { get; set; }
        public decimal? DecimalValue { get; set; }
        public int? IntValue { get; set; }
        public string? VarcharValue { get; set; }

        public string? TextValue { get; set; }
    }
}
