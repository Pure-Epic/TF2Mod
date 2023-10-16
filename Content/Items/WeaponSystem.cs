using Terraria.GameInput;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Items
{
    // This class mainly stores weapon ui information. It contains no weapon code (except for the reload hotkey).
    public class WeaponSystem : ModPlayer
    {
        public int ammoCurrent;
        public const int defaultAmmoMax = 100;
        public int ammoMax;
        public int ammoMax2;
        public const float defaultReloadRate = 60f;
        public float ammoReloadRate;
        public float ammoReloadRate2;
        public float initialAmmoReloadRate;
        public float initialAmmoReloadRate2;
        public bool startReload;

        public override void Initialize()
        {
            ammoMax = defaultAmmoMax;
            ammoCurrent = ammoMax;
            ammoReloadRate = defaultReloadRate;
            initialAmmoReloadRate = defaultReloadRate;
        }

        public override void ResetEffects() => ResetVariables();

        public override void UpdateDead() => ResetVariables();

        private void ResetVariables()
        {
            ammoReloadRate2 = ammoReloadRate;
            initialAmmoReloadRate2 = initialAmmoReloadRate;
            ammoMax2 = ammoMax;
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (KeybindSystem.Reload.JustPressed && ammoCurrent != ammoMax)
                startReload = true;
        }
    }
}