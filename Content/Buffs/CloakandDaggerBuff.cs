using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Items;
using TF2.Content.Items.Spy;
using TF2.Content.Projectiles;
using TF2.Common;

namespace TF2.Content.Buffs
{
    public class CloakandDagger : ModBuff
    {
        public override string Texture => "TF2/Content/Buffs/Cloaked";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cloaked");
            Description.SetDefault("Invincible and invisible");
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<CloakandDaggerPlayer>().cloakandDaggerBuff = true;
    }

    public class BrokenCloak : ModBuff
    {
        public override string Texture => "TF2/Content/Buffs/JarateCooldown";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Broken Cloak");
            Description.SetDefault("You cannot cloak");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            TF2BuffBase.cooldownBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<TF2Player>().brokenCloak = true;
    }

    public class CloakandDaggerPlayer : ModPlayer
    {
        public bool cloakandDaggerBuff;
        public int cloakMeter = 600;
        public int cloakMeterMax = 600;
        public int timer;
        private bool playDecloakingSound;

        public override void OnRespawn(Player player) => cloakMeter = cloakMeterMax;

        public override void PostNurseHeal(NPC nurse, int health, bool removeDebuffs, int price) => cloakMeter = cloakMeterMax;

        public override void ResetEffects()
        {
            cloakandDaggerBuff = false;
            Player.opacityForCreditsRoll = 1f;
            cloakMeterMax = 600;
        }

        public override void PostUpdate()
        {
            if (Player.GetModPlayer<TF2Player>().lEtrangerEquipped)
                cloakMeterMax += 240;
            if (Player.GetModPlayer<TF2Player>().yourEternalRewardEquipped)
                cloakMeterMax -= 200;
            if (!Player.GetModPlayer<TF2Player>().initializedClass)
                cloakMeter = cloakMeterMax;
            if (playDecloakingSound && Player.GetModPlayer<TF2Player>().cloakandDaggerEquipped && cloakMeter <= 0)
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spy_cloak"), Player.Center);
                playDecloakingSound = false;
            }

            if (Player.GetModPlayer<TF2Player>().cloakandDaggerEquipped)
                timer++;
            else
                timer = 0;
            cloakMeter = Utils.Clamp(cloakMeter, 0, cloakMeterMax);
            if (!cloakandDaggerBuff && Player.GetModPlayer<TF2Player>().cloakandDaggerEquipped && timer >= 3)
            {
                cloakMeter++;
                timer = 0;
            }
            else if (Player.HasBuff(ModContent.BuffType<CloakandDagger>()))
            {
                int buffIndex = Player.FindBuffIndex(ModContent.BuffType<CloakandDagger>());
                if (Player.buffTime[buffIndex] > 0)
                {
                    if (!(Player.controlUp || Player.controlDown || Player.controlLeft || Player.controlRight || Player.controlJump))
                        cloakMeter++;
                    else if (Player.controlRight)
                        cloakMeter -= (int)(Player.velocity.X * 0.25f);
                    else if (Player.controlLeft)
                        cloakMeter -= (int)(Player.velocity.X * -0.25f);
                    else if (Player.controlDown)
                        cloakMeter -= (int)(Player.velocity.Y * 0.25f);
                    else if (Player.controlUp || Player.controlJump)
                        cloakMeter -= (int)(Player.velocity.Y * -0.25f);
                }
                Player.buffTime[buffIndex] = cloakMeter;
                Player.opacityForCreditsRoll = 0.5f;
                playDecloakingSound = true;
                timer = 0;
            }

            if (Player.GetModPlayer<TF2Player>().cloakandDaggerEquipped && Player.HeldItem.ModItem is YourEternalReward && cloakMeter == cloakMeterMax)
                Player.HeldItem.GetGlobalItem<TF2ItemBase>().allowBackstab = true;
            else if (Player.GetModPlayer<TF2Player>().cloakandDaggerEquipped && Player.HeldItem.ModItem is YourEternalReward)
                Player.HeldItem.GetGlobalItem<TF2ItemBase>().allowBackstab = false;
        }

        #region Cloak Drain On Attack
        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            if (cloakandDaggerBuff && Player.HasBuff<CloakandDagger>()) // Prevents index out of range exceptions
            {
                cloakMeter = 0;
                Player.ClearBuff(ModContent.BuffType<CloakandDagger>());
                Player.AddBuff(ModContent.BuffType<BrokenCloak>(), 600);
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spy_cloak"), Player.Center);
            }
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (proj.GetGlobalProjectile<TF2ProjectileBase>().spawnedFromNPC) return;
            if (cloakandDaggerBuff && Player.HasBuff<CloakandDagger>()) // Prevents index out of range exceptions
            {
                cloakMeter = 0;
                Player.ClearBuff(ModContent.BuffType<CloakandDagger>());
                Player.AddBuff(ModContent.BuffType<BrokenCloak>(), 600);
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spy_cloak"), Player.Center);
            }
            else if (Player.GetModPlayer<TF2Player>().lEtrangerEquipped && proj.GetGlobalProjectile<TF2ProjectileBase>().lEtrangerProjectile)
                cloakMeter += 90;
        }

        public override void ModifyHitPvp(Item item, Player target, ref int damage, ref bool crit)
        {
            if (cloakandDaggerBuff && Player.HasBuff<CloakandDagger>()) // Prevents index out of range exceptions
            {
                cloakMeter = 0;
                Player.ClearBuff(ModContent.BuffType<CloakandDagger>());
                Player.AddBuff(ModContent.BuffType<BrokenCloak>(), 600);
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spy_cloak"), Player.Center);
            }
        }

        public override void ModifyHitPvpWithProj(Projectile proj, Player target, ref int damage, ref bool crit)
        {
            if (proj.GetGlobalProjectile<TF2ProjectileBase>().spawnedFromNPC) return;
            if (cloakandDaggerBuff && Player.HasBuff<CloakandDagger>()) // Prevents index out of range exceptions
            {
                cloakMeter = 0;
                Player.ClearBuff(ModContent.BuffType<CloakandDagger>());
                Player.AddBuff(ModContent.BuffType<BrokenCloak>(), 600);
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spy_cloak"), Player.Center);
            }
            else if (Player.GetModPlayer<TF2Player>().lEtrangerEquipped && proj.GetGlobalProjectile<TF2ProjectileBase>().lEtrangerProjectile)
                cloakMeter += 90;
        }
        #endregion Cloak Drain On Attack

        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource, ref int cooldownCounter)
        {
            if (cloakandDaggerBuff && Player.HasBuff<Cloaked>() && Player.GetModPlayer<TF2Player>().cloakImmuneTime <= 0) // Prevents index out of range exceptions
            {
                cloakMeter -= 60;
                Player.GetModPlayer<TF2Player>().cloakImmuneTime += 30;
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/cloak_hit"), Player.Center);
            }               
            return !cloakandDaggerBuff;
        }
    }
}