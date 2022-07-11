using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TF2.Buffs
{
    // This class serves as an example of a debuff that causes constant loss of life
    // See ExampleLifeRegenDebuffPlayer.UpdateBadLifeRegen at the end of the file for more information
    public class Cloaked : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cloaked"); // Buff display name
            Description.SetDefault("Invincible and invisible"); // Buff description
            Main.buffNoSave[Type] = true; // Causes this buff not to persist when exiting and rejoining the world
        }

        // Allows you to make this buff give certain effects to the given player
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<CloakPlayer>().cloakBuff = true;
        }
    }

    public class CloakCooldown : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cloak Cooldown"); // Buff display name
            Description.SetDefault("You cannot cloak"); // Buff description
            Main.debuff[Type] = true;  // Is it a debuff?
            Main.buffNoSave[Type] = true; // Causes this buff not to persist when exiting and rejoining the world
        }

        // Allows you to make this buff give certain effects to the given player
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<TFClass>().cloakCooldown = true;
            player.GetModPlayer<CloakPlayer>().playerID = player.whoAmI;
        }
    }

    public class CloakPlayer : ModPlayer
    {
        public bool cloakBuff;
        public int playerID;

        public override void ResetEffects()
        {
            cloakBuff = false;
            Player p = Main.player[playerID];
            p.creativeGodMode = false;
            p.opacityForCreditsRoll = 1f;
        }

        public override void PostUpdate()
        {
            if (cloakBuff)
            {
                Player p = Main.player[playerID];
                p.creativeGodMode = true;
                p.opacityForCreditsRoll = 0.5f;
            }
        }

        public override void OnHitAnything(float x, float y, Entity victim)
        {
            if (cloakBuff)
            {
                Player p = Main.player[playerID];
                int buffIndex = p.FindBuffIndex(ModContent.BuffType<Cloaked>());
                if (p.buffTime[buffIndex] > 0) //prevents index out of range exceptions
                {
                    p.buffTime[buffIndex] -= 150;
                }
            }
        }
    }
}