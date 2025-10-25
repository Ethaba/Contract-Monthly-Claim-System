using CMCS.Data;
using CMCS.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace CMCS
{
    public partial class App : Application
    {
        public static IServiceProvider? ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();

            // configure sqlite DB in local app folder
            var dbPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CMCS", "cmcs.db");
            var dbDir = System.IO.Path.GetDirectoryName(dbPath);
            if (!string.IsNullOrEmpty(dbDir) && !System.IO.Directory.Exists(dbDir))
                System.IO.Directory.CreateDirectory(dbDir);

            services.AddDbContext<AppDbContext>(options => options.UseSqlite($"Data Source={dbPath}"));

            // add services
            services.AddScoped<IClaimService, ClaimService>();
            services.AddScoped<IFileService, FileService>();

            ServiceProvider = services.BuildServiceProvider();

            // Ensure DB created and seed - do this here and show any exception so we know why it's failing
            try
            {
                using var scope = ServiceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // THIS will create the database file and schema if missing
                db.Database.EnsureCreated();

                // seed users if missing
                if (!db.Users.Any())
                {
                    db.Users.AddRange(
                        new Models.User { Username = "lect1", FirstName = "Lindi", LastName = "Ndlovu", Email = "lindi@example.com", Role = "Lecturer" },
                        new Models.User { Username = "coord1", FirstName = "Nomsa", LastName = "Dlamini", Email = "nomsa@example.com", Role = "Coordinator" },
                        new Models.User { Username = "mgr1", FirstName = "Thabo", LastName = "Mokoena", Email = "thabo@example.com", Role = "Manager" }
                    );
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                // Show the full error so we can fix it (this is safe for a prototype)
                MessageBox.Show("Database initialization failed:\n\n" + ex.ToString(), "DB Error", MessageBoxButton.OK, MessageBoxImage.Error);
                // allow app to continue (we still attempt to show main window so you can see UI)
            }

            // Show MainWindow
            try
            {
                var main = new Views.MainWindow();
                this.MainWindow = main;
                main.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open MainWindow:\n\n" + ex.ToString(), "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            base.OnStartup(e);
        }
    }
}
