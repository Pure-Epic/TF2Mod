using Microsoft.Xna.Framework;
using Terraria;
using TF2.Content.Items.Weapons.Demoman;

namespace TF2.Content.Projectiles.Demoman
{
    public class ScottishResistanceStickybomb : Stickybomb
    {
        protected override void ProjectileAI()
        {
            if (Timer >= TF2.Time(5))
                noDistanceModifier = true;
            if (Timer >= 103 && TF2.FindNPC(Projectile, 50f))
            {
                Projectile.timeLeft = 0;
                Timer = 0;
                Player.GetModPlayer<ScottishResistancePlayer>().stickybombsReturned++;
                for (int i = 0; i < Player.inventory.Length; i++)
                {
                    if (Player.inventory[i].ModItem is ScottishResistance thisWeapon && weapon == thisWeapon)
                        (weapon as ScottishResistance).ReturnStickybombs(Player);
                }
            }
            if (Projectile.timeLeft == 0)
            {
                Projectile.position = Projectile.Center;
                Projectile.Size = new Vector2(250, 250);
                Projectile.friendly = true;
                Projectile.hide = true;
                Projectile.tileCollide = false;
                Projectile.Center = Projectile.position;
                StickyJump(velocity);
            }
            if (!Stick && Projectile.timeLeft != 0)
                StartingAI();
            else
                GroundAI();
            foreach (NPC npc in Main.npc)
            {
                if (Projectile.Hitbox.Intersects(npc.Hitbox) && npc.boss && !npc.friendly && npc.active && !npc.dontTakeDamage)
                {
                    TargetWhoAmI = npc.whoAmI;
                    Stick = true;
                    StickOnEnemy = true;
                    Projectile.netUpdate = true;
                }
            }
        }
    }
}