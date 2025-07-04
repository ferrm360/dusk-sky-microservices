﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ModerationService.Api.Infrastructure;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ModerationService.Api.Migrations
{
    [DbContext(typeof(ModerationDbContext))]
    partial class ModerationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ModerationService.Api.Models.Report", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("char(36)");

                    b.Property<string>("ContentType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Reason")
                        .HasColumnType("text");

                    b.Property<DateTime>("ReportedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamptz")
                        .HasColumnName("reported_at")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("ReportedUserId")
                        .IsRequired()
                        .HasColumnType("char(36)")
                        .HasColumnName("reported_user_id");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Report", (string)null);
                });

            modelBuilder.Entity("ModerationService.Api.Models.Sanction", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("end_date");

                    b.Property<string>("Reason")
                        .HasColumnType("text");

                    b.Property<string>("ReportId")
                        .HasColumnType("char(36)")
                        .HasColumnName("report_id");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("start_date");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("char(36)")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("ReportId")
                        .IsUnique();

                    b.ToTable("Sanction", (string)null);
                });

            modelBuilder.Entity("ModerationService.Api.Models.Sanction", b =>
                {
                    b.HasOne("ModerationService.Api.Models.Report", "Report")
                        .WithOne()
                        .HasForeignKey("ModerationService.Api.Models.Sanction", "ReportId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Report");
                });
#pragma warning restore 612, 618
        }
    }
}
