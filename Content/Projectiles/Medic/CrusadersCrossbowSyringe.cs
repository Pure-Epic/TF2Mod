using Terraria;
using Terraria.ID;
using TF2.Common;

namespace TF2.Content.Projectiles.Medic
{
    public class CrusadersCrossbowSyringe : Syringe
    {
        public override string Texture => "TF2/Content/Projectiles/Medic/Syringe";

        public int Timer
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public bool[] healedNPC = new bool[Main.maxNPCs];
        public bool[] healedPlayer = new bool[Main.maxPlayers];

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 8;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            AIType = ProjectileID.WoodenArrowFriendly;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.GetGlobalProjectile<TF2ProjectileBase>().healingProjectile = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            foreach (NPC npc in Main.npc)
            {
                if (Projectile.Hitbox.Intersects(npc.Hitbox) && npc.friendly && npc.active && !healedNPC[npc.whoAmI])
                {
                    int healingAmount = (int)(npc.lifeMax * 0.25f);
                    npc.life += healingAmount;
                    npc.HealEffect(healingAmount);
                    healedNPC[npc.whoAmI] = true;
                    npc.netUpdate = true;
                    Projectile.penetrate--;
                }
            }
            foreach (Player player in Main.player)
            {
                if (Projectile.Hitbox.Intersects(player.Hitbox) && player.whoAmI != Projectile.owner && player.active && !healedPlayer[player.whoAmI])
                {
                    TF2Player p = player.GetModPlayer<TF2Player>();
                    int healingAmount = (int)(75 * player.statLifeMax2 / 500 * p.healReduction);
                    player.Heal(healingAmount);
                    healedPlayer[player.whoAmI] = true;
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendData(MessageID.PlayerHeal, number: player.whoAmI, number2: healingAmount);
                    Projectile.penetrate--;
                }
            }
        }
    }
}