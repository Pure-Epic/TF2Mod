using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Items.Modules
{
    public class NinjaModule : TF2Module
    {
        public override bool Passive => true;

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            WeaponAddQuality(Unique);
            noThe = true;
            availability = Purchase;
        }

        protected override void WeaponDescription(List<TooltipLine> tooltips)
        {
            AddPositiveAttribute(tooltips);
            AddNeutralAttribute(tooltips);
        }

        protected override void ModuleUpdate(Player player)
        {
            player.GetModPlayer<NinjaModulePlayer>().ninjaModuleEquipped = true;
            player.GetModPlayer<TF2Player>().extraJumps += 1;
            player.spikedBoots = 2;
            player.autoJump = true;
            player.GetJumpState<TF2DoubleJump>().Enable();
        }
    }

    public class NinjaModulePlayer : ModPlayer
    {
        public bool ninjaModuleEquipped;
        public int dashDirection;
        public int dashTimer;

        public override void ResetEffects()
        {
            ninjaModuleEquipped = false;
            if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[2] < TF2.Time(0.25) && Player.doubleTapCardinalTimer[3] == 0)
                dashDirection = 2;
            else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[3] < TF2.Time(0.25) && Player.doubleTapCardinalTimer[2] == 0)
                dashDirection = 3;
            else
                dashDirection = 0;
        }

        public override void PreUpdateMovement()
        {
            if (ninjaModuleEquipped && !Player.mount.Active && dashDirection != 0 && dashTimer >= TF2.Time(1))
            {
                Vector2 newVelocity = Player.velocity;
                switch (dashDirection)
                {
                    case 2:
                    case 3:
                        newVelocity.X = (dashDirection == 2 ? 1f : -1f) * 10f;
                        break;
                    default:
                        break;
                }
                dashTimer = 0;
                Player.velocity = newVelocity;
            }
            dashTimer++;
        }
    }
}