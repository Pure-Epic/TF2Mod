using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.Utilities;
using TF2.Common;

namespace TF2.Content.Items
{
    public abstract class TF2Item : ModItem
    {
        public int classType;
        public int weaponType;
        public int availability;
        public const int MultiClass = 0;
        public const int Scout = 1;
        public const int Soldier = 2;
        public const int Pyro = 3;
        public const int Demoman = 4;
        public const int Heavy = 5;
        public const int Engineer = 6;
        public const int Medic = 7;
        public const int Sniper = 8;
        public const int Spy = 9;
        public const int Primary = 1;
        public const int Secondary = 2;
        public const int Melee = 3;
        public const int PDA = 4;
        public const int PDA2 = 5;
        protected const int Starter = 1;
        protected const int Unlock = 2;
        protected const int Craft = 3;
        protected const int Contract = 4;
        protected const int Exclusive = 5;
        protected const int Stock = 1;
        protected const int Unique = 2;
        protected const int Vintage = 3;
        protected const int Genuine = 4;
        protected const int Strange = 5;
        protected const int Unusual = 6;

        protected int metalValue;
        public int[] timer = new int[5];
        protected readonly HashSet<int> classHashSet = new HashSet<int>();
        protected readonly HashSet<int> qualityHashSet = new HashSet<int>();
        protected readonly int[] qualityTypes = new int[]
        {
            ItemRarityID.White,
            ModContent.RarityType<NormalRarity>(),
            ModContent.RarityType<UniqueRarity>(),
            0,
            0,
            0,
            ModContent.RarityType<UnusualRarity>()
        };
        public bool equipped;

        public enum Classes
        {
            Classless,
            Scout,
            Soldier,
            Pyro,
            Demoman,
            Heavy,
            Engineer,
            Medic,
            Sniper,
            Spy
        }

        public enum Availability
        {
            Exotic,
            Starter,
            Unlocked,
            Crafted,
            Contract,
            Exclusive
        }

        public enum WeaponCategory
        {
            Weapon,
            Primary,
            Secondary,
            Melee,
            PDA,
            PDA2
        }

        protected virtual int WeaponResearchCost() => availability switch
        {
            Starter => 1,
            Unlock or Craft or Contract => 5,
            Exclusive => 10,
            _ => 0,
        };

        protected virtual void WeaponStatistics()
        { }

        protected virtual void WeaponDescription(List<TooltipLine> description)
        { }

        protected virtual void WeaponAddQuality(int quality)
        {
            if (quality <= 0 || quality > Unusual)
                quality = 0;
            if (qualityHashSet.Add(quality) && qualityHashSet.Max() <= quality)
                Item.rare = qualityTypes[quality];
        }

        protected virtual bool WeaponDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) => true;

