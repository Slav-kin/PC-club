using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using PC_club.Models;
using PC_club.ViewModels;
using PC_club.Views;
using System;
using System.Collections.Generic;
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
                //SeedData();

                DisableAvaloniaDataAnnotationValidation();

                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

        // 2. ОПИС методу (окремий блок поза межами інших методів)
        //        private void SeedData()
        //        {
        //            using var db = new PcClubContext();

        //            // Перевіряємо, чи база вже має дані
        //            if (db.Categories.Any()) return;

        //            // Додаємо категорії
        //            var basic = new Category { CategoryName = "basic" };
        //            var console = new Category { CategoryName = "console" };
        //            var pro = new Category { CategoryName = "pro" };
        //            var vip = new Category { CategoryName = "vip" };
        //            db.Categories.AddRange(basic, pro, console, vip);
        //            db.SaveChanges();

        //            // Додаємо тарифи
        //            var basicTariff = new Tariff
        //            {
        //                TariffName = "Стандарт",
        //                TariffPrice = 80.00m,
        //                TariffConfiguration = "60 хв"
        //            };

        //            var consoleTariff = new Tariff
        //            {
        //                TariffName = "Консолі",
        //                TariffPrice = 100.00m,
        //                TariffConfiguration = "60 хв"
        //            };

        //            var proTariff = new Tariff
        //            {
        //                TariffName = "Pro",
        //                TariffPrice = 120.00m,
        //                TariffConfiguration = "60 хв"
        //            };

        //            var vipTariff = new Tariff
        //            {
        //                TariffName = "VIP",
        //                TariffPrice = 150.00m,
        //                TariffConfiguration = "60 хв"
        //            };

        //            db.Tariffs.AddRange(basicTariff, consoleTariff, proTariff, vipTariff);
        //            db.SaveChanges();

        //            // Додаємо місця
        //            db.Places.AddRange(
        //                new Place { PlaceNumber = 1, CategoryId = basic.CategoryId, Status = "active" },
        //                new Place { PlaceNumber = 2, CategoryId = basic.CategoryId, Status = "active" },
        //                new Place { PlaceNumber = 3, CategoryId = basic.CategoryId, Status = "inactive" },
        //                new Place { PlaceNumber = 4, CategoryId = basic.CategoryId, Status = "active" },
        //                new Place { PlaceNumber = 5, CategoryId = basic.CategoryId, Status = "active" },
        //                new Place { PlaceNumber = 6, CategoryId = console.CategoryId, Status = "active"},
        //                new Place { PlaceNumber = 7, CategoryId = console.CategoryId, Status = "active" },
        //                new Place { PlaceNumber = 8, CategoryId = console.CategoryId, Status = "active" },
        //                new Place { PlaceNumber = 9, CategoryId = console.CategoryId, Status = "active" },
        //                new Place { PlaceNumber = 10, CategoryId = console.CategoryId, Status = "active" },
        //                new Place { PlaceNumber = 11, CategoryId = pro.CategoryId, Status = "active" },
        //                new Place { PlaceNumber = 12, CategoryId = pro.CategoryId, Status = "inactive" },
        //                new Place { PlaceNumber = 13, CategoryId = pro.CategoryId, Status = "active" },
        //                new Place { PlaceNumber = 14, CategoryId = pro.CategoryId, Status = "active" },
        //                new Place { PlaceNumber = 15, CategoryId = pro.CategoryId, Status = "active" },
        //                new Place { PlaceNumber = 16, CategoryId = vip.CategoryId, Status = "active" },
        //                new Place { PlaceNumber = 17, CategoryId = vip.CategoryId, Status = "active" },
        //                new Place { PlaceNumber = 18, CategoryId = vip.CategoryId, Status = "active" },
        //                new Place { PlaceNumber = 19, CategoryId = vip.CategoryId, Status = "inactive" },
        //                new Place { PlaceNumber = 20, CategoryId = vip.CategoryId, Status = "active" }
        //            );

        //            // Додаємо клієнта
        //            var clients = new List<Client>
        //{
        //    new Client
        //    {
        //        FirstName = "Олексій",
        //        LastName = "Петренко",
        //        Nickname = "AlexUA",
        //        Email = "alex@email.com",
        //        Phone = "+380501112233",
        //        Balance = 200.00m,
        //        Status = "active"
        //    },
        //    new Client
        //    {
        //        FirstName = "Дмитро",
        //        LastName = "Коваленко",
        //        Nickname = "DarkKnight",
        //        Email = "dima@email.com",
        //        Phone = "+380671234567",
        //        Balance = 500.50m,
        //        Status = "active"
        //    },
        //    new Client
        //    {
        //        FirstName = "Марія",
        //        LastName = "Зайцева",
        //        Nickname = "CyberCat",
        //        Email = "masha@email.com",
        //        Phone = "+380939876543",
        //        Balance = 150.00m,
        //        Status = "active"
        //    },
        //    new Client
        //    {
        //        FirstName = "Ігор",
        //        LastName = "Мороз",
        //        Nickname = "Ghost",
        //        Email = "igor@email.com",
        //        Phone = "+380505554433",
        //        Balance = 0.00m,
        //        Status = "inactive"
        //    },
        //    new Client
        //    {
        //        FirstName = "Анна",
        //        LastName = "Світла",
        //        Nickname = "StarLight",
        //        Email = "anna@email.com",
        //        Phone = "+380661110099",
        //        Balance = 1000.00m,
        //        Status = "active"
        //    }
        //};

        //            db.Clients.AddRange(clients);
        //            db.SaveChanges();

        //            // Створюємо сесію
        //            db.Sessions.AddRange(new List<Session>
        //{
        //    // Ваші існуючі сесії...
        //    new Session { PlaceId = 1, Client = clients[0], StartSession = DateTime.Now, Status = "active", Tariff = basicTariff },
        //    new Session { PlaceId = 3, Client = clients[1], StartSession = DateTime.Now, Status = "active", Tariff = proTariff },

        //    // ДОДАЄМО НОВУ СЕСІЮ ТУТ:
        //    new Session
        //    {
        //        PlaceId = 2,           // Номер місця в базі
        //        Client = clients[2],    // Марія (третя у списку, індекс 2)
        //        StartSession = DateTime.Now,
        //        Status = "active",
        //        Tariff = proTariff      // Використовуємо тариф Pro
        //    },

        //    new Session 
        //    { 
        //        PlaceId = 7,
        //        Client = clients[4],
        //        StartSession = DateTime.Today.AddHours(17),
        //        Status = "booked",
        //        Tariff = consoleTariff
        //    }
        //});

        //            db.SaveChanges(); // Обов'язково зберігаємо зміни в SQL


        //            db.SaveChanges();
        //        }

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