using Microsoft.Xna.Framework.Input;
using System.Linq;
using Terraria;
using TF2.Common;

namespace TF2.Content.Items.Weapons.Engineer
{
    public class ConstructionPDA : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Engineer, PDA, Stock, Starter);
            SetWeaponSize(50, 50);
            SetPDAUseStyle();
        }

        public override bool WeaponCanBeUsed(Player player) => false;

        protected override void WeaponActiveUpdate(Player player)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (p.currentClass == Engineer)
            {
                Keys[] keybind = [Keys.D1, Keys.D2, Keys.D3, Keys.D4];
                bool keyDown = false;
                for (int i = 0; i < 4; i++)
                {
                    if (Main.keyState.GetPressedKeys().Contains(keybind[i]))
                    {
                        keyDown = true;
                        break;
                    }
                }
                p.lockPDA = keyDown;
            }
            base.WeaponActiveUpdate(player);
        }
    }
}