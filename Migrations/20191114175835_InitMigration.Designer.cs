﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TradingSite.Database;

namespace TradingSite.Migrations
{
    [DbContext(typeof(AppDatabaseContext))]
    [Migration("20191114175835_InitMigration")]
    partial class InitMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("TradingSite.Data.Item", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("IdFirst");

                    b.Property<string>("IdSecond");

                    b.Property<string>("ImageHref");

                    b.Property<bool>("IsGood");

                    b.Property<string>("Name");

                    b.Property<double>("Price");

                    b.Property<double>("SteamPrice");

                    b.HasKey("Id");

                    b.ToTable("Items");
                });
#pragma warning restore 612, 618
        }
    }
}
