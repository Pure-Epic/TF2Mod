using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.GameContent.Personalities;
using Terraria.DataStructures;
using System.Collections.Generic;
using ReLogic.Content;
using TF2.Projectiles.NPCs;

namespace TF2.NPCs.TownNPCs
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
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
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

        public override bool CanTownNPCSpawn(int numTownNPCs, int money)
        {
            return true;
        }

        public override ITownNPCProfile TownNPCProfile()
        {
            return new SaxtonHaleProfile();
        }

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
            WeightedRandom<string> chat = new WeightedRandom<string>();

            int dryad = NPC.FindFirstNPC(NPCID.Dryad);
            if (dryad >= 0 && Main.rand.NextBool(10))
            {
                chat.Add("Get goofy ahh " + Main.npc[dryad].GivenName + " away from me! Also tell her to wear some clothes.");
            }
            // These are things that the NPC has a chance of telling you when you talk to it.
            chat.Add("Welcome to the Mann Co. Store! We sell products and get in fights!", 10.0);
            chat.Add("Browse, buy, design, sell and wear Mann Co.'s ever-growing catalog of fine products with your BARE HANDS--all without leaving the COMFORT OF YOUR CHAIRS!");
            chat.Add("If you aren't 100% satisfied with our product line, you can take it up with me!");
            chat.Add("I love fighting! But do you know what I don't love? Anime.");
            chat.Add("I beat up Touhou girls. I am wanted for my war crimes.", 0.1);
            return chat; // chat is implicitly cast to a string.
        }

        public override void SetChatButtons(ref string button, ref string button2)
        { // What the chat buttons are when you open up the chat UI
            button = "Mann Co. Store";
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
            {
                shop = true;
            }
        }

        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.MannCoSupplyCrateKey>());
            shop.item[nextSlot].shopCustomPrice = 1;
            shop.item[nextSlot].shopSpecialCurrency = TF2.Australium;
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Bundles.ScoutBundle>());
            shop.item[nextSlot].shopCustomPrice = 1;
            shop.item[nextSlot].shopSpecialCurrency = TF2.Australium;
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Bundles.SoldierBundle>());
            shop.item[nextSlot].shopCustomPrice = 1;
            shop.item[nextSlot].shopSpecialCurrency = TF2.Australium;
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Bundles.PyroBundle>());
            shop.item[nextSlot].shopCustomPrice = 1;
            shop.item[nextSlot].shopSpecialCurrency = TF2.Australium;
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Bundles.DemomanBundle>());
            shop.item[nextSlot].shopCustomPrice = 1;
            shop.item[nextSlot].shopSpecialCurrency = TF2.Australium;
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Bundles.HeavyBundle>());
            shop.item[nextSlot].shopCustomPrice = 1;
            shop.item[nextSlot].shopSpecialCurrency = TF2.Australium;
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Bundles.EngineerBundle>());
            shop.item[nextSlot].shopCustomPrice = 1;
            shop.item[nextSlot].shopSpecialCurrency = TF2.Australium;
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Bundles.MedicBundle>());
            shop.item[nextSlot].shopCustomPrice = 1;
            shop.item[nextSlot].shopSpecialCurrency = TF2.Australium;
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Bundles.SniperBundle>());
            shop.item[nextSlot].shopCustomPrice = 1;
            shop.item[nextSlot].shopSpecialCurrency = TF2.Australium;
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Bundles.SpyBundle>());
            shop.item[nextSlot].shopCustomPrice = 1;
            shop.item[nextSlot].shopSpecialCurrency = TF2.Australium;
            nextSlot++;

            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Items.Ammo.PrimaryAmmo>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Items.Ammo.SecondaryAmmo>());

            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Items.Ammo.SmallHealthPotion>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Items.Ammo.MediumHealthPotion>());
            shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Items.Ammo.LargeHealthPotion>());

            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Mounts.TFMountItem>());
            shop.item[nextSlot].shopCustomPrice = Item.buyPrice(gold: 1);
            nextSlot++;

            shop.item[nextSlot++].SetDefaults(ItemID.LifeCrystal);
            shop.item[nextSlot++].SetDefaults(ItemID.TerrasparkBoots);
            shop.item[nextSlot++].SetDefaults(ItemID.CellPhone);
            shop.item[nextSlot++].SetDefaults(ItemID.ManaFlower);

            if (Main.hardMode)
            {
                shop.item[nextSlot++].SetDefaults(ItemID.AnkhShield);
                shop.item[nextSlot++].SetDefaults(ItemID.DiscountCard);
                shop.item[nextSlot++].SetDefaults(ItemID.RodofDiscord);
            }
            if (NPC.downedMechBossAny)
            {
                shop.item[nextSlot++].SetDefaults(ItemID.LifeFruit);
            }
            if (NPC.downedPlantBoss)
            {
                shop.item[nextSlot++].SetDefaults(ItemID.LihzahrdAltar);
            }
            if (NPC.downedEmpressOfLight)
            {
                shop.item[nextSlot++].SetDefaults(ItemID.EmpressBlade);
            }
            if (NPC.downedGolemBoss)
            {
                shop.item[nextSlot++].SetDefaults(ItemID.DestroyerEmblem);
                shop.item[nextSlot++].SetDefaults(ItemID.ReconScope);
            }
            if (NPC.downedMoonlord)
            {
                shop.item[nextSlot++].SetDefaults(ItemID.Meowmere);
                shop.item[nextSlot++].SetDefaults(ItemID.StarWrath);
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
                    {
                        shootVel = new Vector2(0f, 1f);
                    }
                    if ((targetCenter - NPC.Center).X > 0f)
                    {
                        NPC.spriteDirection = NPC.direction = 1;
                    }
                    else if ((targetCenter - NPC.Center).X < 0f)
                    {
                        NPC.spriteDirection = NPC.direction = -1;
                    }
                    float speed = 10f;
                    int type = ModContent.ProjectileType<KnifeProjectileNPC>();
                    int damage = NPC.damage;
                    var projectileSource = NPC.GetSource_FromAI();
                    SoundEngine.PlaySound(new SoundStyle("TF2/Sounds/SFX/melee_swing"), NPC.Center);
                    if ((targetCenter - NPC.Center).Y >= 0f)
                    {
                        NPC.velocity = new Vector2(25f * NPC.direction, 15f);
                    }
                    if ((targetCenter - NPC.Center).Y <= 0f)
                    {
                        NPC.velocity = new Vector2(25f * NPC.direction, -15f);
                    }
                    int projectile = Projectile.NewProjectile(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, Main.myPlayer, 0f, 0f);
                    KnifeProjectileNPC spawnedModProjectile = Main.projectile[projectile].ModProjectile as KnifeProjectileNPC;
                    spawnedModProjectile.owner = NPC;
                    NetMessage.SendData(MessageID.SyncProjectile, number: projectile);
                }
            }
        }
    }

    public class SaxtonHaleProfile : ITownNPCProfile
    {
        public int RollVariation() => 0;
        public string GetNameForVariant(NPC npc) => npc.getNewNPCName();

        public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc)
        {
            if (npc.IsABestiaryIconDummy && !npc.ForcePartyHatOn)
                return ModContent.Request<Texture2D>("TF2/NPCs/TownNPCs/SaxtonHale");

            if (npc.altTexture == 1)
                return ModContent.Request<Texture2D>("TF2/NPCs/TownNPCs/SaxtonHale");

            return ModContent.Request<Texture2D>("TF2/NPCs/TownNPCs/SaxtonHale");
        }

        public int GetHeadTextureIndex(NPC npc) => ModContent.GetModHeadSlot("TF2/NPCs/TownNPCs/SaxtonHale_Head");
    }
}