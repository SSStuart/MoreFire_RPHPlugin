using Rage;

namespace MoreFire
{
    internal static class Settings
    {
        internal static double FIRE_DESIRED_BURN_DURATION = 100;
        internal static double FIRE_SPREAD_RADIUS = 2;
        internal static double MAX_FIRES = 80;
        internal static double FIRE_ELAPSED_TIME_INCREMENT_PLAYER = 0.5;
        internal static double FIRE_ELAPSED_TIME_INCREMENT_NPC = 1;

        internal static void LoadSettings()
        {
            Game.LogTrivial("Loading plugin settings" );
            var path = "Plugins/MoreFire.ini";
            var ini = new InitializationFile(path);
            ini.Create();
            FIRE_DESIRED_BURN_DURATION = ini.ReadDouble("Fire behavior", "FireDesiredBurnDuration", 100);
            Game.LogTrivial($"- Fire desired burn duration: {FIRE_DESIRED_BURN_DURATION}");
            FIRE_SPREAD_RADIUS = ini.ReadDouble("Fire behavior", "FireSpreadRadius", 2);
            Game.LogTrivial($"- Fire spread radius: {FIRE_SPREAD_RADIUS}");
            MAX_FIRES = ini.ReadInt16("Fire behavior", "MaxFires", 80);
            Game.LogTrivial($"- Max fires: {MAX_FIRES}");
            FIRE_ELAPSED_TIME_INCREMENT_PLAYER = ini.ReadDouble("Fire extinguishing", "FireElapsedTimeIncrementPlayer", 0.5);
            Game.LogTrivial($"- Elapsed time increment (Player): {FIRE_ELAPSED_TIME_INCREMENT_PLAYER}");
            FIRE_ELAPSED_TIME_INCREMENT_NPC = ini.ReadDouble("Fire extinguishing", "FireElapsedTimeIncrementNPC", 1);
            Game.LogTrivial($"- Elapsed time increment (NPC): {FIRE_ELAPSED_TIME_INCREMENT_NPC}");

            Game.LogTrivial($"Plugin settings loaded.");
        }
    }
}