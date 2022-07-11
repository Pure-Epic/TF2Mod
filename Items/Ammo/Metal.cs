using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Items;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace TF2.Items.Ammo
{
    public class Metal : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Medium Ammo Box");
        }
        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 14;
            Item.rare = ItemRarityID.White;
        }
        
        public override bool OnPickup(Player player)
        {
            TFClass p = player.GetModPlayer<TFClass>();
            p.metal += 100;
            SoundEngine.PlaySound(new SoundStyle("TF2/Sounds/SFX/medkit"), player.Center);
            Item.stack = 0;
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.Lerp(lightColor, Color.White, 0.4f);
        }
    }
}
