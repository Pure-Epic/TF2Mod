using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using TF2.Common;
using TF2.Content.Items;
using TF2.Content.Items.Modules;
using TF2.Content.Items.Weapons;
using TF2.Content.Items.Weapons.Engineer;
using TF2.Content.NPCs.Buildings;
using TF2.Content.NPCs.Buildings.Dispenser;
using TF2.Content.NPCs.Buildings.SentryGun;
using TF2.Content.NPCs.Buildings.Teleporter;

namespace TF2.Content.UI.HUD.Engineer
{
    [Autoload(Side = ModSide.Client)]
    internal class ConstructionPDAHUD : TF2HUD
    {
        protected override bool CanDisplay => Player.HeldItem.ModItem is ConstructionPDA && Player.GetModPlayer<TF2Player>().currentClass == TF2Item.Engineer;

        protected override void HUDCreate()
        {
            RemoveAllChildren();
            UIPanel mainPanel = new UIPanel
            {
                Width = StyleDimension.FromPixels(825f),
                Height = StyleDimension.FromPixels(240f),
                HAlign = 0.5f,
                VAlign = 0.5f,
                BackgroundColor = new Color(17, 14, 12, 64),
                BorderColor = new Color(3, 3, 3)
            };
            mainPanel.SetPadding(0f);
            Append(mainPanel);
            UIElement uIElement = new UIElement
            {
                Width = StyleDimension.FromPixels(765f),
                Height = StyleDimension.FromPixels(180f),
                HAlign = 0.5f,
                VAlign = 0.5f,
            };
            uIElement.SetPadding(0f);
            mainPanel.Append(uIElement);
            ConstructionPDAPanel sentryPanel = new ConstructionPDAPanel(HUDTextures.SentryIcon.Value, 130, 1)
            {
                Width = StyleDimension.FromPixels(180f),
                Height = MaxHeight,
                HAlign = 0f,
                VAlign = 0.5f
            };
            sentryPanel.SetPadding(0f);
            uIElement.Append(sentryPanel);
            ConstructionPDAPanel dispenserPanel = new ConstructionPDAPanel(HUDTextures.DispenserIcon.Value, 100, 2)
            {
                Width = StyleDimension.FromPixels(180f),
                Height = MaxHeight,
                HAlign = 0.333f,
                VAlign = 0.5f
            };
            dispenserPanel.SetPadding(0f);
            uIElement.Append(dispenserPanel);
            ConstructionPDAPanel teleporterEntrancePanel = new ConstructionPDAPanel(HUDTextures.TeleporterEntranceIcon.Value, 50, 3)
            {
                Width = StyleDimension.FromPixels(180f),
                Height = MaxHeight,
                HAlign = 0.666f,
                VAlign = 0.5f
            };
            teleporterEntrancePanel.SetPadding(0f);
            uIElement.Append(teleporterEntrancePanel);
            ConstructionPDAPanel teleporterExitPanel = new ConstructionPDAPanel(HUDTextures.TeleporterExitIcon.Value, 50, 4)
            {
                Width = StyleDimension.FromPixels(180f),
                Height = MaxHeight,
                HAlign = 1f,
                VAlign = 0.5f
            };
            teleporterExitPanel.SetPadding(0f);
            uIElement.Append(teleporterExitPanel);
        }
    }

    [Autoload(Side = ModSide.Client)]
    internal class ConstructionPDAPanel : UIState
    {
        public Texture2D buildingTexture;
        public Color BackgroundColor = Color.White;
        private UIText costText;
        private UIText keyText;
        public int cost;
        private float costReduction;
        private readonly int key;

        public ConstructionPDAPanel(Texture2D texture, int metalCost, int keybind)
        {
            buildingTexture = texture;
            cost = metalCost;
            key = keybind;
            BuildPage();
        }

