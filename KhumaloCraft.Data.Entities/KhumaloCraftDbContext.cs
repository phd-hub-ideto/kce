using KhumaloCraft.Configuration;
using KhumaloCraft.Data.Entities.Entities;
using Microsoft.EntityFrameworkCore;

namespace KhumaloCraft.Data.Entities;

public class KhumaloCraftDbContext : DbContext
{
    public DbSet<DalCart> Cart { get; set; }
    public DbSet<DalCartItem> CartItem { get; set; }
    public DbSet<DalCraftwork> Craftwork { get; set; }
    public DbSet<DalCraftworkImage> CraftworkImage { get; set; }
    public DbSet<DalCraftworkCategory> CraftworkCategory { get; set; }
    public DbSet<DalCraftworkQuantity> CraftworkQuantity { get; set; }
    public DbSet<DalImage> Image { get; set; }
    public DbSet<DalImageReference> ImageReference { get; set; }
    public DbSet<DalImageType> ImageType { get; set; }
    public DbSet<DalOrder> Order { get; set; }
    public DbSet<DalOrderStatus> OrderStatus { get; set; }
    public DbSet<DalPermission> Permission { get; set; }
    public DbSet<DalPermissionSecurityEntityType> PermissionSecurityEntityType { get; set; }
    public DbSet<DalRole> Role { get; set; }
    public DbSet<DalRolePermission> RolePermission { get; set; }
    public DbSet<DalSecurityEntityType> SecurityEntityType { get; set; }
    public DbSet<DalSetting> Setting { get; set; }
    public DbSet<DalUser> User { get; set; }
    public DbSet<DalUserRole> UserRole { get; set; }

    public KhumaloCraftDbContext() { }

    public KhumaloCraftDbContext(int commandTimeout = 30)
    {
        _commandTimeout = commandTimeout;
    }

    private readonly int _commandTimeout;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        //WHEN YOU WANT TO IGNORE SOME ENTITIES, EITHER TEMP OR PERM
        //modelBuilder.Ignore<DalSetting>();

        //DalCart
        modelBuilder.Entity<DalCart>(entity =>
        {
            entity.HasKey(d => d.Id)
                  .HasName("PK_Cart_Id")
                  .IsClustered();

            entity.HasOne(d => d.User)
                  .WithMany()
                  .HasForeignKey(d => d.UserId)
                  .HasConstraintName("FK_Cart_UserId")
                  .OnDelete(DeleteBehavior.Restrict);
        });

