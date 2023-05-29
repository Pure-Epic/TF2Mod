using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria.ModLoader;

namespace TF2.Gensokyo.Content.NPCs
{
    [ExtendsFromMod("Gensokyo")]
    public class GensokyoBossHealthBar : ModBossBar
    {
        public static readonly Asset<Texture2D> Divider = ModContent.Request<Texture2D>("Gensokyo/UI/Assets/BossBarDivider");

        private int bossHeadIndex = -1;

        public override Asset<Texture2D> GetIconTexture(ref Rectangle? iconFrame)
        {
            // Display the previously assigned head index
            if (bossHeadIndex != -1)
                return TextureAssets.NpcHeadBoss[bossHeadIndex];
            return null;
        }

        public override bool? ModifyInfo(ref BigProgressBarInfo info, ref float lifePercent, ref float shieldPercent)
        {
            NPC npc = Main.npc[info.npcIndexToAimAt];
            if (!npc.active)
                return false;
            // We assign bossHeadIndex here because we need to use it in GetIconTexture
            bossHeadIndex = npc.GetBossHeadTextureIndex();
            return null;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, NPC npc, ref BossBarDrawParams drawParams)
        {
            // Don't know
            return true;
        }

        public override void PostDraw(SpriteBatch spriteBatch, NPC npc, BossBarDrawParams drawParams)
        {
            if (npc.ModNPC is not GensokyoBoss boss || Divider == null) return;
            float dividerWidth = drawParams.BarCenter.X - Divider.Value.Width / 2f - 1f;
            float dividerLength = drawParams.BarCenter.Y - Divider.Value.Height / 2f;
            for (int i = 0; i < boss.spellCardAmount - 1; i++)
            {
                spriteBatch.Draw(Divider.Value, new Vector2(MathHelper.Lerp(dividerWidth - 226f, dividerWidth + 226f, (i + 1) / (float)boss.spellCardAmount), dividerLength - 2f), Color.White);
            }
        }
    }
}
