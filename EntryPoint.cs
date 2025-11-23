using Rage;
using Rage.Native;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

[assembly: Rage.Attributes.Plugin("MoreFire", Description = "A plugin that makes fires last longer.", Author = "SSStuart", PrefersSingleInstance = true, SupportUrl = "https://ssstuart.net/discord")]


namespace MoreFire
{
    public class EntryPoint
    {
        public static string pluginName = "MoreFire";
        public static string pluginVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public static void Main()
        {
            Game.LogTrivial($"{pluginName} plugin v{pluginVersion} loaded.");

            UpdateChecker.CheckForUpdates();

            Settings.LoadSettings();

            GameFiber.StartNew(delegate
            {
                List<Entity> closeFirefighters = new List<Entity>();
                uint lastTick = 0;

                while (true)
                {
                    GameFiber.Wait(0);
                    Game.LocalPlayer.Character.IsFireProof = Settings.PLAYER_FIRE_PROOF || Game.LocalPlayer.Character.IsFireProof;

                    if (Game.GameTime < lastTick + 500)
                        continue;

                    lastTick = Game.GameTime;

                    Fire[] fires = World.GetAllFires();
                    closeFirefighters.Clear();
                    foreach (Fire fire in fires)
                    {
                        if ((fire.DesiredBurnDuration != (float)Settings.FIRE_DESIRED_BURN_DURATION || fire.SpreadRadius != (float)Settings.FIRE_SPREAD_RADIUS) && World.NumberOfFires <= Settings.MAX_FIRES)
                        {
                            fire.DesiredBurnDuration = (float)Settings.FIRE_DESIRED_BURN_DURATION;
                            fire.SpreadRadius = (float)Settings.FIRE_SPREAD_RADIUS;
                        }

                        if (Game.LocalPlayer.Character.IsShooting && Game.LocalPlayer.Character.Inventory.EquippedWeapon.Hash == WeaponHash.FireExtinguisher)
                        {
                            float distanceNear = DistanceSquared2D(Game.LocalPlayer.Character.GetOffsetPositionFront(1f), fire.Position);
                            float distanceFar = DistanceSquared2D(Game.LocalPlayer.Character.GetOffsetPositionFront(2f), fire.Position);
                            if (distanceNear < 1f)
                            {
                                fire.ElapsedBurnDuration += (float)Settings.FIRE_ELAPSED_TIME_INCREMENT_PLAYER * 2;
                                //NativeFunction.Natives.DRAW_LINE(fire.Position.X, fire.Position.Y, fire.Position.Z, Game.LocalPlayer.Character.Position.X, Game.LocalPlayer.Character.Position.Y, Game.LocalPlayer.Character.Position.Z, 255, 255, 0, 255);
                            }
                            else if (distanceFar < (1.5f * 1.5))
                            {
                                fire.ElapsedBurnDuration += ((float)Settings.FIRE_ELAPSED_TIME_INCREMENT_PLAYER / 2) * 2;
                                //NativeFunction.Natives.DRAW_LINE(fire.Position.X, fire.Position.Y, fire.Position.Z, Game.LocalPlayer.Character.Position.X, Game.LocalPlayer.Character.Position.Y, Game.LocalPlayer.Character.Position.Z, 255, 255, 0, 50);
                            }

                        }
                        var firefighters = World.GetEntities(fire.Position, 3f, GetEntitiesFlags.ConsiderHumanPeds | GetEntitiesFlags.ExcludePlayerPed)
                           .OfType<Ped>()
                           .Where(ped => ped.IsAlive && ped.Inventory.EquippedWeapon?.Hash == WeaponHash.FireExtinguisher && ped.IsAiming);

                        foreach (var firefighter in firefighters)
                        {
                            fire.ElapsedBurnDuration += (float)Settings.FIRE_ELAPSED_TIME_INCREMENT_NPC * 2f;
                            //NativeFunction.Natives.DRAW_LINE(fire.Position.X, fire.Position.Y, fire.Position.Z+1, firefighter.Position.X, firefighter.Position.Y, firefighter.Position.Z, 255, 255, 0, 255);
                        }

                        if (closeFirefighters.Count == 0)
                            continue;
                        foreach (Ped firefighter in closeFirefighters)
                        {
                            if (firefighter.IsAlive && firefighter.Inventory.EquippedWeapon != null && firefighter.Inventory.EquippedWeapon.Hash == WeaponHash.FireExtinguisher && firefighter.IsAiming)
                                {
                                fire.ElapsedBurnDuration += (float)Settings.FIRE_ELAPSED_TIME_INCREMENT_NPC * 2;
                                //Rage.Native.NativeFunction.Natives.DRAW_LINE(fire.Position.X, fire.Position.Y, fire.Position.Z+1, firefighter.Position.X, firefighter.Position.Y, firefighter.Position.Z, 255, 255, 0, 255);
                            }
                        }
                    }
                }
            });
        }

        private static float DistanceSquared2D(Vector3 pos1, Vector3 pos2)
        {
            float distX = pos1.X - pos2.X;
            float distY = pos1.Y - pos2.Y;
            return distX * distX + distY * distY;
        }
    }

    public static class DeleteAllFireCommand
    {
        [Rage.Attributes.ConsoleCommand]
        public static void Command_RemoveAllFire()
        {
            Fire[] fire = World.GetAllFires();
            foreach (Fire f in fire)
            {
                f.Delete();
            }
            Game.LogTrivial($"{fire.Length} fire(s) removed.");
        }
    }
}
