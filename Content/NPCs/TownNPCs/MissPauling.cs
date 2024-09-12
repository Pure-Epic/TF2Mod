using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using TF2.Content.Items.Consumables;
using TF2.Content.Projectiles;

namespace TF2.Content.NPCs.TownNPCs
{
    [ExtendsFromMod("Deprecated Code")]
    [AutoloadHead]
    public class MissPauling : ModNPC
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
            NPCID.Sets.HatOffsetY[Type] = 2; // For when a party is active, the party hat spawns at a Y offset.

            // Influences how the NPC looks in the Bestiary
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new()
            {
                Velocity = 1f, // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
                Direction = 1, // -1 is left and 1 is right. NPCs are drawn facing the left by default but ExamplePerson will be drawn facing the right
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

            // Set Example Person's biome and neighbor preferences with the NPCHappiness hook. You can add happiness text and remarks with localization (See an example in ExampleMod/Localization/en-US.lang).
            // NOTE: The following code uses chaining - a style that works due to the fact that the SetXAffection methods return the same NPCHappiness instance they're called on.
            NPC.Happiness
                .SetBiomeAffection<ForestBiome>(AffectionLevel.Love)
                .SetNPCAffection(ModContent.NPCType<Administrator>(), AffectionLevel.Love)
                .SetNPCAffection(NPCID.Dryad, AffectionLevel.Hate)
            ; // < Mind the semicolon!
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true; // Sets NPC to be a Town NPC
            NPC.friendly = true; // NPC Will not attack player
            NPC.width = 30;
            NPC.height = 85;
            NPC.aiStyle = 7;
            NPC.damage = 15;
            NPC.defense = 50;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;

            AnimationType = NPCID.Guide;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) =>
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange([
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,

				// Sets your NPC's flavor text in the bestiary.
				new FlavorTextBestiaryInfoElement("Miss Pauling is the assistant of the Administrator. She only got one day off per year, and spent it fighting alongside you!"),
            ]);

        public override void HitEffect(NPC.HitInfo hit)
        {
        }

        public override bool CanTownNPCSpawn(int numTownNPCs) => true;

        public override ITownNPCProfile TownNPCProfile() => new MissPaulingProfile();

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

