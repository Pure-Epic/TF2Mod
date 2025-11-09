using Terraria.ModLoader;

namespace TF2.Common
{
    // Acts as a container for keybinds registered by this mod.
    // See Common/Players/ExampleKeybindPlayer for usage.
    public class KeybindSystem : ModSystem
    {
        public static ModKeybind Reload { get; private set; }

        public static ModKeybind SoldierBuff { get; private set; }

        public static ModKeybind ShieldCharge { get; private set; }

        public static ModKeybind MoveBuilding { get; private set; }

        public static ModKeybind Cloak { get; private set; }

        public static ModKeybind ModulePower { get; private set; }

        public static ModKeybind ModuleSecondaryPower { get; private set; }

        public override void Load()
        {
            Reload = KeybindLoader.RegisterKeybind(Mod, "Reload", "Mouse3");
            SoldierBuff = KeybindLoader.RegisterKeybind(Mod, "Buff Banner", "F");
            ShieldCharge = KeybindLoader.RegisterKeybind(Mod, "Shield Charge", "Mouse2");
            MoveBuilding = KeybindLoader.RegisterKeybind(Mod, "Move Building", "B");
            Cloak = KeybindLoader.RegisterKeybind(Mod, "Cloak", "F");
            ModulePower = KeybindLoader.RegisterKeybind(Mod, "Module Power", "I");
            ModuleSecondaryPower = KeybindLoader.RegisterKeybind(Mod, "Module Secondary Power", "O");
        }

        public override void Unload()
        {
            Reload = null;
            SoldierBuff = null;
            ShieldCharge = null;
            MoveBuilding = null;
            Cloak = null;
            ModulePower = null;
            ModuleSecondaryPower = null;
        }
    }
}