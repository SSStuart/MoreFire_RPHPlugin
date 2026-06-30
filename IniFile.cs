using Rage;
using System.Collections.Generic;
using System.Linq;

namespace MoreFire
{
    internal static class Settings
    {
        internal static double FIRE_DESIRED_BURN_DURATION = 100;
        internal static double FIRE_SPREAD_RADIUS = 2;
        internal static bool AUTO_MAX_FIRES = false;
        internal static int MAX_FIRES = 80;
        internal static double FIRE_ELAPSED_TIME_INCREMENT_PLAYER = 0.5;
        internal static double FIRE_ELAPSED_TIME_INCREMENT_NPC = 1;
        internal static bool PLAYER_FIRE_PROOF = false;
        internal static List<string> FIRETRUCK_MODELS = new List<string>();
        internal static double FIRETRUCK_EFFECT_RADIUS = 20;

        internal static void LoadSettings()
        {
            Game.LogTrivial("Loading plugin settings");
            var path = "Plugins/MoreFire.ini";
            var ini = new InitializationFile(path);
            ini.Create();

            FIRE_DESIRED_BURN_DURATION = ini.ReadDouble("Fire behavior", "FireDesiredBurnDuration", 100);
            Game.LogTrivial($"- Fire desired burn duration: {FIRE_DESIRED_BURN_DURATION}s");
            FIRE_SPREAD_RADIUS = ini.ReadDouble("Fire behavior", "FireSpreadRadius", 2);
            Game.LogTrivial($"- Fire spread radius: {FIRE_SPREAD_RADIUS}m");
            AUTO_MAX_FIRES = ini.ReadBoolean("Fire behavior", "AutoMaxFires", false);
            Game.LogTrivial($"- Auto max fires: {(AUTO_MAX_FIRES ? "Yes" : "No")}");
            MAX_FIRES = ini.ReadInt16("Fire behavior", "MaxFires", 80);
            Game.LogTrivial($"- Max fires: {MAX_FIRES}");

            FIRE_ELAPSED_TIME_INCREMENT_PLAYER = ini.ReadDouble("Fire extinguishing", "FireElapsedTimeIncrementPlayer", 0.5);
            Game.LogTrivial($"- Elapsed time increment (Player): {FIRE_ELAPSED_TIME_INCREMENT_PLAYER}s");
            FIRE_ELAPSED_TIME_INCREMENT_NPC = ini.ReadDouble("Fire extinguishing", "FireElapsedTimeIncrementNPC", 1);
            Game.LogTrivial($"- Elapsed time increment (NPC): {FIRE_ELAPSED_TIME_INCREMENT_NPC}s");
            FIRETRUCK_MODELS = ini.ReadString("Fire extinguishing", "FireTruckModels", "firetruk").ToUpper().Split(',').ToList();
            Game.LogTrivial($"- Fire truck models: {string.Join(", ", FIRETRUCK_MODELS)}");
            FIRETRUCK_EFFECT_RADIUS = ini.ReadDouble("Fire extinguishing", "FireTruckEffectRadius", 20);
            Game.LogTrivial($"- Fire truck effect radius: {FIRETRUCK_EFFECT_RADIUS}m");

            PLAYER_FIRE_PROOF = ini.ReadBoolean("Player", "PlayerFireProof", false);
            Game.LogTrivial($"- Is player fire proof: {(PLAYER_FIRE_PROOF ? "Yes" : "No")}");

            Game.LogTrivial($"Plugin settings loaded.");
        }
    }
}