using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using TF2.Common;
using TF2.Content.Items.Accessories;
using TF2.Content.Items.Ammo;
using TF2.Content.Items.Bundles;
using TF2.Content.Items.Consumables;
using TF2.Content.Items.Placeables.Crafting;
using TF2.Content.Mounts;
using TF2.Content.Projectiles.NPCs;

namespace TF2.Content.NPCs.TownNPCs
{
    // [AutoloadHead] and NPC.townNPC are extremely important and absolutely both necessary for any Town NPC to work at all.
    [AutoloadHead]
    public class SaxtonHale : ModNPC
    {
        public int ai;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 25; // The amount of frames the NPC has

            NPCID.Sets.ExtraFramesCount[Type] = 9; // Generally for Town NPCs, but this is how the NPC does extra things such as sitting in a chair and talking to other NPCs.
            NPCID.Sets.AttackFrameCount[Type] = 4;
            NPCID.Sets.DangerDetectRange[Type] = 700; // The amount of pixels away from the center of the npc that it tries to attack enemies.
            NPCID.Sets.AttackType[Type] = 0;
            NPCID.Sets.AttackTime[Type] = 60; // The amount of time it takes for the NPC's attack animation to be over once it starts.
            NPCID.Sets.AttackAverageChance[Type] = 100;
            NPCID.Sets.HatOffsetY[Type] = 5; // For when a party is active, the party hat spawns at a Y offset.
            NPCID.Sets.MPAllowedEnemies[Type] = true;

