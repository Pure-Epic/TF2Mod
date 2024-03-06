using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.Graphics;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using TF2.Common;
using TF2.Content.Items;
using TF2.Content.Items.Tools;
using TF2.Content.Items.Weapons.Demoman;
using TF2.Content.Items.Weapons.Engineer;
using TF2.Content.Items.Weapons.Heavy;
using TF2.Content.Items.Weapons.Medic;
using TF2.Content.Items.Weapons.MultiClass;
using TF2.Content.Items.Weapons.Pyro;
using TF2.Content.Items.Weapons.Scout;
using TF2.Content.Items.Weapons.Sniper;
using TF2.Content.Items.Weapons.Soldier;
using TF2.Content.Items.Weapons.Spy;
using TF2.Content.Mounts;

namespace TF2.Content.UI.MercenaryCreationMenu
{
    [Autoload(Side = ModSide.Client)]
    public class TF2ChoosePlayerType : UIState
    {
        public override void OnInitialize() => BuildPage();

        public override void Update(GameTime gameTime)
        {
            if (Main.keyState.IsKeyDown(Keys.Escape) && !Main.oldKeyState.IsKeyDown(Keys.Escape))
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
                Main.OpenCharacterSelectUI();
            }
            base.Update(gameTime);
        }

        private void BuildPage()
        {
            RemoveAllChildren();
            UIElement uIElement = new UIElement
            {
                Width = StyleDimension.FromPixels(800f),
                Height = StyleDimension.FromPixels(550f),
                HAlign = 0.5f,
                VAlign = 0.5f
            };
            uIElement.SetPadding(0f);
            Append(uIElement);
            TF2ChoosePlayerTypePanel mercenaryPanel = new TF2ChoosePlayerTypePanel
            {
                Width = StyleDimension.FromPercent(0.495f),
                Height = MaxHeight,
                HAlign = 0f,
                VAlign = 0.5f,
                BackgroundTexture = ModContent.Request<Texture2D>("TF2/Content/Textures/UI/MercenaryCreationMenu/Mercenary", AssetRequestMode.ImmediateLoad).Value,
                BackgroundColor = Color.White,
                BorderColor = new Color(3, 3, 3)
            };
            mercenaryPanel.SetPadding(0f);
            mercenaryPanel.OnLeftMouseDown += Click_MercenarySelect;
            mercenaryPanel.OnMouseOver += SelectMouseOver;
            mercenaryPanel.OnMouseOut += SelectMouseOut;
            uIElement.Append(mercenaryPanel);
            UIText mercenaryPanelText = new UIText(Language.GetText("Mods.TF2.UI.TF2MercenaryCreation.Mercenary"), 1.5f, true)
            {
                HAlign = 0.5f,
                VAlign = 0.925f,
                IgnoresMouseInteraction = true
            };
            mercenaryPanel.Append(mercenaryPanelText);
            TF2ChoosePlayerTypePanel classlessPanel = new TF2ChoosePlayerTypePanel
            {
                Width = StyleDimension.FromPercent(0.495f),
                Height = MaxHeight,
                HAlign = 1f,
                VAlign = 0.5f,
                BackgroundTexture = ModContent.Request<Texture2D>("TF2/Content/Textures/UI/MercenaryCreationMenu/Classless", AssetRequestMode.ImmediateLoad).Value,
                BackgroundColor = Color.White,
                BorderColor = new Color(3, 3, 3)
            };
            classlessPanel.OnLeftMouseDown += Click_ClasslessSelect;
            classlessPanel.OnMouseOver += SelectMouseOver;
            classlessPanel.OnMouseOut += SelectMouseOut;
            classlessPanel.SetPadding(0f);
            uIElement.Append(classlessPanel);
            UIText classlessText = new UIText(Language.GetText("Mods.TF2.UI.TF2MercenaryCreation.Classless"), 1.5f, true)
            {
                HAlign = 0.5f,
                VAlign = 0.925f,
                IgnoresMouseInteraction = true
            };
            classlessPanel.Append(classlessText);
            MakeBackButton(this);
        }

