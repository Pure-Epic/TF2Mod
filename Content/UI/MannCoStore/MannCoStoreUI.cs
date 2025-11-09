using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Globalization;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using TF2.Common;
using TF2.Content.Items;
using TF2.Content.Items.Consumables;
using TF2.Content.NPCs.TownNPCs;
using TF2.Content.UI.HUD;

namespace TF2.Content.UI.MannCoStore
{
    [Autoload(Side = ModSide.Client)]
    internal class MannCoStoreUI : UIState
    {
        public static Color CheckoutColor => (TF2.TotalCost <= TF2.Money && TF2.ShoppingCart.Count > 0) ? new Color(115, 105, 95) : new Color(146, 71, 56);

        public static MannCoStoreFilter[] filters;
        public static int currentCategory;
        private UIText money;
        private static MannCoStoreItemGrid itemGrid;
        public static int length;
        public static MannCoStoreNavigationUI previous;
        public static MannCoStoreNavigationUI next;
        private UIText page;
        private UITextPanel<LocalizedText> checkout;

        public MannCoStoreUI()
        {
            BuildPage();
            SaxtonHale.CreateMannCoStore();
        }

        private void BuildPage()
        {
            UIElement uIElement = new UIElement
            {
                Width = StyleDimension.FromPixels(Main.screenWidth * 1.1f),
                Height = StyleDimension.FromPixels(Main.screenHeight * 1.1f),
                HAlign = 0.5f,
                VAlign = 0.5f
            };
            uIElement.SetPadding(0f);
            Append(uIElement);
            UIPanel mainPanel = new UIPanel
            {
                Width = StyleDimension.FromPixels(1152f),
                Height = StyleDimension.FromPixels(648f),
                HAlign = 0.5f,
                VAlign = 0.5f,
                BackgroundColor = new Color(17, 14, 12),
                BorderColor = new Color(3, 3, 3)
            };
            mainPanel.SetPadding(0f);
            uIElement.Append(mainPanel);
            MakeFilterButtons(mainPanel);
            UIText classes = new UIText(Language.GetText("Mods.TF2.UI.MannCoStore.Classes"), 0.5f, true)
            {
                Left = StyleDimension.FromPixels(38.5f),
                Top = StyleDimension.FromPixels(38.5f),
                TextOriginX = 0f,
                IgnoresMouseInteraction = true
            };
            mainPanel.Append(classes);
            money = new UIText("")
            {
                Left = StyleDimension.FromPixels(1036.5f),
                Top = StyleDimension.FromPixels(38.5f),
                TextOriginX = 0f,
                IgnoresMouseInteraction = true
            };
            mainPanel.Append(money);
            itemGrid = new MannCoStoreItemGrid(5)
            {
                Width = StyleDimension.FromPixels(1075f),
                Height = StyleDimension.FromPixels(405f),
                Left = StyleDimension.FromPixels(38.5f),
                Top = StyleDimension.FromPixels(121.5f),
                ListPadding = 18.75f
            };
            mainPanel.Append(itemGrid);
            MakeNavigationButtons(mainPanel);
            MannCoStoreShoppingCartButton shoppingCart = new MannCoStoreShoppingCartButton
            {
                Width = StyleDimension.FromPixels(100f),
                Height = StyleDimension.FromPixels(50f),
                Left = StyleDimension.FromPixels(38.5f),
                Top = StyleDimension.FromPixels(559.5f)
            };
            shoppingCart.OnLeftMouseDown += delegate (UIMouseEvent _, UIElement _)
            {
                TF2.MannCoStore.SetState(new MannCoStoreShoppingCartUI());
                SoundEngine.PlaySound(SoundID.MenuOpen);
            };
            shoppingCart.OnMouseOver += delegate (UIMouseEvent evt, UIElement _)
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                ((MannCoStoreShoppingCartButton)evt.Target).Texture = UITextures.MannCoStoreShopTextures[1];
            };
            shoppingCart.OnMouseOut += delegate (UIMouseEvent evt, UIElement _)
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                ((MannCoStoreShoppingCartButton)evt.Target).Texture = UITextures.MannCoStoreShopTextures[0];
            };
            mainPanel.Append(shoppingCart);
            MakeBackAndCheckoutButton(uIElement);
            if (ModContent.GetInstance<TF2ConfigClient>().WarningText)
            {
                UIText warningText = new UIText(Language.GetText("Mods.TF2.UI.MannCoStore.Warning"), 0.5f, true)
                {
                    HAlign = 0.5f,
                    VAlign = 0.0375f,
                    TextOriginX = 0f,
                    IgnoresMouseInteraction = true
                };
                warningText.SetPadding(0f);
                mainPanel.Append(warningText);
                warningText = new UIText(Language.GetText("Mods.TF2.UI.MannCoStore.Warning2"), 0.5f, true)
                {
                    HAlign = 0.5f,
                    VAlign = 0.0875f,
                    TextOriginX = 0f,
                    IgnoresMouseInteraction = true
                };
                warningText.SetPadding(0f);
                mainPanel.Append(warningText);
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (ContainsPoint(Main.MouseScreen))
                Main.LocalPlayer.mouseInterface = true;
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), new Color(0, 0, 0, 192));
        }

        public override void Update(GameTime gameTime)
        {
            if (Main.keyState.IsKeyDown(Keys.Escape) && !Main.oldKeyState.IsKeyDown(Keys.Escape))
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
                TF2.MannCoStore.SetState(null);
                Main.CloseNPCChatOrSign();
            }
            Player player = Main.LocalPlayer;
            player.releaseInventory = false;
            money.SetText(Language.GetText("Mods.TF2.UI.MannCoStore.Balance") + ": " + TF2.Money.ToString("C", CultureInfo.CurrentCulture));
            money.Left = StyleDimension.FromPixels(1036.5f - FontAssets.MouseText.Value.MeasureString(money.Text).X / 2);
            previous.end = TF2.MannCoStorePage == 1;
            next.end = TF2.MannCoStorePage == length;
            page.SetText(TF2.MannCoStorePage + "/" + length);
            if (ContainsPoint(Main.MouseScreen))
                checkout.BackgroundColor = CheckoutColor;
            base.Update(gameTime);
        }

        private static void MakeFilterButtons(UIElement outerContainer)
        {
            int filterAmount = Enum.GetNames(typeof(MannCoStoreCategory)).Length;
            filters = new MannCoStoreFilter[filterAmount];
            foreach (MannCoStoreCategory category in (MannCoStoreCategory[])Enum.GetValues(typeof(MannCoStoreCategory)))
            {
                if ((int)category >= filterAmount) break;
                filters[(int)category] = new MannCoStoreFilter(category);
                filters[(int)category].Left.Set((float)category * 35f + 38.5f, 0f);
                filters[(int)category].Top.Set(70f, 0f);
                filters[(int)category].OnLeftMouseDown += delegate (UIMouseEvent _, UIElement _)
                {
                    UpdateItemGrid();
                };
                outerContainer.Append(filters[(int)category]);
            }
        }

        private void MakeNavigationButtons(UIElement outerContainer)
        {
            previous = new MannCoStoreNavigationUI(UITextures.MannCoStoreNavigationTextures[0], false)
            {
                Width = StyleDimension.FromPixels(46f),
                Height = StyleDimension.FromPixels(46f),
                Left = StyleDimension.FromPixels(867.5f),
                Top = StyleDimension.FromPixels(555.5f),
            };
            previous.OnLeftMouseDown += delegate (UIMouseEvent _, UIElement _)
            {
                SoundEngine.PlaySound(SoundID.MenuOpen);
                if (TF2.MannCoStorePage > 1)
                    TF2.MannCoStorePage--;
            };
            previous.OnMouseOver += delegate (UIMouseEvent evt, UIElement _)
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                ((MannCoStoreNavigationUI)evt.Target).hovered = true;
            };
            previous.OnMouseOut += delegate (UIMouseEvent evt, UIElement _)
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                ((MannCoStoreNavigationUI)evt.Target).hovered = false;
            };
            outerContainer.Append(previous);
            next = new MannCoStoreNavigationUI(UITextures.MannCoStoreNavigationTextures[6], true)
            {
                Width = StyleDimension.FromPixels(46f),
                Height = StyleDimension.FromPixels(46f),
                Left = StyleDimension.FromPixels(1067.5f),
                Top = StyleDimension.FromPixels(555.5f),
            };
            next.OnLeftMouseDown += delegate (UIMouseEvent _, UIElement _)
            {
                SoundEngine.PlaySound(SoundID.MenuOpen);
                if (TF2.MannCoStorePage < length)
                    TF2.MannCoStorePage++;
            };
            next.OnMouseOver += delegate (UIMouseEvent evt, UIElement _)
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                ((MannCoStoreNavigationUI)evt.Target).hovered = true;
            };
            next.OnMouseOut += delegate (UIMouseEvent evt, UIElement _)
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                ((MannCoStoreNavigationUI)evt.Target).hovered = false;
            };
            outerContainer.Append(next);
            page = new UIText("", 0.65f, true)
            {
                Left = StyleDimension.FromPixels(967.5f),
                Top = StyleDimension.FromPixels(568.5f),
                TextOriginX = 0.5f,
                TextOriginY = 0.5f,
                IgnoresMouseInteraction = true
            };
            outerContainer.Append(page);
        }

        private void MakeBackAndCheckoutButton(UIElement outerContainer)
        {
            UITextPanel<LocalizedText> uITextPanel = new UITextPanel<LocalizedText>(Language.GetText("UI.Back"), 0.7f, true)
            {
                Width = StyleDimension.FromPixels(250f),
                Height = StyleDimension.FromPixels(50f),
                HAlign = 0.5f,
                VAlign = 0.5f,
                Left = StyleDimension.FromPixels(-451f),
                Top = StyleDimension.FromPixels(354f),
                BackgroundColor = new Color(115, 105, 95)
            };
            uITextPanel.OnLeftMouseDown += Click_GoBack;
            uITextPanel.OnMouseOver += FadedMouseOver;
            uITextPanel.OnMouseOut += FadedMouseOut;
            uITextPanel.SetSnapPoint("Back", 0, null, null);
            outerContainer.Append(uITextPanel);
            checkout = new UITextPanel<LocalizedText>(Language.GetText("Mods.TF2.UI.MannCoStore.Checkout"), 0.7f, true)
            {
                Width = StyleDimension.FromPixels(250f),
                Height = StyleDimension.FromPixels(50f),
                HAlign = 0.5f,
                VAlign = 0.5f,
                Left = StyleDimension.FromPixels(451f),
                Top = StyleDimension.FromPixels(354f),
                BackgroundColor = CheckoutColor
            };
            checkout.OnMouseOver += CheckoutMouseOver;
            checkout.OnMouseOut += CheckoutMouseOut;
            checkout.OnLeftMouseDown += Click_Checkout;
            checkout.SetSnapPoint("Checkout", 0, null, null);
            outerContainer.Append(checkout);
        }

        private void Click_GoBack(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuClose);
            TF2.MannCoStore.SetState(null);
            Main.CloseNPCChatOrSign();
        }

        private void Click_Checkout(UIMouseEvent evt, UIElement listeningElement) => BuyItems();

        public static void BuyItems()
        {
            if (TF2.TotalCost <= TF2.Money && TF2.ShoppingCart.Count > 0)
            {
                SoundEngine.PlaySound(SoundID.ResearchComplete);
                foreach (MannCoStoreItem item in TF2.ShoppingCart)
                {
                    if (item.Item.ModItem is TF2Item weapon)
                    {
                        weapon.WeaponAddQuality(TF2Item.Unique);
                        weapon.availability = TF2Item.Purchase;
                    }
                    if (item.Item2.ModItem is TF2Item weapon2)
                    {
                        weapon2.WeaponAddQuality(TF2Item.Unique);
                        weapon2.availability = TF2Item.Purchase;
                    }
                    Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_GiftOrReward(), item.Item, item.Item.stack);
                    if (item.Item2 != null && item.Item2.type != ItemID.None)
                        Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_GiftOrReward(), item.Item2, item.Item2.stack);
                }
                if (TF2.TotalCost >= 19.99f)
                    Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_GiftOrReward(), ModContent.ItemType<MannCoStorePackage>());
                TF2.Money -= TF2.TotalCost;
                TF2.ShoppingCart.Clear();
                TF2.MannCoStore.SetState(null);
            }
        }

        private void FadedMouseOver(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            ((UIPanel)evt.Target).BackgroundColor = new Color(115, 105, 95) * 1.25f;
            ((UIPanel)evt.Target).BorderColor = Colors.FancyUIFatButtonMouseOver;
        }

        private void FadedMouseOut(UIMouseEvent evt, UIElement listeningElement)
        {
            ((UIPanel)evt.Target).BackgroundColor = new Color(115, 105, 95);
            ((UIPanel)evt.Target).BorderColor = Color.Black;
        }

        private void CheckoutMouseOver(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            ((UIPanel)evt.Target).BackgroundColor = CheckoutColor * 1.25f;
            ((UIPanel)evt.Target).BorderColor = Colors.FancyUIFatButtonMouseOver;
        }

        private void CheckoutMouseOut(UIMouseEvent evt, UIElement listeningElement)
        {
            ((UIPanel)evt.Target).BackgroundColor = CheckoutColor;
            ((UIPanel)evt.Target).BorderColor = Color.Black;
        }

        public static void UpdateItemGrid()
        {
            itemGrid.Clear();
            List<MannCoStoreCategory> list = new List<MannCoStoreCategory>();
            foreach (MannCoStoreFilter category in filters)
            {
                if (category.enabled)
                    list.Add(category.Category);
            }
            foreach (KeyValuePair<MannCoStoreCategory, List<MannCoStoreItem>> keyValuePair in SaxtonHale.Inventory)
            {
                if (list.Contains(keyValuePair.Key))
                {
                    for (int i = 0; i < SaxtonHale.Inventory[keyValuePair.Key].Count; i++)
                    {
                        if (SaxtonHale.Inventory[keyValuePair.Key][i].Visible)
                        {
                            MannCoStoreItemSlot slot = new MannCoStoreItemSlot(keyValuePair.Key, SaxtonHale.Inventory[keyValuePair.Key][i], i, 1f);
                            slot.OnLeftMouseDown += delegate (UIMouseEvent _, UIElement _)
                            {
                                SoundEngine.PlaySound(SoundID.CoinPickup);
                                TF2.ShoppingCart.Add(slot.Item);
                            };
                            slot.OnMouseOver += delegate (UIMouseEvent evt, UIElement _)
                            {
                                SoundEngine.PlaySound(SoundID.MenuTick);
                                ((MannCoStoreItemSlot)evt.Target).Texture = UITextures.MannCoStoreBackgroundTextures[1];
                            };
                            slot.OnMouseOut += delegate (UIMouseEvent evt, UIElement _)
                            {
                                ((MannCoStoreItemSlot)evt.Target).Texture = UITextures.MannCoStoreBackgroundTextures[0];
                            };
                            itemGrid.Items.Add(slot);
                            itemGrid.InnerList.Append(slot);
                        }
                    }
                }
            }
            length = (int)Math.Ceiling((float)itemGrid.Items.Count / 15);
            if (length < 1)
                length = 1;
            itemGrid.InnerList.Recalculate();
        }
    }

    internal class MannCoStoreFilter(MannCoStoreCategory category) : UIElement
    {
        public readonly MannCoStoreCategory Category = category;
        public bool enabled = true;
        private static readonly Asset<Texture2D> Texture = UITextures.MannCoStoreNavigationTextures[11];

        public override void OnInitialize()
        {
            Width.Set(30f, 0f);
            Height.Set(30f, 0f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (ContainsPoint(Main.MouseScreen))
                Main.LocalPlayer.mouseInterface = true;
            CalculatedStyle innerDimensions = GetInnerDimensions();
            spriteBatch.Draw(Texture.Value, innerDimensions.Center(), new Rectangle?(new Rectangle(Texture.Value.Width / 10 * (int)Category, enabled ? (Texture.Value.Height / 2) : 0, Texture.Value.Width / 10, Texture.Value.Height / 2)), Color.White, 0f, new Vector2(Texture.Value.Width / (float)10, Texture.Value.Height / 2f) * 0.5f, 1f, 0, 0f);
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            if ((int)Category != MannCoStoreUI.currentCategory)
                TF2.MannCoStorePage = 1;
            if (Category == MannCoStoreCategory.All || MannCoStoreUI.currentCategory == (int)Category)
            {
                MannCoStoreUI.currentCategory = 0;
                foreach (MannCoStoreFilter category in MannCoStoreUI.filters)
                    category.enabled = true;
            }
            else
            {
                MannCoStoreUI.currentCategory = (int)Category;
                foreach (MannCoStoreFilter category in MannCoStoreUI.filters)
                    category.enabled = category.Category == Category;
            }
            SoundEngine.PlaySound(SoundID.MenuTick, null, null);
            base.LeftMouseDown(evt);
        }
    }

    internal class MannCoStoreItemGrid : UIGrid
    {
        public MannCoStoreItemGrid(int columns = 1)
        {
            _cols = columns;
            InnerList.OverflowHidden = false;
            InnerList.Width.Set(0f, 1f);
            InnerList.Height.Set(0f, 1f);
            OverflowHidden = true;
            Append(InnerList);
        }

        public override void RecalculateChildren()
        {
            base.RecalculateChildren();
            float num = 0f;
            float num2 = 0f;
            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].Top.Set(num, 0f);
                Items[i].Left.Set(num2, 0f);
                Items[i].Recalculate();
                if (i % _cols == _cols - 1)
                {
                    num += Items[i].GetOuterDimensions().Height + ListPadding;
                    num2 = 0f;
                }
                else
                    num2 += Items[i].GetOuterDimensions().Width + ListPadding;
            }
            if (Items.Count > 0)
                num += ListPadding + Items[0].GetOuterDimensions().Height;
            _innerListHeight = num;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            InnerList.Top.Set((TF2.MannCoStorePage - 1) * -416.25f, 0f);
            Recalculate();
        }
    }

    internal class MannCoStoreItemSlot : UIElement
    {
        public Asset<Texture2D> Texture = UITextures.MannCoStoreBackgroundTextures[0];
        public readonly MannCoStoreItem Item;
        public readonly MannCoStoreCategory Category;
        public readonly int ListID;
        private readonly float _scale;
        private readonly UIText cost;

        public MannCoStoreItemSlot(MannCoStoreCategory category, MannCoStoreItem item, int listID, float scale)
        {
            Item = item;
            Category = category;
            ListID = listID;
            Width.Set(Texture.Width() * scale, 0f);
            Height.Set(Texture.Height() * scale, 0f);
            _scale = scale;
            cost = new UIText("")
            {
                HAlign = 0.5f,
                VAlign = 0.5f,
                Left = StyleDimension.FromPixels(90f),
                Top = StyleDimension.FromPixels(45f),
                TextOriginX = 0f,
                IgnoresMouseInteraction = true
            };
            cost.SetText(Item.Cost.ToString("C", CultureInfo.CurrentCulture));
            cost.Left = StyleDimension.FromPixels(90f - FontAssets.MouseText.Value.MeasureString(cost.Text).X / 2);
            Append(cost);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (ContainsPoint(Main.MouseScreen))
                Main.LocalPlayer.mouseInterface = true;
            spriteBatch.Draw(Texture.Value, GetDimensions().Position(), null, Color.White, 0f, Vector2.Zero, _scale, 0, 0f);
            DrawItemTexture(spriteBatch);
            if (Item.Item.stack > 1)
            {
                Vector2 vector = GetDimensions().Position() + new Vector2(10f, 24f);
                Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.MouseText.Value, Item.Item.stack.ToString(), vector.X, vector.Y, Color.White, Color.Black, Vector2.Zero, _scale);
            }
            base.DrawSelf(spriteBatch);
            if (IsMouseHovering && Main.mouseItem.type == ItemID.None)
            {
                Main.hoverItemName = Item.Item.Name;
                Main.HoverItem = Item.Item.Clone();
                Main.instance.MouseText(Main.hoverItemName, 0, 0, -1, -1, -1, -1, 0);
            }
        }

        public void DrawItemTexture(SpriteBatch spriteBatch)
        {
            Item item = Item.Item;
            CalculatedStyle innerDimensions = GetInnerDimensions();
            Main.instance.LoadItem(item.type);
            Texture2D texture = TextureAssets.Item[item.type].Value;
            Rectangle rectangle = (Main.itemAnimations[item.type] == null) ? texture.Frame(1, 1, 0, 0, 0, 0) : Main.itemAnimations[item.type].GetFrame(texture, -1);
            float num = 3.5f;
            if (rectangle.Width * num > 100 || rectangle.Height * num > 100)
                num = (rectangle.Width <= rectangle.Height) ? (100f / rectangle.Height) : (100f / rectangle.Width);
            spriteBatch.Draw(texture, innerDimensions.Center(), new Rectangle?(rectangle), Color.White, 0f, new Vector2(rectangle.Width, rectangle.Height) * 0.5f, num, 0, 0f);
        }
    }

    internal class MannCoStoreNavigationUI(Asset<Texture2D> texture, bool next) : UIElement
    {
        public Asset<Texture2D> Texture = texture;
        private readonly bool _next = next;
        public bool end = true;
        public bool hovered;

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (ContainsPoint(Main.MouseScreen))
                Main.LocalPlayer.mouseInterface = true;
            spriteBatch.Draw(Texture.Value, GetDimensions().Position(), null, Color.White, 0f, Vector2.Zero, 1f, 0, 0f);
        }

        public override void Update(GameTime gameTime) => Texture = !_next
                ? (end ? (!hovered ? UITextures.MannCoStoreNavigationTextures[0] : UITextures.MannCoStoreNavigationTextures[1]) : (!hovered ? UITextures.MannCoStoreNavigationTextures[2] : UITextures.MannCoStoreNavigationTextures[3]))
                : (!end ? (!hovered ? UITextures.MannCoStoreNavigationTextures[4] : UITextures.MannCoStoreNavigationTextures[5]) : (!hovered ? UITextures.MannCoStoreNavigationTextures[6] : UITextures.MannCoStoreNavigationTextures[7]));
    }

    internal class MannCoStoreShoppingCartButton : UIElement
    {
        public Asset<Texture2D> Texture = UITextures.MannCoStoreShopTextures[0];
        private readonly UIText items;

        public MannCoStoreShoppingCartButton()
        {
            items = new UIText("0", 0.75f, true)
            {
                HAlign = 0.825f,
                VAlign = 0.5f,
                IgnoresMouseInteraction = true
            };
            items.SetText("0");
            Append(items);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (ContainsPoint(Main.MouseScreen))
                Main.LocalPlayer.mouseInterface = true;
            spriteBatch.Draw(Texture.Value, GetDimensions().Position(), null, Color.White, 0f, Vector2.Zero, 1f, 0, 0f);
        }

        public override void Update(GameTime gameTime) => items.SetText(TF2.ShoppingCart.Count.ToString());
    }

    internal class MannCoStoreShoppingCartUI : UIState
    {
        private static UIGrid itemGrid;
        public static UIScrollbar scrollBar;
        private UITextPanel<LocalizedText> checkout;
        private UIText totalCost;
        private UIText emptyText;

        public MannCoStoreShoppingCartUI() => BuildPage();

        private void BuildPage()
        {
            UIElement uIElement = new UIElement
            {
                Width = StyleDimension.FromPixels(Main.screenWidth * 1.1f),
                Height = StyleDimension.FromPixels(Main.screenHeight * 1.1f),
                HAlign = 0.5f,
                VAlign = 0.5f
            };
            uIElement.SetPadding(0f);
            Append(uIElement);
            UIPanel mainPanel = new UIPanel
            {
                Width = StyleDimension.FromPixels(1152f),
                Height = StyleDimension.FromPixels(648f),
                HAlign = 0.5f,
                VAlign = 0.5f,
                BackgroundColor = new Color(17, 14, 12),
                BorderColor = new Color(3, 3, 3)
            };
            mainPanel.SetPadding(0f);
            uIElement.Append(mainPanel);
            itemGrid = new UIGrid
            {
                Width = StyleDimension.FromPixels(1075f),
                Height = StyleDimension.FromPixels(495f),
                Left = StyleDimension.FromPixels(38.5f),
                Top = StyleDimension.FromPixels(38.5f),
                ListPadding = 5f
            };
            mainPanel.Append(itemGrid);
            int length = TF2.ShoppingCart.Count;
            if (length > 4)
            {
                scrollBar = new UIScrollbar
                {
                    Left = StyleDimension.FromPixels(1123.125f),
                    Top = StyleDimension.FromPixels(38.5f),
                    Height = StyleDimension.FromPixels(495f),
                };
                scrollBar.SetView(100f, 1000f);
                scrollBar.OnMouseOver += delegate (UIMouseEvent evt, UIElement _)
                {
                    ((UIScrollbar)evt.Target).texture = UITextures.MannCoStoreNavigationTextures[10];
                };
                scrollBar.OnMouseOut += delegate (UIMouseEvent evt, UIElement _)
                {
                    ((UIScrollbar)evt.Target).texture = UITextures.MannCoStoreNavigationTextures[9];
                };
                mainPanel.Append(scrollBar);
                itemGrid.SetScrollbar(scrollBar);
            }
            totalCost = new UIText("", 0.55f, true)
            {
                HAlign = 0.5f,
                VAlign = 0.925f,
                Left = StyleDimension.FromPixels(450f),
                TextOriginX = 0f,
                IgnoresMouseInteraction = true
            };
            mainPanel.Append(totalCost);
            emptyText = new UIText("", 0.85f, true)
            {
                HAlign = 0.5f,
                VAlign = 0.5f,
                IgnoresMouseInteraction = true
            };
            mainPanel.Append(emptyText);
            MakeContinueShoppingAndCheckoutButton(uIElement);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (ContainsPoint(Main.MouseScreen))
                Main.LocalPlayer.mouseInterface = true;
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), new Color(0, 0, 0, 192));
        }

        public override void Update(GameTime gameTime)
        {
            if (Main.keyState.IsKeyDown(Keys.Escape) && !Main.oldKeyState.IsKeyDown(Keys.Escape))
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
                TF2.MannCoStore.SetState(new MannCoStoreUI());
            }
            if (TF2.ShoppingCart.Count > 0)
            {
                float cost = 0f;
                List<MannCoStoreItem> shoppingCart = TF2.ShoppingCart;
                for (int i = 0; i < shoppingCart.Count; i++)
                    cost += shoppingCart[i].Cost;
                totalCost.SetText(Language.GetText("Mods.TF2.UI.MannCoStore.Total") + ": " + cost.ToString("C", CultureInfo.CurrentCulture));
                totalCost.Left = StyleDimension.FromPixels(450f - FontAssets.MouseText.Value.MeasureString(totalCost.Text).X / 2);
            }
            if (TF2.ShoppingCart.Count > 0)
                emptyText.SetText("");
            else
            {
                emptyText.SetText(Language.GetText("Mods.TF2.UI.MannCoStore.EmptyShoppingCart"));
                totalCost.SetText("");
            }
            if (ContainsPoint(Main.MouseScreen))
                checkout.BackgroundColor = MannCoStoreUI.CheckoutColor;
            base.Update(gameTime);
        }

        private void MakeContinueShoppingAndCheckoutButton(UIElement outerContainer)
        {
            UITextPanel<LocalizedText> uITextPanel = new UITextPanel<LocalizedText>(Language.GetText("Mods.TF2.UI.MannCoStore.ContinueShopping"), 0.6f, true)
            {
                Width = StyleDimension.FromPixels(250f),
                Height = StyleDimension.FromPixels(50f),
                HAlign = 0.5f,
                VAlign = 0.5f,
                Left = StyleDimension.FromPixels(-451f),
                Top = StyleDimension.FromPixels(354f),
                BackgroundColor = new Color(115, 105, 95)
            };
            uITextPanel.OnLeftMouseDown += Click_GoBack;
            uITextPanel.OnMouseOver += FadedMouseOver;
            uITextPanel.OnMouseOut += FadedMouseOut;
            uITextPanel.SetSnapPoint("Continue Shopping", 0, null, null);
            outerContainer.Append(uITextPanel);
            checkout = new UITextPanel<LocalizedText>(Language.GetText("Mods.TF2.UI.MannCoStore.Checkout"), 0.7f, true)
            {
                Width = StyleDimension.FromPixels(250f),
                Height = StyleDimension.FromPixels(50f),
                HAlign = 0.5f,
                VAlign = 0.5f,
                Left = StyleDimension.FromPixels(451f),
                Top = StyleDimension.FromPixels(354f),
                BackgroundColor = MannCoStoreUI.CheckoutColor
            };
            checkout.OnMouseOver += CheckoutMouseOver;
            checkout.OnMouseOut += CheckoutMouseOut;
            checkout.OnLeftMouseDown += Click_Checkout;
            checkout.SetSnapPoint("Checkout", 0, null, null);
            outerContainer.Append(checkout);
            UpdateItemGrid();
        }

        private void Click_GoBack(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuClose);
            TF2.MannCoStore.SetState(new MannCoStoreUI());
        }

        private void Click_Checkout(UIMouseEvent evt, UIElement listeningElement) => MannCoStoreUI.BuyItems();

        private void FadedMouseOver(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            ((UIPanel)evt.Target).BackgroundColor = new Color(115, 105, 95) * 1.25f;
            ((UIPanel)evt.Target).BorderColor = Colors.FancyUIFatButtonMouseOver;
        }

        private void FadedMouseOut(UIMouseEvent evt, UIElement listeningElement)
        {
            ((UIPanel)evt.Target).BackgroundColor = new Color(115, 105, 95);
            ((UIPanel)evt.Target).BorderColor = Color.Black;
        }

        private void CheckoutMouseOver(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            ((UIPanel)evt.Target).BackgroundColor = MannCoStoreUI.CheckoutColor * 1.25f;
            ((UIPanel)evt.Target).BorderColor = Colors.FancyUIFatButtonMouseOver;
        }

        private void CheckoutMouseOut(UIMouseEvent evt, UIElement listeningElement)
        {
            ((UIPanel)evt.Target).BackgroundColor = MannCoStoreUI.CheckoutColor;
            ((UIPanel)evt.Target).BorderColor = Color.Black;
        }

        public static void UpdateItemGrid()
        {
            itemGrid.Clear();
            List<MannCoStoreItem> shoppingCart = TF2.ShoppingCart;
            for (int i = 0; i < shoppingCart.Count; i++)
            {
                MannCoStoreShoppingCartItem listItem = new MannCoStoreShoppingCartItem(shoppingCart[i], i);
                itemGrid.Items.Add(listItem);
                itemGrid.InnerList.Append(listItem);
            }
            itemGrid.UpdateOrder();
            itemGrid.InnerList.Recalculate();
        }
    }

    internal class MannCoStoreShoppingCartItem : UIElement
    {
        public Asset<Texture2D> Texture = UITextures.MannCoStoreShopTextures[2];
        public readonly MannCoStoreItem Item;
        public readonly int ListID;
        private readonly UIText name;
        private readonly UIText cost;

        public MannCoStoreShoppingCartItem(MannCoStoreItem item, int listID)
        {
            Item = item;
            ListID = listID;
            Width.Set(Texture.Width(), 0f);
            Height.Set(Texture.Height(), 0f);
            name = new UIText("", 0.5f, true)
            {
                HAlign = 0.3625f,
                VAlign = 0.5f,
                TextColor = new Color(255, 215, 0),
                IgnoresMouseInteraction = true
            };
            name.SetText(Item.Item.ModItem is TF2Item weapon ? weapon.GetItemName() : Item.Item.HoverName);
            Append(name);
            UITextPanel<LocalizedText> removeButton = new UITextPanel<LocalizedText>(Language.GetText("Mods.TF2.UI.MannCoStore.Remove"), 0.6f, true)
            {
                Width = StyleDimension.FromPixels(125f),
                Height = StyleDimension.FromPixels(25f),
                HAlign = 0.75f,
                VAlign = 0.5f,
                BackgroundColor = new Color(115, 105, 95),
                BorderColor = new Color(115, 105, 95)
            };
            removeButton.OnLeftMouseDown += delegate (UIMouseEvent _, UIElement _)
            {
                SoundEngine.PlaySound(SoundID.MenuOpen);
                List<MannCoStoreItem> shoppingCart = TF2.ShoppingCart;
                shoppingCart.RemoveAt(ListID);
                if (shoppingCart.Count <= 4 && MannCoStoreShoppingCartUI.scrollBar != null)
                {
                    MannCoStoreShoppingCartUI.scrollBar.Deactivate();
                    MannCoStoreShoppingCartUI.scrollBar.Remove();
                }
                MannCoStoreShoppingCartUI.UpdateItemGrid();
            };
            removeButton.OnMouseOver += delegate (UIMouseEvent evt, UIElement _)
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                ((UITextPanel<LocalizedText>)evt.Target).BackgroundColor = ((UITextPanel<LocalizedText>)evt.Target).BorderColor = new Color(146, 71, 56);
            };
            removeButton.OnMouseOut += delegate (UIMouseEvent evt, UIElement _)
            {
                ((UITextPanel<LocalizedText>)evt.Target).BackgroundColor = ((UITextPanel<LocalizedText>)evt.Target).BorderColor = new Color(115, 105, 95);
            };
            removeButton.SetSnapPoint("Back", 0, null, null);
            Append(removeButton);
            cost = new UIText("", 0.55f, true)
            {
                HAlign = 0.5f,
                VAlign = 0.5f,
                Left = StyleDimension.FromPixels(450f),
                TextOriginX = 0f,
                IgnoresMouseInteraction = true
            };
            cost.SetText(Item.Cost.ToString("C", CultureInfo.CurrentCulture));
            cost.Left = StyleDimension.FromPixels(450f - FontAssets.MouseText.Value.MeasureString(cost.Text).X / 2);
            Append(cost);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (ContainsPoint(Main.MouseScreen))
                Main.LocalPlayer.mouseInterface = true;
            spriteBatch.Draw(Texture.Value, GetDimensions().Position(), null, Color.White, 0f, Vector2.Zero, 1f, 0, 0f);
            DrawItemTexture(spriteBatch);
            if (Item.Item.stack > 1)
            {
                Vector2 vector = GetDimensions().Position() + new Vector2(10f, 24f);
                Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.MouseText.Value, Item.Item.stack.ToString(), vector.X, vector.Y, Color.White, Color.Black, Vector2.Zero, 1f);
            }
            base.DrawSelf(spriteBatch);
        }

        public void DrawItemTexture(SpriteBatch spriteBatch)
        {
            CalculatedStyle innerDimensions = GetInnerDimensions();
            Main.instance.LoadItem(Item.Item.type);
            Texture2D value = TextureAssets.Item[Item.Item.type].Value;
            Rectangle rectangle = (Main.itemAnimations[Item.Item.type] == null) ? value.Frame(1, 1, 0, 0, 0, 0) : Main.itemAnimations[Item.Item.type].GetFrame(value, -1);
            float num = 3.5f;
            if (rectangle.Width * num > 100 || rectangle.Height * num > 100)
                num = (rectangle.Width <= rectangle.Height) ? (100f / rectangle.Height) : (100f / rectangle.Width);
            spriteBatch.Draw(value, new Vector2(innerDimensions.X + 145f, innerDimensions.Center().Y), new Rectangle?(rectangle), Color.White, 0f, new Vector2(rectangle.Width, rectangle.Height) * 0.5f, num, 0, 0f);
        }
    }

    internal class UIGrid : UIElement
    {
        public int Count => Items.Count;

        public float ListPadding = 5f;
        public readonly List<UIElement> Items = new List<UIElement>();
        internal readonly UIElement InnerList = new UIInnerList();
        protected float _innerListHeight;
        protected int _cols;
        public static UIScrollbar scrollbar;

        public delegate bool ElementSearchMethod(UIElement element);

        public UIGrid(int columns = 1)
        {
            _cols = columns;
            InnerList.OverflowHidden = false;
            InnerList.Width.Set(0f, 1f);
            InnerList.Height.Set(0f, 1f);
            OverflowHidden = true;
            Append(InnerList);
        }

        public float GetTotalHeight() => _innerListHeight;

        public void Goto(ElementSearchMethod searchMethod, bool center = false)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (searchMethod(Items[i]))
                {
                    scrollbar.ViewPosition = Items[i].Top.Pixels;
                    if (center)
                        scrollbar.ViewPosition = Items[i].Top.Pixels - GetInnerDimensions().Height / 2f + Items[i].GetOuterDimensions().Height / 2f;
                    return;
                }
            }
        }

        public virtual void Add(UIElement item)
        {
            Items.Add(item);
            InnerList.Append(item);
            UpdateOrder();
            InnerList.Recalculate();
        }

        public virtual bool Remove(UIElement item)
        {
            InnerList.RemoveChild(item);
            UpdateOrder();
            return Items.Remove(item);
        }

        public virtual void Clear()
        {
            InnerList.RemoveAllChildren();
            Items.Clear();
        }

        public override void Recalculate()
        {
            base.Recalculate();
            UpdateScrollbar();
        }

        public override void ScrollWheel(UIScrollWheelEvent evt)
        {
            base.ScrollWheel(evt);
            if (scrollbar != null)
                scrollbar.ViewPosition -= evt.ScrollWheelValue;
        }

        public override void RecalculateChildren()
        {
            base.RecalculateChildren();
            float num = 0f;
            float num2 = 0f;
            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].Top.Set(num, 0f);
                Items[i].Left.Set(num2, 0f);
                Items[i].Recalculate();
                if (i % _cols == _cols - 1)
                {
                    num += Items[i].GetOuterDimensions().Height + ListPadding;
                    num2 = 0f;
                }
                else
                    num2 += Items[i].GetOuterDimensions().Width + ListPadding;
            }
            if (Items.Count > 0 && _cols > 1)
                num += ListPadding + Items[0].GetOuterDimensions().Height;
            _innerListHeight = num;
        }

        private void UpdateScrollbar()
        {
            if (scrollbar == null) return;
            scrollbar.SetView(GetInnerDimensions().Height, _innerListHeight);
        }

        public void SetScrollbar(UIScrollbar scrollbar)
        {
            UIGrid.scrollbar = scrollbar;
            UpdateScrollbar();
        }

        public void UpdateOrder() => UpdateScrollbar();

        public static int SortMethod(UIElement item1, UIElement item2) => item1.CompareTo(item2);

        public override List<SnapPoint> GetSnapPoints()
        {
            List<SnapPoint> list = new List<SnapPoint>();
            if (GetSnapPoint(out SnapPoint item))
                list.Add(item);
            foreach (UIElement uielement in Items)
                list.AddRange(uielement.GetSnapPoints());
            return list;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            InnerList.Top.Set(scrollbar != null ? -scrollbar.GetValue() : 0f, 0f);
            Recalculate();
        }

        protected class UIInnerList : UIElement
        {
            public override bool ContainsPoint(Vector2 point) => true;

            protected override void DrawChildren(SpriteBatch spriteBatch)
            {
                Vector2 position = Parent.GetDimensions().Position();
                Vector2 dimensions = new Vector2(Parent.GetDimensions().Width, Parent.GetDimensions().Height);
                foreach (UIElement uielement in Elements)
                {
                    Vector2 position2 = uielement.GetDimensions().Position();
                    Vector2 dimensions2 = new Vector2(uielement.GetDimensions().Width, uielement.GetDimensions().Height);
                    if (Collision.CheckAABBvAABBCollision(position, dimensions, position2, dimensions2))
                        uielement.Draw(spriteBatch);
                }
            }
        }
    }

    internal class UIScrollbar : UIElement
    {
        private float _viewPosition;
        private float _viewSize = 1f;
        private float _maxViewSize = 20f;
        private bool _isDragging;
        private bool _isHoveringOverHandle;
        private float _dragYOffset;
        private readonly Asset<Texture2D> _texture;
        public Asset<Texture2D> texture;

        public float ViewPosition
        {
            get => _viewPosition;
            set => _viewPosition = MathHelper.Clamp(value, 0f, _maxViewSize - _viewSize);
        }

        public bool CanScroll => _maxViewSize != _viewSize;

        public float ViewSize => _viewSize;

        public float MaxViewSize => _maxViewSize;

        public void GoToBottom() => ViewPosition = _maxViewSize - _viewSize;

        public UIScrollbar()
        {
            Width.Set(20f, 0f);
            MaxWidth.Set(20f, 0f);
            _texture = UITextures.MannCoStoreNavigationTextures[8];
            texture = UITextures.MannCoStoreNavigationTextures[9];
            PaddingTop = 5f;
            PaddingBottom = 5f;
        }

        public void SetView(float viewSize, float maxViewSize)
        {
            viewSize = MathHelper.Clamp(viewSize, 0f, maxViewSize);
            _viewPosition = MathHelper.Clamp(_viewPosition, 0f, maxViewSize - viewSize);
            _viewSize = viewSize;
            _maxViewSize = maxViewSize;
        }

        public float GetValue() => _viewPosition;

        private Rectangle GetHandleRectangle()
        {
            CalculatedStyle innerDimensions = GetInnerDimensions();
            if (_maxViewSize == 0f && _viewSize == 0f)
            {
                _viewSize = 1f;
                _maxViewSize = 1f;
            }
            return new Rectangle((int)innerDimensions.X, (int)(innerDimensions.Y + innerDimensions.Height * (_viewPosition / _maxViewSize)) - 3, 20, (int)(innerDimensions.Height * (_viewSize / _maxViewSize)) + 7);
        }

        public static void DrawBar(SpriteBatch spriteBatch, Texture2D texture, Rectangle dimensions, Color color)
        {
            spriteBatch.Draw(texture, new Rectangle(dimensions.X, dimensions.Y - 6, dimensions.Width, 6), new Rectangle(0, 0, texture.Width, 6), color);
            spriteBatch.Draw(texture, new Rectangle(dimensions.X, dimensions.Y, dimensions.Width, dimensions.Height), new Rectangle(0, 6, texture.Width, 4), color);
            spriteBatch.Draw(texture, new Rectangle(dimensions.X, dimensions.Y + dimensions.Height, dimensions.Width, 6), new Rectangle(0, texture.Height - 6, texture.Width, 6), color);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetDimensions();
            CalculatedStyle innerDimensions = GetInnerDimensions();
            if (_isDragging)
            {
                float num = UserInterface.ActiveInstance.MousePosition.Y - innerDimensions.Y - _dragYOffset;
                _viewPosition = MathHelper.Clamp(num / innerDimensions.Height * _maxViewSize, 0f, _maxViewSize - _viewSize);
            }

            Rectangle handleRectangle = GetHandleRectangle();
            Vector2 mousePosition = UserInterface.ActiveInstance.MousePosition;
            bool isHoveringOverHandle = _isHoveringOverHandle;
            _isHoveringOverHandle = handleRectangle.Contains(new Point((int)mousePosition.X, (int)mousePosition.Y));
            if (!isHoveringOverHandle && _isHoveringOverHandle && Main.hasFocus)
                SoundEngine.PlaySound(SoundID.MenuTick);

            DrawBar(spriteBatch, _texture.Value, dimensions.ToRectangle(), Color.White);
            DrawBar(spriteBatch, texture.Value, handleRectangle, Color.White * ((_isDragging || _isHoveringOverHandle) ? 1f : 0.85f));
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            base.LeftMouseDown(evt);
            if (evt.Target == this)
            {
                Rectangle handleRectangle = GetHandleRectangle();
                if (handleRectangle.Contains(new Point((int)evt.MousePosition.X, (int)evt.MousePosition.Y)))
                {
                    _isDragging = true;
                    _dragYOffset = evt.MousePosition.Y - handleRectangle.Y;
                }
                else
                {
                    CalculatedStyle innerDimensions = GetInnerDimensions();
                    float num = UserInterface.ActiveInstance.MousePosition.Y - innerDimensions.Y - (handleRectangle.Height >> 1);
                    _viewPosition = MathHelper.Clamp(num / innerDimensions.Height * _maxViewSize, 0f, _maxViewSize - _viewSize);
                }
            }
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);
            _isDragging = false;
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            PlayerInput.LockVanillaMouseScroll("ModLoader/UIScrollbar");
        }

        public override void OnDeactivate() => UIGrid.scrollbar = null;
    }

    internal class ManCoStoreSystem : ModSystem
    {
        public override void UpdateUI(GameTime gameTime) => TF2.MannCoStore?.Update(gameTime);

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) => TF2HUDSystem.DrawHUD(layers, TF2.MannCoStore);
    }
}