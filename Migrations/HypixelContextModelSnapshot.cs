﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using hypixel;

namespace hypixel.Migrations
{
    [DbContext(typeof(HypixelContext))]
    partial class HypixelContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("dev.BazaarPull", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime");

                    b.HasKey("Id");

                    b.HasIndex("Timestamp");

                    b.ToTable("BazaarPull");
                });

            modelBuilder.Entity("dev.BuyOrder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("Amount")
                        .HasColumnType("int");

                    b.Property<short>("Orders")
                        .HasColumnType("smallint");

                    b.Property<double>("PricePerUnit")
                        .HasColumnType("double");

                    b.Property<int?>("ProductInfoId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProductInfoId");

                    b.ToTable("BuyOrder");
                });

            modelBuilder.Entity("dev.ProductInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ProductId")
                        .HasColumnType("varchar(40)")
                        .HasMaxLength(40);

                    b.Property<int?>("PullInstanceId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("PullInstanceId");

                    b.ToTable("BazaarPrices");
                });

            modelBuilder.Entity("dev.QuickStatus", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<long>("BuyMovingWeek")
                        .HasColumnType("bigint");

                    b.Property<int>("BuyOrders")
                        .HasColumnType("int");

                    b.Property<double>("BuyPrice")
                        .HasColumnType("double");

                    b.Property<long>("BuyVolume")
                        .HasColumnType("bigint");

                    b.Property<int?>("QuickStatusID")
                        .HasColumnType("int");

                    b.Property<long>("SellMovingWeek")
                        .HasColumnType("bigint");

                    b.Property<int>("SellOrders")
                        .HasColumnType("int");

                    b.Property<double>("SellPrice")
                        .HasColumnType("double");

                    b.Property<long>("SellVolume")
                        .HasColumnType("bigint");

                    b.HasKey("ID");

                    b.HasIndex("QuickStatusID")
                        .IsUnique();

                    b.ToTable("QuickStatus");
                });

            modelBuilder.Entity("dev.SellOrder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("Amount")
                        .HasColumnType("int");

                    b.Property<short>("Orders")
                        .HasColumnType("smallint");

                    b.Property<double>("PricePerUnit")
                        .HasColumnType("double");

                    b.Property<int?>("ProductInfoId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProductInfoId");

                    b.ToTable("SellOrder");
                });

            modelBuilder.Entity("hypixel.AlternativeName", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("DBItemId")
                        .HasColumnType("MEDIUMINT(9)");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(767)")
                        .HasAnnotation("MySQL:Charset", "utf8");

                    b.Property<int>("OccuredTimes")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DBItemId");

                    b.HasIndex("Name");

                    b.ToTable("AltItemNames");
                });

            modelBuilder.Entity("hypixel.AveragePrice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<float>("Avg")
                        .HasColumnType("float");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime");

                    b.Property<int>("ItemId")
                        .HasColumnType("int");

                    b.Property<float>("Max")
                        .HasColumnType("float");

                    b.Property<float>("Min")
                        .HasColumnType("float");

                    b.Property<int>("Volume")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ItemId", "Date")
                        .IsUnique();

                    b.ToTable("Prices");
                });

            modelBuilder.Entity("hypixel.DBItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("MEDIUMINT(9)");

                    b.Property<int>("Category")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasAnnotation("MySQL:Charset", "utf8");

                    b.Property<bool>("Enchantable")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Extra")
                        .HasColumnType("text")
                        .HasAnnotation("MySQL:Charset", "utf8");

                    b.Property<int>("HitCount")
                        .HasColumnType("int");

                    b.Property<string>("IconUrl")
                        .HasColumnType("text");

                    b.Property<bool>("IsBazaar")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("MinecraftType")
                        .HasColumnType("varchar(44)")
                        .HasMaxLength(44)
                        .HasAnnotation("MySQL:Charset", "utf8");

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasAnnotation("MySQL:Charset", "utf8");

                    b.Property<bool>("Reforgeable")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Tag")
                        .HasColumnType("varchar(44)")
                        .HasMaxLength(44);

                    b.Property<int>("Tier")
                        .HasColumnType("int");

                    b.Property<string>("color")
                        .HasColumnType("varchar(12)")
                        .HasMaxLength(12);

                    b.HasKey("Id");

                    b.HasIndex("Tag")
                        .IsUnique();

                    b.ToTable("Items");
                });

            modelBuilder.Entity("hypixel.Device", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ConnectionId")
                        .HasColumnType("varchar(32)")
                        .HasMaxLength(32);

                    b.Property<int?>("GoogleUserId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(40)")
                        .HasMaxLength(40);

                    b.Property<string>("Token")
                        .HasColumnType("text");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ConnectionId");

                    b.HasIndex("GoogleUserId");

                    b.ToTable("Device");
                });

            modelBuilder.Entity("hypixel.Enchantment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("ItemType")
                        .HasColumnType("MEDIUMINT(9)");

                    b.Property<byte>("Level")
                        .HasColumnType("TINYINT(3)");

                    b.Property<int>("SaveAuctionId")
                        .HasColumnType("int");

                    b.Property<byte>("Type")
                        .HasColumnType("TINYINT(3)");

                    b.HasKey("Id");

                    b.HasIndex("SaveAuctionId");

                    b.HasIndex("ItemType", "Type", "Level");

                    b.ToTable("Enchantment");
                });

            modelBuilder.Entity("hypixel.GoogleUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("GoogleId")
                        .HasColumnType("char(32)");

                    b.Property<string>("MinecraftUuid")
                        .HasColumnType("char(32)");

                    b.Property<DateTime>("PremiumExpires")
                        .HasColumnType("datetime");

                    b.Property<int>("ReferedBy")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GoogleId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("hypixel.NBTKey", b =>
                {
                    b.Property<short>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint");

                    b.Property<string>("Slug")
                        .HasColumnType("varchar(45)")
                        .HasMaxLength(45);

                    b.HasKey("Id");

                    b.HasIndex("Slug");

                    b.ToTable("NBTKeys");
                });

            modelBuilder.Entity("hypixel.NBTLookup", b =>
                {
                    b.Property<int>("AuctionId")
                        .HasColumnType("int");

                    b.Property<short>("KeyId")
                        .HasColumnType("smallint");

                    b.Property<long>("Value")
                        .HasColumnType("bigint");

                    b.HasKey("AuctionId", "KeyId");

                    b.HasIndex("KeyId", "Value");

                    b.ToTable("NBTLookups");
                });

            modelBuilder.Entity("hypixel.NBTValue", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<short>("KeyId")
                        .HasColumnType("smallint");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("NBTValues");
                });

            modelBuilder.Entity("hypixel.NbtData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<byte[]>("data")
                        .HasColumnType("varbinary(1000)")
                        .HasMaxLength(1000);

                    b.HasKey("Id");

                    b.ToTable("NbtData");
                });

            modelBuilder.Entity("hypixel.Player", b =>
                {
                    b.Property<string>("UuId")
                        .HasColumnType("char(32)");

                    b.Property<bool>("ChangedFlag")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("HitCount")
                        .HasColumnType("int");

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(16)")
                        .HasMaxLength(16);

                    b.Property<DateTime>("UpdatedAt")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime");

                    b.HasKey("UuId");

                    b.HasIndex("Id");

                    b.HasIndex("Name");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("hypixel.SaveAuction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<short>("AnvilUses")
                        .HasColumnType("smallint");

                    b.Property<string>("AuctioneerId")
                        .HasColumnType("char(32)");

                    b.Property<bool>("Bin")
                        .HasColumnType("bit");

                    b.Property<byte>("Category")
                        .HasColumnType("TINYINT(2)");

                    b.Property<bool>("Claimed")
                        .HasColumnType("bit");

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<DateTime>("End")
                        .HasColumnType("datetime");

                    b.Property<long>("HighestBidAmount")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("ItemCreatedAt")
                        .HasColumnType("datetime");

                    b.Property<int>("ItemId")
                        .HasColumnType("MEDIUMINT(9)");

                    b.Property<string>("ItemName")
                        .HasColumnType("varchar(45)")
                        .HasMaxLength(45)
                        .HasAnnotation("MySQL:Charset", "utf8");

                    b.Property<int?>("NbtDataId")
                        .HasColumnType("int");

                    b.Property<string>("ProfileId")
                        .HasColumnType("char(32)");

                    b.Property<byte>("Reforge")
                        .HasColumnType("TINYINT(2)");

                    b.Property<int>("SellerId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Start")
                        .HasColumnType("datetime");

                    b.Property<long>("StartingBid")
                        .HasColumnType("bigint");

                    b.Property<string>("Tag")
                        .HasColumnType("varchar(40)")
                        .HasMaxLength(40);

                    b.Property<byte>("Tier")
                        .HasColumnType("TINYINT(2)");

                    b.Property<string>("Uuid")
                        .HasColumnType("char(32)");

                    b.HasKey("Id");

                    b.HasIndex("End");

                    b.HasIndex("NbtDataId");

                    b.HasIndex("SellerId");

                    b.HasIndex("Uuid")
                        .IsUnique();

                    b.HasIndex("ItemId", "End");

                    b.ToTable("Auctions");
                });

            modelBuilder.Entity("hypixel.SaveBids", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<long>("Amount")
                        .HasColumnType("bigint");

                    b.Property<string>("Bidder")
                        .HasColumnType("char(32)");

                    b.Property<int>("BidderId")
                        .HasColumnType("int");

                    b.Property<string>("ProfileId")
                        .HasColumnType("char(32)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime");

                    b.Property<int?>("Uuid")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BidderId");

                    b.HasIndex("Uuid");

                    b.ToTable("Bids");
                });

            modelBuilder.Entity("hypixel.SubscribeItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("GeneratedAt")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime");

                    b.Property<DateTime>("NotTriggerAgainBefore")
                        .HasColumnType("datetime");

                    b.Property<long>("Price")
                        .HasColumnType("bigint");

                    b.Property<string>("TopicId")
                        .HasColumnType("varchar(45)")
                        .HasMaxLength(45);

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("SubscribeItem");
                });

            modelBuilder.Entity("hypixel.UuId", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int?>("SaveAuctionId")
                        .HasColumnType("int");

                    b.Property<int?>("SaveAuctionId1")
                        .HasColumnType("int");

                    b.Property<string>("value")
                        .HasColumnType("char(32)");

                    b.HasKey("Id");

                    b.HasIndex("SaveAuctionId");

                    b.HasIndex("SaveAuctionId1");

                    b.ToTable("UuId");
                });

            modelBuilder.Entity("dev.BuyOrder", b =>
                {
                    b.HasOne("dev.ProductInfo", null)
                        .WithMany("BuySummery")
                        .HasForeignKey("ProductInfoId");
                });

            modelBuilder.Entity("dev.ProductInfo", b =>
                {
                    b.HasOne("dev.BazaarPull", "PullInstance")
                        .WithMany("Products")
                        .HasForeignKey("PullInstanceId");
                });

            modelBuilder.Entity("dev.QuickStatus", b =>
                {
                    b.HasOne("dev.ProductInfo", "Info")
                        .WithOne("QuickStatus")
                        .HasForeignKey("dev.QuickStatus", "QuickStatusID");
                });

            modelBuilder.Entity("dev.SellOrder", b =>
                {
                    b.HasOne("dev.ProductInfo", null)
                        .WithMany("SellSummary")
                        .HasForeignKey("ProductInfoId");
                });

            modelBuilder.Entity("hypixel.AlternativeName", b =>
                {
                    b.HasOne("hypixel.DBItem", null)
                        .WithMany("Names")
                        .HasForeignKey("DBItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("hypixel.Device", b =>
                {
                    b.HasOne("hypixel.GoogleUser", null)
                        .WithMany("Devices")
                        .HasForeignKey("GoogleUserId");
                });

            modelBuilder.Entity("hypixel.Enchantment", b =>
                {
                    b.HasOne("hypixel.SaveAuction", null)
                        .WithMany("Enchantments")
                        .HasForeignKey("SaveAuctionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("hypixel.NBTLookup", b =>
                {
                    b.HasOne("hypixel.SaveAuction", null)
                        .WithMany("NBTLookup")
                        .HasForeignKey("AuctionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("hypixel.SaveAuction", b =>
                {
                    b.HasOne("hypixel.NbtData", "NbtData")
                        .WithMany()
                        .HasForeignKey("NbtDataId");
                });

            modelBuilder.Entity("hypixel.SaveBids", b =>
                {
                    b.HasOne("hypixel.SaveAuction", "Auction")
                        .WithMany("Bids")
                        .HasForeignKey("Uuid");
                });

            modelBuilder.Entity("hypixel.UuId", b =>
                {
                    b.HasOne("hypixel.SaveAuction", null)
                        .WithMany("ClaimedBids")
                        .HasForeignKey("SaveAuctionId");

                    b.HasOne("hypixel.SaveAuction", null)
                        .WithMany("CoopMembers")
                        .HasForeignKey("SaveAuctionId1");
                });
#pragma warning restore 612, 618
        }
    }
}
