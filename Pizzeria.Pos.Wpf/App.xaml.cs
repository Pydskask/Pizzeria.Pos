using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pizzeria.Pos.Services;
using Pizzeria.Pos.Wpf.ViewModels;
using Pizzeria.Pos.Wpf.Views;
using System;
using System.IO;
using System.Windows;
using Pizzeria.Pos.Data;

namespace Pizzeria.Pos.Wpf;

public partial class App : Application
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;
    public static string DbPath { get; private set; } = string.Empty;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();

        DbPath = Path.Combine(AppContext.BaseDirectory, "app.db");

        services.AddDbContext<PosDataContext>(options =>
            options.UseSqlite($"Data Source={DbPath}"));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IPrintService, PrintService>();
        services.AddScoped<IPizzaAddonRepository, PizzaAddonRepository>();
        services.AddSingleton<IBackupService, BackupService>();

        services.AddTransient<LoginViewModel>();
        services.AddTransient<OrderViewModel>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<UserManagementViewModel>();


        ServiceProvider = services.BuildServiceProvider();

        using (var scope = ServiceProvider.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<PosDataContext>();
            db.Database.Migrate();
        }


        var loginWindow = new LoginWindow();
        Current.MainWindow = loginWindow;
        loginWindow.Show();
    }

}