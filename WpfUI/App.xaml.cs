using ContactBook.Services;
using ContactBook.ViewModels;
using DataAccessLibrary;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Windows;

namespace ContactBook;

public partial class App : Application
{
    private IHost _appHost;

    public App()
    {
        try
        {
            string env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
            bool isDevelopment = string.IsNullOrEmpty(env) || env.ToLower() == "development";

            _appHost = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(config =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", true, true);
                    config.AddJsonFile($"appsettings.{env}.json", true, true);
                    config.AddUserSecrets<App>(optional: true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddDbContext<ContactDbContext>(options =>
                    {
                        options.UseSqlite($@"Data Source={Environment.CurrentDirectory}\Contacts.db;");
                    });
                    services.AddScoped<IContactDataService, MockDataService>();
                    services.AddScoped<IDialogService, WindowDialogService>();
                    services.AddScoped<MainViewModel>();
                    services.AddScoped(sp => new MainWindow(sp.GetRequiredService<MainViewModel>()));
                })
                .Build();
        }
        catch (Exception ex)
        {
            string type = ex.GetType().Name;
            if (type.Equals("StopTheHostException", StringComparison.Ordinal))
            {
                throw;
            }

            Current.Shutdown();
        }
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await _appHost.StartAsync();
        using IServiceScope scope = _appHost.Services.CreateScope();
        //NotesDbContext db = scope.ServiceProvider.GetRequiredService<NotesDbContext>();
        //db.Database.Migrate();
        Window mainWindow = scope.ServiceProvider.GetRequiredService<MainWindow>();
        mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        mainWindow.Show();
        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        try
        {
            if (_appHost != null)
            {
                await _appHost.StopAsync();
                _appHost.Dispose();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }
        finally
        {
            base.OnExit(e);
        }
    }
}
