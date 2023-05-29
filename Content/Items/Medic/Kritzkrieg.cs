using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Projectiles.Medic;
using TF2.Common;

namespace TF2.Content.Items.Medic
{
    public class Kritzkrieg : MediGun
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Medic's Unlocked Secondary\n"
                + "ÜberCharge grants 100 % critical chance");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) // needs System.Linq
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Damage" && x.Mod == "Terraria");
            tooltips.Remove(tt);
            TooltipLine tt2 = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt2);

            var line = new TooltipLine(Mod, "Positive Attributes",
                "+25% ÜberCharge rate.")
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line);
        }

        public override void SafeSetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.scale = 1f;
            Item.useTime = 1;
            Item.useAnimation = 1;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item15;
            Item.autoReuse = true;
            Item.channel = true;

            Item.damage = 1;
            Item.shoot = ModContent.ProjectileType<HealingBeamKritzkrieg>();
            Item.shootSpeed = 1f;
            Item.knockBack = 0;
            Item.crit = 0;

            uberchargeCapacity = 750;

            Item.value = Item.buyPrice(platinum: 1);
            Item.rare = ModContent.RarityType<UniqueRarity>();
        }

        public override void UpdateInventory(Player player)
        {
            if (player.GetModPlayer<UbersawPlayer>().ubersawHit)
                player.GetModPlayer<TF2Player>().ubercharge += uberchargeCapacity / 4 + 1;
            if (player.GetModPlayer<VitaSawPlayer>().giveUberchargeFromVitaSaw)
                player.GetModPlayer<TF2Player>().ubercharge = (int)(player.GetModPlayer<VitaSawPlayer>().deathUbercharge * 0.75f);
        }

        public override void HoldItem(Player player)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            p.maxUbercharge = uberchargeCapacity;
            //UpdateResource();
        }

        public override bool AltFunctionUse(Player player)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (p.ubercharge >= uberchargeCapacity)
            {
                p.activateUbercharge = true;
                player.AddBuff(ModContent.BuffType<Buffs.KritzkriegUbercharge>(), 480);
            }
            return false;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (p.classAccessory && !p.classHideVanity)
                Item.noUseGraphic = true;
            else
                Item.noUseGraphic = false;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10f, 0f);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}