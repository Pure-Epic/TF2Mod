using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Items.Ammo;
using TF2.Content.Projectiles.Medic;
using TF2.Common;

namespace TF2.Content.Items.Medic
{
    public class SyringeGun : TF2Weapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Syringe Gun");
            Tooltip.SetDefault("Medic's Starter Primary");
        }

        public override void SafeSetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.scale = 1f;
            Item.useTime = 6;
            Item.useAnimation = 6;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/syringegun_shoot"); ;
            Item.autoReuse = true;

            Item.damage = 10;
            Item.shoot = ModContent.ProjectileType<Syringe>();
            Item.shootSpeed = 25f;
            Item.useAmmo = ModContent.ItemType<PrimaryAmmo>();
            Item.knockBack = 0;
            Item.crit = 0;

            Item.rare = ModContent.RarityType<NormalRarity>();
            ammoCost = 1;
            maxAmmoClip = 40;
            ammoInClip = 40;
            reloadRate = 78f;
            magazine = true;
            reloadSound = new SoundStyle("TF2/Content/Sounds/SFX/syringegun_reload");
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
            var i = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<Syringe>(), damage, knockback, player.whoAmI);
            if (player.GetModPlayer<TF2Player>().focus)
            {
                Main.projectile[i].GetGlobalProjectile<Projectiles.TF2ProjectileBase>().homing = true;
                Main.projectile[i].GetGlobalProjectile<Projectiles.TF2ProjectileBase>().shootSpeed = Item.shootSpeed;
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