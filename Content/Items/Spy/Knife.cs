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
    public class Knife : TF2WeaponMelee
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Spy's Starter Melee");

            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SafeSetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.useTime = 48;
            Item.useAnimation = 48;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.noMelee = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/knife_swing");
            Item.noUseGraphic = true;
            Item.autoReuse = true;

            Item.damage = 40;
            Item.shoot = ModContent.ProjectileType<KnifeProjectile>();
            Item.shootSpeed = 2.1f;
            Item.GetGlobalItem<TF2ItemBase>().noRandomCrits = true;

            Item.rare = ModContent.RarityType<NormalRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);

            var line = new TooltipLine(Mod, "Neutral Attributes",
                "Alt-Fire: Does a backstab that lunges the player\n"
                + "and deals critical damage.")
            {
                OverrideColor = new Color(255, 255, 255)
            };
            tooltips.Add(line);
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (player.altFunctionUse == 2)
            {
                damage = (int)(120 * p.classMultiplier);
                p.backStab = true;
                player.velocity = velocity * 12.5f;
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                player.immuneTime += 24;
                return false;
            }
            else
                return true;
        }
    }
}