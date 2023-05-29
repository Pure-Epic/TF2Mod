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

namespace TF2.Gensokyo.Content.Items.Spy
{
    [ExtendsFromMod("Gensokyo")]
    public class HeadhunterPistols : TF2Weapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Headhunter Pistols");
            Tooltip.SetDefault("Spy's Exclusive Secondary");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeSetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/pistol_shoot");
            Item.autoReuse = true;

            Item.damage = 45;
            Item.shoot = ModContent.ProjectileType<Bullet>();
            Item.shootSpeed = 10f;
            Item.useAmmo = ModContent.ItemType<SecondaryAmmo>();
            Item.GetGlobalItem<TF2ItemBase>().noRandomCrits = true;

            ammoCost = 1;
            maxAmmoClip = 20;
            ammoInClip = 20;
            reloadRate = 136;
            magazine = true;
            reloadSound = new SoundStyle("TF2/Gensokyo/Content/Sounds/SFX/headhunterpistols_reload");

            Item.value = Item.buyPrice(platinum: 10);
            Item.rare = ModContent.RarityType<UnusualRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);

            var line = new TooltipLine(Mod, "Positive Attributes",
                "+66% clip size\n"
                + "+200% damage bonus")
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line);

            var line2 = new TooltipLine(Mod, "Negative Attributes",
                "100% slower firing speed\n"
                + "100% slower reload time\n"
                + "No random critical hits")
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
            clip.ammoCurrent = ammoInClip;
            if (clip.startReload)
            {
                reload = true;
                clip.startReload = false;
            }
            UpdateResource();
            spreadRecovery++;
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

            Vector2 newVelocity;
            if (spreadRecovery >= 75)
                newVelocity = velocity;
            else
                newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(2.5f));
            var i = Projectile.NewProjectile(source, position, newVelocity, ModContent.ProjectileType<Bullet>(), damage, knockback, player.whoAmI);
            if (player.GetModPlayer<TF2Player>().focus)
            {
                Main.projectile[i].GetGlobalProjectile<TF2ProjectileBase>().homing = true;
                Main.projectile[i].GetGlobalProjectile<TF2ProjectileBase>().shootSpeed = Item.shootSpeed;
                NetMessage.SendData(MessageID.SyncProjectile, number: i);
            }
            spreadRecovery = 0;

            if (ammoInClip <= 0)
                reload = true;
            return false;
        }

        public override Vector2? HoldoutOffset() => new Vector2(2.5f, 0f);

        public override bool CanUseItem(Player player) => !(ammoInClip <= 0 || reload);
    }
}