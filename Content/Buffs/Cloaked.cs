using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items;
using TF2.Content.Items.Spy;
using TF2.Content.Projectiles;

namespace TF2.Content.Buffs
{
    public class Cloaked : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cloaked");
            Description.SetDefault("Invincible and invisible");
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<CloakPlayer>().cloakBuff = true;
    }

    public class FeignDeath : ModBuff
    {
        public override string Texture => "TF2/Content/Buffs/Cloaked";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cloaked");
            Description.SetDefault("Invincible and invisible");
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<FeignDeathPlayer>().feignDeath = true;
    }

    public class CloakPlayer : ModPlayer
    {
        public bool cloakBuff;
        public int cloakMeter;
        public int cloakMeterMax = 600;
        public int timer;
        private bool playDecloakingSound;

        public override void OnRespawn(Player player) => cloakMeter = cloakMeterMax;

        public override void PostNurseHeal(NPC nurse, int health, bool removeDebuffs, int price) => cloakMeter = cloakMeterMax;

        public override void ResetEffects()
        {       
            cloakBuff = false;
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
            if (playDecloakingSound && Player.GetModPlayer<TF2Player>().invisWatchEquipped && cloakMeter <= 0)
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spy_cloak"), Player.Center);
                playDecloakingSound = false;
            }

            if (Player.GetModPlayer<TF2Player>().invisWatchEquipped)
                timer++;
            else
                timer = 0;
            cloakMeter = Utils.Clamp(cloakMeter, 0, cloakMeterMax);
            if (!cloakBuff && Player.GetModPlayer<TF2Player>().invisWatchEquipped && timer >= 3)
            {
                cloakMeter++;
                timer = 0;
            }
            else if (Player.HasBuff(ModContent.BuffType<Cloaked>()))
            {
                cloakMeter--;
                int buffIndex = Player.FindBuffIndex(ModContent.BuffType<Cloaked>());
                Player.buffTime[buffIndex] = cloakMeter;
                Player.opacityForCreditsRoll = 0.5f;
                playDecloakingSound = true;
                timer = 0;
            }

            if (Player.GetModPlayer<TF2Player>().invisWatchEquipped && Player.HeldItem.ModItem is YourEternalReward && cloakMeter == cloakMeterMax)
                Player.HeldItem.GetGlobalItem<TF2ItemBase>().allowBackstab = true;
            else if (Player.GetModPlayer<TF2Player>().invisWatchEquipped && Player.HeldItem.ModItem is YourEternalReward)
                Player.HeldItem.GetGlobalItem<TF2ItemBase>().allowBackstab = false;
        }

        #region Cloak Drain On Attack
        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            if (cloakBuff && Player.HasBuff<Cloaked>()) // Prevents index out of range exceptions
                cloakMeter -= 150;
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (proj.GetGlobalProjectile<TF2ProjectileBase>().spawnedFromNPC) return;
            if (cloakBuff && Player.HasBuff<Cloaked>()) // Prevents index out of range exceptions
                cloakMeter -= 150;
            else if (Player.GetModPlayer<TF2Player>().lEtrangerEquipped && proj.GetGlobalProjectile<TF2ProjectileBase>().lEtrangerProjectile)
                cloakMeter += 126;
        }

        public override void ModifyHitPvp(Item item, Player target, ref int damage, ref bool crit)
        {
            if (cloakBuff && Player.HasBuff<Cloaked>()) // Prevents index out of range exceptions
                cloakMeter -= 150;
        }

        public override void ModifyHitPvpWithProj(Projectile proj, Player target, ref int damage, ref bool crit)
        {
            if (proj.GetGlobalProjectile<TF2ProjectileBase>().spawnedFromNPC) return;
            if (cloakBuff && Player.HasBuff<Cloaked>()) // Prevents index out of range exceptions
                cloakMeter -= 150;
            else if (Player.GetModPlayer<TF2Player>().lEtrangerEquipped && proj.GetGlobalProjectile<TF2ProjectileBase>().lEtrangerProjectile)
                cloakMeter += 126;
        }
        #endregion Cloak Drain On Attack

        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource, ref int cooldownCounter)
        {
            if (cloakBuff && Player.HasBuff<Cloaked>() && Player.GetModPlayer<TF2Player>().cloakImmuneTime <= 0) // Prevents index out of range exceptions
            {
                cloakMeter -= 60;
                Player.GetModPlayer<TF2Player>().cloakImmuneTime += 30;
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/cloak_hit"), Player.Center);
            }
            return !cloakBuff;
        }
    }
}