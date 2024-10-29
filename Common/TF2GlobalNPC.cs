using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Items;
using TF2.Content.Items.Consumables;
using TF2.Content.Items.Materials;
using TF2.Content.Items.Placeables.Crafting;
using TF2.Content.Items.Weapons;
using TF2.Content.NPCs.Buildings;
using TF2.Content.NPCs.Enemies;
using TF2.Content.Projectiles;

namespace TF2.Common
{
    public class TF2GlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool building;
        public bool miniCrit;
        public bool crit;
        public int pickupCooldown;
        public int overhealTimer;

        public override void SetStaticDefaults()
        { }

        public override void SetDefaults(NPC npc)
        {
            if (NPCID.Sets.ImmuneToAllBuffs[npc.type])
            {
                NPCID.Sets.ImmuneToAllBuffs[npc.type] = false;
                NPCID.Sets.ImmuneToRegularBuffs[npc.type] = true;
            }
            if (!TF2.gensokyoLoaded || !ModLoader.TryGetMod("CalamityMod", out Mod calamity)) return;
            bool revengeance = (bool)calamity.Call("GetDifficultyActive", "revengeance");
            bool death = (bool)calamity.Call("GetDifficultyActive", "death");
            if ((npc.ModNPC?.Mod == TF2.Gensokyo || npc.ModNPC?.Mod is TF2) && !npc.friendly)
            {
                if (revengeance && !death)
                    npc.damage = TF2.Round(npc.damage * 1.25f);
                else if (death)
                    npc.damage = TF2.Round(npc.damage * 2.5f);
            }
        }

        public override void AI(NPC npc)
        {
            if (pickupCooldown > 0)
                pickupCooldown--;
        }