        private void Click_MercenarySelect(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuOpen);
            Main.PendingPlayer = new Player();
            Main.menuMode = 888;
            Main.MenuUI.SetState(new TF2MercenarySelect(Main.PendingPlayer));
        }

        private void Click_ClasslessSelect(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuOpen);
            Main.PendingPlayer = new Player();
            Main.menuMode = 888;
            Main.MenuUI.SetState(new UICharacterCreation(Main.PendingPlayer));
        }

        private void SelectMouseOver(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            ((TF2ChoosePlayerTypePanel)evt.Target).BorderColor = Colors.FancyUIFatButtonMouseOver;
        }

        private void SelectMouseOut(UIMouseEvent evt, UIElement listeningElement) => ((TF2ChoosePlayerTypePanel)evt.Target).BorderColor = new Color(3, 3, 3);

        private void MakeBackButton(UIElement outerContainer)
        {
            UITextPanel<LocalizedText> uITextPanel = new UITextPanel<LocalizedText>(Language.GetText("UI.Back"), 0.7f, true)
            {
                Width = StyleDimension.FromPixels(250f),
                Height = StyleDimension.FromPixels(50f),
                VAlign = 0.5f,
                HAlign = 0.5f,
                Top = StyleDimension.FromPixels(307f)
            };
            uITextPanel.OnLeftMouseDown += Click_GoBack;
            uITextPanel.OnMouseOver += FadedMouseOver;
            uITextPanel.OnMouseOut += FadedMouseOut;
            uITextPanel.SetSnapPoint("Back", 0, null, null);
            outerContainer.Append(uITextPanel);
        }

        private void Click_GoBack(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuClose);
            Main.OpenCharacterSelectUI();
        }

        private void FadedMouseOver(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            ((UIPanel)evt.Target).BackgroundColor = new Color(73, 94, 171);
            ((UIPanel)evt.Target).BorderColor = Colors.FancyUIFatButtonMouseOver;
        }

        private void FadedMouseOut(UIMouseEvent evt, UIElement listeningElement)
        {
            ((UIPanel)evt.Target).BackgroundColor = new Color(63, 82, 151) * 0.8f;
            ((UIPanel)evt.Target).BorderColor = Color.Black;
        }
    }

    [Autoload(Side = ModSide.Client)]
    public class TF2MercenarySelect : UIState
    {
        private readonly Player _player;
        private readonly TF2Player mercenaryPlayer;
        private UIText classDescription;

        public TF2MercenarySelect(Player player, int mercenary = -1)
        {
            _player = player;
            mercenaryPlayer = _player.GetModPlayer<TF2Player>();
            player.difficulty = 0;
            player.hairColor = Color.Transparent;
            player.skinColor = Color.Transparent;
            mercenaryPlayer.currentClass = mercenary;
            BuildPage();
        }

        public override void Update(GameTime gameTime)
        {
            if (Main.keyState.IsKeyDown(Keys.Escape) && !Main.oldKeyState.IsKeyDown(Keys.Escape))
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
                Main.menuMode = 888;
                Main.MenuUI.SetState(new TF2ChoosePlayerType());
            }
            base.Update(gameTime);
        }

        private void BuildPage()
        {
            RemoveAllChildren();
            UIElement uIElement = new UIElement
            {
                Width = StyleDimension.FromPixels(850f),
                Height = StyleDimension.FromPixels(550f),
                HAlign = 0.5f,
                VAlign = 0.5f
            };
            uIElement.SetPadding(0f);
            Append(uIElement);
            UIPanel mainPanel = new UIPanel
            {
                Width = MaxWidth,
                Height = MaxHeight,
                HAlign = 0.5f,
                VAlign = 0.5f,
                BackgroundColor = new Color(48, 47, 45),
                BorderColor = new Color(3, 3, 3)
            };
            mainPanel.SetPadding(0f);
            uIElement.Append(mainPanel);
            DrawMercenaries(uIElement);
            UIPanel subPanel = new UIPanel
            {
                Width = MaxWidth,
                Height = StyleDimension.FromPercent(0.8f),
                Top = StyleDimension.FromPixels(110f),
                HAlign = 0.5f,
                VAlign = 0f,
                BackgroundColor = new Color(122, 107, 100),
                BorderColor = new Color(3, 3, 3)
            };
            subPanel.SetPadding(0f);
            mainPanel.Append(subPanel);
            DrawMercenaryDescription(subPanel);
            DrawMercenaryPreview(subPanel);
            UIPanel bottomPanel = new UIPanel
            {
                Width = MaxWidth,
                Height = StyleDimension.FromPercent(0.15f),
                Top = StyleDimension.FromPixels(467.5f),
                HAlign = 0.5f,
                VAlign = 0f,
                BackgroundColor = new Color(48, 47, 45),
                BorderColor = new Color(3, 3, 3)
            };
            bottomPanel.SetPadding(0f);
            mainPanel.Append(bottomPanel);
            UIText selectClassText = new UIText(Language.GetText("Mods.TF2.UI.TF2MercenaryCreation.SelectClassText"), 0.75f, true)
            {
                HAlign = 0.0375f,
                VAlign = 0.5f,
                IgnoresMouseInteraction = true,
            };
            bottomPanel.Append(selectClassText);
            UITextPanel<LocalizedText> randomClassPanel = new UITextPanel<LocalizedText>(Language.GetText("Mods.TF2.UI.TF2MercenaryCreation.RandomClass"), 0.5f, true)
            {
                Width = StyleDimension.FromPixels(200f),
                Height = StyleDimension.FromPixels(50f),
                VAlign = 0.5f,
                HAlign = 0.965f,
                BackgroundColor = new Color(115, 105, 95, 255)
            };
            randomClassPanel.OnLeftMouseDown += Click_RandomClass;
            randomClassPanel.OnMouseOver += FadedMouseOver;
            randomClassPanel.OnMouseOut += FadedMouseOut;
            randomClassPanel.SetSnapPoint("RandomClass", 0, null, null);
            bottomPanel.Append(randomClassPanel);
            MakeBackAndNextButton(this);
        }

        private void DrawMercenaries(UIElement outerContainer)
        {
            TF2MercenaryRender soldier = null;
            TF2MercenaryRender heavy = null;
            TF2MercenaryRender sniper = null;
            for (int i = 1; i <= 9; i++)
            {
                TF2MercenaryRender mercenary = new TF2MercenaryRender(i)
                {
                    Width = StyleDimension.FromPixels(32f),
                    Height = StyleDimension.FromPixels(44f),
                    Left = i switch
                    {
                        1 or 4 or 7 => StyleDimension.FromPixels(-50f),
                        3 or 6 or 9 => StyleDimension.FromPixels(50f),
                        _ => StyleDimension.FromPixels(0f)
                    },
                    HAlign = i switch
                    {
                        1 or 2 or 3 => 0.2f,
                        4 or 5 or 6 => 0.5f,
                        7 or 8 or 9 => 0.8f,
                        _ => 0f
                    },
                    VAlign = 0.0425f
                };
                mercenary.SetPadding(0f);
                mercenary.OnMouseOver += ClassSelected;
                switch (i)
                {
                    case 2:
                        soldier = mercenary;
                        break;

                    case 5:
                        heavy = mercenary;
                        break;

                    case 8:
                        sniper = mercenary;
                        break;

                    default:
                        break;
                }
                outerContainer.Append(mercenary);
            }
            UIText offenseText = new UIText(Language.GetText("Mods.TF2.UI.TF2MercenaryCreation.Offense"))
            {
                HAlign = 0.5f,
                VAlign = 1.85f,
                IgnoresMouseInteraction = true
            };
            soldier.Append(offenseText);
            UIText defenseText = new UIText(Language.GetText("Mods.TF2.UI.TF2MercenaryCreation.Defense"))
            {
                HAlign = 0.5f,
                VAlign = 1.85f,
                IgnoresMouseInteraction = true
            };
            heavy.Append(defenseText);
            UIText supportText = new UIText(Language.GetText("Mods.TF2.UI.TF2MercenaryCreation.Support"))
            {
                HAlign = 0.5f,
                VAlign = 1.85f,
                IgnoresMouseInteraction = true
            };
            sniper.Append(supportText);
        }

        private void DrawMercenaryPreview(UIPanel container)
        {
            TF2MercenaryPreview element = new TF2MercenaryPreview(_player, 5f)
            {
                Width = StyleDimension.FromPixels(200f),
                Height = StyleDimension.FromPixels(200f),
                VAlign = 0.3375f,
                HAlign = 0.16f
            };
            container.Append(element);
        }

        private void DrawMercenaryDescription(UIPanel container)
        {
            UIPanel textPanel = new UIPanel
            {
                Width = StyleDimension.FromPixels(400f),
                Height = StyleDimension.FromPixels(250f),
                HAlign = 0.9f,
                VAlign = 0.3f,
                BackgroundColor = new Color(91, 88, 99),
                BorderColor = new Color(192, 192, 192)
            };
            textPanel.SetPadding(0f);
            container.Append(textPanel);
            UIText classDesciption = new UIText("")
            {
                Width = MaxWidth,
                Height = MaxHeight,
                PaddingTop = 25f,
                PaddingLeft = 50f,
                PaddingRight = 50f,
                IgnoresMouseInteraction = true,
                IsWrapped = true
            };
            textPanel.Append(classDesciption);
            classDescription = classDesciption;
        }

        private void MakeBackAndNextButton(UIElement outerContainer)
        {
            UITextPanel<LocalizedText> uITextPanel = new UITextPanel<LocalizedText>(Language.GetText("UI.Back"), 0.7f, true)
            {
                Width = StyleDimension.FromPixels(250f),
                Height = StyleDimension.FromPixels(50f),
                VAlign = 0.5f,
                HAlign = 0.405f,
                Top = StyleDimension.FromPixels(307f),
                BackgroundColor = new Color(115, 105, 95, 255)
            };
            uITextPanel.OnLeftMouseDown += Click_GoBack;
            uITextPanel.OnMouseOver += FadedMouseOver;
            uITextPanel.OnMouseOut += FadedMouseOut;
            uITextPanel.SetSnapPoint("Back", 0, null, null);
            outerContainer.Append(uITextPanel);
            UITextPanel<LocalizedText> uITextPanel2 = new UITextPanel<LocalizedText>(Language.GetText("Mods.TF2.UI.TF2MercenaryCreation.Next"), 0.7f, true)
            {
                Width = StyleDimension.FromPixels(250f),
                Height = StyleDimension.FromPixels(50f),
                VAlign = 0.5f,
                HAlign = 0.595f,
                Top = StyleDimension.FromPixels(307f),
                BackgroundColor = new Color(115, 105, 95, 255)
            };
            uITextPanel2.OnMouseOver += FadedMouseOver;
            uITextPanel2.OnMouseOut += FadedMouseOut;
            uITextPanel2.OnLeftMouseDown += Click_Next;
            uITextPanel2.SetSnapPoint("Next", 0, null, null);
            outerContainer.Append(uITextPanel2);
        }

        private void ClassSelected(UIMouseEvent evt, UIElement listeningElement)
        {
            mercenaryPlayer.currentClass = ((TF2MercenaryRender)evt.Target).classID;
            SoundEngine.PlaySound(new SoundStyle($"TF2/Content/Sounds/SFX/UI/class_menu_{mercenaryPlayer.currentClass}"));
            classDescription.SetText(Language.GetText($"Mods.TF2.UI.TF2MercenaryCreation.{(TF2Player.ClassName)mercenaryPlayer.currentClass}"));
        }

        private void Click_RandomClass(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/UI/class_menu_random"));
            mercenaryPlayer.currentClass = -1;
            classDescription.SetText("");
        }

        private void Click_GoBack(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuClose);
            Main.menuMode = 888;
            Main.MenuUI.SetState(new TF2ChoosePlayerType());
        }

        private void Click_Next(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuOpen);
            Main.PendingPlayer = _player;
            Main.menuMode = 888;
            if (mercenaryPlayer.currentClass == -1)
                mercenaryPlayer.currentClass = Main.rand.Next(1, 10);
            Main.MenuUI.SetState(new TF2MercenaryCreation(Main.PendingPlayer, mercenaryPlayer.currentClass));
        }

        private void FadedMouseOver(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            ((UIPanel)evt.Target).BackgroundColor = new Color(115, 105, 95, 255) * 1.25f;
            ((UIPanel)evt.Target).BorderColor = Colors.FancyUIFatButtonMouseOver;
        }

        private void FadedMouseOut(UIMouseEvent evt, UIElement listeningElement)
        {
            ((UIPanel)evt.Target).BackgroundColor = new Color(115, 105, 95, 255);
            ((UIPanel)evt.Target).BorderColor = Color.Black;
        }
    }

    [Autoload(Side = ModSide.Client)]
    public class TF2MercenaryCreation : UIState
    {
        private readonly Player _player;
        private UICharacterNameButton _charName;

        public TF2MercenaryCreation(Player player, int mercenary)
        {
            _player = player;
            _player.difficulty = 0;
            _player.GetModPlayer<TF2Player>().currentClass = mercenary;
            BuildPage();
        }

        public override void Update(GameTime gameTime)
        {
            if (Main.keyState.IsKeyDown(Keys.Escape) && !Main.oldKeyState.IsKeyDown(Keys.Escape))
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
                Main.PendingPlayer = _player;
                Main.menuMode = 888;
                Main.MenuUI.SetState(new TF2MercenarySelect(Main.PendingPlayer, Main.PendingPlayer.GetModPlayer<TF2Player>().currentClass));
            }
            base.Update(gameTime);
        }

        private void BuildPage()
        {
            RemoveAllChildren();
            UIElement uIElement = new UIElement
            {
                Width = StyleDimension.FromPixels(500f),
                Height = StyleDimension.FromPixels(200f),
                Top = StyleDimension.FromPixels(404f),
                HAlign = 0.5f,
                VAlign = 0f
            };
            uIElement.SetPadding(0f);
            Append(uIElement);
            UIPanel uIPanel = new UIPanel
            {
                Width = StyleDimension.FromPercent(1f),
                Height = StyleDimension.FromPixels(60f),
                Top = StyleDimension.FromPixels(36f),
                BackgroundColor = new Color(33, 43, 79) * 0.8f
            };
            uIPanel.SetPadding(0f);
            uIElement.Append(uIPanel);
            MakeBackAndCreatebuttons(uIElement);
            MakeCharPreview(uIPanel);
            MakeInfoMenu(uIPanel);
        }

        private void MakeCharPreview(UIPanel container)
        {
            TF2MercenaryPreview element = new TF2MercenaryPreview(_player, 1.5f)
            {
                Width = StyleDimension.FromPixels(80f),
                Height = StyleDimension.FromPixels(80f),
                Top = StyleDimension.FromPixels(-100f),
                VAlign = 0f,
                HAlign = 0.5f
            };
            container.Append(element);
        }

        private void MakeInfoMenu(UIElement parentContainer)
        {
            UIElement uIElement = new UIElement
            {
                Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
                Height = StyleDimension.FromPixelsAndPercent(0f, 1f),
                HAlign = 0.5f,
                VAlign = 0.5f
            };
            uIElement.SetPadding(10f);
            uIElement.PaddingBottom = 0f;
            uIElement.PaddingTop = 0f;
            parentContainer.Append(uIElement);
            UICharacterNameButton uICharacterNameButton = new UICharacterNameButton(Language.GetText("UI.WorldCreationName"), Language.GetText("UI.PlayerEmptyName"), null);
            uICharacterNameButton.Width = StyleDimension.FromPixelsAndPercent(0f, 1f);
            uICharacterNameButton.HAlign = 0.5f;
            uICharacterNameButton.VAlign = 0.5f;
            uIElement.Append(uICharacterNameButton);
            _charName = uICharacterNameButton;
            uICharacterNameButton.OnLeftMouseDown += Click_Naming;
            uICharacterNameButton.SetSnapPoint("Middle", 0, null, null);
        }

        private void Click_Naming(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuOpen);
            _player.name = "";
            Main.clrInput();
            UIVirtualKeyboard uIVirtualKeyboard = new UIVirtualKeyboard(Lang.menu[45].Value, "", new UIVirtualKeyboard.KeyboardSubmitEvent(OnFinishedNaming), new Action(OnCanceledNaming), 0, true);
            uIVirtualKeyboard.SetMaxInputLength(20);
            Main.MenuUI.SetState(uIVirtualKeyboard);
        }

        private void MakeBackAndCreatebuttons(UIElement outerContainer)
        {
            UITextPanel<LocalizedText> uITextPanel = new UITextPanel<LocalizedText>(Language.GetText("UI.Back"), 0.7f, true)
            {
                Width = StyleDimension.FromPixelsAndPercent(-10f, 0.5f),
                Height = StyleDimension.FromPixels(50f),
                VAlign = 1f,
                HAlign = 0f,
                Top = StyleDimension.FromPixels(-45f)
            };
            uITextPanel.OnMouseOver += FadedMouseOver;
            uITextPanel.OnMouseOut += FadedMouseOut;
            uITextPanel.OnLeftMouseDown += Click_GoBack;
            uITextPanel.SetSnapPoint("Back", 0, null, null);
            outerContainer.Append(uITextPanel);
            UITextPanel<LocalizedText> uITextPanel2 = new UITextPanel<LocalizedText>(Language.GetText("UI.Create"), 0.7f, true)
            {
                Width = StyleDimension.FromPixelsAndPercent(-10f, 0.5f),
                Height = StyleDimension.FromPixels(50f),
                VAlign = 1f,
                HAlign = 1f,
                Top = StyleDimension.FromPixels(-45f)
            };
            uITextPanel2.OnMouseOver += FadedMouseOver;
            uITextPanel2.OnMouseOut += FadedMouseOut;
            uITextPanel2.OnLeftMouseDown += Click_NamingAndCreating;
            uITextPanel2.SetSnapPoint("Create", 0, null, null);
            outerContainer.Append(uITextPanel2);
        }

        private void Click_NamingAndCreating(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuOpen);
            if (string.IsNullOrEmpty(_player.name))
            {
                _player.name = "";
                Main.clrInput();
                UIVirtualKeyboard uIVirtualKeyboard = new UIVirtualKeyboard(Lang.menu[45].Value, "", new UIVirtualKeyboard.KeyboardSubmitEvent(OnFinishedNamingAndCreating), new Action(OnCanceledNaming), 0, false);
                uIVirtualKeyboard.SetMaxInputLength(20);
                Main.MenuUI.SetState(uIVirtualKeyboard);
                return;
            }
            FinishCreatingCharacter();
        }

        private void OnFinishedNaming(string name)
        {
            _player.name = name.Trim();
            Main.MenuUI.SetState(this);
            _charName.SetContents(_player.name);
        }

        private void OnCanceledNaming() => Main.MenuUI.SetState(this);

        private void OnFinishedNamingAndCreating(string name)
        {
            _player.name = name.Trim();
            Main.MenuUI.SetState(this);
            _charName.SetContents(_player.name);
            FinishCreatingCharacter();
        }

        private void FinishCreatingCharacter()
        {
            SetupMercenary();
            PlayerFileData.CreateAndSave(_player);
            Main.LoadPlayers();
            Main.menuMode = 1;
        }


        private void SetupMercenary()
        {
            int i = 0;
            switch (_player.GetModPlayer<TF2Player>().currentClass)
            {
                case TF2Item.Scout:
                    _player.inventory[i++].SetDefaults(ModContent.ItemType<Scattergun>());
                    _player.inventory[i++].SetDefaults(ModContent.ItemType<Pistol>());
                    _player.inventory[i++].SetDefaults(ModContent.ItemType<Bat>());
                    _player.inventory[i++].SetDefaults(ModContent.ItemType<AustraliumDrill>());
                    break;
                case TF2Item.Soldier:
                    _player.inventory[i++].SetDefaults(ModContent.ItemType<RocketLauncher>());
                    _player.inventory[i++].SetDefaults(ModContent.ItemType<Shotgun>());
                    _player.inventory[i++].SetDefaults(ModContent.ItemType<Shovel>());
                    _player.inventory[i++].SetDefaults(ModContent.ItemType<AustraliumDrill>());
                    break;
                case TF2Item.Pyro:
                    _player.inventory[i++].SetDefaults(ModContent.ItemType<FlameThrower>());
                    _player.inventory[i++].SetDefaults(ModContent.ItemType<Shotgun>());
                    _player.inventory[i++].SetDefaults(ModContent.ItemType<FireAxe>());
                    _player.inventory[i++].SetDefaults(ModContent.ItemType<AustraliumDrill>());
                    break;
                case TF2Item.Demoman:
                    _player.inventory[i++].SetDefaults(ModContent.ItemType<GrenadeLauncher>());
                    _player.inventory[i++].SetDefaults(ModContent.ItemType<StickybombLauncher>());
                    _player.inventory[i++].SetDefaults(ModContent.ItemType<Bottle>());
                    _player.inventory[i++].SetDefaults(ModContent.ItemType<AustraliumDrill>());
                    break;
                case TF2Item.Heavy:
                    _player.inventory[i++].SetDefaults(ModContent.ItemType<Minigun>());
                    _player.inventory[i++].SetDefaults(ModContent.ItemType<Shotgun>());
                    _player.inventory[i++].SetDefaults(ModContent.ItemType<Fists>());
                    _player.inventory[i++].SetDefaults(ModContent.ItemType<AustraliumDrill>());
                    break;
                case TF2Item.Engineer:
                    _player.inventory[i++].SetDefaults(ModContent.ItemType<Shotgun>());
                    _player.inventory[i++].SetDefaults(ModContent.ItemType<Pistol>());
                    _player.inventory[i++].SetDefaults(ModContent.ItemType<Wrench>());
                    _player.inventory[i++].SetDefaults(ModContent.ItemType<ConstructionPDA>());
                    _player.inventory[i++].SetDefaults(ModContent.ItemType<DestructionPDA>());
                    _player.inventory[i++].SetDefaults(ModContent.ItemType<AustraliumDrill>());
                    break;
                case TF2Item.Medic:
                    _player.inventory[i++].SetDefaults(ModContent.ItemType<SyringeGun>());
                    _player.inventory[i++].SetDefaults(ModContent.ItemType<MediGun>());
                    _player.inventory[i++].SetDefaults(ModContent.ItemType<Bonesaw>());
                    _player.inventory[i++].SetDefaults(ModContent.ItemType<AustraliumDrill>());
                    break;
                case TF2Item.Sniper:
                    _player.inventory[i++].SetDefaults(ModContent.ItemType<SniperRifle>());
                    _player.inventory[i++].SetDefaults(ModContent.ItemType<SMG>());
                    _player.inventory[i++].SetDefaults(ModContent.ItemType<Kukri>());
                    _player.inventory[i++].SetDefaults(ModContent.ItemType<AustraliumDrill>());
                    break;
                case TF2Item.Spy:
                    _player.inventory[i++].SetDefaults(ModContent.ItemType<Revolver>());
                    _player.inventory[i++].SetDefaults(ModContent.ItemType<Knife>());
                    _player.inventory[i++].SetDefaults(ModContent.ItemType<Sapper>());
                    _player.inventory[i++].SetDefaults(ModContent.ItemType<AustraliumDrill>());
                    break;
            }
            _player.miscEquips[3].SetDefaults(ModContent.ItemType<TF2MountItem>());
        }

        private void Click_GoBack(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuClose);
            Main.OpenCharacterSelectUI();
        }

        private void FadedMouseOver(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            ((UIPanel)evt.Target).BackgroundColor = new Color(73, 94, 171);
            ((UIPanel)evt.Target).BorderColor = Colors.FancyUIFatButtonMouseOver;
        }

        private void FadedMouseOut(UIMouseEvent evt, UIElement listeningElement)
        {
            ((UIPanel)evt.Target).BackgroundColor = new Color(63, 82, 151) * 0.8f;
            ((UIPanel)evt.Target).BorderColor = Color.Black;
        }
    }

    [Autoload(Side = ModSide.Client)]
    internal class TF2ChoosePlayerTypePanel : UIState
    {
        public Texture2D BackgroundTexture;
        public Color BorderColor = new Color(3, 3, 3);
        public Color BackgroundColor = Color.White;

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetDimensions();
            Point point = new Point((int)dimensions.X, (int)dimensions.Y);
            int width = (int)Math.Ceiling(dimensions.Width);
            int height = (int)Math.Ceiling(dimensions.Height);
            spriteBatch.Draw(BackgroundTexture, new Rectangle(point.X, point.Y, width, height), Color.White);
            spriteBatch.Draw(ModContent.Request<Texture2D>("TF2/Content/Textures/UI/MercenaryCreationMenu/IconBorder", AssetRequestMode.ImmediateLoad).Value, new Rectangle(point.X, point.Y, width, height), BorderColor);
        }
    }

    [Autoload(Side = ModSide.Client)]
    internal class TF2MercenaryRender : UIState
    {
        public Asset<Texture2D> BackgroundTexture;
        public Color BackgroundColor = Color.White;
        public int classID;

        public TF2MercenaryRender(int mercenary)
        {
            BackgroundTexture = ModContent.Request<Texture2D>($"TF2/Content/Textures/UI/MercenaryCreationMenu/{(TF2Player.ClassName)mercenary}", AssetRequestMode.ImmediateLoad);
            classID = mercenary;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetDimensions();
            Point point = new Point((int)dimensions.X, (int)dimensions.Y);
            int width = (int)Math.Ceiling(dimensions.Width);
            int height = (int)Math.Ceiling(dimensions.Height);
            spriteBatch.Draw(BackgroundTexture.Value, new Rectangle(point.X, point.Y, width, height), Color.White);
        }
    }

    [Autoload(Side = ModSide.Client)]
    internal class TF2MercenaryPreview : UIState
    {
        private readonly Player _player;
        private readonly float _scale;

        public TF2MercenaryPreview(Player player, float scale)
        {
            _player = player;
            _scale = scale;
            UseImmediateMode = true;
            OverrideSamplerState = SamplerState.PointClamp;
        }

        public override void Update(GameTime gameTime)
        {
            using (new Main.CurrentPlayerOverride(_player))
            {
                _player.ResetEffects();
                _player.ResetVisibleAccessories();
                _player.UpdateMiscCounter();
                _player.UpdateDyes();
                _player.PlayerFrame();
                base.Update(gameTime);
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetDimensions();
            Point point = new Point((int)dimensions.X, (int)dimensions.Y);
            int width = (int)Math.Ceiling(dimensions.Width);
            int height = (int)Math.Ceiling(dimensions.Height);
            Rectangle rectangle = new Rectangle(point.X, point.Y, width, height);
            if (_player.GetModPlayer<TF2Player>().currentClass == -1)
            {
                spriteBatch.Draw(ModContent.Request<Texture2D>($"TF2/Content/Textures/UI/MercenaryCreationMenu/RandomClass", AssetRequestMode.ImmediateLoad).Value, rectangle, Color.White);
                return;
            }
            int num = (int)(Main.GlobalTimeWrappedHourly / 0.07f) % 14 + 6;
            _player.bodyFrame.Y = _player.legFrame.Y = _player.headFrame.Y = num * 56;
            _player.WingFrame(false, false);
            TF2.PlayerRenderer.DrawPlayer(Main.Camera, _player, GetPlayerPosition(ref dimensions) + Main.screenPosition, 0f, Vector2.Zero, 0f, _scale);
        }

        private static Vector2 GetPlayerPosition(ref CalculatedStyle dimensions) => dimensions.Position() + new Vector2(dimensions.Width * 0.4f, dimensions.Height * 0.825f);
    }

    [Autoload(Side = ModSide.Client)]
    public class TF2PlayerRenderer : IPlayerRenderer
    {
        private readonly List<DrawData> _drawData = new List<DrawData>();

        private readonly List<int> _dust = new List<int>();

        private readonly List<int> _gore = new List<int>();

        public static SamplerState MountedSamplerState
        {
            get
            {
                if (!Main.drawToScreen)
                    return SamplerState.AnisotropicClamp;
                return SamplerState.LinearClamp;
            }
        }

        public void DrawPlayers(Camera camera, IEnumerable<Player> players)
        {
            foreach (Player player in players)
                DrawPlayerFull(camera, player);
        }

        public void DrawPlayerHead(Camera camera, Player drawPlayer, Vector2 position, float alpha = 1f, float scale = 1f, Color borderColor = default) => DrawPlayerInternal(drawPlayer, position + Main.screenPosition, 0f, Vector2.Zero, 0f, alpha, scale, true);

        public void DrawPlayer(Camera camera, Player drawPlayer, Vector2 position, float rotation, Vector2 rotationOrigin, float shadow = 0f, float scale = 1f) => DrawPlayerInternal(drawPlayer, position, rotation, rotationOrigin, shadow, 1f, scale);

        private void DrawPlayerInternal(Player drawPlayer, Vector2 position, float rotation, Vector2 rotationOrigin, float shadow = 0f, float alpha = 1f, float scale = 1f, bool headOnly = false)
        {
            if (drawPlayer.ShouldNotDraw) return;

            PlayerDrawSet drawInfo = default;
            _drawData.Clear();
            _dust.Clear();
            _gore.Clear();
            if (headOnly)
                drawInfo.HeadOnlySetup(drawPlayer, _drawData, _dust, _gore, position.X, position.Y, alpha, scale);
            else
                drawInfo.BoringSetup(drawPlayer, _drawData, _dust, _gore, position, shadow, rotation, rotationOrigin);

            PlayerLoader.ModifyDrawInfo(ref drawInfo);
            PlayerDrawLayer[] drawLayers = PlayerDrawLayerLoader.GetDrawLayers(drawInfo);
            foreach (PlayerDrawLayer playerDrawLayer in drawLayers)
            {
                if (!headOnly || playerDrawLayer.IsHeadLayer)
                    playerDrawLayer.DrawWithTransformationAndChildren(ref drawInfo);
            }

            PlayerDrawLayers.DrawPlayer_MakeIntoFirstFractalAfterImage(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_TransformDrawData(ref drawInfo);
            if (scale != 1f)
                PlayerDrawLayers.DrawPlayer_ScaleDrawData(ref drawInfo, scale);

            PlayerDrawLayers.DrawPlayer_RenderAllLayers(ref drawInfo);
            if (!drawInfo.drawPlayer.mount.Active || !drawInfo.drawPlayer.UsingSuperCart) return;

            for (int j = 0; j < 1000; j++)
            {
                if (Main.projectile[j].active && Main.projectile[j].owner == drawInfo.drawPlayer.whoAmI && Main.projectile[j].type == 591)
                    Main.instance.DrawProj(j);
            }
        }

        private void DrawPlayerFull(Camera camera, Player drawPlayer)
        {
            SpriteBatch spriteBatch = camera.SpriteBatch;
            SamplerState samplerState = camera.Sampler;
            if (drawPlayer.mount.Active && drawPlayer.fullRotation != 0f)
                samplerState = MountedSamplerState;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, samplerState, DepthStencilState.None, camera.Rasterizer, null, camera.GameViewMatrix.TransformationMatrix);
            if (Main.gamePaused)
                drawPlayer.PlayerFrame();

            if (drawPlayer.ghost)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 vector = drawPlayer.position - drawPlayer.velocity * (2 + i * 2);
                    DrawGhost(camera, drawPlayer, vector, 0.5f + 0.2f * i);
                }
                DrawGhost(camera, drawPlayer, drawPlayer.position);
            }
            else
            {
                if (drawPlayer.inventory[drawPlayer.selectedItem].flame || drawPlayer.head == 137 || drawPlayer.wings == 22)
                {
                    drawPlayer.itemFlameCount--;
                    if (drawPlayer.itemFlameCount <= 0)
                    {
                        drawPlayer.itemFlameCount = 5;
                        for (int j = 0; j < 7; j++)
                        {
                            drawPlayer.itemFlamePos[j].X = Main.rand.Next(-10, 11) * 0.15f;
                            drawPlayer.itemFlamePos[j].Y = Main.rand.Next(-10, 1) * 0.35f;
                        }
                    }
                }

                if (drawPlayer.armorEffectDrawShadowEOCShield)
                {
                    int num = drawPlayer.eocDash / 4;
                    if (num > 3)
                        num = 3;

                    for (int k = 0; k < num; k++)
                        DrawPlayer(camera, drawPlayer, drawPlayer.shadowPos[k], drawPlayer.shadowRotation[k], drawPlayer.shadowOrigin[k], 0.5f + 0.2f * k);
                }

                Vector2 position = default;
                if (drawPlayer.invis)
                {
                    drawPlayer.armorEffectDrawOutlines = false;
                    drawPlayer.armorEffectDrawShadow = false;
                    drawPlayer.armorEffectDrawShadowSubtle = false;
                    position = drawPlayer.position;
                    if (drawPlayer.aggro <= -750)
                        DrawPlayer(camera, drawPlayer, position, drawPlayer.fullRotation, drawPlayer.fullRotationOrigin, 1f);
                    else
                    {
                        drawPlayer.invis = false;
                        DrawPlayer(camera, drawPlayer, position, drawPlayer.fullRotation, drawPlayer.fullRotationOrigin);
                        drawPlayer.invis = true;
                    }
                }

                if (drawPlayer.armorEffectDrawOutlines)
                {
                    _ = drawPlayer.position;
                    if (!Main.gamePaused)
                        drawPlayer.ghostFade += drawPlayer.ghostDir * 0.075f;

                    if (drawPlayer.ghostFade < 0.1)
                    {
                        drawPlayer.ghostDir = 1f;
                        drawPlayer.ghostFade = 0.1f;
                    }
                    else if (drawPlayer.ghostFade > 0.9)
                    {
                        drawPlayer.ghostDir = -1f;
                        drawPlayer.ghostFade = 0.9f;
                    }

                    float num2 = drawPlayer.ghostFade * 5f;
                    for (int l = 0; l < 4; l++)
                    {
                        float num3;
                        float num4;
                        switch (l)
                        {
                            default:
                                num3 = num2;
                                num4 = 0f;
                                break;

                            case 1:
                                num3 = 0f - num2;
                                num4 = 0f;
                                break;

                            case 2:
                                num3 = 0f;
                                num4 = num2;
                                break;

                            case 3:
                                num3 = 0f;
                                num4 = 0f - num2;
                                break;
                        }

                        position = new Vector2(drawPlayer.position.X + num3, drawPlayer.position.Y + drawPlayer.gfxOffY + num4);
                        DrawPlayer(camera, drawPlayer, position, drawPlayer.fullRotation, drawPlayer.fullRotationOrigin, drawPlayer.ghostFade);
                    }
                }

                if (drawPlayer.armorEffectDrawOutlinesForbidden)
                {
                    _ = drawPlayer.position;
                    if (!Main.gamePaused)
                        drawPlayer.ghostFade += drawPlayer.ghostDir * 0.025f;

                    if (drawPlayer.ghostFade < 0.1)
                    {
                        drawPlayer.ghostDir = 1f;
                        drawPlayer.ghostFade = 0.1f;
                    }
                    else if (drawPlayer.ghostFade > 0.9)
                    {
                        drawPlayer.ghostDir = -1f;
                        drawPlayer.ghostFade = 0.9f;
                    }

                    float num5 = drawPlayer.ghostFade * 5f;
                    for (int m = 0; m < 4; m++)
                    {
                        float num6;
                        float num7;
                        switch (m)
                        {
                            default:
                                num6 = num5;
                                num7 = 0f;
                                break;

                            case 1:
                                num6 = 0f - num5;
                                num7 = 0f;
                                break;

                            case 2:
                                num6 = 0f;
                                num7 = num5;
                                break;

                            case 3:
                                num6 = 0f;
                                num7 = 0f - num5;
                                break;
                        }

                        position = new Vector2(drawPlayer.position.X + num6, drawPlayer.position.Y + drawPlayer.gfxOffY + num7);
                        DrawPlayer(camera, drawPlayer, position, drawPlayer.fullRotation, drawPlayer.fullRotationOrigin, drawPlayer.ghostFade);
                    }
                }

                if (drawPlayer.armorEffectDrawShadowBasilisk)
                {
                    int num8 = (int)(drawPlayer.basiliskCharge * 3f);
                    for (int n = 0; n < num8; n++)
                        DrawPlayer(camera, drawPlayer, drawPlayer.shadowPos[n], drawPlayer.shadowRotation[n], drawPlayer.shadowOrigin[n], 0.5f + 0.2f * n);
                }
                else if (drawPlayer.armorEffectDrawShadow)
                {
                    for (int num9 = 0; num9 < 3; num9++)
                        DrawPlayer(camera, drawPlayer, drawPlayer.shadowPos[num9], drawPlayer.shadowRotation[num9], drawPlayer.shadowOrigin[num9], 0.5f + 0.2f * num9);
                }

                if (drawPlayer.armorEffectDrawShadowLokis)
                {
                    for (int num10 = 0; num10 < 3; num10++)
                        DrawPlayer(camera, drawPlayer, Vector2.Lerp(drawPlayer.shadowPos[num10], drawPlayer.position + new Vector2(0f, drawPlayer.gfxOffY), 0.5f), drawPlayer.shadowRotation[num10], drawPlayer.shadowOrigin[num10], MathHelper.Lerp(1f, 0.5f + 0.2f * num10, 0.5f));
                }

                if (drawPlayer.armorEffectDrawShadowSubtle)
                {
                    for (int num11 = 0; num11 < 4; num11++)
                    {
                        position.X = drawPlayer.position.X + Main.rand.Next(-20, 21) * 0.1f;
                        position.Y = drawPlayer.position.Y + Main.rand.Next(-20, 21) * 0.1f + drawPlayer.gfxOffY;
                        DrawPlayer(camera, drawPlayer, position, drawPlayer.fullRotation, drawPlayer.fullRotationOrigin, 0.9f);
                    }
                }

                if (drawPlayer.shadowDodge)
                {
                    drawPlayer.shadowDodgeCount += 1f;
                    if (drawPlayer.shadowDodgeCount > 30f)
                        drawPlayer.shadowDodgeCount = 30f;
                }
                else
                {
                    drawPlayer.shadowDodgeCount -= 1f;
                    if (drawPlayer.shadowDodgeCount < 0f)
                        drawPlayer.shadowDodgeCount = 0f;
                }

                if (drawPlayer.shadowDodgeCount > 0f)
                {
                    _ = drawPlayer.position;
                    position.X = drawPlayer.position.X + drawPlayer.shadowDodgeCount;
                    position.Y = drawPlayer.position.Y + drawPlayer.gfxOffY;
                    DrawPlayer(camera, drawPlayer, position, drawPlayer.fullRotation, drawPlayer.fullRotationOrigin, 0.5f + Main.rand.Next(-10, 11) * 0.005f);
                    position.X = drawPlayer.position.X - drawPlayer.shadowDodgeCount;
                    DrawPlayer(camera, drawPlayer, position, drawPlayer.fullRotation, drawPlayer.fullRotationOrigin, 0.5f + Main.rand.Next(-10, 11) * 0.005f);
                }

                if (drawPlayer.brainOfConfusionDodgeAnimationCounter > 0)
                {
                    Vector2 vector2 = drawPlayer.position + new Vector2(0f, drawPlayer.gfxOffY);
                    float lerpValue = Utils.GetLerpValue(300f, 270f, drawPlayer.brainOfConfusionDodgeAnimationCounter);
                    float y = MathHelper.Lerp(2f, 120f, lerpValue);
                    if (lerpValue >= 0f && lerpValue <= 1f)
                    {
                        for (float num12 = 0f; num12 < MathF.PI * 2f; num12 += MathF.PI / 3f)
                        {
                            position = vector2 + new Vector2(0f, y).RotatedBy(MathF.PI * 2f * lerpValue * 0.5f + num12);
                            DrawPlayer(camera, drawPlayer, position, drawPlayer.fullRotation, drawPlayer.fullRotationOrigin, lerpValue);
                        }
                    }
                }

                position = drawPlayer.position;
                position.Y += drawPlayer.gfxOffY;
                if (drawPlayer.stoned)
                    DrawPlayerStoned(camera, drawPlayer, position);
                else if (!drawPlayer.invis)
                    DrawPlayer(camera, drawPlayer, position, drawPlayer.fullRotation, drawPlayer.fullRotationOrigin);
            }

            spriteBatch.End();
        }

        private static void DrawPlayerStoned(Camera camera, Player drawPlayer, Vector2 position)
        {
            if (!drawPlayer.dead)
            {
                SpriteEffects spriteEffects = drawPlayer.direction != 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                camera.SpriteBatch.Draw(TextureAssets.Extra[37].Value, new Vector2((int)(position.X - camera.UnscaledPosition.X - drawPlayer.bodyFrame.Width / 2 + drawPlayer.width / 2), (int)(position.Y - camera.UnscaledPosition.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 8f)) + drawPlayer.bodyPosition + new Vector2(drawPlayer.bodyFrame.Width / 2, drawPlayer.bodyFrame.Height / 2), null, Lighting.GetColor((int)(position.X + drawPlayer.width * 0.5) / 16, (int)(position.Y + drawPlayer.height * 0.5) / 16, Color.White), 0f, new Vector2(TextureAssets.Extra[37].Width() / 2, TextureAssets.Extra[37].Height() / 2), 1f, spriteEffects, 0f);
            }
        }

        private static void DrawGhost(Camera camera, Player drawPlayer, Vector2 position, float shadow = 0f)
        {
            byte mouseTextColor = Main.mouseTextColor;
            SpriteEffects effects = drawPlayer.direction != 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Color immuneAlpha = drawPlayer.GetImmuneAlpha(Lighting.GetColor((int)(drawPlayer.position.X + drawPlayer.width * 0.5) / 16, (int)(drawPlayer.position.Y + drawPlayer.height * 0.5) / 16, new Color(mouseTextColor / 2 + 100, mouseTextColor / 2 + 100, mouseTextColor / 2 + 100, mouseTextColor / 2 + 100)), shadow);
            immuneAlpha.A = (byte)(immuneAlpha.A * (1f - Math.Max(0.5f, shadow - 0.5f)));
            Rectangle value = new Rectangle(0, TextureAssets.Ghost.Height() / 4 * drawPlayer.ghostFrame, TextureAssets.Ghost.Width(), TextureAssets.Ghost.Height() / 4);
            Vector2 origin = new Vector2(value.Width * 0.5f, value.Height * 0.5f);
            camera.SpriteBatch.Draw(TextureAssets.Ghost.Value, new Vector2((int)(position.X - camera.UnscaledPosition.X + value.Width / 2), (int)(position.Y - camera.UnscaledPosition.Y + value.Height / 2)), value, immuneAlpha, 0f, origin, 1f, effects, 0f);
        }
    }
}