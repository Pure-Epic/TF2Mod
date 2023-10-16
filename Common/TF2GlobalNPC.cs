using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Items.Accessories;
using TF2.Content.Items.Ammo;
using TF2.Content.Items.Consumables;
using TF2.Content.Items.Materials;
using TF2.Content.Projectiles.Medic;

namespace TF2.Common
{
    public class TF2GlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool building;
        public int originalDefense;

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults(NPC npc)
        {
            if (npc.friendly)
                npc.buffImmune[ModContent.BuffType<Content.Buffs.Ubercharge>()] = false; // This line doesn't work (I haven't checked yet)

            // Don't remove libraries!
            /*
            NPCID.Sets.DebuffImmunitySets.Remove(npc.type);
            NPCDebuffImmunityData debuffWhitelist = new NPCDebuffImmunityData
            {
                ImmuneToWhips = false
            };
            NPCID.Sets.DebuffImmunitySets.Add(npc.type, debuffWhitelist); // I can't change enemy immunities, sadly
            */

            // Do this instead!
            if (NPCID.Sets.DebuffImmunitySets.ContainsKey(npc.type))
            {
                if (NPCID.Sets.DebuffImmunitySets[npc.type] == null)
                    NPCID.Sets.DebuffImmunitySets[npc.type] = new NPCDebuffImmunityData();
                NPCID.Sets.DebuffImmunitySets[npc.type].ImmuneToWhips = false;
            }

            originalDefense = npc.defense;

            if (!ModLoader.TryGetMod("Gensokyo", out Mod gensokyo) || !ModLoader.TryGetMod("CalamityMod", out Mod calamity)) return;
            bool revengeance = (bool)calamity.Call("GetDifficultyActive", "revengeance");
            bool death = (bool)calamity.Call("GetDifficultyActive", "death");
            if ((npc.ModNPC?.Mod == gensokyo || npc.ModNPC?.Mod is TF2) && !npc.friendly)
            {
                if (revengeance && !death)
                    npc.damage = (int)(npc.damage * 1.25f);
                else if (death)
                    npc.damage = (int)(npc.damage * 2.5f);
            }
        }

        public override void AI(NPC npc)
        {
            if (npc.life > npc.lifeMax * 1.5f)
                npc.life = (int)(npc.lifeMax * 1.5f);

            if (ModLoader.TryGetMod("Gensokyo", out Mod gensokyo))
            {
                /*
                if (gensokyo.TryFind("UtsuhoReiuji", out ModNPC okuuNPC) && npc.type == okuuNPC.Type && !Main.player[npc.target].ZoneUnderworldHeight && !Main.player[npc.target].dead)
                {
                    // Moved to IL editing
                    // DO NOT modify modded bosses like this
                }
                */
            }
        }

        public override void SetupShop(int type, Chest shop, ref int nextSlot)
        {
            // May or may not be used
        }

