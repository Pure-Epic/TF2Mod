using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace TF2.Content.Items.Medic
{
    public class Bonesaw : TF2WeaponMelee
    {
        public override void SetStaticDefaults() => Tooltip.SetDefault("Medic's Starter Melee");

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
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/melee_swing");
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.rare = ModContent.RarityType<NormalRarity>();

            Item.damage = 65;
            Item.knockBack = 0;
            Item.crit = 0;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);
        }
    }
}