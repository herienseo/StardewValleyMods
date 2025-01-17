﻿using Netcode;
using Newtonsoft.Json;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;
using System.Collections.Generic;
using System.Linq;
using Object = StardewValley.Object;

namespace Restauranteer
{
    public partial class ModEntry
    {
        private void UpdateOrders()
        {
            foreach(var c in Game1.player.currentLocation.characters)
            {

                if (c.isVillager() && !Config.IgnoredNPCs.Contains(c.Name))
                {
                    CheckOrder(c);
                }
                else
                {
                    c.modData.Remove(orderKey);
                }
            }
        }

        private void CheckOrder(NPC npc)
        {
            if (npc.modData.TryGetValue(orderKey, out string orderData))
            {
                //npc.modData.Remove(orderKey);
                UpdateOrder(npc, JsonConvert.DeserializeObject<OrderData>(orderData));
                return;
            }
            if (!Game1.NPCGiftTastes.ContainsKey(npc.Name) || npcOrderNumbers.Value.TryGetValue(npc.Name, out int amount) && amount >= Config.MaxNPCOrdersPerNight)
                return;
            if(Game1.random.NextDouble() < Config.OrderChance)
            {
                StartOrder(npc);
            }
        }

        private void UpdateOrder(NPC npc, OrderData orderData)
        {
            if (!npc.IsEmoting)
            {
                npc.doEmote(424242, false);
            }
        }

        private void StartOrder(NPC npc)
        {
            List<int> loves = new();
            foreach(var str in Game1.NPCGiftTastes["Universal_Love"].Split(' '))
            {
                if (int.TryParse(str, out int i) && Game1.objectInformation.TryGetValue(i, out string data) && CraftingRecipe.cookingRecipes.ContainsKey(data.Split('/')[0]))
                {
                    loves.Add(int.Parse(str));
                }
            }
            foreach(var str in Game1.NPCGiftTastes[npc.Name].Split('/')[1].Split(' '))
            {
                if (int.TryParse(str, out int i) && Game1.objectInformation.TryGetValue(i, out string data) && CraftingRecipe.cookingRecipes.ContainsKey(data.Split('/')[0]))
                {
                    loves.Add(int.Parse(str));
                }
            }
            List<int> likes = new();
            foreach(var str in Game1.NPCGiftTastes["Universal_Like"].Split(' '))
            {
                if (int.TryParse(str, out int i) && Game1.objectInformation.TryGetValue(i, out string data) && CraftingRecipe.cookingRecipes.ContainsKey(data.Split('/')[0]))
                {
                    likes.Add(int.Parse(str));
                }
            }
            foreach (var str in Game1.NPCGiftTastes[npc.Name].Split('/')[3].Split(' '))
            {
                if (int.TryParse(str, out int i) && Game1.objectInformation.TryGetValue(i, out string data) && CraftingRecipe.cookingRecipes.ContainsKey(data.Split('/')[0]))
                {
                    likes.Add(int.Parse(str));
                }
            }
            if (!loves.Any() && !likes.Any())
                return;
            bool loved = true;
            int dish;
            if (loves.Any() && (!likes.Any() || (Game1.random.NextDouble() <= Config.LovedDishChance)))
            {
                dish = loves[Game1.random.Next(loves.Count)];
            }
            else
            {
                loved = false;
                dish = likes[Game1.random.Next(likes.Count)];
            }
            var name = Game1.objectInformation[dish].Split('/')[0];
            Monitor.Log($"{npc.Name} is going to order {name}");
            npc.modData[orderKey] = JsonConvert.SerializeObject(new OrderData(dish, name, loved));
        }

        private static NetRef<Chest> GetFridge(GameLocation __instance)
        {
            if(__instance is FarmHouse)
            {
                return (__instance as FarmHouse).fridge;
            }
            if(__instance is IslandFarmHouse)
            {
                return (__instance as IslandFarmHouse).fridge;
            }
            NetRef<Chest> fridge;
            if (__instance.Objects.TryGetValue(fridgeHideTile, out Object value) && value is Chest)
            {
                fridge = new NetRef<Chest>(value as Chest);

            }
            else
            {
                SMonitor.Log($"adding fridge to {__instance.Name}");
                fridge = new NetRef<Chest>(new Chest(true, 130));
                __instance.objects.Add(fridgeHideTile, fridge.Value);
            }

            __instance.NetFields.AddFields(new INetSerializable[]
            {
                fridge
            });
            return fridge;
        }
    }
}