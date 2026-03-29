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
    public PosDataContext() { }

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
            .HasIndex(i => i.OrderId);

        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Name = "Manager", Pin = "1989", Role = "Manager", IsActive = true },
            new User { Id = 2, Name = "Kelner1", Pin = "1648", Role = "Kelner", IsActive = true },
            new User { Id = 3, Name = "Kelner2", Pin = "2211", Role = "Kelner", IsActive = true }
        );

        modelBuilder.Entity<Product>().HasData(
            new Product { Id =  1, Name = "Margherita", Price = 32m, Category = "Pizza" },
            new Product { Id =  2, Name = "Pepperoni", Price = 38m, Category = "Pizza" },
            new Product { Id =  3, Name = "Quattro Formaggi", Price = 42m, Category = "Pizza" },
            new Product { Id =  4, Name = "Prosciutto", Price = 40m, Category = "Pizza" },
            new Product { Id =  5, Name = "Diavola", Price = 39m, Category = "Pizza" },
            new Product { Id =  6, Name = "Hawajska", Price = 37m, Category = "Pizza" },
            new Product { Id =  7, Name = "Capricciosa",         Price = 40m, Category = "Pizza" },
            new Product { Id =  8, Name = "Vege",                Price = 35m, Category = "Pizza" },
            new Product { Id =  9, Name = "BBQ Kurczak",         Price = 41m, Category = "Pizza" },
            new Product { Id = 10, Name = "Fungi",               Price = 36m, Category = "Pizza" },
            new Product { Id = 11, Name = "Tonno",               Price = 39m, Category = "Pizza" },
            new Product { Id = 12, Name = "Romana",              Price = 36m, Category = "Pizza" },
            new Product { Id = 13, Name = "Calabrese",           Price = 41m, Category = "Pizza" },
            new Product { Id = 14, Name = "Truflowa",            Price = 48m, Category = "Pizza" },

            new Product { Id = 15, Name = "Coca-Cola 0,33l",     Price =  7m, Category = "Napoje zimne" },
            new Product { Id = 16, Name = "Coca-Cola 0,5l",      Price =  9m, Category = "Napoje zimne" },
            new Product { Id = 17, Name = "Sprite 0,33l",        Price =  7m, Category = "Napoje zimne" },
            new Product { Id = 18, Name = "Fanta 0,33l",         Price =  7m, Category = "Napoje zimne" },
            new Product { Id = 19, Name = "Woda niegazowana",    Price =  5m, Category = "Napoje zimne" },
            new Product { Id = 20, Name = "Woda gazowana",       Price =  5m, Category = "Napoje zimne" },
            new Product { Id = 21, Name = "Sok pomarańczowy",    Price =  8m, Category = "Napoje zimne" },
            new Product { Id = 22, Name = "Sok jabłkowy",        Price =  8m, Category = "Napoje zimne" },
            new Product { Id = 23, Name = "Lemońska domowa",     Price = 10m, Category = "Napoje zimne" },

            new Product { Id = 24, Name = "Kawa espresso",       Price =  8m, Category = "Napoje ciepłe" },
            new Product { Id = 25, Name = "Cappuccino",          Price = 10m, Category = "Napoje ciepłe" },
            new Product { Id = 26, Name = "Latte macchiato",     Price = 11m, Category = "Napoje ciepłe" },
            new Product { Id = 27, Name = "Herbata",             Price =  7m, Category = "Napoje ciepłe" }
        );

        modelBuilder.Entity<PrintedDocument>()
            .Property(x => x.DocumentType).HasMaxLength(100);
        modelBuilder.Entity<PrintedDocument>()
            .Property(x => x.PrinterName).HasMaxLength(200);
        modelBuilder.Entity<PrintedDocument>()
            .Property(x => x.JobName).HasMaxLength(200);
        modelBuilder.Entity<OrderItem>()
            .Property(i => i.ConfigurationJson).HasColumnType("TEXT");
        modelBuilder.Entity<PizzaAddonDefinition>()
            .Property(x => x.Name).HasMaxLength(120);
        modelBuilder.Entity<PizzaAddonDefinition>()
            .Property(x => x.GroupName).HasMaxLength(80);

        modelBuilder.Entity<PizzaAddonDefinition>().HasData(

            new PizzaAddonDefinition { Id =  1, Name = "Extra ser",               GroupName = "Ser",     Price = 6m, SortOrder = 1, IsActive = true },
            new PizzaAddonDefinition { Id =  2, Name = "Mozzarella",               GroupName = "Ser",     Price = 7m, SortOrder = 2, IsActive = true },
            new PizzaAddonDefinition { Id =  3, Name = "Gorgonzola",               GroupName = "Ser",     Price = 8m, SortOrder = 3, IsActive = true },
            new PizzaAddonDefinition { Id =  4, Name = "Parmezan",                 GroupName = "Ser",     Price = 7m, SortOrder = 4, IsActive = true },
            new PizzaAddonDefinition { Id =  5, Name = "Ricotta",                  GroupName = "Ser",     Price = 8m, SortOrder = 5, IsActive = true },

            new PizzaAddonDefinition { Id =  6, Name = "Szynka",                   GroupName = "Mięsne", Price = 7m, SortOrder = 1, IsActive = true },
            new PizzaAddonDefinition { Id =  7, Name = "Salami",                   GroupName = "Mięsne", Price = 7m, SortOrder = 2, IsActive = true },
            new PizzaAddonDefinition { Id =  8, Name = "Boczek",                   GroupName = "Mięsne", Price = 8m, SortOrder = 3, IsActive = true },
            new PizzaAddonDefinition { Id =  9, Name = "Kurczak z grilla",         GroupName = "Mięsne", Price = 9m, SortOrder = 4, IsActive = true },
            new PizzaAddonDefinition { Id = 10, Name = "Pepperoni",                GroupName = "Mięsne", Price = 8m, SortOrder = 5, IsActive = true },
            new PizzaAddonDefinition { Id = 11, Name = "Nduja",                    GroupName = "Mięsne", Price = 9m, SortOrder = 6, IsActive = true },
            new PizzaAddonDefinition { Id = 12, Name = "Salsiccia",                GroupName = "Mięsne", Price = 9m, SortOrder = 7, IsActive = true },

            new PizzaAddonDefinition { Id = 13, Name = "Pieczarki",               GroupName = "Warzywne", Price = 5m, SortOrder = 1, IsActive = true },
            new PizzaAddonDefinition { Id = 14, Name = "Papryka",                 GroupName = "Warzywne", Price = 4m, SortOrder = 2, IsActive = true },
            new PizzaAddonDefinition { Id = 15, Name = "Cebula",                  GroupName = "Warzywne", Price = 3m, SortOrder = 3, IsActive = true },
            new PizzaAddonDefinition { Id = 16, Name = "Oliwki czarne",           GroupName = "Warzywne", Price = 5m, SortOrder = 4, IsActive = true },
            new PizzaAddonDefinition { Id = 17, Name = "Oliwki zielone",          GroupName = "Warzywne", Price = 5m, SortOrder = 5, IsActive = true },
            new PizzaAddonDefinition { Id = 18, Name = "Pomidory suszone",        GroupName = "Warzywne", Price = 6m, SortOrder = 6, IsActive = true },
            new PizzaAddonDefinition { Id = 19, Name = "Rukola",                  GroupName = "Warzywne", Price = 4m, SortOrder = 7, IsActive = true },
            new PizzaAddonDefinition { Id = 20, Name = "Ananas",                  GroupName = "Warzywne", Price = 4m, SortOrder = 8, IsActive = true },
            new PizzaAddonDefinition { Id = 21, Name = "Szpinak",                 GroupName = "Warzywne", Price = 4m, SortOrder = 9, IsActive = true },
            new PizzaAddonDefinition { Id = 22, Name = "Karczoch",                GroupName = "Warzywne", Price = 6m, SortOrder = 10, IsActive = true },

            new PizzaAddonDefinition { Id = 23, Name = "Sos pomidorowy",          GroupName = "Sosy",    Price = 4m, SortOrder = 1, IsActive = true },
            new PizzaAddonDefinition { Id = 24, Name = "Sos czosnkowy",           GroupName = "Sosy",    Price = 4m, SortOrder = 2, IsActive = true },
            new PizzaAddonDefinition { Id = 25, Name = "Sos BBQ",                 GroupName = "Sosy",    Price = 4m, SortOrder = 3, IsActive = true },
            new PizzaAddonDefinition { Id = 26, Name = "Sos ostry",               GroupName = "Sosy",    Price = 4m, SortOrder = 4, IsActive = true },
            new PizzaAddonDefinition { Id = 27, Name = "Pesto",                   GroupName = "Sosy",    Price = 5m, SortOrder = 5, IsActive = true },
            new PizzaAddonDefinition { Id = 28, Name = "Sos śmietanowy",          GroupName = "Sosy",    Price = 4m, SortOrder = 6, IsActive = true }
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
