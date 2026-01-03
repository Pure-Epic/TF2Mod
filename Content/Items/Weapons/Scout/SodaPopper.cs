using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items.Materials;
using TF2.Content.Projectiles;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Scout
{
    public class SodaPopper : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Scout, Primary, Unique, Craft);
            SetWeaponSize(50, 16);
            SetGunUseStyle();
            SetWeaponDamage(damage: 6, projectile: ModContent.ProjectileType<Bullet>(), projectileCount: 10, shootAngle: 12f);
            SetWeaponAttackSpeed(0.3125);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/scatter_gun_shoot");
            SetWeaponAttackIntervals(maxAmmo: 2, maxReserve: 32, reloadTime: 1.1333, usesMagazine: true, reloadSoundPath: "TF2/Content/Sounds/SFX/Weapons/scatter_gun_double_tube_reload");
            SetWeaponPrice(weapon: 2, reclaimed: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
            AddNeutralAttribute(description);
        }

        protected override void WeaponActiveUpdate(Player player)
        {
            SodaPopperPlayer p = player.GetModPlayer<SodaPopperPlayer>();
            if (player.controlUseTile && p.sodaPopperEquipped && p.hype >= 350)
            {
                player.AddBuff(ModContent.BuffType<SodaPopperBuff>(), TF2.Time(10));
                TF2.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/disciplinary_action_power_up"), player.Center);
            }
            usesFocusShot = p.sodaPopperBuff;
        }

        protected override void WeaponPassiveUpdate(Player player) => player.GetModPlayer<SodaPopperPlayer>().sodaPopperEquipped = true;

        public override void AddRecipes() => CreateRecipe().AddIngredient<ForceANature>().AddIngredient<BonkAtomicPunch>().AddIngredient<ReclaimedMetal>().AddTile<AustraliumAnvil>().Register();
    }

    public class SodaPopperPlayer : ModPlayer
    {
        public bool sodaPopperEquipped;
        public int hype;
        public bool sodaPopperBuff;
        public int buffDuration;
        private bool playBuffSound;

        public override void ResetEffects()
        {
            sodaPopperEquipped = false;
            sodaPopperBuff = false;
        }

        public override void PostUpdate()
        {
            hype = Utils.Clamp(hype, 0, 350);
            if (!sodaPopperEquipped)
                hype = 0;
            if (sodaPopperBuff && Player.HasBuff<SodaPopperBuff>())
            {
                hype = 0;
                int buffIndex = Player.FindBuffIndex(ModContent.BuffType<SodaPopperBuff>());
                buffDuration = Player.buffTime[buffIndex];
                playBuffSound = true;
            }
            else if (playBuffSound)
            {
                TF2.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/disciplinary_action_power_down"), Player.Center);
                playBuffSound = false;
            }
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (!info.PvP) return;
            Player opponent = Main.player[info.DamageSource.SourcePlayerIndex];
            SodaPopperPlayer sodaPopper = opponent.GetModPlayer<SodaPopperPlayer>();
            if (sodaPopper.sodaPopperEquipped)
                sodaPopper.hype += TF2.Round(info.Damage / opponent.GetModPlayer<TF2Player>().damageMultiplier);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (sodaPopperEquipped && target.type != NPCID.TargetDummy)
                hype += TF2.Round(damageDone / Player.GetModPlayer<TF2Player>().damageMultiplier);
        }
    }
}