            // Influences how the NPC looks in the Bestiary
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new()
            {
                Velocity = 0f, // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
                Direction = 1, // -1 is left and 1 is right. NPCs are drawn facing the left by default but ExamplePerson will be drawn facing the right
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

            // Set Example Person's biome and neighbor preferences with the NPCHappiness hook. You can add happiness text and remarks with localization (See an example in ExampleMod/Localization/en-US.lang).
            // NOTE: The following code uses chaining - a style that works due to the fact that the SetXAffection methods return the same NPCHappiness instance they're called on.
            NPC.Happiness
                .SetBiomeAffection<JungleBiome>(AffectionLevel.Love)
                .SetNPCAffection(ModContent.NPCType<Administrator>(), AffectionLevel.Like)
                .SetNPCAffection(NPCID.Dryad, AffectionLevel.Hate)
            ; // < Mind the semicolon!
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true; // Sets NPC to be a Town NPC
            NPC.friendly = true; // NPC Will not attack player
            NPC.width = 50;
            NPC.height = 110;
            NPC.aiStyle = 7;
            NPC.damage = 100;
            NPC.defense = 100;
            NPC.lifeMax = 1000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;

            AnimationType = NPCID.Guide;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Jungle,

				// Sets your NPC's flavor text in the bestiary.
				new FlavorTextBestiaryInfoElement("Saxton Hale is the CEO of Mann Co. He loves destroying anime girls with his Australian power! He currently has 2764 confirmed kills."),
            });
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
        }

        public override bool CanTownNPCSpawn(int numTownNPCs) => true;

        public override ITownNPCProfile TownNPCProfile() => new SaxtonHaleProfile();

        public override string GetChat()
        {
            WeightedRandom<string> chat = new();

            int dryad = NPC.FindFirstNPC(NPCID.Dryad);
            if (dryad >= 0 && Main.rand.NextBool(10))
                chat.Add("Get goofy ahh " + Main.npc[dryad].GivenName + " away from me! Also tell her to wear some clothes.");
            chat.Add("Welcome to the Mann Co. Store! We sell products and get in fights!", 10.0);
            chat.Add("Browse, buy, design, sell and wear Mann Co.'s ever-growing catalog of fine products with your BARE HANDS--all without leaving the COMFORT OF YOUR CHAIRS!");
            chat.Add("If you aren't 100% satisfied with our product line, you can take it up with me!");
            chat.Add("I love fighting! But do you know what I don't love? Reimu Hakurei.", 0.1);
            chat.Add("I beat up Touhou girls. I am wanted for my war crimes.", 0.1);
            if (!Main.rand.NextBool(100))
                return chat;
            else
            {
                Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_FromThis(), ModContent.ItemType<MannCoStorePackage>(), 1);
                return "Seems like you need a little help. Have this package. It's on me!";
            }
        }

        public enum ShopType
        {
            Armory,
            General,
            Ores,
            Bars,
            Potions,
            MorePotions,
            Enemies,
            Melee,
            Ranger,
            Mage,
            Summoner
        }

        public override void SetChatButtons(ref string button, ref string button2)
        { // What the chat buttons are when you open up the chat UI
            button = "Mann Co. Store";
            if ((ShopType)Main.LocalPlayer.GetModPlayer<TF2Player>().shopRotation != ShopType.MorePotions)
                button2 = "Shop: " + (ShopType)Main.LocalPlayer.GetModPlayer<TF2Player>().shopRotation;
            else
                button2 = "Shop: More Potions";
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
                shopName = ((ShopType)Main.LocalPlayer.GetModPlayer<TF2Player>().shopRotation).ToString();
            else
            {
                Player player = Main.LocalPlayer;
                player.GetModPlayer<TF2Player>().shopRotation++;
                if (player.GetModPlayer<TF2Player>().shopRotation > Enum.GetNames(typeof(ShopType)).Length - 1)
                    player.GetModPlayer<TF2Player>().shopRotation = 0;
            }
        }

        public override void AddShops()
        {
            NPCShop armory = new NPCShop(Type, "Armory")
                .Add(new Item(ModContent.ItemType<MannCoSupplyCrateKey>()) { shopCustomPrice = 1, shopSpecialCurrency = TF2.Australium })
                .Add(new Item(ModContent.ItemType<ScoutBundle>()) { shopCustomPrice = 1, shopSpecialCurrency = TF2.Australium })
                .Add(new Item(ModContent.ItemType<SoldierBundle>()) { shopCustomPrice = 1, shopSpecialCurrency = TF2.Australium })
                .Add(new Item(ModContent.ItemType<PyroBundle>()) { shopCustomPrice = 1, shopSpecialCurrency = TF2.Australium })
                .Add(new Item(ModContent.ItemType<DemomanBundle>()) { shopCustomPrice = 1, shopSpecialCurrency = TF2.Australium })
                .Add(new Item(ModContent.ItemType<HeavyBundle>()) { shopCustomPrice = 1, shopSpecialCurrency = TF2.Australium })
                .Add(new Item(ModContent.ItemType<EngineerBundle>()) { shopCustomPrice = 1, shopSpecialCurrency = TF2.Australium })
                .Add(new Item(ModContent.ItemType<MedicBundle>()) { shopCustomPrice = 1, shopSpecialCurrency = TF2.Australium })
                .Add(new Item(ModContent.ItemType<SniperBundle>()) { shopCustomPrice = 1, shopSpecialCurrency = TF2.Australium })
                .Add(new Item(ModContent.ItemType<SpyBundle>()) { shopCustomPrice = 1, shopSpecialCurrency = TF2.Australium })
                .Add<PrimaryAmmo>()
                .Add<SecondaryAmmo>()
                .Add<SmallHealthPotion>()
                .Add<MediumHealthPotion>()
                .Add<LargeHealthPotion>()
                .Add(new Item(ModContent.ItemType<TF2MountItem>()) { shopCustomPrice = Item.buyPrice(gold: 1) })
                .Add(new Item(ModContent.ItemType<CraftingAnvilItem>()), new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ModContent.ItemType<TournamentStandard>()), new Condition("DownedBoss2", () => NPC.downedBoss2));
            armory.Register();

            NPCShop general = new NPCShop(Type, "General")
                .Add(ItemID.LifeCrystal)
                .Add(ItemID.TerrasparkBoots)
                .Add(ItemID.Shellphone)
                .Add(ItemID.FeralClaws)
                .Add(ItemID.GoldenDelight)
                .Add(new Item(ItemID.AlchemyTable), new Condition("DownedBoss3", () => NPC.downedBoss3))
                .Add(new Item(ItemID.AnkhShield), new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.DiscountCard), new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.GreedyRing), new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.RodofDiscord), new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.CrossNecklace), new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.StarCloak), new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.SliceOfCake), new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.CrystalShard), new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.LifeFruit), new Condition("DownedOneMechBoss", () => NPC.downedMechBossAny))
                .Add(new Item(ItemID.AvengerEmblem), new Condition("DownedMechBoss", () => NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3))
                .Add(new Item(ItemID.LihzahrdAltar), new Condition("DownedPlantera", () => NPC.downedPlantBoss))
                .Add(new Item(ItemID.MasterNinjaGear), new Condition("DownedPlantera", () => NPC.downedPlantBoss))
                .Add(new Item(ItemID.DestroyerEmblem), new Condition("DownedGolem", () => NPC.downedGolemBoss))
                .Add(new Item(ItemID.Picksaw), new Condition("DownedGolem", () => NPC.downedGolemBoss))
                .Add(new Item(ItemID.DestroyerEmblem), new Condition("DownedGolem", () => NPC.downedGolemBoss))
                .Add(new Item(ItemID.GoldenKey) { shopCustomPrice = Item.buyPrice(gold: 5) }, new Condition("DownedBoss3", () => NPC.downedBoss3))
                .Add(new Item(ItemID.ShadowKey), new Condition("DownedBoss3", () => NPC.downedBoss3))
                .Add(new Item(ItemID.JungleKey) { shopCustomPrice = Item.buyPrice(platinum: 1) }, new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.CorruptionKey) { shopCustomPrice = Item.buyPrice(platinum: 1) }, new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.CrimsonKey) { shopCustomPrice = Item.buyPrice(platinum: 1) }, new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.HallowedKey) { shopCustomPrice = Item.buyPrice(platinum: 1) }, new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.FrozenKey) { shopCustomPrice = Item.buyPrice(platinum: 1) }, new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.DungeonDesertKey) { shopCustomPrice = Item.buyPrice(platinum: 1) }, new Condition("HardMode", () => Main.hardMode));
            general.Register();

            NPCShop ores = new NPCShop(Type, "Ores")
                .Add(new Item(ItemID.CopperOre) { shopCustomPrice = Item.buyPrice(copper: 50) })
                .Add(new Item(ItemID.TinOre) { shopCustomPrice = Item.buyPrice(copper: 75) })
                .Add(new Item(ItemID.IronOre) { shopCustomPrice = Item.buyPrice(silver: 1) })
                .Add(new Item(ItemID.LeadOre) { shopCustomPrice = Item.buyPrice(silver: 1, copper: 50) })
                .Add(new Item(ItemID.SilverOre) { shopCustomPrice = Item.buyPrice(silver: 1, copper: 50) })
                .Add(new Item(ItemID.TungstenOre) { shopCustomPrice = Item.buyPrice(silver: 2, copper: 25) })
                .Add(new Item(ItemID.GoldOre) { shopCustomPrice = Item.buyPrice(silver: 3) })
                .Add(new Item(ItemID.PlatinumOre) { shopCustomPrice = Item.buyPrice(silver: 4, copper: 50) })
                .Add(new Item(ItemID.DemoniteOre) { shopCustomPrice = Item.buyPrice(silver: 10) })
                .Add(new Item(ItemID.CrimtaneOre) { shopCustomPrice = Item.buyPrice(silver: 13) })
                .Add(new Item(ItemID.Meteorite) { shopCustomPrice = Item.buyPrice(silver: 2) })
                .Add(new Item(ItemID.Obsidian) { shopCustomPrice = Item.buyPrice(silver: 2, copper: 50) })
                .Add(new Item(ItemID.Hellstone) { shopCustomPrice = Item.buyPrice(silver: 2, copper: 50) }, new Condition("DownedBoss2", () => NPC.downedBoss2))
                .Add(new Item(ItemID.CobaltOre) { shopCustomPrice = Item.buyPrice(silver: 7) }, new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.PalladiumOre) { shopCustomPrice = Item.buyPrice(silver: 9) }, new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.MythrilOre) { shopCustomPrice = Item.buyPrice(silver: 11) }, new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.OrichalcumOre) { shopCustomPrice = Item.buyPrice(silver: 13) }, new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.AdamantiteOre) { shopCustomPrice = Item.buyPrice(silver: 15) }, new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.TitaniumOre) { shopCustomPrice = Item.buyPrice(silver: 17) }, new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.ChlorophyteOre) { shopCustomPrice = Item.buyPrice(silver: 15) }, new Condition("DownedMechBoss", () => NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3))
                .Add(new Item(ItemID.LunarOre) { shopCustomPrice = Item.buyPrice(silver: 30) }, new Condition("DownedMoonLord", () => NPC.downedMoonlord))
                .Add(new Item(ItemID.Amethyst) { shopCustomPrice = Item.buyPrice(silver: 3, copper: 75) })
                .Add(new Item(ItemID.Topaz) { shopCustomPrice = Item.buyPrice(silver: 7, copper: 50) })
                .Add(new Item(ItemID.Sapphire) { shopCustomPrice = Item.buyPrice(silver: 11, copper: 25) })
                .Add(new Item(ItemID.Emerald) { shopCustomPrice = Item.buyPrice(silver: 15) })
                .Add(new Item(ItemID.Ruby) { shopCustomPrice = Item.buyPrice(silver: 22, copper: 50) })
                .Add(new Item(ItemID.Amber) { shopCustomPrice = Item.buyPrice(silver: 30) })
                .Add(new Item(ItemID.Diamond) { shopCustomPrice = Item.buyPrice(silver: 30) });
            ores.Register();

            NPCShop bars = new NPCShop(Type, "Bars")
                .Add(new Item(ItemID.CopperBar) { shopCustomPrice = Item.buyPrice(silver: 7, copper: 50) })
                .Add(new Item(ItemID.TinBar) { shopCustomPrice = Item.buyPrice(silver: 11, copper: 25) })
                .Add(new Item(ItemID.IronBar) { shopCustomPrice = Item.buyPrice(silver: 15) })
                .Add(new Item(ItemID.LeadBar) { shopCustomPrice = Item.buyPrice(silver: 22, copper: 50) })
                .Add(new Item(ItemID.SilverBar) { shopCustomPrice = Item.buyPrice(silver: 30) })
                .Add(new Item(ItemID.TungstenBar) { shopCustomPrice = Item.buyPrice(silver: 45) })
                .Add(new Item(ItemID.GoldBar) { shopCustomPrice = Item.buyPrice(silver: 60) })
                .Add(new Item(ItemID.PlatinumBar) { shopCustomPrice = Item.buyPrice(silver: 90) })
                .Add(new Item(ItemID.DemoniteBar) { shopCustomPrice = Item.buyPrice(gold: 1, silver: 50) })
                .Add(new Item(ItemID.CrimtaneBar) { shopCustomPrice = Item.buyPrice(gold: 2, silver: 36) })
                .Add(new Item(ItemID.MeteoriteBar) { shopCustomPrice = Item.buyPrice(silver: 70) })
                .Add(new Item(ItemID.HellstoneBar) { shopCustomPrice = Item.buyPrice(gold: 3, silver: 50) }, new Condition("DownedBoss2", () => NPC.downedBoss2))
                .Add(new Item(ItemID.CobaltBar) { shopCustomPrice = Item.buyPrice(gold: 1, silver: 5) }, new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.PalladiumBar) { shopCustomPrice = Item.buyPrice(gold: 1, silver: 35) }, new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.MythrilBar) { shopCustomPrice = Item.buyPrice(gold: 2, silver: 20) }, new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.OrichalcumBar) { shopCustomPrice = Item.buyPrice(gold: 2, silver: 60) }, new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.AdamantiteBar) { shopCustomPrice = Item.buyPrice(gold: 3) }, new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.TitaniumBar) { shopCustomPrice = Item.buyPrice(gold: 3, silver: 40) }, new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.HallowedBar) { shopCustomPrice = Item.buyPrice(gold: 3, silver: 75) }, new Condition("DownedOneMechBoss", () => NPC.downedMechBossAny))
                .Add(new Item(ItemID.ChlorophyteBar) { shopCustomPrice = Item.buyPrice(gold: 4, silver: 50) }, new Condition("DownedMechBoss", () => NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3))
                .Add(new Item(ItemID.ShroomiteBar) { shopCustomPrice = Item.buyPrice(gold: 5) }, new Condition("DownedGolem", () => NPC.downedGolemBoss))
                .Add(new Item(ItemID.SpectreBar) { shopCustomPrice = Item.buyPrice(gold: 5) }, new Condition("DownedGolem", () => NPC.downedGolemBoss))
                .Add(new Item(ItemID.LunarBar) { shopCustomPrice = Item.buyPrice(gold: 6) }, new Condition("DownedMoonLord", () => NPC.downedMoonlord));
            bars.Register();

            NPCShop potions = new NPCShop(Type, "Potions")
                .Add(ItemID.BottledWater)
                .Add(ItemID.LesserHealingPotion)
                .Add(ItemID.HealingPotion)
                .Add(new Item(ItemID.GreaterHealingPotion), new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.SuperHealingPotion), new Condition("DownedLunaticCultist", () => NPC.downedAncientCultist))
                .Add(ItemID.LesserManaPotion)
                .Add(ItemID.ManaPotion)
                .Add(new Item(ItemID.GreaterManaPotion), new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.SuperManaPotion), new Condition("HardMode", () => Main.hardMode))
                .Add(ItemID.LuckPotionLesser)
                .Add(ItemID.LuckPotion)
                .Add(ItemID.LuckPotionGreater)
                .Add(ItemID.AmmoReservationPotion)
                .Add(ItemID.ArcheryPotion)
                .Add(ItemID.BattlePotion)
                .Add(ItemID.BuilderPotion)
                .Add(ItemID.CalmingPotion)
                .Add(ItemID.CratePotion)
                .Add(ItemID.TrapsightPotion)
                .Add(ItemID.EndurancePotion)
                .Add(ItemID.FeatherfallPotion)
                .Add(ItemID.FishingPotion)
                .Add(ItemID.FlipperPotion)
                .Add(ItemID.GenderChangePotion)
                .Add(ItemID.GillsPotion)
                .Add(ItemID.GravitationPotion)
                .Add(ItemID.EndurancePotion)
                .Add(ItemID.HeartreachPotion)
                .Add(ItemID.HunterPotion)
                .Add(ItemID.InfernoPotion)
                .Add(ItemID.InvisibilityPotion)
                .Add(ItemID.IronskinPotion);
            potions.Register();

            NPCShop morePotions = new NPCShop(Type, "MorePotions")
                .Add(ItemID.EndurancePotion)
                .Add(new Item(ItemID.LifeforcePotion), new Condition("HardMode", () => Main.hardMode))
                .Add(ItemID.EndurancePotion)
                .Add(ItemID.MagicPowerPotion)
                .Add(ItemID.ManaRegenerationPotion)
                .Add(ItemID.MiningPotion)
                .Add(ItemID.NightOwlPotion)
                .Add(ItemID.ObsidianSkinPotion)
                .Add(ItemID.RagePotion)
                .Add(ItemID.RecallPotion)
                .Add(ItemID.RegenerationPotion)
                .Add(ItemID.ShinePotion)
                .Add(ItemID.SonarPotion)
                .Add(ItemID.SpelunkerPotion)
                .Add(ItemID.SummoningPotion)
                .Add(ItemID.SwiftnessPotion)
                .Add(ItemID.TeleportationPotion)
                .Add(ItemID.ThornsPotion)
                .Add(ItemID.TitanPotion)
                .Add(ItemID.WarmthPotion)
                .Add(ItemID.WaterWalkingPotion)
                .Add(ItemID.WormholePotion)
                .Add(ItemID.WrathPotion);
            morePotions.Register();

            NPCShop enemies = new NPCShop(Type, "Enemies")
                .Add(new Item(ItemID.CursedFlame), new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.Ichor), new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.SoulofLight), new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.SoulofNight), new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.SoulofFlight), new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.SoulofMight), new Condition("DownedDestroyer", () => NPC.downedMechBoss1))
                .Add(new Item(ItemID.SoulofSight), new Condition("DownedTwins", () => NPC.downedMechBoss2))
                .Add(new Item(ItemID.SoulofFright), new Condition("DownedSkeletronPrime", () => NPC.downedMechBoss3))
                .Add(new Item(ItemID.DarkShard), new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.LightShard), new Condition("HardMode", () => Main.hardMode));
            enemies.Register();

            NPCShop melee = new NPCShop(Type, "Melee")
                .Add(ItemID.CopperShortsword)
                .Add(ItemID.Starfury)
                .Add(ItemID.EnchantedSword)
                .Add(ItemID.BladeofGrass)
                .Add(new Item(ItemID.BeeKeeper), new Condition("DownedQueenBee", () => NPC.downedQueenBee))
                .Add(new Item(ItemID.Muramasa), new Condition("DownedBoss3", () => NPC.downedBoss3))
                .Add(new Item(ItemID.Seedler), new Condition("DownedPlantera", () => NPC.downedPlantBoss))
                .Add(new Item(ItemID.TheHorsemansBlade), new Condition("DownedPlantera", () => NPC.downedPlantBoss))
                .Add(new Item(ItemID.InfluxWaver), new Condition("DownedGolem", () => NPC.downedGolemBoss))
                .Add(new Item(ItemID.Meowmere), new Condition("DownedMoonLord", () => NPC.downedMoonlord))
                .Add(new Item(ItemID.StarWrath), new Condition("DownedMoonLord", () => NPC.downedMoonlord))
                .Add(new Item(ItemID.BrokenHeroSword), new Condition("DownedPlantera", () => NPC.downedPlantBoss))
                .Add(new Item(ItemID.ShadowFlameKnife), new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.YoyoBag), new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.BouncingShield), new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.BerserkerGlove), new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.TurtleShell), new Condition("DownedMechBoss", () => NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3))
                .Add(new Item(ItemID.FireGauntlet), new Condition("DownedMechBoss", () => NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3))
                .Add(new Item(ItemID.BeetleHusk), new Condition("DownedGolem", () => NPC.downedGolemBoss))
                .Add(new Item(ItemID.CelestialShell), new Condition("DownedGolem", () => NPC.downedGolemBoss))
                .Add(ItemID.SharpeningStation)
                .Add(new Item(ItemID.FragmentSolar), new Condition("DownedLunaticCultist", () => NPC.downedAncientCultist));
            melee.Register();

            NPCShop ranger = new NPCShop(Type, "Ranger")
                .Add(new Item(ItemID.PhoenixBlaster), new Condition("DownedBoss3", () => NPC.downedBoss3))
                .Add(new Item(ItemID.ZapinatorGray), new Condition("DownedAnyBoss", () => NPC.downedBoss1 || NPC.downedBoss2 || NPC.downedBoss3 || NPC.downedQueenBee))
                .Add(new Item(ItemID.ZapinatorOrange), new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.DaedalusStormbow), new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.Megashark), new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.Uzi), new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.SDMG), new Condition("DownedMoonLord", () => NPC.downedMoonlord))
                .Add(new Item(ItemID.TacticalShotgun), new Condition("DownedPlantera", () => NPC.downedPlantBoss))
                .Add(new Item(ItemID.Celeb2), new Condition("DownedMoonLord", () => NPC.downedMoonlord))
                .Add(new Item(ItemID.MoltenQuiver), new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.StalkersQuiver), new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.ReconScope), new Condition("DownedGolem", () => NPC.downedGolemBoss))
                .Add(new Item(ItemID.EndlessQuiver), new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.EndlessMusketPouch), new Condition("HardMode", () => Main.hardMode))
                .Add(ItemID.AmmoBox)
                .Add(new Item(ItemID.FragmentVortex), new Condition("DownedLunaticCultist", () => NPC.downedAncientCultist));
            ranger.Register();

            NPCShop mage = new NPCShop(Type, "Mage")
                .Add(ItemID.ManaCrystal)
                .Add(ItemID.DemonScythe)
                .Add(new Item(ItemID.WaterBolt), new Condition("DownedBoss3", () => NPC.downedBoss3))
                .Add(new Item(ItemID.SkyFracture), new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.MeteorStaff), new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.CrystalSerpent), new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.UnholyTrident), new Condition("DownedOneMechBoss", () => NPC.downedMechBossAny))
                .Add(new Item(ItemID.ShadowbeamStaff), new Condition("DownedPlantera", () => NPC.downedPlantBoss))
                .Add(new Item(ItemID.InfernoFork), new Condition("DownedPlantera", () => NPC.downedPlantBoss))
                .Add(new Item(ItemID.SpectreStaff), new Condition("DownedPlantera", () => NPC.downedPlantBoss))
                .Add(new Item(ItemID.LastPrism), new Condition("DownedMoonLord", () => NPC.downedMoonlord))
                .Add(new Item(ItemID.LunarFlareBook), new Condition("DownedMoonLord", () => NPC.downedMoonlord))
                .Add(ItemID.ManaFlower)
                .Add(ItemID.CelestialMagnet)
                .Add(ItemID.MagicCuffs)
                .Add(new Item(ItemID.CrystalBall), new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.Ectoplasm), new Condition("DownedPlantera", () => NPC.downedPlantBoss))
                .Add(new Item(ItemID.FragmentNebula), new Condition("DownedLunaticCultist", () => NPC.downedAncientCultist));
            mage.Register();

            NPCShop summoner = new NPCShop(Type, "Summoner")
                .Add(ItemID.SlimeStaff)
                .Add(ItemID.FlinxStaff)
                .Add(ItemID.VampireFrogStaff)
                .Add(ItemID.AbigailsFlower)
                .Add(new Item(ItemID.SanguineStaff), new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.Smolstar), new Condition("DownedQueenSlime", () => NPC.downedQueenSlime))
                .Add(new Item(ItemID.OpticStaff), new Condition("DownedTwins", () => NPC.downedMechBoss2))
                .Add(new Item(ItemID.PygmyStaff), new Condition("DownedPlantera", () => NPC.downedPlantBoss))
                .Add(new Item(ItemID.EmpressBlade), new Condition("DownedEmpressOfLight", () => NPC.downedEmpressOfLight))
                .Add(new Item(ItemID.MoonlordTurretStaff), new Condition("DownedMoonLord", () => NPC.downedMoonlord))
                .Add(new Item(ItemID.RainbowCrystalStaff), new Condition("DownedMoonLord", () => NPC.downedMoonlord))
                .Add(ItemID.ThornWhip)
                .Add(new Item(ItemID.BoneWhip), new Condition("DownedBoss3", () => NPC.downedBoss3))
                .Add(new Item(ItemID.FireWhip), new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.CoolWhip), new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.SwordWhip), new Condition("DownedOneMechBoss", () => NPC.downedMechBossAny))
                .Add(new Item(ItemID.ScytheWhip), new Condition("DownedPlantera", () => NPC.downedPlantBoss))
                .Add(new Item(ItemID.MaceWhip), new Condition("DownedPlantera", () => NPC.downedPlantBoss))
                .Add(new Item(ItemID.RainbowWhip), new Condition("DownedEmpressOfLight", () => NPC.downedEmpressOfLight))
                .Add(ItemID.PygmyNecklace)
                .Add(new Item(ItemID.NecromanticScroll), new Condition("DownedPlantera", () => NPC.downedPlantBoss))
                .Add(new Item(ItemID.PapyrusScarab), new Condition("DownedPlantera", () => NPC.downedPlantBoss))
                .Add(new Item(ItemID.BewitchingTable), new Condition("DownedBoss3", () => NPC.downedBoss3))
                .Add(new Item(ItemID.FragmentStardust), new Condition("DownedLunaticCultist", () => NPC.downedAncientCultist));
            summoner.Register();
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
        }

        // Make this Town NPC teleport to the King and/or Queen statue when triggered.
        public override bool CanGoToStatue(bool toKingStatue) => true;

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 100;
            knockback = 20f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 60;
            randExtraCooldown = 0;
        }

        // todo: implement
        // public override void TownNPCAttackProj(ref int projType, ref int attackDelay) {
        // 	projType = ProjectileType<SparklingBall>();
        // 	attackDelay = 1;
        // }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 12f;
            randomOffset = 2f;
        }

        public override void AI()
        {
            ai += 1;
            float distanceFromTarget = 1000f;
            Vector2 targetCenter = NPC.Center;
            bool foundTarget = false;
            if (!foundTarget && ai >= 60) //&& Main.netMode != NetmodeID.MultiplayerClient
            {
                // This code is required either way, used for finding a target
                foreach (NPC targetNPC in Main.npc)
                {
                    ai = 0;
                    if (targetNPC.CanBeChasedBy() && targetNPC.type != NPCID.TargetDummy)
                    {
                        float between = Vector2.Distance(targetNPC.Center, NPC.Center);
                        bool closest = Vector2.Distance(NPC.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, targetNPC.position, targetNPC.width, targetNPC.height);
                        // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                        // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
                        bool closeThroughWall = between < 100f;
                        if ((closest && inRange || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            targetCenter = targetNPC.Center;
                            foundTarget = true;
                        }
                    }
                }
                if (foundTarget)
                {
                    Vector2 shootVel = targetCenter - NPC.Center;
                    if (shootVel == Vector2.Zero)
                        shootVel = Vector2.UnitY;
                    if ((targetCenter - NPC.Center).X > 0f)
                        NPC.spriteDirection = NPC.direction = 1;
                    else if ((targetCenter - NPC.Center).X < 0f)
                        NPC.spriteDirection = NPC.direction = -1;
                    float speed = 10f;
                    int type = ModContent.ProjectileType<KnifeProjectileNPC>();
                    int damage = NPC.damage;
                    IEntitySource projectileSource = NPC.GetSource_FromAI();
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/melee_swing"), NPC.Center);
                    if ((targetCenter - NPC.Center).Y >= 0f)
                        NPC.velocity = new Vector2(25f * NPC.direction, 15f);
                    if ((targetCenter - NPC.Center).Y <= 0f)
                        NPC.velocity = new Vector2(25f * NPC.direction, -15f);
                    int projectile = Projectile.NewProjectile(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, Main.myPlayer, 0f, 0f);
                    KnifeProjectileNPC spawnedModProjectile = (KnifeProjectileNPC)Main.projectile[projectile].ModProjectile;
                    spawnedModProjectile.owner = NPC;
                    NetMessage.SendData(MessageID.SyncProjectile, number: projectile);
                }
            }
        }

        public override bool UsesPartyHat() => false;
    }

    public class SaxtonHaleProfile : ITownNPCProfile
    {
        public int RollVariation() => 0;

        public string GetNameForVariant(NPC npc) => npc.getNewNPCName();

        public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc)
        {
            if (npc.IsABestiaryIconDummy && !npc.ForcePartyHatOn)
                return ModContent.Request<Texture2D>("TF2/Content/NPCs/TownNPCs/SaxtonHale");

            if (npc.altTexture == 1)
                return ModContent.Request<Texture2D>("TF2/Content/NPCs/TownNPCs/SaxtonHale");

            return ModContent.Request<Texture2D>("TF2/Content/NPCs/TownNPCs/SaxtonHale");
        }

        public int GetHeadTextureIndex(NPC npc) => ModContent.GetModHeadSlot("TF2/Content/NPCs/TownNPCs/SaxtonHale_Head");
    }
}