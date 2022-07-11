using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Items.Accessories;
using TF2.Items.Ammo;
using Terraria.GameContent.ItemDropRules;
using TF2.Projectiles.Demoman;
using TF2.Projectiles.Medic;
using Terraria.DataStructures;

namespace TF2
{
    class TF2GlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public bool building;

        public override void SetDefaults(NPC npc)
        {
            npc.buffImmune[ModContent.BuffType<Buffs.Ubercharge>()] = false;
        }

        public override void AI(NPC npc)
        {
            if (npc.life > npc.lifeMax * 1.5f)
            {
                npc.life = (int)(npc.lifeMax * 1.5f);
            }
        }

        public override void SetupShop(int type, Chest shop, ref int nextSlot)
        {

        }

        public override void OnKill(NPC npc)
        {
            if (npc.type == NPCID.WallofFlesh)
            {
                NPC.SetEventFlagCleared(ref DownedWallOfFleshSystem.downedWallOfFlesh, -1);
            }
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.friendly || NPCID.Sets.CountsAsCritter[npc.type] || npc.type == NPCID.TargetDummy) { return; }

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PrimaryAmmo>(), 1, 1, 10));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SecondaryAmmo>(), 1, 1, 10));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Metal>(), 1, 1, 5));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SmallHealth>(), 1));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MediumHealth>(), 10));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LargeHealth>(), 100));

            if (npc.boss)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Australium>(), 10));
            }

            if (npc.type == NPCID.EyeofCthulhu)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Rank1>(), 1));
            }
            if (System.Array.IndexOf(new int[] { NPCID.EaterofWorldsBody, NPCID.EaterofWorldsHead, NPCID.EaterofWorldsTail }, npc.type) > -1)
            {
                LeadingConditionRule leadingConditionRule = new(new Conditions.LegacyHack_IsABoss());
                leadingConditionRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Rank2>(), 1));
                npcLoot.Add(leadingConditionRule);

                LeadingConditionRule leadingConditionRule2 = new(new Conditions.LegacyHack_IsABoss());
                leadingConditionRule2.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Items.Australium>(), 10));
                npcLoot.Add(leadingConditionRule2);
            }
            if (npc.type == NPCID.BrainofCthulhu)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Rank2>(), 1));
            }
            if (npc.type == NPCID.SkeletronHead)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Rank3>(), 1));
            }
            if (npc.type == NPCID.WallofFlesh)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Rank4>(), 1));

                LeadingConditionRule leadingConditionRule = new LeadingConditionRule(new Conditions.IsHardmode());
                leadingConditionRule.OnFailedConditions(ItemDropRule.Common(ModContent.ItemType<Items.Australium>(), 1));
                npcLoot.Add(leadingConditionRule);
            }
            if (npc.type == NPCID.TheDestroyer)
            {
                if (!NPC.downedMechBoss1)
                {
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Rank5>(), 1));
                }
                else if (NPC.downedMechBoss2 && NPC.downedMechBoss3)
                {
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Rank6>(), 1));
                }
            }
            if (npc.type == NPCID.Spazmatism || npc.type == NPCID.Retinazer)
            {
                if (!NPC.downedMechBoss2)
                {
                    LeadingConditionRule leadingConditionRule = new LeadingConditionRule(new Conditions.MissingTwin());
                    leadingConditionRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Rank5>(), 1));
                    npcLoot.Add(leadingConditionRule);
                }
                else if (NPC.downedMechBoss1 && NPC.downedMechBoss3)
                {
                    LeadingConditionRule leadingConditionRule = new LeadingConditionRule(new Conditions.MissingTwin());
                    leadingConditionRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Rank6>(), 1));
                    npcLoot.Add(leadingConditionRule);
                }
            }
            if (npc.type == NPCID.SkeletronPrime)
            {
                if (!NPC.downedMechBoss3)
                {
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Rank5>(), 1));
                }
                else if (NPC.downedMechBoss1 && NPC.downedMechBoss2)
                {
                    npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Rank6>(), 1));
                }
            }
            if (npc.type == NPCID.Plantera)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Rank7>(), 1));
            }
            if (npc.type == NPCID.CultistBoss)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Rank8>(), 1));
            }
            if (npc.type == NPCID.MoonLordCore)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Rank9>(), 1));
            }
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (npc.type == NPCID.TheDestroyer || npc.type == NPCID.TheDestroyerBody || npc.type == NPCID.TheDestroyerBody)
            {
                if (projectile.ModProjectile?.Mod == Mod && projectile.type
                    != ModContent.ProjectileType<Stickybomb>())
                {
                    //damage = (int)(2f * damage);
                }
            }

            if (projectile.type == ModContent.ProjectileType<HealingBeam>())
            {
                crit = false;
                damage = 0;
                //npc.life += 1;
            }
        }
    }
}