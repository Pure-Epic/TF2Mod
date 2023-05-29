using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.NPCs;
using TF2.Content.Projectiles;

namespace TF2.Content.Items.Spy
{
    public class DeadRinger : TF2WeaponNoAmmo
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dead Ringer");
            Tooltip.SetDefault("Cloak Type: Feign Death.\n"
                             + "Leave a clone on taking damage and temporarily gain invisibility, speed and damage resistance.\n"
                             + "Hitting reduces cloak duration");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeSetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Thrust;
            Item.autoReuse = true;

            Item.value = Item.buyPrice(platinum: 1);
            Item.rare = ModContent.RarityType<UniqueRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);

            var line = new TooltipLine(Mod, "Positive Attributes",
                "+50% cloak regen rate\n"
                + "+40% cloak duration")
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line);

            var line2 = new TooltipLine(Mod, "Negative Attributes",
                "-50% cloak meter when Feign Death is activated")
            {
                OverrideColor = new Color(255, 64, 64)
            };
            tooltips.Add(line2);
        }

        public override bool CanUseItem(Player player) => false;

        public override void UpdateInventory(Player player)
        {
            for (int i = 0; i < 10; i++)
            {
                if (player.inventory[i].type == Type && !inHotbar)
                    inHotbar = true;
            }
            if (!inHotbar && !ModContent.GetInstance<TF2ConfigClient>().InventoryStats) return;
            TF2Player p = player.GetModPlayer<TF2Player>();
            p.deadRingerEquipped = true;
            inHotbar = false;
        }
    }

    public class FeignDeathPlayer : ModPlayer
    {
        public bool feignDeath;
        public float cloakMeter;
        public float cloakMeterMax = 840;
        public int timer;
        public int timer2;
        private bool playDecloakingSound;

        public override void OnRespawn(Player player) => cloakMeter = cloakMeterMax;

        public override void PostNurseHeal(NPC nurse, int health, bool removeDebuffs, int price) => cloakMeter = cloakMeterMax;

        public override void ResetEffects()
        {
            feignDeath = false;
            Player.opacityForCreditsRoll = 1f;
            cloakMeterMax = 840;
        }

        public override void PostUpdate()
        {
            if (Player.GetModPlayer<TF2Player>().lEtrangerEquipped)
                cloakMeterMax += 336;
            if (Player.GetModPlayer<TF2Player>().yourEternalRewardEquipped)
                cloakMeterMax -= 280;
            if (!Player.GetModPlayer<TF2Player>().initializedClass)
                cloakMeter = cloakMeterMax;
            if (playDecloakingSound && Player.GetModPlayer<TF2Player>().deadRingerEquipped && cloakMeter <= 0)
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spy_cloak"), Player.Center);
                playDecloakingSound = false;
            }

            if (Player.GetModPlayer<TF2Player>().deadRingerEquipped)
                timer++;
            else
                timer = 0;
            if (!feignDeath)
                timer2 = 0;
            cloakMeter = Utils.Clamp(cloakMeter, 0, cloakMeterMax);
            if (!feignDeath && Player.GetModPlayer<TF2Player>().deadRingerEquipped && timer >= 3)
            {
                cloakMeter += 2.1f;
                timer = 0;
            }
            else if (Player.HasBuff(ModContent.BuffType<FeignDeath>()))
            {
                timer2++;
                cloakMeter--;
                int buffIndex = Player.FindBuffIndex(ModContent.BuffType<FeignDeath>());
                Player.buffTime[buffIndex] = Convert.ToInt32(cloakMeter);
                Player.opacityForCreditsRoll = 0.5f;
                playDecloakingSound = true;
                timer = 0;
                if (timer2 <= 180)
                {
                    Player.GetModPlayer<TF2Player>().damageReduction += 0.65f;
                    Player.moveSpeed += 2.5f;
                    Player.GetModPlayer<TF2Player>().speedMultiplier += 1f;
                }
            }

            if (Player.GetModPlayer<TF2Player>().deadRingerEquipped && Player.HeldItem.ModItem is YourEternalReward && cloakMeter == cloakMeterMax)
                Player.HeldItem.GetGlobalItem<TF2ItemBase>().allowBackstab = true;
            else if (Player.GetModPlayer<TF2Player>().deadRingerEquipped && Player.HeldItem.ModItem is YourEternalReward)
                Player.HeldItem.GetGlobalItem<TF2ItemBase>().allowBackstab = false;
        }

        #region Cloak Drain On Attack
        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            if (feignDeath && Player.HasBuff<FeignDeath>()) // Prevents index out of range exceptions
                cloakMeter -= 150;
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (proj.GetGlobalProjectile<TF2ProjectileBase>().spawnedFromNPC) return;
            if (feignDeath && Player.HasBuff<FeignDeath>()) // Prevents index out of range exceptions
                cloakMeter -= 150;
            else if (Player.GetModPlayer<TF2Player>().lEtrangerEquipped && proj.GetGlobalProjectile<TF2ProjectileBase>().lEtrangerProjectile)
                cloakMeter += 176;
        }

        public override void ModifyHitPvp(Item item, Player target, ref int damage, ref bool crit)
        {
            if (feignDeath && Player.HasBuff<FeignDeath>()) // Prevents index out of range exceptions
                cloakMeter -= 150;
        }

        public override void ModifyHitPvpWithProj(Projectile proj, Player target, ref int damage, ref bool crit)
        {
            if (proj.GetGlobalProjectile<TF2ProjectileBase>().spawnedFromNPC) return;
            if (feignDeath && Player.HasBuff<FeignDeath>()) // Prevents index out of range exceptions
                cloakMeter -= 150;
            else if (Player.GetModPlayer<TF2Player>().lEtrangerEquipped && proj.GetGlobalProjectile<TF2ProjectileBase>().lEtrangerProjectile)
                cloakMeter += 176;
        }
        #endregion Cloak Drain On Attack

        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource, ref int cooldownCounter)
        {
            if (Player.HeldItem.ModItem is DeadRinger && Player.GetModPlayer<TF2Player>().deadRingerEquipped)
            {
                if (cloakMeter >= cloakMeterMax)
                    cloakMeter /= 2;
                else return true;
                damage -= (int)(damage * 0.75);
                Player.AddBuff(ModContent.BuffType<FeignDeath>(), 420);
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    var source = Player.GetSource_FromThis();
                    int npc = NPC.NewNPC(source, (int)Player.position.X, (int)Player.position.Y, ModContent.NPCType<SpyNPC>(), Player.whoAmI);
                    SpyNPC spawnedModNPC = Main.npc[npc].ModNPC as SpyNPC;
                    spawnedModNPC.npcOwner = Player.whoAmI;
                    NetMessage.SendData(MessageID.SyncNPC, number: npc);
                }
                else
                {
                    Player.stealth = 1000;
                    Player.stealthTimer = (int)cloakMeter;
                }
                for (int i = 0; i < Player.MaxBuffs; i++)
                {
                    int buffTypes = Player.buffType[i];
                    if (Main.debuff[buffTypes] && Player.buffTime[i] > 0 && !BuffID.Sets.NurseCannotRemoveDebuff[buffTypes] && !TF2BuffBase.cooldownBuff[buffTypes])
                    {
                        Player.DelBuff(i);
                        i = -1;
                    }
                }
            }

            if (feignDeath && Player.HasBuff<FeignDeath>() && Player.GetModPlayer<TF2Player>().cloakImmuneTime <= 0) // Prevents index out of range exceptions
            {
                cloakMeter -= 60;
                Player.GetModPlayer<TF2Player>().cloakImmuneTime += 30;
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/cloak_hit"), Player.Center);
            }
            return !feignDeath;
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (KeybindSystem.Cloak.JustPressed && Player.GetModPlayer<TF2Player>().deadRingerEquipped && Player.HasBuff<FeignDeath>())
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spy_cloak"), Player.Center);
                Player.ClearBuff(ModContent.BuffType<FeignDeath>());
            }
        }
    }
}