        protected virtual bool WeaponDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) => true;

        protected virtual bool WeaponModifyDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) => true;

        protected virtual void WeaponEarlyUpdate(Player player)
        { }

        protected virtual void SetWeaponSlot(int weaponCategory)
        { }

        protected void SetWeaponClass(int[] classes)
        {
            foreach (int i in classes)
            {
                if (i > 0 && i <= Spy)
                    classHashSet.Add(i);
            }
        }

        protected bool EquippedWeaponType(Player player)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            return (p.primaryEquipped && weaponType == Primary) || (p.secondaryEquipped && weaponType == Secondary) || (p.pdaEquipped && weaponType == PDA);
        }

        protected static void RemoveDefaultTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine nameTooltip = tooltips.FirstOrDefault(x => x.Name == "ItemName" && x.Mod == "Terraria");
            tooltips.Remove(nameTooltip);
            TooltipLine materialTooltip = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(materialTooltip);
            TooltipLine damageTooltip = tooltips.FirstOrDefault(x => x.Name == "Damage" && x.Mod == "Terraria");
            tooltips.Remove(damageTooltip);
            TooltipLine speedTooltip = tooltips.FirstOrDefault(x => x.Name == "Speed" && x.Mod == "Terraria");
            tooltips.Remove(speedTooltip);
            TooltipLine knockbackTooltip = tooltips.FirstOrDefault(x => x.Name == "Knockback" && x.Mod == "Terraria");
            tooltips.Remove(knockbackTooltip);
            TooltipLine consumableTooltip = tooltips.FirstOrDefault(x => x.Name == "Consumable" && x.Mod == "Terraria");
            tooltips.Remove(consumableTooltip);
            TooltipLine equipableTooltip = tooltips.FirstOrDefault(x => x.Name == "Equipable" && x.Mod == "Terraria");
            tooltips.Remove(equipableTooltip);
            TooltipLine favoriteTooltip = tooltips.FirstOrDefault(x => x.Name == "Favorite" && x.Mod == "Terraria");
            tooltips.Remove(favoriteTooltip);
            TooltipLine favoriteDescriptionTooltip = tooltips.FirstOrDefault(x => x.Name == "FavoriteDesc" && x.Mod == "Terraria");
            tooltips.Remove(favoriteDescriptionTooltip);
            TooltipLine noTransferTooltip = tooltips.FirstOrDefault(x => x.Name == "NoTransfer" && x.Mod == "Terraria");
            tooltips.Remove(noTransferTooltip);
        }

        protected void AddName(List<TooltipLine> description)
        {
            List<string> lines2 = StringSplitter(GetItemName());
            for (int i = 0; i < lines2.Count; i++)
            {
                TooltipLine line = new TooltipLine(Mod, "Name", lines2[i])
                {
                    OverrideColor = GetItemColor()
                };
                description.Insert(i, line);
            }
        }

        protected void AddHeader(List<TooltipLine> description)
        {
            string text = (string)this.GetLocalization("Header");
            List<string> lines = TooltipSplitter(text);
            foreach (string tooltip in lines)
            {
                List<string> lines2 = StringSplitter(tooltip);
                for (int i = 0; i < lines2.Count; i++)
                {
                    TooltipLine line = new TooltipLine(Mod, "Header", lines2[i])
                    {
                        OverrideColor = new Color(235, 226, 202)
                    };
                    description.Add(line);
                }
            }
        }

        protected void AddPositiveAttribute(List<TooltipLine> description)
        {
            string text = (string)this.GetLocalization("Upsides");
            List<string> lines = TooltipSplitter(text);
            foreach (string tooltip in lines)
            {
                List<string> lines2 = StringSplitter(tooltip);
                for (int i = 0; i < lines2.Count; i++)
                {
                    TooltipLine line = new TooltipLine(Mod, "Positive Attributes", lines2[i])
                    {
                        OverrideColor = new Color(153, 204, 255)
                    };
                    description.Add(line);
                }
            }
        }

        protected void AddNegativeAttribute(List<TooltipLine> description)
        {
            string text = (string)this.GetLocalization("Downsides");
            List<string> lines = TooltipSplitter(text);
            foreach (string tooltip in lines)
            {
                List<string> lines2 = StringSplitter(tooltip);
                for (int i = 0; i < lines2.Count; i++)
                {
                    TooltipLine line = new TooltipLine(Mod, "Negative Attributes", lines2[i])
                    {
                        OverrideColor = new Color(255, 64, 64)
                    };
                    description.Add(line);
                }
            }
        }

        protected void AddNeutralAttribute(List<TooltipLine> description)
        {
            string text = (string)this.GetLocalization("Notes");
            List<string> lines = TooltipSplitter(text);
            foreach (string tooltip in lines)
            {
                List<string> lines2 = StringSplitter(tooltip);
                for (int i = 0; i < lines2.Count; i++)
                {
                    TooltipLine line = new TooltipLine(Mod, "Neutral Attributes", lines2[i])
                    {
                        OverrideColor = new Color(235, 226, 202)
                    };
                    description.Add(line);
                }
            }
        }

        protected void AddOtherAttribute(List<TooltipLine> description, string customText = "")
        {
            string text = customText;
            List<string> lines = TooltipSplitter(text);
            foreach (string tooltip in lines)
            {
                List<string> lines2 = StringSplitter(tooltip);
                for (int i = 0; i < lines2.Count; i++)
                {
                    TooltipLine line = new TooltipLine(Mod, "Neutral Attributes", lines2[i])
                    {
                        OverrideColor = new Color(235, 226, 202)
                    };
                    description.Add(line);
                }
            }
        }

        protected void AddTooltip(List<TooltipLine> description, string name, string text, Color color)
        {
            List<string> lines2 = StringSplitter(text);
            for (int i = 0; i < lines2.Count; i++)
            {
                TooltipLine line = new TooltipLine(Mod, name, lines2[i])
                {
                    OverrideColor = color
                };
                description.Insert(i, line);
            }
        }

        protected void AddLore(List<TooltipLine> description)
        {
            string text = (string)this.GetLocalization("Lore");
            string wrappedText = FontAssets.MouseText.Value.CreateWrappedText(text, 350f);
            TooltipLine line = new TooltipLine(Mod, "Lore", wrappedText)
            {
                OverrideColor = Color.Yellow
            };
            description.Add(line);
        }

        protected void SetWeaponCategory(int classCategory, int weaponCategory, int quality, int weaponAvailability)
        {
            classType = classCategory;
            weaponType = weaponCategory;
            availability = weaponAvailability;
            WeaponAddQuality(quality);
        }

        protected void SetWeaponPrice(int weapon = 0, int refined = 0, int reclaimed = 0, int scrap = 0)
        {
            int metalCost = refined * 18 + reclaimed * 6 + scrap * 2;
            metalValue = Item.buyPrice(platinum: weapon, gold: metalCost);
        }

        protected void SetTimers(int timer1 = 0, int timer2 = 0, int timer3 = 0, int timer4 = 0, int timer5 = 0)
        {
            timer[0] = timer1;
            timer[1] = timer2;
            timer[2] = timer3;
            timer[3] = timer4;
            timer[4] = timer5;
        }

        protected void ClampVariables()
        {
            if (classType <= 0 || classType > Spy)
                classType = 0;
            if (weaponType <= 0 || weaponType > PDA2)
                weaponType = 0;
            if (availability <= 0 || availability > Exclusive)
                availability = 0;
        }

        public bool IsWeaponType(int type) => weaponType == type;

        public bool HasClass(int classes) => classHashSet.Contains(classes);

        public bool HasQuality(int quality) => qualityHashSet.Contains(quality);

        public int GetHighestQuality() => qualityHashSet.Max();

        public static Vector2 DrawColorCodedStringWithShadow(SpriteBatch spriteBatch, DynamicSpriteFont font, string text, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, float maxWidth = -1f, float spread = 2f)
        {
            TextSnippet[] snippets = ChatManager.ParseMessage(text, baseColor).ToArray();
            ChatManager.ConvertNormalSnippets(snippets);
            ChatManager.DrawColorCodedStringShadow(spriteBatch, font, snippets, position, new Color(0, 0, 0, baseColor.A), rotation, origin, baseScale, maxWidth, spread);
            return DrawColorCodedString(spriteBatch, font, snippets, position, baseColor, rotation, origin, baseScale, out _, maxWidth);
        }

        public static Vector2 DrawColorCodedString(SpriteBatch spriteBatch, DynamicSpriteFont font, TextSnippet[] snippets, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, out int hoveredSnippet, float maxWidth, bool ignoreColors = false)
        {
            int num = -1;
            Vector2 vec;
            vec = new Vector2(Main.mouseX, Main.mouseY);
            Vector2 vector = position;
            Vector2 result = vector;
            float x = font.MeasureString(" ").X;
            Color color = baseColor;
            float num2 = 0f;
            for (int i = 0; i < snippets.Length; i++)
            {
                TextSnippet textSnippet = snippets[i];
                textSnippet.Update();
                float num3 = textSnippet.Scale;
                if (textSnippet.UniqueDraw(false, out Vector2 size, spriteBatch, vector, color, baseScale.X * num3))
                {
                    if (vec.Between(vector, vector + size))
                    {
                        num = i;
                    }
                    vector.X += size.X;
                    result.X = Math.Max(result.X, vector.X);
                }
                else
                {
                    string[] array = Regex.Split(textSnippet.Text, "(\n)");
                    bool flag = true;
                    foreach (string text in array)
                    {
                        string[] array2 = Regex.Split(text, "( )");
                        array2 = text.Split(' ', StringSplitOptions.None);
                        if (text == "\n")
                        {
                            vector.Y += font.LineSpacing * num2 * baseScale.Y;
                            vector.X = position.X;
                            result.Y = Math.Max(result.Y, vector.Y);
                            num2 = 0f;
                            flag = false;
                        }
                        else
                        {
                            for (int j = 0; j < array2.Length; j++)
                            {
                                if (j != 0)
                                {
                                    vector.X += x * baseScale.X * num3;
                                }
                                if (maxWidth > 0f)
                                {
                                    float num4 = font.MeasureString(array2[j]).X * baseScale.X * num3;
                                    if (vector.X - position.X + num4 > maxWidth)
                                    {
                                        vector.X = position.X;
                                        vector.Y += font.LineSpacing * num2 * baseScale.Y;
                                        result.Y = Math.Max(result.Y, vector.Y);
                                        num2 = 0f;
                                    }
                                }
                                if (num2 < num3)
                                {
                                    num2 = num3;
                                }
                                DynamicSpriteFontExtensionMethods.DrawString(spriteBatch, font, array2[j], vector, color, rotation, origin, baseScale * textSnippet.Scale * num3, 0, 0f);
                                Vector2 vector2 = font.MeasureString(array2[j]);
                                if (vec.Between(vector, vector + vector2))
                                {
                                    num = i;
                                }
                                vector.X += vector2.X * baseScale.X * num3;
                                result.X = Math.Max(result.X, vector.X);
                            }
                            if (array.Length > 1 && flag)
                            {
                                vector.Y += font.LineSpacing * num2 * baseScale.Y;
                                vector.X = position.X;
                                result.Y = Math.Max(result.Y, vector.Y);
                                num2 = 0f;
                            }
                            flag = true;
                        }
                    }
                }
            }
            hoveredSnippet = num;
            return result;
        }

        public string GetItemName()
        {
            string name = Item.Name;
            if (qualityHashSet.Contains(Unique) && !qualityHashSet.Contains(Unusual) && !qualityHashSet.Contains(Strange) && !qualityHashSet.Contains(Genuine) && !qualityHashSet.Contains(Vintage))
                name = "The " + name;
            if (qualityHashSet.Contains(Vintage))
                name = "Vintage " + name;
            if (qualityHashSet.Contains(Genuine))
                name = "Genuine " + name;
            if (qualityHashSet.Contains(Strange))
                name = "Strange " + name;
            if (qualityHashSet.Contains(Unusual))
                name = "Unusual " + name;
            return name;
        }

        public Color GetItemColor()
        {
            return qualityHashSet.Max() switch
            {
                Unusual => new Color(134, 80, 172),
                Strange => new Color(207, 106, 50),
                Genuine => new Color(77, 116, 85),
                Vintage => new Color(71, 98, 145),
                Unique => new Color(255, 215, 0),
                Stock => new Color(178, 178, 178),
                _ => Color.White
            };
        }

        public static Item GetWeapon(Player player, int weaponType)
        {
            Item foundItem = null;
            TF2Player p = player.GetModPlayer<TF2Player>();
            for (int i = 0; i < 10; i++)
            {
                if (player.inventory[i].ModItem is TF2Item weapon && (weapon.classType == p.currentClass || weapon.classHashSet.Contains(p.currentClass)) && weapon.weaponType == weaponType && weapon is not TF2Accessory && !weapon.EquippedWeaponType(player))
                {
                    foundItem = player.inventory[i];
                    break;
                }
            }
            return foundItem;
        }

        public static List<string> TooltipSplitter(string text) => text.Split("\n").ToList();

        public static List<string> StringSplitter(string text)
        {
            List<string> list = text.Split(' ').ToList();
            string currentString = "";
            List<string> finalString = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                if ((int)FontAssets.MouseText.Value.MeasureString(currentString + " " + list[i]).X < 350f)
                    currentString += " " + list[i];
                else
                {
                    finalString.Add(currentString);
                    currentString = list[i];
                }
                if (i == list.Count - 1)
                    finalString.Add(currentString);
            }
            return finalString;
        }

        public sealed override bool? PrefixChance(int pre, UnifiedRandom rand) => false;

        public sealed override bool PreDrawTooltip(ReadOnlyCollection<TooltipLine> lines, ref int x, ref int y)
        {
            Color color = new Color(36, 32, 27);
            Item hoverItem = Main.HoverItem;
            int yoyoLogo = -1;
            int researchLine = -1;
            float knockBack = hoverItem.knockBack;
            int arrayLength = 30;
            int numLines = 1;
            string[] array = new string[arrayLength];
            bool[] array2 = new bool[arrayLength];
            bool[] array3 = new bool[arrayLength];
            for (int m = 0; m < arrayLength; m++)
            {
                array2[m] = false;
                array3[m] = false;
            }
            string[] tooltipNames = new string[arrayLength];
            Main.MouseText_DrawItemTooltip_GetLinesInfo(hoverItem, ref yoyoLogo, ref researchLine, knockBack, ref numLines, array, array2, array3, tooltipNames, out int prefixlineIndex);
            if (Main.npcShop > 0 && hoverItem.value >= 0 && (hoverItem.type < ItemID.CopperCoin || hoverItem.type > ItemID.PlatinumCoin))
            {
                Main.LocalPlayer.GetItemExpectedPrice(hoverItem, out long calcForSelling, out long calcForBuying);
                long price = (hoverItem.isAShopItem || hoverItem.buyOnce) ? calcForBuying : calcForSelling;
                if (price > 0L)
                {
                    string text = "";
                    long platinum = 0L;
                    long gold = 0L;
                    long silver = 0L;
                    long copper = 0L;
                    long sellPrice = price * hoverItem.stack;
                    if (!hoverItem.buy)
                    {
                        sellPrice = price / 5L;
                        if (sellPrice < 1L)
                            sellPrice = 1L;
                        long sellPriceMultiple = sellPrice;
                        sellPrice *= hoverItem.stack;
                        int amount = Main.shopSellbackHelper.GetAmount(hoverItem);
                        if (amount > 0)
                            sellPrice += (-sellPriceMultiple + calcForBuying) * Math.Min(amount, hoverItem.stack);
                    }
                    if (sellPrice < 1L)
                        sellPrice = 1L;
                    if (sellPrice >= 1000000L)
                        platinum = sellPrice / 1000000L;
                    sellPrice -= platinum * 1000000L;
                    if (sellPrice >= 10000L)
                        gold = sellPrice / 10000L;
                    sellPrice -= gold * 10000L;
                    if (sellPrice >= 100L)
                    {
                        silver = sellPrice / 100L;
                        sellPrice -= silver * 100L;
                    }
                    if (sellPrice >= 1L)
                        copper = sellPrice;
                    if (platinum > 0L)
                        text = string.Concat(new string[] { text, platinum.ToString(), " ", Lang.inter[15].Value, " " });
                    if (gold > 0L)
                        text = string.Concat(new string[] { text, gold.ToString(), " ", Lang.inter[16].Value, " " });
                    if (silver > 0L)
                        text = string.Concat(new string[] { text, silver.ToString(), " ", Lang.inter[17].Value, " " });
                    if (copper > 0L)
                        text = string.Concat(new string[] { text, copper.ToString(), " ", Lang.inter[18].Value, " " });
                    array[numLines] = !hoverItem.buy ? Lang.tip[49].Value + " " + text : Lang.tip[50].Value + " " + text;
                    tooltipNames[numLines] = "sellPrice";
                    numLines++;
                    if (platinum > 0L)
                        color = new Color(220, 220, 220);
                    else if (gold > 0L)
                        color = new Color(224, 201, 92);
                    else if (silver > 0L)
                        color = new Color(181, 192, 193);
                    else if (copper > 0L)
                        color = new Color(246, 138, 96);
                }
                else if (hoverItem.type != ItemID.DefenderMedal)
                {
                    array[numLines] = Lang.tip[51].Value;
                    tooltipNames[numLines] = "sellPrice";
                    numLines++;
                }
            }
            Vector2 zero = Vector2.Zero;
            List<TooltipLine> lines2 = ItemLoader.ModifyTooltips(Main.HoverItem, ref numLines, tooltipNames, ref array, ref array2, ref array3, ref yoyoLogo, out Color?[] overrideColor, prefixlineIndex);
            List<DrawableTooltipLine> drawableLines = lines2.Select((TooltipLine x, int i) => new DrawableTooltipLine(x, i, 0, 0, Color.White)).ToList();
            int minimumHeight = 25;
            zero.X = 400f;
            for (int j = 0; j < numLines; j++)
            {
                Vector2 stringSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, array[j], Vector2.One);
                zero.Y += stringSize.Y;
            }
            if (yoyoLogo != -1)
                zero.Y += 24f;
            x = (int)Main.MouseScreen.X - (int)(zero.X / 2);
            y += 6;
            int margin = 25;
            int width = Main.screenWidth;
            int height = Main.screenHeight;
            if (x + zero.X + margin > width)
                x = (int)(width - zero.X - margin);
            else if (x - margin < 0)
                x = margin;
            if (y + zero.Y + margin * 2 + 12 > height)
                y = (int)(height - zero.Y - margin * 2 - minimumHeight / 2);
            else if (y - margin < 0)
                y = margin;
            if (Main.SettingsEnabled_OpaqueBoxBehindTooltips)
                Utils.DrawInvBG(Main.spriteBatch, new Rectangle(x, y, (int)zero.X, (int)zero.Y + minimumHeight * 2 - 6), new Color(36, 32, 27));
            return true;
        }

        public sealed override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            string text = (int)line.Font.MeasureString(line.Text).X < 350 ? line.Text : line.Font.CreateWrappedText(line.Text, 350);
            float drawX = Main.MouseScreen.X - (int)line.Font.MeasureString(text).X / 2;
            int margin = 25;
            int width = Main.screenWidth;
            int x = (int)Main.MouseScreen.X - 200;
            if (x + 400f + margin > width)
                drawX = width - margin - 200f - (int)line.Font.MeasureString(text).X / 2;
            else if (x - margin < 0)
                drawX = margin + 200f - (int)line.Font.MeasureString(text).X / 2;
            line.X = (int)drawX;
            line.Y += 25;
            Color realLineColor = line.OverrideColor ?? line.Color;
            DrawColorCodedStringWithShadow(Main.spriteBatch, line.Font, text, new Vector2(line.X, line.Y), realLineColor * (255f / Main.mouseTextColor), line.Rotation, line.Origin, line.BaseScale, line.MaxWidth, line.Spread);
            return false;
        }

        public sealed override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) => WeaponDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);

        public sealed override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) => WeaponDrawInWorld(spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);

        public sealed override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (!WeaponModifyDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale)) return;
            if (Item != GetWeapon(Main.LocalPlayer, weaponType) && Item.ModItem is not TF2Accessory && TF2.IsItemInHotbar(Main.LocalPlayer, Item))
                spriteBatch.Draw(TextureAssets.Cd.Value, position - TextureAssets.InventoryBack9.Value.Size() / 4.225f * Main.inventoryScale, null, drawColor, 0f, new Vector2(0.5f, 0.5f), 0.8f * Main.inventoryScale, SpriteEffects.None, 0f);
        }
    }

    public abstract class TF2Accessory : TF2Item
    {
        protected (int index, Item accessory) FindDifferentEquippedAccessory()
        {
            int maxAccessoryIndex = 5 + Main.LocalPlayer.extraAccessorySlots;
            for (int i = 3; i < 3 + maxAccessoryIndex; i++)
            {
                Item otherAccessory = Main.LocalPlayer.armor[i];
                if (!otherAccessory.IsAir && Item.type != otherAccessory.type && otherAccessory.ModItem is TF2Accessory item && item.weaponType == weaponType)
                    return (i, otherAccessory);
            }
            return (-1, null);
        }

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = WeaponResearchCost();

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.accessory = true;
            WeaponStatistics();
            ClampVariables();
            if (availability == Unlock)
                metalValue += Item.buyPrice(platinum: 1);
            Item.value = metalValue;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine nameTooltip = tooltips.FirstOrDefault(x => x.Name == "ItemName" && x.Mod == "Terraria");
            AddName(tooltips);
            tooltips.Remove(nameTooltip);
            RemoveDefaultTooltips(tooltips);
            string category = ((Classes)classType).ToString() + Language.GetTextValue("Mods.TF2.UI.Items.Owner") + " " + ((Availability)availability).ToString() + " " + ((WeaponCategory)weaponType).ToString();
            if (classType == MultiClass)
            {
                int currentClass = Main.LocalPlayer.GetModPlayer<TF2Player>().currentClass;
                category = HasClass(currentClass) ? ((Classes)currentClass).ToString() + Language.GetTextValue("Mods.TF2.UI.Items.Owner") + " " + ((Availability)availability).ToString() + " " + ((WeaponCategory)weaponType).ToString() : Language.GetTextValue("Mods.TF2.UI.Items.MultiClass") + " " + ((Availability)availability).ToString() + " " + ((WeaponCategory)weaponType).ToString();
            }
            TooltipLine line = new TooltipLine(Mod, "Weapon Category", category)
            {
                OverrideColor = new Color(117, 107, 94, 255)
            };
            tooltips.Insert(tooltips.FindLastIndex(x => x.Name == "Name" && x.Mod == "TF2") + 1, line);
            WeaponDescription(tooltips);
            if (Item.favorited)
            {
                TooltipLine favorite = new TooltipLine(Mod, "Favorite", FontAssets.MouseText.Value.CreateWrappedText(Lang.tip[56].Value, 350f))
                {
                    OverrideColor = new Color(235, 226, 202, 255)
                };
                tooltips.Add(favorite);
                TooltipLine favoriteDescription = new TooltipLine(Mod, "Favorite Description", FontAssets.MouseText.Value.CreateWrappedText(Lang.tip[57].Value, 350f))
                {
                    OverrideColor = new Color(235, 226, 202, 255)
                };
                tooltips.Add(favoriteDescription);
                if (Main.LocalPlayer.chest != -1)
                {
                    ChestUI.GetContainerUsageInfo(out bool sync, out Item[] chestinv);
                    if (ChestUI.IsBlockedFromTransferIntoChest(Item, chestinv))
                    {
                        TooltipLine noTransfer = new TooltipLine(Mod, "No Transfer", FontAssets.MouseText.Value.CreateWrappedText(Language.GetTextValue("UI.ItemCannotBePlacedInsideItself"), 350f))
                        {
                            OverrideColor = new Color(235, 226, 202, 255)
                        };
                        tooltips.Add(favorite);
                    }
                }
            }
            TooltipLine priceTooltip = tooltips.FirstOrDefault(x => x.Name == "Price" && x.Mod == "Terraria");
            TooltipLine price = priceTooltip;
            tooltips.Add(price);
            tooltips.Remove(priceTooltip);
            TooltipLine specialPriceTooltip = tooltips.FirstOrDefault(x => x.Name == "SpecialPrice" && x.Mod == "Terraria");
            TooltipLine specialPrice = specialPriceTooltip;
            tooltips.Add(specialPrice);
            tooltips.Remove(specialPriceTooltip);
            TooltipLine journeyResearchTooltip = tooltips.FirstOrDefault(x => x.Name == "JourneyResearch" && x.Mod == "Terraria");
            TooltipLine journeyModeTooltip = journeyResearchTooltip;
            tooltips.Add(journeyModeTooltip);
            tooltips.Remove(journeyResearchTooltip);
        }

        public override bool CanEquipAccessory(Player player, int slot, bool modded)
        {
            if (slot < 10)
            {
                int index = FindDifferentEquippedAccessory().index;
                if (index != -1)
                    return slot == index;
            }
            return classType == player.GetModPlayer<TF2Player>().currentClass;
        }

        /*
        public override void UpdateEquip(Player player)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (weaponType == Primary)
                p.primaryEquipped = true;
            if (weaponType == Secondary)
                p.secondaryEquipped = true;
            if (weaponType == PDA)
                p.pdaEquipped = true;
        }
        */
    }
}
