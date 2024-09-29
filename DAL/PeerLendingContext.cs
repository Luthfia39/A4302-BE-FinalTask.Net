using System;
using System.Collections.Generic;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public partial class PeerLendingContext : DbContext
{
    public PeerLendingContext()
    {
    }

    public PeerLendingContext(DbContextOptions<PeerLendingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<MstUser> MstUsers { get; set; }
    public virtual DbSet<MstLoans> MstLoans { get; set; }
    public virtual DbSet<TrnFunding> TrnFundings { get; set; }
    public virtual DbSet<TrnRepayment> TrnRepayment { get; set; }
    public virtual DbSet<TrnMonthlyRepayment> TrnMonthlyRepayment { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MstUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("mst_user_pkey");

            entity.ToTable("mst_user");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Balance).HasColumnName("balance");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(150)
                .HasColumnName("password");
            entity.Property(e => e.Role)
                .HasMaxLength(30)
                .HasColumnName("role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