        public override void OnKill(NPC npc)
        {
            /*
            if (npc.type == NPCID.WallofFlesh)
                NPC.SetEventFlagCleared(ref DownedWallOfFleshSystem.downedWallOfFlesh, -1);
            */
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            // ModifyGlobalLoot would NOT work due to the boolean below being needed.
            if (npc.friendly || NPCID.Sets.CountsAsCritter[npc.type] || npc.type == NPCID.TargetDummy || ModContent.GetInstance<TF2Config>().NoTF2Loot) return;

            if (ModContent.GetInstance<TF2Config>().Loot)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PrimaryAmmo>(), 1, 1, 10));
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SecondaryAmmo>(), 1, 1, 10));
            }
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Metal>(), 1, 1, 5));
            if (ModContent.GetInstance<TF2Config>().Loot)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SmallHealth>(), 1));
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MediumHealth>(), 10));
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LargeHealth>(), 100));
            }

            if (npc.boss)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Content.Items.Currencies.Australium>(), 4));
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Content.Items.Consumables.MannCoSupplyCrate>(), 1));
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ScrapMetal>(), 1, 1, 6));
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ReclaimedMetal>(), 3, 1, 2));
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RefinedMetal>(), 9));
            }

            if (!npc.boss && !ModContent.GetInstance<TF2Config>().Loot)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PrimaryAmmo>(), 1, 1, 10));
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Metal>(), 1, 1, 5));
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SecondaryAmmo>(), 1, 1, 10));
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SmallHealth>(), 1));
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MediumHealth>(), 10));
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LargeHealth>(), 100));
            }

            if (npc.type == NPCID.EyeofCthulhu)
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Rank1>(), 1));
            if (Array.IndexOf(new int[] { NPCID.EaterofWorldsBody, NPCID.EaterofWorldsHead, NPCID.EaterofWorldsTail }, npc.type) > -1)
            {
                LeadingConditionRule leadingConditionRule = new LeadingConditionRule(new Conditions.LegacyHack_IsABoss());
                leadingConditionRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Rank2>(), 1));
                npcLoot.Add(leadingConditionRule);

                LeadingConditionRule leadingConditionRule2 = new LeadingConditionRule(new Conditions.LegacyHack_IsABoss());
                leadingConditionRule2.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Content.Items.Currencies.Australium>(), 10));
                npcLoot.Add(leadingConditionRule2);
            }
            if (npc.type == NPCID.BrainofCthulhu)
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Rank2>(), 1));
            if (npc.type == NPCID.SkeletronHead)
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Rank3>(), 1));
            if (npc.type == NPCID.WallofFlesh)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Content.Items.Placeables.Crafting.CraftingAnvilItem>(), 1));

                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Rank4>(), 1));

                LeadingConditionRule leadingConditionRule = new LeadingConditionRule(new Conditions.IsHardmode());
                leadingConditionRule.OnFailedConditions(ItemDropRule.Common(ModContent.ItemType<Content.Items.Currencies.Australium>(), 1));
                npcLoot.Add(leadingConditionRule);
            }
            if (npc.type == NPCID.TheDestroyer)
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Rank5>(), 1));
            if (npc.type == NPCID.Spazmatism || npc.type == NPCID.Retinazer)
            {
                LeadingConditionRule leadingConditionRule = new LeadingConditionRule(new Conditions.MissingTwin());
                leadingConditionRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Rank5>(), 1));
                npcLoot.Add(leadingConditionRule);
            }
            if (npc.type == NPCID.SkeletronPrime)
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Rank5>(), 1));
            if (npc.type == NPCID.Plantera)
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Rank6>(), 1));
            if (npc.type == NPCID.Golem)
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Rank7>(), 1));
            if (npc.type == NPCID.MoonLordCore)
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Rank8>(), 1));
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamity))
            {
                if (calamity.TryFind("Providence", out ModNPC providenceNPC) && npc.type == providenceNPC.Type)
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Rank9>(), 1));
                if (calamity.TryFind("DevourerofGodsHead", out ModNPC theDevourerofGodsNPC) && npc.type == theDevourerofGodsNPC.Type)
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Rank10>(), 1));
                if (calamity.TryFind("Yharon", out ModNPC yharonNPC) && npc.type == yharonNPC.Type)
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Rank11>(), 1));
                if (calamity.TryFind("SupremeCalamitas", out ModNPC supremeCalamitasNPC) && npc.type == supremeCalamitasNPC.Type)
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Rank12>(), 1));
            }
            else
            {
                // In case the Gensokyo DLC gets removed from the mod, the rest of the mod can still compile successfully
                if (Mod.TryFind("ByakurenHijiri", out ModNPC byakuren) && npc.type == byakuren.Type)
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Rank9>(), 1));
            }
        }

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            if (item.ModItem?.Mod is TF2)
                npc.defense = item.ArmorPenetration; // Ignore defense
            else
                npc.defense = originalDefense;
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.ModProjectile?.Mod is TF2)
            {
                npc.defense = projectile.ArmorPenetration; // Ignore defense
                // Used for making some boss fights not extremely time-consuming
                if (npc.type == NPCID.TheDestroyer || npc.type == NPCID.TheDestroyerBody || npc.type == NPCID.TheDestroyerBody)
                    damage = (int)(2.5f * damage);
                if (ModLoader.TryGetMod("CalamityMod", out Mod calamity))
                {
                    if (npc.type == NPCID.Plantera)
                        damage = (int)(2.5f * damage);
                    if (calamity.TryFind("AstrumDeusHead", out ModNPC astrumDeusNPC) && npc.type == astrumDeusNPC.Type)
                        damage = (int)(3f * damage);
                    if (calamity.TryFind("AstrumDeusBody", out ModNPC astrumDeusNPC2) && npc.type == astrumDeusNPC2.Type)
                        damage = (int)(3f * damage);
                    if (calamity.TryFind("AstrumDeusTail", out ModNPC astrumDeusNPC3) && npc.type == astrumDeusNPC3.Type)
                        damage = (int)(3f * damage);
                    if (calamity.TryFind("StormWeaverHead", out ModNPC stormWeaverNPC) && npc.type == stormWeaverNPC.Type)
                        damage = (int)(2.5f * damage);
                    if (calamity.TryFind("StormWeaverBody", out ModNPC stormWeaverNPC2) && npc.type == stormWeaverNPC2.Type)
                        damage = (int)(1.5f * damage);
                    if (calamity.TryFind("StormWeaverTail", out ModNPC stormWeaverNPC3) && npc.type == stormWeaverNPC3.Type)
                        damage = (int)(5f * damage);
                    if (calamity.TryFind("DevourerofGodsHead", out ModNPC theDevourerofGodsNPC) && npc.type == theDevourerofGodsNPC.Type)
                        damage = (int)(5f * damage);
                    if (calamity.TryFind("DevourerofGodsBody", out ModNPC theDevourerofGodsNPC2) && npc.type == theDevourerofGodsNPC2.Type)
                        damage = (int)(5f * damage);
                    if (calamity.TryFind("DevourerofGodsTail", out ModNPC theDevourerofGodsNPC3) && npc.type == theDevourerofGodsNPC3.Type)
                        damage = (int)(5f * damage);
                }

                if (projectile.type == ModContent.ProjectileType<HealingBeam>())
                {
                    crit = false;
                    damage = 0;
                }
            }
            else
                npc.defense = originalDefense;
        }
    }
}