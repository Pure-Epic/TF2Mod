﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Items.Ammo
{
    public class Metal : ModItem
    {
        public override void SetStaticDefaults() => DisplayName.SetDefault("Medium Ammo Box");

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 14;
            Item.rare = ItemRarityID.White;
        }

        public override bool OnPickup(Player player)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            p.metal += 100;
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/medkit"), player.Center);
            Item.stack = 0;
            return false;
        }

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.4f);

        public override bool ItemSpace(Player player) => true;
    }
}