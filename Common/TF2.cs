using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System;
using System.Reflection;
using System.Threading;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items;
using TF2.Content.Items.Demoman;
using TF2.Content.Items.Engineer;
using TF2.Content.Items.Heavy;
using TF2.Content.Items.Medic;
using TF2.Content.Items.MultiClass;
using TF2.Content.Items.Pyro;
using TF2.Content.Items.Scout;
using TF2.Content.Items.Sniper;
using TF2.Content.Items.Soldier;
using TF2.Content.Items.Spy;
using TF2.Content.Projectiles;
using TF2.Content.UI;
using TF2.Gensokyo.Content.Items.BossSummons;
using Terraria.UI;

namespace TF2
{
    public class TF2 : Mod
    {
        // Registers a new custom currency
        public static readonly int Australium = CustomCurrencyManager.RegisterCurrency(new Content.Items.Currencies.AustraliumCurrency(ModContent.ItemType<Content.Items.Currencies.Australium>(), 999L, "Australium"));
        private ClassIcon classUI;
        internal static event Action UnloadReflection;
        public static Mod Gensokyo;
        public static bool gensokyoLoaded;
        public static bool heeheeheehaw;

        public override void Load()
        {
            if (ModLoader.TryGetMod("HEROsMod", out Mod HEROsMod))
            {
                HEROsMod.Call("AddItemCategory", "Mercenary", "Weapons",
                (Predicate<Item>)((Item item) =>
                {
                    return item.ModItem is TF2Item;
                }));
            }

            IL_Player.Update += Hook_Update;
            On_ItemSlot.LeftClick_ItemArray_int_int += OnHook_LeftClick;
            On_UICharacterListItem.ctor += Hook_UICharacterList;
            On_UICharacter.DrawSelf += Hook_UICharacter_DrawSelf;
        }

        public override void Unload()
        {
            Interlocked.Exchange(ref UnloadReflection, null)?.Invoke();
            IL_Player.Update -= Hook_Update;
            On_ItemSlot.LeftClick_ItemArray_int_int -= OnHook_LeftClick;
            On_UICharacterListItem.ctor -= Hook_UICharacterList;
            On_UICharacter.DrawSelf -= Hook_UICharacter_DrawSelf;
            Gensokyo = null;
        }


        public override void PostSetupContent() => gensokyoLoaded = ModLoader.TryGetMod("Gensokyo", out Gensokyo);

