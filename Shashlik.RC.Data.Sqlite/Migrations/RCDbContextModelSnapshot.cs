﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Shashlik.RC.Data;

namespace Shashlik.RC.Data.Sqlite.Migrations
{
    [DbContext(typeof(RCDbContext))]
    partial class RCDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("Shashlik.RC.Data.Entities.AccountLocks", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<long>("LockEnd")
                        .HasColumnType("INTEGER");

                    b.Property<int>("LoginFailedCount")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("AccountLocks");
                });

            modelBuilder.Entity("Shashlik.RC.Data.Entities.Apps", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(32)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Desc")
                        .HasMaxLength(512)
                        .HasColumnType("TEXT");

                    b.Property<bool>("Enabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Apps");
                });

            modelBuilder.Entity("Shashlik.RC.Data.Entities.Configs", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Content")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("DeleteTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Desc")
                        .HasMaxLength(512)
                        .HasColumnType("TEXT");

                    b.Property<bool>("Enabled")
                        .HasColumnType("INTEGER");

                    b.Property<int>("EnvId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("EnvId");

                    b.ToTable("Configs");
                });

            modelBuilder.Entity("Shashlik.RC.Data.Entities.Envs", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AppId")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("TEXT");

                    b.Property<string>("Desc")
                        .HasMaxLength(32)
                        .HasColumnType("TEXT");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AppId", "Name")
                        .IsUnique();

                    b.ToTable("Envs");
                });

            modelBuilder.Entity("Shashlik.RC.Data.Entities.IpWhites", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("EnvId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Ip")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("EnvId");

                    b.ToTable("IpWhites");
                });

            modelBuilder.Entity("Shashlik.RC.Data.Entities.ModifyRecords", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AfterContent")
                        .HasColumnType("TEXT");

                    b.Property<string>("BeforeContent")
                        .HasColumnType("TEXT");

                    b.Property<int>("ConfigId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Desc")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("ModifyTime")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ConfigId");

                    b.ToTable("ModifyRecords");
                });

            modelBuilder.Entity("Shashlik.RC.Data.Entities.Configs", b =>
                {
                    b.HasOne("Shashlik.RC.Data.Entities.Envs", "Env")
                        .WithMany("Configs")
                        .HasForeignKey("EnvId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Env");
                });

            modelBuilder.Entity("Shashlik.RC.Data.Entities.Envs", b =>
                {
                    b.HasOne("Shashlik.RC.Data.Entities.Apps", "App")
                        .WithMany("Envs")
                        .HasForeignKey("AppId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("App");
                });

            modelBuilder.Entity("Shashlik.RC.Data.Entities.IpWhites", b =>
                {
                    b.HasOne("Shashlik.RC.Data.Entities.Envs", "Env")
                        .WithMany("IpWhites")
                        .HasForeignKey("EnvId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Env");
                });

            modelBuilder.Entity("Shashlik.RC.Data.Entities.ModifyRecords", b =>
                {
                    b.HasOne("Shashlik.RC.Data.Entities.Configs", "Config")
                        .WithMany("ModifyRecords")
                        .HasForeignKey("ConfigId");

                    b.Navigation("Config");
                });

            modelBuilder.Entity("Shashlik.RC.Data.Entities.Apps", b =>
                {
                    b.Navigation("Envs");
                });

            modelBuilder.Entity("Shashlik.RC.Data.Entities.Configs", b =>
                {
                    b.Navigation("ModifyRecords");
                });

            modelBuilder.Entity("Shashlik.RC.Data.Entities.Envs", b =>
                {
                    b.Navigation("Configs");

                    b.Navigation("IpWhites");
                });
#pragma warning restore 612, 618
        }
    }
}
