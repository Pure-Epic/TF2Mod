using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Items.Ammo;
using TF2.Content.Projectiles.Demoman;
using TF2.Common;
using TF2.Content.Projectiles;

namespace TF2.Content.Items.Demoman
{
    public class StickybombLauncher : TF2Weapon
    {
        public int stickybombsAmount = 0;
        public int maxStickybombs = 0;

        public float chargeTime = 0f;
        public float maxChargeUp;
        public float stickybombChargeInterval;
        public float chargeUpRate;

        public bool rightClick;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stickybomb Launcher");
            Tooltip.SetDefault("Alt-Fire: Detonate all Stickybombs\n"
                + "Demoman's Starter Secondary");
            ItemID.Sets.ItemsThatCountAsBombsForDemolitionistToSpawn[Item.type] = true;
        }

        public override void SafeSetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.scale = 1f;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.channel = true;

            Item.damage = 120;
            Item.shoot = ModContent.ProjectileType<Stickybomb>();
            Item.shootSpeed = 10f;
            Item.useAmmo = ModContent.ItemType<SecondaryAmmo>();
            Item.knockBack = 20;
            Item.rare = ModContent.RarityType<NormalRarity>();

            maxStickybombs = 8;
            ammoCost = 1;
            maxAmmoClip = 8;
            ammoInClip = 8;
            reloadRate = 40f;
            initialReloadRate = 65f;
            maxChargeUp = 240f;
            stickybombChargeInterval = 1f;
            chargeUpRate = 1f;
            reloadSound = new SoundStyle("TF2/Content/Sounds/SFX/stickybomblauncher_reload");
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);
        }

        public override void HoldItem(Player player)
        {
            WeaponSystem clip = player.GetModPlayer<WeaponSystem>();
            clip.ammoMax = maxAmmoClip;
            clip.ammoReloadRate = reloadRate;
            clip.initialAmmoReloadRate = initialReloadRate;
            clip.ammoCurrent = ammoInClip;
            chargeTime = Utils.Clamp(chargeTime, 0, maxChargeUp);
            if (clip.startReload)
            {
                reload = true;
                clip.startReload = false;
            }
            UpdateResource();
            if (ModContent.GetInstance<TF2ConfigClient>().Channel)
                Charge();
            if (!ModContent.GetInstance<TF2ConfigClient>().Channel)
                maxChargeUp = 240f;
            else
                maxChargeUp = 240f + Item.useTime;
            TF2Player p = player.GetModPlayer<TF2Player>();
            p.stickybombMaxCharge = maxChargeUp;
            p.stickybombCharge = chargeTime;
            p.stickybombChargeTimer = stickybombChargeInterval;
            p.stickybombAmount = stickybombsAmount;
            p.stickybombMax = maxStickybombs;

            if ((Main.mouseLeftRelease || chargeTime == maxChargeUp) && isCharging && !ModContent.GetInstance<TF2ConfigClient>().Channel && !player.dead)
            {
                if (stickybombsAmount < maxStickybombs && player.altFunctionUse != 2 && !rightClick)
                {
                    Vector2 shootDirection = player.DirectionTo(Main.MouseWorld);
                    reload = false;
                    ammoInClip -= ammoCost;

                    Vector2 newVelocity = new Vector2(shootDirection.X * Item.shootSpeed + chargeTime / 60, shootDirection.Y * Item.shootSpeed - chargeTime / 60);
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/stickybomblauncher_shoot"), player.Center);
                    Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, newVelocity, ModContent.ProjectileType<Stickybomb>(), Item.damage, Item.knockBack, player.whoAmI);
                    chargeTime = 0f;
                    stickybombsAmount += 1;

                    isCharging = false;
                    ChargeWeaponConsumeAmmo(player);

                    if (ammoInClip <= 0)
                        reload = true;
                }
            }

            if (player.dead)
                rightClick = false;

            if (stickybombsAmount < 0)
                stickybombsAmount = 0;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (p.classAccessory && !p.classHideVanity)
                Item.noUseGraphic = true;
            else
                Item.noUseGraphic = false;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (stickybombsAmount < maxStickybombs)
                return ModContent.GetInstance<TF2ConfigClient>().Channel;
            else
                return false;
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                if (!ModContent.GetInstance<TF2ConfigClient>().Channel && player.controlUseItem && player.itemTime == 0 && Main.mouseLeft)
                {
                    isCharging = true;
                    rightClick = false;
                    Charge();
                    player.itemAnimation = Item.useTime;
                    return false;
                }
                else
                    return true;
            }
            return null;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!ModContent.GetInstance<TF2ConfigClient>().Channel)
                return false;
            if (stickybombsAmount < maxStickybombs && player.altFunctionUse != 2)
            {
                reload = false;
                ammoInClip -= ammoCost;

                Vector2 newVelocity = new Vector2(velocity.X + chargeTime / 60, velocity.Y - chargeTime / 60);
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/stickybomblauncher_shoot"), player.Center);
                Projectile.NewProjectile(source, position, newVelocity, ModContent.ProjectileType<Stickybomb>(), damage, knockback, player.whoAmI);
                chargeTime = 0f;
                stickybombsAmount += 1;
                rightClick = false;

                if (ammoInClip <= 0)
                    reload = true;
            }
            return false;
        }

        public override bool AltFunctionUse(Player player)
        {
            if (Main.mouseLeft)
                return false;
            rightClick = true;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.owner == player.whoAmI && proj.type == ModContent.ProjectileType<Stickybomb>() && proj.GetGlobalProjectile<TF2ProjectileBase>().time >= 42)
                {
                    proj.timeLeft = 0;
                    stickybombsAmount--;
                }
            }
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/stickybomblauncher_det"), player.Center);
            return true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(0, 0);

        public void Charge()
        {
            if (stickybombsAmount == maxStickybombs) return;
            stickybombChargeInterval++;
            if (stickybombChargeInterval > chargeUpRate)
            {
                chargeTime += 1f;
                stickybombChargeInterval = 0;
            }
            chargeTime = Utils.Clamp(chargeTime, 0, maxChargeUp);
        }
    }
}