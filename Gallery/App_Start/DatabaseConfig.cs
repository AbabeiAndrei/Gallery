using Gallery.DataLayer.Startup;

namespace Gallery
{
    public static class DatabaseConfig
    {
        public static void Config(DatabaseUpdater updater)
        {
            updater.UpdateDatabase();
            updater.SeedData().Wait();
        }
    }
}