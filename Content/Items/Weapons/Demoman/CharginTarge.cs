using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Projectiles;
using TF2.Content.Projectiles.Demoman;

namespace TF2.Content.Items.Weapons.Demoman
{
    public class CharginTarge : TF2Accessory
    {
        protected override void WeaponStatistics() => SetWeaponCategory(Demoman, Secondary, Unique, Unlock);

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            List<string> currentShieldChargeKey = KeybindSystem.ShieldCharge.GetAssignedKeys(0);
            if (currentShieldChargeKey.Count <= 0 || currentShieldChargeKey.Contains("None"))
                AddNeutralAttribute(description);
            else
                AddOtherAttribute(description, currentShieldChargeKey[0] + (string)this.GetLocalization("Notes2"));
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            p.hasShield = true;
            p.shieldType = 1;

            CharginTargePlayer shield = player.GetModPlayer<CharginTargePlayer>();
            if (shield.chargeActive && !shield.chargeProjectileCreated)
            {
                Vector2 chargeDirection = player.DirectionTo(Main.MouseWorld);
                float speed = 25f;
                TF2Projectile projectile = TF2.CreateProjectile(null, player.GetSource_Accessory(Item), player.Center, chargeDirection * 2.5f, ModContent.ProjectileType<ShieldHitbox>(), TF2.Round((50 + player.GetModPlayer<EyelanderPlayer>().heads) * p.classMultiplier), 0f, player.whoAmI);
                if (p.miniCrit)
                    projectile.miniCrit = true;
                player.velocity = chargeDirection * speed;
                TF2.SetPlayerDirection(player);
                shield.chargeProjectileCreated = true;
                for (int i = 0; i < Player.MaxBuffs; i++)
                {
                    int buffTypes = player.buffType[i];
                    if (Main.debuff[buffTypes] && player.buffTime[i] > 0 && !BuffID.Sets.NurseCannotRemoveDebuff[buffTypes] && !TF2BuffBase.cooldownBuff[buffTypes])
                    {
                        player.DelBuff(i);
                        i = -1;
                    }
                }
            }
        }
    }

    public abstract class ShieldPlayer : ModPlayer
    {
        public int shieldRechargeTime = 720;
        public int chargeDuration = 90;
        public int timer;
        public bool activateGracePeriod;
        public bool chargeActive;
        public int chargeLeft;
        public bool chargeProjectileCreated;
        public int buffDelay;

        public override void PostUpdate()
        {
            SafePostUpdate();
            if (Player.GetModPlayer<TF2Player>().hasShield && !chargeActive)
                timer++;               
            else
                timer = 0;
            timer = Utils.Clamp(timer, 0, shieldRechargeTime);
            if (!activateGracePeriod) return;
            buffDelay++;
            if (buffDelay >= 30)
            {
                Player.ClearBuff(ModContent.BuffType<MeleeCrit>());
                buffDelay = 0;
            }
        }

        public virtual void SafePostUpdate()
        { }
    }

    public class CharginTargePlayer : ShieldPlayer
    {
        public override void SafePostUpdate() => shieldRechargeTime = 720;

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            if (Player.GetModPlayer<TF2Player>().hasShield && Player.GetModPlayer<TF2Player>().shieldType == 1)
                modifiers.FinalDamage *= 0.5f;
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (Player.GetModPlayer<TF2Player>().hasShield && Player.GetModPlayer<TF2Player>().shieldType == 1)
                modifiers.FinalDamage *= 0.7f;
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (KeybindSystem.ShieldCharge.JustPressed && timer >= shieldRechargeTime)
            {
                chargeActive = true;
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Voicelines/demoman_charge_windup1"), Player.Center);
            }
        }
    }
}