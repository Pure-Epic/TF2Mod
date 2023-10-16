using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Items
{
    // This class handles everything for our custom damage class
    // Any class that we wish to be using our custom damage class will derive from this class, instead of ModItem
    public abstract class TF2Weapon : ModItem
    {
        public int ammoCost = 0;
        public int ammoInClip = 0;
        public float reloadRate = 0;
        public int maxAmmoClip = 100;
        public int ammoReloadRateTimer;
        public int cooldownTimer;
        public float initialReloadRate;
        public int deployTimer = 0;
        public int deploySpeed = 0;
        public int holsterTimer = 0;
        public int holsterSpeed = 0;
        public bool reload;
        public bool magazine;
        public SoundStyle reloadSound;

        public bool isCharging;
        public bool startedAttacking;
        private bool finishReloadSound;
        public int spreadRecovery;

        public bool inHotbar;

        // Custom items should override this to set their defaults
        public virtual void SafeSetDefaults()
        {

        }

        // By making the override sealed, we prevent derived classes from further overriding the method and enforcing the use of SafeSetDefaults()
        // We do this to ensure that the vanilla damage types are always set to false, which makes the custom damage type work
        public sealed override void SetDefaults()
        {
            SafeSetDefaults();
            Item.DamageType = ModContent.GetInstance<TF2DamageClass>();
            if (magazine)
                initialReloadRate = reloadRate;
        }

        public override void ModifyWeaponCrit(Player player, ref float crit)
        {
            if (player.HeldItem.ModItem is TF2WeaponMelee)
            {
                if (Main.rand.NextBool(6) && !player.HeldItem.GetGlobalItem<TF2ItemBase>().noRandomCrits)
                    player.GetModPlayer<TF2Player>().crit = true;
            }
            else
            {
                if (Main.rand.NextBool(50) && !player.HeldItem.GetGlobalItem<TF2ItemBase>().noRandomCrits)
                    player.GetModPlayer<TF2Player>().crit = true;
            }
            crit = 0;
        }

        // Make sure you can't use the item if you don't have enough resource and then use 10 resource otherwise.
        public override bool CanUseItem(Player player)
        {
            if (reload && !ModContent.GetInstance<TF2ConfigClient>().SingleReload) return false;
            return (ammoInClip > 0 && ammoInClip >= ammoCost) || player.HeldItem.ModItem is TF2WeaponNoAmmo;
        }

        public void UpdateResource()
        {
            if (reload)
                cooldownTimer++;
            else
                cooldownTimer = 0;

            // A simple timer that goes up to 1 frame, increases the ammoCurrent by 1 and then resets back to 0.
            if (reload)
            {
                if (cooldownTimer < (int)(initialReloadRate - reloadRate)) return;

                if (!finishReloadSound)
                {
                    SoundEngine.PlaySound(reloadSound, Main.LocalPlayer.Center);
                    finishReloadSound = true;
                }

                if (ammoReloadRateTimer >= reloadRate)
                {
                    if (magazine)
                        ammoInClip = maxAmmoClip;
                    else
                    {
                        ammoInClip += 1;
                        finishReloadSound = false;
                    }
                    ammoReloadRateTimer = 0;
                }
            }

            // For our resource lets make it regen slowly over time to keep it simple, let's use ammoReloadRateTimer to count up to whatever value we want, then increase currentResource.
            if (reload)
                ammoReloadRateTimer++; // Increase it by 60 per second, or 1 per tick.
            else
                ammoReloadRateTimer = 0;

            // Limit ammoCurrent from going over the limit imposed by ammoMax.
            ammoInClip = Utils.Clamp(ammoInClip, 0, maxAmmoClip);

            if (ammoInClip == maxAmmoClip)
                StopReload();

            if (!reload)
                finishReloadSound = false;
            else
                spreadRecovery = 75;
        }

        public void ReloadWeapon() => reload = true;

        public void StopReload() => reload = false;

        public void ChargeWeaponConsumeAmmo(Player player)
        {
            bool ammoSlot = false;

            for (int i = 54; i < player.inventory.Length; i++)
            {
                var item = player.inventory[i];
                if (item.type == Item.useAmmo)
                {
                    item.stack -= 1;
                    ammoSlot = true;
                    break;
                }
            }
            if (!ammoSlot)
            {
                for (int i = 0; i < player.inventory.Length; i++)
                {
                    var item = player.inventory[i];
                    if (item.type == Item.useAmmo)
                    {
                        item.stack -= 1;
                        break;
                    }
                }
            }
        }
    }

    public abstract class TF2WeaponNoAmmo : TF2Weapon
    {
        // Weapons without ammo won't render the ammo ui
    }

    public abstract class TF2WeaponMelee : TF2WeaponNoAmmo
    {
        // Chargin' Targe Support

        public override void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
        {
            if (player.GetModPlayer<TF2Player>().critMelee)
                player.GetModPlayer<TF2Player>().crit = true;
            player.ClearBuff(ModContent.BuffType<Buffs.MeleeCrit>());
        }

        public override void ModifyHitPvp(Player player, Player target, ref int damage, ref bool crit)
        {
            if (player.GetModPlayer<TF2Player>().critMelee)
                player.GetModPlayer<TF2Player>().crit = true;
            player.ClearBuff(ModContent.BuffType<Buffs.MeleeCrit>());
        }
    }
}