using Microsoft.EntityFrameworkCore;
using Pizzeria.Pos.Core.Models;
using System;
using System.Linq;

namespace Pizzeria.Pos.Data;

/// <summary>
/// Wstawia przykładowe zamówienia przy pierwszym uruchomieniu (gdy baza jest pusta).
/// Wywołać raz po DbContext.Database.Migrate() w App.xaml.cs.
/// </summary>
public static class DbSeeder
{
    public static void SeedOrders(PosDataContext db)
    {
        if (db.Orders.Any())
            return; // już zasiane

        var now = DateTime.Now;

        var orders = new[]
        {
            // --- NA MIEJSCU ---
            new Order
            {
                Type = "M", UserId = 1, IsPaid = true,
                PaymentMethod = "Gotówka", Total = 74m,
                CustomerName = "Marek Kowalski",
                CreatedAt = now.AddDays(-20).AddHours(-3),
            },
            new Order
            {
                Type = "M", UserId = 2, IsPaid = true,
                PaymentMethod = "Karta", Total = 42m,
                CustomerName = "Anna Nowak",
                CreatedAt = now.AddDays(-18).AddHours(-1),
            },
            new Order
            {
                Type = "M", UserId = 2, IsPaid = true,
                PaymentMethod = "Karta", Total = 80m,
                CustomerName = "Tomasz Wiśniewsk", 
                CreatedAt = now.AddDays(-17).AddHours(-2),
            },
            new Order
            {
                Type = "M", UserId = 1, IsPaid = true,
                PaymentMethod = "Gotówka", Total = 37m,
                CustomerName = "Katarzyna Dąbrowska",
                CreatedAt = now.AddDays(-15).AddHours(-4),
            },
            new Order
            {
                Type = "M", UserId = 2, IsPaid = true,
                PaymentMethod = "Karta", Total = 121m,
                CustomerName = "Piotr Lewandowski",
                CreatedAt = now.AddDays(-13).AddHours(-1),
            },
            new Order
            {
                Type = "M", UserId = 1, IsPaid = true,
                PaymentMethod = "Gotówka", Total = 56m,
                CustomerName = "Magdalena Zielińska",
                CreatedAt = now.AddDays(-11).AddHours(-2),
            },
            new Order
            {
                Type = "M", UserId = 2, IsPaid = false,
                PaymentMethod = "", Total = 68m,
                CustomerName = "Robert Wójcik",
                CreatedAt = now.AddDays(-5).AddHours(-1),
            },
            new Order
            {
                Type = "M", UserId = 1, IsPaid = true,
                PaymentMethod = "Karta", Total = 90m,
                CustomerName = "Aleksandra Kaminska",
                CreatedAt = now.AddDays(-3).AddHours(-3),
            },

            // --- NA WYNOS ---
            new Order
            {
                Type = "W", UserId = 2, IsPaid = true,
                PaymentMethod = "Gotówka", Total = 38m,
                CustomerName = "Bartłomiej Szymanski",
                CustomerPhone = "500 111 222",
                CreatedAt = now.AddDays(-19).AddHours(-2),
            },
            new Order
            {
                Type = "W", UserId = 1, IsPaid = true,
                PaymentMethod = "Karta", Total = 76m,
                CustomerName = "Monika Król",
                CustomerPhone = "601 222 333",
                CreatedAt = now.AddDays(-16).AddHours(-3),
            },
            new Order
            {
                Type = "W", UserId = 2, IsPaid = true,
                PaymentMethod = "Gotówka", Total = 42m,
                CustomerName = "Damian Wieczorek",
                CustomerPhone = "512 333 444",
                CreatedAt = now.AddDays(-14).AddHours(-1),
            },
            new Order
            {
                Type = "W", UserId = 1, IsPaid = true,
                PaymentMethod = "Karta", Total = 80m,
                CustomerName = "Natalia Kaczmarek",
                CustomerPhone = "604 444 555",
                CreatedAt = now.AddDays(-10).AddHours(-2),
            },
            new Order
            {
                Type = "W", UserId = 2, IsPaid = true,
                PaymentMethod = "Gotówka", Total = 35m,
                CustomerName = "Lukasz Pawlak",
                CustomerPhone = "723 555 666",
                CreatedAt = now.AddDays(-8).AddHours(-4),
            },
            new Order
            {
                Type = "W", UserId = 1, IsPaid = false,
                PaymentMethod = "", Total = 48m,
                CustomerName = "Karolina Michalska",
                CustomerPhone = "880 666 777",
                CreatedAt = now.AddDays(-2).AddHours(-1),
            },

            // --- DOSTAWA ---
            new Order
            {
                Type = "D", UserId = 2, IsPaid = true,
                PaymentMethod = "Gotówka", Total = 55m,
                CustomerName = "Grzegorz Jankowski",
                CustomerPhone = "500 100 200",
                DeliveryCity = "Koszalin",
                DeliveryStreet = "ul. Zwyciestwa",
                DeliveryHouseNumber = "12",
                DeliveryApartmentNumber = "3",
                DeliveryPostalCode = "75-001",
                Notes = "Domofon 13, 3 piętro",
                CreatedAt = now.AddDays(-21).AddHours(-2),
            },
            new Order
            {
                Type = "D", UserId = 1, IsPaid = true,
                PaymentMethod = "Karta", Total = 88m,
                CustomerName = "Ewelina Mazur",
                CustomerPhone = "601 200 300",
                DeliveryCity = "Koszalin",
                DeliveryStreet = "ul. Monte Cassino",
                DeliveryHouseNumber = "45",
                DeliveryApartmentNumber = "",
                DeliveryPostalCode = "75-412",
                Notes = "",
                CreatedAt = now.AddDays(-19).AddHours(-3),
            },
            new Order
            {
                Type = "D", UserId = 2, IsPaid = true,
                PaymentMethod = "Gotówka", Total = 46m,
                CustomerName = "Krzysztof Piotrowsk",
                CustomerPhone = "512 300 400",
                DeliveryCity = "Koszalin",
                DeliveryStreet = "ul. Andersa",
                DeliveryHouseNumber = "7",
                DeliveryApartmentNumber = "12",
                DeliveryPostalCode = "75-950",
                Notes = "Proszę zadzwonić",
                CreatedAt = now.AddDays(-17).AddHours(-1),
            },
            new Order
            {
                Type = "D", UserId = 1, IsPaid = true,
                PaymentMethod = "Online", Total = 120m,
                CustomerName = "Joanna Wójcik",
                CustomerPhone = "604 400 500",
                DeliveryCity = "Koszalin",
                DeliveryStreet = "ul. Piłsudskiego",
                DeliveryHouseNumber = "23",
                DeliveryApartmentNumber = "8",
                DeliveryPostalCode = "75-510",
                Notes = "Bez dzwonienia, SMS",
                CreatedAt = now.AddDays(-15).AddHours(-4),
            },
            new Order
            {
                Type = "D", UserId = 2, IsPaid = true,
                PaymentMethod = "Gotówka", Total = 62m,
                CustomerName = "Szymon Grabowski",
                CustomerPhone = "723 500 600",
                DeliveryCity = "Koszalin",
                DeliveryStreet = "ul. Fabryczna",
                DeliveryHouseNumber = "3",
                DeliveryApartmentNumber = "1",
                DeliveryPostalCode = "75-209",
                Notes = "",
                CreatedAt = now.AddDays(-13).AddHours(-2),
            },
            new Order
            {
                Type = "D", UserId = 1, IsPaid = true,
                PaymentMethod = "Karta", Total = 78m,
                CustomerName = "Patrycja Nowicka",
                CustomerPhone = "880 600 700",
                DeliveryCity = "Koszalin",
                DeliveryStreet = "ul. Morska",
                DeliveryHouseNumber = "18",
                DeliveryApartmentNumber = "5",
                DeliveryPostalCode = "75-220",
                Notes = "Sos BBQ zamiast pomidorowego",
                CreatedAt = now.AddDays(-11).AddHours(-3),
            },
            new Order
            {
                Type = "D", UserId = 2, IsPaid = true,
                PaymentMethod = "Gotówka", Total = 41m,
                CustomerName = "Michal Adamczyk",
                CustomerPhone = "501 700 800",
                DeliveryCity = "Koszalin",
                DeliveryStreet = "ul. Rokosowo",
                DeliveryHouseNumber = "9",
                DeliveryApartmentNumber = "",
                DeliveryPostalCode = "75-819",
                Notes = "",
                CreatedAt = now.AddDays(-9).AddHours(-1),
            },
            new Order
            {
                Type = "D", UserId = 1, IsPaid = true,
                PaymentMethod = "Online", Total = 95m,
                CustomerName = "Agnieszka Kowalczyk",
                CustomerPhone = "601 800 900",
                DeliveryCity = "Koszalin",
                DeliveryStreet = "ul. Słowiańska",
                DeliveryHouseNumber = "34",
                DeliveryApartmentNumber = "2",
                DeliveryPostalCode = "75-846",
                Notes = "Zadzwonić 10 min przed",
                CreatedAt = now.AddDays(-7).AddHours(-2),
            },
            new Order
            {
                Type = "D", UserId = 2, IsPaid = false,
                PaymentMethod = "Gotówka", Total = 53m,
                CustomerName = "Rafał Dąbrowski",
                CustomerPhone = "512 900 100",
                DeliveryCity = "Koszalin",
                DeliveryStreet = "ul. Grottgera",
                DeliveryHouseNumber = "6",
                DeliveryApartmentNumber = "4",
                DeliveryPostalCode = "75-025",
                Notes = "Portier na dołu",
                CreatedAt = now.AddDays(-4).AddHours(-3),
            },
            new Order
            {
                Type = "D", UserId = 1, IsPaid = true,
                PaymentMethod = "Karta", Total = 110m,
                CustomerName = "Weronika Szymanska",
                CustomerPhone = "604 100 200",
                DeliveryCity = "Koszalin",
                DeliveryStreet = "ul. 4 Marca",
                DeliveryHouseNumber = "55",
                DeliveryApartmentNumber = "10",
                DeliveryPostalCode = "75-708",
                Notes = "",
                CreatedAt = now.AddDays(-2).AddHours(-4),
            },
        };

        db.Orders.AddRange(orders);
        db.SaveChanges();
    }
}
