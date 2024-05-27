using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CommonInsfrastructure.Models.EmailModels;

public partial class EmailDbContext : DbContext
{
    public EmailDbContext()
    {
    }

    public EmailDbContext(DbContextOptions<EmailDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblEmailList> TblEmailLists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblEmailList>(entity =>
        {
            entity.HasKey(e => e.EmailId).HasName("PK_tbl_emai_3FEF87660662F0A3");

            entity.ToTable("tbl_email_list");

            entity.Property(e => e.EmailId).HasColumnName("email_id");
            entity.Property(e => e.AttachmentList)
                .HasColumnType("text")
                .HasColumnName("attachment_list");
            entity.Property(e => e.Body)
                .HasColumnType("text")
                .HasColumnName("body");
            entity.Property(e => e.BookingCode)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("booking_code");
            entity.Property(e => e.CcList)
                .HasColumnType("text")
                .HasColumnName("cc_list");
            entity.Property(e => e.GeneratedTime)
                .HasColumnType("datetime")
                .HasColumnName("generated_time");
            entity.Property(e => e.SendTime)
                .HasColumnType("datetime")
                .HasColumnName("send_time");
            entity.Property(e => e.Status)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("status");
            entity.Property(e => e.Subject)
                .HasColumnType("text")
                .HasColumnName("subject");
            entity.Property(e => e.ToList)
                .HasColumnType("text")
                .HasColumnName("to_list");
            entity.Property(e => e.Type)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("type");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
