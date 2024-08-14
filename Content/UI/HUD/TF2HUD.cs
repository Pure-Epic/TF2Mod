using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace TF2.Content.UI.HUD
{
    [Autoload(Side = ModSide.Client)]
    public abstract class TF2HUD : UIState
    {
        protected static Player Player => Main.LocalPlayer;

        protected virtual bool CanDisplay => false;

        protected virtual string Texture => "TF2/Content/Textures/Nothing";

        protected UIElement area;
        protected UIImage texture;
        private bool updating;

        protected virtual void HUDCreate()
        {
            Width = StyleDimension.Fill;
            Height = StyleDimension.Fill;
            HUDPreInitialize(out UIElement _area, out UIImage _texture);
            area = _area;
            texture = _texture;
            if (texture != null)
                area.Append(texture);
            Append(area);
            HUDPostInitialize(area);
        }

        protected virtual void HUDPreInitialize(out UIElement _area, out UIImage _texture)
        {
            _area = new UIElement();
            _texture = new UIImage(ModContent.Request<Texture2D>(Texture, AssetRequestMode.ImmediateLoad).Value);
        }

        protected virtual void HUDPostInitialize(UIElement area)
        { }

        protected virtual void HUDDraw(SpriteBatch spriteBatch)
        { }

        protected virtual void HUDUpdate(GameTime gameTime)
        { }

        public sealed override void OnInitialize() => HUDCreate();

        public sealed override void Draw(SpriteBatch spriteBatch)
        {
            if (CanDisplay && updating && !TF2.MannCoStoreActive)
                base.Draw(spriteBatch);
        }

        protected sealed override void DrawSelf(SpriteBatch spriteBatch)
        {
            HUDDraw(spriteBatch);
            base.DrawSelf(spriteBatch);
        }

        public sealed override void Update(GameTime gameTime)
        {
            if (CanDisplay && !TF2.MannCoStoreActive)
            {
                updating = true;
                HUDUpdate(gameTime);
                base.Update(gameTime);
            }
        }
    }

    public class HUDTextures : ModSystem
    {
        public static Asset<Texture2D>[] PDATextures;
        public static Asset<Texture2D>[] SentryHUDTextures;
        public static Asset<Texture2D>[] DispenserHUDTextures;
        public static Asset<Texture2D>[] TeleporterHUDTextures;

        public static Texture2D SentryIcon => PDATextures[0].Value;

        public static Texture2D DispenserIcon => PDATextures[1].Value;

        public static Texture2D TeleporterEntranceIcon => PDATextures[2].Value;

        public static Texture2D TeleporterExitIcon => PDATextures[3].Value;

        public static Texture2D DestructionPDABackground => PDATextures[4].Value;

        public static Texture2D SentryHUD => SentryHUDTextures[0].Value;

        public static Texture2D SentryLevel1HUDInitial => SentryHUDTextures[1].Value;

        public static Texture2D MiniSentryHUDInitial => SentryHUDTextures[2].Value;

        public static Texture2D SentryLevel1HUD => SentryHUDTextures[3].Value;

        public static Texture2D SentryLevel2HUD => SentryHUDTextures[4].Value;

        public static Texture2D SentryLevel3HUD => SentryHUDTextures[5].Value;

        public static Texture2D MiniSentryHUD => SentryHUDTextures[6].Value;

        public static Texture2D DispenserHUD => DispenserHUDTextures[0].Value;

        public static Texture2D DispenserLevel1HUDInitial => DispenserHUDTextures[1].Value;

        public static Texture2D DispenserLevel1HUD => DispenserHUDTextures[2].Value;

        public static Texture2D DispenserLevel2HUD => DispenserHUDTextures[3].Value;

        public static Texture2D DispenserLevel3HUD => DispenserHUDTextures[4].Value;

        public static Texture2D TeleporterEntranceHUD => TeleporterHUDTextures[0].Value;

        public static Texture2D TeleporterExitHUD => TeleporterHUDTextures[1].Value;

        public static Texture2D TeleporterEntranceHUDInitial => TeleporterHUDTextures[2].Value;

        public static Texture2D TeleporterExitHUDInitial => TeleporterHUDTextures[3].Value;

        public static Texture2D TeleporterEntranceLevel1HUD => TeleporterHUDTextures[4].Value;

        public static Texture2D TeleporterExitLevel1HUD => TeleporterHUDTextures[5].Value;

        public static Texture2D TeleporterEntranceLevel3HUD => TeleporterHUDTextures[6].Value;

        public static Texture2D TeleporterExitLevel3HUD => TeleporterHUDTextures[7].Value;

        public override void Load()
        {
            PDATextures =
            [
                ModContent.Request<Texture2D>("TF2/Content/Textures/UI/HUD/SentryGun"),
                ModContent.Request<Texture2D>("TF2/Content/NPCs/Buildings/Dispenser/DispenserLevel1"),
                ModContent.Request<Texture2D>("TF2/Content/Textures/UI/HUD/TeleporterEntrance"),
                ModContent.Request<Texture2D>("TF2/Content/Textures/UI/HUD/TeleporterExit"),
                ModContent.Request<Texture2D>("TF2/Content/Textures/UI/HUD/DestructionPDABackground")
            ];
            SentryHUDTextures =
            [
                ModContent.Request<Texture2D>("TF2/Content/Textures/UI/HUD/SentryHUD"),
                ModContent.Request<Texture2D>("TF2/Content/Textures/UI/HUD/SentryLevel1HUDInitial"),
                ModContent.Request<Texture2D>("TF2/Content/Textures/UI/HUD/MiniSentryHUDInitial"),
                ModContent.Request<Texture2D>("TF2/Content/Textures/UI/HUD/SentryLevel1HUD"),
                ModContent.Request<Texture2D>("TF2/Content/Textures/UI/HUD/SentryLevel2HUD"),
                ModContent.Request<Texture2D>("TF2/Content/Textures/UI/HUD/SentryLevel3HUD"),
                ModContent.Request<Texture2D>("TF2/Content/Textures/UI/HUD/MiniSentryHUD")
            ];
            DispenserHUDTextures =
            [
                ModContent.Request<Texture2D>("TF2/Content/Textures/UI/HUD/DispenserHUD"),
                ModContent.Request<Texture2D>("TF2/Content/Textures/UI/HUD/DispenserLevel1HUDInitial"),
                ModContent.Request<Texture2D>("TF2/Content/Textures/UI/HUD/DispenserLevel1HUD"),
                ModContent.Request<Texture2D>("TF2/Content/Textures/UI/HUD/DispenserLevel2HUD"),
                ModContent.Request<Texture2D>("TF2/Content/Textures/UI/HUD/DispenserLevel3HUD")
            ];
            TeleporterHUDTextures =
            [
                ModContent.Request<Texture2D>("TF2/Content/Textures/UI/HUD/TeleporterEntranceHUD"),
                ModContent.Request<Texture2D>("TF2/Content/Textures/UI/HUD/TeleporterExitHUD"),
                ModContent.Request<Texture2D>("TF2/Content/Textures/UI/HUD/TeleporterEntranceHUDInitial"),
                ModContent.Request<Texture2D>("TF2/Content/Textures/UI/HUD/TeleporterExitHUDInitial"),
                ModContent.Request<Texture2D>("TF2/Content/Textures/UI/HUD/TeleporterEntranceLevel1HUD"),
                ModContent.Request<Texture2D>("TF2/Content/Textures/UI/HUD/TeleporterExitLevel1HUD"),
                ModContent.Request<Texture2D>("TF2/Content/Textures/UI/HUD/TeleporterEntranceLevel3HUD"),
                ModContent.Request<Texture2D>("TF2/Content/Textures/UI/HUD/TeleporterExitLevel3HUD")
            ];
        }
    }
}