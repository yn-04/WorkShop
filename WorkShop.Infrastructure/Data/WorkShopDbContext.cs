using Microsoft.EntityFrameworkCore;
using WorkShop.Core.Entities;

namespace WorkShop.Infrastructure.Data
{
    public partial class WorkShopDbContext : DbContext
    {
        public WorkShopDbContext(DbContextOptions<WorkShopDbContext> options) : base(options) { }

        public virtual DbSet<Menu> Menus { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<RoleMenuPermission> RoleMenuPermissions { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Menu>(entity =>
            {
                entity.ToTable("Menu");

                entity.HasIndex(e => e.MenuCode, "IX_Menu_1").IsUnique();

                entity.Property(e => e.ActionName).HasMaxLength(150);
                entity.Property(e => e.ControllerName).HasMaxLength(150);
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(sysutcdatetime())")
                    .HasAnnotation("Relational:DefaultConstraintName", "DF_Menu_CreatedDate")
                    .HasColumnType("datetime");
                entity.Property(e => e.IsActive)
                    .HasDefaultValue((byte)1)
                    .HasAnnotation("Relational:DefaultConstraintName", "DF_Menu_IsActive");
                entity.Property(e => e.MenuCode)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.MenuName)
                    .IsRequired()
                    .HasMaxLength(150);
                entity.Property(e => e.SortOrder).HasAnnotation("Relational:DefaultConstraintName", "DF_Menu_SortOrder");
                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.ParentMenu).WithMany(p => p.InverseParentMenu)
                    .HasForeignKey(d => d.ParentMenuId)
                    .HasConstraintName("FK_Menu_Menu");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE1A909BA1F3");

                entity.ToTable("Role");

                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(sysutcdatetime())")
                    .HasColumnType("datetime");
                entity.Property(e => e.IsActive)
                    .HasDefaultValue((byte)1)
                    .HasColumnName("isActive");
                entity.Property(e => e.RoleName)
                    .IsRequired()
                    .HasMaxLength(150);
                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<RoleMenuPermission>(entity =>
            {
                // Composite Key
                entity.HasKey(e => new { e.RoleId, e.MenuId });

                entity.ToTable("RoleMenuPermission");

                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getdate())")
                    .HasAnnotation("Relational:DefaultConstraintName", "DF_RoleMenuPermission_CreatedDate")
                    .HasColumnType("datetime");
                entity.Property(e => e.Permission).HasComment("bit flags: 1=Read, 2=Modify, 4=Delete");

                entity.HasOne(d => d.Menu).WithMany(p => p.RoleMenuPermissions)
                    .HasForeignKey(d => d.MenuId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RoleMenuPermission_Menu");

                entity.HasOne(d => d.Role).WithMany(p => p.RoleMenuPermissions)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RoleMenuPermission_Role");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4C08D1CFE8");

                entity.ToTable("User");

                entity.HasIndex(e => e.Email, "UQ__User__A9D10534A17985F7").IsUnique();

                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(sysutcdatetime())")
                    .HasAnnotation("Relational:DefaultConstraintName", "DF__User__CreatedDat__17F790F9")
                    .HasColumnType("datetime");
                entity.Property(e => e.DisplayName).HasMaxLength(200);
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.FirstName).HasMaxLength(100);
                entity.Property(e => e.IsActive)
                    .HasDefaultValue((byte)1)
                    .HasAnnotation("Relational:DefaultConstraintName", "DF__User__IsActive__17036CC0");
                entity.Property(e => e.LastName).HasMaxLength(100);
                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasDefaultValue("-")
                    .HasAnnotation("Relational:DefaultConstraintName", "DF_User_Password");
                entity.Property(e => e.Phone).HasMaxLength(50);

                // ✅ ใช้ UpdatedDate (มี d) ตามที่คุณเลือก
                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                // ✅ Composite Key (UserId + RoleId)
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.ToTable("UserRole");

                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(sysutcdatetime())")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK__UserRole__RoleId__4D94879B");

                entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__UserRole__UserId__367C1819");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}