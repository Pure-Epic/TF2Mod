using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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
using Terraria.ModLoader.IO;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.Utilities;
using TF2.Common;
using TF2.Content.Items.Weapons;
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
using TF2.Content.UI.Inventory;

namespace TF2.Content.Items
{
    public abstract class TF2Item : ModItem
    {
        protected virtual string CustomCategory => "";

        public virtual Asset<Texture2D> WeaponActiveTexture => TextureAssets.Item[Type];

        protected virtual string BackTexture => null;

        protected virtual string BackTextureReverse => BackTexture;

        protected virtual string ArmTexture => null;

        protected virtual string ArmTextureReverse => ArmTexture;

        protected virtual string LegTexture => null;

        protected virtual string LegTextureReverse => LegTexture;

        protected virtual int HealthBoost => 0;

        protected virtual bool TemporaryHealthBoost => false;

        protected virtual bool Reskin => false;

        internal static int GetRandomWeapon
        {
            get
            {
                int[] weaponList =
                [
                    ModContent.ItemType<ForceANature>(),
                    ModContent.ItemType<Shortstop>(),
                    ModContent.ItemType<SodaPopper>(),
                    ModContent.ItemType<BonkAtomicPunch>(),
                    ModContent.ItemType<CritaCola>(),
                    ModContent.ItemType<MadMilk>(),
                    ModContent.ItemType<Winger>(),
                    ModContent.ItemType<Sandman>(),
                    ModContent.ItemType<HolyMackerel>(),
                    ModContent.ItemType<CandyCane>(),
                    ModContent.ItemType<BostonBasher>(),
                    ModContent.ItemType<SunonaStick>(),
                    ModContent.ItemType<FanOWar>(),
                    ModContent.ItemType<Atomizer>(),
                    ModContent.ItemType<DirectHit>(),
                    ModContent.ItemType<BlackBox>(),
                    ModContent.ItemType<RocketJumper>(),
                    ModContent.ItemType<LibertyLauncher>(),
                    ModContent.ItemType<BuffBanner>(),
                    ModContent.ItemType<Gunboats>(),
                    ModContent.ItemType<BattalionsBackup>(),
                    ModContent.ItemType<Concheror>(),
                    ModContent.ItemType<ReserveShooter>(),
                    ModContent.ItemType<Mantreads>(),
                    ModContent.ItemType<Equalizer>(),
                    ModContent.ItemType<PainTrain>(),
                    ModContent.ItemType<MarketGardener>(),
                    ModContent.ItemType<DisciplinaryAction>(),
                    ModContent.ItemType<Backburner>(),
                    ModContent.ItemType<Degreaser>(),
                    ModContent.ItemType<FlareGun>(),
                    ModContent.ItemType<Detonator>(),
                    ModContent.ItemType<Axtinguisher>(),
                    ModContent.ItemType<Homewrecker>(),
                    ModContent.ItemType<Powerjack>(),
                    ModContent.ItemType<BackScratcher>(),
                    ModContent.ItemType<SharpenedVolcanoFragment>(),
                    ModContent.ItemType<LochnLoad>(),
                    ModContent.ItemType<AliBabasWeeBooties>(),
                    ModContent.ItemType<ScottishResistance>(),
                    ModContent.ItemType<CharginTarge>(),
                    ModContent.ItemType<SplendidScreen>(),
                    ModContent.ItemType<StickyJumper>(),
                    ModContent.ItemType<Eyelander>(),
                    ModContent.ItemType<ScotsmansSkullcutter>(),
                    ModContent.ItemType<UllapoolCaber>(),
                    ModContent.ItemType<ClaidheamhMor>(),
                    ModContent.ItemType<HalfZatoichi>(),
                    ModContent.ItemType<PersianPersuader>(),
                    ModContent.ItemType<Natascha>(),
                    ModContent.ItemType<BrassBeast>(),
                    ModContent.ItemType<Tomislav>(),
                    ModContent.ItemType<Sandvich>(),
                    ModContent.ItemType<DalokohsBar>(),
                    ModContent.ItemType<BuffaloSteakSandvich>(),
                    ModContent.ItemType<FamilyBusiness>(),
                    ModContent.ItemType<KillingGlovesOfBoxing>(),
                    ModContent.ItemType<GlovesOfRunningUrgently>(),
                    ModContent.ItemType<WarriorsSpirit>(),
                    ModContent.ItemType<FistsOfSteel>(),
                    ModContent.ItemType<EvictionNotice>(),
                    ModContent.ItemType<FrontierJustice>(),
                    ModContent.ItemType<Wrangler>(),
                    ModContent.ItemType<Gunslinger>(),
                    ModContent.ItemType<SouthernHospitality>(),
                    ModContent.ItemType<Jag>(),
                    ModContent.ItemType<Blutsauger>(),
                    ModContent.ItemType<CrusadersCrossbow>(),
                    ModContent.ItemType<Overdose>(),
                    ModContent.ItemType<Kritzkrieg>(),
                    ModContent.ItemType<QuickFix>(),
                    ModContent.ItemType<Ubersaw>(),
                    ModContent.ItemType<VitaSaw>(),
                    ModContent.ItemType<Amputator>(),
                    ModContent.ItemType<SolemnVow>(),
                    ModContent.ItemType<Huntsman>(),
                    ModContent.ItemType<SydneySleeper>(),
                    ModContent.ItemType<BazaarBargain>(),
                    ModContent.ItemType<Jarate>(),
                    ModContent.ItemType<Razorback>(),
                    ModContent.ItemType<DarwinsDangerShield>(),
                    ModContent.ItemType<TribalmansShiv>(),
                    ModContent.ItemType<Bushwacka>(),
                    ModContent.ItemType<Shahanshah>(),
                    ModContent.ItemType<Ambassador>(),
                    ModContent.ItemType<LEtranger>(),
                    ModContent.ItemType<Enforcer>(),
                    ModContent.ItemType<YourEternalReward>(),
                    ModContent.ItemType<ConniversKunai>(),
                    ModContent.ItemType<BigEarner>(),
                    ModContent.ItemType<CloakAndDagger>(),
                    ModContent.ItemType<DeadRinger>(),
                ];
                return Main.rand.Next(weaponList);
            }
        }

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
        public const int Starter = 1;
        public const int Unlock = 2;
        public const int Craft = 3;
        public const int Purchase = 4;
        public const int Uncrate = 5;
        public const int Contract = 6;
        public const int Exclusive = 7;
        public const int Stock = 1;
        public const int Unique = 2;
        public const int Vintage = 3;
        public const int Genuine = 4;
        public const int Strange = 5;
        public const int Unusual = 6;

