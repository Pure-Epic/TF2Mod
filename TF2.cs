using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using ReLogic.Content;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.GameInput;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items;
using TF2.Content.Items.Consumables;
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
using TF2.Content.Mounts;
using TF2.Content.Projectiles;
using TF2.Content.UI;
using TF2.Content.UI.MercenaryCreationMenu;
using TF2.Gensokyo.Content.Items.BossSummons;

namespace TF2
{
    public class TF2 : Mod
    {
        internal static LocalizedText[] TF2DeathMessagesLocalization { get; private set; }

        internal static LocalizedText TF2MercenaryText { get; private set; }

        internal static Asset<Texture2D> ClassPowerIcon;
        // Registers a new custom currency
        public static readonly int Australium = CustomCurrencyManager.RegisterCurrency(new Content.Items.Currencies.AustraliumCurrency(ModContent.ItemType<Content.Items.Currencies.Australium>(), 999L, "Australium"));
        public static IPlayerRenderer PlayerRenderer = new TF2PlayerRenderer();
        private ClassIcon classUI;
        public static Mod Gensokyo;
        public static bool gensokyoLoaded;
        public Hook ModifyMaxStats = new Hook(typeof(PlayerLoader).GetMethod("ModifyMaxStats", BindingFlags.Static | BindingFlags.Public), Hook_ModifyMaxStats);
        public delegate void ModifyMaxStatsAction(Player player);

        public override void Load()
        {
            if (ModLoader.TryGetMod("HEROsMod", out Mod HEROsMod))
            {
                HEROsMod.Call("AddItemCategory", "Mercenary", "Weapons",
                (Predicate<Item>)((item) =>
                {
                    return item.ModItem is TF2Item;
                }));
            }
            TF2DeathMessagesLocalization = new LocalizedText[]
            {
                Language.GetText("Mods.TF2.DeathMessages.Bleeding"),
                Language.GetText("Mods.TF2.DeathMessages.BostonBasher"),
                Language.GetText("Mods.TF2.DeathMessages.Explosion"),
                Language.GetText("Mods.TF2.DeathMessages.HalfZatoichi"),
                Language.GetText("Mods.TF2.DeathMessages.Fire"),
                Language.GetText("Mods.TF2.DeathMessages.Sapper"),
                Language.GetText("Mods.TF2.DeathMessages.Backstab")
            };
            TF2MercenaryText = Language.GetText("Mods.TF2.UI.TF2MercenaryCreation.Mercenary");
            ClassPowerIcon = ModContent.Request<Texture2D>("TF2/Content/Textures/UI/ClassPower");
            On_UICharacter.DrawSelf += Hook_UICharacter_DrawSelf;
            On_UICharacterListItem.ctor += Hook_UICharacterList;
            IL_UICharacterListItem.DrawSelf += Hook_UICharacterListItem_DrawSelf; ;
            On_UICharacterSelect.NewCharacterClick += Hook_NewCharacterClick;
            On_Player.Spawn += Hook_Spawn;
            IL_Player.Update += Hook_Update;
            On_Player.GetWeaponDamage += Hook_GetWeaponDamage;
            On_Player.GetImmuneAlpha += Hook_GetImmuneAlpha;
            On_Player.GetImmuneAlphaPure += Hook_GetImmuneAlphaPure;
            On_PlayerDrawSet.HeadOnlySetup += Hook_HeadOnlySetup;
            IL_PlayerDrawSet.BoringSetup_2 += Hook_PlayerDrawSet;
            On_ItemSlot.LeftClick_ItemArray_int_int += Hook_LeftClick;
            On_Main.GUIHotbarDrawInner += Hook_GUIHotbarDrawInner;
            IL_Main.MouseText_DrawItemTooltip += Hook_MouseText_DrawItemTooltip;
            On_Main.DrawMouseOver += Hook_DrawMouseOver;
            On_NPC.HitModifiers.GetDamage += Hook_GetDamage;
            On_NPC.NPCLoot_DropHeals += Hook_NPCLoot_DropHeals;
            On_NPC.DoDeathEvents_DropBossPotionsAndHearts += Hook_DoDeathEvents_DropBossPotionsAndHearts;
        }

