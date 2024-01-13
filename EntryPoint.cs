using Rage;
using System.Linq;

[assembly: Rage.Attributes.Plugin("MoreFire", Description = "A plugin that makes fires last longer.", Author = "SSStuart")]


namespace MoreFire
{
    public class EntryPoint
    {
        public static string pluginName = "MoreFire";
        public static string pluginVersion = "v 0.1.0";
        public static void Main()
        {
            Game.LogTrivial(pluginName + " loaded.");

            Settings.LoadSettings();
            Game.LogTrivial("[" + pluginName + "] Plugin settings loaded.");
            Game.LogTrivial("[" + pluginName + "] FIRE_DESIRED_BURN_DURATION : " + Settings.FIRE_DESIRED_BURN_DURATION + " | FIRE_SPREAD_RADIUS : " + Settings.FIRE_SPREAD_RADIUS + " | FIRE_ELAPSED_TIME_INCREMENT_PLAYER : " + Settings.FIRE_ELAPSED_TIME_INCREMENT_PLAYER + " | -NPC : " + Settings.FIRE_ELAPSED_TIME_INCREMENT_NPC);

            GameFiber.StartNew(delegate
            {
                Entity[] closeFirefighters = new Entity[0];
                int nbFirefighters = 0;
                uint lastTick = 0;


                while (true)
                {
                    GameFiber.Yield();

                    if (Game.GameTime > lastTick + 500)
                    {
                        lastTick = Game.GameTime;

                        nbFirefighters = 0;
                        Fire[] fires = World.GetAllFires();
                        foreach (Fire fire in fires)
                        {
                            if (fire.DesiredBurnDuration != (float)Settings.FIRE_DESIRED_BURN_DURATION || fire.SpreadRadius != (float)Settings.FIRE_SPREAD_RADIUS)
                            {
                                fire.DesiredBurnDuration = (float)Settings.FIRE_DESIRED_BURN_DURATION;
                                fire.SpreadRadius = (float)Settings.FIRE_SPREAD_RADIUS;
                            }

                            closeFirefighters = World.GetEntities(fire.Position, 2f, GetEntitiesFlags.ConsiderHumanPeds | GetEntitiesFlags.ExcludePlayerPed);
                            nbFirefighters += closeFirefighters.Count();
                        }


                        foreach (Fire fire in fires)
                        {
                            if (Game.LocalPlayer.Character.IsShooting && Game.LocalPlayer.Character.Inventory.EquippedWeapon.Hash == WeaponHash.FireExtinguisher)
                            {
                                float distanceNear = Game.LocalPlayer.Character.GetOffsetPositionFront(1f).DistanceTo2D(fire.Position);
                                float distanceFar = Game.LocalPlayer.Character.GetOffsetPositionFront(2f).DistanceTo2D(fire.Position);
                                if (distanceNear < 1f)
                                {
                                    fire.ElapsedBurnDuration += (float)Settings.FIRE_ELAPSED_TIME_INCREMENT_PLAYER * 2;
                                    //Rage.Native.NativeFunction.Natives.DRAW_LINE(fire.Position.X, fire.Position.Y, fire.Position.Z, Game.LocalPlayer.Character.Position.X, Game.LocalPlayer.Character.Position.Y, Game.LocalPlayer.Character.Position.Z, 255, 255, 0, 255);
                                }
                                else if (distanceFar < 1.5f)
                                {
                                    fire.ElapsedBurnDuration += ((float)Settings.FIRE_ELAPSED_TIME_INCREMENT_PLAYER / 2) * 2;
                                    //Rage.Native.NativeFunction.Natives.DRAW_LINE(fire.Position.X, fire.Position.Y, fire.Position.Z, Game.LocalPlayer.Character.Position.X, Game.LocalPlayer.Character.Position.Y, Game.LocalPlayer.Character.Position.Z, 255, 255, 0, 50);
                                }

                            }
                            if (nbFirefighters > 0)
                            {
                                closeFirefighters = World.GetEntities(fire.Position, 5f, GetEntitiesFlags.ConsiderHumanPeds | GetEntitiesFlags.ExcludePlayerPed);
                                if (closeFirefighters.Count() > 0)
                                {
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
                        }
                    }
                }
            });
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
            Game.LogTrivial("[" + EntryPoint.pluginName + "] " + fire.Length + " fire(s) removed.");
        }
    }
}
