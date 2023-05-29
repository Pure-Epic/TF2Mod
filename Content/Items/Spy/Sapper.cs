using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Projectiles.Spy;

namespace TF2.Content.Items.Spy
{
    public class Sapper : TF2WeaponNoAmmo
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sapper");
            Tooltip.SetDefault("Stuns enemies");
        }

        public override void SafeSetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/melee_swing");
            Item.noUseGraphic = true;
            Item.autoReuse = true;

            Item.damage = 25;
            Item.shoot = ModContent.ProjectileType<SapperProjectile>();
            Item.shootSpeed = 25;

            Item.rare = ModContent.RarityType<NormalRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var i = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (player.GetModPlayer<TF2Player>().focus)
            {
                Main.projectile[i].GetGlobalProjectile<Projectiles.TF2ProjectileBase>().homing = true;
                Main.projectile[i].GetGlobalProjectile<Projectiles.TF2ProjectileBase>().shootSpeed = Item.shootSpeed;
                NetMessage.SendData(MessageID.SyncProjectile, number: i);
            }
            return false;
        }
    }
}