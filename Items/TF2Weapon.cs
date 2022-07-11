using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace TF2.Items
{
    // This class handles everything for our custom damage class
    // Any class that we wish to be using our custom damage class will derive from this class, instead of ModItem
    public abstract class TF2Weapon : ModItem
    {
        //public override bool CloneNewInstances => true;
        public int ammoCost = 0;
        public int ammoInClip = 0;
        public float reloadRate = 0;
        public int maxAmmoClip = 100;
        public int ammoReloadRateTimer;
        public bool reload = false;
        public bool magazine;

        // Custom items should override this to set their defaults
        public virtual void SafeSetDefaults()
        {

        }

        // By making the override sealed, we prevent derived classes from further overriding the method and enforcing the use of SafeSetDefaults()
        // We do this to ensure that the vanilla damage types are always set to false, which makes the custom damage type work
        public sealed override void SetDefaults()
        {
            SafeSetDefaults();
            // all vanilla damage types must be false for custom damage types to work
            //Item.melee = false;
            Item.DamageType = ModContent.GetInstance<TF2DamageClass>();
            //Item.magic = false;
            //Item.thrown = false;
            //Item.summon = false;
            //WeaponSystem.ModPlayer(player).ammoCurrent = WeaponSystem.ModPlayer(player).ammoMax;
        }

        // As a modder, you could also opt to make these overrides also sealed. Up to the modder
        // Depricated due to DamageClass
        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            //damage += WeaponSystem.ModPlayer(player).tf2DamageAdd;
            damage *= WeaponSystem.ModPlayer(player).tf2DamageMult;
        }

        public override void ModifyWeaponKnockback(Player player, ref StatModifier knockback)
        {
            // Adds knockback bonuses
            knockback += WeaponSystem.ModPlayer(player).tf2Knockback;
        }

        public override void ModifyWeaponCrit(Player player, ref float crit)
        {
            // Adds crit bonuses
            crit += WeaponSystem.ModPlayer(player).tf2Crit;
        }

        // Because we want the damage tooltip to show our custom damage, we need to modify it
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            /*
			// Get the vanilla damage tooltip
			TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Damage" && x.Mod == "Terraria");
			if (tt != null)
			{
				// We want to grab the last word of the tooltip, which is the translated word for 'damage' (depending on what language the player is using)
				// So we split the string by whitespace, and grab the last word from the returned arrays to get the damage word, and the first to get the damage shown in the tooltip
				string[] splitText = tt.Text.Split(' ');
				string damageValue = splitText.First();
				string damageWord = splitText.Last();
				// Change the tooltip text
				tt.Text = damageValue + " mercenary " + damageWord;
			}
			*/

            /*
			if (Main.LocalPlayer.HeldItem.ModItem is not TF2WeaponNoAmmo)
			{
				tooltips.Add(new TooltipLine(Mod, "Clip size", $"Has a capacity of {maxAmmoClip}"));
			}
			*/
        }

        // Make sure you can't use the item if you don't have enough resource and then use 10 resource otherwise.
        public override bool CanUseItem(Player player)
        {
            var tf2Player = player.GetModPlayer<WeaponSystem>();
            /*
            if (magazine && tf2Player.ammoCurrent <= 0)
            {
                Item.autoReuse = false;
            }
            else if (magazine && tf2Player.ammoCurrent > 0)
            {
                Item.autoReuse = true;
            */
            if (tf2Player.ammoCurrent <= 0)
            {
                tf2Player.ReloadWeapon();
                return false;
            }
            return tf2Player.ammoCurrent >= ammoCost;
            //return false;
        }

        public void UpdateResource()
        {
            // A simple timer that goes up to 1 frame, increases the ammoCurrent by 1 and then resets back to 0.
            if (ammoReloadRateTimer > reloadRate && reload == true)
            {
                if (magazine)
                {
                    ammoInClip = maxAmmoClip;
                }
                else
                {
                    ammoInClip += 1;
                }
                ammoReloadRateTimer = 0;
            }

            // For our resource lets make it regen slowly over time to keep it simple, let's use ammoReloadRateTimer to count up to whatever value we want, then increase currentResource.
            if (reload == true)
            {
                ammoReloadRateTimer++; //Increase it by 60 per second, or 1 per tick.
            }

            // Limit ammoCurrent from going over the limit imposed by ammoMax.
            ammoInClip = Utils.Clamp(ammoInClip, 0, maxAmmoClip);

            if (ammoInClip == maxAmmoClip)
            {
                StopReload();
            }
        }

        public void ReloadWeapon()
        {
            reload = true;
        }

        public void StopReload()
        {
            reload = false;
        }
    }

    public abstract class TF2WeaponNoAmmo : TF2Weapon
    {
        //So weapons without ammo won't render the ammo ui
    }
}