            int administrator = NPC.FindFirstNPC(ModContent.NPCType<Administrator>());
            if (administrator >= 0 && Main.rand.NextBool(5))
                chat.Add("I barely get any breaks, but working with " + Main.npc[administrator].GivenName + " is totally worth it!.");
            // These are things that the NPC has a chance of telling you when you talk to it.
            chat.Add(Main.LocalPlayer.name + "? Pauling here.", 10.0);
            chat.Add("This contract's big. Don't pass this up.");
            chat.Add("I made some calls and got you one of the big contracts. I don't give these out everyday, so don't screw it up, all right?");
            chat.Add("Nice work!");
            chat.Add("I owe you one.");
            return chat; // chat is implicitly cast to a string.
        }

        public override void SetChatButtons(ref string button, ref string button2) => button = "Shop";

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
                shopName = "Shop";
        }

        public override void AddShops()
        {
            NPCShop npcShop = new NPCShop(Type, "Shop")
                .Add(new Item(ItemID.SlimeCrown) { shopCustomPrice = Item.buyPrice(silver: 25) })
                .Add(new Item(ItemID.SuspiciousLookingEye) { shopCustomPrice = Item.buyPrice(silver: 25) })
                .Add(new Item(ItemID.WormFood) { shopCustomPrice = Item.buyPrice(silver: 35) }, new Condition("DownedBoss1", () => NPC.downedBoss1))
                .Add(new Item(ItemID.BloodySpine) { shopCustomPrice = Item.buyPrice(silver: 35) }, new Condition("DownedBoss1", () => NPC.downedBoss1))
                .Add(new Item(ItemID.Abeemination) { shopCustomPrice = Item.buyPrice(silver: 40) }, new Condition("DownedBoss2", () => NPC.downedBoss2))
                .Add(new Item(ItemID.ClothierVoodooDoll) { shopCustomPrice = Item.buyPrice(silver: 50) }, new Condition("DownedBoss2", () => NPC.downedBoss2))
                .Add(new Item(ItemID.DeerThing) { shopCustomPrice = Item.buyPrice(silver: 75) }, new Condition("DownedBoss3", () => NPC.downedBoss3))
                .Add(new Item(ItemID.GuideVoodooDoll) { shopCustomPrice = Item.buyPrice(gold: 1) }, new Condition("DownedBoss3", () => NPC.downedBoss3))
                .Add(new Item(ItemID.QueenSlimeCrystal) { shopCustomPrice = Item.buyPrice(gold: 10) }, new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.MechanicalWorm) { shopCustomPrice = Item.buyPrice(gold: 25) }, new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.MechanicalEye) { shopCustomPrice = Item.buyPrice(gold: 25) }, new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ItemID.MechanicalSkull) { shopCustomPrice = Item.buyPrice(gold: 25) }, new Condition("HardMode", () => Main.hardMode))
                .Add(new Item(ModContent.ItemType<PlanteraItem>()) { shopCustomPrice = Item.buyPrice(gold: 30) }, new Condition("DownedMechBoss", () => NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3))
                .Add(new Item(ItemID.LihzahrdPowerCell) { shopCustomPrice = Item.buyPrice(gold: 40) }, new Condition("DownedPlantera", () => NPC.downedPlantBoss))
                .Add(new Item(ItemID.EmpressButterfly) { shopCustomPrice = Item.buyPrice(gold: 35) }, new Condition("DownedPlantera", () => NPC.downedPlantBoss))
                .Add(new Item(ItemID.TruffleWorm) { shopCustomPrice = Item.buyPrice(gold: 45) }, new Condition("DownedGolem", () => NPC.downedGolemBoss))
                .Add(new Item(ItemID.CelestialSigil) { shopCustomPrice = Item.buyPrice(gold: 50) }, new Condition("DownedLunaticCultist", () => NPC.downedAncientCultist));
            npcShop.Register();
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
        }

        // Make this Town NPC teleport to the King and/or Queen statue when triggered.
        public override bool CanGoToStatue(bool toKingStatue) => false;

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 15;
            knockback = 0f;
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
                        float distance = Vector2.Distance(targetNPC.Center, NPC.Center);
                        bool closest = Vector2.Distance(NPC.Center, targetCenter) > distance;
                        bool inRange = distance < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, targetNPC.position, targetNPC.width, targetNPC.height);
                        // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                        // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
                        bool closeThroughWall = distance < 100f;
                        if ((closest && inRange || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = distance;
                            targetCenter = targetNPC.Center;
                            foundTarget = true;
                        }
                    }
                }
                if (foundTarget)
                {
                    Vector2 shootVel = NPC.DirectionTo(targetCenter);
                    if ((targetCenter - NPC.Center).X > 0f)
                        NPC.spriteDirection = NPC.direction = 1;
                    else if ((targetCenter - NPC.Center).X < 0f)
                        NPC.spriteDirection = NPC.direction = -1;
                    float speed = 10f;
                    int type = ModContent.ProjectileType<Bullet>();
                    int damage = NPC.damage;
                    IEntitySource projectileSource = NPC.GetSource_FromAI();
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/pistol_shoot"), NPC.Center);
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        TF2.CreateProjectile(null, projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, Main.myPlayer, 0f, 0f);
                    else
                        NetMessage.SendData(MessageID.SyncProjectile, number: Projectile.NewProjectile(projectileSource, NPC.Center, shootVel * speed, type, damage, 0f, Main.myPlayer, 0f, 0f));
                }
            }
        }

        public override bool UsesPartyHat() => false;
    }

    public class MissPaulingProfile : ITownNPCProfile
    {
        public int RollVariation() => 0;

        public string GetNameForVariant(NPC npc) => npc.getNewNPCName();

        public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc)
        {
            if (npc.IsABestiaryIconDummy && !npc.ForcePartyHatOn)
                return ModContent.Request<Texture2D>("TF2/Content/NPCs/TownNPCs/MissPauling");

            if (npc.altTexture == 1)
                return ModContent.Request<Texture2D>("TF2/Content/NPCs/TownNPCs/MissPauling");

            return ModContent.Request<Texture2D>("TF2/Content/NPCs/TownNPCs/MissPauling");
        }

        public int GetHeadTextureIndex(NPC npc) => ModContent.GetModHeadSlot("TF2/Content/NPCs/TownNPCs/MissPauling_Head");
    }
}