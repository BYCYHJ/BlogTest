﻿// <auto-generated />
using System;
using FileService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FileService.Infrastructure.Migrations
{
    [DbContext(typeof(FileDbContext))]
    [Migration("20240325163010_Init")]
    partial class Init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("FileService.Domain.Entities.ServerFile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("CreateOnTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("FileSha256Hash")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<long>("FileSizeBytes")
                        .HasColumnType("bigint");

                    b.Property<string>("FileUrl")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("UpdateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("UserId")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("blog_files", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
