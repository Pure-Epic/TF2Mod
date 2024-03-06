using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour;
using ReLogic.Content;
using System;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.Graphics.Capture;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;
using Terraria.UI;
using Terraria.UI.Gamepad;
using TF2.Common;
using TF2.Content.Items;
using TF2.Content.Mounts;

namespace TF2.Content.UI.Inventory
{
    public class TF2Inventory : ModSystem
    {
        public static int mapHeight;
        public static int accSlotToSwapTo;
        public Hook DrawAccSlots = new Hook(typeof(AccessorySlotLoader).GetMethod("DrawAccSlots", BindingFlags.Instance | BindingFlags.Public), Hook_DrawAccSlots);
        public Hook DrawVisibility = new Hook(typeof(AccessorySlotLoader).GetMethod("DrawVisibility", BindingFlags.Instance | BindingFlags.NonPublic), Hook_DrawVisibility);
        public delegate void DrawAccSlotsAction(AccessorySlotLoader self, int num20);
        public delegate bool DrawVisibilityFunction(AccessorySlotLoader self, ref bool visbility, int context, int xLoc, int yLoc, out int xLoc2, out int yLoc2, out Texture2D value4);

        public override void Load()
        {
            On_Main.GUIBarsDrawInner += Hook_GUIBarsDrawInner;
            On_Main.DrawInterface_16_MapOrMinimap += Hook_DrawInterface_16_MapOrMinimap;
            On_Player.OpenInventory += Hook_OpenInventory;
            On_Main.DrawInventory += Hook_DrawInventory;
            On_Main.DrawPageIcons += Hook_DrawPageIcons;
            On_Main.DrawDefenseCounter += Hook_DrawDefenseCounter;
            On_ItemSlot.SelectEquipPage += Hook_SelectEquipPage;
            On_ItemSlot.SwapEquip_ItemArray_int_int += Hook_SwapEquip;
            On_ItemSlot.OverrideLeftClick += On_OverrideLeftClick;
            On_ItemSlot.PickItemMovementAction += Hook_PickItemMovementAction;
            On_ItemSlot.RightClick_ItemArray_int_int += Hook_RightClick;
            On_Mount.SetMount += On_SetMount;
        }

        public override void Unload()
        {
            On_Main.GUIBarsDrawInner -= Hook_GUIBarsDrawInner;
            On_Main.DrawInterface_16_MapOrMinimap -= Hook_DrawInterface_16_MapOrMinimap;
            On_Player.OpenInventory -= Hook_OpenInventory;
            On_Main.DrawInventory -= Hook_DrawInventory;
            On_Main.DrawPageIcons -= Hook_DrawPageIcons;
            On_Main.DrawDefenseCounter -= Hook_DrawDefenseCounter;
            On_ItemSlot.SelectEquipPage -= Hook_SelectEquipPage;
            On_ItemSlot.SwapEquip_ItemArray_int_int -= Hook_SwapEquip;
            On_ItemSlot.OverrideLeftClick -= On_OverrideLeftClick;
            On_ItemSlot.PickItemMovementAction -= Hook_PickItemMovementAction;
            On_ItemSlot.RightClick_ItemArray_int_int -= Hook_RightClick;
            On_Mount.SetMount -= On_SetMount;
        }

        private static void Hook_DrawAccSlots(DrawAccSlotsAction orig, AccessorySlotLoader self, int num20)
        {
            if (Main.LocalPlayer.GetModPlayer<TF2Player>().ClassSelected)
                Main.inventoryBack = Color.White;
            orig(self, num20);
        }

        private static bool Hook_DrawVisibility(DrawVisibilityFunction orig, AccessorySlotLoader self, ref bool visbility, int context, int xLoc, int yLoc, out int xLoc2, out int yLoc2, out Texture2D value4)
        {
            if (!Main.LocalPlayer.GetModPlayer<TF2Player>().ClassSelected)
                return orig(self, ref visbility, context, xLoc, yLoc, out xLoc2, out yLoc2, out value4);
            for (int i = 0; i < Main.LocalPlayer.GetModPlayer<ModAccessorySlotPlayer>().SlotCount; i++)
            {
                if (i == ModContent.GetInstance<PrimarySlot>().Type || i == ModContent.GetInstance<SecondarySlot>().Type || i == ModContent.GetInstance<PDASlot>().Type)
                {
                    yLoc2 = yLoc - 2;
                    xLoc2 = xLoc - 58 + 64 + 28;
                    value4 = ModContent.Request<Texture2D>("TF2/Content/Textures/Nothing").Value;
                    return false;
                }
            }
            return orig(self, ref visbility, context, xLoc, yLoc, out xLoc2, out yLoc2, out value4);
        }

