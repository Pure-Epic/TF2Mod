using Terraria.GameInput;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Items
{
    // This class mainly stores weapon UI information. It contains no weapon code (except for the reload hotkey).
    public class AmmoInterface : ModPlayer
    {
        public int ammoCurrent;
        public const int defaultAmmoMax = 100;
        public int ammoMax;
        public int ammoMax2;
        public bool startReload;

        public override void Initialize()
        {
            ammoMax = defaultAmmoMax;
            ammoCurrent = ammoMax;
        }

        public override void ResetEffects() => ResetVariables();

        public override void UpdateDead() => ResetVariables();

        private void ResetVariables() => ammoMax2 = ammoMax;

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (KeybindSystem.Reload.JustPressed && ammoCurrent != ammoMax)
                startReload = true;
        }
    }
}