using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace PC_club.Models;

public partial class PcClubContext : DbContext
{
    public PcClubContext()
    {
    }

    public PcClubContext(DbContextOptions<PcClubContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Place> Places { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    public virtual DbSet<Tariff> Tariffs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;database=pc_club;user=root;password=Parol", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.45-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookId).HasName("PRIMARY");

            entity.ToTable("bookings");

            entity.HasIndex(e => e.ClientId, "client_id");

            entity.HasIndex(e => e.PlaceId, "place_id");

            entity.Property(e => e.BookId).HasColumnName("book_id");
            entity.Property(e => e.BookLengthMinutes).HasColumnName("book_length_minutes");
            entity.Property(e => e.BookTime)
                .HasColumnType("datetime")
                .HasColumnName("book_time");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.PlaceId).HasColumnName("place_id");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'active'")
                .HasColumnName("status");

            entity.HasOne(d => d.Client).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("bookings_ibfk_1");

            entity.HasOne(d => d.Place).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.PlaceId)
                .HasConstraintName("bookings_ibfk_2");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PRIMARY");

            entity.ToTable("categories");

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(50)
                .HasColumnName("category_name");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.ClientId).HasName("PRIMARY");

            entity.ToTable("clients");

            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.Balance)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("'0.00'")
                .HasColumnName("balance");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
            entity.Property(e => e.Nickname)
                .HasMaxLength(50)
                .HasColumnName("nickname");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'active'")
                .HasColumnName("status");
        });

        modelBuilder.Entity<Place>(entity =>
        {
            entity.HasKey(e => e.PlaceId).HasName("PRIMARY");

            entity.ToTable("places");

            entity.HasIndex(e => e.CategoryId, "category_id");

            entity.Property(e => e.PlaceId).HasColumnName("place_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.PlaceNumber).HasColumnName("place_number");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'active'")
                .HasColumnName("status");

            entity.HasOne(d => d.Category).WithMany(p => p.Places)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("places_ibfk_1");
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.SessionId).HasName("PRIMARY");

            entity.ToTable("sessions");

            entity.HasIndex(e => e.ClientId, "client_id");

            entity.HasIndex(e => e.PlaceId, "place_id");

            entity.HasIndex(e => e.TariffId, "tariff_id");

            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.EndSession)
                .HasColumnType("datetime")
                .HasColumnName("end_session");
            entity.Property(e => e.GameAccount)
                .HasMaxLength(10)
                .HasColumnName("game_account");
            entity.Property(e => e.PlaceId).HasColumnName("place_id");
            entity.Property(e => e.StartSession)
                .HasColumnType("datetime")
                .HasColumnName("start_session");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'active'")
                .HasColumnName("status");
            entity.Property(e => e.TariffId).HasColumnName("tariff_id");
            entity.Property(e => e.TotalPrice)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("'0.00'")
                .HasColumnName("total_price");

            entity.HasOne(d => d.Client).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("sessions_ibfk_1");

            entity.HasOne(d => d.Place).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.PlaceId)
                .HasConstraintName("sessions_ibfk_2");

            entity.HasOne(d => d.Tariff).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.TariffId)
                .HasConstraintName("sessions_ibfk_3");
        });

        modelBuilder.Entity<Tariff>(entity =>
        {
            entity.HasKey(e => e.TariffId).HasName("PRIMARY");

            entity.ToTable("tariffs");

            entity.Property(e => e.TariffId).HasColumnName("tariff_id");
            entity.Property(e => e.TariffConfiguration)
                .HasColumnType("text")
                .HasColumnName("tariff_configuration");
            entity.Property(e => e.TariffName)
                .HasMaxLength(100)
                .HasColumnName("tariff_name");
            entity.Property(e => e.TariffPrice)
                .HasPrecision(10, 2)
                .HasColumnName("tariff_price");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
