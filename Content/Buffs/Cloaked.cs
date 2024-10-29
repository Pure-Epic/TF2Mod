using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Weapons.Spy;
using TF2.Content.Projectiles;
using TF2.Content.Projectiles.Spy;

namespace TF2.Content.Buffs
{
    public class Cloaked : ModBuff
    {
        public override void SetStaticDefaults() => Main.buffNoSave[Type] = true;

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<CloakPlayer>().cloakBuff = true;
    }

    public class FeignDeath : ModBuff
    {
        public override string Texture => "TF2/Content/Buffs/Cloaked";

        public override void SetStaticDefaults() => Main.buffNoSave[Type] = true;

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<FeignDeathPlayer>().feignDeath = true;
    }

    public class CloakPlayer : ModPlayer
    {
        public bool invisWatchEquipped;
        public bool cloakBuff;
        public int cloakMeter = TF2.Time(10);
        public int cloakMeterMax = TF2.Time(10);
        public int timer;
        private bool playDecloakingSound;
        public bool fullCloak;

        public override void OnRespawn() => cloakMeter = cloakMeterMax;

        public override void PostNurseHeal(NPC nurse, int health, bool removeDebuffs, int price) => cloakMeter = cloakMeterMax;

        public override void ResetEffects()
        {
            invisWatchEquipped = false;
            cloakBuff = false;
            Player.opacityForAnimation = 1f;
            cloakMeterMax = TF2.Time(10);
        }

        public override void PostUpdate()
        {
            if (Player.GetModPlayer<LEtrangerPlayer>().lEtrangerEquipped)
                cloakMeterMax += TF2.Time(4);
            if (Player.GetModPlayer<YourEternalRewardPlayer>().yourEternalRewardEquipped)
                cloakMeterMax -= TF2.Time(3.333);
            if (!Player.GetModPlayer<TF2Player>().initializedClass)
                cloakMeter = cloakMeterMax;
            if (playDecloakingSound && invisWatchEquipped && cloakMeter <= 0)
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/spy_cloak"), Player.Center);
                playDecloakingSound = false;
            }
            fullCloak = cloakMeter == cloakMeterMax;
            if (invisWatchEquipped)
                timer++;
            else
            {
                Player.ClearBuff(ModContent.BuffType<Cloaked>());
                cloakMeter = 0;
                timer = 0;
            }
            if (!cloakBuff && invisWatchEquipped && timer >= 3)
            {
                cloakMeter++;
                cloakMeter = Utils.Clamp(cloakMeter, 0, cloakMeterMax);
                timer = 0;
            }
            else if (Player.HasBuff(ModContent.BuffType<Cloaked>()))
            {
                cloakMeter--;
                cloakMeter = Utils.Clamp(cloakMeter, 0, cloakMeterMax);
                int buffIndex = Player.FindBuffIndex(ModContent.BuffType<Cloaked>());
                Player.buffTime[buffIndex] = cloakMeter;
                Player.opacityForAnimation = 0.5f;
                playDecloakingSound = true;
                timer = 0;
            }
        }

        #region Cloak Drain On Attack
        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (cloakBuff && Player.HasBuff<Cloaked>())
                cloakMeter -= TF2.Time(2.5);
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (proj.ModProjectile is not TF2Projectile projectile || projectile.spawnedFromNPC) return;
            if (cloakBuff && Player.HasBuff<Cloaked>())
                cloakMeter -= TF2.Time(2.5);
            else if (Player.GetModPlayer<LEtrangerPlayer>().lEtrangerEquipped && (proj.ModProjectile as TF2Projectile).lEtrangerProjectile)
                cloakMeter += TF2.Time(2.1);
            else if (proj.ModProjectile is BigEarnerProjectile)
                cloakMeter += TF2.Round(cloakMeterMax / 3f);
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (!modifiers.PvP) return;
            Player opponent = Main.player[modifiers.DamageSource.SourcePlayerIndex];
            if (opponent.GetModPlayer<CloakPlayer>().cloakBuff && opponent.HasBuff<Cloaked>())
                opponent.GetModPlayer<CloakPlayer>().cloakMeter -= TF2.Time(2.5);
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (!hurtInfo.PvP) return;
            Player opponent = Main.player[proj.owner];
            if (opponent.GetModPlayer<CloakPlayer>().cloakBuff && opponent.HasBuff<Cloaked>())
                opponent.GetModPlayer<CloakPlayer>().cloakMeter -= TF2.Time(2.5);
            else if (opponent.GetModPlayer<LEtrangerPlayer>().lEtrangerEquipped && (proj.ModProjectile as TF2Projectile).lEtrangerProjectile)
                opponent.GetModPlayer<CloakPlayer>().cloakMeter += TF2.Time(2.1);
        }
        #endregion Cloak Drain On Attack

        public override bool FreeDodge(Player.HurtInfo info)
        {
            if (cloakBuff && Player.HasBuff<Cloaked>() && Player.GetModPlayer<TF2Player>().cloakImmuneTime <= 0)
            {
                cloakMeter -= TF2.Time(1);
                Player.GetModPlayer<TF2Player>().cloakImmuneTime += TF2.Time(0.5);
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/cloak_hit"), Player.Center);
            }
            return cloakBuff;
        }
    }
}