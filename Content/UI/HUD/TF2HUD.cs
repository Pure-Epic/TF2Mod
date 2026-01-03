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

        protected virtual Asset<Texture2D> Texture { get; private set; }

        protected UIElement area;
        private bool updating;

        protected virtual void HUDCreate()
        {
            Width = StyleDimension.Fill;
            Height = StyleDimension.Fill;
            HUDPreInitialize(out UIElement _area, out UIImage _texture);
            area = _area;
            Append(area);
            if (_texture != null)
                area.Append(_texture);
            HUDPostInitialize(area);
        }

        protected virtual void HUDPreInitialize(out UIElement _area, out UIImage _texture)
        {
            _area = new UIElement();
            _texture = new UIImage(Texture);
        }

        protected virtual void HUDPostInitialize(UIElement area)
        { }

        protected virtual void HUDDraw(SpriteBatch spriteBatch)
        { }

        protected virtual void HUDUpdate(GameTime gameTime)
        { }

        protected virtual void HUDSilentUpdate(GameTime gameTime)
        { }

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
            RemoveAllChildren();
            HUDCreate();
            HUDSilentUpdate(gameTime);
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
        internal static Asset<Texture2D>[] WeaponHUDTextures { get; private set; }

        internal static Asset<Texture2D>[] PDAHUDTextures { get; private set; }

        internal static Asset<Texture2D>[] SentryHUDTextures { get; private set; }

        internal static Asset<Texture2D>[] DispenserHUDTextures { get; private set; }

        internal static Asset<Texture2D>[] TeleporterHUDTextures { get; private set; }

        public static Asset<Texture2D> AmmoHUDTexture => WeaponHUDTextures[0];

        public static Asset<Texture2D> AmmoChargeHUDTexture => WeaponHUDTextures[1];

        public static Asset<Texture2D> LeftChargeHUDTexture => WeaponHUDTextures[2];

        public static Asset<Texture2D> MiddleChargeHUDTexture => WeaponHUDTextures[3];

        public static Asset<Texture2D> RightChargeHUDTexture => WeaponHUDTextures[4];

        public static Asset<Texture2D> WeaponCounterHUDTexture => WeaponHUDTextures[5];

        public static Asset<Texture2D> StickybombAmountHUDTexture => WeaponHUDTextures[6];

        public static Asset<Texture2D> ShieldChargeHUDTexture => WeaponHUDTextures[7];

        public static Asset<Texture2D> MetalHUDTexture => WeaponHUDTextures[8];

        public static Asset<Texture2D> UberchargeHUDTexture => WeaponHUDTextures[9];

        public static Asset<Texture2D> OrganCounterHUDTexture => WeaponHUDTextures[10];

        public static Asset<Texture2D> KnifeHUDTexture => WeaponHUDTextures[11];

        public static Asset<Texture2D> SentryIcon => PDAHUDTextures[0];

        public static Asset<Texture2D> DispenserIcon => PDAHUDTextures[1];

        public static Asset<Texture2D> TeleporterEntranceIcon => PDAHUDTextures[2];

        public static Asset<Texture2D> TeleporterExitIcon => PDAHUDTextures[3];

        public static Asset<Texture2D> DestructionPDABackground => PDAHUDTextures[4];

        public static Asset<Texture2D> SentryHUD => SentryHUDTextures[0];

        public static Asset<Texture2D> SentryLevel1HUDInitial => SentryHUDTextures[1];

        public static Asset<Texture2D> MiniSentryHUDInitial => SentryHUDTextures[2];

        public static Asset<Texture2D> SentryLevel1HUD => SentryHUDTextures[3];

        public static Asset<Texture2D> SentryLevel2HUD => SentryHUDTextures[4];

        public static Asset<Texture2D> SentryLevel3HUD => SentryHUDTextures[5];

        public static Asset<Texture2D> MiniSentryHUD => SentryHUDTextures[6];

        public static Asset<Texture2D> DispenserHUD => DispenserHUDTextures[0];

        public static Asset<Texture2D> DispenserLevel1HUDInitial => DispenserHUDTextures[1];

        public static Asset<Texture2D> DispenserLevel1HUD => DispenserHUDTextures[2];

        public static Asset<Texture2D> DispenserLevel2HUD => DispenserHUDTextures[3];

        public static Asset<Texture2D> DispenserLevel3HUD => DispenserHUDTextures[4];

        public static Asset<Texture2D> TeleporterEntranceHUD => TeleporterHUDTextures[0];

        public static Asset<Texture2D> TeleporterExitHUD => TeleporterHUDTextures[1];

        public static Asset<Texture2D> TeleporterEntranceHUDInitial => TeleporterHUDTextures[2];

        public static Asset<Texture2D> TeleporterExitHUDInitial => TeleporterHUDTextures[3];

        public static Asset<Texture2D> TeleporterEntranceLevel1HUD => TeleporterHUDTextures[4];

        public static Asset<Texture2D> TeleporterExitLevel1HUD => TeleporterHUDTextures[5];

        public static Asset<Texture2D> TeleporterEntranceLevel3HUD => TeleporterHUDTextures[6];

        public static Asset<Texture2D> TeleporterExitLevel3HUD => TeleporterHUDTextures[7];

        public static Asset<Texture2D> ItemDropIcon { get; private set; }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                WeaponHUDTextures =
                [
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/HUD/AmmoHUD"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/HUD/AmmoChargeMeterHUD"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/HUD/LeftChargeMeterHUD"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/HUD/MiddleChargeMeterHUD"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/HUD/RightChargeMeterHUD"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/HUD/CounterHUD"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/HUD/StickybombAmountHUD"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/HUD/ShieldChargeMeterHUD"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/HUD/MetalHUD"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/HUD/UberchargeHUD"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/HUD/OrganCounterHUD"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/HUD/KnifeHUD")
                ];
                PDAHUDTextures =
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
                ItemDropIcon = ModContent.Request<Texture2D>("TF2/Content/Textures/UI/Inventory/Alert");
            }
        }
    }
}