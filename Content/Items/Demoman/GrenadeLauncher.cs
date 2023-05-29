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

namespace TF2.Content.Items.Demoman
{
    public class GrenadeLauncher : TF2Weapon
    {
        public bool startReloadSound;
        public bool finishReloadSound;
        public bool closeDrum;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Grenade Launcher");
            Tooltip.SetDefault("Demoman's Starter Primary");
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
            Item.UseSound = new SoundStyle("TF2/Content/Sounds/SFX/grenade_launcher_shoot");
            Item.autoReuse = true;

            Item.damage = 100; //Damage set on projectile
            Item.shoot = ModContent.ProjectileType<Grenade>();
            Item.shootSpeed = 12.5f;
            Item.useAmmo = ModContent.ItemType<PrimaryAmmo>();
            Item.knockBack = 20;
            Item.rare = ModContent.RarityType<NormalRarity>();

            ammoCost = 1;
            maxAmmoClip = 4;
            ammoInClip = 4;
            reloadRate = 36f;
            initialReloadRate = 74f;
            reloadSound = new SoundStyle("TF2/Content/Sounds/SFX/grenade_launcher_reload");
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
            ammoInClip -= ammoCost;

            var i = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<Grenade>(), damage, knockback, player.whoAmI);
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

        new public void UpdateResource()
        {
            if (reload)
                cooldownTimer++;
            else
                cooldownTimer = 0;

            if (!startReloadSound && reload)
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/grenade_launcher_drum_open"), Main.LocalPlayer.Center);
                startReloadSound = true;
            }

            // A simple timer that goes up to 1 frame, increases the ammoCurrent by 1 and then resets back to 0.
            if (reload)
            {
                if (cooldownTimer < (int)(initialReloadRate - reloadRate)) return;

                if (!finishReloadSound && startReloadSound)
                {
                    SoundEngine.PlaySound(reloadSound, Main.LocalPlayer.Center);
                    finishReloadSound = true;
                }

                if (ammoReloadRateTimer >= reloadRate)
                {
                    ammoInClip += 1;
                    finishReloadSound = false;
                    ammoReloadRateTimer = 0;
                }
            }

            // For our resource lets make it regen slowly over time to keep it simple, let's use ammoReloadRateTimer to count up to whatever value we want, then increase currentResource.
            if (reload) // && cooldownTimer >= (int)(initialReloadRate - reloadRate + 30f)
                ammoReloadRateTimer++; //Increase it by 60 per second, or 1 per tick.
            else
                ammoReloadRateTimer = 0;

            // Limit ammoCurrent from going over the limit imposed by ammoMax.
            ammoInClip = Utils.Clamp(ammoInClip, 0, maxAmmoClip);

            if (ammoInClip == maxAmmoClip)
            {
                if (!finishReloadSound && reload)
                    closeDrum = true;
                StopReload();
            }
            if (closeDrum && !reload)
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/grenade_launcher_drum_close"), Main.LocalPlayer.Center);
                closeDrum = false;
            }

            if (!reload)
            {
                startReloadSound = false;
                finishReloadSound = false;
            }
        }
    }
}