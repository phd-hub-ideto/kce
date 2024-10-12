﻿// <auto-generated />
using System;
using KhumaloCraft.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace KhumaloCraft.Data.Entities.Migrations
{
    [DbContext(typeof(KhumaloCraftDbContext))]
    [Migration("20241008124603_MigrationCreate_DalUserRole")]
    partial class MigrationCreate_DalUserRole
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("KhumaloCraft.Data.Entities.Entities.DalPermission", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.HasKey("Id")
                        .HasName("PK_Permission_Id");

                    SqlServerKeyBuilderExtensions.IsClustered(b.HasKey("Id"));

                    b.ToTable("Permission");
                });

            modelBuilder.Entity("KhumaloCraft.Data.Entities.Entities.DalPermissionSecurityEntityType", b =>
                {
                    b.Property<int>("PermissionId")
                        .HasColumnType("int");

                    b.Property<int>("SecurityEntityTypeId")
                        .HasColumnType("int");

                    b.HasKey("PermissionId", "SecurityEntityTypeId")
                        .HasName("PK_PermissionSecurityEntityType_PermissionId_SecurityEntityTypeId");

                    SqlServerKeyBuilderExtensions.IsClustered(b.HasKey("PermissionId", "SecurityEntityTypeId"));

                    b.HasIndex("SecurityEntityTypeId");

                    b.ToTable("PermissionSecurityEntityType");
                });

            modelBuilder.Entity("KhumaloCraft.Data.Entities.Entities.DalRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("SecurityEntityTypeId")
                        .HasColumnType("int");

                    b.Property<int>("LastEditedByUserId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("SysEndTime")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasColumnName("SysEndTime");

                    b.Property<DateTime>("SysStartTime")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasColumnName("SysStartTime");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id", "SecurityEntityTypeId")
                        .HasName("PK_Role_Id_SecurityEntityTypeId");

                    SqlServerKeyBuilderExtensions.IsClustered(b.HasKey("Id", "SecurityEntityTypeId"));

                    b.HasIndex("LastEditedByUserId");

                    b.HasIndex("SecurityEntityTypeId");

                    b.ToTable("Role", (string)null);

                    b.ToTable(tb => tb.IsTemporal(ttb =>
                            {
                                ttb.UseHistoryTable("RoleHistory");
                                ttb
                                    .HasPeriodStart("SysStartTime")
                                    .HasColumnName("SysStartTime");
                                ttb
                                    .HasPeriodEnd("SysEndTime")
                                    .HasColumnName("SysEndTime");
                            }));
                });

            modelBuilder.Entity("KhumaloCraft.Data.Entities.Entities.DalRolePermission", b =>
                {
                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.Property<int>("SecurityEntityTypeId")
                        .HasColumnType("int");

                    b.Property<int>("PermissionId")
                        .HasColumnType("int");

                    b.Property<int>("LastEditedByUserId")
                        .HasColumnType("int");

                    b.Property<DateTime>("SysEndTime")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasColumnName("SysEndTime");

                    b.Property<DateTime>("SysStartTime")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasColumnName("SysStartTime");

                    b.HasKey("RoleId", "SecurityEntityTypeId", "PermissionId")
                        .HasName("PK_RolePermission_RoleId_SecurityEntityTypeId_PermissionId");

                    SqlServerKeyBuilderExtensions.IsClustered(b.HasKey("RoleId", "SecurityEntityTypeId", "PermissionId"));

                    b.HasIndex("LastEditedByUserId");

                    b.HasIndex("PermissionId");

                    b.HasIndex("SecurityEntityTypeId");

                    b.ToTable("RolePermission", (string)null);

                    b.ToTable(tb => tb.IsTemporal(ttb =>
                            {
                                ttb.UseHistoryTable("RolePermissionHistory");
                                ttb
                                    .HasPeriodStart("SysStartTime")
                                    .HasColumnName("SysStartTime");
                                ttb
                                    .HasPeriodEnd("SysEndTime")
                                    .HasColumnName("SysEndTime");
                            }));
                });

            modelBuilder.Entity("KhumaloCraft.Data.Entities.Entities.DalSecurityEntityType", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.HasKey("Id")
                        .HasName("PK_SecurityEntityType_Id");

                    SqlServerKeyBuilderExtensions.IsClustered(b.HasKey("Id"));

                    b.ToTable("SecurityEntityType");
                });

            modelBuilder.Entity("KhumaloCraft.Data.Entities.Entities.DalUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("ActivatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ActivationEmailSentDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Deleted")
                        .HasColumnType("bit");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<int?>("ImageReferenceId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("LastLoginDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("MobileNumber")
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("ValidatedEmail")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "Username" }, "UQ_User_Username")
                        .IsUnique();

                    b.ToTable("User");
                });

            modelBuilder.Entity("KhumaloCraft.Data.Entities.Entities.DalUserRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("LastEditedByUserId")
                        .HasColumnType("int");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.Property<int>("SecurityEntityTypeId")
                        .HasColumnType("int");

                    b.Property<DateTime>("SysEndTime")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasColumnName("SysEndTime");

                    b.Property<DateTime>("SysStartTime")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasColumnName("SysStartTime");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id")
                        .HasName("PK_UserRole_Id");

                    SqlServerKeyBuilderExtensions.IsClustered(b.HasKey("Id"));

                    b.HasIndex("LastEditedByUserId");

                    b.HasIndex("RoleId");

                    b.HasIndex("SecurityEntityTypeId");

                    b.HasIndex("UserId");

                    b.ToTable("UserRole", (string)null);

                    b.ToTable(tb => tb.IsTemporal(ttb =>
                            {
                                ttb.UseHistoryTable("UserRoleHistory");
                                ttb
                                    .HasPeriodStart("SysStartTime")
                                    .HasColumnName("SysStartTime");
                                ttb
                                    .HasPeriodEnd("SysEndTime")
                                    .HasColumnName("SysEndTime");
                            }));
                });

            modelBuilder.Entity("KhumaloCraft.Data.Entities.Entities.DalPermissionSecurityEntityType", b =>
                {
                    b.HasOne("KhumaloCraft.Data.Entities.Entities.DalPermission", "Permission")
                        .WithMany()
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_PermissionSecurityEntityType_PermissionId");

                    b.HasOne("KhumaloCraft.Data.Entities.Entities.DalSecurityEntityType", "SecurityEntityType")
                        .WithMany()
                        .HasForeignKey("SecurityEntityTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_PermissionSecurityEntityType_SecurityEntityTypeId");

                    b.Navigation("Permission");

                    b.Navigation("SecurityEntityType");
                });

            modelBuilder.Entity("KhumaloCraft.Data.Entities.Entities.DalRole", b =>
                {
                    b.HasOne("KhumaloCraft.Data.Entities.Entities.DalUser", "LastEditedByUser")
                        .WithMany()
                        .HasForeignKey("LastEditedByUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Role_LastEditedByUserId");

                    b.HasOne("KhumaloCraft.Data.Entities.Entities.DalSecurityEntityType", "SecurityEntityType")
                        .WithMany()
                        .HasForeignKey("SecurityEntityTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Role_SecurityEntityTypeId");

                    b.Navigation("LastEditedByUser");

                    b.Navigation("SecurityEntityType");
                });

            modelBuilder.Entity("KhumaloCraft.Data.Entities.Entities.DalRolePermission", b =>
                {
                    b.HasOne("KhumaloCraft.Data.Entities.Entities.DalUser", "LastEditedByUser")
                        .WithMany()
                        .HasForeignKey("LastEditedByUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_RolePermission_LastEditedByUserId");

                    b.HasOne("KhumaloCraft.Data.Entities.Entities.DalPermission", "Permission")
                        .WithMany()
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_RolePermission_PermissionId");

                    b.HasOne("KhumaloCraft.Data.Entities.Entities.DalRole", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .HasPrincipalKey("Id")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_RolePermission_RoleId");

                    b.HasOne("KhumaloCraft.Data.Entities.Entities.DalSecurityEntityType", "SecurityEntityType")
                        .WithMany()
                        .HasForeignKey("SecurityEntityTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_RolePermission_SecurityEntityTypeId");

                    b.Navigation("LastEditedByUser");

                    b.Navigation("Permission");

                    b.Navigation("Role");

                    b.Navigation("SecurityEntityType");
                });

            modelBuilder.Entity("KhumaloCraft.Data.Entities.Entities.DalUserRole", b =>
                {
                    b.HasOne("KhumaloCraft.Data.Entities.Entities.DalUser", "LastEditedByUser")
                        .WithMany()
                        .HasForeignKey("LastEditedByUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_UserRole_LastEditedByUserId");

                    b.HasOne("KhumaloCraft.Data.Entities.Entities.DalRole", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .HasPrincipalKey("Id")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_UserRole_RoleId");

                    b.HasOne("KhumaloCraft.Data.Entities.Entities.DalSecurityEntityType", "SecurityEntityType")
                        .WithMany()
                        .HasForeignKey("SecurityEntityTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_UserRole_SecurityEntityTypeId");

                    b.HasOne("KhumaloCraft.Data.Entities.Entities.DalUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_UserRole_UserId");

                    b.Navigation("LastEditedByUser");

                    b.Navigation("Role");

                    b.Navigation("SecurityEntityType");

                    b.Navigation("User");
                });
#pragma warning restore 612, 618
        }
    }
}