        protected bool noThe;
        protected int metalValue;
        public int[] timer = new int[5];
        protected readonly HashSet<int> classHashSet = new HashSet<int>();
        protected HashSet<int> qualityHashSet = new HashSet<int>();
        private Asset<Texture2D> backTexture;
        private Asset<Texture2D> backTextureReverse;
        private Asset<Texture2D> armTexture;
        private Asset<Texture2D> armTextureReverse;
        private Asset<Texture2D> legTexture;
        private Asset<Texture2D> legTextureReverse;

        protected readonly string[] classNames =
        [
            "",
            Language.GetTextValue("Mods.TF2.Class.Scout"),
            Language.GetTextValue("Mods.TF2.Class.Soldier"),
            Language.GetTextValue("Mods.TF2.Class.Pyro"),
            Language.GetTextValue("Mods.TF2.Class.Demoman"),
            Language.GetTextValue("Mods.TF2.Class.Heavy"),
            Language.GetTextValue("Mods.TF2.Class.Engineer"),
            Language.GetTextValue("Mods.TF2.Class.Medic"),
            Language.GetTextValue("Mods.TF2.Class.Sniper"),
            Language.GetTextValue("Mods.TF2.Class.Spy")
        ];
        protected readonly LocalizedText[] rarityNames =
        [
            Language.GetText("Mods.TF2.UI.Items.Rarity.Unique"),
            Language.GetText("Mods.TF2.UI.Items.Rarity.Vintage"),
            Language.GetText("Mods.TF2.UI.Items.Rarity.Genuine"),
            Language.GetText("Mods.TF2.UI.Items.Rarity.Strange"),
            Language.GetText("Mods.TF2.UI.Items.Rarity.Unusual")
        ];
        protected readonly string[] availabilityNames =
        [
            "",
            Language.GetTextValue("Mods.TF2.UI.Items.Availability.Starter"),
            Language.GetTextValue("Mods.TF2.UI.Items.Availability.Unlocked"),
            Language.GetTextValue("Mods.TF2.UI.Items.Availability.Crafted"),
            Language.GetTextValue("Mods.TF2.UI.Items.Availability.Purchased"),
            Language.GetTextValue("Mods.TF2.UI.Items.Availability.Uncrated"),
            Language.GetTextValue("Mods.TF2.UI.Items.Availability.Contract"),
            Language.GetTextValue("Mods.TF2.UI.Items.Availability.Exclusive")
        ];
        protected readonly string[] categoryNames =
        [
            "",
            Language.GetTextValue("Mods.TF2.UI.Items.Category.Primary"),
            Language.GetTextValue("Mods.TF2.UI.Items.Category.Secondary"),
            Language.GetTextValue("Mods.TF2.UI.Items.Category.Melee"),
            Language.GetTextValue("Mods.TF2.UI.Items.Category.PDA"),
            Language.GetTextValue("Mods.TF2.UI.Items.Category.PDA")
        ];

