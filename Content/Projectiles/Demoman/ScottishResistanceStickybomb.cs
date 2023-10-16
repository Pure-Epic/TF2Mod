using Terraria;
using TF2.Content.Items.Demoman;

namespace TF2.Content.Projectiles.Demoman
{
    public class ScottishResistanceStickybomb : Stickybomb
    {
        public override void AI()
        {
            Timer++;
            if (Timer >= 103 && TF2.FindNPC(Projectile, 50f))
            {
                Projectile.timeLeft = 0;
                Timer = 0;
                Main.player[Projectile.owner].GetModPlayer<ScottishResistancePlayer>().stickybombsReturned++;
                for (int i = 0; i < Main.player[Projectile.owner].inventory.Length; i++)
                {
                    if (Main.player[Projectile.owner].inventory[i].ModItem is ScottishResistance weapon && owner == weapon)
                    {
                        ScottishResistance scottishResistance = weapon;
                        scottishResistance.ReturnStickybombs(Main.player[Projectile.owner]);
                    }
                }
            }
            if (Projectile.timeLeft == 0)
            {
                Projectile.friendly = true;
                Projectile.tileCollide = false;
                Projectile.alpha = 255;
                Projectile.position = Projectile.Center;
                Projectile.width = 250;
                Projectile.height = 250;
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