        private void Hook_GUIBarsDrawInner(On_Main.orig_GUIBarsDrawInner orig, Main self)
        {
            if (Main.LocalPlayer.GetModPlayer<TF2Player>().ClassSelected)
            {
                MethodInfo drawInterface_Resources_Breath = typeof(Main).GetMethod("DrawInterface_Resources_Breath", BindingFlags.Static | BindingFlags.NonPublic);
                DrawHealthBar(Main.spriteBatch);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
                drawInterface_Resources_Breath.Invoke(self, null);
                Main.DrawInterface_Resources_ClearBuffs();
                if (!Main.ingameOptionsWindow && !Main.playerInventory && !Main.inFancyUI)
                    self.DrawInterface_Resources_Buffs();
            }
            else
                orig(self);
        }

        private static void DrawHealthBar(SpriteBatch spriteBatch)
        {
            Player player = Main.LocalPlayer;
            TF2Player p = player.GetModPlayer<TF2Player>();
            int maxHealth = p.cachedHealth;
            float amount = (float)player.statLife / maxHealth;
            amount = Utils.Clamp(amount, 0f, 1f);
            Rectangle hitbox = new Rectangle(Main.screenWidth - 296, 3, 250, 80);
            hitbox.X += 52;
            hitbox.Y += 20;
            hitbox.Width = 194;
            hitbox.Height = 40;
            int left = hitbox.Left;
            int right = hitbox.Right;
            int steps = (int)((right - left) * amount);
            spriteBatch.Draw(ModContent.Request<Texture2D>("TF2/Content/Textures/UI/HUD/HealthBar", AssetRequestMode.ImmediateLoad).Value, new Vector2(Main.screenWidth - 296, 3), Color.White);
            if (player.statLife > maxHealth / 4f)
            {
                for (int i = 0; i < steps; i++)
                {
                    float percent = (float)i / steps;
                    spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(left + i, hitbox.Y, 1, hitbox.Height), Color.Lerp(new Color(179, 169, 145), new Color(252, 235, 202), percent));
                }
            }
            else
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(left, hitbox.Y, TF2.Round(hitbox.Width * (float)(player.statLife - p.overheal) / maxHealth), hitbox.Height), new Color(178, 0, 0));
            if (p.overheal > 0)
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(left, hitbox.Y, TF2.Round(hitbox.Width * Math.Clamp((float)p.overheal / TF2.OverhealRound(maxHealth * p.overhealMultiplier * 0.5f), 0f, 1f)), hitbox.Height), new Color(255, 255, 255, 128));
            spriteBatch.Draw(ModContent.Request<Texture2D>("TF2/Content/Textures/UI/HUD/HealthIcon", AssetRequestMode.ImmediateLoad).Value, new Rectangle(Main.screenWidth - 296, 3, 80, 80), Color.White);
            string health = player.statLife.ToString();
            TF2Item.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, health, new Vector2(Main.screenWidth - 256 - (FontAssets.MouseText.Value.MeasureString(health) / 2f).X, 46 - (FontAssets.MouseText.Value.MeasureString(health) / 2f).Y), Color.White, 0f, default, Vector2.One);
        }
       
        private void Hook_DrawInterface_16_MapOrMinimap(On_Main.orig_DrawInterface_16_MapOrMinimap orig, Main self)
        {
            mapHeight = 0;
            if (!Main.mapEnabled)
            {
                orig(self);
                return;
            }
            if (!Main.mapFullscreen && Main.mapStyle == 1)
                mapHeight = 256;
            if (mapHeight + self.RecommendedEquipmentAreaPushUp > Main.screenHeight)
                mapHeight = Main.screenHeight - self.RecommendedEquipmentAreaPushUp;
            orig(self);
        }

        private void Hook_OpenInventory(On_Player.orig_OpenInventory orig)
        {
            if (Main.LocalPlayer.GetModPlayer<TF2Player>().ClassSelected)
            {
                Recipe.FindRecipes();
                Main.playerInventory = true;
                Main.EquipPageSelected = 4;
                SoundEngine.PlaySound(SoundID.MenuOpen);
            }
            else
                orig();
        }

        private void Hook_DrawInventory(On_Main.orig_DrawInventory orig, Main self)
        {
            if (Main.EquipPage == 4)
            {
                Main.inventoryScale = 0.85f;
                Point value = new Point(Main.mouseX, Main.mouseY);
                Rectangle r = new Rectangle(0, 0, (int)(TextureAssets.InventoryBack.Width() * Main.inventoryScale), (int)(TextureAssets.InventoryBack.Height() * Main.inventoryScale));
                Item[] inv = Main.LocalPlayer.miscEquips;
                int num23 = Main.screenWidth - 92;
                int num24 = mapHeight + 174;
                r.X = num23 - 47;
                for (int m = 0; m <= 2; m++)
                {
                    int context = 0;
                    switch (m)
                    {
                        case 0:
                            context = 17;
                            break;

                        case 1:
                            context = 19;
                            break;

                        case 2:
                            context = 20;
                            break;
                    }
                    r.Y = num24 + m * 47;
                    int slot = m switch
                    {
                        0 => 3,
                        1 => 0,
                        2 => 1,
                        _ => 0
                    };
                    if (r.Contains(value) && !PlayerInput.IgnoreMouseInterface)
                    {
                        Player player = Main.LocalPlayer;
                        player.mouseInterface = true;
                        Main.armorHide = true;
                        ItemSlot.Handle(inv, context, slot);
                    }
                    ItemSlot.Draw(Main.spriteBatch, inv, context, slot, r.TopLeft(), default);
                }
            }
            orig(self);
        }

        private int Hook_DrawPageIcons(On_Main.orig_DrawPageIcons orig, int yPos)
        {
            if (Main.LocalPlayer.GetModPlayer<TF2Player>().ClassSelected)
            {
                if (Main.EquipPage == 0)
                    Main.EquipPage = 4;
                if (Main.EquipPageSelected == 0)
                    Main.EquipPageSelected = 4;
                int num = -1;
                Vector2 vector;
                vector = new Vector2(Main.screenWidth - 162, yPos);
                vector.X += 82f;
                Texture2D value = TextureAssets.EquipPage[(Main.EquipPage == 1) ? 5 : 4].Value;
                if (Collision.CheckAABBvAABBCollision(vector, value.Size(), new Vector2(Main.mouseX, Main.mouseY), Vector2.One) && Main.mouseItem.stack < 1)
                    num = 1;
                if (num == 1)
                    Main.spriteBatch.Draw(TextureAssets.EquipPage[7].Value, vector, null, Main.OurFavoriteColor, 0f, new Vector2(2f), 0.9f, 0, 0f);
                Main.spriteBatch.Draw(value, vector, null, Color.White, 0f, Vector2.Zero, 0.9f, 0, 0f);
                UILinkPointNavigator.SetPosition(306, vector + value.Size() * 0.75f);
                vector.X -= 48f;
                value = TextureAssets.EquipPage[(Main.EquipPage == 3) ? 10 : 8].Value;
                if (Collision.CheckAABBvAABBCollision(vector, value.Size(), new Vector2(Main.mouseX, Main.mouseY), Vector2.One) && Main.mouseItem.stack < 1)
                    num = 3;
                if (num == 3 && !Main.CaptureModeDisabled)
                    Main.spriteBatch.Draw(TextureAssets.EquipPage[9].Value, vector, null, Main.OurFavoriteColor, 0f, Vector2.Zero, 0.9f, 0, 0f);
                Main.spriteBatch.Draw(value, vector, null, Main.CaptureModeDisabled ? Color.Red : Color.White, 0f, Vector2.Zero, 0.9f, 0, 0f);
                UILinkPointNavigator.SetPosition(307, vector + value.Size() * 0.75f);
                if (num != -1)
                {
                    Main.LocalPlayer.mouseInterface = true;
                    if (Main.mouseLeft && Main.mouseLeftRelease)
                    {
                        bool flag = true;
                        if (num == 3)
                        {
                            if (Main.CaptureModeDisabled)
                                flag = false;
                            else if (PlayerInput.UsingGamepad)
                                CaptureInterface.QuickScreenshot();
                            else
                            {
                                CaptureManager.Instance.Active = true;
                                Main.blockMouse = true;
                            }
                        }
                        else if (Main.EquipPageSelected != num)
                            Main.EquipPageSelected = num;
                        else
                            Main.EquipPageSelected = 4;
                        if (flag)
                            SoundEngine.PlaySound(SoundID.MenuOpen);
                    }
                }
                ItemSlot.SelectEquipPage(Main.mouseItem);
                if (Main.EquipPage == -1 || Main.EquipPage == 0)
                    Main.EquipPage = Main.EquipPageSelected;
                return num;
            }
            else
                return orig(yPos);
        }

        private void Hook_DrawDefenseCounter(On_Main.orig_DrawDefenseCounter orig, int inventoryX, int inventoryY)
        {
            if (!Main.LocalPlayer.GetModPlayer<TF2Player>().ClassSelected)
                orig(inventoryX, inventoryY);
        }

        private void Hook_SelectEquipPage(On_ItemSlot.orig_SelectEquipPage orig, Item item)
        {
            if (Main.LocalPlayer.GetModPlayer<TF2Player>().ClassSelected)
            {
                Main.EquipPage = -1;
                if (!item.IsAir)
                {
                    if (item.legSlot != -1 || item.headSlot != -1 || item.bodySlot != -1 || item.accessory)
                        Main.EquipPage = 4;
                }
            }
            else
                orig(item);
        }

        private void Hook_SwapEquip(On_ItemSlot.orig_SwapEquip_ItemArray_int_int orig, Item[] inv, int context, int slot)
        {
            if (Main.LocalPlayer.GetModPlayer<TF2Player>().ClassSelected) return;
            orig(inv, context, slot);
        }

        private bool On_OverrideLeftClick(On_ItemSlot.orig_OverrideLeftClick orig, Item[] inv, int context, int slot) => (Main.LocalPlayer.GetModPlayer<TF2Player>().ClassSelected && context == 17) ? (Main.mouseItem.type != ModContent.ItemType<TF2MountItem>() && !Main.mouseItem.IsAir) : orig(inv, context, slot);

        private int Hook_PickItemMovementAction(On_ItemSlot.orig_PickItemMovementAction orig, Item[] inv, int context, int slot, Item checkItem)
        {
            Player player = Main.LocalPlayer;
            if (player.GetModPlayer<TF2Player>().ClassSelected)
            {
                if (context == 17 && Main.mouseLeft)
                {
                    if (player.miscEquips[3].type == ModContent.ItemType<TF2MountItem>() && Main.mouseLeftRelease)
                        player.ClearBuff(ModContent.BuffType<TF2MountBuff>());
                    else if (player.miscEquips[3].type == ModContent.ItemType<TF2MountItem>() && checkItem.IsAir)
                        player.mount.SetMount(player.miscEquips[3].mountType, player);
                }
                if (context == 19 && player.miscEquips[0].IsAir && checkItem.buffType > 0 && Main.vanityPet[checkItem.buffType] && !Main.lightPet[checkItem.buffType] && Main.mouseLeft)
                    player.hideMisc[0] = true;
                if (context == 20 && player.miscEquips[1].IsAir && checkItem.buffType > 0 && Main.lightPet[checkItem.buffType] && Main.mouseLeft)
                    player.hideMisc[1] = true;
            }
            return orig(inv, context, slot, checkItem);
        }

        private void Hook_RightClick(On_ItemSlot.orig_RightClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
        {
            Item item = inv[slot];
            if (Main.LocalPlayer.GetModPlayer<TF2Player>().ClassSelected && (item.mountType > 0 || Main.vanityPet[item.buffType] || Main.lightPet[item.buffType])) return;
            orig(inv, context, slot);
        }

        private void On_SetMount(On_Mount.orig_SetMount orig, Mount self, int m, Player mountedPlayer, bool faceLeft)
        {
            if (mountedPlayer.GetModPlayer<TF2Player>().ClassSelected && m != ModContent.MountType<TF2Mount>())
                return;
            else
                orig(self, m, mountedPlayer, faceLeft);
        }
    }

    public class PetBuffs : GlobalBuff
    {
        public override bool RightClick(int type, int buffIndex)
        {
            Player player = Main.LocalPlayer;
            if (type == ModContent.BuffType<TF2MountBuff>() && player.GetModPlayer<TF2Player>().ClassSelected)
            {
                player.QuickSpawnItem(player.GetSource_FromThis(), player.miscEquips[3]);
                player.miscEquips[3].type = ItemID.None;
                player.miscEquips[3].stack = 0;
            }
            if (Main.vanityPet[type] && player.GetModPlayer<TF2Player>().ClassSelected)
            {
                player.QuickSpawnItem(player.GetSource_FromThis(), player.miscEquips[0]);
                player.miscEquips[0].type = ItemID.None;
                player.miscEquips[0].stack = 0;
            }
            if (Main.lightPet[type] && player.GetModPlayer<TF2Player>().ClassSelected)
            {
                player.QuickSpawnItem(player.GetSource_FromThis(), player.miscEquips[1]);
                player.miscEquips[1].type = ItemID.None;
                player.miscEquips[1].stack = 0;
            }
            return base.RightClick(type, buffIndex);
        }
    }

    public abstract class TF2AccessorySlot : ModAccessorySlot
    {
        public virtual int SlotType => 0;

        public virtual int SlotIndex => 0;

        public override Vector2? CustomLocation
        {
            get
            {
                int num19 = 8 + Main.LocalPlayer.GetAmountOfExtraAccessorySlotsToShow();
                int num20 = 174 + TF2Inventory.mapHeight;
                int num21 = 950;
                if (Main.screenHeight < num21 && num19 >= 10)
                    num20 -= (int)(56f * Main.inventoryScale * (num19 - 9));
                int width = Main.screenWidth - 64 - 28;
                int height = (int)(num20 + (SlotIndex - 1) * 56 * Main.inventoryScale);
                return new Vector2(width, height);
            }
        }

        public override bool DrawVanitySlot => false;

        public override bool DrawDyeSlot => false;

        public override string FunctionalTexture => "TF2/Content/Textures/UI/Inventory/Inventory_Primary";

        public override string FunctionalBackgroundTexture => "TF2/Content/Textures/UI/Inventory/Inventory_Back";

        public override bool IsHidden() => Main.EquipPage != 4 || SlotType == 0 || SlotIndex == 0;

        public override bool IsVisibleWhenNotEnabled() => false;

        public override bool CanAcceptItem(Item checkItem, AccessorySlotType context) => checkItem.ModItem is TF2Accessory item && item.IsWeaponType(SlotType);

        public override void OnMouseHover(AccessorySlotType context)
        {
            Main.hoverItemName = SlotType switch
            {
                TF2Item.Primary => "Primary",
                TF2Item.Secondary => "Secondary",
                TF2Item.PDA => "PDA",
                _ => ""
            };
        }
    }

    public class PrimarySlot : TF2AccessorySlot
    {
        public override int SlotType => TF2Item.Primary;

        public override int SlotIndex => Main.LocalPlayer.GetModPlayer<TF2Player>().currentClass switch
        {
            TF2Item.Demoman => 1,
            _ => 0
        };

        public override string FunctionalTexture => "TF2/Content/Textures/UI/Inventory/Inventory_Primary";

        public override string FunctionalBackgroundTexture => "TF2/Content/Textures/UI/Inventory/Inventory_Back";
    }

    public class SecondarySlot : TF2AccessorySlot
    {
        public override int SlotType => TF2Item.Secondary;

        public override int SlotIndex => Main.LocalPlayer.GetModPlayer<TF2Player>().currentClass switch
        {
            TF2Item.Soldier or TF2Item.Sniper => 1,
            TF2Item.Demoman => 2,
            _ => 0
        };

        public override string FunctionalTexture => "TF2/Content/Textures/UI/Inventory/Inventory_Secondary";

        public override string FunctionalBackgroundTexture => "TF2/Content/Textures/UI/Inventory/Inventory_Back";
    }

    public class PDASlot : TF2AccessorySlot
    {
        public override int SlotType => TF2Item.PDA;

        public override int SlotIndex => Main.LocalPlayer.GetModPlayer<TF2Player>().currentClass switch
        {
            TF2Item.Spy => 1,
            _ => 0
        };

        public override string FunctionalTexture => "TF2/Content/Textures/UI/Inventory/Inventory_PDA";

        public override string FunctionalBackgroundTexture => "TF2/Content/Textures/UI/Inventory/Inventory_Back";
    }
}