        public override void OnKill(NPC npc)
        {
            if (!npc.friendly && !NPCID.Sets.CountsAsCritter[npc.type] && npc.type != NPCID.TargetDummy && !ModContent.GetInstance<TF2Config>().NoTF2Loot)
            {
                if (!ModContent.GetInstance<TF2Config>().Loot)
                {
                    if (Main.rand.Next(0, 10) == 0)
                        TF2.DropLoot(npc, ModContent.ItemType<LargeAmmoBox>());
                    else if (Main.rand.Next(0, 5) == 0)
                        TF2.DropLoot(npc, ModContent.ItemType<MediumAmmoBox>());
                    else
                        TF2.DropLoot(npc, ModContent.ItemType<SmallAmmoBox>());
                    if (Main.rand.Next(0, 10) == 0)
                        TF2.DropLoot(npc, ModContent.ItemType<LargeHealth>());
                    else if (Main.rand.Next(0, 5) == 0)
                        TF2.DropLoot(npc, ModContent.ItemType<MediumHealth>());
                    else
                        TF2.DropLoot(npc, ModContent.ItemType<SmallHealth>());
                }
                if (npc.boss)
                {
                    TF2.DropLoot(npc, ModContent.ItemType<MannCoSupplyCrate>());
                    TF2.DropLoot(npc, ModContent.ItemType<ScrapMetal>(), 1, 1, 6);
                    TF2.DropLoot(npc, ModContent.ItemType<ReclaimedMetal>(), 3, 1, 2);
                    TF2.DropLoot(npc, ModContent.ItemType<RefinedMetal>(), 9);
                }
            }
            if (npc.ModNPC is BLUMercenary || npc.boss || TF2.BasicEnemiesThatCanDropMoney(npc))
            {
                TF2.AddMoney(Main.player[npc.lastInteraction], 0.05f, npc.Center);
                if (Main.rand.Next(0, 100) == 0 && TF2.ScreamFortress)
                    TF2.DropLoot(npc, ModContent.ItemType<HauntedMetalScrap>());
            }
            if (npc.type == NPCID.EyeofCthulhu)
                TF2.CreateSoulItem(npc, 0.75f, 1f);
            if ((npc.type == NPCID.EaterofWorldsHead && EaterOfWorldsDrop() == 1 && NPC.CountNPCS(NPCID.EaterofWorldsBody) == 0) || npc.type == NPCID.BrainofCthulhu)
            {
                TF2.CreateSoulItem(npc, 1f, 1.5f);
                TF2.UpgradeDrill(npc, 65);
            }
            if (npc.type == NPCID.SkeletronHead)
            {
                TF2.CreateSoulItem(npc, 1.5f, 2f, 2);
                TF2.UpgradeDrill(npc, 100);
            }
            if (npc.type == NPCID.WallofFlesh)
            {
                TF2.CreateSoulItem(npc, 3.5f, 3f, 3);
                TF2.UpgradeDrill(npc, 180);
            }
            if (npc.type == NPCID.TheDestroyer || ((npc.type == NPCID.Spazmatism || npc.type == NPCID.Retinazer) && TwinsDrop(npc)) || npc.type == NPCID.SkeletronPrime)
            {
                TF2.CreateSoulItem(npc, 5f, 4f, 3);
                TF2.UpgradeDrill(npc, 200);
            }
            if (npc.type == NPCID.Plantera)
            {
                TF2.CreateSoulItem(npc, 7.5f, 4f, 4);
                TF2.UpgradeDrill(npc, 200);
            }
            if (npc.type == NPCID.Golem)
            {
                TF2.CreateSoulItem(npc, 10f, 5f, 4);
                TF2.UpgradeDrill(npc, 210);
            }
            if (npc.type == NPCID.CultistBoss)
                TF2.CreateSoulItem(npc, 15f, 5f, 4);
            if (npc.type == NPCID.MoonLordCore)
            {
                TF2.CreateSoulItem(npc, 25f, 7.5f, 5);
                TF2.UpgradeDrill(npc, 225);
            }
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamity))
            {
                if (calamity.TryFind("Providence", out ModNPC providenceNPC) && npc.type == providenceNPC.Type)
                {
                    TF2.CreateSoulItem(npc, 50f, 7.5f, 5);
                    TF2.UpgradeDrill(npc, 250);
                }
                if (calamity.TryFind("DevourerofGodsHead", out ModNPC theDevourerofGodsNPC) && npc.type == theDevourerofGodsNPC.Type)
                {
                    TF2.CreateSoulItem(npc, 100f, 10f, 5);
                    TF2.UpgradeDrill(npc, 250);
                }
                if (calamity.TryFind("Yharon", out ModNPC yharonNPC) && npc.type == yharonNPC.Type)
                {
                    TF2.CreateSoulItem(npc, 150f, 10f, 5);
                    TF2.UpgradeDrill(npc, 250);
                }
                if (calamity.TryFind("SupremeCalamitas", out ModNPC supremeCalamitasNPC) && npc.type == supremeCalamitasNPC.Type)
                {
                    TF2.CreateSoulItem(npc, 250f, 20f, 6);
                    TF2.UpgradeDrill(npc, 250);
                }
            }
            else
            {
                // In case Gensokyo DLC gets removed from the mod, the rest of the mod can still be compiled
                if (Mod.TryFind("ByakurenHijiri", out ModNPC byakuren) && npc.type == byakuren.Type)
                {
                    TF2.CreateSoulItem(npc, 50f, 7.5f, 5);
                    TF2.UpgradeDrill(npc, 250);
                }
            }
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == NPCID.WallofFlesh)
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AustraliumAnvilItem>(), 1));
        }

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (item.ModItem?.Mod is TF2)
                modifiers.DefenseEffectiveness *= 0;
            if (player.HeldItem.ModItem is TF2Weapon weapon)
            {
                if (weapon.IsWeaponType(TF2Item.Melee) && weapon.WeaponCriticalHits(player) && !weapon.noRandomCrits)
                    player.GetModPlayer<TF2Player>().crit = true;
            }
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (projectile.ModProjectile is TF2Projectile tf2Projectile)
            {
                if (tf2Projectile.weapon != null)
                {
                    TF2Weapon weapon = tf2Projectile.weapon;
                    if (weapon != null && weapon.IsWeaponType(TF2Item.Melee) && weapon.WeaponCriticalHits(Main.player[projectile.owner]) && !weapon.noRandomCrits)
                        tf2Projectile.crit = true;
                }
                modifiers.DefenseEffectiveness *= 0;
                modifiers.DamageVariationScale *= 0;
                // Used for making some boss fights not extremely time-consuming
                if (npc.type == NPCID.TheDestroyer || npc.type == NPCID.TheDestroyerBody || npc.type == NPCID.TheDestroyerBody)
                    modifiers.SourceDamage *= 2.5f;
                if (ModLoader.TryGetMod("CalamityMod", out Mod calamity))
                {
                    if (npc.type == NPCID.Plantera)
                        modifiers.SourceDamage *= 2.5f;
                    if (calamity.TryFind("AstrumDeusHead", out ModNPC astrumDeusNPC) && npc.type == astrumDeusNPC.Type)
                        modifiers.SourceDamage *= 3f;
                    if (calamity.TryFind("AstrumDeusBody", out ModNPC astrumDeusNPC2) && npc.type == astrumDeusNPC2.Type)
                        modifiers.SourceDamage *= 3f;
                    if (calamity.TryFind("AstrumDeusTail", out ModNPC astrumDeusNPC3) && npc.type == astrumDeusNPC3.Type)
                        modifiers.SourceDamage *= 3f;
                    if (calamity.TryFind("StormWeaverHead", out ModNPC stormWeaverNPC) && npc.type == stormWeaverNPC.Type)
                        modifiers.SourceDamage *= 2.5f;
                    if (calamity.TryFind("StormWeaverBody", out ModNPC stormWeaverNPC2) && npc.type == stormWeaverNPC2.Type)
                        modifiers.SourceDamage *= 1.5f;
                    if (calamity.TryFind("StormWeaverTail", out ModNPC stormWeaverNPC3) && npc.type == stormWeaverNPC3.Type)
                        modifiers.SourceDamage *= 5f;
                    if (calamity.TryFind("DevourerofGodsHead", out ModNPC theDevourerofGodsNPC) && npc.type == theDevourerofGodsNPC.Type)
                        modifiers.SourceDamage *= 5f;
                    if (calamity.TryFind("DevourerofGodsBody", out ModNPC theDevourerofGodsNPC2) && npc.type == theDevourerofGodsNPC2.Type)
                        modifiers.SourceDamage *= 5f;
                    if (calamity.TryFind("DevourerofGodsTail", out ModNPC theDevourerofGodsNPC3) && npc.type == theDevourerofGodsNPC3.Type)
                        modifiers.SourceDamage *= 5f;
                }
            }
        }

        public override void OnHitNPC(NPC npc, NPC target, NPC.HitInfo hit)
        {
            if (target.ModNPC is Building building && !building.Initialized)
            {
                building.preConstructedDamage = hit.Damage;
                target.netUpdate = true;
            }
        }

        private static int EaterOfWorldsDrop()
        {
            var amount = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.type == NPCID.EaterofWorldsHead && npc.active)
                    amount++;
            }
            return amount;
        }

        private static bool TwinsDrop(NPC npc)
        {
            int type = NPCID.Retinazer;
            if (npc.type == NPCID.Retinazer)
                type = NPCID.Spazmatism;
            return !NPC.AnyNPCs(type);
        }
    }
}