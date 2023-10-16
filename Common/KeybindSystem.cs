using Terraria.ModLoader;

namespace TF2.Common
{
    // Acts as a container for keybinds registered by this mod.
    // See Common/Players/ExampleKeybindPlayer for usage.
    public class KeybindSystem : ModSystem
    {
        public static ModKeybind Cloak { get; private set; }

        public static ModKeybind Reload { get; private set; }

        public static ModKeybind SoldierBuff { get; private set; }

        public static ModKeybind ShieldCharge { get; private set; }

        public static ModKeybind HomingPower { get; private set; }

        public static ModKeybind MountSpeed { get; private set; }

        public override void Load()
        {
            // Registers a new keybind
            Cloak = KeybindLoader.RegisterKeybind(Mod, "Cloak", "F");
            Reload = KeybindLoader.RegisterKeybind(Mod, "Reload", "Mouse3");
            SoldierBuff = KeybindLoader.RegisterKeybind(Mod, "Buff Banner", "F");
            ShieldCharge = KeybindLoader.RegisterKeybind(Mod, "Shield Charge", "Mouse2");

            // Mount keybinds
            HomingPower = KeybindLoader.RegisterKeybind(Mod, "Hominh Distance", "I");
            MountSpeed = KeybindLoader.RegisterKeybind(Mod, "Mount Speed", "O");
        }

        // Please see ExampleMod.cs' Unload() method for a detailed explanation of the unloading process.
        public override void Unload()
        {
            // Not required if your AssemblyLoadContext is unloading properly, but nulling out static fields can help you figure out what's keeping it loaded.
            Cloak = null;
            Reload = null;
            SoldierBuff = null;
            ShieldCharge = null;

            HomingPower = null;
            MountSpeed = null;
        }
    }
}