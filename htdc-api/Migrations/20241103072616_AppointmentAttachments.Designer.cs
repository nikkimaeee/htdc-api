﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using htdc_api.Authentication;

#nullable disable

namespace htdc_api.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241103072616_AppointmentAttachments")]
    partial class AppointmentAttachments
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("htdc_api.Models.AppointmentAttachments", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int?>("AppointmentInformationId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AppointmentInformationId");

                    b.ToTable("AppointmentAttachments");
                });

            modelBuilder.Entity("htdc_api.Models.AppointmentInformation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<decimal?>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("AppointmentDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("AppointmentDuration")
                        .HasColumnType("int");

                    b.Property<string>("AppointmentTimeIds")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AspNetUserId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateUpdated")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPaid")
                        .HasColumnType("bit");

                    b.Property<bool>("IsWalkIn")
                        .HasColumnType("bit");

                    b.Property<string>("MedCert")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PatientInformationId")
                        .HasColumnType("int");

                    b.Property<int>("PaymentMethod")
                        .HasColumnType("int");

                    b.Property<string>("Prescriptions")
                        .HasMaxLength(10000)
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<string>("ReferenceNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("TransactionId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("AppointmentInformations");
                });

            modelBuilder.Entity("htdc_api.Models.AppointmentTime", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("From")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<int>("MilitaryTime")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("To")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("AppointmentTimes");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            DateCreated = new DateTime(2024, 10, 25, 9, 47, 33, 959, DateTimeKind.Utc).AddTicks(5679),
                            From = "09:00:00",
                            IsActive = true,
                            MilitaryTime = 9,
                            Name = "09:00am - 10:00am",
                            To = "10:00:00"
                        },
                        new
                        {
                            Id = 2,
                            DateCreated = new DateTime(2024, 10, 25, 9, 47, 33, 959, DateTimeKind.Utc).AddTicks(5679),
                            From = "10:00:00",
                            IsActive = true,
                            MilitaryTime = 10,
                            Name = "10:00am - 11:00am",
                            To = "11:00:00"
                        },
                        new
                        {
                            Id = 3,
                            DateCreated = new DateTime(2024, 10, 25, 9, 47, 33, 959, DateTimeKind.Utc).AddTicks(5679),
                            From = "11:00:00",
                            IsActive = true,
                            MilitaryTime = 11,
                            Name = "11:00am - 12:00pm",
                            To = "12:00:00"
                        },
                        new
                        {
                            Id = 4,
                            DateCreated = new DateTime(2024, 10, 25, 9, 47, 33, 959, DateTimeKind.Utc).AddTicks(5679),
                            From = "12:00:00",
                            IsActive = true,
                            MilitaryTime = 12,
                            Name = "12:00pm - 01:00pm",
                            To = "13:00:00"
                        },
                        new
                        {
                            Id = 5,
                            DateCreated = new DateTime(2024, 10, 25, 9, 47, 33, 959, DateTimeKind.Utc).AddTicks(5679),
                            From = "13:00:00",
                            IsActive = true,
                            MilitaryTime = 13,
                            Name = "01:00pm - 02:00pm",
                            To = "14:00:00"
                        },
                        new
                        {
                            Id = 6,
                            DateCreated = new DateTime(2024, 10, 25, 9, 47, 33, 959, DateTimeKind.Utc).AddTicks(5679),
                            From = "14:00:00",
                            IsActive = true,
                            MilitaryTime = 14,
                            Name = "02:00pm - 03:00pm",
                            To = "15:00:00"
                        },
                        new
                        {
                            Id = 7,
                            DateCreated = new DateTime(2024, 10, 25, 9, 47, 33, 959, DateTimeKind.Utc).AddTicks(5679),
                            From = "15:00:00",
                            IsActive = true,
                            MilitaryTime = 15,
                            Name = "03:00pm - 04:00pm",
                            To = "16:00:00"
                        },
                        new
                        {
                            Id = 8,
                            DateCreated = new DateTime(2024, 10, 25, 9, 47, 33, 959, DateTimeKind.Utc).AddTicks(5679),
                            From = "16:00:00",
                            IsActive = true,
                            MilitaryTime = 16,
                            Name = "04:00pm - 05:00pm",
                            To = "17:00:00"
                        },
                        new
                        {
                            Id = 9,
                            DateCreated = new DateTime(2024, 10, 25, 9, 47, 33, 959, DateTimeKind.Utc).AddTicks(5679),
                            From = "17:00:00",
                            IsActive = true,
                            MilitaryTime = 17,
                            Name = "05:00pm - 06:00pm",
                            To = "18:00:00"
                        },
                        new
                        {
                            Id = 10,
                            DateCreated = new DateTime(2024, 10, 25, 9, 47, 33, 959, DateTimeKind.Utc).AddTicks(5679),
                            From = "18:00:00",
                            IsActive = true,
                            MilitaryTime = 18,
                            Name = "06:00pm - 07:00pm",
                            To = "19:00:00"
                        });
                });

            modelBuilder.Entity("htdc_api.Models.PatientInformation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPregnant")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPwd")
                        .HasColumnType("bit");

                    b.Property<bool>("IsSenior")
                        .HasColumnType("bit");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("PatientInformations");
                });

            modelBuilder.Entity("htdc_api.Models.Products", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<bool>("AllowPregnant")
                        .HasColumnType("bit");

                    b.Property<bool>("AllowPwd")
                        .HasColumnType("bit");

                    b.Property<bool>("AllowSenior")
                        .HasColumnType("bit");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Duration")
                        .HasColumnType("int");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageFileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("htdc_api.Models.UserProfile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("AspNetUserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Avatar")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPregnant")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPwd")
                        .HasColumnType("bit");

                    b.Property<bool>("IsSenior")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastLogin")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PatientInformationId")
                        .HasColumnType("int");

                    b.Property<string>("Phone")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("PatientInformationId");

                    b.ToTable("UserProfiles");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            AspNetUserId = "b74ddd14-6340-4840-95c2-db12554843e5",
                            Avatar = "",
                            DateCreated = new DateTime(2022, 9, 19, 9, 47, 33, 959, DateTimeKind.Utc).AddTicks(5679),
                            FirstName = "Admin",
                            IsActive = true,
                            IsPregnant = false,
                            IsPwd = false,
                            IsSenior = false,
                            LastName = "Admin"
                        });
                });

            modelBuilder.Entity("htdc_api.Models.VerificationTokens", b =>
                {
                    b.Property<string>("Token")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("AspNetUserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ExpiresDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("IssuedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Token");

                    b.ToTable("VerificationTokens");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);

                    b.HasData(
                        new
                        {
                            Id = "fab4fac1-c546-41de-aebc-a14da6895711",
                            ConcurrencyStamp = "1",
                            Name = "Admin",
                            NormalizedName = "ADMIN"
                        },
                        new
                        {
                            Id = "466231f7-f3e1-4580-b13d-d3a26dbb74c1",
                            ConcurrencyStamp = "3",
                            Name = "Patient",
                            NormalizedName = "PATIENT"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);

                    b.HasData(
                        new
                        {
                            Id = "b74ddd14-6340-4840-95c2-db12554843e5",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "505d3481-aefd-4c46-9901-01867e1ba956",
                            Email = "admin@test.com",
                            EmailConfirmed = false,
                            LockoutEnabled = false,
                            NormalizedEmail = "ADMIN@TEST.COM",
                            NormalizedUserName = "ADMIN",
                            PasswordHash = "AQAAAAEAACcQAAAAEC+JtF2rYLpMmJtZ5Yt6vVyplT2sEucaRDGi533vIB1Tg1BRwqUyYmjzvN7J1EPqyQ==",
                            PhoneNumber = "1234567890",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "30e5f131-4d2a-4f34-9225-f8d8390ba5d4",
                            TwoFactorEnabled = false,
                            UserName = "Admin"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);

                    b.HasData(
                        new
                        {
                            UserId = "b74ddd14-6340-4840-95c2-db12554843e5",
                            RoleId = "fab4fac1-c546-41de-aebc-a14da6895711"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("htdc_api.Models.AppointmentAttachments", b =>
                {
                    b.HasOne("htdc_api.Models.AppointmentInformation", null)
                        .WithMany("Attachments")
                        .HasForeignKey("AppointmentInformationId");
                });

            modelBuilder.Entity("htdc_api.Models.UserProfile", b =>
                {
                    b.HasOne("htdc_api.Models.PatientInformation", "PatientInformation")
                        .WithMany()
                        .HasForeignKey("PatientInformationId");

                    b.Navigation("PatientInformation");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("htdc_api.Models.AppointmentInformation", b =>
                {
                    b.Navigation("Attachments");
                });
#pragma warning restore 612, 618
        }
    }
}