        //DalCartItem
        modelBuilder.Entity<DalCartItem>(entity =>
        {
            entity.HasKey(d => d.Id)
                  .HasName("PK_CartItem_Id")
                  .IsClustered();

            entity.HasOne(d => d.Cart)
                  .WithMany()
                  .HasForeignKey(d => d.CartId)
                  .HasConstraintName("FK_CartItem_CartId")
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Craftwork)
                  .WithMany()
                  .HasForeignKey(d => d.CraftworkId)
                  .HasConstraintName("FK_CartItem_CraftworkId")
                  .OnDelete(DeleteBehavior.Restrict);
        });

        //DalCraftwork
        ConfigureTemporal<DalCraftwork>(modelBuilder, "Craftwork");

        modelBuilder.Entity<DalCraftwork>(entity =>
        {
            entity.HasKey(d => d.Id)
                  .HasName("PK_Craftwork_Id")
                  .IsClustered();

            entity.HasOne(d => d.CraftworkCategory)
                  .WithMany()
                  .HasForeignKey(d => d.CraftworkCategoryId)
                  .HasConstraintName("FK_Craftwork_CraftworkCategoryId")
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.LastEditedByUser)
                  .WithMany()
                  .HasForeignKey(d => d.LastEditedByUserId)
                  .HasConstraintName("FK_Craftwork_LastEditedByUserId")
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.LastEditedByUser)
                  .WithMany()
                  .HasForeignKey(d => d.LastEditedByUserId)
                  .HasConstraintName("FK_Craftwork_LastEditedByUserId")
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.ImageReference)
                  .WithMany()
                  .HasForeignKey(d => d.PrimaryImageReferenceId)
                  .HasConstraintName("FK_Craftwork_PrimaryImageReferenceId")
                  .OnDelete(DeleteBehavior.Restrict);
        });

        //DalCraftworkImage
        modelBuilder.Entity<DalCraftworkImage>(entity =>
        {
            entity.HasKey(d => d.Id)
                  .HasName("PK_CraftworkImage_Id")
                  .IsClustered();

            entity.HasOne(d => d.Craftwork)
                  .WithMany()
                  .HasForeignKey(d => d.CraftworkId)
                  .HasConstraintName("FK_CraftworkImage_CraftworkId")
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.ImageReference)
                  .WithMany()
                  .HasForeignKey(d => d.ImageReferenceId)
                  .HasConstraintName("FK_CraftworkImage_ImageReferenceId")
                  .OnDelete(DeleteBehavior.Restrict);
        });

        //DalCraftworkCategory
        modelBuilder.Entity<DalCraftworkCategory>(entity =>
        {
            entity.HasKey(d => d.Id)
                  .HasName("PK_CraftworkCategory_Id")
                  .IsClustered();
        });

        //DalCraftworkQuantity
        modelBuilder.Entity<DalCraftworkQuantity>(entity =>
        {
            entity.HasKey(d => d.Id)
                  .HasName("PK_CraftworkQuantity_Id")
                  .IsClustered();

            entity.HasIndex(u => u.CraftworkId, "UQ_CraftworkQuantity_CraftworkId")
                  .IsUnique();

            entity.HasOne(d => d.Craftwork)
                  .WithMany()
                  .HasForeignKey(d => d.CraftworkId)
                  .HasConstraintName("FK_CraftworkQuantity_CraftworkId")
                  .OnDelete(DeleteBehavior.Restrict);
        });

        //DalImage
        modelBuilder.Entity<DalImage>(entity =>
        {
            entity.HasKey(d => d.Id)
                  .HasName("PK_Image_Id")
                  .IsClustered();

            entity.HasOne(d => d.ImageType)
                  .WithMany()
                  .HasForeignKey(d => d.ImageTypeId)
                  .HasConstraintName("FK_Image_ImageTypeId")
                  .OnDelete(DeleteBehavior.Restrict);
        });

        //DalImageReference
        modelBuilder.Entity<DalImageReference>(entity =>
        {
            entity.HasKey(d => d.ReferenceId)
                  .HasName("PK_ImageReference_ReferenceId")
                  .IsClustered();

            entity.HasOne(d => d.Image)
                  .WithMany()
                  .HasForeignKey(d => d.ImageId)
                  .HasConstraintName("FK_ImageReference_ImageId")
                  .OnDelete(DeleteBehavior.Restrict);
        });

        //DalImageType
        modelBuilder.Entity<DalImageType>(entity =>
        {
            entity.HasKey(d => d.Id)
                  .HasName("PK_ImageType_Id")
                  .IsClustered();

            entity.HasIndex(u => u.Name, "UQ_ImageType_Name")
                  .IsUnique();
        });

        //DalOrder
        ConfigureTemporal<DalOrder>(modelBuilder, "Order");

        modelBuilder.Entity<DalOrder>(entity =>
        {
            entity.HasKey(d => d.Id)
                  .HasName("PK_Order_Id")
                  .IsClustered();

            entity.HasOne(d => d.Cart)
                  .WithMany()
                  .HasForeignKey(d => d.CartId)
                  .HasConstraintName("FK_Order_CartId")
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.OrderStatus)
                  .WithMany()
                  .HasForeignKey(d => d.OrderStatusId)
                  .HasConstraintName("FK_Order_OrderStatusId")
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.LastEditedByUser)
                  .WithMany()
                  .HasForeignKey(d => d.LastEditedByUserId)
                  .HasConstraintName("FK_Order_LastEditedByUserId")
                  .OnDelete(DeleteBehavior.Restrict);
        });

        //DalOrderStatus
        modelBuilder.Entity<DalOrderStatus>(entity =>
        {
            entity.HasKey(d => d.Id)
                  .HasName("PK_OrderStatus_Id")
                  .IsClustered();
        });

        //DalPermission
        modelBuilder.Entity<DalPermission>(entity =>
        {
            entity.HasKey(d => d.Id)
                  .HasName("PK_Permission_Id")
                  .IsClustered();
        });

        //DalPermissionSecurityEntityType
        modelBuilder.Entity<DalPermissionSecurityEntityType>(entity =>
        {
            entity.HasKey("PermissionId", "SecurityEntityTypeId")
                  .HasName("PK_PermissionSecurityEntityType_PermissionId_SecurityEntityTypeId")
                  .IsClustered();

            entity.HasOne(d => d.Permission)
                  .WithMany()
                  .HasForeignKey(d => d.PermissionId)
                  .HasConstraintName("FK_PermissionSecurityEntityType_PermissionId")
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.SecurityEntityType)
                  .WithMany()
                  .HasForeignKey(d => d.SecurityEntityTypeId)
                  .HasConstraintName("FK_PermissionSecurityEntityType_SecurityEntityTypeId")
                  .OnDelete(DeleteBehavior.Restrict);
        });

        //DalRole
        ConfigureTemporal<DalRole>(modelBuilder, "Role");

        modelBuilder.Entity<DalRole>(entity =>
        {
            entity.HasKey("Id", "SecurityEntityTypeId")
                  .HasName("PK_Role_Id_SecurityEntityTypeId")
                  .IsClustered();

            entity.HasOne(d => d.SecurityEntityType)
                  .WithMany()
                  .HasForeignKey(d => d.SecurityEntityTypeId)
                  .HasConstraintName("FK_Role_SecurityEntityTypeId")
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.LastEditedByUser)
                  .WithMany()
                  .HasForeignKey(d => d.LastEditedByUserId)
                  .HasConstraintName("FK_Role_LastEditedByUserId")
                  .OnDelete(DeleteBehavior.Restrict);
        });

        //DalRolePermission
        ConfigureTemporal<DalRolePermission>(modelBuilder, "RolePermission");

        modelBuilder.Entity<DalRolePermission>(entity =>
        {
            entity.HasKey("RoleId", "SecurityEntityTypeId", "PermissionId")
                  .HasName("PK_RolePermission_RoleId_SecurityEntityTypeId_PermissionId")
                  .IsClustered();

            entity.HasOne(d => d.Role)
                  .WithMany()
                  .HasForeignKey(d => d.RoleId)
                  .HasPrincipalKey("Id")
                  .HasConstraintName("FK_RolePermission_RoleId")
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.SecurityEntityType)
                  .WithMany()
                  .HasForeignKey(d => d.SecurityEntityTypeId)
                  .HasConstraintName("FK_RolePermission_SecurityEntityTypeId")
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Permission)
                  .WithMany()
                  .HasForeignKey(d => d.PermissionId)
                  .HasConstraintName("FK_RolePermission_PermissionId")
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.LastEditedByUser)
                  .WithMany()
                  .HasForeignKey(d => d.LastEditedByUserId)
                  .HasConstraintName("FK_RolePermission_LastEditedByUserId")
                  .OnDelete(DeleteBehavior.Restrict);
        });

        //DalSecurityEntityType
        modelBuilder.Entity<DalSecurityEntityType>(entity =>
        {
            entity.HasKey(d => d.Id)
                  .HasName("PK_SecurityEntityType_Id")
                  .IsClustered();
        });

        //DalSetting
        ConfigureTemporal<DalSetting>(modelBuilder, "Setting");

        modelBuilder.Entity<DalSetting>(entity =>
        {
            entity.HasOne(d => d.LastEditedByUser)
                .WithMany()
                .HasForeignKey(d => d.LastEditedByUserId)
                .HasConstraintName("FK_Setting_LastEditedByUserId")
                .OnDelete(DeleteBehavior.Restrict);
        });

        //DalUser
        modelBuilder.Entity<DalUser>()
            .HasIndex(u => u.Username, "UQ_User_Username")
            .IsUnique();

        //DalUserRole
        ConfigureTemporal<DalUserRole>(modelBuilder, "UserRole");

        modelBuilder.Entity<DalUserRole>(entity =>
        {
            entity.HasKey(d => d.Id)
                  .HasName("PK_UserRole_Id")
                  .IsClustered();

            entity.HasOne(d => d.Role)
                  .WithMany()
                  .HasForeignKey(d => d.RoleId)
                  .HasPrincipalKey("Id")
                  .HasConstraintName("FK_UserRole_RoleId")
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.SecurityEntityType)
                  .WithMany()
                  .HasForeignKey(d => d.SecurityEntityTypeId)
                  .HasConstraintName("FK_UserRole_SecurityEntityTypeId")
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.User)
                  .WithMany()
                  .HasForeignKey(d => d.UserId)
                  .HasConstraintName("FK_UserRole_UserId")
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.LastEditedByUser)
                  .WithMany()
                  .HasForeignKey(d => d.LastEditedByUserId)
                  .HasConstraintName("FK_UserRole_LastEditedByUserId")
                  .OnDelete(DeleteBehavior.Restrict);
        });

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = StaticConfiguration.ConnectionString(DatabaseConnectionName.KhumaloCraft);

            optionsBuilder.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.CommandTimeout(_commandTimeout);
            });

            base.OnConfiguring(optionsBuilder);
        }
    }

    private void ConfigureTemporal<TEntity>(ModelBuilder modelBuilder, string tableName) where TEntity : class
    {
        modelBuilder.Entity<TEntity>()
            .ToTable(tableName, b => b.IsTemporal(
                b =>
                {
                    b.HasPeriodStart("SysStartTime");
                    b.HasPeriodEnd("SysEndTime");
                }));
    }
}