﻿// <auto-generated />
using System;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DAL.Migrations
{
    [DbContext(typeof(Context))]
    partial class ContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Models.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DateTime")
                        .IsConcurrencyToken()
                        .HasPrecision(5)
                        .HasColumnType("datetime2(5)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("ModifiedDate")
                        .HasPrecision(5)
                        .HasColumnType("datetime2(5)");

                    b.HasKey("Id");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("From")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasPrecision(5)
                        .HasColumnType("datetime2(5)")
                        .HasColumnName("From");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("ModifiedDate")
                        .HasPrecision(5)
                        .HasColumnType("datetime2(5)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("s_Name");

                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.Property<float>("Price")
                        .HasColumnType("real");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<DateTime>("To")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasPrecision(5)
                        .HasColumnType("datetime2(5)")
                        .HasColumnName("To");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.ToTable("Products", (string)null);

                    b.ToTable(tb => tb.IsTemporal(ttb =>
                            {
                                ttb.UseHistoryTable("ProductsHistory");
                                ttb
                                    .HasPeriodStart("From")
                                    .HasColumnName("From");
                                ttb
                                    .HasPeriodEnd("To")
                                    .HasColumnName("To");
                            }));
                });

            modelBuilder.Entity("Models.Product", b =>
                {
                    b.HasOne("Models.Order", "Order")
                        .WithMany("Products")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("Models.Order", b =>
                {
                    b.Navigation("Products");
                });
#pragma warning restore 612, 618
        }
    }
}