        public override void Unload()
        {
            On_UICharacter.DrawSelf -= Hook_UICharacter_DrawSelf;
            On_UICharacterListItem.ctor -= Hook_UICharacterList;
            IL_UICharacterListItem.DrawSelf -= Hook_UICharacterListItem_DrawSelf;
            On_UICharacterSelect.NewCharacterClick -= Hook_NewCharacterClick;
            On_Player.Spawn -= Hook_Spawn;
            IL_Player.Update -= Hook_Update;
            On_Player.GetWeaponDamage -= Hook_GetWeaponDamage;
            On_Player.GetImmuneAlpha -= Hook_GetImmuneAlpha;
            On_Player.GetImmuneAlphaPure -= Hook_GetImmuneAlphaPure;
            On_PlayerDrawSet.HeadOnlySetup -= Hook_HeadOnlySetup;
            IL_PlayerDrawSet.BoringSetup_2 -= Hook_PlayerDrawSet;
            On_ItemSlot.LeftClick_ItemArray_int_int -= Hook_LeftClick;
            On_Main.GUIHotbarDrawInner -= Hook_GUIHotbarDrawInner;
            IL_Main.MouseText_DrawItemTooltip -= Hook_MouseText_DrawItemTooltip;
            On_Main.DrawMouseOver -= Hook_DrawMouseOver;
            On_NPC.HitModifiers.GetDamage -= Hook_GetDamage;
            On_NPC.NPCLoot_DropHeals -= Hook_NPCLoot_DropHeals;
            On_NPC.DoDeathEvents_DropBossPotionsAndHearts -= Hook_DoDeathEvents_DropBossPotionsAndHearts;
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

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            switch ((MessageType)reader.ReadByte())
            {
                case MessageType.SyncPlayer:
                    byte i = reader.ReadByte();
                    Player player = Main.player[i];
                    TF2Player p = Main.player[i].GetModPlayer<TF2Player>();
                    p.currentClass = reader.ReadByte();
                    p.healthBonus = reader.ReadInt32();
                    p.healthMultiplier = reader.ReadSingle();
                    p.healReduction = reader.ReadSingle();
                    p.overhealMultiplier = reader.ReadSingle();
                    player.itemAnimation = reader.ReadInt32();
                    player.itemAnimationMax = reader.ReadInt32();
                    p.overheal = reader.ReadInt32();
                    p.focus = reader.ReadBoolean();
                    if (Main.netMode == NetmodeID.Server)
                        p.SyncPlayer(-1, whoAmI, false);
                    break;
                case MessageType.SyncMount:
                    i = reader.ReadByte();
                    player = Main.player[i];
                    player.Center = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                    player.velocity = Vector2.Zero;
                    player.direction = reader.ReadInt32();
                    if (Main.netMode == NetmodeID.Server)
                        TF2Mount.SendMountMessage(player);
                    break;
                case MessageType.SyncSyringe:
                    i = reader.ReadByte();
                    NetMessage.SendData(MessageID.SpiritHeal, number: reader.ReadByte(), number2: reader.ReadInt32());
                    Main.projectile[i].Kill();
                    break;
                case MessageType.Overheal:
                    i = reader.ReadByte();
                    int healAmount = reader.ReadInt32();
                    float limit = reader.ReadSingle();
                    if (healAmount > 0)
                    {
                        player = Main.player[i];
                        p = player.GetModPlayer<TF2Player>();
                        int maxHealth = TF2Player.TotalHealth(player);
                        player.statLife += healAmount;
                        if (player.statLife > maxHealth && p.overheal < OverhealRound(maxHealth * limit * p.overhealMultiplier))
                        {
                            int extraHealth = player.statLife - maxHealth - p.overheal;
                            p.overheal += extraHealth;
                            if (p.overheal > OverhealRound(maxHealth * limit * p.overhealMultiplier))
                                p.overheal = OverhealRound(maxHealth * limit * p.overhealMultiplier);
                            player.statLife = Round((p.BaseHealth + p.healthBonus) * p.healthMultiplier + p.overheal);
                        }
                        player.HealEffect(healAmount, broadcast: false);
                        if (Main.netMode == NetmodeID.Server)
                            OverhealMultiplayer(player, healAmount, limit);
                    }
                    break;
                case MessageType.KillProjectile:
                    i = reader.ReadByte();
                    Main.projectile[i].Kill();
                    break;
                default:
                    break;
            }
        }

        private static void AddShopItem(int argument, bool availability, string category, int type, int amount) => Gensokyo.Call("AddShopItem", argument, availability, category, type, amount, (int)ItemID.None, 0);

        internal enum MessageType : byte
        {
            SyncPlayer,
            SyncMount,
            SyncSyringe,
            Overheal,
            KillProjectile
        }

        private static void Hook_ModifyMaxStats(ModifyMaxStatsAction orig, Player player)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (p.ClassSelected)
            {
                player.statLifeMax = Round((p.BaseHealth + p.healthBonus) * p.healthMultiplier + p.overheal);
                player.statManaMax = 0;
            }
            else
                orig(player);
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
            if (_player.GetModPlayer<TF2Player>().ClassSelected) return;
            orig(self, spriteBatch);
        }

        private void Hook_UICharacterList(On_UICharacterListItem.orig_ctor orig, UICharacterListItem self, PlayerFileData data, int snapPointIndex)
        {
            orig(self, data, snapPointIndex);
            if (ModContent.GetInstance<TF2ConfigClient>().DisablePlayerIcons) return;
            if (data.Player.GetModPlayer<TF2Player>().ClassSelected)
            {
                classUI = new ClassIcon(data.Player);
                classUI.Left.Set(4f, 0f);
                self.Append(classUI);
            }
        }

