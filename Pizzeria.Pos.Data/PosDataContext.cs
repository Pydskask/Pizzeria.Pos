using Microsoft.EntityFrameworkCore;
using Pizzeria.Pos.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Pizzeria.Pos.Data;

public class PosDataContext : DbContext
{
    // Pusty ctor dla EF tools (migracje)
    public PosDataContext() { }

    // KONSTRUKTOR DLA DI (App.xaml.cs) - BRAKOWAŁ!
    public PosDataContext(DbContextOptions<PosDataContext> options)
        : base(options) { }
    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<PrintedDocument> PrintedDocuments { get; set; }
    public DbSet<PizzaAddonDefinition> PizzaAddonDefinitions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany()
            .HasForeignKey(o => o.UserId);

        modelBuilder.Entity<OrderItem>()
            .HasIndex(i => i.OrderId); // Dla szybkich query

        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Name = "Manager", Pin = "1989", Role = "Manager", IsActive = true },
            new User { Id = 2, Name = "Kelner1", Pin = "1648", Role = "Kelner", IsActive = true }
        );

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Margherita 30cm", Price = 35m, Category = "Pizza" },
            new Product { Id = 2, Name = "Cola 0.5l", Price = 8m, Category = "Napoje zimne" }
        );

        modelBuilder.Entity<PrintedDocument>()
            .Property(x => x.DocumentType)
            .HasMaxLength(100);

        modelBuilder.Entity<PrintedDocument>()
            .Property(x => x.PrinterName)
            .HasMaxLength(200);

        modelBuilder.Entity<PrintedDocument>()
            .Property(x => x.JobName)
            .HasMaxLength(200);

        modelBuilder.Entity<OrderItem>()
            .Property(i => i.ConfigurationJson)
            .HasColumnType("TEXT");

        modelBuilder.Entity<PizzaAddonDefinition>()
                .Property(x => x.Name)
                .HasMaxLength(120);

        modelBuilder.Entity<PizzaAddonDefinition>()
            .Property(x => x.GroupName)
            .HasMaxLength(80);

        modelBuilder.Entity<PizzaAddonDefinition>().HasData(
            new PizzaAddonDefinition { Id = 1, Name = "Extra ser", GroupName = "Serowe", Price = 6m, SortOrder = 1, IsActive = true },
            new PizzaAddonDefinition { Id = 2, Name = "Szynka", GroupName = "Mięsne", Price = 7m, SortOrder = 1, IsActive = true },
            new PizzaAddonDefinition { Id = 3, Name = "Dodatkowy sos", GroupName = "Sosy", Price = 4m, SortOrder = 1, IsActive = true },
            new PizzaAddonDefinition { Id = 4, Name = "Pieczarki", GroupName = "Warzywne", Price = 5m, SortOrder = 1, IsActive = true },
            new PizzaAddonDefinition { Id = 5, Name = "Papryka", GroupName = "Warzywne", Price = 4m, SortOrder = 2, IsActive = true }
        );

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=app.db");
        }
    }

}
