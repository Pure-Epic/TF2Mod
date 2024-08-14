using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Buffs
{
    public class QuickFixUberCharge : ModBuff
    {
        public override string Texture => "TF2/Content/Buffs/UberCharge";

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.pvpBuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<UberChargeNPC>().uberCharge = true;
            for (int i = 0; i < BuffLoader.BuffCount; i++)
            {
                if (Main.debuff[i])
                    npc.buffImmune[i] = true;
            }
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<QuickFixUberChargePlayer>().uberCharge = true;
            for (int i = 0; i < BuffLoader.BuffCount; i++)
            {
                if (Main.debuff[i] && !BuffID.Sets.NurseCannotRemoveDebuff[i] && !TF2BuffBase.cooldownBuff[i])
                    player.buffImmune[i] = true;
            }
        }

        public override bool RightClick(int buffIndex)
        {
            TF2Player p = Main.LocalPlayer.GetModPlayer<TF2Player>();
            p.uberChargeTime = 0;
            p.uberCharge = 0;
            p.activateUberCharge = false;
            return true;
        }
    }

    public class QuickFixUberChargePlayer : UberChargePlayer
    {
        public override void PostUpdate()
        {
            TF2Player p = Player.GetModPlayer<TF2Player>();
            if (uberCharge)
                SpawnDusts(Player);
            if (p.activateUberCharge && Player.HasBuff<QuickFixUberCharge>())
            {
                p.uberChargeTime++;
                p.uberChargeDuration = uberChargeDuration;
                if (p.uberChargeTime >= uberChargeDuration)
                {
                    p.uberChargeTime = 0;
                    p.uberCharge = 0;
                    p.activateUberCharge = false;
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/invulnerable_off"), Player.Center);
                }
            }
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (Player.HasBuff<QuickFixUberCharge>())
                modifiers.Knockback *= 0;
        }
    }

    public class QuickFixUberChargeNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public override void ModifyHitNPC(NPC npc, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (npc.HasBuff<QuickFixUberCharge>())
                modifiers.DisableKnockback();
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (npc.HasBuff<QuickFixUberCharge>())
                modifiers.DisableKnockback();
        }
    }
}