﻿// <auto-generated />
using System.Numerics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using VKRServer.DataBase;

#nullable disable

namespace VKRServer.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.15")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("VKRServer.DataBase.AdminData", b =>
                {
                    b.Property<int>("ID")
                        .HasColumnType("integer");

                    b.Property<string>("Department")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("MiddleName")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("ID");

                    b.ToTable("AdminData", (string)null);
                });

            modelBuilder.Entity("VKRServer.DataBase.MarkTable", b =>
                {
                    b.Property<int>("ID")
                        .HasColumnType("integer");

                    b.Property<string>("Attendance")
                        .HasColumnType("text");

                    b.Property<int>("Mark")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0);

                    b.HasKey("ID");

                    b.HasIndex("ID")
                        .IsUnique();

                    b.ToTable("MarkTable", (string)null);
                });

            modelBuilder.Entity("VKRServer.DataBase.ModerData", b =>
                {
                    b.Property<int>("ID")
                        .HasColumnType("integer");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<BigInteger>("Key")
                        .HasColumnType("numeric(80, 0)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("MiddleName")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("ID");

                    b.ToTable("ModerData", (string)null);
                });

            modelBuilder.Entity("VKRServer.DataBase.TimeTable", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ID"));

                    b.Property<string>("Audience")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.Property<int>("DayOfWeek")
                        .HasColumnType("integer");

                    b.Property<int>("EndTime")
                        .HasColumnType("integer");

                    b.Property<string>("Groop")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.Property<int>("ModerID")
                        .HasColumnType("integer");

                    b.Property<int>("N")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("StartTime")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.HasIndex("Groop", "DayOfWeek", "N", "ModerID")
                        .IsUnique();

                    b.HasIndex("ModerID", "DayOfWeek", "N", "Groop")
                        .IsUnique();

                    b.ToTable("TimeTable", (string)null);
                });

            modelBuilder.Entity("VKRServer.DataBase.User", b =>
                {
                    b.Property<int>("ID")
                        .HasColumnType("integer");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("character varying(60)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Token")
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.HasIndex("Login")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("VKRServer.DataBase.UserData", b =>
                {
                    b.Property<int>("ID")
                        .HasColumnType("integer");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("Groop")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("MiddleName")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("ID");

                    b.ToTable("UserData", (string)null);
                });

            modelBuilder.Entity("VKRServer.DataBase.AdminData", b =>
                {
                    b.HasOne("VKRServer.DataBase.User", "User")
                        .WithOne("AdminData")
                        .HasForeignKey("VKRServer.DataBase.AdminData", "ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("VKRServer.DataBase.MarkTable", b =>
                {
                    b.HasOne("VKRServer.DataBase.UserData", "UserData")
                        .WithOne("MarkTable")
                        .HasForeignKey("VKRServer.DataBase.MarkTable", "ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserData");
                });

            modelBuilder.Entity("VKRServer.DataBase.ModerData", b =>
                {
                    b.HasOne("VKRServer.DataBase.User", "User")
                        .WithOne("ModerData")
                        .HasForeignKey("VKRServer.DataBase.ModerData", "ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("VKRServer.DataBase.UserData", b =>
                {
                    b.HasOne("VKRServer.DataBase.User", "User")
                        .WithOne("UserData")
                        .HasForeignKey("VKRServer.DataBase.UserData", "ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("VKRServer.DataBase.User", b =>
                {
                    b.Navigation("AdminData");

                    b.Navigation("ModerData");

                    b.Navigation("UserData");
                });

            modelBuilder.Entity("VKRServer.DataBase.UserData", b =>
                {
                    b.Navigation("MarkTable");
                });
#pragma warning restore 612, 618
        }
    }
}
