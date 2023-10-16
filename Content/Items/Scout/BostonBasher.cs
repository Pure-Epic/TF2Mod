using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items.Sniper;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Scout
{
    public class BostonBasher : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Scout, Melee, Unique, Craft);
            SetSwingUseStyle();
            SetWeaponDamage(damage: 35);
            SetWeaponAttackSpeed(0.5);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/melee_swing");
            SetWeaponPrice(weapon: 3);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override void WeaponPassiveUpdate(Player player)
        {
            BostonBasherPlayer p = player.GetModPlayer<BostonBasherPlayer>();
            if (!p.resetHit && p.miss && player.ItemAnimationEndingOrEnded)
            {
                if (!player.immuneNoBlink)
                {
                    BleedingPlayer bleedPlayer = player.GetModPlayer<BleedingPlayer>();
                    bleedPlayer.damageMultiplier = Convert.ToInt32(Math.Ceiling(player.statLifeMax2 * 0.008f));
                    player.AddBuff(ModContent.BuffType<Bleeding>(), 300);
                }
                player.Hurt(PlayerDeathReason.ByCustomReason(player.name + " bashed their own skull open."), Convert.ToInt32(Math.Floor(player.statLifeMax2 * 0.144f)), 0);
                p.resetHit = true;
            }
        }

        protected override bool? WeaponOnUse(Player player)
        {
            BostonBasherPlayer p = player.GetModPlayer<BostonBasherPlayer>();
            p.miss = true;
            p.resetHit = false;
            Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<BostonBasherHitbox>(), (int)(Item.damage * player.GetModPlayer<TF2Player>().classMultiplier), 0f);
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Sandman>()
                .AddIngredient<TribalmansShiv>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }

    public class BostonBasherPlayer : ModPlayer
    {
        public bool miss;
        public bool resetHit;
    }

    public class BostonBasherHitbox : ModProjectile
    {
        public override string Texture => "TF2/Content/Items/Scout/BostonBasher";

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.alpha = 255;
            Projectile.timeLeft = 30;
        }

        public override void AI()
        {
            if (Projectile.owner == Main.myPlayer)
                Projectile.Center = Main.player[Projectile.owner].Center;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox) => hitbox = TF2.MeleeHitbox(Main.player[Projectile.owner]);

        public override bool CanHitPlayer(Player player) => Projectile.owner != Main.myPlayer;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            BostonBasherPlayer p = player.GetModPlayer<BostonBasherPlayer>();
            p.miss = false;
            p.resetHit = true;
            BleedingNPC npc = target.GetGlobalNPC<BleedingNPC>();
            npc.damageMultiplier = player.GetModPlayer<TF2Player>().classMultiplier;
            target.AddBuff(ModContent.BuffType<Bleeding>(), 300);
            target.immune[player.whoAmI] = player.itemAnimation;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (!info.PvP) return;
            BostonBasherPlayer p = Main.player[Projectile.owner].GetModPlayer<BostonBasherPlayer>();
            p.miss = false;
            p.resetHit = true;
            BleedingPlayer player = target.GetModPlayer<BleedingPlayer>();
            player.damageMultiplier = Main.player[Projectile.owner].GetModPlayer<TF2Player>().classMultiplier;
            target.AddBuff(ModContent.BuffType<Bleeding>(), 300);
        }
    }
}