        private void Hook_UICharacterListItem_DrawSelf(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(
                    x => x.MatchLdflda("Terraria.Player", "statLifeMax2"),
                    x => x.MatchCall("System.Int32", "ToString"),
                    x => x.MatchLdstr("GameUI.PlayerLifeMax"),
                    x => x.MatchCall("Terraria.Localization.Language", "GetTextValue"),
                    x => x.MatchCall("System.String", "Concat")
                    ))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Ldfld, typeof(UICharacterListItem).GetField("_data", BindingFlags.Instance | BindingFlags.NonPublic));
                c.Emit(OpCodes.Callvirt, typeof(PlayerFileData).GetMethod("get_Player", BindingFlags.Instance | BindingFlags.Public));
                c.EmitDelegate<Func<Player, bool>>((player) => player.GetModPlayer<TF2Player>().ClassSelected);

                ILLabel branch = il.DefineLabel();
                ILLabel end = il.DefineLabel();

                c.Emit(OpCodes.Brtrue, branch);
                c.Index += 5;
                c.Emit(OpCodes.Br, end);
                c.MarkLabel(branch);
                c.EmitDelegate<Func<Player, string>>((player) =>
                    {
                        TF2Player p = player.GetModPlayer<TF2Player>();
                        return p.cachedHealth.ToString() + Language.GetTextValue("GameUI.PlayerLifeMax");
                    }
                );
                c.MarkLabel(end);

                if (c.TryGotoNext(
                    x => x.MatchLdsfld("Terraria.GameContent.TextureAssets", "Mana"),
                    x => x.MatchCallvirt<Asset<Texture2D>>("get_Value")
                    ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.Emit(OpCodes.Ldfld, typeof(UICharacterListItem).GetField("_data", BindingFlags.Instance | BindingFlags.NonPublic));
                    c.Emit(OpCodes.Callvirt, typeof(PlayerFileData).GetMethod("get_Player", BindingFlags.Instance | BindingFlags.Public));
                    c.EmitDelegate<Func<Player, bool>>((player) => player.GetModPlayer<TF2Player>().ClassSelected);

                    ILLabel branch2 = il.DefineLabel(); // If the player is a mercenary
                    ILLabel end2 = il.DefineLabel(); // If the player is a Terrarian

                    c.Emit(OpCodes.Brtrue, branch2);
                    c.Index += 2;
                    c.Emit(OpCodes.Br, end2);
                    c.MarkLabel(branch2);
                    c.EmitDelegate(GetClassPowerTexture);
                    c.Emit(OpCodes.Callvirt, typeof(Asset<Texture2D>).GetMethod("get_Value", BindingFlags.Public | BindingFlags.Instance));
                    c.MarkLabel(end2);

                    if (c.TryGotoNext(
                        x => x.MatchLdflda("Terraria.Player", "statManaMax2"),
                        x => x.MatchCall("System.Int32", "ToString"),
                        x => x.MatchLdstr("GameUI.PlayerManaMax"),
                        x => x.MatchCall("Terraria.Localization.Language", "GetTextValue"),
                        x => x.MatchCall("System.String", "Concat")
                        ))
                    {
                        c.Emit(OpCodes.Ldarg_0);
                        c.Emit(OpCodes.Ldfld, typeof(UICharacterListItem).GetField("_data", BindingFlags.Instance | BindingFlags.NonPublic));
                        c.Emit(OpCodes.Callvirt, typeof(PlayerFileData).GetMethod("get_Player", BindingFlags.Instance | BindingFlags.Public));
                        c.EmitDelegate<Func<Player, bool>>((player) => player.GetModPlayer<TF2Player>().ClassSelected);

                        ILLabel branch3 = il.DefineLabel();
                        ILLabel end3 = il.DefineLabel();

                        c.Emit(OpCodes.Brtrue, branch3);
                        c.Index += 5;
                        c.Emit(OpCodes.Br, end3);
                        c.MarkLabel(branch3);
                        c.EmitDelegate<Func<Player, string>>((player) => "x" + player.GetModPlayer<TF2Player>().classMultiplier.ToString());
                        c.MarkLabel(end3);

                        if (c.TryGotoNext(
                        x => x.MatchLdstr("UI.Softcore"),
                        x => x.MatchCall("Terraria.Localization.Language", "GetTextValue"),
                        x => x.MatchStloc(10)
                        ))
                        {
                            ILLabel branch4 = il.DefineLabel();
                            ILLabel end4 = il.DefineLabel();

                            c.Emit(OpCodes.Brtrue, branch4);
                            c.Index += 3;
                            c.MarkLabel(branch4);
                            c.Emit(OpCodes.Ldarg_0);
                            c.Emit(OpCodes.Ldfld, typeof(UICharacterListItem).GetField("_data", BindingFlags.Instance | BindingFlags.NonPublic));
                            c.Emit(OpCodes.Callvirt, typeof(PlayerFileData).GetMethod("get_Player", BindingFlags.Instance | BindingFlags.Public));
                            c.EmitDelegate<Func<Player, string>>((player) =>
                            {
                                if (player.GetModPlayer<TF2Player>().ClassSelected)
                                    return TF2MercenaryText.Value;
                                else
                                {
                                    return player.difficulty switch
                                    {
                                        0 => Language.GetTextValue("UI.Softcore"),
                                        1 => Language.GetTextValue("UI.Mediumcore"),
                                        2 => Language.GetTextValue("UI.Hardcore"),
                                        3 => Language.GetTextValue("UI.Creative"),
                                        _ => ""
                                    };
                                }
                            });
                            c.Emit(OpCodes.Stloc, 10);
                            c.Emit(OpCodes.Ldarg_0);
                            c.Emit(OpCodes.Ldfld, typeof(UICharacterListItem).GetField("_data", BindingFlags.Instance | BindingFlags.NonPublic));
                            c.Emit(OpCodes.Callvirt, typeof(PlayerFileData).GetMethod("get_Player", BindingFlags.Instance | BindingFlags.Public));
                            c.EmitDelegate<Func<Player, Color>>((player) =>
                            {
                                if (player.GetModPlayer<TF2Player>().ClassSelected)
                                    return new Color(243, 169, 87);
                                else
                                {
                                    return player.difficulty switch
                                    {
                                        0 => Color.White,
                                        1 => Main.mcColor,
                                        2 => Main.hcColor,
                                        3 => Main.creativeModeColor,
                                        _ => Color.Red
                                    };
                                }
                            });
                            c.Emit(OpCodes.Stloc, 11);
                            c.MarkLabel(end4);
                        }
                    }
                }
            }
        }

        private Asset<Texture2D> GetClassPowerTexture() => ClassPowerIcon;

        private void Hook_NewCharacterClick(On_UICharacterSelect.orig_NewCharacterClick orig, UICharacterSelect self, UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuOpen);
            Main.menuMode = 888;
            Main.MenuUI.SetState(new TF2ChoosePlayerType());
        }

        private void Hook_Spawn(On_Player.orig_Spawn orig, Player self, PlayerSpawnContext context)
        {
            orig(self, context);
            self.statLife = self.statLifeMax;
            TF2Player p = self.GetModPlayer<TF2Player>();
            p.overheal = 0;
            p.metal = p.maxMetal;
            VitaSawPlayer vitaSawPlayer = self.GetModPlayer<VitaSawPlayer>();
            if (vitaSawPlayer.deathUberCharge > 0)
            {
                for (int i = 0; i < self.inventory.Length; i++)
                {
                    Item item = self.inventory[i];
                    if (item.ModItem is TF2Weapon weapon && weapon.GetWeaponMechanic("Medi Gun"))
                        weapon.uberCharge = vitaSawPlayer.deathUberCharge;
                }
                p.organs = 0;
                vitaSawPlayer.deathUberCharge = 0;
            }
            self.GetModPlayer<RazorbackPlayer>().timer = Time(30);
            CloakPlayer cloakPlayer = self.GetModPlayer<CloakPlayer>();
            cloakPlayer.cloakMeter = cloakPlayer.cloakMeterMax;
            CloakAndDaggerPlayer cloakAndDaggerPlayer = self.GetModPlayer<CloakAndDaggerPlayer>();
            cloakAndDaggerPlayer.cloakMeter = cloakAndDaggerPlayer.cloakMeterMax;
            FeignDeathPlayer feignDeathPlayer = self.GetModPlayer<FeignDeathPlayer>();
            feignDeathPlayer.cloakMeter = feignDeathPlayer.cloakMeterMax;
        }

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
            }
        }

        private int Hook_GetWeaponDamage(On_Player.orig_GetWeaponDamage orig, Player self, Item sItem, bool forTooltip)
        {
            StatModifier modifier = self.GetTotalDamage(sItem.DamageType);
            CombinedHooks.ModifyWeaponDamage(self, sItem, ref modifier);
            return self.GetModPlayer<TF2Player>().ClassSelected ? Math.Max(0, Round(modifier.ApplyTo(sItem.damage))) : orig(self, sItem, forTooltip);
        }

        private Color Hook_GetImmuneAlpha(On_Player.orig_GetImmuneAlpha orig, Player self, Color newColor, float alphaReduction) => self.GetModPlayer<TF2Player>().ClassSelected ? newColor : orig(self, newColor, alphaReduction);

        private Color Hook_GetImmuneAlphaPure(On_Player.orig_GetImmuneAlphaPure orig, Player self, Color newColor, float alphaReduction) => self.GetModPlayer<TF2Player>().ClassSelected ? newColor : orig(self, newColor, alphaReduction);

        private void Hook_HeadOnlySetup(On_PlayerDrawSet.orig_HeadOnlySetup orig, ref PlayerDrawSet self, Player drawPlayer2, List<DrawData> drawData, List<int> dust, List<int> gore, float X, float Y, float Alpha, float Scale)
        {
            orig(ref self, drawPlayer2, drawData, dust, gore, X, Y, Alpha, Scale);
            if (drawPlayer2.GetModPlayer<TF2Player>().ClassSelected)
                drawPlayer2.hairColor = Color.Transparent;
        }

        private void Hook_PlayerDrawSet(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            ILLabel LabelKey = null;
            FieldInfo drawPlayer = typeof(PlayerDrawSet).GetField("drawPlayer", BindingFlags.Instance | BindingFlags.Public);
            FieldInfo opacityForAnimation = typeof(Player).GetField("opacityForAnimation", BindingFlags.Instance | BindingFlags.Public);
            if (c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld(drawPlayer),
                    x => x.MatchLdfld(opacityForAnimation),
                    x => x.MatchLdcR4(1),
                    x => x.MatchBeq(out LabelKey)
                    ))
            {
                c.Emit(OpCodes.Ldarg_1);
                c.EmitDelegate((Player self) => self.GetModPlayer<TF2Player>().ClassSelected && self.GetModPlayer<TF2Player>().currentClass != 9);
                c.Emit(OpCodes.Brtrue, LabelKey);
            }
        }

        private void Hook_LeftClick(On_ItemSlot.orig_LeftClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
        {
            Player player = Main.LocalPlayer;
            if (TF2Weapon.CanSwitchWeapon(player))
                orig(inv, context, slot);
        }

        private void Hook_MouseText_DrawItemTooltip(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            ILLabel LabelKey = null;
            FieldInfo mouseTextColor = typeof(Main).GetField("mouseTextColor", BindingFlags.Static | BindingFlags.Public);
            if (c.TryGotoNext(
                    MoveType.After,
                    x => x.MatchLdcI4(0),
                    x => x.MatchStloc(25),
                    x => x.MatchLdsfld(mouseTextColor),
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(255),
                    x => x.MatchDiv(),
                    x => x.MatchStloc(14),
                    x => x.MatchLdloc(0),
                    x => x.MatchBrfalse(out LabelKey)
                    ))
            {
                c.EmitDelegate(static () => Main.HoverItem.ModItem is TF2Item);
                c.Emit(OpCodes.Brtrue, LabelKey);
            }
        }

        private void Hook_DrawMouseOver(On_Main.orig_DrawMouseOver orig, Main self)
        {
            MethodInfo hoverOverNPCs = typeof(Main).GetMethod("HoverOverNPCs", BindingFlags.Instance | BindingFlags.NonPublic);
            Matrix _uiScaleMatrix = (Matrix)typeof(Main).GetField("_uiScaleMatrix", BindingFlags.Static | BindingFlags.NonPublic).GetValue(self);
            PlayerInput.SetZoom_Unscaled();
            PlayerInput.SetZoom_MouseInWorld();
            Rectangle mouseRectangle = new Rectangle((int)(Main.mouseX + Main.screenPosition.X), (int)(Main.mouseY + Main.screenPosition.Y), 1, 1);
            if (Main.LocalPlayer.gravDir == -1f)
                mouseRectangle.Y = (int)Main.screenPosition.Y + Main.screenHeight - Main.mouseY;
            PlayerInput.SetZoom_UI();
            if (!Main.LocalPlayer.ghost)
                Main.ResourceSetsManager.TryToHoverOverResources();
            Main.AchievementAdvisor.DrawMouseHover();
            IngameOptions.MouseOver();
            IngameFancyUI.MouseOver();
            if (!Main.mouseText)
            {
                for (int i = 0; i < 400; i++)
                {
                    if (!Main.item[i].active)
                        continue;
                    Rectangle drawHitbox = Item.GetDrawHitbox(Main.item[i].type, null);
                    Vector2 bottom = Main.item[i].Bottom;
                    Rectangle value = new Rectangle((int)(bottom.X - drawHitbox.Width * 0.5f), (int)(bottom.Y - drawHitbox.Height), drawHitbox.Width, drawHitbox.Height);
                    if (mouseRectangle.Intersects(value))
                    {
                        Main.LocalPlayer.cursorItemIconEnabled = false;
                        string text = Main.item[i].AffixName();
                        if (Main.item[i].stack > 1)
                            text = text + " (" + Main.item[i].stack + ")";
                        if (Main.item[i].playerIndexTheItemIsReservedFor < 255 && Main.showItemOwner)
                            text = text + " <" + Main.player[Main.item[i].playerIndexTheItemIsReservedFor].name + ">";
                        Main.rare = Main.item[i].rare;
                        if (Main.item[i].expert)
                            Main.rare = ItemRarityID.Expert;
                        if (Main.item[i].master)
                            Main.rare = ItemRarityID.Master;
                        if (Main.item[i].ModItem is TF2Item weapon)
                            text = weapon.GetItemName();
                        self.MouseTextHackZoom(text, Main.rare, 0);
                        Main.mouseText = true;
                        break;
                    }
                }
            }
            for (int j = 0; j < 255; j++)
            {
                if (!Main.player[j].active || Main.myPlayer == j || Main.player[j].dead || Main.player[j].ShouldNotDraw || !(Main.player[j].stealth > 0.5))
                    continue;
                if (!Main.mouseText && mouseRectangle.Intersects(new Rectangle((int)(Main.player[j].position.X + Main.player[j].width * 0.5 - 16.0), (int)(Main.player[j].position.Y + Main.player[j].height - 48f), 32, 48)))
                {
                    Main.LocalPlayer.cursorItemIconEnabled = false;
                    TF2Player p = Main.player[j].GetModPlayer<TF2Player>();
                    int num = Main.player[j].statLife;
                    if (num < 0)
                        num = 0;
                    string text2 = Main.player[j].name + ": " + num + "/" + Round((p.BaseHealth + p.healthBonus) * p.healthMultiplier);
                    if (Main.player[j].hostile)
                        text2 = text2 + " " + Language.GetTextValue("Game.PvPFlag");
                    self.MouseTextHackZoom(text2, 0, Main.player[j].difficulty);
                    Main.mouseText = true;
                }
            }
            Main.HoveringOverAnNPC = false;
            if (!Main.mouseText)
                hoverOverNPCs.Invoke(self, new object[] { mouseRectangle });
            if (!Main.mouseText && Main.signHover != -1 && Main.sign[Main.signHover] != null && !Main.LocalPlayer.mouseInterface && !string.IsNullOrWhiteSpace(Main.sign[Main.signHover].text))
            {
                string[] array = Utils.WordwrapString(Main.sign[Main.signHover].text, FontAssets.MouseText.Value, 460, 10, out int lineAmount);
                lineAmount++;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(0, null, null, null, null, null, _uiScaleMatrix);
                PlayerInput.SetZoom_UI();
                int num2 = Main.screenWidth;
                int num3 = Main.screenHeight;
                int num4 = Main.mouseX;
                int num5 = Main.mouseY;
                PlayerInput.SetZoom_UI();
                PlayerInput.SetZoom_Test();
                float num6 = 0f;
                for (int k = 0; k < lineAmount; k++)
                {
                    float x = FontAssets.MouseText.Value.MeasureString(array[k]).X;
                    if (num6 < x)
                        num6 = x;
                }
                if (num6 > 460f)
                    num6 = 460f;
                bool settingsEnabled_OpaqueBoxBehindTooltips = Main.SettingsEnabled_OpaqueBoxBehindTooltips;
                Vector2 vector = new Vector2(num4, num5) + new Vector2(16f);
                if (settingsEnabled_OpaqueBoxBehindTooltips)
                    vector += new Vector2(8f, 2f);
                if (vector.Y > num3 - 30 * lineAmount)
                    vector.Y = num3 - 30 * lineAmount;
                if (vector.X > num2 - num6)
                    vector.X = num2 - num6;
                Color color = new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor);
                if (settingsEnabled_OpaqueBoxBehindTooltips)
                {
                    color = Color.Lerp(color, Color.White, 1f);
                    int num7 = 10;
                    int num8 = 5;
                    Utils.DrawInvBG(Main.spriteBatch, new Rectangle((int)vector.X - num7, (int)vector.Y - num8, (int)num6 + num7 * 2, 30 * lineAmount + num8 + num8 / 2), new Color(23, 25, 81, 255) * 0.925f * 0.85f);
                }
                for (int l = 0; l < lineAmount; l++)
                    Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, array[l], vector.X, vector.Y + l * 30, color, Color.Black, Vector2.Zero);
                Main.mouseText = true;
            }
            PlayerInput.SetZoom_UI();
        }

        private void Hook_GUIHotbarDrawInner(On_Main.orig_GUIHotbarDrawInner orig, Main self)
        {
            if (Main.playerInventory || Main.LocalPlayer.ghost) return;
            string text = Lang.inter[37].Value;
            Item item = Main.LocalPlayer.inventory[Main.LocalPlayer.selectedItem];
            TF2Item modItem = item.ModItem as TF2Item;
            if (item.Name != null && item.Name != "")
                text = item.ModItem is TF2Item ? modItem.GetItemName() : item.AffixName();
            Vector2 vector = new Vector2(236f - (FontAssets.MouseText.Value.MeasureString(text) / 2f).X, 0f);
            Color color = item.ModItem is TF2Item ? modItem.GetItemColor() : new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor);
            if (item.ModItem is TF2Item)
                TF2Item.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, text, vector, color, 0f, default, Vector2.One);
            else
                DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.MouseText.Value, text, vector, color, 0f, default, 1f, 0, 0f);
            int num = 20;
            for (int i = 0; i < 10; i++)
            {
                if (i == Main.LocalPlayer.selectedItem)
                {
                    if (Main.hotbarScale[i] < 1f)
                        Main.hotbarScale[i] += 0.05f;
                }
                else if (Main.hotbarScale[i] > 0.75)
                    Main.hotbarScale[i] -= 0.05f;
                float num2 = Main.hotbarScale[i];
                int num3 = (int)(20f + 22f * (1f - num2));
                int a = (int)(75f + 150f * num2);
                Color lightColor;
                lightColor = new Color(255, 255, 255, a);
                if (!Main.LocalPlayer.hbLocked && !PlayerInput.IgnoreMouseInterface && Main.mouseX >= num && Main.mouseX <= num + TextureAssets.InventoryBack.Width() * Main.hotbarScale[i] && Main.mouseY >= num3 && Main.mouseY <= num3 + TextureAssets.InventoryBack.Height() * Main.hotbarScale[i] && !Main.LocalPlayer.channel)
                {
                    Main.LocalPlayer.mouseInterface = true;
                    Main.LocalPlayer.cursorItemIconEnabled = false;
                    if (Main.mouseLeft && !Main.LocalPlayer.hbLocked && !Main.blockMouse)
                        Main.LocalPlayer.changeItem = i;
                    Item item2 = Main.LocalPlayer.inventory[i];
                    if (item2.ModItem is TF2Item)
                        Main.hoverItemName = (item2.ModItem as TF2Item).GetItemName();
                    else
                    {
                        Main.hoverItemName = item2.AffixName();
                        if (item2.stack > 1)
                            Main.hoverItemName = Main.hoverItemName + " (" + item2.stack.ToString() + ")";
                    }
                    Main.rare = Main.LocalPlayer.inventory[i].rare;
                }
                float num6 = Main.inventoryScale;
                Main.inventoryScale = num2;
                ItemSlot.Draw(Main.spriteBatch, Main.LocalPlayer.inventory, 13, i, new Vector2(num, num3), lightColor);
                Main.inventoryScale = num6;
                num += (int)(TextureAssets.InventoryBack.Width() * Main.hotbarScale[i]) + 4;
            }
            int selectedItem = Main.LocalPlayer.selectedItem;
            if (selectedItem >= 10 && (selectedItem != 58 || Main.mouseItem.type > ItemID.None))
            {
                float num4 = 1f;
                int num5 = (int)(20f + 22f * (1f - num4));
                int a2 = (int)(75f + 150f * num4);
                Color lightColor2;
                lightColor2 = new Color(255, 255, 255, a2);
                float num7 = Main.inventoryScale;
                Main.inventoryScale = num4;
                ItemSlot.Draw(Main.spriteBatch, Main.LocalPlayer.inventory, 13, selectedItem, new Vector2(num, num5), lightColor2);
                Main.inventoryScale = num7;
            }
        }

        private int Hook_GetDamage(On_NPC.HitModifiers.orig_GetDamage orig, ref NPC.HitModifiers self, float baseDamage, bool crit, bool damageVariation, float luck)
        {
            crit = (bool?)typeof(NPC.HitModifiers).GetField("_critOverride", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(self) ?? crit;
            float damage = self.SourceDamage.ApplyTo(baseDamage);
            damage *= self.TargetDamageMultiplier.Value;
            damage = Math.Max(damage, 1f);
            damage = (crit ? self.CritDamage : self.NonCritDamage).ApplyTo(damage);
            return self.DamageType == ModContent.GetInstance<TF2DamageClass>() ? Math.Clamp(Round(self.FinalDamage.ApplyTo(damage)), 1, (int)typeof(NPC.HitModifiers).GetField("_damageLimit", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(self)) : orig(ref self, baseDamage, crit, damageVariation, luck);
        }

        private void Hook_NPCLoot_DropHeals(On_NPC.orig_NPCLoot_DropHeals orig, NPC self, Player closestPlayer)
        {
            if (closestPlayer.GetModPlayer<TF2Player>().ClassSelected) return;
            else orig(self, closestPlayer);
        }

        private void Hook_DoDeathEvents_DropBossPotionsAndHearts(On_NPC.orig_DoDeathEvents_DropBossPotionsAndHearts orig, NPC self, ref string typeName)
        {
            if (!Main.player[self.FindClosestPlayer()].GetModPlayer<TF2Player>().ClassSelected)
                orig(self, ref typeName);
        }

        public static int Time(double time) => Convert.ToInt32(time * 60);

        public static int GetHealth(Player player, double value) => Round(player.GetModPlayer<TF2Player>().healthMultiplier * value);

        public static float GetHealthRaw(Player player, double value) => (float)(player.GetModPlayer<TF2Player>().healthMultiplier * value);

        public static int Round(double value) => (int)Math.Round(value, 0, MidpointRounding.AwayFromZero);

        public static int OverhealRound(double value) => (int)(value / 5f) * 5;

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
                if (player.inventory[i].type == type && player.inventory[i].ModItem is TF2Item weapon && weapon.equipped)
                {
                    foundItem = true;
                    break;
                }
            }
            return foundItem;
        }

        public static Item GetItemInHotbar(Player player, Item item)
        {
            Item foundItem = null;
            for (int i = 0; i < 10; i++)
            {
                if (player.inventory[i] == item && player.inventory[i].ModItem is TF2Item weapon && weapon.equipped)
                {
                    foundItem = player.inventory[i];
                    break;
                }
            }
            return foundItem;
        }

        public static Item GetItemInHotbar(Player player, int type)
        {
            Item foundItem = null;
            for (int i = 0; i < 10; i++)
            {
                if (player.inventory[i].type == type && player.inventory[i].ModItem is TF2Item weapon && weapon.equipped)
                {
                    foundItem = player.inventory[i];
                    break;
                }
            }
            return foundItem;
        }

        public static int GetItemTypeInHotbar(Player player, int type)
        {
            int inventorySlot = 0;
            for (int i = 0; i < 10; i++)
            {
                if (player.inventory[i].type == type && player.inventory[i].ModItem is TF2Item weapon && weapon.equipped)
                {
                    inventorySlot = i;
                    break;
                }
            }
            return inventorySlot;
        }

        public static Item GetItemInHotbar(Player player, int[] type)
        {
            Item foundItem = null;
            for (int i = 0; i < 10; i++)
            {
                if (type.Contains(player.inventory[i].type) && player.inventory[i].ModItem is TF2Item weapon && weapon.equipped)
                {
                    foundItem = player.inventory[i];
                    break;
                }
            }
            return foundItem;
        }

        public static TF2Projectile CreateProjectile(TF2Weapon weapon, IEntitySource spawnSource, Vector2 position, Vector2 velocity, int type, int damage, float knockBack, int owner = -1, float ai0 = 0, float ai1 = 0, float ai2 = 0)
        {
            int i = Projectile.NewProjectile(spawnSource, position, velocity, type, damage, knockBack, owner, ai0, ai1, ai2);
            TF2Projectile projectile = Main.projectile[i].ModProjectile as TF2Projectile;
            projectile.weapon = weapon;
            if (weapon != null)
                projectile.noDistanceModifier = projectile.weapon.noDistanceModifier;
            TF2Player player = Main.player[projectile.Projectile.owner].GetModPlayer<TF2Player>();
            if (player.crit)
                projectile.crit = true;
            else if (player.miniCrit)
                projectile.miniCrit = true;
            return projectile;
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
            if (projectile.ModProjectile is TF2Projectile tf2Projectile && tf2Projectile.healingProjectile) return false;
            if (projectile.ModProjectile == null) return projectile.aiStyle != 84;
            return projectile.ModProjectile.ShouldUpdatePosition();
        }

        public static void Explode(Projectile projectile, SoundStyle sound)
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

        public static float SwordRotation(Player player, float angle) => MathHelper.ToRadians(angle * (player.gravDir >= 0 ? 1f : -1f));

        public static bool IsPlayerOnFire(Player player) => player.HasBuff(ModContent.BuffType<PyroFlames>()) || player.HasBuff(ModContent.BuffType<PyroFlamesDegreaser>());

        public static bool IsNPCOnFire(NPC npc) => npc.HasBuff(ModContent.BuffType<PyroFlames>()) || npc.HasBuff(ModContent.BuffType<PyroFlamesDegreaser>());

        public static void Overheal(Player player, int healAmount, float limit = 0.5f)
        {
            if (healAmount > 0)
            {
                TF2Player p = player.GetModPlayer<TF2Player>();
                int maxHealth = TF2Player.TotalHealth(player);
                player.statLife += healAmount;
                if (player.statLife > maxHealth && p.overheal < OverhealRound(maxHealth * limit * p.overhealMultiplier))
                {
                    int extraHealth = player.statLife - maxHealth - p.overheal;
                    p.overheal += extraHealth;
                    if (p.overheal > OverhealRound(maxHealth * limit * p.overhealMultiplier))
                        p.overheal = OverhealRound(maxHealth * limit * p.overhealMultiplier);
                    player.statLife = Round((p.BaseHealth + p.healthBonus) * p.healthMultiplier + p.overheal);
                }
                player.HealEffect(healAmount, broadcast: false);
            }
        }

        public static void OverhealMultiplayer(Player player, int healAmount, float limit = 0.5f)
        {
            if (!player.GetModPlayer<TF2Player>().ClassSelected) return;
            ModPacket packet = ModContent.GetInstance<TF2>().GetPacket();
            packet.Write((byte)MessageType.Overheal);
            packet.Write((byte)player.whoAmI);
            packet.Write(healAmount);
            packet.Write(limit);
            packet.Send(-1, Main.myPlayer);
        }

        public static void DropLoot(NPC npc, int type, int chanceDenominator = 1, int minimumDropped = 1, int maximumDropped = 1)
        {
            if (Main.rand.NextBool(chanceDenominator))
            {
                int item = Item.NewItem(npc.GetSource_Loot(), (int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, type, Main.rand.Next(minimumDropped, maximumDropped + 1), noBroadcast: true);
                if (Main.netMode == NetmodeID.Server)
                {
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        if (Main.player[i].active)
                            NetMessage.SendData(MessageID.InstancedItem, i, -1, null, item);
                    }
                }
            }
        }

        public static void CreateSoulItem(NPC npc, float damage, float health, int pierce = 1)
        {
            SoulItem soulItem = Main.item[Item.NewItem(npc.GetSource_Loot(), (int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<SoulItem>())].ModItem as SoulItem;
            soulItem.damageMultiplier = damage;
            soulItem.healthMultiplier = health;
            soulItem.pierce = pierce;
        }

        public static void UpgradeDrill(NPC npc, int miningPower)
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (npc.playerInteraction[i] && miningPower > Main.player[i].GetModPlayer<TF2Player>().miningPower)
                    Main.player[i].GetModPlayer<TF2Player>().miningPower = miningPower;
            }
        }

        public static void Dialogue(string text, Color color)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(text, color);
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(25, -1, -1, NetworkText.FromLiteral(text), 255, color.R, color.G, color.B, 0, 0, 0);
        }

        public static bool IsTheSameAs(Item item, Item compareItem) => item.netID == compareItem.netID && item.type == compareItem.type;

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