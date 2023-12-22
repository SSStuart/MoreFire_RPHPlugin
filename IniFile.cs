using Rage;

namespace SSStuartTools
{
    internal static class Settings
    {
        internal static double FIRE_DESIRED_BURN_DURATION = 100;
        internal static double FIRE_SPREAD_RADIUS = 2;
        internal static double FIRE_ELAPSED_TIME_INCREMENT_PLAYER = 0.5;
        internal static double FIRE_ELAPSED_TIME_INCREMENT_NPC = 1;

        internal static void LoadSettings()
        {
            Game.LogTrivial("[LOG]: Loading config file for Longer Fire.");
            var path = "Plugins/LongerFire.ini";
            var ini = new InitializationFile(path);
            ini.Create();
            FIRE_DESIRED_BURN_DURATION = ini.ReadDouble("Fire behavior", "FireDesiredBurnDuration", 0.5);
            FIRE_SPREAD_RADIUS = ini.ReadDouble("Fire behavior", "FireSpreadRadius", 0.5);
            FIRE_ELAPSED_TIME_INCREMENT_PLAYER = ini.ReadDouble("Fire extinguishing", "FireElapsedTimeIncrementPlayer", 0.5);
            FIRE_ELAPSED_TIME_INCREMENT_NPC = ini.ReadDouble("Fire extinguishing", "FireElapsedTimeIncrementNPC", 0.5);
        }
    }
}