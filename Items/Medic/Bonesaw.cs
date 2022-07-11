using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;

namespace TF2.Items.Medic
{
    public class Bonesaw : TF2WeaponNoAmmo
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Medic's Starter Melee");
        }

        public override void SafeSetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.scale = 1f;
            Item.useTime = 48;
            Item.useAnimation = 48;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = false;
            Item.UseSound = new SoundStyle("TF2/Sounds/SFX/melee_swing");
            Item.autoReuse = true;
            Item.useTurn = true;

            Item.damage = 65;
            Item.knockBack = -1;
            Item.crit = 0;
        }
    }
}
