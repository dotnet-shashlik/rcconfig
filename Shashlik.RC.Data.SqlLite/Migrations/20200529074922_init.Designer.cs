﻿// <auto-generated />

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Shashlik.RC.Data.SqlLite.Migrations
{
    [DbContext(typeof(RCDbContext))]
    [Migration("20200529074922_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity("RC.Data.Entities.Apps", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(32);

                    b.Property<DateTime>("CreateTime");

                    b.Property<string>("Desc")
                        .HasMaxLength(512);

                    b.Property<bool>("Enabled");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32);

                    b.Property<string>("Password");

                    b.HasKey("Id");

                    b.ToTable("Apps");
                });

            modelBuilder.Entity("RC.Data.Entities.Configs", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content");

                    b.Property<DateTime?>("DeleteTime");

                    b.Property<string>("Desc")
                        .HasMaxLength(512);

                    b.Property<bool>("Enabled");

                    b.Property<int>("EnvId");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32);

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(32);

                    b.HasKey("Id");

                    b.HasIndex("EnvId", "Name")
                        .IsUnique();

                    b.ToTable("Configs");
                });

            modelBuilder.Entity("RC.Data.Entities.Envs", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AppId")
                        .IsRequired()
                        .HasMaxLength(32);

                    b.Property<string>("Desc")
                        .HasMaxLength(32);

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(64);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32);

                    b.Property<string>("NotifyUrl")
                        .HasMaxLength(512);

                    b.HasKey("Id");

                    b.HasIndex("AppId", "Name")
                        .IsUnique();

                    b.ToTable("Envs");
                });

            modelBuilder.Entity("RC.Data.Entities.IpWhites", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("EnvId");

                    b.Property<string>("Ip")
                        .IsRequired()
                        .HasMaxLength(32);

                    b.HasKey("Id");

                    b.HasIndex("EnvId");

                    b.ToTable("IpWhites");
                });

            modelBuilder.Entity("RC.Data.Entities.ModifyRecords", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AfterContent");

                    b.Property<string>("BeforeContent");

                    b.Property<int>("ConfigId");

                    b.Property<string>("Desc");

                    b.Property<DateTime>("ModifyTime");

                    b.HasKey("Id");

                    b.HasIndex("ConfigId");

                    b.ToTable("ModifyRecords");
                });

            modelBuilder.Entity("RC.Data.Entities.Configs", b =>
                {
                    b.HasOne("RC.Data.Entities.Envs", "Env")
                        .WithMany("Configs")
                        .HasForeignKey("EnvId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RC.Data.Entities.Envs", b =>
                {
                    b.HasOne("RC.Data.Entities.Apps", "App")
                        .WithMany("Envs")
                        .HasForeignKey("AppId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RC.Data.Entities.IpWhites", b =>
                {
                    b.HasOne("RC.Data.Entities.Envs", "Env")
                        .WithMany("IpWhites")
                        .HasForeignKey("EnvId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RC.Data.Entities.ModifyRecords", b =>
                {
                    b.HasOne("RC.Data.Entities.Configs", "Config")
                        .WithMany("ModifyRecords")
                        .HasForeignKey("ConfigId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
