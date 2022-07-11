using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using System.Linq;
using Terraria.Audio;
using TF2.Projectiles.Spy;

namespace TF2.Items.Spy
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
            Item.scale = 1f;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = false;
            Item.UseSound = new SoundStyle("TF2/Sounds/SFX/melee_swing");
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.shoot = ModContent.ProjectileType<SapperProjectile>();
            Item.shootSpeed = 25;
            Item.noUseGraphic = true;
            Item.damage = 25;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                TFClass p = player.GetModPlayer<TFClass>();
                return true;
            }
            return false;
        }

        public override void HoldItem(Player player)
        {
            Item.scale = 0f;
        }
    }
}
