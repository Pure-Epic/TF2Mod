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
using TF2.Content.Projectiles.Pyro;
using TF2.Gensokyo.Content.Projectiles.Pyro;

namespace TF2.Gensokyo.Content.Items.Pyro
{
    [ExtendsFromMod("Gensokyo")]
    public class ManualInferno : TF2Weapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Manual Inferno");
            Tooltip.SetDefault("Pyro's Exclusive Primary");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeSetDefaults()
        {
            Item.width = 55;
            Item.height = 22;
            Item.useTime = 6;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.autoReuse = true;

            Item.damage = 78;
            Item.shoot = ModContent.ProjectileType<HellFire>();
            Item.shootSpeed = 10f;
            Item.useAmmo = ModContent.ItemType<PrimaryAmmo>();
            Item.mana = 20;

            ammoCost = 1;
            maxAmmoClip = 65;
            ammoInClip = 65;
            reloadRate = 90f;
            magazine = true;
            reloadSound = new SoundStyle("TF2/Gensokyo/Content/Sounds/SFX/manualinferno_reload");

            Item.value = Item.buyPrice(platinum: 10);
            Item.rare = ModContent.RarityType<UnusualRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);

            var line = new TooltipLine(Mod, "Positive Attributes",
                "+200% afterburn damage bonus\n"
                + "Extinguishing teammates restores 11% of max health")
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line);

            var line2 = new TooltipLine(Mod, "Negative Attributes",
                "Reloading required")
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
            if (ammoInClip <= 0 && player.itemAnimation == 0)
                reload = true;
            if (clip.startReload && player.itemAnimation == 0)
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

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.useTime = 45;
                Item.useAnimation = 45;
                Item.damage = 1;
                Item.shoot = ModContent.ProjectileType<Airblast>();
                Item.shootSpeed = 25f;
            }
            else
            {
                Item.useTime = 6;
                Item.useAnimation = 30;
                Item.damage = 78;
                Item.shoot = ModContent.ProjectileType<HellFire>();
                Item.shootSpeed = 10f;
            }
            return !(ammoInClip <= 0 || reload);
        }

        public override bool AltFunctionUse(Player player) => true;

        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            if (player.altFunctionUse != 2)
                reduce -= 20;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (player.altFunctionUse != 2)
            {
                if (player.itemAnimation >= player.itemAnimationMax - 5)
                    ammoInClip -= ammoCost;
                return player.itemAnimation >= player.itemAnimationMax - 5 && !reload;
            }
            else
                return false;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {          
            if (player.altFunctionUse != 2)
            {
                Vector2 muzzleOffset = Vector2.Normalize(velocity) * 54f;
                if (Collision.CanHit(position, 6, 6, position + muzzleOffset, 6, 6))
                {
                    position += muzzleOffset;
                    SoundEngine.PlaySound(SoundID.Item34, player.Center);
                    Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<HellFire>(), damage, knockback, player.whoAmI);
                }
                return false;
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/flame_thrower_airblast"), player.Center);
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<Airblast>(), damage, knockback, player.whoAmI);
                return false;
            }
        }

        public override Vector2? HoldoutOffset() => new Vector2(-7.5f, 0f);
    }
}