        private void BuildPage()
        {
            RemoveAllChildren();
            UIPanel mainPanel = new UIPanel
            {
                Width = StyleDimension.FromPixels(180f),
                Height = StyleDimension.FromPixels(180f),
                HAlign = 0.5f,
                VAlign = 0.5f,
                BackgroundColor = new Color(39, 31, 81),
                BorderColor = Color.White
            };
            mainPanel.SetPadding(0f);
            Append(mainPanel);
            BuildingIcon buildingIcon = new BuildingIcon(buildingTexture)
            {
                HAlign = 0.5f,
                VAlign = 0.5f,
                IgnoresMouseInteraction = true
            };
            buildingIcon.SetPadding(0f);
            mainPanel.Append(buildingIcon);
            costText = new UIText("")
            {
                HAlign = 0.1f,
                VAlign = 0.1f,
                TextOriginX = 0f,
                IgnoresMouseInteraction = true
            };
            costText.SetPadding(0f);
            mainPanel.Append(costText);
            keyText = new UIText("")
            {
                HAlign = 0.5f,
                VAlign = 0.9f,
                TextOriginX = 0f,
                IgnoresMouseInteraction = true
            };
            keyText.SetPadding(0f);
            mainPanel.Append(keyText);
        }

        public override void Update(GameTime gameTime)
        {
            Player player = Main.LocalPlayer;
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (key == 1)
                cost = !player.GetModPlayer<GunslingerPlayer>().gunslingerEquipped ? 130 : 100;
            costReduction = key switch
            {
                1 => p.sentryCostReduction,
                2 => p.dispenserCostReduction,
                3 or 4 => p.teleporterCostReduction,
                _ => 1f
            };
            costText.SetText(((int)(cost * costReduction)).ToString());
            keyText.SetText(key.ToString());
            Keys keybind = key switch
            {
                1 => Keys.D1,
                2 => Keys.D2,
                3 => Keys.D3,
                4 => Keys.D4,
                _ => Keys.Escape
            };
            if (Main.keyState.IsKeyDown(keybind) && !Main.oldKeyState.IsKeyDown(keybind) && !p.CarryingBuilding)
            {
                int totalCost = TF2.Round(cost * costReduction);
                if (p.metal >= totalCost)
                {
                    switch (key)
                    {
                        case 1:
                            if (p.sentryWhoAmI > -1) return;
                            Building.ConstructBuilding(!player.GetModPlayer<GunslingerPlayer>().gunslingerEquipped ? ModContent.NPCType<SentryLevel1>() : ModContent.NPCType<MiniSentry>(), player.whoAmI, -player.direction, 0, player.GetModPlayer<MannsAntiDanmakuSystemPlayer>().mannsAntiDanmakuSystemActive);
                            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Voicelines/engineer_autobuildingsentry01"), player.Center);
                            break;
                        case 2:
                            if (p.dispenserWhoAmI > -1) return;
                            Building.ConstructBuilding(ModContent.NPCType<DispenserLevel1>(), player.whoAmI, -player.direction, 0, false);
                            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Voicelines/engineer_autobuildingdispenser01"), player.Center);
                            break;
                        case 3:
                            if (p.teleporterEntranceWhoAmI > -1) return;
                            Building.ConstructBuilding(ModContent.NPCType<TeleporterEntranceLevel1>(), player.whoAmI, -player.direction, 0, false);
                            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Voicelines/engineer_autobuildingteleporter01"), player.Center);
                            break;
                        case 4:
                            if (p.teleporterExitWhoAmI > -1) return;
                            Building.ConstructBuilding(ModContent.NPCType<TeleporterExitLevel1>(), player.whoAmI, -player.direction, 0, false);
                            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Voicelines/engineer_autobuildingteleporter01"), player.Center);
                            break;
                        default:
                            return;
                    }
                    p.metal -= totalCost;
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spawn_item"), player.Center);
                    for (int i = 0; i < 10; i++)
                    {
                        if (player.inventory[i].ModItem is TF2Weapon weapon && weapon.weaponType == TF2Item.Melee && weapon.equipped)
                        {
                            player.selectedItem = i;
                            break;
                        }
                    }
                }
            }
            base.Update(gameTime);
        }
    }

    [Autoload(Side = ModSide.Client)]
    internal class DestructionPDAHUD : TF2HUD
    {
        protected override bool CanDisplay => Player.HeldItem.ModItem is DestructionPDA && Player.GetModPlayer<TF2Player>().currentClass == TF2Item.Engineer;

