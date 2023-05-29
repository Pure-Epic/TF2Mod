using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Buffs;

namespace TF2.Content.Items.Sniper
{
    public class Jarate : TF2WeaponNoAmmo
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Sniper's Unlocked Secondary");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeSetDefaults()
        {
            Item.width = 27;
            Item.height = 50;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/sniper_jaratetoss01");
            Item.autoReuse = true;
            Item.noUseGraphic = true;

            Item.damage = 1;
            Item.shoot = ModContent.ProjectileType<Projectiles.Sniper.JarateProjectile>();
            Item.shootSpeed = 10f;

            Item.value = Item.buyPrice(platinum: 1);
            Item.rare = ModContent.RarityType<UniqueRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) // needs System.Linq
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Damage" && x.Mod == "Terraria");
            tooltips.Remove(tt);
            TooltipLine tt2 = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt2);

            var line = new TooltipLine(Mod, "Positive Attributes",
                "Extinguishing teammates reduces cooldown by -20%")
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line);

            var line2 = new TooltipLine(Mod, "Neutral Attributes",
                "Coated enemies take mini-crits\n"
                + "\"It's just ichor in a jar, guys!\"")
            {
                OverrideColor = new Color(255, 255, 255)
            };
            tooltips.Add(line2);
        }

        public override bool CanUseItem(Player player) => !player.GetModPlayer<JaratePlayer>().jarateCooldown;

        public override bool? UseItem(Player player)
        {
            player.AddBuff(ModContent.BuffType<JarateCooldown>(), 1200);
            return true;
        }
    }
}