using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Spy;
using TF2.Content.Projectiles;

namespace TF2.Content.Buffs
{
    public class CloakAndDaggerBuff : ModBuff
    {
        public override string Texture => "TF2/Content/Buffs/Cloaked";

        public override void SetStaticDefaults() => Main.buffNoSave[Type] = true;

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<CloakAndDaggerPlayer>().cloakAndDaggerBuff = true;
    }

    public class BrokenCloak : ModBuff
    {
        public override string Texture => "TF2/Content/Buffs/JarateCooldown";

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            TF2BuffBase.cooldownBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) => player.GetModPlayer<TF2Player>().brokenCloak = true;
    }

    public class CloakAndDaggerPlayer : ModPlayer
    {
        public bool cloakAndDaggerEquipped;
        public bool cloakAndDaggerBuff;
        public int cloakMeter = 600;
        public int cloakMeterMax = 600;
        public int timer;
        private bool playDecloakingSound;
        public bool fullCloak;

        public override void OnRespawn() => cloakMeter = cloakMeterMax;

        public override void PostNurseHeal(NPC nurse, int health, bool removeDebuffs, int price) => cloakMeter = cloakMeterMax;

        public override void ResetEffects()
        {
            cloakAndDaggerEquipped = false;
            cloakAndDaggerBuff = false;
            Player.opacityForAnimation = 1f;
            cloakMeterMax = 600;
        }

        public override void PostUpdate()
        {
            if (Player.GetModPlayer<LEtrangerPlayer>().lEtrangerEquipped)
                cloakMeterMax += 240;
            if (Player.GetModPlayer<YourEternalRewardPlayer>().yourEternalRewardEquipped)
                cloakMeterMax -= 200;
            if (!Player.GetModPlayer<TF2Player>().initializedClass)
                cloakMeter = cloakMeterMax;
            if (playDecloakingSound && cloakAndDaggerEquipped && cloakMeter <= 0)
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spy_cloak"), Player.Center);
                playDecloakingSound = false;
            }
            fullCloak = cloakMeter == cloakMeterMax;
            if (cloakAndDaggerEquipped)
                timer++;
            else
            {
                Player.ClearBuff(ModContent.BuffType<CloakAndDaggerBuff>());
                cloakMeter = 0;
                timer = 0;
            }

            if (!cloakAndDaggerBuff && cloakAndDaggerEquipped && timer >= 3)
            {
                cloakMeter++;
                cloakMeter = Utils.Clamp(cloakMeter, 0, cloakMeterMax);
                timer = 0;
            }
            else if (Player.HasBuff(ModContent.BuffType<CloakAndDaggerBuff>()))
            {
                int buffIndex = Player.FindBuffIndex(ModContent.BuffType<CloakAndDaggerBuff>());
                if (Player.buffTime[buffIndex] > 0)
                {
                    if (cloakAndDaggerEquipped)
                        timer++;
                    if (!(Player.controlUp || Player.controlDown || Player.controlLeft || Player.controlRight || Player.controlJump) && timer >= 3)
                    {
                        cloakMeter++;
                        timer = 0;
                    }
                    else if (Player.controlRight)
                        cloakMeter -= (int)(Player.velocity.X * 0.25f);
                    else if (Player.controlLeft)
                        cloakMeter -= (int)(Player.velocity.X * -0.25f);
                    else if (Player.controlDown)
                        cloakMeter -= (int)(Player.velocity.Y * 0.25f);
                    else if (Player.controlUp || Player.controlJump)
                        cloakMeter -= (int)(Player.velocity.Y * -0.25f);
                    cloakMeter = Utils.Clamp(cloakMeter, 0, cloakMeterMax);
                }
                Player.buffTime[buffIndex] = cloakMeter;
                Player.opacityForAnimation = 0.5f;
                playDecloakingSound = true;
            }
        }

        #region Cloak Drain On Attack

        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (cloakAndDaggerBuff && Player.HasBuff<CloakAndDaggerBuff>())
            {
                cloakMeter = 0;
                Player.ClearBuff(ModContent.BuffType<CloakAndDaggerBuff>());
                Player.AddBuff(ModContent.BuffType<BrokenCloak>(), 600);
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spy_cloak"), Player.Center);
            }
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (proj.GetGlobalProjectile<TF2ProjectileBase>().spawnedFromNPC) return;
            if (cloakAndDaggerBuff && Player.HasBuff<CloakAndDaggerBuff>())
            {
                cloakMeter = 0;
                Player.ClearBuff(ModContent.BuffType<CloakAndDaggerBuff>());
                Player.AddBuff(ModContent.BuffType<BrokenCloak>(), 600);
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spy_cloak"), Player.Center);
            }
            else if (Player.GetModPlayer<LEtrangerPlayer>().lEtrangerEquipped && proj.GetGlobalProjectile<TF2ProjectileBase>().lEtrangerProjectile)
                cloakMeter += 90;
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (!modifiers.PvP) return;
            Player opponent = Main.player[modifiers.DamageSource.SourcePlayerIndex];
            if (opponent.GetModPlayer<CloakAndDaggerPlayer>().cloakAndDaggerBuff && opponent.HasBuff<CloakAndDaggerBuff>())
            {
                opponent.GetModPlayer<CloakAndDaggerPlayer>().cloakMeter = 0;
                opponent.ClearBuff(ModContent.BuffType<CloakAndDaggerBuff>());
                opponent.AddBuff(ModContent.BuffType<BrokenCloak>(), 600);
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spy_cloak"), Player.Center);
            }
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (!hurtInfo.PvP) return;
            Player opponent = Main.player[proj.owner];
            if (opponent.GetModPlayer<CloakAndDaggerPlayer>().cloakAndDaggerBuff && opponent.HasBuff<CloakAndDaggerBuff>())
            {
                opponent.GetModPlayer<CloakAndDaggerPlayer>().cloakMeter = 0;
                opponent.ClearBuff(ModContent.BuffType<CloakAndDaggerBuff>());
                opponent.AddBuff(ModContent.BuffType<BrokenCloak>(), 600);
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spy_cloak"), Player.Center);
            }
            else if (opponent.GetModPlayer<LEtrangerPlayer>().lEtrangerEquipped && proj.GetGlobalProjectile<TF2ProjectileBase>().lEtrangerProjectile)
                opponent.GetModPlayer<CloakAndDaggerPlayer>().cloakMeter += 90;
        }

        #endregion Cloak Drain On Attack

        public override bool FreeDodge(Player.HurtInfo info)
        {
            if (cloakAndDaggerBuff && Player.HasBuff<Cloaked>() && Player.GetModPlayer<TF2Player>().cloakImmuneTime <= 0) // Prevents index out of range exceptions
            {
                cloakMeter -= 60;
                Player.GetModPlayer<TF2Player>().cloakImmuneTime += 30;
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/cloak_hit"), Player.Center);
            }
            return cloakAndDaggerBuff;
        }
    }
}