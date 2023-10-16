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
    public class Shotgun_Soldier : TF2Weapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shotgun");
            Tooltip.SetDefault("Soldier's Starter Secondary");
        }

        public override void SafeSetDefaults()
        {
            Item.width = 50;
            Item.height = 12;
            Item.useTime = 38;
            Item.useAnimation = 38;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/shotgun_shoot");
            Item.autoReuse = true;

            Item.damage = 6;
            Item.shoot = ModContent.ProjectileType<Bullet>();
            Item.shootSpeed = 10f;
            Item.useAmmo = ModContent.ItemType<SecondaryAmmo>();

            ammoCost = 1;
            maxAmmoClip = 6;
            ammoInClip = 6;
            reloadRate = 31f;
            initialReloadRate = 60f;
            reloadSound = new SoundStyle("TF2/Content/Sounds/SFX/shotgun_reload");

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
            clip.initialAmmoReloadRate = initialReloadRate;
            clip.ammoCurrent = ammoInClip;
            if (clip.startReload)
            {
                reload = true;
                clip.startReload = false;
            }
            UpdateResource();
        }

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
            reload = false;
            ammoInClip -= ammoCost;

            for (int i = 0; i < 10; i++)
            {
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(12f));
                Projectile.NewProjectile(source, position, newVelocity, ModContent.ProjectileType<Bullet>(), damage, knockback, player.whoAmI);
            }

            if (ammoInClip <= 0)
                reload = true;
            return false;
        }
    }

    public class Shotgun_Pyro : TF2Weapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shotgun");
            Tooltip.SetDefault("Pyro's Starter Secondary");
        }

        public override void SafeSetDefaults()
        {
            Item.width = 50;
            Item.height = 12;
            Item.useTime = 38;
            Item.useAnimation = 38;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/shotgun_shoot");
            Item.autoReuse = true;

            Item.damage = 6;
            Item.shoot = ModContent.ProjectileType<Bullet>();
            Item.shootSpeed = 10f;
            Item.useAmmo = ModContent.ItemType<SecondaryAmmo>();

            ammoCost = 1;
            maxAmmoClip = 6;
            ammoInClip = 6;
            reloadRate = 31f;
            initialReloadRate = 62f;
            reloadSound = new SoundStyle("TF2/Content/Sounds/SFX/shotgun_reload");

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
            clip.initialAmmoReloadRate = initialReloadRate;
            clip.ammoCurrent = ammoInClip;
            if (clip.startReload)
            {
                reload = true;
                clip.startReload = false;
            }
            UpdateResource();
        }

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
            reload = false;
            ammoInClip -= ammoCost;

            for (int i = 0; i < 10; i++)

            {
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(12f));
                Projectile.NewProjectile(source, position, newVelocity, ModContent.ProjectileType<Bullet>(), damage, knockback, player.whoAmI);
            }

            if (ammoInClip <= 0)
                reload = true;
            return false;
        }
    }

    public class Shotgun_Heavy : TF2Weapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shotgun");
            Tooltip.SetDefault("Heavy's Starter Secondary");
        }

        public override void SafeSetDefaults()
        {
            Item.width = 50;
            Item.height = 12;
            Item.useTime = 38;
            Item.useAnimation = 38;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/shotgun_shoot");
            Item.autoReuse = true;

            Item.damage = 6;
            Item.shoot = ModContent.ProjectileType<Bullet>();
            Item.shootSpeed = 10f;
            Item.useAmmo = ModContent.ItemType<SecondaryAmmo>();

            ammoCost = 1;
            maxAmmoClip = 6;
            ammoInClip = 6;
            reloadRate = 31f;
            initialReloadRate = 52f;
            reloadSound = new SoundStyle("TF2/Content/Sounds/SFX/shotgun_reload");

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
            clip.initialAmmoReloadRate = initialReloadRate;
            clip.ammoCurrent = ammoInClip;
            if (clip.startReload)
            {
                reload = true;
                clip.startReload = false;
            }
            UpdateResource();
        }

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
            reload = false;
            ammoInClip -= ammoCost;

            for (int i = 0; i < 10; i++)

            {
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(12f));
                Projectile.NewProjectile(source, position, newVelocity, ModContent.ProjectileType<Bullet>(), damage, knockback, player.whoAmI);
            }

            if (ammoInClip <= 0)
                reload = true;
            return false;
        }
    }

    public class Shotgun_Engineer : TF2Weapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shotgun");
            Tooltip.SetDefault("Engineer's Starter Primary");
        }

        public override void SafeSetDefaults()
        {
            Item.width = 50;
            Item.height = 12;
            Item.useTime = 38;
            Item.useAnimation = 38;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/shotgun_shoot");
            Item.autoReuse = true;

            Item.damage = 6;
            Item.shoot = ModContent.ProjectileType<Bullet>();
            Item.shootSpeed = 10f;
            Item.useAmmo = ModContent.ItemType<PrimaryAmmo>();

            ammoCost = 1;
            maxAmmoClip = 6;
            ammoInClip = 6;
            reloadRate = 31f;
            initialReloadRate = 52f;
            reloadSound = new SoundStyle("TF2/Content/Sounds/SFX/shotgun_reload");

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
            clip.initialAmmoReloadRate = initialReloadRate;
            clip.ammoCurrent = ammoInClip;
            if (clip.startReload)
            {
                reload = true;
                clip.startReload = false;
            }
            UpdateResource();
        }

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
            reload = false;
            ammoInClip -= ammoCost;

            for (int i = 0; i < 10; i++)

            {
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(12f));
                Projectile.NewProjectile(source, position, newVelocity, ModContent.ProjectileType<Bullet>(), damage, knockback, player.whoAmI);
            }

            if (ammoInClip <= 0)
                reload = true;
            return false;
        }
    }
}