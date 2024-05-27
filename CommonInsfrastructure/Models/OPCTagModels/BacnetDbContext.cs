using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CommonInsfrastructure.Models.OPCTagModels;

public partial class BacnetDbContext : DbContext
{
    public BacnetDbContext()
    {
    }

    public BacnetDbContext(DbContextOptions<BacnetDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblSchedule> TblSchedules { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblSchedule>(entity =>
        {
            entity.HasKey(e => e.EventId);

            entity.ToTable("tbl_schedules");

            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.InsertTime)
                .HasColumnType("datetime")
                .HasColumnName("insert_time");
            entity.Property(e => e.OffActionDoneStatus).HasColumnName("off_action_done_status");
            entity.Property(e => e.OffActionDoneTime)
                .HasColumnType("datetime")
                .HasColumnName("off_action_done_time");
            entity.Property(e => e.OffActionStatus)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("off_action_status");
            entity.Property(e => e.OffActionTime)
                .HasColumnType("datetime")
                .HasColumnName("off_action_time");
            entity.Property(e => e.OffActionValue)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("off_action_value");
            entity.Property(e => e.OffCheckDoneTime)
                .HasColumnType("datetime")
                .HasColumnName("off_check_done_time");
            entity.Property(e => e.OffCheckStatus)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("off_check_status");
            entity.Property(e => e.OnActionDoneStatus).HasColumnName("on_action_done_status");
            entity.Property(e => e.OnActionDoneTime)
                .HasColumnType("datetime")
                .HasColumnName("on_action_done_time");
            entity.Property(e => e.OnActionStatus)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("on_action_status");
            entity.Property(e => e.OnActionTime)
                .HasColumnType("datetime")
                .HasColumnName("on_action_time");
            entity.Property(e => e.OnActionValue)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("on_action_value");
            entity.Property(e => e.OnCheckDoneTime)
                .HasColumnType("datetime")
                .HasColumnName("on_check_done_time");
            entity.Property(e => e.OnCheckStatus)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("on_check_status");
            entity.Property(e => e.OpcTag)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("opc_tag");
            entity.Property(e => e.OracleInsertTime)
                .HasColumnType("datetime")
                .HasColumnName("oracle_insert_time");
            entity.Property(e => e.ReferenceCode1)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("reference_code_1");
            entity.Property(e => e.ReferenceCode2)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("reference_code_2");
            entity.Property(e => e.ReferenceCode3)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("reference_code_3");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
