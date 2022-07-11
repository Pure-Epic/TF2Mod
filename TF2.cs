using System.Collections;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.GameContent.UI;

namespace TF2
{
    public class TF2 : Mod
    {

        public static int Australium;

        public override void Load()
        {
            // Registers a new custom currency
            Australium = CustomCurrencyManager.RegisterCurrency(new Items.Currencies.AustraliumCurrency(ModContent.ItemType<Items.Australium>(), 999L, "Australium"));
        }
    }

    public class SaxtonHaleSpawn : ModSystem
    {
		public static bool saxtonHaleSummoned = false;

		public override void OnWorldLoad()
		{
			saxtonHaleSummoned = false;
		}

		public override void OnWorldUnload()
		{
			saxtonHaleSummoned = false;
		}

		// We save our data sets using TagCompounds.
		// NOTE: The tag instance provided here is always empty by default.
		public override void SaveWorldData(TagCompound tag)
		{
			if (saxtonHaleSummoned)
			{
				tag["saxtonHaleSummoned"] = true;
			}
		}

		public override void LoadWorldData(TagCompound tag)
		{
			saxtonHaleSummoned = tag.ContainsKey("saxtonHaleSummoned");
		}
	}
}