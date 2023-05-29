using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace TF2.Content.Items.Medic
{
    public class Ubersaw : TF2WeaponMelee
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Medic's Unlocked Melee");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeSetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.scale = 1f;
            Item.useTime = 58;
            Item.useAnimation = 58;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = false;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/melee_swing");
            Item.autoReuse = true;
            Item.useTurn = true;

            Item.damage = 65;
            Item.knockBack = 0;
            Item.crit = 0;

            Item.value = Item.buyPrice(platinum: 1);
            Item.rare = ModContent.RarityType<UniqueRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);

            var line = new TooltipLine(Mod, "Positive Attributes",
                "On Hit: 25% ÜberCharge added")
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line);

            var line2 = new TooltipLine(Mod, "Negative Attributes",
                "20% slower firing speed")
            {
                OverrideColor = new Color(255, 64, 64)
            };
            tooltips.Add(line2);
        }
    }

    public class UbersawPlayer : ModPlayer
    {
        public bool ubersawHit;
        public int timer;

        public override void PostUpdate()
        {
            if (ubersawHit)
            {
                timer++;
                if (timer >= 2) // MUST ALWAYS be 2!
                {
                    ubersawHit = false;
                    timer = 0;
                }
            }
        }

        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            if (item.type == ModContent.ItemType<Ubersaw>())
                ubersawHit = true;
        }
    }
}