﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Configurations
{
    internal class ProductConfiguration : EntityConfiguration<Product>
    {
        public override void Configure(EntityTypeBuilder<Product> builder)
        {
            base.Configure(builder);

            builder.ToTable("Products", x => x.IsTemporal(xx =>
            {
                xx.HasPeriodEnd("To"); //domyślna nazwa PeriodEnd
                xx.HasPeriodStart("From"); //domyślna nazwa PeriodStart
            }));

            builder.HasOne(x => x.Order).WithMany(x => x.Products).IsRequired();
            builder.Property(x => x.Timestamp).IsRowVersion();
        }
    }
}
