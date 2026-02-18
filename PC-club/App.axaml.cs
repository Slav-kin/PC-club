using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using PC_club.Models;
using PC_club.ViewModels;
using PC_club.Views;
using System;
using System.Linq;

namespace PC_club
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // 1. ВИКЛИК методу (виконується при запуску)
                SeedData();

                DisableAvaloniaDataAnnotationValidation();

                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

        // 2. ОПИС методу (окремий блок поза межами інших методів)
        private void SeedData()
        {
            using var db = new PcClubContext();

            // Перевіряємо, чи база вже має дані
            if (db.Categories.Any()) return;

            // Додаємо категорії
            var basic = new Category { CategoryName = "basic" };
            var pro = new Category { CategoryName = "pro" };
            var vip = new Category { CategoryName = "vip" };
            db.Categories.AddRange(basic, pro, vip);
            db.SaveChanges();

            // Додаємо тарифи
            var basicTariff = new Tariff
            {
                TariffName = "Стандарт",
                TariffPrice = 50.00m,
                TariffConfiguration = "60 хв"
            };
            db.Tariffs.Add(basicTariff);
            db.SaveChanges();

            // Додаємо місця
            db.Places.AddRange(
                new Place { PlaceNumber = 1, CategoryId = basic.CategoryId, Status = "active" },
                new Place { PlaceNumber = 2, CategoryId = basic.CategoryId, Status = "active" },
                new Place { PlaceNumber = 3, CategoryId = pro.CategoryId, Status = "active" },
                new Place { PlaceNumber = 4, CategoryId = pro.CategoryId, Status = "active" },
                new Place { PlaceNumber = 5, CategoryId = vip.CategoryId, Status = "active" }
            );

            // Додаємо клієнта
            var client = new Client
            {
                FirstName = "Олексій",
                LastName = "Петренко",
                Nickname = "AlexUA",
                Email = "alex@email.com",
                Phone = "+380501112233",
                Balance = 200.00m,
                Status = "active"
            };
            db.Clients.Add(client);
            db.SaveChanges();

            // Створюємо сесію
            db.Sessions.Add(new Session
            {
                ClientId = client.ClientId,
                PlaceId = 1,
                TariffId = basicTariff.TariffId,
                StartSession = DateTime.Now,
                Status = "active",
                GameAccount = "club",
                TotalPrice = 0.00m
            });

            db.SaveChanges();
        }

        private void DisableAvaloniaDataAnnotationValidation()
        {
            var dataValidationPluginsToRemove =
                BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

            foreach (var plugin in dataValidationPluginsToRemove)
            {
                BindingPlugins.DataValidators.Remove(plugin);
            }
        }
    }
}