using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using DataAccessLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WpfUI.Services;
using WpfUI.ViewModels;

namespace WpfUI;

public partial class App : Application
{
    private const string AppTitle = "Contact Book";

    private IHost? _appHost;

    public App()
    {
        try
        {
            string appDataDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ContactBookApp");
            Directory.CreateDirectory(appDataDir);
            string dbPath = Path.Combine(appDataDir, "Contacts.db");
            string connectionString = $"Data Source={dbPath};";

            _appHost = Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddDbContext<ContactDbContext>(options =>
                    {
                        options.UseSqlite(connectionString);
                    });
                    services.AddTransient(sp => new ContactDbContextFactory(connectionString));
                    services.AddTransient<IDialogService, WindowDialogService>();
                    services.AddTransient<MainViewModel>();
                    services.AddSingleton(sp => new MainWindow(sp.GetRequiredService<MainViewModel>()));
                })
                .Build();
        }
        catch (Exception ex)
        {
            ShowFatalError("The application failed to initialize.", ex);
            Current.Shutdown();
        }
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        if (_appHost is null)
        {
            return;
        }

        try
        {
            await _appHost.StartAsync();

            using IServiceScope scope = _appHost.Services.CreateScope();
            ContactDbContextFactory dbContextFactory = scope.ServiceProvider.GetRequiredService<ContactDbContextFactory>();
            using ContactDbContext db = dbContextFactory.CreateDbContext();
            await db.Database.MigrateAsync();

            EventManager.RegisterClassHandler(
                typeof(TextBox),
                TextBox.GotKeyboardFocusEvent,
                new RoutedEventHandler(SelectAllText));

            Window mainWindow = scope.ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            mainWindow.Show();
            base.OnStartup(e);
        }
        catch (Exception ex)
        {
            ShowFatalError("The application failed to start.", ex);
            Current.Shutdown();
        }
    }

    private void SelectAllText(object sender, RoutedEventArgs e)
    {
        TextBox? textBox = sender as TextBox;

        if (textBox != null)
        {
            if (textBox.IsReadOnly == false)
            {
                textBox.SelectAll();
            }
        }
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        try
        {
            if (_appHost is not null)
            {
                await _appHost.StopAsync();
                _appHost.Dispose();
            }
        }
        catch (Exception ex)
        {
            // WPF apps don't have a console, so Console.WriteLine goes nowhere.
            // Debug.WriteLine surfaces in the IDE Output window during a debugger session.
            Debug.WriteLine($"Error during application shutdown: {ex}");
        }
        finally
        {
            base.OnExit(e);
        }
    }

    private static void ShowFatalError(string headline, Exception ex)
    {
        MessageBox.Show(
            $"{headline}\n\n{ex.GetType().Name}: {ex.Message}\n\n{ex}",
            $"{AppTitle} - Startup Error",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }
}
