using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
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
        public static IServiceProvider Services { get; private set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var servicecollection = new ServiceCollection();

            // реєструємо контекст бази даних (створюється новий екземпляр при кожному запиті)
            servicecollection.AddTransient<PcClubContext>();

            // реєструємо твої viewmodel
            servicecollection.AddTransient<HomeViewModel>();
            servicecollection.AddTransient<ClientsViewModel>();
            servicecollection.AddTransient<SessionViewModel>();

            // збираємо сервіси
            Services = servicecollection.BuildServiceProvider();

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