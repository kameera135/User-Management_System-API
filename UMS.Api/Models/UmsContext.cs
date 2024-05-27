using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace UMS.Api.Models;

public partial class UmsContext : DbContext
{
    public UmsContext()
    {
    }

    public UmsContext(DbContextOptions<UmsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActivityLog> ActivityLogs { get; set; }

    public virtual DbSet<Apitoken> Apitokens { get; set; }

    public virtual DbSet<AuthToken> AuthTokens { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Platform> Platforms { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserPlatform> UserPlatforms { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActivityLog>(entity =>
        {
            entity.HasKey(e => e.LogId);

            entity.Property(e => e.LogId).HasColumnName("LogID");
            entity.Property(e => e.ActivityType).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Details).HasColumnType("text");
            entity.Property(e => e.PlatformId).HasColumnName("PlatformID");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Platform).WithMany(p => p.ActivityLogs)
                .HasForeignKey(d => d.PlatformId)
                .HasConstraintName("FK__ActivityL__Platf__32767D0B");

            entity.HasOne(d => d.Role).WithMany(p => p.ActivityLogs)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__ActivityL__RoleI__336AA144");

            entity.HasOne(d => d.User).WithMany(p => p.ActivityLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ActivityL__UserI__318258D2");
        });

        modelBuilder.Entity<Apitoken>(entity =>
        {
            entity.HasKey(e => e.TokenId);

            entity.ToTable("APITokens");

            entity.Property(e => e.TokenId).HasColumnName("TokenID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.ExpireDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<AuthToken>(entity =>
        {
            entity.HasKey(e => e.TokenId);

            entity.Property(e => e.TokenId).HasColumnName("TokenID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.ExpireDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.AuthTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AuthToken__UserI__308E3499");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.Property(e => e.PermissionId).HasColumnName("PermissionID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.Permission1)
                .HasMaxLength(100)
                .HasColumnName("Permission");
            entity.Property(e => e.PlatformId).HasColumnName("PlatformID");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.Is_licence).IsRequired().HasDefaultValueSql("((1))");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Platform).WithMany(p => p.Permissions)
                .HasForeignKey(d => d.PlatformId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Permissio__Platf__2F9A1060");
        });

        modelBuilder.Entity<Platform>(entity =>
        {
            entity.Property(e => e.PlatformId).HasColumnName("PlatformID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(150);
            entity.Property(e => e.PlatformName).HasMaxLength(100);
            entity.Property(e => e.PlatformCode).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.PlatformId).HasColumnName("PlatformID");
            entity.Property(e => e.Role1)
                .HasMaxLength(100)
                .HasColumnName("Role");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Platform).WithMany(p => p.Roles)
                .HasForeignKey(d => d.PlatformId)
                .HasConstraintName("FK__Roles__PlatformI__2EA5EC27");
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.PermissionId).HasColumnName("PermissionID");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Permission).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RolePermi__Permi__39237A9A");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RolePermi__RoleI__382F5661");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.EmployeeId)
                .HasMaxLength(100)
                .HasColumnName("EmployeeID");
            entity.Property(e => e.FirstLogin).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e=> e._2FA_authenticate).IsRequired().HasDefaultValueSql("((0))");
            ///entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UserName).HasMaxLength(100);
        });

        modelBuilder.Entity<UserPlatform>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.PlatformId).HasColumnName("PlatformID");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Platform).WithMany(p => p.UserPlatforms)
                .HasForeignKey(d => d.PlatformId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserPlatf__Platf__3552E9B6");

            entity.HasOne(d => d.User).WithMany(p => p.UserPlatforms)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserPlatf__UserI__345EC57D");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserRoles__RoleI__373B3228");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserRoles__UserI__36470DEF");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
