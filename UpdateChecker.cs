﻿using Rage;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace MoreFire
{
    public static class UpdateChecker
    {
        private static readonly string url = "https://ssstuart.net/api/GTAModVersion/More%20Fire";
        private static readonly HttpClient httpClient = new HttpClient();
        private static Version lastVersion = null;
        private static readonly Version currentVersion = new Version(EntryPoint.pluginVersion);

        public static async System.Threading.Tasks.Task<bool> CheckUpdate()
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                HttpResponseMessage response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseMessage = await response.Content.ReadAsStringAsync();
                Match m = new Regex("last_version\":\"([\\d.]+)\"").Match(responseMessage);

                if (m.Success)
                {
                    System.Text.RegularExpressions.Group g = m.Groups[1];
                    CaptureCollection cc = g.Captures;
                    Capture c = cc[0];
                    lastVersion = new Version(c.ToString());

                    Game.LogTrivial($"[{EntryPoint.pluginName}] Current version: {currentVersion}, Latest version: {lastVersion}");
                    if (currentVersion < lastVersion)
                    {
                        Game.LogTrivial($"[{EntryPoint.pluginName}] Update available ! Current version: {currentVersion}, Latest version: {lastVersion}");
                        return true;
                    } else if (currentVersion >= lastVersion)
                    {
                        Game.LogTrivial($"[{EntryPoint.pluginName}] You are using the latest version ({currentVersion}).");
                        return false;
                    }
                    
                } else
                {
                    Game.LogTrivial($"[{EntryPoint.pluginName}] Update check failed: Could not parse version from response : {responseMessage}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Game.LogTrivial($"[{EntryPoint.pluginName}] Update check failed: {ex.InnerException}");
                return false;
            }

            return false;
        }

        public static void DisplayUpdateNotification()
        {
            do
            {
                GameFiber.Yield();
                GameFiber.Sleep(5000);
            } while (Game.IsLoading);
            Game.DisplayNotification("mpturf", "swap", EntryPoint.pluginName, $"V {lastVersion}", $"~y~Update available !");
        }
    }
}
