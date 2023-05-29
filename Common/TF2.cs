using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TF2.Content.Items.Scout;
using TF2.Content.Items.Soldier;
using TF2.Content.Items.Pyro;
using TF2.Content.Items.Demoman;
using TF2.Content.Items.Heavy;
using TF2.Content.Items.Engineer;
using TF2.Content.Items.Medic;
using TF2.Content.Items.Sniper;
using TF2.Content.Items.Spy;
using TF2.Gensokyo.Content.Items.BossSummons;

namespace TF2
{
    public class TF2 : Mod
    {
        // Registers a new custom currency
        public static readonly int Australium = CustomCurrencyManager.RegisterCurrency(new Content.Items.Currencies.AustraliumCurrency(ModContent.ItemType<Content.Items.Currencies.Australium>(), 999L, "Australium"));

        private static Mod Gensokyo;

        public override void Load() => ModLoader.TryGetMod("Gensokyo", out Gensokyo);

        public override void Unload() => Gensokyo = null;

        public static bool FindPlayer(Projectile projectile, float maxDetectDistance)
        {
            bool playerFound = false;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            for (int i = 0; i < Main.PlayerList.Count; i++)
            {
                Player target = Main.player[i];
                float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, projectile.Center);

                // Check if it is within the radius
                if (sqrDistanceToTarget < sqrMaxDetectDistance && target == Main.player[projectile.owner])
                {
                    sqrMaxDetectDistance = sqrDistanceToTarget;
                    playerFound = true;
                }
            }

            return playerFound;
        }

        public static void Dialogue(string text, Color color)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(text, color);
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(25, -1, -1, NetworkText.FromLiteral(text), 255, color.R, color.G, color.B, 0, 0, 0);
        }

        // Call() cannot be in the GensokyoDLC class
        public override object Call(params object[] args)
        {
            try
            {
                string message = args[0] as string;

                switch (message)
                {
                    case "AddGensokyoShopItem":
                        if (Gensokyo != null)
                        {
                            AddShopItem((int)args[1], NPC.downedMoonlord, "Tools", ModContent.ItemType<BossRushSummon>(), 1);

                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<ForceANature>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<Shortstop>(), 1);
                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<BonkAtomicPunch>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<MadMilk>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<Sandman>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<HolyMackerel>(), 1);

                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<DirectHit>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<BlackBox>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<RocketJumper>(), 1);
                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<BuffBanner>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<Gunboats>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<BattalionsBackup>(), 1);
                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<Equalizer>(), 1);

                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<Backburner>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<Degreaser>(), 1);
                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<FlareGun>(), 1);
                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<Axtinguisher>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<Powerjack>(), 1);

                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<ScottishResistance>(), 1);
                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<CharginTarge>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<StickyJumper>(), 1);
                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<Eyelander>(), 1);

                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<Natascha>(), 1);
                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<Sandvich>(), 1);
                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<KillingGlovesofBoxing>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<GlovesofRunningUrgently>(), 1);

                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<FrontierJustice>(), 1);
                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<Wrangler>(), 1);
                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<Gunslinger>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<SouthernHospitality>(), 1);

                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<Blutsauger>(), 1);
                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<Kritzkrieg>(), 1);
                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<Ubersaw>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<VitaSaw>(), 1);

                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<Huntsman>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<SydneySleeper>(), 1);
                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<Jarate>(), 1);
                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<Razorback>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<DarwinsDangerShield>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<Bushwacka>(), 1);

                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<Ambassador>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<LEtranger>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<YourEternalReward>(), 1);
                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<CloakandDagger>(), 1);
                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<DeadRinger>(), 1);
                        }
                        return "Success";
                }
                Logger.Debug("GensokyoDLC Call Error: Unknown Message: " + message);
            }
            catch (Exception e)
            {
                Logger.Warn("GensokyoDLC Call Error: " + e.StackTrace + e.Message);
            }
            return "Failure";
        }

        private static void AddShopItem(int argument, bool availability, string category, int type, int amount) => Gensokyo.Call("AddShopItem", argument, availability, category, type, amount, (int)ItemID.None, 0);
    }

    public class SaxtonHaleSpawn : ModSystem
    {
        public static bool saxtonHaleSummoned = false;

        public override void OnWorldLoad() => saxtonHaleSummoned = false;

        public override void OnWorldUnload() => saxtonHaleSummoned = false;

        // We save our data sets using TagCompounds.
        // NOTE: The tag instance provided here is always empty by default.
        public override void SaveWorldData(TagCompound tag)
        {
            if (saxtonHaleSummoned)
                tag["saxtonHaleSummoned"] = true;
        }

        public override void LoadWorldData(TagCompound tag) => saxtonHaleSummoned = tag.ContainsKey("saxtonHaleSummoned");
    }
}