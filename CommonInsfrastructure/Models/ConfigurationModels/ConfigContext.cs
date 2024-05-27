using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CommonInsfrastructure.Models.ConfigurationModels;

public partial class ConfigContext : DbContext
{
    public ConfigContext()
    {
    }

    public ConfigContext(DbContextOptions<ConfigContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AescommonSetting> AescommonSettings { get; set; }

    public virtual DbSet<AgreementDefaultWorkingHour> AgreementDefaultWorkingHours { get; set; }

    public virtual DbSet<AgreementInfo> AgreementInfos { get; set; }

    public virtual DbSet<AgreementRate> AgreementRates { get; set; }

    public virtual DbSet<AgreementService> AgreementServices { get; set; }

    public virtual DbSet<AgreementTaxis> AgreementTaxes { get; set; }

    public virtual DbSet<AgreementUnit> AgreementUnits { get; set; }

    public virtual DbSet<CommonSetting> CommonSettings { get; set; }

    public virtual DbSet<ControlTag> ControlTags { get; set; }

    public virtual DbSet<LocationMapBuilding> LocationMapBuildings { get; set; }

    public virtual DbSet<LocationMapLevel> LocationMapLevels { get; set; }

    public virtual DbSet<LocationMapUnit> LocationMapUnits { get; set; }

    public virtual DbSet<LocationMapUnitControlTagMapping> LocationMapUnitControlTagMappings { get; set; }

    public virtual DbSet<LocationMapUnitMeterMapping> LocationMapUnitMeterMappings { get; set; }

    public virtual DbSet<LocationMapUnitTenantMapping> LocationMapUnitTenantMappings { get; set; }

    public virtual DbSet<LocationMapUnitUserMapping> LocationMapUnitUserMappings { get; set; }

    public virtual DbSet<LocationMapUnitWorkingHour> LocationMapUnitWorkingHours { get; set; }

    public virtual DbSet<MasterDataDefaultOfficeHour> MasterDataDefaultOfficeHours { get; set; }

    public virtual DbSet<MasterDataHoliday> MasterDataHolidays { get; set; }

    public virtual DbSet<MasterDataMeter> MasterDataMeters { get; set; }

    public virtual DbSet<MasterDataPaymentTerm> MasterDataPaymentTerms { get; set; }

    public virtual DbSet<MasterDataRateType> MasterDataRateTypes { get; set; }

    public virtual DbSet<MasterDataRateTypeValue> MasterDataRateTypeValues { get; set; }

    public virtual DbSet<MasterDataService> MasterDataServices { get; set; }

    public virtual DbSet<MasterDataTaxType> MasterDataTaxTypes { get; set; }

    public virtual DbSet<MasterDataTaxTypeValue> MasterDataTaxTypeValues { get; set; }

    public virtual DbSet<TenantInfo> TenantInfos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AescommonSetting>(entity =>
        {
            entity.HasKey(e => e.Setting);

            entity.ToTable("AESCommonSettings");

            entity.HasIndex(e => e.Setting, "UQ__AESCommo__81E7DFFD65DD00C2").IsUnique();

            entity.Property(e => e.Setting).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Data).HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<AgreementDefaultWorkingHour>(entity =>
        {
            entity.Property(e => e.AgreementId)
                .HasMaxLength(150)
                .HasColumnName("AgreementID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("CreatedAT");
            entity.Property(e => e.Day).HasMaxLength(20);
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Agreement).WithMany(p => p.AgreementDefaultWorkingHours)
                .HasForeignKey(d => d.AgreementId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Agreement__Agree__490FC9A0");
        });

        modelBuilder.Entity<AgreementInfo>(entity =>
        {
            entity.HasKey(e => e.AgreementId);

            entity.ToTable("AgreementInfo");

            entity.Property(e => e.AgreementId)
                .HasMaxLength(150)
                .HasColumnName("AgreementID");
            entity.Property(e => e.AgreementUploadPath).HasMaxLength(255);
            entity.Property(e => e.BillingAddress).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("CreatedAT");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.EnableTax).HasDefaultValueSql("((1))");
            entity.Property(e => e.EndDate).HasColumnType("date");
            entity.Property(e => e.ExtensionCharges).HasDefaultValueSql("((0))");
            entity.Property(e => e.HasAircon).HasDefaultValueSql("((0))");
            entity.Property(e => e.HasServices).HasDefaultValueSql("((0))");
            entity.Property(e => e.LatePayment).HasDefaultValueSql("((0))");
            entity.Property(e => e.PaymentTerm).HasMaxLength(150);
            entity.Property(e => e.SignedDate).HasColumnType("date");
            entity.Property(e => e.Socharges)
                .HasDefaultValueSql("((0))")
                .HasColumnName("SOCharges");
            entity.Property(e => e.StartDate).HasColumnType("date");
            entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            entity.Property(e => e.TenantId)
                .HasMaxLength(150)
                .HasColumnName("TenantID");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Tenant).WithMany(p => p.AgreementInfos)
                .HasForeignKey(d => d.TenantId)
                .HasConstraintName("FK__Agreement__Tenan__46335CF5");
        });

        modelBuilder.Entity<AgreementRate>(entity =>
        {
            entity.Property(e => e.AgreementId)
                .HasMaxLength(150)
                .HasColumnName("AgreementID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("CreatedAT");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.Rate).HasMaxLength(150);
            entity.Property(e => e.RateType).HasMaxLength(150);
            entity.Property(e => e.Service).HasMaxLength(150);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UseGlobalValue).HasDefaultValueSql("((0))");

            entity.HasOne(d => d.Agreement).WithMany(p => p.AgreementRates)
                .HasForeignKey(d => d.AgreementId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Agreement__Agree__4AF81212");
        });

        modelBuilder.Entity<AgreementService>(entity =>
        {
            entity.Property(e => e.AgreementId)
                .HasMaxLength(150)
                .HasColumnName("AgreementID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("CreatedAT");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.Service).HasMaxLength(150);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Agreement).WithMany(p => p.AgreementServices)
                .HasForeignKey(d => d.AgreementId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Agreement__Agree__4A03EDD9");
        });

        modelBuilder.Entity<AgreementTaxis>(entity =>
        {
            entity.Property(e => e.AgreementId)
                .HasMaxLength(150)
                .HasColumnName("AgreementID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("CreatedAT");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.Tax).HasMaxLength(150);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Agreement).WithMany(p => p.AgreementTaxes)
                .HasForeignKey(d => d.AgreementId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Agreement__Agree__4BEC364B");
        });

        modelBuilder.Entity<AgreementUnit>(entity =>
        {
            entity.Property(e => e.AgreementId)
                .HasMaxLength(150)
                .HasColumnName("AgreementID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("CreatedAT");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.UnitId)
                .HasMaxLength(100)
                .HasColumnName("UnitID");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Agreement).WithMany(p => p.AgreementUnits)
                .HasForeignKey(d => d.AgreementId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Agreement__Agree__4727812E");

            entity.HasOne(d => d.Unit).WithMany(p => p.AgreementUnits)
                .HasForeignKey(d => d.UnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Agreement__UnitI__481BA567");
        });

        modelBuilder.Entity<CommonSetting>(entity =>
        {
            entity.HasKey(e => e.Setting);

            entity.HasIndex(e => e.Setting, "UQ__CommonSe__81E7DFFDFEDC0E30").IsUnique();

            entity.Property(e => e.Setting).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Data).HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<ControlTag>(entity =>
        {
            entity.HasIndex(e => e.Tag, "UQ__ControlT__C45164133ACD787F").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("CreatedAT");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Tag).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<LocationMapBuilding>(entity =>
        {
            entity.HasKey(e => e.BuildingId);

            entity.ToTable("LocationMapBuilding");

            entity.HasIndex(e => e.BuildingId, "UQ__Location__5463CDE55B787CD3").IsUnique();

            entity.Property(e => e.BuildingId)
                .HasMaxLength(100)
                .HasColumnName("BuildingID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("CreatedAT");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<LocationMapLevel>(entity =>
        {
            entity.HasKey(e => e.LevelId);

            entity.ToTable("LocationMapLevel");

            entity.HasIndex(e => e.LevelId, "UQ__Location__09F03C07177F3128").IsUnique();

            entity.Property(e => e.LevelId)
                .HasMaxLength(100)
                .HasColumnName("LevelID");
            entity.Property(e => e.BuildingId)
                .HasMaxLength(100)
                .HasColumnName("BuildingID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("CreatedAT");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.Icon).HasMaxLength(20);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.ParentId)
                .HasMaxLength(100)
                .HasColumnName("ParentID");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Building).WithMany(p => p.LocationMapLevels)
                .HasForeignKey(d => d.BuildingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LocationM__Build__3AC1AA49");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK__LocationM__Paren__3BB5CE82");
        });

        modelBuilder.Entity<LocationMapUnit>(entity =>
        {
            entity.HasKey(e => e.UnitId);

            entity.ToTable("LocationMapUnit");

            entity.HasIndex(e => e.UnitId, "UQ__Location__44F5EC947F38589C").IsUnique();

            entity.Property(e => e.UnitId)
                .HasMaxLength(100)
                .HasColumnName("UnitID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("CreatedAT");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.ParentLevelId)
                .HasMaxLength(100)
                .HasColumnName("ParentLevelID");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.ParentLevel).WithMany(p => p.LocationMapUnits)
                .HasForeignKey(d => d.ParentLevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LocationM__Paren__3CA9F2BB");
        });

        modelBuilder.Entity<LocationMapUnitControlTagMapping>(entity =>
        {
            entity.ToTable("LocationMapUnitControlTagMapping");

            entity.Property(e => e.ControlTag)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("CreatedAT");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.UnitId)
                .HasMaxLength(100)
                .HasColumnName("UnitID");
            entity.Property(e => e.UnitName).HasMaxLength(200);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<LocationMapUnitMeterMapping>(entity =>
        {
            entity.ToTable("LocationMapUnitMeterMapping");

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("CreatedAT");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.MeterId)
                .HasMaxLength(150)
                .HasColumnName("MeterID");
            entity.Property(e => e.MeterName).HasMaxLength(255);
            entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            entity.Property(e => e.UnitId)
                .HasMaxLength(100)
                .HasColumnName("UnitID");
            entity.Property(e => e.UnitName).HasMaxLength(200);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Meter).WithMany(p => p.LocationMapUnitMeterMappings)
                .HasForeignKey(d => d.MeterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LocationM__Meter__453F38BC");
        });

        modelBuilder.Entity<LocationMapUnitTenantMapping>(entity =>
        {
            entity.ToTable("LocationMapUnitTenantMapping");

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("CreatedAT");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.TenantId)
                .HasMaxLength(150)
                .HasColumnName("TenantID");
            entity.Property(e => e.UnitId)
                .HasMaxLength(100)
                .HasColumnName("UnitID");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Tenant).WithMany(p => p.LocationMapUnitTenantMappings)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LocationM__Tenan__407A839F");

            entity.HasOne(d => d.Unit).WithMany(p => p.LocationMapUnitTenantMappings)
                .HasForeignKey(d => d.UnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LocationM__UnitI__3F865F66");
        });

        modelBuilder.Entity<LocationMapUnitUserMapping>(entity =>
        {
            entity.ToTable("LocationMapUnitUserMapping");

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("CreatedAT");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.UnitId)
                .HasMaxLength(100)
                .HasColumnName("UnitID");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Unit).WithMany(p => p.LocationMapUnitUserMappings)
                .HasForeignKey(d => d.UnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LocationM__UnitI__3E923B2D");
        });

        modelBuilder.Entity<LocationMapUnitWorkingHour>(entity =>
        {
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("CreatedAT");
            entity.Property(e => e.Day).HasMaxLength(20);
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.UnitId)
                .HasMaxLength(100)
                .HasColumnName("UnitID");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Unit).WithMany(p => p.LocationMapUnitWorkingHours)
                .HasForeignKey(d => d.UnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LocationM__UnitI__3D9E16F4");
        });

        modelBuilder.Entity<MasterDataDefaultOfficeHour>(entity =>
        {
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("CreatedAT");
            entity.Property(e => e.Day).HasMaxLength(20);
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<MasterDataHoliday>(entity =>
        {
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("CreatedAT");
            entity.Property(e => e.Date).HasColumnType("date");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<MasterDataMeter>(entity =>
        {
            entity.HasKey(e => e.MeterId);

            entity.Property(e => e.MeterId)
                .HasMaxLength(150)
                .HasColumnName("MeterID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("CreatedAT");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.MeterName).HasMaxLength(255);
            entity.Property(e => e.MeterTag).HasMaxLength(150);
            entity.Property(e => e.MeterType).HasMaxLength(150);
            entity.Property(e => e.MeterUnit).HasMaxLength(20);
            entity.Property(e => e.ServiceType).HasMaxLength(150);
            entity.Property(e => e.Status).HasDefaultValueSql("((0))");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.ServiceTypeNavigation).WithMany(p => p.MasterDataMeters)
                .HasForeignKey(d => d.ServiceType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MasterDat__Servi__416EA7D8");
        });

        modelBuilder.Entity<MasterDataPaymentTerm>(entity =>
        {
            entity.HasKey(e => e.PaymentTermId);

            entity.Property(e => e.PaymentTermId)
                .HasMaxLength(150)
                .HasColumnName("PaymentTermID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("CreatedAT");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.PaymentTerm).HasMaxLength(150);
            entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<MasterDataRateType>(entity =>
        {
            entity.HasKey(e => e.RateId);

            entity.Property(e => e.RateId)
                .HasMaxLength(150)
                .HasColumnName("RateID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("CreatedAT");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.RateCategory).HasMaxLength(150);
            entity.Property(e => e.RateType).HasMaxLength(20);
            entity.Property(e => e.ServiceType).HasMaxLength(150);
            entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.ServiceTypeNavigation).WithMany(p => p.MasterDataRateTypes)
                .HasForeignKey(d => d.ServiceType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MasterDat__Servi__4262CC11");
        });

        modelBuilder.Entity<MasterDataRateTypeValue>(entity =>
        {
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("CreatedAT");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.EffectiveDate).HasColumnType("date");
            entity.Property(e => e.Rate).HasMaxLength(150);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.RateNavigation).WithMany(p => p.MasterDataRateTypeValues)
                .HasForeignKey(d => d.Rate)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MasterData__Rate__4356F04A");
        });

        modelBuilder.Entity<MasterDataService>(entity =>
        {
            entity.HasKey(e => e.ServiceId);

            entity.Property(e => e.ServiceId)
                .HasMaxLength(150)
                .HasColumnName("ServiceID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("CreatedAT");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.ServiceType).HasMaxLength(150);
            entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            entity.Property(e => e.Unit).HasMaxLength(150);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<MasterDataTaxType>(entity =>
        {
            entity.HasKey(e => e.TaxId);

            entity.Property(e => e.TaxId)
                .HasMaxLength(150)
                .HasColumnName("TaxID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("CreatedAT");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            entity.Property(e => e.TaxCategory).HasMaxLength(150);
            entity.Property(e => e.TaxType).HasMaxLength(20);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<MasterDataTaxTypeValue>(entity =>
        {
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("CreatedAT");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.EffectiveDate).HasColumnType("date");
            entity.Property(e => e.Tax).HasMaxLength(150);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.TaxNavigation).WithMany(p => p.MasterDataTaxTypeValues)
                .HasForeignKey(d => d.Tax)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MasterDataT__Tax__444B1483");
        });

        modelBuilder.Entity<TenantInfo>(entity =>
        {
            entity.HasKey(e => e.TenantId);

            entity.ToTable("TenantInfo");

            entity.Property(e => e.TenantId)
                .HasMaxLength(150)
                .HasColumnName("TenantID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("CreatedAT");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.Status).HasDefaultValueSql("((1))");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
