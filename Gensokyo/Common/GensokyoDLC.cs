using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using ReLogic.Content;
using System;
using System.Reflection;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace TF2.Gensokyo.Common
{
    [ExtendsFromMod("Gensokyo")]
    public class GensokyoDLC : ModSystem
    {
        // Looking back at this before discontinuing this addon, the IL edits and boss AI were peak for when they were introduced
        // Archiving this code in case anyone wants to mess with despawning condition of some bosses
        public static Mod Gensokyo;
        public static readonly bool gensokyoLoaded = ModLoader.TryGetMod("Gensokyo", out Gensokyo);
        private MethodInfo RumiaLimits = null;
        private MethodInfo OkuuLimits = null;
        public static bool bossRush;
        public bool doPatch = true;

        public override void Load()
        {
            // Gensokyo DLC code by Rhoenicx
            // "My brain hurts" - Pure_Epic
            if (Gensokyo != null)
            {
                On_AWorldListItem.GetIconElement += Hook_GetIconElement;
                Type gensokyoRumia = null;
                Type gensokyoUtsuhoReiuji = null;
                Assembly gensokyoAssembly = Gensokyo.GetType().Assembly;
                foreach (Type t in gensokyoAssembly.GetTypes())
                {
                    if (t.Name == "Rumia")
                        gensokyoRumia = t;
                    if (t.Name == "UtsuhoReiuji")
                        gensokyoUtsuhoReiuji = t;
                }
                if (gensokyoRumia != null)
                    RumiaLimits = gensokyoRumia.GetMethod("AI", BindingFlags.Instance | BindingFlags.Public);
                // The original method is private, therefore mark it with BindingFlags.NonPublic
                if (gensokyoUtsuhoReiuji != null)
                    OkuuLimits = gensokyoUtsuhoReiuji.GetMethod("TargetIsValid_BossSpecific", BindingFlags.Instance | BindingFlags.NonPublic);
                if (RumiaLimits != null)
                    MonoModHooks.Modify(RumiaLimits, ILWeakReferences_ModifyRumiaLimits);
                if (OkuuLimits != null)
                    MonoModHooks.Modify(OkuuLimits, ILWeakReferences_ModifyOkuuLimits);
                Gensokyo.Call("RegisterShopAccess", Mod.Name);
            }
        }

        public override void Unload() => Gensokyo = null;

        public override void SaveWorldHeader(TagCompound tag) => tag["downedByakurenHijiri"] = true;

        public static void DrawLaser(Texture2D tex, Vector2 start, Vector2 end, Vector2 scale, Color color, int tailLength, int bodySegmentLength, int headLength)
        {
            Vector2 laserDir = Vector2.Normalize(end - start);
            float laserLength = (end - start).Length();
            float rotation = laserDir.ToRotation() - MathHelper.PiOver2;
            if (!laserDir.HasNaNs())
            {
                // Draw Laser Tail
                Rectangle sourceRectangle = new Rectangle(0, 0, tex.Width, tailLength);
                Vector2 origin = sourceRectangle.Size() / 2f;
                Main.EntitySpriteDraw(tex, start, sourceRectangle, color, rotation, sourceRectangle.Size() / 2f, scale, SpriteEffects.None);
                // Set initial values for the drawing of the Laser Body
                float distanceCovered = sourceRectangle.Height;
                float totalDistanceCovered = distanceCovered * scale.Y;
                Vector2 drawPosition = start + laserDir * (sourceRectangle.Height - origin.Y) * scale.Y;
                // Draw Laser Body
                while (totalDistanceCovered + 1 < laserLength)
                {
                    sourceRectangle = new Rectangle(0, tailLength + 2, tex.Width, bodySegmentLength);
                    distanceCovered = sourceRectangle.Height;
                    origin = new Vector2(sourceRectangle.Width / 2f, 0);
                    // The remaining laser length is shorter than a whole body segment
                    if (laserLength - totalDistanceCovered < sourceRectangle.Height)
                    {
                        distanceCovered *= (laserLength - totalDistanceCovered) / sourceRectangle.Height;
                        sourceRectangle.Height = (int)(laserLength - totalDistanceCovered + 1);
                    }
                    totalDistanceCovered += distanceCovered * scale.Y;
                    Main.EntitySpriteDraw(tex, drawPosition, sourceRectangle, color, rotation, origin, scale, SpriteEffects.None);
                    drawPosition += laserDir * distanceCovered * scale.Y;
                }
                // Draw Laser Head
                sourceRectangle = new Rectangle(0, tailLength + bodySegmentLength + 4, tex.Width, headLength);
                origin = new Vector2(sourceRectangle.Width / 2f, 0f);
                Main.EntitySpriteDraw(tex, drawPosition, sourceRectangle, color, rotation, origin, scale, SpriteEffects.None);
            }
        }

        // Since we are modifing a void, we cannot simply return a value
        private void ILWeakReferences_ModifyRumiaLimits(ILContext il)
        {
            if (doPatch)
            {
                ILCursor c = new ILCursor(il);
                ILLabel LabelKey = null;
                if (c.TryGotoNext(
                    x => x.MatchLdarg(0),
                    // The MatchCall being empty is not really a big deal since this piece of code is short anyway.
                    x => x.MatchCall(out _),
                    x => x.MatchLdcI4(99),
                    x => x.MatchBeq(out LabelKey)
                    ))
                {
                    // The reason why we increase the index by 4 is because by default, the cursor will be placed on the first line encountered by the TryGotoNext.
                    // In our case, we want to move past these lines to get inside the if statement.
                    int Index = c.Index + 4;
                    // For safety, we should actually also check the lines where we want to branch over, since we cannot guarantee this code stays the same in the future.
                    if (c.TryGotoNext(
                    x => x.MatchLdsfld<Main>("dayTime"),
                    x => x.MatchBrfalse(out LabelKey),
                    x => x.MatchLdsfld<Main>("eclipse"),
                    x => x.MatchBrtrue(out LabelKey),
                    x => x.MatchLdsfld<Main>("remixWorld"),
                    x => x.MatchLdcI4(0),
                    x => x.MatchCeq(),
                    x => x.MatchBr(out _),
                    x => x.MatchLdcI4(0),
                    x => x.MatchBrfalse(out _)
                    ))
                    {
                        c.Index = Index;
                        c.Emit(OpCodes.Br, LabelKey);
                    }
                }
            }
        }

        // This boolean returns true
        private void ILWeakReferences_ModifyOkuuLimits(ILContext il)
        {
            if (doPatch)
            {
                ILCursor c = new ILCursor(il);

                c.Emit(OpCodes.Ldc_I4_1);
                c.Emit(OpCodes.Ret);
            }
        }

        private UIElement Hook_GetIconElement(On_AWorldListItem.orig_GetIconElement orig, AWorldListItem self) => self.Data.TryGetHeaderData(this, out var data) && data.GetBool("downedByakurenHijiri") ? orig(self) : new UIImage(ModContent.Request<Texture2D>("TF2/Gensokyo/Content/Textures/FantasyModeIcon", AssetRequestMode.ImmediateLoad).Value)
        {
            Left = new StyleDimension(4f, 0f)
        };
    }
}