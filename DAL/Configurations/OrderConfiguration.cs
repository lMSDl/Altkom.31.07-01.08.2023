using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Configurations
{
    internal class OrderConfiguration : EntityConfiguration<Order>
    {
        public override void Configure(EntityTypeBuilder<Order> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.DateTime).IsConcurrencyToken();

            builder.Property(x => x.Description).HasComputedColumnSql("'Data utworzenia zamówienia: ' + [s_Number]", stored: true);
            builder.Property(x => x.Number).HasDefaultValueSql("STR(NEXT VALUE FOR OrderNumber)");


            builder.Property(x => x.OrderType).HasConversion(
                x => x.ToString(),
                x => Enum.Parse<OrderType>(x)
                ) ;
        }
    }
}
