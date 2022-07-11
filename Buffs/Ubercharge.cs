using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TF2.Buffs
{
	public class Ubercharge : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ubercharge");
			Description.SetDefault("I am bulletproof!");
			Main.buffNoSave[Type] = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.GetGlobalNPC<UberchargeNPC>().ubercharge = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<UberchargePlayer>().ubercharge = true;
		}
	}

	public class UberchargePlayer : ModPlayer
	{
		public bool ubercharge;

		public override void ResetEffects()
		{
			ubercharge = false;
			Player.creativeGodMode = false;
		}

		// Allows you to give the player a negative life regeneration based on its state (for example, the "On Fire!" debuff makes the player take damage-over-time)
		// This is typically done by setting player.lifeRegen to 0 if it is positive, setting player.lifeRegenTime to 0, and subtracting a number from player.lifeRegen
		// The player will take damage at a rate of half the number you subtract per second
		public override void PostUpdate()
		{
			if (ubercharge)
			{
				Player.creativeGodMode = true;
				SpawnDusts(Player);
			}
		}

		public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
		{
			if (proj.Name == "HealingBeam")
			{
				damage = -1;
				crit = false;
			}
		}

		private void SpawnDusts(Player player)
		{
			for (int i = 0; i < 2; ++i)
			{
				Dust dust = Main.dust[Dust.NewDust(player.position, 8, 8, DustID.Clentaminator_Red, 0.0f, 0.0f, 100, new Color(), 1.5f)];
				dust.velocity *= 0.5f;
				dust.velocity.Y = -Math.Abs(dust.velocity.Y);
			}
		}
	}

	public class UberchargeNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;

		public int timer = 0;

		public bool ubercharge;

		public override void ResetEffects(NPC npc)
		{
			ubercharge = false;
			npc.immortal = false;
		}

		public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			if (projectile.Name == "HealingBeam")
			{
				damage = -1;
				crit = false;
				timer = 0;
			}
		}

		public override void AI(NPC npc)
		{
			timer++;
			if (npc.life > npc.lifeMax && timer > 50)
			{
				npc.life -= 1;
				timer = 0;
			}
			if (ubercharge)
			{
				npc.immortal = true;
				SpawnDusts(npc);
			}
		}

		private void SpawnDusts(NPC npc)
		{
			for (int i = 0; i < 2; ++i)
			{
				Dust dust = Main.dust[Dust.NewDust(npc.position, 8, 8, DustID.Clentaminator_Red, 0.0f, 0.0f, 100, new Color(), 1.5f)];
				dust.velocity *= 0.5f;
				dust.velocity.Y = -Math.Abs(dust.velocity.Y);
			}
		}


	}
}
