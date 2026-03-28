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
            return;

        var now = DateTime.Now;
        // Kotwica na 28 marca 2026
        var mar = new DateTime(2026, 3, 1);

        var orders = new[]
        {
            // ===== STARE (przed marcem) =====
            new Order { Type="M", UserId=1, IsPaid=true,  PaymentMethod="Gotówka", Total=74m,  CustomerName="Marek Kowalski",         CreatedAt=now.AddDays(-60).AddHours(12) },
            new Order { Type="M", UserId=2, IsPaid=true,  PaymentMethod="Karta",   Total=42m,  CustomerName="Anna Nowak",             CreatedAt=now.AddDays(-60).AddHours(14) },
            new Order { Type="D", UserId=1, IsPaid=true,  PaymentMethod="Gotówka", Total=55m,  CustomerName="Grzegorz Jankowski",     CustomerPhone="500 100 200", DeliveryCity="Koszalin", DeliveryStreet="ul. Zwyciestwa",    DeliveryHouseNumber="12", DeliveryApartmentNumber="3",  DeliveryPostalCode="75-001", Notes="Domofon 13",              CreatedAt=now.AddDays(-60).AddHours(18) },
            new Order { Type="W", UserId=2, IsPaid=true,  PaymentMethod="Gotówka", Total=38m,  CustomerName="Bartłomiej Szymanski",   CustomerPhone="500 111 222", CreatedAt=now.AddDays(-57).AddHours(13) },
            new Order { Type="M", UserId=1, IsPaid=true,  PaymentMethod="Karta",   Total=80m,  CustomerName="Tomasz Wiśniewsk",       CreatedAt=now.AddDays(-57).AddHours(19) },
            new Order { Type="D", UserId=2, IsPaid=true,  PaymentMethod="Karta",   Total=88m,  CustomerName="Ewelina Mazur",          CustomerPhone="601 200 300", DeliveryCity="Koszalin", DeliveryStreet="ul. Monte Cassino",  DeliveryHouseNumber="45", DeliveryApartmentNumber="",   DeliveryPostalCode="75-412", Notes="",                        CreatedAt=now.AddDays(-57).AddHours(20) },
            new Order { Type="M", UserId=1, IsPaid=true,  PaymentMethod="Gotówka", Total=37m,  CustomerName="Katarzyna Dąbrowska",    CreatedAt=now.AddDays(-54).AddHours(12) },
            new Order { Type="W", UserId=2, IsPaid=true,  PaymentMethod="Karta",   Total=76m,  CustomerName="Monika Król",            CustomerPhone="601 222 333", CreatedAt=now.AddDays(-54).AddHours(17) },
            new Order { Type="D", UserId=1, IsPaid=true,  PaymentMethod="Gotówka", Total=46m,  CustomerName="Krzysztof Piotrowski",   CustomerPhone="512 300 400", DeliveryCity="Koszalin", DeliveryStreet="ul. Andersa",        DeliveryHouseNumber="7",  DeliveryApartmentNumber="12", DeliveryPostalCode="75-950", Notes="Proszę zadzwonić",       CreatedAt=now.AddDays(-54).AddHours(19) },
            new Order { Type="M", UserId=2, IsPaid=true,  PaymentMethod="Karta",   Total=121m, CustomerName="Piotr Lewandowski",      CreatedAt=now.AddDays(-51).AddHours(13) },
            new Order { Type="M", UserId=1, IsPaid=true,  PaymentMethod="Gotówka", Total=56m,  CustomerName="Magdalena Zielińska",   CreatedAt=now.AddDays(-51).AddHours(15) },
            new Order { Type="D", UserId=2, IsPaid=true,  PaymentMethod="Online",  Total=120m, CustomerName="Joanna Wójcik",          CustomerPhone="604 400 500", DeliveryCity="Koszalin", DeliveryStreet="ul. Piłsudskiego",    DeliveryHouseNumber="23", DeliveryApartmentNumber="8",  DeliveryPostalCode="75-510", Notes="Bez dzwonienia",          CreatedAt=now.AddDays(-51).AddHours(20) },
            new Order { Type="W", UserId=1, IsPaid=true,  PaymentMethod="Gotówka", Total=42m,  CustomerName="Damian Wieczorek",       CustomerPhone="512 333 444", CreatedAt=now.AddDays(-48).AddHours(12) },
            new Order { Type="M", UserId=2, IsPaid=true,  PaymentMethod="Karta",   Total=68m,  CustomerName="Robert Wójcik",          CreatedAt=now.AddDays(-48).AddHours(18) },
            new Order { Type="D", UserId=1, IsPaid=true,  PaymentMethod="Gotówka", Total=62m,  CustomerName="Szymon Grabowski",       CustomerPhone="723 500 600", DeliveryCity="Koszalin", DeliveryStreet="ul. Fabryczna",      DeliveryHouseNumber="3",  DeliveryApartmentNumber="1",  DeliveryPostalCode="75-209", Notes="",                        CreatedAt=now.AddDays(-48).AddHours(20) },
            new Order { Type="M", UserId=1, IsPaid=true,  PaymentMethod="Gotówka", Total=90m,  CustomerName="Aleksandra Kaminska",    CreatedAt=now.AddDays(-45).AddHours(13) },
            new Order { Type="W", UserId=2, IsPaid=true,  PaymentMethod="Karta",   Total=80m,  CustomerName="Natalia Kaczmarek",      CustomerPhone="604 444 555", CreatedAt=now.AddDays(-45).AddHours(16) },
            new Order { Type="D", UserId=1, IsPaid=true,  PaymentMethod="Karta",   Total=78m,  CustomerName="Patrycja Nowicka",       CustomerPhone="880 600 700", DeliveryCity="Koszalin", DeliveryStreet="ul. Morska",         DeliveryHouseNumber="18", DeliveryApartmentNumber="5",  DeliveryPostalCode="75-220", Notes="Sos BBQ",                 CreatedAt=now.AddDays(-45).AddHours(19) },
            new Order { Type="M", UserId=2, IsPaid=true,  PaymentMethod="Karta",   Total=48m,  CustomerName="Karolina Michalska",     CreatedAt=now.AddDays(-42).AddHours(12) },
            new Order { Type="M", UserId=1, IsPaid=true,  PaymentMethod="Gotówka", Total=35m,  CustomerName="Lukasz Pawlak",          CreatedAt=now.AddDays(-42).AddHours(14) },
            new Order { Type="D", UserId=2, IsPaid=true,  PaymentMethod="Gotówka", Total=41m,  CustomerName="Michal Adamczyk",        CustomerPhone="501 700 800", DeliveryCity="Koszalin", DeliveryStreet="ul. Rokosowo",       DeliveryHouseNumber="9",  DeliveryApartmentNumber="",   DeliveryPostalCode="75-819", Notes="",                        CreatedAt=now.AddDays(-42).AddHours(19) },
            new Order { Type="M", UserId=1, IsPaid=true,  PaymentMethod="Gotówka", Total=66m,  CustomerName="Marcin Kowalczyk",       CreatedAt=now.AddDays(-39).AddHours(13) },
            new Order { Type="W", UserId=2, IsPaid=true,  PaymentMethod="Gotówka", Total=39m,  CustomerName="Dorota Wąsik",           CustomerPhone="512 400 500", CreatedAt=now.AddDays(-39).AddHours(17) },
            new Order { Type="D", UserId=1, IsPaid=true,  PaymentMethod="Online",  Total=95m,  CustomerName="Agnieszka Kowalczyk",    CustomerPhone="601 800 900", DeliveryCity="Koszalin", DeliveryStreet="ul. Słowiańska",    DeliveryHouseNumber="34", DeliveryApartmentNumber="2",  DeliveryPostalCode="75-846", Notes="Zadzwonić 10 min przed",  CreatedAt=now.AddDays(-39).AddHours(20) },
            new Order { Type="M", UserId=2, IsPaid=true,  PaymentMethod="Karta",   Total=84m,  CustomerName="Sebastian Wróbel",        CreatedAt=now.AddDays(-36).AddHours(12) },
            new Order { Type="M", UserId=1, IsPaid=true,  PaymentMethod="Gotówka", Total=42m,  CustomerName="Paulina Jaworska",       CreatedAt=now.AddDays(-36).AddHours(19) },
            new Order { Type="D", UserId=2, IsPaid=true,  PaymentMethod="Karta",   Total=110m, CustomerName="Weronika Szymanska",     CustomerPhone="604 100 200", DeliveryCity="Koszalin", DeliveryStreet="ul. 4 Marca",        DeliveryHouseNumber="55", DeliveryApartmentNumber="10", DeliveryPostalCode="75-708", Notes="",                        CreatedAt=now.AddDays(-36).AddHours(20) },
            new Order { Type="W", UserId=1, IsPaid=true,  PaymentMethod="Gotówka", Total=44m,  CustomerName="Michał Brzezinski",      CustomerPhone="723 600 700", CreatedAt=now.AddDays(-33).AddHours(13) },
            new Order { Type="M", UserId=2, IsPaid=true,  PaymentMethod="Karta",   Total=57m,  CustomerName="Izabela Olejnik",        CreatedAt=now.AddDays(-33).AddHours(16) },
            new Order { Type="D", UserId=1, IsPaid=true,  PaymentMethod="Gotówka", Total=53m,  CustomerName="Rafał Dąbrowski",         CustomerPhone="512 900 100", DeliveryCity="Koszalin", DeliveryStreet="ul. Grottgera",      DeliveryHouseNumber="6",  DeliveryApartmentNumber="4",  DeliveryPostalCode="75-025", Notes="Portier na dołu",         CreatedAt=now.AddDays(-33).AddHours(19) },

            // ===== MARZEC 2026 (1-28.03) =====

            // 1 marca
            new Order { Type="M", UserId=1, IsPaid=true,  PaymentMethod="Gotówka", Total=58m,  CustomerName="Kamil Nowakowski",       CreatedAt=mar.AddDays(0).AddHours(12) },
            new Order { Type="W", UserId=2, IsPaid=true,  PaymentMethod="Karta",   Total=36m,  CustomerName="Sylwia Majewska",        CustomerPhone="880 700 800", CreatedAt=mar.AddDays(0).AddHours(17) },
            new Order { Type="D", UserId=1, IsPaid=true,  PaymentMethod="Online",  Total=85m,  CustomerName="Tomasz Ostrowski",       CustomerPhone="500 200 300", DeliveryCity="Koszalin", DeliveryStreet="ul. Wyspiańskiego",   DeliveryHouseNumber="11", DeliveryApartmentNumber="",   DeliveryPostalCode="75-603", Notes="",                        CreatedAt=mar.AddDays(0).AddHours(20) },

            // 2 marca
            new Order { Type="M", UserId=2, IsPaid=true,  PaymentMethod="Karta",   Total=63m,  CustomerName="Klaudia Witek",          CreatedAt=mar.AddDays(1).AddHours(13) },
            new Order { Type="D", UserId=1, IsPaid=true,  PaymentMethod="Gotówka", Total=69m,  CustomerName="Natalia Sobieraj",       CustomerPhone="601 300 400", DeliveryCity="Koszalin", DeliveryStreet="ul. Kościuszki",      DeliveryHouseNumber="8",  DeliveryApartmentNumber="3",  DeliveryPostalCode="75-401", Notes="",                        CreatedAt=mar.AddDays(1).AddHours(19) },

            // 3 marca
            new Order { Type="W", UserId=2, IsPaid=true,  PaymentMethod="Karta",   Total=54m,  CustomerName="Artur Zawadzki",         CustomerPhone="512 500 600", CreatedAt=mar.AddDays(2).AddHours(14) },
            new Order { Type="M", UserId=1, IsPaid=true,  PaymentMethod="Gotówka", Total=82m,  CustomerName="Ewa Kubiak",             CreatedAt=mar.AddDays(2).AddHours(18) },
            new Order { Type="D", UserId=2, IsPaid=true,  PaymentMethod="Karta",   Total=73m,  CustomerName="Dariusz Wierzbicki",     CustomerPhone="723 700 800", DeliveryCity="Koszalin", DeliveryStreet="ul. Krzywoustego",   DeliveryHouseNumber="15", DeliveryApartmentNumber="7",  DeliveryPostalCode="75-332", Notes="Dzwonić na bramofon",      CreatedAt=mar.AddDays(2).AddHours(20) },

            // 4 marca
            new Order { Type="M", UserId=2, IsPaid=true,  PaymentMethod="Karta",   Total=96m,  CustomerName="Justyna Krawczyk",       CreatedAt=mar.AddDays(3).AddHours(12) },
            new Order { Type="W", UserId=1, IsPaid=true,  PaymentMethod="Gotówka", Total=43m,  CustomerName="Piotr Adamski",          CustomerPhone="880 800 900", CreatedAt=mar.AddDays(3).AddHours(16) },

            // 5 marca
            new Order { Type="D", UserId=1, IsPaid=true,  PaymentMethod="Online",  Total=107m, CustomerName="Monika Laskowska",       CustomerPhone="500 300 400", DeliveryCity="Koszalin", DeliveryStreet="ul. Niepodległości", DeliveryHouseNumber="30", DeliveryApartmentNumber="",   DeliveryPostalCode="75-252", Notes="",                        CreatedAt=mar.AddDays(4).AddHours(19) },
            new Order { Type="M", UserId=2, IsPaid=true,  PaymentMethod="Gotówka", Total=47m,  CustomerName="Paweł Rutkowski",         CreatedAt=mar.AddDays(4).AddHours(13) },

            // 6 marca
            new Order { Type="M", UserId=1, IsPaid=true,  PaymentMethod="Karta",   Total=58m,  CustomerName="Marek Lewicki",          CreatedAt=mar.AddDays(5).AddHours(12) },
            new Order { Type="W", UserId=2, IsPaid=true,  PaymentMethod="Gotówka", Total=49m,  CustomerName="Katarzyna Kwiatkowska",  CustomerPhone="512 600 700", CreatedAt=mar.AddDays(5).AddHours(17) },
            new Order { Type="D", UserId=1, IsPaid=true,  PaymentMethod="Gotówka", Total=60m,  CustomerName="Wojciech Zając",          CustomerPhone="601 400 500", DeliveryCity="Koszalin", DeliveryStreet="ul. Sienkiewicza",   DeliveryHouseNumber="22", DeliveryApartmentNumber="9",  DeliveryPostalCode="75-515", Notes="",                        CreatedAt=mar.AddDays(5).AddHours(20) },

            // 7 marca
            new Order { Type="M", UserId=2, IsPaid=true,  PaymentMethod="Karta",   Total=77m,  CustomerName="Zuzanna Piętak",          CreatedAt=mar.AddDays(6).AddHours(14) },
            new Order { Type="D", UserId=1, IsPaid=true,  PaymentMethod="Karta",   Total=83m,  CustomerName="Magdalena Sikora",       CustomerPhone="723 800 900", DeliveryCity="Koszalin", DeliveryStreet="ul. Chopina",         DeliveryHouseNumber="5",  DeliveryApartmentNumber="2",  DeliveryPostalCode="75-729", Notes="Zostawić pod drzwiami",   CreatedAt=mar.AddDays(6).AddHours(19) },

            // 8 marca
            new Order { Type="M", UserId=1, IsPaid=true,  PaymentMethod="Gotówka", Total=45m,  CustomerName="Bartosz Konieczny",      CreatedAt=mar.AddDays(7).AddHours(12) },
            new Order { Type="W", UserId=2, IsPaid=true,  PaymentMethod="Karta",   Total=67m,  CustomerName="Angelika Marek",         CustomerPhone="880 900 100", CreatedAt=mar.AddDays(7).AddHours(16) },
            new Order { Type="D", UserId=1, IsPaid=true,  PaymentMethod="Online",  Total=92m,  CustomerName="Ryszard Baran",          CustomerPhone="500 400 500", DeliveryCity="Koszalin", DeliveryStreet="ul. Reymonta",        DeliveryHouseNumber="40", DeliveryApartmentNumber="",   DeliveryPostalCode="75-316", Notes="",                        CreatedAt=mar.AddDays(7).AddHours(20) },

            // 9 marca
            new Order { Type="M", UserId=2, IsPaid=true,  PaymentMethod="Karta",   Total=53m,  CustomerName="Sandra Wysocka",         CreatedAt=mar.AddDays(8).AddHours(13) },
            new Order { Type="M", UserId=1, IsPaid=true,  PaymentMethod="Gotówka", Total=88m,  CustomerName="Michał Czarnecki",        CreatedAt=mar.AddDays(8).AddHours(15) },

            // 10 marca
            new Order { Type="D", UserId=2, IsPaid=true,  PaymentMethod="Gotówka", Total=64m,  CustomerName="Aleksandra Wieczorek",   CustomerPhone="601 500 600", DeliveryCity="Koszalin", DeliveryStreet="ul. Fitelberga",      DeliveryHouseNumber="2",  DeliveryApartmentNumber="6",  DeliveryPostalCode="75-133", Notes="Koło apteki",              CreatedAt=mar.AddDays(9).AddHours(19) },
            new Order { Type="M", UserId=1, IsPaid=true,  PaymentMethod="Karta",   Total=115m, CustomerName="Krzysztof Zalewski",     CreatedAt=mar.AddDays(9).AddHours(18) },
            new Order { Type="W", UserId=2, IsPaid=true,  PaymentMethod="Gotówka", Total=40m,  CustomerName="Jacek Kaczor",           CustomerPhone="512 700 800", CreatedAt=mar.AddDays(9).AddHours(12) },

            // 11 marca
            new Order { Type="M", UserId=1, IsPaid=true,  PaymentMethod="Gotówka", Total=72m,  CustomerName="Natalia Marciniak",      CreatedAt=mar.AddDays(10).AddHours(13) },
            new Order { Type="D", UserId=2, IsPaid=true,  PaymentMethod="Karta",   Total=98m,  CustomerName="Tomasz Jasiński",         CustomerPhone="723 900 100", DeliveryCity="Koszalin", DeliveryStreet="ul. Słowackiego",      DeliveryHouseNumber="17", DeliveryApartmentNumber="",   DeliveryPostalCode="75-457", Notes="",                        CreatedAt=mar.AddDays(10).AddHours(20) },

            // 12 marca
            new Order { Type="M", UserId=2, IsPaid=true,  PaymentMethod="Gotówka", Total=61m,  CustomerName="Ola Nowacka",            CreatedAt=mar.AddDays(11).AddHours(12) },
            new Order { Type="W", UserId=1, IsPaid=true,  PaymentMethod="Karta",   Total=46m,  CustomerName="Kamil Szulc",            CustomerPhone="880 100 200", CreatedAt=mar.AddDays(11).AddHours(16) },
            new Order { Type="D", UserId=2, IsPaid=true,  PaymentMethod="Online",  Total=79m,  CustomerName="Beata Walczak",          CustomerPhone="500 500 600", DeliveryCity="Koszalin", DeliveryStreet="ul. Krakówska",        DeliveryHouseNumber="28", DeliveryApartmentNumber="3",  DeliveryPostalCode="75-641", Notes="",                        CreatedAt=mar.AddDays(11).AddHours(20) },

            // 13 marca
            new Order { Type="M", UserId=1, IsPaid=true,  PaymentMethod="Karta",   Total=103m, CustomerName="Piotr Czerwinski",       CreatedAt=mar.AddDays(12).AddHours(12) },
            new Order { Type="M", UserId=2, IsPaid=true,  PaymentMethod="Gotówka", Total=39m,  CustomerName="Zofia Grabowska",        CreatedAt=mar.AddDays(12).AddHours(14) },

            // 14 marca
            new Order { Type="W", UserId=1, IsPaid=true,  PaymentMethod="Karta",   Total=55m,  CustomerName="Hubert Kozłowski",        CustomerPhone="601 600 700", CreatedAt=mar.AddDays(13).AddHours(17) },
            new Order { Type="D", UserId=2, IsPaid=true,  PaymentMethod="Online",  Total=118m, CustomerName="Karolina Stępień",       CustomerPhone="512 800 900", DeliveryCity="Koszalin", DeliveryStreet="ul. Woodstock",       DeliveryHouseNumber="1",  DeliveryApartmentNumber="",   DeliveryPostalCode="75-001", Notes="Szybko!",                 CreatedAt=mar.AddDays(13).AddHours(20) },

            // 15 marca
            new Order { Type="M", UserId=1, IsPaid=true,  PaymentMethod="Gotówka", Total=66m,  CustomerName="Damian Krupa",           CreatedAt=mar.AddDays(14).AddHours(13) },
            new Order { Type="M", UserId=2, IsPaid=true,  PaymentMethod="Karta",   Total=91m,  CustomerName="Renata Kowalska",        CreatedAt=mar.AddDays(14).AddHours(18) },
            new Order { Type="D", UserId=1, IsPaid=true,  PaymentMethod="Gotówka", Total=74m,  CustomerName="Piotr Mazurek",          CustomerPhone="501 200 300", DeliveryCity="Koszalin", DeliveryStreet="ul. Zwycięstwa",      DeliveryHouseNumber="3",  DeliveryApartmentNumber="",   DeliveryPostalCode="75-001", Notes="",                        CreatedAt=mar.AddDays(14).AddHours(20) },

            // 16 marca
            new Order { Type="W", UserId=2, IsPaid=true,  PaymentMethod="Gotówka", Total=48m,  CustomerName="Tomasz Kubicki",         CustomerPhone="604 300 400", CreatedAt=mar.AddDays(15).AddHours(14) },
            new Order { Type="M", UserId=1, IsPaid=true,  PaymentMethod="Karta",   Total=57m,  CustomerName="Aleksandra Wrońska",      CreatedAt=mar.AddDays(15).AddHours(19) },

            // 17 marca
            new Order { Type="M", UserId=2, IsPaid=true,  PaymentMethod="Gotówka", Total=43m,  CustomerName="Łukasz Wojtyla",           CreatedAt=mar.AddDays(16).AddHours(12) },
            new Order { Type="D", UserId=1, IsPaid=true,  PaymentMethod="Online",  Total=89m,  CustomerName="Iwona Lis",              CustomerPhone="512 100 200", DeliveryCity="Koszalin", DeliveryStreet="ul. Pańska",           DeliveryHouseNumber="19", DeliveryApartmentNumber="5",  DeliveryPostalCode="75-104", Notes="",                        CreatedAt=mar.AddDays(16).AddHours(20) },
            new Order { Type="W", UserId=2, IsPaid=true,  PaymentMethod="Karta",   Total=62m,  CustomerName="Grzegorz Kubiak",        CustomerPhone="723 200 300", CreatedAt=mar.AddDays(16).AddHours(17) },

            // 18 marca
            new Order { Type="M", UserId=1, IsPaid=true,  PaymentMethod="Gotówka", Total=78m,  CustomerName="Marta Kowalczyk",        CreatedAt=mar.AddDays(17).AddHours(13) },
            new Order { Type="M", UserId=2, IsPaid=true,  PaymentMethod="Karta",   Total=52m,  CustomerName="Bartłomiej Kaczor",       CreatedAt=mar.AddDays(17).AddHours(15) },
            new Order { Type="D", UserId=1, IsPaid=true,  PaymentMethod="Gotówka", Total=97m,  CustomerName="Dominika Wróblewska",     CustomerPhone="880 200 300", DeliveryCity="Koszalin", DeliveryStreet="ul. Dąbrowskiego",    DeliveryHouseNumber="10", DeliveryApartmentNumber="2",  DeliveryPostalCode="75-553", Notes="Koć na górę",              CreatedAt=mar.AddDays(17).AddHours(20) },

            // 19 marca
            new Order { Type="W", UserId=2, IsPaid=true,  PaymentMethod="Gotówka", Total=35m,  CustomerName="Kamila Wojciechowska",   CustomerPhone="500 600 700", CreatedAt=mar.AddDays(18).AddHours(12) },
            new Order { Type="M", UserId=1, IsPaid=true,  PaymentMethod="Karta",   Total=109m, CustomerName="Marek Szymanski",        CreatedAt=mar.AddDays(18).AddHours(19) },

            // 20 marca
            new Order { Type="M", UserId=2, IsPaid=true,  PaymentMethod="Gotówka", Total=67m,  CustomerName="Anna Wisniewska",        CreatedAt=mar.AddDays(19).AddHours(13) },
            new Order { Type="D", UserId=1, IsPaid=true,  PaymentMethod="Online",  Total=86m,  CustomerName="Robert Janicki",         CustomerPhone="601 700 800", DeliveryCity="Koszalin", DeliveryStreet="ul. Krasickiego",     DeliveryHouseNumber="33", DeliveryApartmentNumber="",   DeliveryPostalCode="75-361", Notes="",                        CreatedAt=mar.AddDays(19).AddHours(20) },
            new Order { Type="W", UserId=2, IsPaid=true,  PaymentMethod="Karta",   Total=44m,  CustomerName="Sandra Kwiatkowska",     CustomerPhone="512 800 900", CreatedAt=mar.AddDays(19).AddHours(16) },

            // 21 marca
            new Order { Type="M", UserId=1, IsPaid=true,  PaymentMethod="Karta",   Total=93m,  CustomerName="Maciej Dudek",           CreatedAt=mar.AddDays(20).AddHours(12) },
            new Order { Type="M", UserId=2, IsPaid=true,  PaymentMethod="Gotówka", Total=38m,  CustomerName="Joanna Markowska",       CreatedAt=mar.AddDays(20).AddHours(15) },
            new Order { Type="D", UserId=1, IsPaid=true,  PaymentMethod="Gotówka", Total=71m,  CustomerName="Tomasz Borkowski",       CustomerPhone="723 300 400", DeliveryCity="Koszalin", DeliveryStreet="ul. Wiejska",         DeliveryHouseNumber="7",  DeliveryApartmentNumber="",   DeliveryPostalCode="75-900", Notes="Proszę puknąć",           CreatedAt=mar.AddDays(20).AddHours(20) },

            // 22 marca
            new Order { Type="W", UserId=2, IsPaid=true,  PaymentMethod="Gotówka", Total=51m,  CustomerName="Agata Chmielewska",      CustomerPhone="880 300 400", CreatedAt=mar.AddDays(21).AddHours(14) },
            new Order { Type="M", UserId=1, IsPaid=true,  PaymentMethod="Karta",   Total=84m,  CustomerName="Krzysztof Mazur",        CreatedAt=mar.AddDays(21).AddHours(19) },

            // 23 marca
            new Order { Type="M", UserId=2, IsPaid=true,  PaymentMethod="Gotówka", Total=59m,  CustomerName="Ewelina Nowicka",        CreatedAt=mar.AddDays(22).AddHours(12) },
            new Order { Type="D", UserId=1, IsPaid=true,  PaymentMethod="Karta",   Total=102m, CustomerName="Karol Wiśniewsk",         CustomerPhone="500 700 800", DeliveryCity="Koszalin", DeliveryStreet="ul. Mickiewicza",     DeliveryHouseNumber="14", DeliveryApartmentNumber="6",  DeliveryPostalCode="75-004", Notes="",                        CreatedAt=mar.AddDays(22).AddHours(20) },
            new Order { Type="W", UserId=2, IsPaid=true,  PaymentMethod="Karta",   Total=41m,  CustomerName="Natalia Pawlak",         CustomerPhone="604 500 600", CreatedAt=mar.AddDays(22).AddHours(16) },

            // 24 marca
            new Order { Type="M", UserId=1, IsPaid=true,  PaymentMethod="Gotówka", Total=76m,  CustomerName="Sławomir Grabski",        CreatedAt=mar.AddDays(23).AddHours(13) },
            new Order { Type="M", UserId=2, IsPaid=true,  PaymentMethod="Karta",   Total=49m,  CustomerName="Patrycja Kowalska",      CreatedAt=mar.AddDays(23).AddHours(15) },
            new Order { Type="D", UserId=1, IsPaid=true,  PaymentMethod="Online",  Total=88m,  CustomerName="Michał Zajączkowski",     CustomerPhone="512 900 100", DeliveryCity="Koszalin", DeliveryStreet="ul. Armii Krajowej",  DeliveryHouseNumber="21", DeliveryApartmentNumber="",   DeliveryPostalCode="75-900", Notes="",                        CreatedAt=mar.AddDays(23).AddHours(20) },

            // 25 marca
            new Order { Type="W", UserId=2, IsPaid=true,  PaymentMethod="Gotówka", Total=37m,  CustomerName="Monika Duda",            CustomerPhone="723 400 500", CreatedAt=mar.AddDays(24).AddHours(12) },
            new Order { Type="M", UserId=1, IsPaid=true,  PaymentMethod="Karta",   Total=95m,  CustomerName="Andrzej Nowicki",        CreatedAt=mar.AddDays(24).AddHours(18) },
            new Order { Type="D", UserId=2, IsPaid=true,  PaymentMethod="Gotówka", Total=68m,  CustomerName="Zuzanna Grabowska",      CustomerPhone="880 400 500", DeliveryCity="Koszalin", DeliveryStreet="ul. Generała Maczka",  DeliveryHouseNumber="4",  DeliveryApartmentNumber="1",  DeliveryPostalCode="75-900", Notes="",                        CreatedAt=mar.AddDays(24).AddHours(20) },

            // 26 marca
            new Order { Type="M", UserId=1, IsPaid=true,  PaymentMethod="Gotówka", Total=44m,  CustomerName="Konrad Lewandowski",     CreatedAt=mar.AddDays(25).AddHours(13) },
            new Order { Type="M", UserId=2, IsPaid=true,  PaymentMethod="Karta",   Total=112m, CustomerName="Izabela Nowak",          CreatedAt=mar.AddDays(25).AddHours(19) },
            new Order { Type="D", UserId=1, IsPaid=true,  PaymentMethod="Karta",   Total=77m,  CustomerName="Piotr Stachowiak",       CustomerPhone="500 800 900", DeliveryCity="Koszalin", DeliveryStreet="ul. Pomorska",        DeliveryHouseNumber="9",  DeliveryApartmentNumber="",   DeliveryPostalCode="75-344", Notes="Bramka po lewej",         CreatedAt=mar.AddDays(25).AddHours(20) },

            // 27 marca
            new Order { Type="W", UserId=2, IsPaid=true,  PaymentMethod="Gotówka", Total=53m,  CustomerName="Dorota Kaminska",        CustomerPhone="601 900 100", CreatedAt=mar.AddDays(26).AddHours(12) },
            new Order { Type="M", UserId=1, IsPaid=true,  PaymentMethod="Karta",   Total=81m,  CustomerName="Marcin Wojcik",          CreatedAt=mar.AddDays(26).AddHours(15) },
            new Order { Type="D", UserId=2, IsPaid=true,  PaymentMethod="Online",  Total=94m,  CustomerName="Aleksandra Nowak",       CustomerPhone="512 100 200", DeliveryCity="Koszalin", DeliveryStreet="ul. Szczecińska",      DeliveryHouseNumber="36", DeliveryApartmentNumber="8",  DeliveryPostalCode="75-135", Notes="",                        CreatedAt=mar.AddDays(26).AddHours(20) },

            // 28 marca (dziś)
            new Order { Type="M", UserId=1, IsPaid=true,  PaymentMethod="Gotówka", Total=103m, CustomerName="Piotr Czerwinski",       CreatedAt=mar.AddDays(27).AddHours(10) },
            new Order { Type="M", UserId=2, IsPaid=true,  PaymentMethod="Karta",   Total=39m,  CustomerName="Zofia Grabowska",        CreatedAt=mar.AddDays(27).AddHours(12) },
            new Order { Type="W", UserId=1, IsPaid=true,  PaymentMethod="Karta",   Total=55m,  CustomerName="Hubert Kozłowski",        CustomerPhone="601 600 700", CreatedAt=mar.AddDays(27).AddHours(13) },
            new Order { Type="D", UserId=2, IsPaid=false, PaymentMethod="Gotówka", Total=118m, CustomerName="Karolina Stępień",       CustomerPhone="512 800 900", DeliveryCity="Koszalin", DeliveryStreet="ul. Woodstock",       DeliveryHouseNumber="1",  DeliveryApartmentNumber="",   DeliveryPostalCode="75-001", Notes="Szybko!",                 CreatedAt=mar.AddDays(27).AddHours(14) },
            new Order { Type="M", UserId=1, IsPaid=false, PaymentMethod="",        Total=66m,  CustomerName="Damian Krupa",           CreatedAt=mar.AddDays(27).AddHours(15) },
        };

        db.Orders.AddRange(orders);
        db.SaveChanges();
    }
}
