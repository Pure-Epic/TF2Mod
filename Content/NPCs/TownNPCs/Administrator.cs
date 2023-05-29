using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace TF2.Content.NPCs.TownNPCs
{
    // [AutoloadHead] and NPC.townNPC are extremely important and absolutely both necessary for any Town NPC to work at all.
    [AutoloadHead]
    public class Administrator : ModNPC
    {
        public int ai;

        public override void SetStaticDefaults()
        {
            // DisplayName automatically assigned from localization files, but the commented line below is the normal approach.
            DisplayName.SetDefault("Administrator");
            Main.npcFrameCount[Type] = 25; // The amount of frames the NPC has

            NPCID.Sets.ExtraFramesCount[Type] = 9; // Generally for Town NPCs, but this is how the NPC does extra things such as sitting in a chair and talking to other NPCs.
            NPCID.Sets.AttackFrameCount[Type] = 4;
            NPCID.Sets.DangerDetectRange[Type] = 700; // The amount of pixels away from the center of the npc that it tries to attack enemies.
            NPCID.Sets.AttackType[Type] = 0;
            NPCID.Sets.AttackTime[Type] = 60; // The amount of time it takes for the NPC's attack animation to be over once it starts.
            NPCID.Sets.AttackAverageChance[Type] = 100;
            NPCID.Sets.HatOffsetY[Type] = 5; // For when a party is active, the party hat spawns at a Y offset.

            // Influences how the NPC looks in the Bestiary
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new(0)
            {
                Velocity = 1f, // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
                Direction = 1, // -1 is left and 1 is right. NPCs are drawn facing the left by default but ExamplePerson will be drawn facing the right
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

            // Set Example Person's biome and neighbor preferences with the NPCHappiness hook. You can add happiness text and remarks with localization (See an example in ExampleMod/Localization/en-US.lang).
            // NOTE: The following code uses chaining - a style that works due to the fact that the SetXAffection methods return the same NPCHappiness instance they're called on.
            NPC.Happiness
                .SetBiomeAffection<ForestBiome>(AffectionLevel.Love)
                .SetNPCAffection(ModContent.NPCType<SaxtonHale>(), AffectionLevel.Like)
                .SetNPCAffection(ModContent.NPCType<MissPauling>(), AffectionLevel.Love)
                .SetNPCAffection(NPCID.Dryad, AffectionLevel.Hate)
            ; // < Mind the semicolon!
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true; // Sets NPC to be a Town NPC
            NPC.friendly = true; // NPC Will not attack player
            NPC.width = 35;
            NPC.height = 90;
            NPC.aiStyle = 7;
            NPC.damage = 0;
            NPC.defense = 50;
            NPC.lifeMax = 500;
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
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,

				// Sets your NPC's flavor text in the bestiary.
				new FlavorTextBestiaryInfoElement("Administrator is the CEO of TF Industries. She announces crucial events during the clash of mercenaries. She sides with no one, only being a double agent to help her gain an advantage."),
            });
        }

        public override void HitEffect(int hitDirection, double damage)
        {

        }

        public override bool CanTownNPCSpawn(int numTownNPCs, int money) => true;

        public override ITownNPCProfile TownNPCProfile() => new AdministratorProfile();

        public override List<string> SetNPCNameList() => new() { "Helen" };

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

            int missPauling = NPC.FindFirstNPC(ModContent.NPCType<MissPauling>());
            if (missPauling >= 0 && Main.rand.NextBool(5))
                chat.Add("In my experience, " + Main.npc[missPauling].GivenName + " nothing kills friendship faster... than a healthy competition.");
            // These are things that the NPC has a chance of telling you when you talk to it.
            chat.Add("Mission begins in " + Main.rand.Next(10, 60) + " seconds.", 10.0);
            chat.Add("Friendships are in direct contravention of mercenary conduct as delineated in your contracts, and on a personal note: I am very, very, disappointed with you.");
            chat.Add("Get fighting!");
            chat.Add("Prepare to compete in " + Main.rand.Next(10, 60) + " seconds.");
            chat.Add("Are you ready?");
            return chat; // chat is implicitly cast to a string.
        }

        public override void SetChatButtons(ref string button, ref string button2) => button = "Hire a mercenary";

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
                shop = true;
        }

        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.NPCSummoners.ScoutSummon>());
            shop.item[nextSlot].shopCustomPrice = Item.buyPrice(platinum: 1);
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.NPCSummoners.SoldierSummon>());
            shop.item[nextSlot].shopCustomPrice = Item.buyPrice(platinum: 2);
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.NPCSummoners.PyroSummon>());
            shop.item[nextSlot].shopCustomPrice = Item.buyPrice(platinum: 1, gold: 50);
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.NPCSummoners.DemomanSummon>());
            shop.item[nextSlot].shopCustomPrice = Item.buyPrice(platinum: 1, gold: 50);
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.NPCSummoners.HeavySummon>());
            shop.item[nextSlot].shopCustomPrice = Item.buyPrice(platinum: 4);
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.NPCSummoners.EngineerSummon>());
            shop.item[nextSlot].shopCustomPrice = Item.buyPrice(platinum: 1, gold: 25);
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.NPCSummoners.MedicSummon>());
            shop.item[nextSlot].shopCustomPrice = Item.buyPrice(platinum: 5);
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.NPCSummoners.SniperSummon>());
            shop.item[nextSlot].shopCustomPrice = Item.buyPrice(platinum: 2);
            nextSlot++;

            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.NPCSummoners.SpySummon>());
            shop.item[nextSlot].shopCustomPrice = Item.buyPrice(platinum: 1, gold: 25);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {

        }

        // Make this Town NPC teleport to the King and/or Queen statue when triggered.
        public override bool CanGoToStatue(bool toKingStatue) => false;

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 0;
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

        public override bool UsesPartyHat() => false;
    }

    public class AdministratorProfile : ITownNPCProfile
    {
        public int RollVariation() => 0;

        public string GetNameForVariant(NPC npc) => npc.getNewNPCName();

        public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc)
        {
            if (npc.IsABestiaryIconDummy && !npc.ForcePartyHatOn)
                return ModContent.Request<Texture2D>("TF2/Content/NPCs/TownNPCs/Administrator");

            if (npc.altTexture == 1)
                return ModContent.Request<Texture2D>("TF2/Content/NPCs/TownNPCs/Administrator");

            return ModContent.Request<Texture2D>("TF2/Content/NPCs/TownNPCs/Administrator");
        }

        public int GetHeadTextureIndex(NPC npc) => ModContent.GetModHeadSlot("TF2/Content/NPCs/TownNPCs/Administrator_Head");
    }
}