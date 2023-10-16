using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;

namespace TF2.Content.Projectiles.Pyro
{
    public class Flare : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 10; // The width of projectile hitbox
            Projectile.height = 10; // The height of projectile hitbox
            Projectile.aiStyle = 1; // The ai style of the projectile, please reference the source code of Terraria
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.penetrate = 1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = true; // Can the projectile collide with tiles?
            Projectile.extraUpdates = 1; // Set to above 0 if you want the projectile to update multiple time in a frame
            Projectile.knockBack = 0f;
            Projectile.damage = 75;
            Projectile.scale = 0.75f;
            AIType = ProjectileID.WoodenArrowFriendly;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool PreDraw(ref Color lightColor) => TF2.DrawProjectile(Projectile, ref lightColor);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            TF2Player p = Main.player[Projectile.owner].GetModPlayer<TF2Player>();
            PyroFlamesNPC npc = target.GetGlobalNPC<PyroFlamesNPC>();
            npc.damageMultiplier = p.classMultiplier;
            TF2.ExtinguishPyroFlames(target, ModContent.BuffType<PyroFlamesDegreaser>());
            target.AddBuff(ModContent.BuffType<PyroFlames>(), 450);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            TF2Player p = Main.player[Projectile.owner].GetModPlayer<TF2Player>();
            PyroFlamesPlayer burntPlayer = target.GetModPlayer<PyroFlamesPlayer>();
            burntPlayer.damageMultiplier = p.classMultiplier;
            target.ClearBuff(ModContent.BuffType<PyroFlamesDegreaser>());
            target.AddBuff(ModContent.BuffType<PyroFlames>(), 450);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.HasBuff(ModContent.BuffType<PyroFlames>()) || target.HasBuff(ModContent.BuffType<PyroFlamesDegreaser>()))
                Main.player[Projectile.owner].GetModPlayer<TF2Player>().crit = true;
        }

        public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation();
    }
}