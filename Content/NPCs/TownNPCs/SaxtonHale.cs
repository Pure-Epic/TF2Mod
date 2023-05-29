using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using TF2.Content.Items.Bundles;
using TF2.Content.Items.Accessories;
using TF2.Content.Items.Ammo;
using TF2.Content.Items.Consumables;
using TF2.Content.Items.Placeables.Crafting;
using TF2.Content.Mounts;
using TF2.Content.Projectiles.NPCs;
using TF2.Common;

namespace TF2.Content.NPCs.TownNPCs
{
    // [AutoloadHead] and NPC.townNPC are extremely important and absolutely both necessary for any Town NPC to work at all.
    [AutoloadHead]
    public class SaxtonHale : ModNPC
    {
        public int ai;

        public override void SetStaticDefaults()
        {
            // DisplayName automatically assigned from localization files, but the commented line below is the normal approach.
            DisplayName.SetDefault("Saxton Hale");
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
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new(0)
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
				new FlavorTextBestiaryInfoElement("Saxton Hale is the CEO of Mann Co. He loves destorying anime girls with his Australian power! He currently has 2764 confirmed kills."),
            });
        }

        public override void HitEffect(int hitDirection, double damage)
        {

        }

        public override bool CanTownNPCSpawn(int numTownNPCs, int money) => true;

        public override ITownNPCProfile TownNPCProfile() => new SaxtonHaleProfile();

        public override void FindFrame(int frameHeight)
        {
            /*npc.frame.Width = 40;
			if (((int)Main.time / 10) % 2 == 0)
			{
				npc.frame.X = 40;
			}
			else
			{
				npc.frame.X = 0;
			}*/
        }

        public override string GetChat()
        {
            WeightedRandom<string> chat = new();

            int dryad = NPC.FindFirstNPC(NPCID.Dryad);
            if (dryad >= 0 && Main.rand.NextBool(10))
                chat.Add("Get goofy ahh " + Main.npc[dryad].GivenName + " away from me! Also tell her to wear some clothes.");
            // These are things that the NPC has a chance of telling you when you talk to it.
            chat.Add("Welcome to the Mann Co. Store! We sell products and get in fights!", 10.0);
            chat.Add("Browse, buy, design, sell and wear Mann Co.'s ever-growing catalog of fine products with your BARE HANDS--all without leaving the COMFORT OF YOUR CHAIRS!");
            chat.Add("If you aren't 100% satisfied with our product line, you can take it up with me!");
            chat.Add("I love fighting! But do you know what I don't love? Anime.");
            chat.Add("I beat up Touhou girls. I am wanted for my war crimes.", 0.1);
            if (!Main.rand.NextBool(100))
                return chat; // chat is implicitly cast to a string.
            else
            {
                Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_FromThis(), ModContent.ItemType<Items.Consumables.MannCoStorePackage>(), 1);
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

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
                shop = true;
            else
            {
                shop = false;
                Player player = Main.LocalPlayer;
                player.GetModPlayer<TF2Player>().shopRotation++;
                if (player.GetModPlayer<TF2Player>().shopRotation > Enum.GetNames(typeof(ShopType)).Length - 1)
                    player.GetModPlayer<TF2Player>().shopRotation = 0;
            }
        }

        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            switch (Main.LocalPlayer.GetModPlayer<TF2Player>().shopRotation)
            {
                case 0:
                    shop.item[nextSlot].SetDefaults(ModContent.ItemType<MannCoSupplyCrateKey>());
                    shop.item[nextSlot].shopCustomPrice = 1;
                    shop.item[nextSlot].shopSpecialCurrency = TF2.Australium;
                    nextSlot++;

                    shop.item[nextSlot].SetDefaults(ModContent.ItemType<ScoutBundle>());
                    shop.item[nextSlot].shopCustomPrice = 1;
                    shop.item[nextSlot].shopSpecialCurrency = TF2.Australium;
                    nextSlot++;

                    shop.item[nextSlot].SetDefaults(ModContent.ItemType<SoldierBundle>());
                    shop.item[nextSlot].shopCustomPrice = 1;
                    shop.item[nextSlot].shopSpecialCurrency = TF2.Australium;
                    nextSlot++;

                    shop.item[nextSlot].SetDefaults(ModContent.ItemType<PyroBundle>());
                    shop.item[nextSlot].shopCustomPrice = 1;
                    shop.item[nextSlot].shopSpecialCurrency = TF2.Australium;
                    nextSlot++;

                    shop.item[nextSlot].SetDefaults(ModContent.ItemType<DemomanBundle>());
                    shop.item[nextSlot].shopCustomPrice = 1;
                    shop.item[nextSlot].shopSpecialCurrency = TF2.Australium;
                    nextSlot++;

                    shop.item[nextSlot].SetDefaults(ModContent.ItemType<HeavyBundle>());
                    shop.item[nextSlot].shopCustomPrice = 1;
                    shop.item[nextSlot].shopSpecialCurrency = TF2.Australium;
                    nextSlot++;

                    shop.item[nextSlot].SetDefaults(ModContent.ItemType<EngineerBundle>());
                    shop.item[nextSlot].shopCustomPrice = 1;
                    shop.item[nextSlot].shopSpecialCurrency = TF2.Australium;
                    nextSlot++;

                    shop.item[nextSlot].SetDefaults(ModContent.ItemType<MedicBundle>());
                    shop.item[nextSlot].shopCustomPrice = 1;
                    shop.item[nextSlot].shopSpecialCurrency = TF2.Australium;
                    nextSlot++;

                    shop.item[nextSlot].SetDefaults(ModContent.ItemType<SniperBundle>());
                    shop.item[nextSlot].shopCustomPrice = 1;
                    shop.item[nextSlot].shopSpecialCurrency = TF2.Australium;
                    nextSlot++;

                    shop.item[nextSlot].SetDefaults(ModContent.ItemType<SpyBundle>());
                    shop.item[nextSlot].shopCustomPrice = 1;
                    shop.item[nextSlot].shopSpecialCurrency = TF2.Australium;
                    nextSlot++;

                    shop.item[nextSlot++].SetDefaults(ModContent.ItemType<PrimaryAmmo>());
                    shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SecondaryAmmo>());

                    shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SmallHealthPotion>());
                    shop.item[nextSlot++].SetDefaults(ModContent.ItemType<MediumHealthPotion>());
                    shop.item[nextSlot++].SetDefaults(ModContent.ItemType<LargeHealthPotion>());

                    shop.item[nextSlot].SetDefaults(ModContent.ItemType<TF2MountItem>());
                    shop.item[nextSlot].shopCustomPrice = Item.buyPrice(gold: 1);
                    nextSlot++;

                    if (Main.hardMode)
                        shop.item[nextSlot++].SetDefaults(ModContent.ItemType<CraftingAnvilItem>());

                    shop.item[nextSlot++].SetDefaults(ModContent.ItemType<TournamentStandard>());
                    break;
                case 1:
                    shop.item[nextSlot++].SetDefaults(ItemID.LifeCrystal);
                    shop.item[nextSlot++].SetDefaults(ItemID.TerrasparkBoots);
                    shop.item[nextSlot++].SetDefaults(ItemID.CellPhone);
                    if (NPC.downedBoss3)
                        shop.item[nextSlot++].SetDefaults(ItemID.AlchemyTable);
                    if (Main.hardMode)
                    {
                        shop.item[nextSlot++].SetDefaults(ItemID.AnkhShield);
                        shop.item[nextSlot++].SetDefaults(ItemID.DiscountCard);
                        shop.item[nextSlot++].SetDefaults(ItemID.GreedyRing);
                        shop.item[nextSlot++].SetDefaults(ItemID.RodofDiscord);
                        shop.item[nextSlot++].SetDefaults(ItemID.CrossNecklace);
                        shop.item[nextSlot++].SetDefaults(ItemID.StarCloak);
                        shop.item[nextSlot++].SetDefaults(ItemID.SliceOfCake);
                        shop.item[nextSlot++].SetDefaults(ItemID.CrystalShard);
                    }
                    if (NPC.downedMechBossAny)
                        shop.item[nextSlot++].SetDefaults(ItemID.LifeFruit);
                    if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
                        shop.item[nextSlot++].SetDefaults(ItemID.AvengerEmblem);
                    if (NPC.downedPlantBoss)
                        shop.item[nextSlot++].SetDefaults(ItemID.LihzahrdAltar);
                    if (NPC.downedGolemBoss)
                    {
                        shop.item[nextSlot++].SetDefaults(ItemID.DestroyerEmblem);
                        shop.item[nextSlot++].SetDefaults(ItemID.MasterNinjaGear);
                        shop.item[nextSlot++].SetDefaults(ItemID.Picksaw);
                    }
                    if (NPC.downedBoss3)
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.GoldenKey);
                        shop.item[nextSlot].shopCustomPrice = Item.buyPrice(gold: 5);
                        nextSlot++;
                        shop.item[nextSlot++].SetDefaults(ItemID.ShadowKey);
                    }
                    if (Main.hardMode)
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.JungleKey);
                        shop.item[nextSlot].shopCustomPrice = Item.buyPrice(platinum: 1);
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ItemID.CorruptionKey);
                        shop.item[nextSlot].shopCustomPrice = Item.buyPrice(platinum: 1);
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ItemID.CrimsonKey);
                        shop.item[nextSlot].shopCustomPrice = Item.buyPrice(platinum: 1);
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ItemID.HallowedKey);
                        shop.item[nextSlot].shopCustomPrice = Item.buyPrice(platinum: 1);
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ItemID.FrozenKey);
                        shop.item[nextSlot].shopCustomPrice = Item.buyPrice(platinum: 1);
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ItemID.DungeonDesertKey);
                        shop.item[nextSlot].shopCustomPrice = Item.buyPrice(platinum: 1);
                        nextSlot++;
                    }
                    break;
                case 2:
                    shop.item[nextSlot].SetDefaults(ItemID.CopperOre);
                    shop.item[nextSlot].shopCustomPrice = Item.buyPrice(copper: 50);
                    nextSlot++;
                    shop.item[nextSlot].SetDefaults(ItemID.TinOre);
                    shop.item[nextSlot].shopCustomPrice = Item.buyPrice(copper: 75);
                    nextSlot++;
                    shop.item[nextSlot].SetDefaults(ItemID.IronOre);
                    shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 1);
                    nextSlot++;
                    shop.item[nextSlot].SetDefaults(ItemID.LeadOre);
                    shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 1, copper: 50);
                    nextSlot++;
                    shop.item[nextSlot].SetDefaults(ItemID.SilverOre);
                    shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 1, copper: 50);
                    nextSlot++;
                    shop.item[nextSlot].SetDefaults(ItemID.TungstenOre);
                    shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 2, copper: 25);
                    nextSlot++;
                    shop.item[nextSlot].SetDefaults(ItemID.GoldOre);
                    shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 3);
                    nextSlot++;
                    shop.item[nextSlot].SetDefaults(ItemID.PlatinumOre);
                    shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 4, copper: 50);
                    nextSlot++;
                    shop.item[nextSlot].SetDefaults(ItemID.DemoniteOre);
                    shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 10);
                    nextSlot++;
                    shop.item[nextSlot].SetDefaults(ItemID.CrimtaneOre);
                    shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 13);
                    nextSlot++;
                    shop.item[nextSlot].SetDefaults(ItemID.Meteorite);
                    shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 2);
                    nextSlot++;
                    shop.item[nextSlot].SetDefaults(ItemID.Obsidian);
                    shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 2, copper: 50);
                    nextSlot++;
                    if (NPC.downedBoss2)
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.Hellstone);
                        shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 2, copper: 50);
                        nextSlot++;
                    }
                    if (Main.hardMode)
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.CobaltOre);
                        shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 7);
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ItemID.PalladiumOre);
                        shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 9);
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ItemID.MythrilOre);
                        shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 11);
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ItemID.OrichalcumOre);
                        shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 13);
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ItemID.AdamantiteOre);
                        shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 15);
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ItemID.TitaniumOre);
                        shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 17);
                        nextSlot++;
                    }
                    if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.ChlorophyteOre);
                        shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 15);
                        nextSlot++;
                    }
                    if (NPC.downedMoonlord)
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.LunarOre);
                        shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 30);
                        nextSlot++;
                    }
                    shop.item[nextSlot].SetDefaults(ItemID.Amethyst);
                    shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 3, copper: 75);
                    nextSlot++;
                    shop.item[nextSlot].SetDefaults(ItemID.Topaz);
                    shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 7, copper: 50);
                    nextSlot++;
                    shop.item[nextSlot].SetDefaults(ItemID.Sapphire);
                    shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 11, copper: 25);
                    nextSlot++;
                    shop.item[nextSlot].SetDefaults(ItemID.Emerald);
                    shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 15);
                    nextSlot++;
                    shop.item[nextSlot].SetDefaults(ItemID.Ruby);
                    shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 22, copper: 50);
                    nextSlot++;
                    shop.item[nextSlot].SetDefaults(ItemID.Amber);
                    shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 30);
                    nextSlot++;
                    shop.item[nextSlot].SetDefaults(ItemID.Diamond);
                    shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 30);
                    nextSlot++;
                    break;
                case 3:
                    shop.item[nextSlot].SetDefaults(ItemID.CopperBar);
                    shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 7, copper: 50);
                    nextSlot++;
                    shop.item[nextSlot].SetDefaults(ItemID.TinBar);
                    shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 11, copper: 25);
                    nextSlot++;
                    shop.item[nextSlot].SetDefaults(ItemID.IronBar);
                    shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 15);
                    nextSlot++;
                    shop.item[nextSlot].SetDefaults(ItemID.LeadBar);
                    shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 22, copper: 50);
                    nextSlot++;
                    shop.item[nextSlot].SetDefaults(ItemID.SilverBar);
                    shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 30);
                    nextSlot++;
                    shop.item[nextSlot].SetDefaults(ItemID.TungstenBar);
                    shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 45);
                    nextSlot++;
                    shop.item[nextSlot].SetDefaults(ItemID.GoldBar);
                    shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 60);
                    nextSlot++;
                    shop.item[nextSlot].SetDefaults(ItemID.PlatinumBar);
                    shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 90);
                    nextSlot++;
                    shop.item[nextSlot].SetDefaults(ItemID.DemoniteBar);
                    shop.item[nextSlot].shopCustomPrice = Item.buyPrice(gold: 1, silver: 50);
                    nextSlot++;
                    shop.item[nextSlot].SetDefaults(ItemID.CrimtaneBar);
                    shop.item[nextSlot].shopCustomPrice = Item.buyPrice(gold: 2, silver: 36);
                    nextSlot++;
                    shop.item[nextSlot].SetDefaults(ItemID.MeteoriteBar);
                    shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 70);
                    nextSlot++;
                    if (NPC.downedBoss2)
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.HellstoneBar);
                        shop.item[nextSlot].shopCustomPrice = Item.buyPrice(silver: 2, copper: 40);
                        nextSlot++;
                    }
                    if (Main.hardMode)
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.CobaltBar);
                        shop.item[nextSlot].shopCustomPrice = Item.buyPrice(gold: 1, silver: 5);
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ItemID.PalladiumBar);
                        shop.item[nextSlot].shopCustomPrice = Item.buyPrice(gold: 1, silver: 35);
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ItemID.MythrilBar);
                        shop.item[nextSlot].shopCustomPrice = Item.buyPrice(gold: 2, silver: 20);
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ItemID.OrichalcumBar);
                        shop.item[nextSlot].shopCustomPrice = Item.buyPrice(gold: 2, silver: 60);
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ItemID.AdamantiteBar);
                        shop.item[nextSlot].shopCustomPrice = Item.buyPrice(gold: 3);
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ItemID.TitaniumBar);
                        shop.item[nextSlot].shopCustomPrice = Item.buyPrice(gold: 3, silver: 40);
                        nextSlot++;
                    }
                    if (NPC.downedMechBossAny)
                        shop.item[nextSlot++].SetDefaults(ItemID.HallowedBar);
                    if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.ChlorophyteBar);
                        shop.item[nextSlot].shopCustomPrice = Item.buyPrice(gold: 4, silver: 50);
                        nextSlot++;
                    }
                    if (NPC.downedPlantBoss)
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.ShroomiteBar);
                        shop.item[nextSlot].shopCustomPrice = Item.buyPrice(gold: 5);
                        nextSlot++;
                        shop.item[nextSlot].SetDefaults(ItemID.SpectreBar);
                        shop.item[nextSlot].shopCustomPrice = Item.buyPrice(gold: 5);
                        nextSlot++;
                    }
                    if (NPC.downedMoonlord)
                    {
                        shop.item[nextSlot].SetDefaults(ItemID.LunarBar);
                        shop.item[nextSlot].shopCustomPrice = Item.buyPrice(gold: 6);
                        nextSlot++;
                    }
                    break;
                case 4:
                    shop.item[nextSlot++].SetDefaults(ItemID.BottledWater);
                    shop.item[nextSlot++].SetDefaults(ItemID.LesserHealingPotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.HealingPotion);
                    if (Main.hardMode)
                        shop.item[nextSlot++].SetDefaults(ItemID.GreaterHealingPotion);
                    if (NPC.downedAncientCultist)
                        shop.item[nextSlot++].SetDefaults(ItemID.SuperHealingPotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.LesserManaPotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.ManaPotion);
                    if (Main.hardMode)
                    {
                        shop.item[nextSlot++].SetDefaults(ItemID.GreaterManaPotion);
                        shop.item[nextSlot++].SetDefaults(ItemID.SuperManaPotion);
                    }
                    shop.item[nextSlot++].SetDefaults(ItemID.LuckPotionLesser);
                    shop.item[nextSlot++].SetDefaults(ItemID.LuckPotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.LuckPotionGreater);
                    shop.item[nextSlot++].SetDefaults(ItemID.AmmoReservationPotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.ArcheryPotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.BattlePotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.BuilderPotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.CalmingPotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.CratePotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.TrapsightPotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.EndurancePotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.FeatherfallPotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.FishingPotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.FlipperPotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.GenderChangePotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.GillsPotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.GravitationPotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.EndurancePotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.HeartreachPotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.HunterPotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.InfernoPotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.InvisibilityPotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.IronskinPotion);
                    break;
                case 5:
                    shop.item[nextSlot++].SetDefaults(ItemID.EndurancePotion);
                    if (Main.hardMode)
                        shop.item[nextSlot++].SetDefaults(ItemID.LifeforcePotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.EndurancePotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.MagicPowerPotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.ManaRegenerationPotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.MiningPotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.NightOwlPotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.ObsidianSkinPotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.RagePotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.RecallPotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.RegenerationPotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.ShinePotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.SonarPotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.SpelunkerPotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.SummoningPotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.SwiftnessPotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.TeleportationPotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.ThornsPotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.TitanPotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.WarmthPotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.WaterWalkingPotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.WormholePotion);
                    shop.item[nextSlot++].SetDefaults(ItemID.WrathPotion);
                    break;
                case 6:
                    if (Main.hardMode)
                    {
                        shop.item[nextSlot++].SetDefaults(ItemID.CursedFlame);
                        shop.item[nextSlot++].SetDefaults(ItemID.Ichor);
                        shop.item[nextSlot++].SetDefaults(ItemID.SoulofLight);
                        shop.item[nextSlot++].SetDefaults(ItemID.SoulofNight);
                        shop.item[nextSlot++].SetDefaults(ItemID.SoulofFlight);
                    }
                    if (NPC.downedMechBoss1)
                        shop.item[nextSlot++].SetDefaults(ItemID.SoulofMight);
                    if (NPC.downedMechBoss2)
                        shop.item[nextSlot++].SetDefaults(ItemID.SoulofSight);
                    if (NPC.downedMechBoss3)
                        shop.item[nextSlot++].SetDefaults(ItemID.SoulofFright);
                    if (Main.hardMode)
                    {
                        shop.item[nextSlot++].SetDefaults(ItemID.LightShard);
                        shop.item[nextSlot++].SetDefaults(ItemID.DarkShard);
                    }
                    break;
                case 7:
                    shop.item[nextSlot++].SetDefaults(ItemID.CopperShortsword);
                    shop.item[nextSlot++].SetDefaults(ItemID.Starfury);
                    shop.item[nextSlot++].SetDefaults(ItemID.EnchantedSword);
                    shop.item[nextSlot++].SetDefaults(ItemID.BladeofGrass);
                    if (NPC.downedQueenBee)
                        shop.item[nextSlot++].SetDefaults(ItemID.BeeKeeper);
                    if (NPC.downedBoss3)
                        shop.item[nextSlot++].SetDefaults(ItemID.Muramasa);
                    if (NPC.downedPlantBoss)
                    {
                        shop.item[nextSlot++].SetDefaults(ItemID.Seedler);
                        shop.item[nextSlot++].SetDefaults(ItemID.TheHorsemansBlade);
                    }
                    if (NPC.downedGolemBoss)
                        shop.item[nextSlot++].SetDefaults(ItemID.InfluxWaver);
                    if (NPC.downedMoonlord)
                    {
                        shop.item[nextSlot++].SetDefaults(ItemID.Meowmere);
                        shop.item[nextSlot++].SetDefaults(ItemID.StarWrath);
                    }
                    if (NPC.downedPlantBoss)
                        shop.item[nextSlot++].SetDefaults(ItemID.BrokenHeroSword);
                    if (Main.hardMode)
                    {
                        shop.item[nextSlot++].SetDefaults(ItemID.ShadowFlameKnife);
                        shop.item[nextSlot++].SetDefaults(ItemID.YoyoBag);
                        shop.item[nextSlot++].SetDefaults(ItemID.BouncingShield);
                        shop.item[nextSlot++].SetDefaults(ItemID.BerserkerGlove);
                    }
                    if (NPC.downedPlantBoss)
                        shop.item[nextSlot++].SetDefaults(ItemID.TurtleShell);
                    if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
                        shop.item[nextSlot++].SetDefaults(ItemID.FireGauntlet);
                    if (NPC.downedGolemBoss)
                    {
                        shop.item[nextSlot++].SetDefaults(ItemID.BeetleHusk);
                        shop.item[nextSlot++].SetDefaults(ItemID.CelestialShell);
                    }
                    shop.item[nextSlot++].SetDefaults(ItemID.SharpeningStation);
                    if (NPC.downedAncientCultist)
                        shop.item[nextSlot++].SetDefaults(ItemID.FragmentSolar);
                    break;
                case 8:
                    if (NPC.downedBoss1 || NPC.downedBoss2 || NPC.downedBoss3 || NPC.downedQueenBee)
                        shop.item[nextSlot++].SetDefaults(ItemID.ZapinatorGray);
                    if (Main.hardMode)
                    {
                        shop.item[nextSlot++].SetDefaults(ItemID.ZapinatorOrange);
                        shop.item[nextSlot++].SetDefaults(ItemID.DaedalusStormbow);
                        shop.item[nextSlot++].SetDefaults(ItemID.Megashark);
                    }
                    if (NPC.downedMoonlord)
                    {
                        shop.item[nextSlot++].SetDefaults(ItemID.SDMG);
                        shop.item[nextSlot++].SetDefaults(ItemID.Celeb2);
                    }
                    if (Main.hardMode)
                    {
                        shop.item[nextSlot++].SetDefaults(ItemID.MoltenQuiver);
                        shop.item[nextSlot++].SetDefaults(ItemID.StalkersQuiver);
                    }
                    if (NPC.downedGolemBoss)
                    {
                        shop.item[nextSlot++].SetDefaults(ItemID.ReconScope);
                    }
                    if (Main.hardMode)
                    {
                        shop.item[nextSlot++].SetDefaults(ItemID.EndlessQuiver);
                        shop.item[nextSlot++].SetDefaults(ItemID.EndlessMusketPouch);
                    }
                    shop.item[nextSlot++].SetDefaults(ItemID.AmmoBox);
                    if (NPC.downedAncientCultist)
                        shop.item[nextSlot++].SetDefaults(ItemID.FragmentVortex);
                    break;
                case 9:
                    if (Main.hardMode)
                        shop.item[nextSlot++].SetDefaults(ItemID.SkyFracture);
                    if (NPC.downedMoonlord)
                    {
                        shop.item[nextSlot++].SetDefaults(ItemID.LastPrism);
                        shop.item[nextSlot++].SetDefaults(ItemID.LunarFlareBook);
                    }
                    shop.item[nextSlot++].SetDefaults(ItemID.ManaFlower);
                    shop.item[nextSlot++].SetDefaults(ItemID.CelestialMagnet);
                    shop.item[nextSlot++].SetDefaults(ItemID.MagicCuffs);
                    if (Main.hardMode)
                        shop.item[nextSlot++].SetDefaults(ItemID.CrystalBall);
                    if (NPC.downedPlantBoss)
                        shop.item[nextSlot++].SetDefaults(ItemID.Ectoplasm);
                    if (NPC.downedAncientCultist)
                        shop.item[nextSlot++].SetDefaults(ItemID.FragmentNebula);
                    break;
                case 10:
                    shop.item[nextSlot++].SetDefaults(ItemID.SlimeStaff);
                    if (Main.hardMode)
                        shop.item[nextSlot++].SetDefaults(ItemID.SanguineStaff);
                    if (NPC.downedEmpressOfLight)
                        shop.item[nextSlot++].SetDefaults(ItemID.EmpressBlade);
                    if (NPC.downedMoonlord)
                    {
                        shop.item[nextSlot++].SetDefaults(ItemID.MoonlordTurretStaff);
                        shop.item[nextSlot++].SetDefaults(ItemID.RainbowCrystalStaff);
                    }
                    shop.item[nextSlot++].SetDefaults(ItemID.PygmyNecklace);
                    if (NPC.downedPlantBoss)
                    {
                        shop.item[nextSlot++].SetDefaults(ItemID.NecromanticScroll);
                        shop.item[nextSlot++].SetDefaults(ItemID.PapyrusScarab);
                    }
                    if (NPC.downedBoss3)
                        shop.item[nextSlot++].SetDefaults(ItemID.BewitchingTable);
                    if (NPC.downedAncientCultist)
                        shop.item[nextSlot++].SetDefaults(ItemID.FragmentStardust);
                    break;
                default:
                    break;
            }
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
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    ai = 0;
                    NPC targetNpc = Main.npc[i];
                    if (targetNpc.CanBeChasedBy() && targetNpc.type != NPCID.TargetDummy)
                    {
                        float between = Vector2.Distance(targetNpc.Center, NPC.Center);
                        bool closest = Vector2.Distance(NPC.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, targetNpc.position, targetNpc.width, targetNpc.height);
                        // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                        // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
                        bool closeThroughWall = between < 100f;
                        if ((closest && inRange || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            targetCenter = targetNpc.Center;
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
                    var projectileSource = NPC.GetSource_FromAI();
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/melee_swing"), NPC.Center);
                    if ((targetCenter - NPC.Center).Y >= 0f)
                        NPC.velocity = new Vector2(25f * NPC.direction, 15f);
                    if ((targetCenter - NPC.Center).Y <= 0f)
                        NPC.velocity = new Vector2(25f * NPC.direction, -15f);
                    int projectile = Projectile.NewProjectile(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, Main.myPlayer, 0f, 0f);
                    KnifeProjectileNPC spawnedModProjectile = Main.projectile[projectile].ModProjectile as KnifeProjectileNPC;
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