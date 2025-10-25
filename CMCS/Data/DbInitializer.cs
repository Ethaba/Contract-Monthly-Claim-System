using CMCS.Models;

namespace CMCS.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            try
            {
                // Create DB and schema if not present
                context.Database.EnsureCreated();

                // Seed default users if none exist
                if (!context.Users.Any())
                {
                    context.Users.AddRange(
                        new User { Username = "lect1", FirstName = "Lindi", LastName = "Ndlovu", Email = "lindi@example.com", Role = "Lecturer" },
                        new User { Username = "coord1", FirstName = "Nomsa", LastName = "Dlamini", Email = "nomsa@example.com", Role = "Coordinator" },
                        new User { Username = "mgr1", FirstName = "Thabo", LastName = "Mokoena", Email = "thabo@example.com", Role = "Manager" }
                    );
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                // If something goes wrong during DB creation, write to console and allow app to continue.
                // In production you'd log this with ILogger.
                System.Diagnostics.Debug.WriteLine("DbInitializer.Initialize error: " + ex.Message);
                // Do not rethrow — we want the app to start so we can see UI errors
            }
        }
    }
}