        public override object Call(params object[] args)
        {
            try
            {
                string message = (string)args[0];

                switch (message)
                {
                    case "AddGensokyoShopItem":
                        if (Gensokyo != null)
                        {
                            AddShopItem((int)args[1], NPC.downedMoonlord, "Tools", ModContent.ItemType<BossRushSummon>(), 1);

                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<ForceANature>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<Shortstop>(), 1);
                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<BonkAtomicPunch>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<CritaCola>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<MadMilk>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<Sandman>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<HolyMackerel>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<CandyCane>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<BostonBasher>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<SunonaStick>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<FanOWar>(), 1);

                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<DirectHit>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<BlackBox>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<RocketJumper>(), 1);
                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<BuffBanner>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<Gunboats>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<BattalionsBackup>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<Concheror>(), 1);
                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<Equalizer>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<PainTrain>(), 1);

                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<Backburner>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<Degreaser>(), 1);
                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<FlareGun>(), 1);
                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<Axtinguisher>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<Homewrecker>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<Powerjack>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<BackScratcher>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<SharpenedVolcanoFragment>(), 1);

                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<LochnLoad>(), 1);
                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<ScottishResistance>(), 1);
                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<CharginTarge>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<StickyJumper>(), 1);
                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<Eyelander>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<ScotsmansSkullcutter>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<UllapoolCaber>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<ClaidheamhMor>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<HalfZatoichi>(), 1);

                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<Natascha>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<BrassBeast>(), 1);
                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<Sandvich>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<DalokohsBar>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<BuffaloSteakSandvich>(), 1);
                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<KillingGlovesOfBoxing>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<GlovesOfRunningUrgently>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<WarriorsSpirit>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<FistsOfSteel>(), 1);

                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<FrontierJustice>(), 1);
                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<Wrangler>(), 1);
                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<Gunslinger>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<SouthernHospitality>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<Jag>(), 1);

                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<Blutsauger>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<CrusadersCrossbow>(), 1);
                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<Kritzkrieg>(), 1);
                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<Ubersaw>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<VitaSaw>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<Amputator>(), 1);

                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<Huntsman>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<SydneySleeper>(), 1);
                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<Jarate>(), 1);
                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<Razorback>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<DarwinsDangerShield>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<TribalmansShiv>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<Bushwacka>(), 1);

                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<Ambassador>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<LEtranger>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<YourEternalReward>(), 1);
                            AddShopItem((int)args[1], Main.hardMode, "Weapons", ModContent.ItemType<ConniversKunai>(), 1);
                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<CloakAndDagger>(), 1);
                            AddShopItem((int)args[1], true, "Weapons", ModContent.ItemType<DeadRinger>(), 1);

                            if (TryFind("PhotonShotgun", out ModItem photonShotgun))
                                AddShopItem((int)args[1], true, "Weapons", photonShotgun.Type, 1);
                            if (TryFind("ManualInferno", out ModItem manualInferno))
                                AddShopItem((int)args[1], NPC.downedMoonlord, "Weapons", manualInferno.Type, 1);
                            if (TryFind("AdvancedScoutRifle", out ModItem advancedScoutRifle))
                                AddShopItem((int)args[1], NPC.downedMoonlord, "Weapons", advancedScoutRifle.Type, 1);
                            if (TryFind("HarshPunisher", out ModItem harshPunisher))
                                AddShopItem((int)args[1], NPC.downedMoonlord, "Weapons", harshPunisher.Type, 1);
                            if (TryFind("OffensiveRocketSystem", out ModItem offensiveRocketSystem))
                                AddShopItem((int)args[1], NPC.downedMoonlord, "Weapons", offensiveRocketSystem.Type, 1);
                            if (TryFind("HeadhunterPistols", out ModItem headhunterPistols))
                                AddShopItem((int)args[1], NPC.downedMoonlord, "Weapons", headhunterPistols.Type, 1);
                        }
                        return "Success";
                    case "RegisterPlayerScaleAccess":
                        if (Gensokyo != null) return "TF2";
                        return "Success";
                }
                Logger.Debug("GensokyoDLC Call Error: Unknown Message: " + message);
            }
            catch (Exception e)
            {
                Logger.Warn("GensokyoDLC Call Error: " + e.StackTrace + e.Message);
            }
            return "Failure";
        }

        private static void AddShopItem(int argument, bool availability, string category, int type, int amount) => Gensokyo.Call("AddShopItem", argument, availability, category, type, amount, (int)ItemID.None, 0);

        private void Hook_Update(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            ILLabel LabelKey = null;
            Type playerType = typeof(Player);
            FieldInfo itemAnimation = playerType.GetField("itemAnimation", BindingFlags.Instance | BindingFlags.Public);
            PropertyInfo ItemTimeIsZero = playerType.GetProperty("ItemTimeIsZero", BindingFlags.Instance | BindingFlags.Public);
            FieldInfo reuseDelay = playerType.GetField("reuseDelay", BindingFlags.Instance | BindingFlags.Public);
            Type mainType = typeof(Main);
            FieldInfo drawingPlayerChat = mainType.GetField("drawingPlayerChat", BindingFlags.Static | BindingFlags.Public);
            FieldInfo selectedItem = playerType.GetField("selectedItem", BindingFlags.Instance | BindingFlags.Public);
            FieldInfo editSign = mainType.GetField("editSign", BindingFlags.Static | BindingFlags.Public);
            FieldInfo editChest = mainType.GetField("editChest", BindingFlags.Static | BindingFlags.Public);

            if (c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld(itemAnimation),
                    x => x.MatchBrtrue(out LabelKey),
                    x => x.MatchLdarg(0),
                    x => x.MatchCall(ItemTimeIsZero.GetMethod),
                    x => x.MatchBrfalse(out LabelKey),
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld(reuseDelay),
                    x => x.MatchBrtrue(out LabelKey)
                    ))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate(static (Player self) => TF2Weapon.CanSwitchWeapon(self));
                c.Emit(OpCodes.Brfalse, LabelKey);
                if (c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchLdcI4(0),
                    x => x.MatchStloc(49),
                    x => x.MatchLdsfld(drawingPlayerChat),
                    x => x.MatchBrtrue(out LabelKey),
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld(selectedItem),
                    x => x.MatchLdcI4(58),
                    x => x.MatchBeq(out LabelKey),
                    x => x.MatchLdsfld(editSign),
                    x => x.MatchBrtrue(out LabelKey),
                    x => x.MatchLdsfld(editChest),
                    x => x.MatchBrtrue(out LabelKey)
                    ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate(static (Player self) => TF2Weapon.CanSwitchWeapon(self));
                    c.Emit(OpCodes.Brfalse, LabelKey);
                }
                else
                    MonoModHooks.DumpIL(ModContent.GetInstance<TF2>(), il);
            }
            else
                MonoModHooks.DumpIL(ModContent.GetInstance<TF2>(), il);
        }

        private void OnHook_LeftClick(On_ItemSlot.orig_LeftClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
        {
            Player player = Main.LocalPlayer;
            if (TF2Weapon.CanSwitchWeapon(player))
                orig(inv, context, slot);
        }

        private void Hook_UICharacterList(On_UICharacterListItem.orig_ctor orig, UICharacterListItem self, PlayerFileData data, int snapPointIndex)
        {
            orig(self, data, snapPointIndex);
            if (ModContent.GetInstance<TF2ConfigClient>().DisablePlayerIcons) return;
            if (data.Player.GetModPlayer<TF2Player>().classIconID != 0)
            {
                classUI = new ClassIcon(data.Player);
                classUI.Left.Set(4f, 0f);
                self.Append(classUI);
            }
        }

        private void Hook_UICharacter_DrawSelf(On_UICharacter.orig_DrawSelf orig, UICharacter self, SpriteBatch spriteBatch)
        {
            if (ModContent.GetInstance<TF2ConfigClient>().DisablePlayerIcons)
            {
                orig(self, spriteBatch);
                return;
            }
            var playerField = typeof(UICharacter).GetField("_player", BindingFlags.Instance | BindingFlags.NonPublic);
            var _player = (Player)playerField.GetValue(self);
            if (_player.GetModPlayer<TF2Player>().classIconID != 0) return;
            orig(self, spriteBatch);
        }

        public static bool IsItemInHotbar(Player player, Item item)
        {
            bool foundItem = false;
            for (int i = 0; i < 10; i++)
            {
                if (player.inventory[i] == item)
                {
                    foundItem = true;
                    break;
                }
            }
            return foundItem;
        }

        public static bool IsItemTypeInHotbar(Player player, int type)
        {
            bool foundItem = false;
            for (int i = 0; i < 10; i++)
            {
                if (player.inventory[i].type == type)
                {
                    foundItem = true;
                    break;
                }
            }
            return foundItem;
        }


        public static int GetItemInHotbar(Player player, Item item)
        {
            int inventorySlot = 0;
            for (int i = 0; i < 10; i++)
            {
                if (player.inventory[i] == item)
                {
                    inventorySlot = i;
                    break;
                }
            }
            return inventorySlot;
        }

        public static int GetItemTypeInHotbar(Player player, int type)
        {
            int inventorySlot = 0;
            for (int i = 0; i < 10; i++)
            {
                if (player.inventory[i].type == type)
                {
                    inventorySlot = i;
                    break;
                }
            }
            return inventorySlot;
        }

        public static bool FindPlayer(Projectile projectile, float maxDetectDistance)
        {
            bool playerFound = false;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            foreach (Player target in Main.player)
            {
                float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, projectile.Center);

                // Check if it is within the radius
                if (sqrDistanceToTarget < sqrMaxDetectDistance && target == Main.player[projectile.owner])
                {
                    sqrMaxDetectDistance = sqrDistanceToTarget;
                    playerFound = true;
                }
            }
            return playerFound;
        }

        public static bool FindNPC(Projectile projectile, float maxDetectDistance)
        {
            bool npcFound = false;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            foreach (NPC target in Main.npc)
            {
                float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, projectile.Center);

                // Check if it is within the radius
                if (sqrDistanceToTarget < sqrMaxDetectDistance && !target.friendly)
                {
                    sqrMaxDetectDistance = sqrDistanceToTarget;
                    npcFound = true;
                }
            }
            return npcFound;
        }

        public static void SetPlayerDirection(Player player)
        {
            Vector2 pointPosition = player.RotatedRelativePoint(player.MountedCenter, reverseRotation: true);
            Vector2 value = Vector2.UnitX.RotatedBy(player.fullRotation);
            Vector2 vector = Main.MouseWorld - pointPosition;
            if (vector != Vector2.Zero)
                vector.Normalize();
            float num = Vector2.Dot(value, vector);
            if (num > 0f)
                player.ChangeDir(1);
            else
                player.ChangeDir(-1);
        }

        public static Rectangle MeleeHitbox(Player player)
        {
            Rectangle heldItemFrame = Item.GetDrawHitbox(player.HeldItem.type, player);
            Rectangle itemRectangle = new Rectangle((int)player.itemLocation.X, (int)player.itemLocation.Y, heldItemFrame.Width, heldItemFrame.Height);
            float adjustedItemScale = player.GetAdjustedItemScale(player.HeldItem);
            itemRectangle.Width = (int)(itemRectangle.Width * adjustedItemScale);
            itemRectangle.Height = (int)(itemRectangle.Height * adjustedItemScale);
            if (player.direction == -1)
                itemRectangle.X -= itemRectangle.Width;
            if (player.gravDir == 1f)
                itemRectangle.Y -= itemRectangle.Height;
            if (player.itemAnimation < player.itemAnimationMax * 0.333)
            {
                if (player.direction == -1)
                    itemRectangle.X -= (int)(itemRectangle.Width * 1.4 - itemRectangle.Width);
                itemRectangle.Width = (int)(itemRectangle.Width * 1.4);
                itemRectangle.Y += (int)(itemRectangle.Height * 0.5 * player.gravDir);
                itemRectangle.Height = (int)(itemRectangle.Height * 1.1);
            }
            else if (!(player.itemAnimation < player.itemAnimationMax * 0.666))
            {
                if (player.direction == 1)
                    itemRectangle.X -= (int)(itemRectangle.Width * 1.2);
                itemRectangle.Width *= 2;
                itemRectangle.Y -= (int)((itemRectangle.Height * 1.4 - itemRectangle.Height) * player.gravDir);
                itemRectangle.Height = (int)(itemRectangle.Height * 1.4);
            }
            return itemRectangle;
        }

        public static bool CanParryProjectile(Projectile projectile)
        {
            if (projectile.GetGlobalProjectile<TF2ProjectileBase>().healingProjectile) return false;
            if (projectile.ModProjectile == null) return projectile.aiStyle != 84;
            return projectile.ModProjectile.ShouldUpdatePosition();
        }

        public static void Explode(Projectile projectile, SoundStyle sound, int radius)
        {
            SoundEngine.PlaySound(sound, projectile.position);
            for (int i = 0; i < 50; i++)
            {
                Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
                dust.velocity *= 1.4f;
            }

            for (int i = 0; i < 80; i++)
            {
                Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Torch, 0f, 0f, 100, default, 3f);
                dust.noGravity = true;
                dust.velocity *= 5f;
                dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Torch, 0f, 0f, 100, default, 2f);
                dust.velocity *= 3f;
            }

            if (projectile.owner == Main.myPlayer && ModContent.GetInstance<TF2Config>().Explosions)
            {
                int explosionRadius = radius;
                int minTileX = (int)(projectile.Center.X / 16f - explosionRadius);
                int maxTileX = (int)(projectile.Center.X / 16f + explosionRadius);
                int minTileY = (int)(projectile.Center.Y / 16f - explosionRadius);
                int maxTileY = (int)(projectile.Center.Y / 16f + explosionRadius);
                Utils.ClampWithinWorld(ref minTileX, ref minTileY, ref maxTileX, ref maxTileY);
                bool explodeWalls = projectile.ShouldWallExplode(projectile.Center, explosionRadius, minTileX, maxTileX, minTileY, maxTileY);
                projectile.ExplodeTiles(projectile.Center, explosionRadius, minTileX, maxTileX, minTileY, maxTileY, explodeWalls);
            }
        }

        public static bool DrawProjectile(Projectile projectile, ref Color lightColor)
        {
            Main.instance.LoadProjectile(projectile.type);
            Texture2D texture = TextureAssets.Projectile[projectile.type].Value;
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, projectile.height * 0.5f);
            for (int i = 0; i < projectile.oldPos.Length; i++)
            {
                Vector2 drawPos = projectile.oldPos[i] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
                Color color = projectile.GetAlpha(lightColor) * ((projectile.oldPos.Length - i) / (float)projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0);
            }
            return true;
        }

        public static void ExtinguishPyroFlames(NPC npc, int buffTypeToRemove)
        {
            for (int i = 0; i < NPC.maxBuffs; i++)
            {
                int buffTypes = npc.buffType[i];
                if (buffTypes == buffTypeToRemove && npc.buffTime[i] > 0)
                {
                    npc.DelBuff(i);
                    i = -1;
                }
            }
        }

        public static bool IsPlayerOnFire(Player player) => player.HasBuff(ModContent.BuffType<PyroFlames>()) || player.HasBuff(ModContent.BuffType<PyroFlamesDegreaser>());

        public static bool IsNPCOnFire(NPC npc) => npc.HasBuff(ModContent.BuffType<PyroFlames>()) || npc.HasBuff(ModContent.BuffType<PyroFlamesDegreaser>());

        public static void Dialogue(string text, Color color)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(text, color);
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(25, -1, -1, NetworkText.FromLiteral(text), 255, color.R, color.G, color.B, 0, 0, 0);
        }

        public struct WeaponSize
        {
            public int X;
            public int Y;

            public WeaponSize(int x, int y)
            {
                X = x;
                Y = y;
            }

            public static WeaponSize MeleeWeaponSize = new WeaponSize(50, 50);

            public static explicit operator Vector2(WeaponSize v)
            {
                float vectorX = v.X;
                float vectorY = v.Y;
                return new Vector2(vectorX, vectorY);
            }
        }
    }
}