        protected override void HUDCreate()
        {
            RemoveAllChildren();
            UIPanel mainPanel = new UIPanel
            {
                Width = StyleDimension.FromPixels(825f),
                Height = StyleDimension.FromPixels(240f),
                HAlign = 0.5f,
                VAlign = 0.5f,
                BackgroundColor = new Color(17, 14, 12, 64),
                BorderColor = new Color(3, 3, 3)
            };
            mainPanel.SetPadding(0f);
            Append(mainPanel);
            UIElement uIElement = new UIElement
            {
                Width = StyleDimension.FromPixels(765f),
                Height = StyleDimension.FromPixels(180f),
                HAlign = 0.5f,
                VAlign = 0.5f,
            };
            uIElement.SetPadding(0f);
            mainPanel.Append(uIElement);
            DestructionPDAPanel sentryPanel = new DestructionPDAPanel(HUDTextures.SentryIcon.Value, 1)
            {
                Width = StyleDimension.FromPixels(180f),
                Height = MaxHeight,
                HAlign = 0f,
                VAlign = 0.5f
            };
            sentryPanel.SetPadding(0f);
            uIElement.Append(sentryPanel);
            DestructionPDAPanel dispenserPanel = new DestructionPDAPanel(HUDTextures.DispenserIcon.Value, 2)
            {
                Width = StyleDimension.FromPixels(180f),
                Height = MaxHeight,
                HAlign = 0.333f,
                VAlign = 0.5f
            };
            dispenserPanel.SetPadding(0f);
            uIElement.Append(dispenserPanel);
            DestructionPDAPanel teleporterEntrancePanel = new DestructionPDAPanel(HUDTextures.TeleporterEntranceIcon.Value, 3)
            {
                Width = StyleDimension.FromPixels(180f),
                Height = MaxHeight,
                HAlign = 0.666f,
                VAlign = 0.5f
            };
            teleporterEntrancePanel.SetPadding(0f);
            uIElement.Append(teleporterEntrancePanel);
            DestructionPDAPanel teleporterExitPanel = new DestructionPDAPanel(HUDTextures.TeleporterExitIcon.Value, 4)
            {
                Width = StyleDimension.FromPixels(180f),
                Height = MaxHeight,
                HAlign = 1f,
                VAlign = 0.5f
            };
            teleporterExitPanel.SetPadding(0f);
            uIElement.Append(teleporterExitPanel);
        }
    }

    [Autoload(Side = ModSide.Client)]
    internal class DestructionPDAPanel : UIState
    {
        public Texture2D buildingTexture;
        public Color BackgroundColor = Color.White;
        private UIText keyText;
        private readonly int key;

        public DestructionPDAPanel(Texture2D texture, int keybind)
        {
            buildingTexture = texture;
            key = keybind;
            BuildPage();
        }

        private void BuildPage()
        {
            RemoveAllChildren();
            UIPanel mainPanel = new UIPanel
            {
                Width = StyleDimension.FromPixels(180f),
                Height = StyleDimension.FromPixels(180f),
                HAlign = 0.5f,
                VAlign = 0.5f,
                BackgroundColor = new Color(50, 44, 35),
                BorderColor = new Color(244, 228, 192)
            };
            mainPanel.SetPadding(0f);
            Append(mainPanel);
            BuildingIcon destructionBackground = new BuildingIcon(HUDTextures.DestructionPDABackground.Value, 160f)
            {
                HAlign = 0.5f,
                VAlign = 0.5f,
                IgnoresMouseInteraction = true
            };
            destructionBackground.SetPadding(0f);
            mainPanel.Append(destructionBackground);
            BuildingIcon buildingIcon = new BuildingIcon(buildingTexture)
            {
                HAlign = 0.5f,
                VAlign = 0.5f,
                IgnoresMouseInteraction = true
            };
            buildingIcon.SetPadding(0f);
            mainPanel.Append(buildingIcon);
            keyText = new UIText("")
            {
                HAlign = 0.5f,
                VAlign = 0.9f,
                TextOriginX = 0f,
                IgnoresMouseInteraction = true
            };
            keyText.SetPadding(0f);
            mainPanel.Append(keyText);
        }

