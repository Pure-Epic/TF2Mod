using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Projectiles.Heavy;

namespace TF2.Content.Items.Heavy
{
    public class KillingGlovesofBoxing : TF2WeaponMelee
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Killing Gloves of Boxing");
            Tooltip.SetDefault("Heavy's Unlocked Melee");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SafeSetDefaults()
        {
            Item.damage = 65;
            Item.useStyle = ItemUseStyleID.Rapier; // Makes the player do the proper arm motion
            Item.useAnimation = 58;
            Item.useTime = 58;
            Item.width = 50;
            Item.height = 50;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/melee_swing");
            Item.autoReuse = true;
            Item.noUseGraphic = true; // The sword is actually a "projectile", so the item should not be visible when used
            Item.noMelee = true; // The projectile will do the damage and not the item

            Item.shoot = ModContent.ProjectileType<KillingGlovesofBoxingProjectile>(); // The projectile is what makes a shortsword work
            Item.shootSpeed = 2.1f; // This value bleeds into the behavior of the projectile as velocity, keep that in mind when tweaking values

            Item.value = Item.buyPrice(platinum: 1);
            Item.rare = ModContent.RarityType<UniqueRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);

            var line = new TooltipLine(Mod, "Positive Attributes",
                "On Hit: 5 seconds of 100% critical chance")
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line);

            var line2 = new TooltipLine(Mod, "Negative Attributes",
                "-20% slower firing speed")
            {
                OverrideColor = new Color(255, 64, 64)
            };
            tooltips.Add(line2);
        }

        public override bool AltFunctionUse(Player player) => true;
    }
}