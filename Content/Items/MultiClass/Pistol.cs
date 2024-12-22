using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Ammo;
using TF2.Content.Projectiles;

namespace TF2.Content.Items.MultiClass
{
    public class Pistol_Scout : TF2Weapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pistol");
            Tooltip.SetDefault("Scout's Starter Secondary");
        }

        public override void SafeSetDefaults()
        {
            Item.width = 30;
            Item.height = 23;
            Item.useTime = 9;
            Item.useAnimation = 9;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/pistol_shoot");
            Item.autoReuse = true;

            Item.damage = 15;
            Item.shoot = ModContent.ProjectileType<Bullet>();
            Item.shootSpeed = 10f;
            Item.useAmmo = ModContent.ItemType<SecondaryAmmo>();

            ammoCost = 1;
            maxAmmoClip = 12;
            ammoInClip = 12;
            reloadRate = 60f;
            magazine = true;
            reloadSound = new SoundStyle("TF2/Content/Sounds/SFX/pistol_reload");

            Item.rare = ModContent.RarityType<NormalRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);
        }

        public override void HoldItem(Player player)
        {
            WeaponSystem clip = player.GetModPlayer<WeaponSystem>();
            clip.ammoMax = maxAmmoClip;
            clip.ammoReloadRate = reloadRate;
            clip.ammoCurrent = ammoInClip;
            if (clip.startReload)
            {
                reload = true;
                clip.startReload = false;
            }
            UpdateResource();
            spreadRecovery++;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) => ammoInClip > 0;

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (p.classAccessory && !p.classHideVanity)
                Item.noUseGraphic = true;
            else
                Item.noUseGraphic = false;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (ammoInClip == 0 || reload) return false;
            ammoInClip -= ammoCost;

            Vector2 newVelocity;
            if (spreadRecovery >= 75)
                newVelocity = velocity;
            else
                newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(2.5f));
            var i = Projectile.NewProjectile(source, position, newVelocity, ModContent.ProjectileType<Bullet>(), damage, knockback, player.whoAmI);
            if (player.GetModPlayer<TF2Player>().focus)
            {
                Main.projectile[i].GetGlobalProjectile<TF2ProjectileBase>().homing = true;
                Main.projectile[i].GetGlobalProjectile<TF2ProjectileBase>().shootSpeed = Item.shootSpeed;
                NetMessage.SendData(MessageID.SyncProjectile, number: i);
            }
            spreadRecovery = 0;

            if (ammoInClip <= 0)
                reload = true;
            return false;
        }
        public override Vector2? HoldoutOffset() => new Vector2(0, 0);

        public override bool CanUseItem(Player player) => !(ammoInClip <= 0 || reload);
    }

    public class Pistol_Engineer : TF2Weapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pistol");
            Tooltip.SetDefault("Engineer's Starter Secondary");
        }

        public override void SafeSetDefaults()
        {
            Item.width = 30;
            Item.height = 23;
            Item.useTime = 9;
            Item.useAnimation = 9;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/pistol_shoot");
            Item.autoReuse = true;

            Item.damage = 15;
            Item.shoot = ModContent.ProjectileType<Bullet>();
            Item.shootSpeed = 10f;
            Item.useAmmo = ModContent.ItemType<SecondaryAmmo>();

            ammoCost = 1;
            maxAmmoClip = 12;
            ammoInClip = 12;
            reloadRate = 60f;
            magazine = true;
            reloadSound = new SoundStyle("TF2/Content/Sounds/SFX/pistol_reload");

            Item.rare = ModContent.RarityType<NormalRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);
        }

        public override void HoldItem(Player player)
        {
            WeaponSystem clip = player.GetModPlayer<WeaponSystem>();
            clip.ammoMax = maxAmmoClip;
            clip.ammoReloadRate = reloadRate;
            clip.ammoCurrent = ammoInClip;
            if (clip.startReload)
            {
                reload = true;
                clip.startReload = false;
            }
            UpdateResource();
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) => ammoInClip > 0;

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (p.classAccessory && !p.classHideVanity)
                Item.noUseGraphic = true;
            else
                Item.noUseGraphic = false;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (ammoInClip == 0 || reload) return false;
            ammoInClip -= ammoCost;

            Vector2 newVelocity;
            if (spreadRecovery >= 75)
                newVelocity = velocity;
            else
                newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(2.5f));
            var i = Projectile.NewProjectile(source, position, newVelocity, ModContent.ProjectileType<Bullet>(), damage, knockback, player.whoAmI);
            if (player.GetModPlayer<TF2Player>().focus)
            {
                Main.projectile[i].GetGlobalProjectile<TF2ProjectileBase>().homing = true;
                Main.projectile[i].GetGlobalProjectile<TF2ProjectileBase>().shootSpeed = Item.shootSpeed;
                NetMessage.SendData(MessageID.SyncProjectile, number: i);
            }

            if (ammoInClip <= 0)
                reload = true;
            return false;
        }
        public override Vector2? HoldoutOffset() => new Vector2(0, 0);

        public override bool CanUseItem(Player player) => !(ammoInClip <= 0 || reload);
    }
}