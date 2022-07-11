using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace TF2.Items
{
    // This class stores necessary player info for our custom damage class, such as damage multipliers, additions to knockback and crit, and our custom resource that governs the usage of the weapons of this damage class.
    public class WeaponSystem : ModPlayer
    {
        public static WeaponSystem ModPlayer(Player player)
        {
            return player.GetModPlayer<WeaponSystem>();
        }

        // Vanilla only really has damage multipliers in code
        // And crit and knockback is usually just added to
        // As a modder, you could make separate variables for multipliers and simple addition bonuses
        // Most likely depricated due to DamageClass
        public float tf2DamageAdd;
        public float tf2DamageMult = 1f;
        public float tf2Knockback;
        public int tf2Crit;

        // Here we include a custom resource, similar to mana or health.
        // Creating some variables to define the current value of our example resource as well as the current maximum value. We also include a temporary max value, as well as some variables to handle the natural regeneration of this resource.
        public int ammoCurrent;
        public const int defaultAmmoMax = 100;
        public int ammoMax;
        public int ammoMax2;
        public const float defaultReloadRate = 60f;
        public float ammoReloadRate;
        public float ammoReloadRate2;
        internal int ammoReloadRateTimer = 0;
        public static readonly Color reloadAmmo = new Color(187, 91, 201); // We can use this for CombatText, if you create an item that replenishes ammoCurrent.

        public bool reload = false; //We need to reload the weapon once its clip runs out.

        /*
		In order to make the Example Resource example straightforward, several things have been left out that would be needed for a fully functional resource similar to mana and health. 
		Here are additional things you might need to implement if you intend to make a custom resource:
		- Multiplayer Syncing: The current example doesn't require MP code, but pretty much any additional functionality will require this. ModPlayer.SendClientChanges and clientClone will be necessary, as well as SyncPlayer if you allow the user to increase ammoMax.
		- Save/Load and increased max resource: You'll need to implement Save/Load to remember increases to your ammoMax cap.
		- Resouce replenishment item: Use GlobalNPC.NPCLoot to drop the item. ModItem.OnPickup and ModItem.ItemSpace will allow it to behave like Mana Star or Heart. Use code similar to Player.HealEffect to spawn (and sync) a colored number suitable to your resource.
		*/

        public override void Initialize()
        {
            ammoMax = defaultAmmoMax;
            ammoCurrent = ammoMax;
            ammoReloadRate = defaultReloadRate;
        }

        public override void ResetEffects()
        {
            ResetVariables();
        }

        public override void UpdateDead()
        {
            ResetVariables();
        }

        private void ResetVariables()
        {
            tf2DamageAdd = 0f;
            tf2DamageMult = 1f;
            tf2Knockback = 0f;
            tf2Crit = 0;
            ammoReloadRate2 = ammoReloadRate;
            ammoMax2 = ammoMax;
        }

        public override void PostUpdateMiscEffects()
        {
            UpdateResource();
        }

        // Lets do all our logic for the custom resource here, such as limiting it, increasing it and so on.
        private void UpdateResource()
        {
            // A simple timer that goes up to 1 frame, increases the ammoCurrent by 1 and then resets back to 0.
            if (ammoReloadRateTimer > ammoReloadRate2 && reload == true)
            {
                ammoCurrent += 1;
                ammoReloadRateTimer = 0;
            }

            // For our resource lets make it regen slowly over time to keep it simple, let's use ammoReloadRateTimer to count up to whatever value we want, then increase currentResource.
            if (reload == true)
            {
                ammoReloadRateTimer++; //Increase it by 60 per second, or 1 per tick.
            }

            // Limit ammoCurrent from going over the limit imposed by ammoMax.
            ammoCurrent = Utils.Clamp(ammoCurrent, 0, ammoMax2);

            if (ammoCurrent == ammoMax)
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
}
