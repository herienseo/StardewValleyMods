﻿using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace MapTeleport
{
    public partial class ModEntry : Mod 
    {
        public static ModEntry context;
        public static ModConfig Config;
        public static IModHelper SHelper;
        public static IMonitor SMonitor;
        private Harmony harmony;

        public static string dictPath = "aedenthorn.MapTeleport/coordinates";

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();
            if (!Config.EnableMod)
                return;

            context = this;
            SMonitor = Monitor;
            SHelper = helper;

            helper.Events.Content.AssetRequested += Content_AssetRequested;

            harmony = new Harmony(ModManifest.UniqueID);
            harmony.PatchAll();
        }

        private void Content_AssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo(dictPath))
            {
                e.LoadFromModFile<CoordinatesList>("assets/coordinates.json", AssetLoadPriority.Exclusive);
            }
        }

    }
}
