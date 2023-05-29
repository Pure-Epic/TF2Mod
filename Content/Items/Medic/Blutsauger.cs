using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Items.Ammo;
using TF2.Content.Projectiles.Medic;
using TF2.Common;

namespace TF2.Content.Items.Medic
{
    public class Blutsauger : TF2Weapon
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Medic's Unlocked Primary");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeSetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.scale = 1f;
            Item.useTime = 6;
            Item.useAnimation = 6;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/syringegun_shoot"); ;
            Item.autoReuse = true;

            Item.damage = 10;
            Item.shoot = ModContent.ProjectileType<BlutsaugerSyringe>();
            Item.shootSpeed = 25f;
            Item.useAmmo = ModContent.ItemType<PrimaryAmmo>();
            Item.knockBack = 0;
            Item.crit = 0;

            ammoCost = 1;
            maxAmmoClip = 40;
            ammoInClip = 40;
            reloadRate = 78f;
            magazine = true;
            reloadSound = new SoundStyle("TF2/Content/Sounds/SFX/syringegun_reload");

            Item.value = Item.buyPrice(platinum: 1);
            Item.rare = ModContent.RarityType<UniqueRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);

            var line = new TooltipLine(Mod, "Positive Attributes",
                "On Hit: Gain up to 2% of maximum health.")
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line);

            var line2 = new TooltipLine(Mod, "Negative Attributes",
                "-66% health regenerated per second on wearer")
            {
                OverrideColor = new Color(255, 64, 64)
            };
            tooltips.Add(line2);
        }

        public override void UpdateInventory(Player player)
        {
            for (int i = 0; i < 10; i++)
            {
                if (player.inventory[i].type == Type && !inHotbar)
                    inHotbar = true;
            }
            if (!inHotbar && !ModContent.GetInstance<TF2ConfigClient>().InventoryStats) return;
            player.GetModPlayer<BlutsaugerPlayer>().blutsaugerEquipped = true;
            inHotbar = false;
        }

        public override void HoldItem(Player player)
        {
            WeaponSystem clip = player.GetModPlayer<WeaponSystem>();
            clip.ammoMax = maxAmmoClip;
            clip.ammoReloadRate = reloadRate;
            clip.ammoCurrent = ammoInClip;
            if (clip.startReload)
            {
                reload = true;
                clip.startReload = false;
            }
            UpdateResource();
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) => ammoInClip > 0;

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
            if (ammoInClip == 0 || reload) return false;
            ammoInClip -= ammoCost;
            var i = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<BlutsaugerSyringe>(), damage, knockback, player.whoAmI);
            if (player.GetModPlayer<TF2Player>().focus)
            {
                Main.projectile[i].GetGlobalProjectile<Projectiles.TF2ProjectileBase>().homing = true;
                Main.projectile[i].GetGlobalProjectile<Projectiles.TF2ProjectileBase>().shootSpeed = Item.shootSpeed;
                NetMessage.SendData(MessageID.SyncProjectile, number: i);
            }

            if (ammoInClip <= 0)
                reload = true;
            return false;
        }
        public override Vector2? HoldoutOffset() => new Vector2(0, 0);

        public override bool CanUseItem(Player player) => !(ammoInClip <= 0 || reload);
    }

    public class BlutsaugerPlayer : ModPlayer
    {
        public bool blutsaugerEquipped;

        public override void ResetEffects() => blutsaugerEquipped = false;
    }
}