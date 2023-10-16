using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items;
using TF2.Content.Items.Ammo;
using TF2.Content.Projectiles;
using TF2.Content.Projectiles.Soldier;

namespace TF2.Gensokyo.Content.Items.Soldier
{
    [ExtendsFromMod("Gensokyo")]
    public class OffensiveRocketSystem : TF2Weapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Offensive Rocket System");
            Tooltip.SetDefault("Soldier's Exclusive Primary");
            ItemID.Sets.ItemsThatCountAsBombsForDemolitionistToSpawn[Item.type] = true;

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeSetDefaults()
        {
            Item.width = 50;
            Item.height = 22;
            Item.useTime = 16;
            Item.useAnimation = 48;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/rocket_shoot");
            Item.autoReuse = true;

            Item.damage = 45;
            Item.shoot = ModContent.ProjectileType<Rocket>();
            Item.shootSpeed = 20f;
            Item.useAmmo = ModContent.ItemType<PrimaryAmmo>();
            Item.knockBack = 20f;

            ammoCost = 1;
            maxAmmoClip = 6;
            ammoInClip = 6;
            reloadRate = 24f;
            initialReloadRate = 28f;
            reloadSound = new SoundStyle("TF2/Content/Sounds/SFX/rocket_reload");

            Item.value = Item.buyPrice(platinum: 10);
            Item.rare = ModContent.RarityType<UnusualRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);

            var line = new TooltipLine(Mod, "Positive Attributes",
                "+50% clip size\n"
                + "50% faster reload time\n"
                + "Fires a burst of 3 rockets")
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line);

            var line2 = new TooltipLine(Mod, "Negative Attributes",
                "-50% damage penalty")
            {
                OverrideColor = new Color(255, 64, 64)
            };
            tooltips.Add(line2);
        }

        public override void HoldItem(Player player)
        {
            WeaponSystem clip = player.GetModPlayer<WeaponSystem>();
            clip.ammoMax = maxAmmoClip;
            clip.ammoReloadRate = reloadRate;
            clip.initialAmmoReloadRate = initialReloadRate;
            clip.ammoCurrent = ammoInClip;
            if (ammoInClip <= 0 && player.itemAnimation == 0)
                reload = true;
            if (clip.startReload)
            {
                reload = true;
                clip.startReload = false;
            }
            UpdateResource();
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (p.classAccessory && !p.classHideVanity)
                Item.noUseGraphic = true;
            else
                Item.noUseGraphic = false;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            reload = false;
            if (player.itemAnimation == player.itemAnimationMax)
                ammoInClip -= ammoCost;

            var i = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<DirectHitRocket>(), damage, knockback, player.whoAmI);
            if (player.GetModPlayer<TF2Player>().focus)
            {
                Main.projectile[i].GetGlobalProjectile<TF2ProjectileBase>().homing = true;
                Main.projectile[i].GetGlobalProjectile<TF2ProjectileBase>().shootSpeed = Item.shootSpeed;
                NetMessage.SendData(MessageID.SyncProjectile, number: i);
            }

            return false;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-28.75f, 0f);

        public override bool CanConsumeAmmo(Item ammo, Player player) => player.itemAnimation >= player.itemAnimationMax - 3 && !reload;
    }
}