        public bool equipped;

        protected virtual int WeaponResearchCost() => availability switch
        {
            Starter => 1,
            Unlock or Craft or Purchase or Uncrate or Contract => 5,
            Exclusive => 10,
            _ => 0,
        };

        protected virtual void WeaponStatistics()
        { }

        protected virtual void WeaponDescription(List<TooltipLine> description)
        { }

        public virtual void WeaponAddQuality(int quality)
        {
            if (quality <= 0 || quality > Unusual)
                quality = 0;
            if (qualityHashSet.Add(quality) && qualityHashSet.Max() <= quality)
                Item.rare = ModContent.RarityType<TF2Rarity>();
        }

        protected virtual bool WeaponDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) => true;

        protected virtual bool WeaponDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) => true;

        protected virtual bool WeaponModifyDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) => false;

        protected virtual void WeaponEarlyUpdate(Player player)
        { }

        protected virtual void UpdateInventoryInternal(Player player)
        { }

        protected virtual void WeaponSetSlot(int weaponCategory)
        { }

        protected virtual bool WeaponModifyHealthCondition(Player player) => false;

        protected virtual bool WeaponAddTextureCondition(Player player) => false;

        protected virtual Asset<Texture2D> WeaponBackTexture(Player player) => player.direction == 1 ? backTexture : backTextureReverse;

        protected virtual Asset<Texture2D> WeaponArmTexture(Player player) => player.direction == 1 ? armTexture : armTextureReverse;

        protected virtual Asset<Texture2D> WeaponLegTexture(Player player) => player.direction == 1 ? legTexture : legTextureReverse;

        protected virtual int WeaponModifyHealth(Player player) => HealthBoost;

        private int ModifyHealth(Player player) => WeaponModifyHealthCondition(player) ? WeaponModifyHealth(player) : 0;

        private Asset<Texture2D> AddBackTexture(Player player) => WeaponAddTextureCondition(player) ? WeaponBackTexture(player) : null;

        private Asset<Texture2D> AddArmTexture(Player player) => WeaponAddTextureCondition(player) ? WeaponArmTexture(player) : null;

        private Asset<Texture2D> AddLegTexture(Player player) => WeaponAddTextureCondition(player) ? WeaponLegTexture(player) : null;

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
            tooltips.Remove(tooltips.FirstOrDefault(x => x.Name == "ItemName" && x.Mod == "Terraria"));
            tooltips.Remove(tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria"));
            tooltips.Remove(tooltips.FirstOrDefault(x => x.Name == "Damage" && x.Mod == "Terraria"));
            tooltips.Remove(tooltips.FirstOrDefault(x => x.Name == "Speed" && x.Mod == "Terraria"));
            tooltips.Remove(tooltips.FirstOrDefault(x => x.Name == "Knockback" && x.Mod == "Terraria"));
            tooltips.Remove(tooltips.FirstOrDefault(x => x.Name == "HealLife" && x.Mod == "Terraria"));
            tooltips.Remove(tooltips.FirstOrDefault(x => x.Name == "Consumable" && x.Mod == "Terraria"));
            tooltips.Remove(tooltips.FirstOrDefault(x => x.Name == "Equipable" && x.Mod == "Terraria"));
            tooltips.Remove(tooltips.FirstOrDefault(x => x.Name == "Favorite" && x.Mod == "Terraria"));
            tooltips.Remove(tooltips.FirstOrDefault(x => x.Name == "FavoriteDesc" && x.Mod == "Terraria"));
            tooltips.Remove(tooltips.FirstOrDefault(x => x.Name == "NoTransfer" && x.Mod == "Terraria"));
        }

        protected void AddName(List<TooltipLine> description)
        {
            List<string> lines2 = StringSplitter(GetItemName());
            for (int i = 0; i < lines2.Count; i++)
            {
                TooltipLine line = new TooltipLine(Mod, "Name", lines2[i])
                {
                    OverrideColor = GetQualityColor()
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
            TextSnippet[] snippets = [.. ChatManager.ParseMessage(text, baseColor)];
            ChatManager.ConvertNormalSnippets(snippets);
            DrawColorCodedStringShadow(spriteBatch, font, snippets, position, new Color(0, 0, 0, baseColor.A), rotation, origin, baseScale, maxWidth, spread);
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
                if (!ignoreColors)
                    color = textSnippet.GetVisibleColor();
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

        public static void DrawColorCodedStringShadow(SpriteBatch spriteBatch, DynamicSpriteFont font, TextSnippet[] snippets, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, float maxWidth = -1f, float spread = 2f)
        {
            for (int i = 0; i < ChatManager.ShadowDirections.Length; i++)
                DrawColorCodedString(spriteBatch, font, snippets, position + ChatManager.ShadowDirections[i] * spread, baseColor, rotation, origin, baseScale, out var _, maxWidth, ignoreColors: true);
        }

        public string GetItemName()
        {
            string name = Item.Name;
            if (qualityHashSet.Contains(Unique) && !qualityHashSet.Contains(Unusual) && !qualityHashSet.Contains(Strange) && !qualityHashSet.Contains(Genuine) && !qualityHashSet.Contains(Vintage) && !noThe)
                name = rarityNames[0].Format(name);
            if (qualityHashSet.Contains(Vintage))
                name = rarityNames[1].Format(name);
            if (qualityHashSet.Contains(Genuine))
                name = rarityNames[2].Format(name);
            if (qualityHashSet.Contains(Strange))
                name = rarityNames[3].Format(name);
            if (qualityHashSet.Contains(Unusual))
                name = rarityNames[4].Format(name);
            return name;
        }

        public Color GetQualityColor() => qualityHashSet.Max() switch
        {
            Unusual => new Color(134, 80, 172),
            Strange => new Color(207, 106, 50),
            Genuine => new Color(77, 116, 85),
            Vintage => new Color(71, 98, 145),
            Unique => new Color(255, 215, 0),
            Stock => new Color(178, 178, 178),
            _ => Color.White
        };

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

        public void DefaultTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Remove(tooltips.FirstOrDefault(x => x.Name == "ItemName" && x.Mod == "Terraria"));
            AddName(tooltips);
            for (int i = 0; i < Item.ToolTip.Lines; i++)
                tooltips.Remove(tooltips.FirstOrDefault(x => x.Name == ("Tooltip" + i) && x.Mod == "Terraria"));
            RemoveDefaultTooltips(tooltips);
            tooltips.Insert(tooltips.FindLastIndex(x => x.Name == "Name" && x.Mod == "TF2") + 1, new TooltipLine(Mod, "Weapon Category", CustomCategory == "" ? Language.GetTextValue("Mods.TF2.UI.Items.Tool") : CustomCategory)
            {
                OverrideColor = new Color(117, 107, 94)
            });
            if (base.Tooltip.Value != "")
                AddOtherAttribute(tooltips, base.Tooltip.Value);
            if (Item.favorited)
            {
                tooltips.Add(new TooltipLine(Mod, "Favorite", FontAssets.MouseText.Value.CreateWrappedText(Lang.tip[56].Value, 350f))
                {
                    OverrideColor = new Color(235, 226, 202)
                });
                tooltips.Add(new TooltipLine(Mod, "Favorite Description", FontAssets.MouseText.Value.CreateWrappedText(Lang.tip[57].Value, 350f))
                {
                    OverrideColor = new Color(235, 226, 202)
                });
                if (Main.LocalPlayer.chest > -1)
                {
                    ChestUI.GetContainerUsageInfo(out bool sync, out Item[] chestinv);
                    if (ChestUI.IsBlockedFromTransferIntoChest(Item, chestinv))
                    {
                        TooltipLine noTransfer = new TooltipLine(Mod, "No Transfer", FontAssets.MouseText.Value.CreateWrappedText(Language.GetTextValue("UI.ItemCannotBePlacedInsideItself"), 350f))
                        {
                            OverrideColor = new Color(235, 226, 202)
                        };
                        tooltips.Add(new TooltipLine(Mod, "Favorite", FontAssets.MouseText.Value.CreateWrappedText(Lang.tip[56].Value, 350f))
                        {
                            OverrideColor = new Color(235, 226, 202)
                        });
                    }
                }
            }
            tooltips.Remove(tooltips.FirstOrDefault(x => x.Name == "Placeable" && x.Mod == "Terraria"));
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

        public void DefaultTooltipsWithAvailability(List<TooltipLine> tooltips)
        {
            TooltipLine nameTooltip = tooltips.FirstOrDefault(x => x.Name == "ItemName" && x.Mod == "Terraria");
            AddName(tooltips);
            tooltips.Remove(nameTooltip);
            RemoveDefaultTooltips(tooltips);
            TooltipLine category = new TooltipLine(Mod, "Weapon Category", Language.GetText("Mods.TF2.UI.Items.ItemCategory").Format(availabilityNames[availability], CustomCategory))
            {
                OverrideColor = new Color(117, 107, 94)
            };
            tooltips.Insert(tooltips.FindLastIndex(x => x.Name == "Name" && x.Mod == "TF2") + 1, category);
            WeaponDescription(tooltips);
            if (Item.favorited)
            {
                TooltipLine favorite = new TooltipLine(Mod, "Favorite", FontAssets.MouseText.Value.CreateWrappedText(Lang.tip[56].Value, 350f))
                {
                    OverrideColor = new Color(235, 226, 202)
                };
                tooltips.Add(favorite);
                TooltipLine favoriteDescription = new TooltipLine(Mod, "Favorite Description", FontAssets.MouseText.Value.CreateWrappedText(Lang.tip[57].Value, 350f))
                {
                    OverrideColor = new Color(235, 226, 202)
                };
                tooltips.Add(favoriteDescription);
                if (Main.LocalPlayer.chest > -1)
                {
                    ChestUI.GetContainerUsageInfo(out bool sync, out Item[] chestinv);
                    if (ChestUI.IsBlockedFromTransferIntoChest(Item, chestinv))
                    {
                        TooltipLine noTransfer = new TooltipLine(Mod, "No Transfer", FontAssets.MouseText.Value.CreateWrappedText(Language.GetTextValue("UI.ItemCannotBePlacedInsideItself"), 350f))
                        {
                            OverrideColor = new Color(235, 226, 202)
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

        protected void CustomTooltips(List<TooltipLine> tooltips, string category, string description)
        {
            TooltipLine nameTooltip = tooltips.FirstOrDefault(x => x.Name == "ItemName" && x.Mod == "Terraria");
            AddName(tooltips);
            tooltips.Remove(nameTooltip);
            for (int i = 0; i < Item.ToolTip.Lines; i++)
                tooltips.Remove(tooltips.FirstOrDefault(x => x.Name == ("Tooltip" + i) && x.Mod == "Terraria"));
            RemoveDefaultTooltips(tooltips);
            TooltipLine categoryText = new TooltipLine(Mod, "Weapon Category", category)
            {
                OverrideColor = new Color(117, 107, 94)
            };
            tooltips.Insert(tooltips.FindLastIndex(x => x.Name == "Name" && x.Mod == "TF2") + 1, categoryText);
            AddOtherAttribute(tooltips, description);
            if (Item.favorited)
            {
                TooltipLine favorite = new TooltipLine(Mod, "Favorite", FontAssets.MouseText.Value.CreateWrappedText(Lang.tip[56].Value, 350f))
                {
                    OverrideColor = new Color(235, 226, 202)
                };
                tooltips.Add(favorite);
                TooltipLine favoriteDescription = new TooltipLine(Mod, "Favorite Description", FontAssets.MouseText.Value.CreateWrappedText(Lang.tip[57].Value, 350f))
                {
                    OverrideColor = new Color(235, 226, 202)
                };
                tooltips.Add(favoriteDescription);
                if (Main.LocalPlayer.chest > -1)
                {
                    ChestUI.GetContainerUsageInfo(out bool sync, out Item[] chestinv);
                    if (ChestUI.IsBlockedFromTransferIntoChest(Item, chestinv))
                    {
                        TooltipLine noTransfer = new TooltipLine(Mod, "No Transfer", FontAssets.MouseText.Value.CreateWrappedText(Language.GetTextValue("UI.ItemCannotBePlacedInsideItself"), 350f))
                        {
                            OverrideColor = new Color(235, 226, 202)
                        };
                        tooltips.Add(favorite);
                    }
                }
            }
            tooltips.Remove(tooltips.FirstOrDefault(x => x.Name == "Placeable" && x.Mod == "Terraria"));
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

        public static List<string> TooltipSplitter(string text) => [.. text.Split("\n")];

        public static List<string> StringSplitter(string text)
        {
            List<string> list = [.. text.Split(' ')];
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

        public sealed override void Load()
        {
            if (BackTexture != null)
            {
                backTexture = ModContent.Request<Texture2D>(BackTexture);
                backTextureReverse = ModContent.Request<Texture2D>(BackTextureReverse);
                TF2Player.backTextures.Add(AddBackTexture);
            }
            if (ArmTexture != null)
            {
                armTexture = ModContent.Request<Texture2D>(ArmTexture);
                armTextureReverse = ModContent.Request<Texture2D>(ArmTextureReverse);
                TF2Player.armTextures.Add(AddArmTexture);
            }
            if (LegTexture != null)
            {
                legTexture = ModContent.Request<Texture2D>(LegTexture);
                legTextureReverse = ModContent.Request<Texture2D>(LegTextureReverse);
                TF2Player.legTextures.Add(AddLegTexture);
            }
            if (!(Reskin || HealthBoost == 0))
            {
                TF2Player.healthModifiers.Add(ModifyHealth);
                if (!TemporaryHealthBoost)
                    TF2Player.cachedHealthModifiers.Add(ModifyHealth);
            }
        }

        public sealed override void Unload()
        {
            if (BackTexture != null)
                TF2Player.backTextures.Remove(AddBackTexture);
            if (ArmTexture != null)
                TF2Player.armTextures.Remove(AddArmTexture);
            if (LegTexture != null)
                TF2Player.legTextures.Remove(AddLegTexture);
            if (!(Reskin || HealthBoost == 0))
                TF2Player.healthModifiers.Remove(ModifyHealth);
        }

        public override sealed bool? PrefixChance(int pre, UnifiedRandom rand) => false;

        public override sealed bool PreDrawTooltip(ReadOnlyCollection<TooltipLine> lines, ref int x, ref int y)
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
                        text = string.Concat([text, platinum.ToString(), " ", Lang.inter[15].Value, " "]);
                    if (gold > 0L)
                        text = string.Concat([text, gold.ToString(), " ", Lang.inter[16].Value, " "]);
                    if (silver > 0L)
                        text = string.Concat([text, silver.ToString(), " ", Lang.inter[17].Value, " "]);
                    if (copper > 0L)
                        text = string.Concat([text, copper.ToString(), " ", Lang.inter[18].Value, " "]);
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
            List<DrawableTooltipLine> drawableLines = [.. lines2.Select((x, i) => new DrawableTooltipLine(x, i, 0, 0, Color.White))];
            int minimumHeight = 25;
            zero.X = 400f;
            for (int j = 0; j < numLines; j++)
            {
                Vector2 stringSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, array[j], Vector2.One);
                zero.Y += stringSize.Y;
            }
            if (yoyoLogo > -1)
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

        public override sealed bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
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

        public override sealed bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) => WeaponDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);

        public override sealed bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) => WeaponDrawInWorld(spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);

        public override sealed void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (!WeaponModifyDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale)) return;
            if (Item != GetWeapon(Main.LocalPlayer, weaponType) && Item.ModItem is not TF2Accessory && TF2.IsItemInHotbar(Main.LocalPlayer, Item))
                spriteBatch.Draw(TextureAssets.Cd.Value, position - TextureAssets.InventoryBack9.Value.Size() / 4.225f * Main.inventoryScale, null, drawColor, 0f, new Vector2(0.5f, 0.5f), 0.8f * Main.inventoryScale, SpriteEffects.None, 0f);
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.GetInt("availability") > 0)
                availability = tag.GetInt("availability");
            else if (Item.ModItem is TF2Weapon || Item.ModItem is TF2Accessory)
            {
                availability = Exclusive;
                WeaponAddQuality(Vintage);
            }
            if (tag.GetList<int>("qualities") != null && tag.GetList<int>("qualities").Count > 0)
                qualityHashSet = [.. tag.GetList<int>("qualities")];
        }

        public override void SaveData(TagCompound tag)
        {
            tag["availability"] = availability;
            tag["qualities"] = qualityHashSet.ToList();
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
            AddName(tooltips);
            tooltips.Remove(tooltips.FirstOrDefault(x => x.Name == "ItemName" && x.Mod == "Terraria"));
            RemoveDefaultTooltips(tooltips);
            string weaponAvailability = !TF2.MannCoStoreActive ? availabilityNames[availability] : availabilityNames[5];
            string weaponCategory = categoryNames[weaponType];
            string category = Language.GetText("Mods.TF2.UI.Items.CategoryText").Format(classNames[classType], weaponAvailability, weaponCategory);
            if (classType == MultiClass)
                category = HasClass(Main.LocalPlayer.GetModPlayer<TF2Player>().currentClass) ? Language.GetText("Mods.TF2.UI.Items.CategoryText").Format(classNames[Main.LocalPlayer.GetModPlayer<TF2Player>().currentClass], weaponAvailability, weaponCategory) : Language.GetText("Mods.TF2.UI.Items.CategoryText").Format(Language.GetTextValue("Mods.TF2.UI.Items.MultiClass"), weaponAvailability, weaponCategory);
            tooltips.Insert(tooltips.FindLastIndex(x => x.Name == "Name" && x.Mod == "TF2") + 1, new TooltipLine(Mod, "Weapon Category", category)
            {
                OverrideColor = new Color(117, 107, 94)
            });
            WeaponDescription(tooltips);
            if (Item.favorited)
            {
                tooltips.Add(new TooltipLine(Mod, "Favorite", FontAssets.MouseText.Value.CreateWrappedText(Lang.tip[56].Value, 350f))
                {
                    OverrideColor = new Color(235, 226, 202)
                });
                tooltips.Add(new TooltipLine(Mod, "Favorite Description", FontAssets.MouseText.Value.CreateWrappedText(Lang.tip[57].Value, 350f))
                {
                    OverrideColor = new Color(235, 226, 202)
                });
                if (Main.LocalPlayer.chest > -1)
                {
                    ChestUI.GetContainerUsageInfo(out bool sync, out Item[] chestinv);
                    if (ChestUI.IsBlockedFromTransferIntoChest(Item, chestinv))
                    {
                        TooltipLine noTransfer = new TooltipLine(Mod, "No Transfer", FontAssets.MouseText.Value.CreateWrappedText(Language.GetTextValue("UI.ItemCannotBePlacedInsideItself"), 350f))
                        {
                            OverrideColor = new Color(235, 226, 202)
                        };
                        tooltips.Add(new TooltipLine(Mod, "Favorite", FontAssets.MouseText.Value.CreateWrappedText(Lang.tip[56].Value, 350f))
                        {
                            OverrideColor = new Color(235, 226, 202)
                        });
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

        protected override bool WeaponModifyDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) => true;

        public override bool CanEquipAccessory(Player player, int slot, bool modded)
        {
            if (slot < 10)
            {
                int index = FindDifferentEquippedAccessory().index;
                if (index > -1)
                    return slot == index;
            }
            return classType == player.GetModPlayer<TF2Player>().currentClass;
        }
    }

    public class TF2Rarity : ModRarity
    {
        public override Color RarityColor => new Color(178, 178, 178);

        public override int GetPrefixedRarity(int offset, float valueMult) => Type;
    }
}