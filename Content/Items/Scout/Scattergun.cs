using Terraria.ModLoader;
using TF2.Content.Projectiles;

namespace TF2.Content.Items.Scout
{
    public class Scattergun : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Scout, Primary, Stock, Starter);
            SetWeaponSize(50, 14);
            SetGunUseStyle();
            SetWeaponDamage(damage: 6, projectile: ModContent.ProjectileType<Bullet>(), projectileCount: 10, shootAngle: 12f);
            SetWeaponAttackSpeed(0.625);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/scatter_gun_shoot");
            SetWeaponAttackIntervals(maxAmmo: 6, reloadTime: 0.5, initialReloadTime: 0.7, reloadSoundPath: "TF2/Content/Sounds/SFX/scatter_gun_reload");
        }
    }
}

/*
    public class Scattergun : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            Item.width = 50;
            Item.height = 14;
            Item.useTime = 38;
            Item.useAnimation = 38;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/scatter_gun_shoot");
            Item.autoReuse = true;

            Item.damage = 6;
            Item.shoot = ModContent.ProjectileType<Bullet>();
            Item.shootSpeed = 10f;
            Item.useAmmo = ModContent.ItemType<PrimaryAmmo>();

            ammoCost = 1;
            currentAmmoClip = 6;
            maxAmmoClip = 6;
            reloadRate = 30;
            initialReloadRate = 42;
            reloadSound = new SoundStyle("TF2/Content/Sounds/SFX/scatter_gun_reload");

            Item.rare = ModContent.RarityType<NormalRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);
        }

        public override void HoldItem(Player player)
        {
            AmmoInterface clip = player.GetModPlayer<AmmoInterface>();
            clip.ammoCurrent = currentAmmoClip;
            clip.ammoMax = maxAmmoClip;
            if (clip.startReload)
            {
                reload = true;
                clip.startReload = false;
            }
            UpdateAmmo(player);
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame) => Item.noUseGraphic = player.GetModPlayer<TF2Player>().classAccessory && !player.GetModPlayer<TF2Player>().classHideVanity;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (currentAmmoClip <= 0) return false;
            reload = false;
            currentAmmoClip -= ammoCost;

            for (int i = 0; i < 10; i++)
            {
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(12f));
                Projectile.NewProjectile(source, position, newVelocity, ModContent.ProjectileType<Bullet>(), damage, knockback, player.whoAmI);
            }

            if (currentAmmoClip <= 0)
                reload = true;
            return false;
        }
    }
 */