        public override void Update(GameTime gameTime)
        {
            Player player = Main.LocalPlayer;
            TF2Player p = player.GetModPlayer<TF2Player>();
            keyText.SetText(key.ToString());
            Keys keybind = key switch
            {
                1 => Keys.D1,
                2 => Keys.D2,
                3 => Keys.D3,
                4 => Keys.D4,
                _ => Keys.Escape
            };
            if (Main.keyState.IsKeyDown(keybind) && !Main.oldKeyState.IsKeyDown(keybind) && !p.CarryingBuilding)
            {
                switch (key)
                {
                    case 1:
                        int sentryID = player.GetModPlayer<TF2Player>().sentryWhoAmI;
                        if (sentryID > -1 && Main.npc[sentryID].ModNPC is TF2Sentry sentry && sentry.Initialized)
                        {
                            TF2.KillNPC(sentry.NPC);
                            if (Main.netMode == NetmodeID.SinglePlayer)
                                sentry.NPC.checkDead();
                            player.GetModPlayer<TF2Player>().sentryWhoAmI = -1;
                            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/NPCs/sentry_explode"), sentry.NPC.Center);
                        }
                        break;
                    case 2:
                        int dispenserID = player.GetModPlayer<TF2Player>().dispenserWhoAmI;
                        if (dispenserID > -1 && Main.npc[dispenserID].ModNPC is TF2Dispenser dispenser && dispenser.Initialized)
                        {
                            TF2.KillNPC(dispenser.NPC);
                            if (Main.netMode == NetmodeID.SinglePlayer)
                                dispenser.NPC.checkDead();
                            player.GetModPlayer<TF2Player>().dispenserWhoAmI = -1;
                            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/NPCs/dispenser_explode"), dispenser.NPC.Center);
                        }
                        break;
                    case 3:
                        int entranceID = player.GetModPlayer<TF2Player>().teleporterEntranceWhoAmI;
                        if (entranceID > -1 && Main.npc[entranceID].ModNPC is TeleporterEntrance entrance && entrance.Initialized)
                        {
                            TF2.KillNPC(entrance.NPC);
                            if (Main.netMode == NetmodeID.SinglePlayer)
                                entrance.NPC.checkDead();
                            player.GetModPlayer<TF2Player>().teleporterEntranceWhoAmI = -1;
                            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/NPCs/teleporter_explode"), entrance.NPC.Center);
                        }
                        break;
                    case 4:
                        int exitID = player.GetModPlayer<TF2Player>().teleporterExitWhoAmI;
                        if (exitID > -1 && Main.npc[exitID].ModNPC is TeleporterExit exit && exit.Initialized)
                        {
                            TF2.KillNPC(exit.NPC);
                            if (Main.netMode == NetmodeID.SinglePlayer)
                                exit.NPC.checkDead();
                            player.GetModPlayer<TF2Player>().teleporterExitWhoAmI = -1;
                            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/NPCs/teleporter_explode"), exit.NPC.Center);
                        }
                        break;
                    default:
                        return;
                }
            }
            base.Update(gameTime);
        }
    }

    [Autoload(Side = ModSide.Client)]
    internal class BuildingIcon(Texture2D buildingTexture, float iconSize = 100f) : UIState
    {
        private readonly Texture2D texture = buildingTexture;
        private readonly float size = iconSize;

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle innerDimensions = GetInnerDimensions();
            Rectangle rectangle = texture.Frame(1, 1, 0, 0, 0, 0);
            float num = 4f;
            if (rectangle.Width * num > size || rectangle.Height * num > size)
                num = rectangle.Width <= rectangle.Height ? size / rectangle.Height : size / rectangle.Width;
            spriteBatch.Draw(texture, innerDimensions.Center(), new Rectangle?(rectangle), Color.White, 0f, new Vector2(rectangle.Width, rectangle.Height) * 0.5f, num, 0, 0f);
        }
    }
}