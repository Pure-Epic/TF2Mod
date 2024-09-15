using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using ReLogic.Content;
using ReLogic.Graphics;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.GameInput;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.UI.Gamepad;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items;
using TF2.Content.Items.Consumables;
using TF2.Content.Items.Weapons;
using TF2.Content.Items.Weapons.Medic;
using TF2.Content.Items.Weapons.Scout;
using TF2.Content.Items.Weapons.Sniper;
using TF2.Content.Items.Weapons.Spy;
using TF2.Content.Mounts;
using TF2.Content.NPCs.Buddies;
using TF2.Content.NPCs.Buildings;
using TF2.Content.NPCs.Buildings.Dispenser;
using TF2.Content.NPCs.Buildings.SentryGun;
using TF2.Content.NPCs.Buildings.Teleporter;
using TF2.Content.NPCs.Enemies;
using TF2.Content.NPCs.TownNPCs;
using TF2.Content.Projectiles;
using TF2.Content.UI;
using TF2.Content.UI.MannCoStore;
using TF2.Content.UI.MercenaryCreationMenu;
using TF2.Gensokyo.Content.Items.BossSummons;

namespace TF2
{
    public class TF2 : Mod
    {
        internal static LocalizedText[] TF2DeathMessagesLocalization { get; private set; }

        internal static LocalizedText TF2MercenaryText { get; private set; }

        internal static Asset<Texture2D> BlankTexture { get; private set; }

        internal static Asset<Texture2D> ClassPowerIcon { get; private set; }

        public static float GlobalHealthMultiplier
        {
            get
            {
                if (NPC.downedMoonlord)
                    return 25f;
                else if (NPC.downedAncientCultist)
                    return 15f;
                else if (NPC.downedGolemBoss)
                    return 10f;
                else if (NPC.downedPlantBoss)
                    return 7.5f;
                else if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
                    return 5f;
                else if (Main.hardMode)
                    return 3.5f;
                else if (NPC.downedBoss3)
                    return 1.5f;
                else if (NPC.downedBoss2)
                    return 1f;
                else if (NPC.downedBoss1)
                    return 0.75f;
                else return 0.5f;
            }
        }

        public static float GlobalDamageMultiplier
        {
            get
            {
                if (NPC.downedMoonlord)
                    return 7.5f;
                else if (NPC.downedGolemBoss)
                    return 5f;
                else if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
                    return 4f;
                else if (Main.hardMode)
                    return 3f;
                else if (NPC.downedBoss3)
                    return 2f;
                else if (NPC.downedBoss2)
                    return 1.5f;
                else return 1f;
            }
        }

        public static float Money
        {
            get => Main.LocalPlayer.GetModPlayer<TF2Player>().money;
            set => Main.LocalPlayer.GetModPlayer<TF2Player>().money = value;
        }

        public static bool MannCoStoreActive => MannCoStore.CurrentState is MannCoStoreUI || MannCoStore.CurrentState is MannCoStoreShoppingCartUI;

        public static int MannCoStorePage
        {
            get => Main.LocalPlayer.GetModPlayer<TF2Player>().page;
            set => Main.LocalPlayer.GetModPlayer<TF2Player>().page = value;
        }

        public static List<MannCoStoreItem> ShoppingCart => Main.LocalPlayer.GetModPlayer<TF2Player>().shoppingCart;

        public static float TotalCost
        {
            get
            {
                float cost = 0f;
                List<MannCoStoreItem> shoppingCart = Main.LocalPlayer.GetModPlayer<TF2Player>().shoppingCart;
                for (int i = 0; i < shoppingCart.Count; i++)
                    cost += shoppingCart[i].Cost;
                return cost;
            }
        }

        public static IPlayerRenderer PlayerRenderer = new TF2PlayerRenderer();
        public static UserInterface MannCoStore = new UserInterface();
        private ClassIcon classUI;
        public static Mod Gensokyo;
        public static bool gensokyoLoaded;
        private static bool damageFalloff;
        public Hook ModifyMaxStats;
        public Hook PlayerModifyHitByProjectile;
        public Hook ModifyHitByProjectile;

        public delegate void ModifyMaxStatsAction(Player player);
        public delegate void PlayerModifyHitByProjectileAction(Player player, Projectile proj, ref Player.HurtModifiers modifiers);
        public delegate void ModifyHitByProjectileAction(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers);

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
            TF2DeathMessagesLocalization =
            [
                Language.GetText("Mods.TF2.DeathMessages.Bleeding"),
                Language.GetText("Mods.TF2.DeathMessages.BostonBasher"),
                Language.GetText("Mods.TF2.DeathMessages.Explosion"),
                Language.GetText("Mods.TF2.DeathMessages.Mantreads"),
                Language.GetText("Mods.TF2.DeathMessages.HalfZatoichi"),
                Language.GetText("Mods.TF2.DeathMessages.Fire"),
                Language.GetText("Mods.TF2.DeathMessages.Sapper"),
                Language.GetText("Mods.TF2.DeathMessages.Backstab")
            ];
            TF2MercenaryText = Language.GetText("Mods.TF2.UI.TF2MercenaryCreation.Mercenary");
            BlankTexture = ModContent.Request<Texture2D>("TF2/Content/Textures/Nothing");
            ClassPowerIcon = ModContent.Request<Texture2D>("TF2/Content/Textures/UI/ClassPower");
            ModifyMaxStats = new Hook(typeof(PlayerLoader).GetMethod("ModifyMaxStats", BindingFlags.Static | BindingFlags.Public), Hook_ModifyMaxStats);
            ModifyMaxStats.Apply();
            PlayerModifyHitByProjectile = new Hook(typeof(PlayerLoader).GetMethod("ModifyHitByProjectile", BindingFlags.Static | BindingFlags.Public), Hook_PlayerModifyHitByProjectile);
            PlayerModifyHitByProjectile.Apply();
            ModifyHitByProjectile = new Hook(typeof(NPCLoader).GetMethod("ModifyHitByProjectile", BindingFlags.Static | BindingFlags.Public), Hook_ModifyHitByProjectile);
            ModifyHitByProjectile.Apply();
            On_UICharacter.DrawSelf += Hook_UICharacter_DrawSelf;
            On_UICharacterListItem.ctor += Hook_UICharacterList;
            IL_UICharacterListItem.DrawSelf += Hook_UICharacterListItem_DrawSelf;
            On_UICharacterSelect.NewCharacterClick += Hook_NewCharacterClick;
            On_Player.Spawn += Hook_Spawn;
            IL_Player.Update += Hook_Update;
            On_Player.UpdateLifeRegen += Hook_UpdateLifeRegen;
            On_Player.GetWeaponDamage += Hook_GetWeaponDamage;
            On_Player.GetImmuneAlpha += Hook_GetImmuneAlpha;
            On_Player.GetImmuneAlphaPure += Hook_GetImmuneAlphaPure;
            On_Player.ApplyEquipFunctional += Hook_ApplyEquipFunctional;
            On_PlayerDrawSet.HeadOnlySetup += Hook_HeadOnlySetup;
            IL_PlayerDrawSet.BoringSetup_2 += Hook_PlayerDrawSet;
            On_ItemSlot.LeftClick_ItemArray_int_int += Hook_LeftClick;
            On_Main.GUIHotbarDrawInner += Hook_GUIHotbarDrawInner;
            IL_Main.MouseText_DrawItemTooltip += Hook_MouseText_DrawItemTooltip;
            On_Main.DrawMouseOver += Hook_DrawMouseOver;
            On_Main.HoverOverNPCs += Hook_HoverOverNPCs;
            On_NPC.HitModifiers.GetDamage += Hook_GetDamage;
            On_NPC.CalculateHitInfo += Hook_CalculateHitInfo;
            On_NPC.UpdateNPC_BuffApplyDOTs += Hook_UpdateNPC_BuffApplyDOTs;
            On_NPC.CheckLifeRegen += Hook_CheckLifeRegen;
            On_NPC.NPCLoot_DropHeals += Hook_NPCLoot_DropHeals;
            On_NPC.DoDeathEvents_DropBossPotionsAndHearts += Hook_DoDeathEvents_DropBossPotionsAndHearts;
            On_Main.DrawNPCChatButtons += Hook_DrawNPCChatButtons;
            On_Projectile.Damage += Hook_Damage;
            On_Main.DamageVar_float_float += Hook_DamageVar;
        }

        public override void Unload()
        {
            ModifyMaxStats.Undo();
            ModifyMaxStats = null;
            ModifyHitByProjectile.Undo();
            ModifyHitByProjectile = null;
            On_UICharacter.DrawSelf -= Hook_UICharacter_DrawSelf;
            On_UICharacterListItem.ctor -= Hook_UICharacterList;
            IL_UICharacterListItem.DrawSelf -= Hook_UICharacterListItem_DrawSelf;
            On_UICharacterSelect.NewCharacterClick -= Hook_NewCharacterClick;
            On_Player.Spawn -= Hook_Spawn;
            IL_Player.Update -= Hook_Update;
            On_Player.UpdateLifeRegen -= Hook_UpdateLifeRegen;
            On_Player.GetWeaponDamage -= Hook_GetWeaponDamage;
            On_Player.GetImmuneAlpha -= Hook_GetImmuneAlpha;
            On_Player.GetImmuneAlphaPure -= Hook_GetImmuneAlphaPure;
            On_Player.ApplyEquipFunctional -= Hook_ApplyEquipFunctional;
            On_PlayerDrawSet.HeadOnlySetup -= Hook_HeadOnlySetup;
            IL_PlayerDrawSet.BoringSetup_2 -= Hook_PlayerDrawSet;
            On_ItemSlot.LeftClick_ItemArray_int_int -= Hook_LeftClick;
            On_Main.GUIHotbarDrawInner -= Hook_GUIHotbarDrawInner;
            IL_Main.MouseText_DrawItemTooltip -= Hook_MouseText_DrawItemTooltip;
            On_Main.DrawMouseOver -= Hook_DrawMouseOver;
            On_Main.HoverOverNPCs -= Hook_HoverOverNPCs;
            On_NPC.HitModifiers.GetDamage -= Hook_GetDamage;
            On_NPC.CalculateHitInfo -= Hook_CalculateHitInfo;
            On_NPC.UpdateNPC_BuffApplyDOTs -= Hook_UpdateNPC_BuffApplyDOTs;
            On_NPC.CheckLifeRegen -= Hook_CheckLifeRegen;
            On_NPC.NPCLoot_DropHeals -= Hook_NPCLoot_DropHeals;
            On_NPC.DoDeathEvents_DropBossPotionsAndHearts -= Hook_DoDeathEvents_DropBossPotionsAndHearts;
            On_Main.DrawNPCChatButtons -= Hook_DrawNPCChatButtons;
            On_Projectile.Damage -= Hook_Damage;
            On_Main.DamageVar_float_float -= Hook_DamageVar;
            TF2DeathMessagesLocalization = null;
            TF2MercenaryText = null;
            BlankTexture = ClassPowerIcon = null;
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
                            if (TryFind("GensokyoDLC_StarterBox", out ModItem starterBox))
                                AddShopItem((int)args[1], true, "Consumables", starterBox.Type, 1);
                            if (!ModContent.GetInstance<TF2Config>().Shop)
                            {
                                AddShopItem((int)args[1], NPC.downedMoonlord, "Tools", ModContent.ItemType<BossRushSummon>(), 1);
                                AddShopItem((int)args[1], true, "Consumables", ItemID.LifeCrystal, 1);
                                AddShopItem((int)args[1], true, "Accessories", ItemID.TerrasparkBoots, 1);
                                AddShopItem((int)args[1], true, "Tools", ItemID.Shellphone, 1);
                                AddShopItem((int)args[1], true, "Accessories", ItemID.FeralClaws, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.GoldenDelight, 1);
                                AddShopItem((int)args[1], NPC.downedBoss3, "Placeables", ItemID.AlchemyTable, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Accessories", ItemID.AnkhShield, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Accessories", ItemID.DiscountCard, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Accessories", ItemID.GreedyRing, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Tools", ItemID.RodofDiscord, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Accessories", ItemID.CrossNecklace, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Accessories", ItemID.StarCloak, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Placeables", ItemID.SliceOfCake, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Miscellaneous", ItemID.CrystalShard, 1);
                                AddShopItem((int)args[1], NPC.downedMechBossAny, "Consumables", ItemID.LifeFruit, 1);
                                AddShopItem((int)args[1], NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3, "Accessories", ItemID.AvengerEmblem, 1);
                                AddShopItem((int)args[1], NPC.downedPlantBoss, "Placeables", ItemID.LihzahrdAltar, 1);
                                AddShopItem((int)args[1], NPC.downedPlantBoss, "Accessories", ItemID.MasterNinjaGear, 1);
                                AddShopItem((int)args[1], NPC.downedGolemBoss, "Accessories", ItemID.DestroyerEmblem, 1);
                                AddShopItem((int)args[1], NPC.downedGolemBoss, "Tools", ItemID.Picksaw, 1);
                                AddShopItem((int)args[1], NPC.downedBoss3, "Miscellaneous", ItemID.GoldenKey, 1);
                                AddShopItem((int)args[1], NPC.downedBoss3, "Miscellaneous", ItemID.ShadowKey, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Miscellaneous", ItemID.JungleKey, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Miscellaneous", ItemID.CorruptionKey, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Miscellaneous", ItemID.CrimsonKey, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Miscellaneous", ItemID.HallowedKey, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Miscellaneous", ItemID.FrozenKey, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Miscellaneous", ItemID.DungeonDesertKey, 1);
                                AddShopItem((int)args[1], true, "Miscellaneous", ItemID.CopperOre, 1);
                                AddShopItem((int)args[1], true, "Miscellaneous", ItemID.TinOre, 1);
                                AddShopItem((int)args[1], true, "Miscellaneous", ItemID.IronOre, 1);
                                AddShopItem((int)args[1], true, "Miscellaneous", ItemID.LeadOre, 1);
                                AddShopItem((int)args[1], true, "Miscellaneous", ItemID.SilverOre, 1);
                                AddShopItem((int)args[1], true, "Miscellaneous", ItemID.TungstenOre, 1);
                                AddShopItem((int)args[1], true, "Miscellaneous", ItemID.GoldOre, 1);
                                AddShopItem((int)args[1], true, "Miscellaneous", ItemID.PlatinumOre, 1);
                                AddShopItem((int)args[1], true, "Miscellaneous", ItemID.DemoniteOre, 1);
                                AddShopItem((int)args[1], true, "Miscellaneous", ItemID.CrimtaneOre, 1);
                                AddShopItem((int)args[1], NPC.downedBoss1, "Miscellaneous", ItemID.Meteorite, 1);
                                AddShopItem((int)args[1], true, "Miscellaneous", ItemID.Obsidian, 1);
                                AddShopItem((int)args[1], NPC.downedBoss2, "Miscellaneous", ItemID.Hellstone, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Miscellaneous", ItemID.CobaltOre, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Miscellaneous", ItemID.PalladiumOre, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Miscellaneous", ItemID.MythrilOre, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Miscellaneous", ItemID.OrichalcumOre, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Miscellaneous", ItemID.AdamantiteOre, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Miscellaneous", ItemID.TitaniumOre, 1);
                                AddShopItem((int)args[1], NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3, "Miscellaneous", ItemID.ChlorophyteOre, 1);
                                AddShopItem((int)args[1], NPC.downedMoonlord, "Miscellaneous", ItemID.LunarOre, 1);
                                AddShopItem((int)args[1], true, "Miscellaneous", ItemID.Amethyst, 1);
                                AddShopItem((int)args[1], true, "Miscellaneous", ItemID.Topaz, 1);
                                AddShopItem((int)args[1], true, "Miscellaneous", ItemID.Sapphire, 1);
                                AddShopItem((int)args[1], true, "Miscellaneous", ItemID.Emerald, 1);
                                AddShopItem((int)args[1], true, "Miscellaneous", ItemID.Ruby, 1);
                                AddShopItem((int)args[1], true, "Miscellaneous", ItemID.Amber, 1);
                                AddShopItem((int)args[1], true, "Miscellaneous", ItemID.Diamond, 1);
                                AddShopItem((int)args[1], true, "Miscellaneous", ItemID.CopperBar, 1);
                                AddShopItem((int)args[1], true, "Miscellaneous", ItemID.TinBar, 1);
                                AddShopItem((int)args[1], true, "Miscellaneous", ItemID.IronBar, 1);
                                AddShopItem((int)args[1], true, "Miscellaneous", ItemID.LeadBar, 1);
                                AddShopItem((int)args[1], true, "Miscellaneous", ItemID.SilverBar, 1);
                                AddShopItem((int)args[1], true, "Miscellaneous", ItemID.TungstenBar, 1);
                                AddShopItem((int)args[1], true, "Miscellaneous", ItemID.GoldBar, 1);
                                AddShopItem((int)args[1], true, "Miscellaneous", ItemID.PlatinumBar, 1);
                                AddShopItem((int)args[1], true, "Miscellaneous", ItemID.DemoniteBar, 1);
                                AddShopItem((int)args[1], true, "Miscellaneous", ItemID.CrimtaneBar, 1);
                                AddShopItem((int)args[1], NPC.downedBoss1, "Miscellaneous", ItemID.MeteoriteBar, 1);
                                AddShopItem((int)args[1], NPC.downedBoss2, "Miscellaneous", ItemID.HellstoneBar, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Miscellaneous", ItemID.CobaltBar, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Miscellaneous", ItemID.PalladiumBar, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Miscellaneous", ItemID.MythrilBar, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Miscellaneous", ItemID.OrichalcumBar, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Miscellaneous", ItemID.AdamantiteBar, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Miscellaneous", ItemID.TitaniumBar, 1);
                                AddShopItem((int)args[1], NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3, "Miscellaneous", ItemID.HallowedBar, 1);
                                AddShopItem((int)args[1], NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3, "Miscellaneous", ItemID.ChlorophyteBar, 1);
                                AddShopItem((int)args[1], NPC.downedPlantBoss, "Miscellaneous", ItemID.ShroomiteBar, 1);
                                AddShopItem((int)args[1], NPC.downedPlantBoss, "Miscellaneous", ItemID.SpectreBar, 1);
                                AddShopItem((int)args[1], NPC.downedMoonlord, "Miscellaneous", ItemID.LunarBar, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.BottledWater, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.LesserHealingPotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.HealingPotion, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Potions", ItemID.GreaterHealingPotion, 1);
                                AddShopItem((int)args[1], NPC.downedAncientCultist, "Potions", ItemID.SuperHealingPotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.LesserManaPotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.ManaPotion, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Potions", ItemID.GreaterManaPotion, 1);
                                AddShopItem((int)args[1], NPC.downedAncientCultist, "Potions", ItemID.SuperManaPotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.LuckPotionLesser, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.LuckPotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.LuckPotionGreater, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.AmmoReservationPotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.ArcheryPotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.BattlePotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.BuilderPotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.CalmingPotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.CratePotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.TrapsightPotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.EndurancePotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.FeatherfallPotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.FishingPotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.FlipperPotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.GenderChangePotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.GillsPotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.GravitationPotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.HeartreachPotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.HunterPotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.InfernoPotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.InvisibilityPotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.IronskinPotion, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Potions", ItemID.LifeforcePotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.MagicPowerPotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.ManaRegenerationPotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.MiningPotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.NightOwlPotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.ObsidianSkinPotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.RagePotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.RecallPotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.RegenerationPotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.ShinePotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.SonarPotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.SpelunkerPotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.SummoningPotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.SwiftnessPotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.TeleportationPotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.ThornsPotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.TitanPotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.WarmthPotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.WaterWalkingPotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.WormholePotion, 1);
                                AddShopItem((int)args[1], true, "Potions", ItemID.WrathPotion, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Miscellaneous", ItemID.CursedFlame, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Miscellaneous", ItemID.Ichor, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Miscellaneous", ItemID.SoulofLight, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Miscellaneous", ItemID.SoulofNight, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Miscellaneous", ItemID.SoulofFlight, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Miscellaneous", ItemID.SoulofMight, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Miscellaneous", ItemID.SoulofSight, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Miscellaneous", ItemID.SoulofFright, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Miscellaneous", ItemID.DarkShard, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Miscellaneous", ItemID.LightShard, 1);
                                AddShopItem((int)args[1], true, "Weapons", ItemID.CopperShortsword, 1);
                                AddShopItem((int)args[1], true, "Weapons", ItemID.Starfury, 1);
                                AddShopItem((int)args[1], true, "Weapons", ItemID.EnchantedSword, 1);
                                AddShopItem((int)args[1], true, "Weapons", ItemID.BladeofGrass, 1);
                                AddShopItem((int)args[1], NPC.downedQueenBee || NPC.downedBoss3, "Weapons", ItemID.BeeKeeper, 1);
                                AddShopItem((int)args[1], NPC.downedBoss3, "Weapons", ItemID.Muramasa, 1);
                                AddShopItem((int)args[1], NPC.downedPlantBoss, "Weapons", ItemID.Seedler, 1);
                                AddShopItem((int)args[1], NPC.downedPlantBoss, "Weapons", ItemID.TheHorsemansBlade, 1);
                                AddShopItem((int)args[1], NPC.downedGolemBoss, "Weapons", ItemID.InfluxWaver, 1);
                                AddShopItem((int)args[1], NPC.downedMoonlord, "Weapons", ItemID.Meowmere, 1);
                                AddShopItem((int)args[1], NPC.downedMoonlord, "Weapons", ItemID.StarWrath, 1);
                                AddShopItem((int)args[1], NPC.downedPlantBoss, "Miscellaneous", ItemID.BrokenHeroSword, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Weapons", ItemID.ShadowFlameKnife, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Weapons", ItemID.DripplerFlail, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Accessories", ItemID.YoyoBag, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Weapons", ItemID.BouncingShield, 1);
                                AddShopItem((int)args[1], NPC.downedFishron, "Weapons", ItemID.Flairon, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Weapons", ItemID.BerserkerGlove, 1);
                                AddShopItem((int)args[1], NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3, "Miscellaneous", ItemID.TurtleShell, 1);
                                AddShopItem((int)args[1], NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3, "Accessories", ItemID.FireGauntlet, 1);
                                AddShopItem((int)args[1], NPC.downedGolemBoss, "Weapons", ItemID.BeetleHusk, 1);
                                AddShopItem((int)args[1], NPC.downedGolemBoss, "Weapons", ItemID.CelestialShell, 1);
                                AddShopItem((int)args[1], true, "Placeables", ItemID.SharpeningStation, 1);
                                AddShopItem((int)args[1], NPC.downedAncientCultist, "Miscellaneous", ItemID.FragmentSolar, 1);
                                AddShopItem((int)args[1], NPC.downedBoss3, "Weapons", ItemID.PhoenixBlaster, 1);
                                AddShopItem((int)args[1], NPC.downedBoss3, "Weapons", ItemID.ZapinatorGray, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Weapons", ItemID.ZapinatorOrange, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Weapons", ItemID.DaedalusStormbow, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Weapons", ItemID.Megashark, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Weapons", ItemID.Uzi, 1);
                                AddShopItem((int)args[1], NPC.downedPlantBoss, "Weapons", ItemID.TacticalShotgun, 1);
                                AddShopItem((int)args[1], NPC.downedFishron, "Weapons", ItemID.Tsunami, 1);
                                AddShopItem((int)args[1], NPC.downedMoonlord, "Weapons", ItemID.SDMG, 1);
                                AddShopItem((int)args[1], NPC.downedMoonlord, "Weapons", ItemID.Celeb2, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Accessories", ItemID.MoltenQuiver, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Accessories", ItemID.StalkersQuiver, 1);
                                AddShopItem((int)args[1], NPC.downedGolemBoss, "Accessories", ItemID.ReconScope, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Miscellaneous", ItemID.EndlessQuiver, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Miscellaneous", ItemID.EndlessMusketPouch, 1);
                                AddShopItem((int)args[1], true, "Placeables", ItemID.AmmoBox, 1);
                                AddShopItem((int)args[1], NPC.downedAncientCultist, "Miscellaneous", ItemID.FragmentVortex, 1);
                                AddShopItem((int)args[1], true, "Consumables", ItemID.ManaCrystal, 1);
                                AddShopItem((int)args[1], true, "Weapons", ItemID.DemonScythe, 1);
                                AddShopItem((int)args[1], NPC.downedBoss3, "Weapons", ItemID.WaterBolt, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Weapons", ItemID.SkyFracture, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Weapons", ItemID.MeteorStaff, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Weapons", ItemID.CrystalSerpent, 1);
                                AddShopItem((int)args[1], NPC.downedMechBossAny, "Weapons", ItemID.UnholyTrident, 1);
                                AddShopItem((int)args[1], NPC.downedPlantBoss, "Weapons", ItemID.ShadowbeamStaff, 1);
                                AddShopItem((int)args[1], NPC.downedPlantBoss, "Weapons", ItemID.InfernoFork, 1);
                                AddShopItem((int)args[1], NPC.downedPlantBoss, "Weapons", ItemID.SpectreStaff, 1);
                                AddShopItem((int)args[1], NPC.downedPlantBoss, "Weapons", ItemID.Razorpine, 1);
                                AddShopItem((int)args[1], NPC.downedFishron, "Weapons", ItemID.RazorbladeTyphoon, 1);
                                AddShopItem((int)args[1], NPC.downedMoonlord, "Weapons", ItemID.LastPrism, 1);
                                AddShopItem((int)args[1], NPC.downedMoonlord, "Weapons", ItemID.LunarFlareBook, 1);
                                AddShopItem((int)args[1], true, "Accessories", ItemID.ManaFlower, 1);
                                AddShopItem((int)args[1], true, "Accessories", ItemID.CelestialMagnet, 1);
                                AddShopItem((int)args[1], true, "Accessories", ItemID.MagicCuffs, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Placeables", ItemID.CrystalBall, 1);
                                AddShopItem((int)args[1], NPC.downedPlantBoss, "Miscellaneous", ItemID.Ectoplasm, 1);
                                AddShopItem((int)args[1], NPC.downedAncientCultist, "Miscellaneous", ItemID.FragmentNebula, 1);
                                AddShopItem((int)args[1], true, "Weapons", ItemID.SlimeStaff, 1);
                                AddShopItem((int)args[1], true, "Weapons", ItemID.FlinxStaff, 1);
                                AddShopItem((int)args[1], true, "Weapons", ItemID.VampireFrogStaff, 1);
                                AddShopItem((int)args[1], true, "Weapons", ItemID.AbigailsFlower, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Weapons", ItemID.SanguineStaff, 1);
                                AddShopItem((int)args[1], NPC.downedQueenSlime, "Weapons", ItemID.Smolstar, 1);
                                AddShopItem((int)args[1], NPC.downedMechBoss2, "Weapons", ItemID.OpticStaff, 1);
                                AddShopItem((int)args[1], NPC.downedPlantBoss, "Weapons", ItemID.PygmyStaff, 1);
                                AddShopItem((int)args[1], NPC.downedEmpressOfLight, "Weapons", ItemID.EmpressBlade, 1);
                                AddShopItem((int)args[1], NPC.downedMoonlord, "Weapons", ItemID.MoonlordTurretStaff, 1);
                                AddShopItem((int)args[1], NPC.downedMoonlord, "Weapons", ItemID.RainbowCrystalStaff, 1);
                                AddShopItem((int)args[1], true, "Weapons", ItemID.ThornWhip, 1);
                                AddShopItem((int)args[1], NPC.downedBoss3, "Weapons", ItemID.BoneWhip, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Weapons", ItemID.FireWhip, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Weapons", ItemID.CoolWhip, 1);
                                AddShopItem((int)args[1], NPC.downedMechBossAny, "Weapons", ItemID.SwordWhip, 1);
                                AddShopItem((int)args[1], NPC.downedPlantBoss, "Weapons", ItemID.ScytheWhip, 1);
                                AddShopItem((int)args[1], NPC.downedPlantBoss, "Weapons", ItemID.MaceWhip, 1);
                                AddShopItem((int)args[1], NPC.downedEmpressOfLight, "Weapons", ItemID.RainbowWhip, 1);
                                AddShopItem((int)args[1], true, "Accessories", ItemID.PygmyNecklace, 1);
                                AddShopItem((int)args[1], NPC.downedPlantBoss, "Accessories", ItemID.NecromanticScroll, 1);
                                AddShopItem((int)args[1], NPC.downedPlantBoss, "Accessories", ItemID.PapyrusScarab, 1);
                                AddShopItem((int)args[1], NPC.downedBoss3, "Placeables", ItemID.BewitchingTable, 1);
                                AddShopItem((int)args[1], NPC.downedAncientCultist, "Miscellaneous", ItemID.FragmentStardust, 1);
                                AddShopItem((int)args[1], true, "Consumables", ItemID.SlimeCrown, 1);
                                AddShopItem((int)args[1], NPC.downedBoss1, "Consumables", ItemID.SuspiciousLookingEye, 1);
                                AddShopItem((int)args[1], NPC.downedBoss1, "Consumables", ItemID.WormFood, 1);
                                AddShopItem((int)args[1], NPC.downedBoss1, "Consumables", ItemID.BloodySpine, 1);
                                AddShopItem((int)args[1], NPC.downedBoss1, "Consumables", ItemID.Abeemination, 1);
                                AddShopItem((int)args[1], NPC.downedBoss2, "Consumables", ItemID.ClothierVoodooDoll, 1);
                                AddShopItem((int)args[1], NPC.downedBoss2, "Consumables", ItemID.DeerThing, 1);
                                AddShopItem((int)args[1], NPC.downedBoss3, "Consumables", ItemID.GuideVoodooDoll, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Consumables", ItemID.QueenSlimeCrystal, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Consumables", ItemID.MechanicalWorm, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Consumables", ItemID.MechanicalEye, 1);
                                AddShopItem((int)args[1], Main.hardMode, "Consumables", ItemID.MechanicalSkull, 1);
                                AddShopItem((int)args[1], NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3, "Consumables", ModContent.ItemType<PlanteraItem>(), 1);
                                AddShopItem((int)args[1], NPC.downedPlantBoss, "Consumables", ItemID.LihzahrdPowerCell, 1);
                                AddShopItem((int)args[1], NPC.downedPlantBoss, "Consumables", ItemID.EmpressButterfly, 1);
                                AddShopItem((int)args[1], NPC.downedPlantBoss, "Consumables", ItemID.TruffleWorm, 1);
                                AddShopItem((int)args[1], NPC.downedAncientCultist, "Consumables", ItemID.CelestialSigil, 1);
                            }
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
                    Player player = Main.player[reader.ReadByte()];
                    TF2Player p = player.GetModPlayer<TF2Player>();
                    p.currentClass = reader.ReadByte();
                    p.healthBonus = reader.ReadInt32();
                    p.healthMultiplier = reader.ReadSingle();
                    p.healReduction = reader.ReadSingle();
                    p.overhealMultiplier = reader.ReadSingle();
                    player.itemAnimation = reader.ReadInt32();
                    player.itemAnimationMax = reader.ReadInt32();
                    p.overheal = reader.ReadInt32();
                    p.focus = reader.ReadBoolean();
                    p.healCooldown = reader.ReadInt32();
                    p.healPenalty = reader.ReadInt32();
                    for (int buddy = 0; buddy < p.buddies.Length; buddy++)
                        p.buddies[buddy] = reader.ReadInt32();
                    for (int buddyCooldown = 0; buddyCooldown < p.buddyCooldown.Length; buddyCooldown++)
                        p.buddyCooldown[buddyCooldown] = reader.ReadInt32();
                    if (Main.dedServ)
                        p.SyncPlayer(-1, whoAmI, false);
                    break;
                case MessageType.SyncMount:
                    player = Main.player[reader.ReadByte()];
                    player.Center = reader.ReadVector2();
                    player.velocity = Vector2.Zero;
                    player.direction = reader.ReadInt32();
                    if (Main.dedServ)
                        TF2Mount.SendMountMessage(player);
                    break;
                case MessageType.SyncSound:
                    SoundEngine.PlaySound(new SoundStyle(reader.ReadString()), reader.ReadVector2());
                    break;
                case MessageType.SyncBuilding:
                    Building building = Main.npc[reader.ReadByte()].ModNPC as Building;
                    building.Timer = reader.ReadInt32();
                    building.Metal = reader.ReadInt32();
                    building.UpgradeCooldown = reader.ReadSingle();
                    building.Timer2 = reader.ReadInt32();
                    building.preConstructedDamage = reader.ReadInt32();
                    building.constructionSpeed = reader.ReadSingle();
                    if (Main.dedServ)
                        building.SyncBuilding();
                    break;
                case MessageType.ConstructBuilding:
                    NPC npc = Main.npc[reader.ReadByte()];
                    building = npc.ModNPC as Building;
                    player = Main.player[building.Owner];
                    if (building is TF2Sentry)
                        building.air = reader.ReadBoolean();
                    if (building is TF2Sentry)
                        player.GetModPlayer<TF2Player>().sentryWhoAmI = npc.whoAmI;
                    else if (building is TF2Dispenser)
                        player.GetModPlayer<TF2Player>().dispenserWhoAmI = npc.whoAmI;
                    else if (building is TeleporterEntrance)
                        player.GetModPlayer<TF2Player>().teleporterEntranceWhoAmI = npc.whoAmI;
                    else if (building is TeleporterExit)
                        player.GetModPlayer<TF2Player>().teleporterExitWhoAmI = npc.whoAmI;
                    break;
                case MessageType.RepairBuilding:
                    npc = Main.npc[reader.ReadByte()];
                    int healAmount = reader.ReadInt32();
                    npc.life += healAmount;
                    if (Main.dedServ)
                    {
                        npc.HealEffect(healAmount);
                        (npc.ModNPC as Building).Repair(healAmount);
                    }
                    break;
                case MessageType.SyncSentry:
                    TF2Sentry sentry = Main.npc[reader.ReadByte()].ModNPC as TF2Sentry;
                    sentry.Metal = reader.ReadInt32();
                    sentry.Ammo = reader.ReadInt32();
                    sentry.RocketAmmo = reader.ReadInt32();
                    sentry.air = reader.ReadBoolean();
                    sentry.UpgradeCooldown = sentry.BuildingCooldownHauled;
                    sentry.hauled = true;
                    if (Main.dedServ)
                        sentry.HaulSentry(sentry.Metal, sentry.Ammo, sentry.RocketAmmo, sentry.air);
                    break;
                case MessageType.SyncSentryAmmo:
                    sentry = Main.npc[reader.ReadByte()].ModNPC as TF2Sentry;
                    int amount = reader.ReadInt32();
                    sentry.Ammo += amount;
                    if (Main.dedServ)
                        sentry.AddSentryAmmo(amount);
                    break;
                case MessageType.SyncSentryRocketAmmo:
                    sentry = Main.npc[reader.ReadByte()].ModNPC as TF2Sentry;
                    amount = reader.ReadInt32();
                    sentry.RocketAmmo += amount;
                    if (Main.dedServ)
                        sentry.AddSentryRocketAmmo(amount);
                    break;
                case MessageType.SyncDispenser:
                    TF2Dispenser dispenser = Main.npc[reader.ReadByte()].ModNPC as TF2Dispenser;
                    dispenser.Metal = reader.ReadInt32();
                    dispenser.ReservedMetal = reader.ReadInt32();
                    dispenser.UpgradeCooldown = dispenser.BuildingCooldownHauled;
                    dispenser.hauled = true;
                    if (Main.dedServ)
                        dispenser.HaulDispenser(dispenser.Metal, dispenser.ReservedMetal);
                    break;
                case MessageType.SyncReservedMetal:
                    dispenser = Main.npc[reader.ReadByte()].ModNPC as TF2Dispenser;
                    int metalCost = reader.ReadInt32();
                    dispenser.ReservedMetal -= metalCost;
                    if (Main.dedServ)
                        dispenser.SyncReservedMetal(metalCost);
                    break;
                case MessageType.SyncTeleporter:
                    TF2Teleporter teleporter = Main.npc[reader.ReadByte()].ModNPC as TF2Teleporter;
                    teleporter.Metal = reader.ReadInt32();
                    teleporter.UpgradeCooldown = teleporter.BuildingCooldownHauled;
                    teleporter.hauled = true;
                    if (Main.dedServ)
                        teleporter.HaulTeleporter(teleporter.Metal);
                    break;
                case MessageType.SyncSyringe:
                    byte i = reader.ReadByte();
                    NetMessage.SendData(MessageID.SpiritHeal, number: reader.ReadByte(), number2: reader.ReadInt32());
                    Main.projectile[i].Kill();
                    break;
                case MessageType.SpawnNPC:
                    player = Main.player[reader.ReadByte()];
                    npc = OverwriteNPCDirect(player.GetSource_FromThis(), (int)player.Center.X, (int)player.Center.Y, reader.ReadInt32(), reader.ReadByte(), 0, 0, 0, player.whoAmI);
                    npc.netUpdate = true;
                    break;
                case MessageType.Overheal:
                    i = reader.ReadByte();
                    healAmount = reader.ReadInt32();
                    float limit = reader.ReadSingle();
                    if (healAmount > 0)
                    {
                        player = Main.player[i];
                        p = player.GetModPlayer<TF2Player>();
                        int maxHealth = TF2Player.TotalHealth(player);
                        if (p.overheal >= OverhealRound(maxHealth * limit * p.overhealMultiplier)) return;
                        player.statLife += healAmount;
                        if (player.statLife > maxHealth)
                        {
                            int extraHealth = player.statLife - maxHealth - p.overheal;
                            p.overheal += extraHealth;
                            Maximum(ref p.overheal, OverhealRound(maxHealth * limit * p.overhealMultiplier));
                            player.statLife = Round((p.BaseHealth + p.healthBonus) * p.healthMultiplier + p.overheal);
                        }
                        player.HealEffect(healAmount);
                        if (Main.dedServ)
                            OverhealMultiplayer(player, healAmount, limit);
                    }
                    break;
                case MessageType.OverhealNPC:
                    i = reader.ReadByte();
                    healAmount = reader.ReadInt32();
                    limit = reader.ReadSingle();
                    if (healAmount > 0)
                    {
                        NPC target = Main.npc[i];
                        MercenaryBuddy buddy = target.ModNPC as MercenaryBuddy;
                        int maxHealth = buddy.finalBaseHealth;
                        if (buddy.overheal >= OverhealRound(maxHealth * limit)) return;
                        target.life += healAmount;
                        if (target.life > maxHealth)
                        {
                            int extraHealth = target.life - maxHealth - buddy.overheal;
                            buddy.overheal += extraHealth;
                            Maximum(ref buddy.overheal, OverhealRound(maxHealth * limit));
                            target.life = Round(buddy.finalBaseHealth + buddy.overheal);
                        }
                        target.HealEffect(healAmount);
                        if (Main.dedServ)
                            OverhealNPCMultiplayer(target, healAmount, limit);
                    }
                    break;
                case MessageType.KillNPC:
                    Main.npc[reader.ReadByte()].life = 0;
                    break;
                case MessageType.KillProjectile:
                    Main.projectile[reader.ReadByte()].Kill();
                    break;
                case MessageType.DespawnHeavy:
                    npc = Main.npc[reader.ReadByte()];
                    if (npc.ModNPC is HeavyNPC heavy)
                    {
                        if (SoundEngine.TryGetActiveSound(heavy.minigunSpinUpSoundSlot, out var spinUp))
                            spinUp.Stop();
                        if (SoundEngine.TryGetActiveSound(heavy.minigunSpinDownSoundSlot, out var spinDown))
                            spinDown.Stop();
                        if (SoundEngine.TryGetActiveSound(heavy.minigunSpinSoundSlot, out var spinSound))
                            spinSound.Stop();
                        if (SoundEngine.TryGetActiveSound(heavy.minigunAttackSoundSlot, out var attackSound))
                            attackSound.Stop();
                    }
                    else if (npc.ModNPC is EnemyHeavyNPC enemyHeavy)
                    {
                        if (SoundEngine.TryGetActiveSound(enemyHeavy.minigunSpinUpSoundSlot, out var spinUp))
                            spinUp.Stop();
                        if (SoundEngine.TryGetActiveSound(enemyHeavy.minigunSpinDownSoundSlot, out var spinDown))
                            spinDown.Stop();
                        if (SoundEngine.TryGetActiveSound(enemyHeavy.minigunSpinSoundSlot, out var spinSound))
                            spinSound.Stop();
                        if (SoundEngine.TryGetActiveSound(enemyHeavy.minigunAttackSoundSlot, out var attackSound))
                            attackSound.Stop();
                    }
                    break;
                case MessageType.FeignDeath:
                    npc = Main.npc[reader.ReadByte()];
                    (npc.ModNPC as SpyNPC).temporaryBuddy = true;
                    if (Main.dedServ)
                        SetFeignDeathSpy(npc);
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
            SyncSound,
            SyncBuilding,
            ConstructBuilding,
            RepairBuilding,
            SyncSentry,
            SyncSentryAmmo,
            SyncSentryRocketAmmo,
            SyncDispenser,
            SyncReservedMetal,
            SyncTeleporter,
            SyncSyringe,
            SpawnNPC,
            Overheal,
            OverhealNPC,
            KillNPC,
            KillProjectile,
            DespawnHeavy,
            FeignDeath,
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

        private static void Hook_PlayerModifyHitByProjectile(PlayerModifyHitByProjectileAction orig, Player player, Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (proj.ModProjectile is not SuperBullet)
                orig(player, proj, ref modifiers);
        }

        private static void Hook_ModifyHitByProjectile(ModifyHitByProjectileAction orig, NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (projectile.ModProjectile is SuperBullet bullet && modifiers.GetDamage(projectile.damage, bullet.crit) <= projectile.damage)
            {
                projectile.TryGetOwner(out var player);
                if (!bullet.noDistanceModifier)
                    bullet.weapon?.WeaponDistanceModifier(player, projectile, npc, ref modifiers);
            }
            else orig(npc, projectile, ref modifiers);
        }

        private void Hook_UICharacter_DrawSelf(On_UICharacter.orig_DrawSelf orig, UICharacter self, SpriteBatch spriteBatch)
        {
            var playerField = typeof(UICharacter).GetField("_player", BindingFlags.Instance | BindingFlags.NonPublic);
            var _player = (Player)playerField.GetValue(self);
            if (_player.GetModPlayer<TF2Player>().ClassSelected) return;
            orig(self, spriteBatch);
        }

        private void Hook_UICharacterList(On_UICharacterListItem.orig_ctor orig, UICharacterListItem self, PlayerFileData data, int snapPointIndex)
        {
            orig(self, data, snapPointIndex);
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
                        c.EmitDelegate<Func<Player, string>>((player) => "x" + player.GetModPlayer<TF2Player>().damageMultiplier.ToString());
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
            p.activateUberCharge = false;
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
            p.buddyCooldown = [0, 0, 0];
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
                c.EmitDelegate(static (Player self) => TF2Weapon.CanSwitchWeapon(self) && !MannCoStoreActive && TF2Player.CanSwitchWeaponPDA(self));
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
                    c.EmitDelegate(static (Player self) => TF2Weapon.CanSwitchWeapon(self) && !MannCoStoreActive && TF2Player.CanSwitchWeaponPDA(self));
                    c.Emit(OpCodes.Brfalse, LabelKey);
                }
            }
        }

        private void Hook_UpdateLifeRegen(On_Player.orig_UpdateLifeRegen orig, Player self)
        {
            TF2Player p = self.GetModPlayer<TF2Player>();
            if (p.ClassSelected)
            {
                orig(self);
                Maximum(ref self.lifeRegen, 0);
            }
            else orig(self);
        }

        private int Hook_GetWeaponDamage(On_Player.orig_GetWeaponDamage orig, Player self, Item sItem, bool forTooltip)
        {
            StatModifier modifier = self.GetTotalDamage(sItem.DamageType);
            CombinedHooks.ModifyWeaponDamage(self, sItem, ref modifier);
            return self.GetModPlayer<TF2Player>().ClassSelected ? Math.Max(0, Round(modifier.ApplyTo(sItem.damage))) : orig(self, sItem, forTooltip);
        }

        private Color Hook_GetImmuneAlpha(On_Player.orig_GetImmuneAlpha orig, Player self, Color newColor, float alphaReduction) => self.GetModPlayer<TF2Player>().ClassSelected ? newColor : orig(self, newColor, alphaReduction);

        private Color Hook_GetImmuneAlphaPure(On_Player.orig_GetImmuneAlphaPure orig, Player self, Color newColor, float alphaReduction) => self.GetModPlayer<TF2Player>().ClassSelected ? newColor : orig(self, newColor, alphaReduction);

        private void Hook_ApplyEquipFunctional(On_Player.orig_ApplyEquipFunctional orig, Player self, Item currentItem, bool hideVisual)
        {
            TF2Player p = self.GetModPlayer<TF2Player>();
            if (p.currentClass == TF2Item.Scout)
            {
                p.extraJumps = 1;
                if (self.GetModPlayer<SodaPopperPlayer>().buffActive)
                    self.GetModPlayer<TF2Player>().extraJumps = 5;
                else if (self.HeldItem.ModItem is Atomizer weapon && weapon.deployTimer >= weapon.deploySpeed)
                    self.GetModPlayer<TF2Player>().extraJumps = 2;
                self.GetJumpState<ScoutDoubleJump>().Enable();
            }
            orig(self, currentItem, hideVisual);
        }

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
            if (MannCoStoreActive) return;
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
                for (int i = 0; i < Main.maxItems; i++)
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
            for (int j = 0; j < Main.maxPlayers; j++)
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
                hoverOverNPCs.Invoke(self, [mouseRectangle]);
            if (!Main.mouseText && Main.signHover > -1 && Main.sign[Main.signHover] != null && !Main.LocalPlayer.mouseInterface && !string.IsNullOrWhiteSpace(Main.sign[Main.signHover].text))
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
                    Utils.DrawInvBG(Main.spriteBatch, new Rectangle((int)vector.X - num7, (int)vector.Y - num8, (int)num6 + num7 * 2, 30 * lineAmount + num8 + num8 / 2), new Color(23, 25, 81) * 0.925f * 0.85f);
                }
                for (int l = 0; l < lineAmount; l++)
                    Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, array[l], vector.X, vector.Y + l * 30, color, Color.Black, Vector2.Zero);
                Main.mouseText = true;
            }
            PlayerInput.SetZoom_UI();
        }

        private void Hook_HoverOverNPCs(On_Main.orig_HoverOverNPCs orig, Main self, Rectangle mouseRectangle)
        {
            MethodInfo tryFreeingElderSlime = typeof(Main).GetMethod("TryFreeingElderSlime", BindingFlags.Static | BindingFlags.NonPublic);
            Player player = Main.LocalPlayer;
            Rectangle value;
            Rectangle rectangle;
            Rectangle value2;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.ShowNameOnHover || !(npc.active & (npc.shimmerTransparency == 0f || npc.CanApplyHunterPotionEffects())))
                    continue;
                int type = npc.type;
                self.LoadNPC(type);
                npc.position += npc.netOffset;
                value = new Rectangle((int)npc.Bottom.X - npc.frame.Width / 2, (int)npc.Bottom.Y - npc.frame.Height, npc.frame.Width, npc.frame.Height);
                if (npc.type >= NPCID.WyvernHead && npc.type <= NPCID.WyvernTail)
                    value = new Rectangle((int)(npc.position.X + npc.width * 0.5 - 32.0), (int)(npc.position.Y + npc.height * 0.5 - 32.0), 64, 64);
                NPCLoader.ModifyHoverBoundingBox(npc, ref value);
                bool flag = mouseRectangle.Intersects(value);
                bool flag2 = flag || (Main.SmartInteractShowingGenuine && Main.SmartInteractNPC == i);
                if (flag2 && ((npc.type != NPCID.Mimic && npc.type != NPCID.PresentMimic && npc.type != NPCID.IceMimic && npc.aiStyle != 87) || npc.ai[0] != 0f) && npc.type != NPCID.TargetDummy)
                {
                    if (npc.type == NPCID.BoundTownSlimeOld)
                    {
                        player.cursorItemIconEnabled = true;
                        player.cursorItemIconID = 327;
                        player.cursorItemIconText = "";
                        player.noThrow = 2;
                        if (!player.dead)
                        {
                            PlayerInput.SetZoom_MouseInWorld();
                            if (Main.mouseRight && Main.npcChatRelease)
                            {
                                Main.npcChatRelease = false;
                                if (PlayerInput.UsingGamepad)
                                    player.releaseInventory = false;
                                if (player.talkNPC != i && !player.tileInteractionHappened && (bool)tryFreeingElderSlime.Invoke(self, [i]))
                                {
                                    NPC.TransformElderSlime(i);
                                    SoundEngine.PlaySound(SoundID.Unlock);
                                }
                            }
                        }
                    }
                    else
                    {
                        bool flag3 = Main.SmartInteractShowingGenuine && Main.SmartInteractNPC == i;
                        bool vanillaCanChat = false;
                        if (npc.townNPC || npc.type == NPCID.BoundGoblin || npc.type == NPCID.BoundWizard || npc.type == NPCID.BoundMechanic || npc.type == NPCID.WebbedStylist || npc.type == NPCID.SleepingAngler || npc.type == NPCID.BartenderUnconscious || npc.type == NPCID.SkeletonMerchant || npc.type == NPCID.GolferRescue)
                            vanillaCanChat = true;
                        if (NPCLoader.CanChat(npc) ?? vanillaCanChat)
                        {
                            rectangle = new Rectangle((int)(player.position.X + player.width / 2 - Player.tileRangeX * 16), (int)(player.position.Y + player.height / 2 - Player.tileRangeY * 16), Player.tileRangeX * 16 * 2, Player.tileRangeY * 16 * 2);
                            value2 = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
                            if (rectangle.Intersects(value2))
                                flag3 = true;
                        }
                        if (player.ownedProjectileCounts[651] > 0)
                            flag3 = false;
                        if (flag3 && !player.dead)
                        {
                            PlayerInput.SetZoom_MouseInWorld();
                            Main.HoveringOverAnNPC = true;
                            self.currentNPCShowingChatBubble = i;
                            if (Main.mouseRight && Main.npcChatRelease)
                            {
                                Main.npcChatRelease = false;
                                if (PlayerInput.UsingGamepad)
                                    player.releaseInventory = false;
                                if (player.talkNPC != i && !player.tileInteractionHappened)
                                {
                                    Main.CancelHairWindow();
                                    Main.SetNPCShopIndex(0);
                                    Main.InGuideCraftMenu = false;
                                    player.dropItemCheck();
                                    Main.npcChatCornerItem = 0;
                                    player.sign = -1;
                                    Main.editSign = false;
                                    player.SetTalkNPC(i);
                                    Main.playerInventory = false;
                                    player.chest = -1;
                                    Recipe.FindRecipes();
                                    Main.npcChatText = npc.GetChat();
                                    SoundEngine.PlaySound(SoundID.Chat);
                                }
                            }
                        }
                        if (flag && !player.mouseInterface)
                        {
                            player.cursorItemIconEnabled = false;
                            string text = npc.GivenOrTypeName;
                            int num = i;
                            if (npc.realLife >= 0)
                                num = npc.realLife;
                            if (Main.npc[num].lifeMax > 1 && !Main.npc[num].dontTakeDamage)
                            {
                                text = (npc.ModNPC is Building building)
                                    ? (building.BuildingName + ": " + Main.npc[num].life + "/" + Main.npc[num].lifeMax + "\n"
                                    + (!Building.MaxLevel(building) ? Language.GetTextValue("Mods.TF2.NPCs.Metal") + ": " + building.Metal + "/" + 200 : ""))
                                    : (npc.ModNPC is MercenaryBuddy buddy)
                                    ? (text + ": " + Main.npc[num].life + "/" + buddy.finalBaseHealth)
                                    : text + ": " + Main.npc[num].life + "/" + Main.npc[num].lifeMax;
                            }
                            self.MouseTextHackZoom(text);
                            Main.mouseText = true;
                            npc.position -= npc.netOffset;
                            break;
                        }
                        if (flag2)
                        {
                            npc.position -= npc.netOffset;
                            break;
                        }
                    }
                }
                npc.position -= npc.netOffset;
            }
        }

        private void Hook_GUIHotbarDrawInner(On_Main.orig_GUIHotbarDrawInner orig, Main self)
        {
            if (Main.playerInventory || Main.LocalPlayer.ghost || MannCoStoreActive) return;
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
            damage = Round(self.FinalDamage.ApplyTo(damage));
            return self.DamageType == ModContent.GetInstance<MercenaryDamage>() ? Math.Clamp((int)damage, 1, (int)typeof(NPC.HitModifiers).GetField("_damageLimit", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(self)) : orig(ref self, baseDamage, crit, damageVariation, luck);
        }

        private NPC.HitInfo Hook_CalculateHitInfo(On_NPC.orig_CalculateHitInfo orig, NPC self, int damage, int hitDirection, bool crit = false, float knockBack = 0f, DamageClass damageType = null, bool damageVariation = false, float luck = 0f)
        {
            NPC.HitModifiers baseModifier = new NPC.HitModifiers();
            NPC.HitModifiers modifiers = self.GetIncomingStrikeModifiers(damageType, hitDirection);
            bool superBullet = modifiers.CritDamage.Flat > 9000f;
            if (superBullet && modifiers.SourceDamage.Base < damage)
                modifiers.SourceDamage.Base = damage;
            if (modifiers.FlatBonusDamage.Value < baseModifier.FlatBonusDamage.Value)
                modifiers.FlatBonusDamage = baseModifier.FlatBonusDamage;
            if (superBullet && modifiers.FinalDamage.Base < damage)
                modifiers.FinalDamage.Base = damage;
            if (superBullet && modifiers.TargetDamageMultiplier.Value < baseModifier.TargetDamageMultiplier.Value)
                modifiers.TargetDamageMultiplier = baseModifier.TargetDamageMultiplier;
            return modifiers.DamageType == ModContent.GetInstance<MercenaryDamage>() ? modifiers.ToHitInfo(damage, crit, knockBack, damageVariation, luck) : orig(self, damage, hitDirection, crit, knockBack, damageType, damageVariation, luck);
        }

        private void Hook_UpdateNPC_BuffApplyDOTs(On_NPC.orig_UpdateNPC_BuffApplyDOTs orig, NPC self)
        {
            if (self.ModNPC is Building)
            {
                MethodInfo strikeNPCNoInteraction = typeof(NPC).GetMethod("StrikeNPCNoInteraction", BindingFlags.Instance | BindingFlags.NonPublic);
                if (self.dontTakeDamage) return;
                int num = self.lifeRegenExpectedLossPerSecond;
                if (self.poisoned)
                {
                    if (self.lifeRegen > 0)
                        self.lifeRegen = 0;
                    self.lifeRegen -= 12;
                }
                if (self.onFire)
                {
                    if (self.lifeRegen > 0)
                        self.lifeRegen = 0;
                    self.lifeRegen -= 8;
                }
                if (self.onFire3)
                {
                    if (self.lifeRegen > 0)
                        self.lifeRegen = 0;
                    self.lifeRegen -= 30;
                    if (num < 5)
                        num = 5;
                }
                if (self.onFrostBurn)
                {
                    if (self.lifeRegen > 0)
                        self.lifeRegen = 0;
                    self.lifeRegen -= 16;
                    if (num < 2)
                        num = 2;
                }
                if (self.onFrostBurn2)
                {
                    if (self.lifeRegen > 0)
                        self.lifeRegen = 0;
                    self.lifeRegen -= 50;
                    if (num < 10)
                        num = 10;
                }
                if (self.onFire2)
                {
                    if (self.lifeRegen > 0)
                        self.lifeRegen = 0;
                    self.lifeRegen -= 48;
                    if (num < 10)
                        num = 10;
                }
                if (self.venom)
                {
                    if (self.lifeRegen > 0)
                        self.lifeRegen = 0;
                    self.lifeRegen -= 60;
                    if (num < 15)
                        num = 15;
                }
                if (self.shadowFlame)
                {
                    if (self.lifeRegen > 0)
                        self.lifeRegen = 0;
                    self.lifeRegen -= 30;
                    if (num < 5)
                        num = 5;
                }
                if (self.oiled && (self.onFire || self.onFire2 || self.onFire3 || self.onFrostBurn || self.onFrostBurn2 || self.shadowFlame))
                {
                    if (self.lifeRegen > 0)
                        self.lifeRegen = 0;
                    self.lifeRegen -= 50;
                    if (num < 10)
                        num = 10;
                }
                if (self.javelined)
                {
                    if (self.lifeRegen > 0)
                        self.lifeRegen = 0;
                    int num7 = 0;
                    int num8 = 1;
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        if (Main.projectile[i].active && Main.projectile[i].type == 598 && Main.projectile[i].ai[0] == 1f && Main.projectile[i].ai[1] == self.whoAmI)
                            num7++;
                    }
                    self.lifeRegen -= num7 * 2 * 3;
                    if (num < num7 * 3 / num8)
                        num = num7 * 3 / num8;
                }
                if (self.tentacleSpiked)
                {
                    if (self.lifeRegen > 0)
                        self.lifeRegen = 0;
                    int num9 = 0;
                    int num10 = 1;
                    for (int j = 0; j < Main.maxProjectiles; j++)
                    {
                        if (Main.projectile[j].active && Main.projectile[j].type == 971 && Main.projectile[j].ai[0] == 1f && Main.projectile[j].ai[1] == self.whoAmI)
                            num9++;
                    }
                    self.lifeRegen -= num9 * 2 * 3;
                    if (num < num9 * 3 / num10)
                        num = num9 * 3 / num10;
                }
                NPCLoader.UpdateLifeRegen(self, ref num);
                if (self.lifeRegen <= -Time(4) && num < 2)
                    num = 2;
                self.lifeRegenCount += self.lifeRegen;
                if (num > 0)
                {
                    while (self.lifeRegenCount <= -Time(2) * num)
                    {
                        self.lifeRegenCount += Time(2) * num;
                        int num5 = self.whoAmI;
                        if (self.realLife >= 0)
                            num5 = self.realLife;
                        if (!Main.npc[num5].immortal)
                        {
                            Main.npc[num5].life -= num;
                            if (Main.npc[num5].ModNPC is Building building && !building.Initialized)
                                building.preConstructedDamage += num;
                        }
                        CombatText.NewText(new Rectangle((int)self.position.X, (int)self.position.Y, self.width, self.height), CombatText.LifeRegenNegative, num, dramatic: false, dot: true);
                        if (Main.npc[num5].life > 0 || Main.npc[num5].immortal) continue;
                        Main.npc[num5].life = 1;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            strikeNPCNoInteraction.Invoke(Main.npc[num5], [9999, 0f, 0]);
                            if (Main.dedServ)
                                NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, num5, 9999f);
                        }
                    }
                    return;
                }
                while (self.lifeRegenCount <= -Time(2))
                {
                    self.lifeRegenCount += Time(2);
                    int num6 = self.whoAmI;
                    if (self.realLife >= 0)
                        num6 = self.realLife;
                    if (!Main.npc[num6].immortal)
                    {
                        Main.npc[num6].life--;
                        if (Main.npc[num6].ModNPC is Building building && !building.Initialized)
                            building.preConstructedDamage += num;
                    }
                    CombatText.NewText(new Rectangle((int)self.position.X, (int)self.position.Y, self.width, self.height), CombatText.LifeRegenNegative, 1, dramatic: false, dot: true);
                    if (Main.npc[num6].life > 0 || Main.npc[num6].immortal) continue;
                    Main.npc[num6].life = 1;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        strikeNPCNoInteraction.Invoke(Main.npc[num6], [9999, 0f, 0]);
                        if (Main.dedServ)
                            NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, num6, 9999f);
                    }
                }
            }
            else orig(self);
        }

        private void Hook_CheckLifeRegen(On_NPC.orig_CheckLifeRegen orig, NPC self)
        {
            if (self.ModNPC is Building || self.ModNPC is MercenaryBuddy) return;
            else orig(self);
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

        private void Hook_DrawNPCChatButtons(On_Main.orig_DrawNPCChatButtons orig, int superColor, Color chatColor, int numLines, string focusText, string focusText3)
        {
            if (Main.npc[Main.LocalPlayer.talkNPC].type == ModContent.NPCType<SaxtonHale>())
            {
                float y = 130 + numLines * 30;
                int num = 180 + (Main.screenWidth - 800) / 2;
                Vector2 vec = new Vector2(Main.mouseX, Main.mouseY);
                Player player = Main.player[Main.myPlayer];
                Vector2 val = new Vector2(num, y);
                string text = focusText;
                DynamicSpriteFont value = FontAssets.MouseText.Value;
                Vector2 vector2 = val;
                Vector2 vector3 = new Vector2(0.9f);
                Vector2 stringSize = ChatManager.GetStringSize(value, text, vector3);
                Color baseColor = chatColor;
                Vector2 vector4 = new Vector2(1f);
                float num2 = 1.2f;
                if (stringSize.X > 260f)
                    vector4.X *= 260f / stringSize.X;
                if (vec.Between(vector2, vector2 + stringSize * vector3 * vector4.X) && !PlayerInput.IgnoreMouseInterface)
                {
                    player.mouseInterface = true;
                    player.releaseUseItem = false;
                    vector3 *= num2;
                    if (!Main.npcChatFocus2)
                        SoundEngine.PlaySound(SoundID.MenuTick);
                    Main.npcChatFocus2 = true;
                }
                else
                {
                    if (Main.npcChatFocus2)
                        SoundEngine.PlaySound(SoundID.MenuTick);
                    Main.npcChatFocus2 = false;
                }
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, value, text, vector2 + stringSize * vector4 * 0.5f, baseColor, (!Main.npcChatFocus2) ? Color.Black : Color.Brown, 0f, stringSize * 0.5f, vector3 * vector4);
                if (text.Length > 0)
                {
                    UILinkPointNavigator.SetPosition(2500, vector2 + stringSize * 0.5f);
                    UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsLeft = true;
                }
                Vector2 vector5 = new Vector2(num + stringSize.X * vector4.X + 30f, y);
                text = Lang.inter[52].Value;
                value = FontAssets.MouseText.Value;
                vector2 = vector5;
                vector3 = new Vector2(0.9f);
                stringSize = ChatManager.GetStringSize(value, text, vector3);
                baseColor = new Color(superColor, (int)(superColor / 1.1), superColor / 2, superColor);
                vector4 = new Vector2(1f);
                if (vec.Between(vector2, vector2 + stringSize * vector3 * vector4.X) && !PlayerInput.IgnoreMouseInterface)
                {
                    player.mouseInterface = true;
                    player.releaseUseItem = false;
                    vector3 *= num2;
                    if (!Main.npcChatFocus1)
                        SoundEngine.PlaySound(SoundID.MenuTick);
                    Main.npcChatFocus1 = true;
                }
                else
                {
                    if (Main.npcChatFocus1)
                        SoundEngine.PlaySound(SoundID.MenuTick);
                    Main.npcChatFocus1 = false;
                }
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, value, text, vector2 + stringSize * vector4 * 0.5f, baseColor, (!Main.npcChatFocus1) ? Color.Black : Color.Brown, 0f, stringSize * 0.5f, vector3 * vector4);
                if (text.Length > 0)
                {
                    UILinkPointNavigator.SetPosition(2501, vector2 + stringSize * 0.5f);
                    UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsMiddle = true;
                }
                if (string.IsNullOrWhiteSpace(focusText3))
                {
                    Main.npcChatFocus3 = false;
                    UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsRight = false;
                }
                else
                {
                    Vector2 vector6 = new Vector2(vector5.X + stringSize.X * vector4.X + 30f, y);
                    text = focusText3;
                    value = FontAssets.MouseText.Value;
                    vector2 = vector6;
                    vector3 = new Vector2(0.9f);
                    stringSize = ChatManager.GetStringSize(value, text, vector3);
                    baseColor = chatColor;
                    vector4 = new Vector2(1f);
                    vector5.X = vector6.X;
                    if (vec.Between(vector2, vector2 + stringSize * vector3 * vector4.X) && !PlayerInput.IgnoreMouseInterface)
                    {
                        player.mouseInterface = true;
                        player.releaseUseItem = false;
                        vector3 *= num2;
                        if (!Main.npcChatFocus3)
                            SoundEngine.PlaySound(SoundID.MenuTick);
                        Main.npcChatFocus3 = true;
                    }
                    else
                    {
                        if (Main.npcChatFocus3)
                            SoundEngine.PlaySound(SoundID.MenuTick);
                        Main.npcChatFocus3 = false;
                    }
                    ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, value, text, vector2 + stringSize * vector4 * 0.5f, baseColor, (!Main.npcChatFocus3) ? Color.Black : Color.Brown, 0f, stringSize * 0.5f, vector3 * vector4);
                    UILinkPointNavigator.SetPosition(2502, vector2 + stringSize * 0.5f);
                    UILinkPointNavigator.Shortcuts.NPCCHAT_ButtonsRight = true;
                }
            }
            else orig(superColor, chatColor, numLines, focusText, focusText3);
        }

        private void Hook_Damage(On_Projectile.orig_Damage orig, Projectile self)
        {
            if (self.ModProjectile is TF2Projectile)
                damageFalloff = true;
            orig(self);
            damageFalloff = false;
        }

        private int Hook_DamageVar(On_Main.orig_DamageVar_float_float orig, float dmg, float luck) => damageFalloff ? Round(dmg) : orig(dmg, luck);

        public static int Time(double time) => Convert.ToInt32(time * 60);

        public static int Minute(double time) => Convert.ToInt32(time * 3600);

        public static int GetHealth(Player player, double value) => Round(player.GetModPlayer<TF2Player>().healthMultiplier * value);

        public static float GetRawHealth(Player player, double value) => (float)(player.GetModPlayer<TF2Player>().healthMultiplier * value);

        public static int Round(double value) => (int)Math.Round(value, 0, MidpointRounding.AwayFromZero);

        public static int RoundByMultiple(double value, int multiple) => Round(value / multiple) * multiple;

        public static int OverhealRound(double value) => Round(value / 5f) * 5;

        public static void Minimum(ref int value, int minimum)
        {
            if (value < minimum)
                value = minimum;
        }

        public static void Minimum(ref float value, float minimum)
        {
            if (value < minimum)
                value = minimum;
        }

        public static void Minimum(ref double value, double minimum)
        {
            if (value < minimum)
                value = minimum;
        }

        public static void Maximum(ref int value, int maximum)
        {
            if (value > maximum)
                value = maximum;
        }

        public static void Maximum(ref float value, float maximum)
        {
            if (value > maximum)
                value = maximum;
        }

        public static void Maximum(ref double value, double maximum)
        {
            if (value > maximum)
                value = maximum;
        }

        public static Vector2 Lerp(Vector2 value1, Vector2 value2, double amount)
        {
            Maximum(ref amount, 1);
            return new Vector2((float)Utils.Lerp(value1.X, value2.X, amount), (float)Utils.Lerp(value1.Y, value2.Y, amount));
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
                if (player.inventory[i].type == type && player.inventory[i].ModItem is TF2Item weapon && weapon.equipped)
                {
                    foundItem = true;
                    break;
                }
            }
            return foundItem;
        }

        public static bool IsItemTypeInHotbar(Player player, int[] type)
        {
            bool foundItem = false;
            for (int i = 0; i < 10; i++)
            {
                if (type.Contains(player.inventory[i].type) && player.inventory[i].ModItem is TF2Item weapon && weapon.equipped)
                {
                    foundItem = true;
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
            projectile.Projectile.netUpdate = true;
            return projectile;
        }

        public static TF2Projectile NPCCreateProjectile(IEntitySource spawnSource, Vector2 position, Vector2 velocity, int type, int damage, float knockBack, int owner = -1, float ai0 = 0, float ai1 = 0, float ai2 = 0, bool crit = false, bool miniCrit = false)
        {
            int i = Projectile.NewProjectile(spawnSource, position, velocity, type, damage, knockBack, owner, ai0, ai1, ai2);
            TF2Projectile projectile = Main.projectile[i].ModProjectile as TF2Projectile;
            if (crit)
                projectile.crit = true;
            else if (miniCrit)
                projectile.miniCrit = true;
            projectile.Projectile.netUpdate = true;
            return projectile;
        }

        public static void NPCDistanceModifier(NPC npc, Projectile projectile, Player target, ref Player.HurtModifiers modifiers, float maxDamageMultiplier = 1.5f, float distance = 500f, bool noDistanceModifier = false)
        {
            if (noDistanceModifier) return;
            if (projectile.ModProjectile is TF2Projectile tf2Projectile)
            {
                if (npc.ModNPC is not PyroNPC || npc.ModNPC is not EnemyPyroNPC)
                {
                    if (!tf2Projectile.crit && !tf2Projectile.miniCrit)
                        modifiers.FinalDamage *= maxDamageMultiplier - Utils.Clamp(Vector2.Distance(npc.Center, target.Center) / distance, 0f, 1f);
                    else
                        modifiers.FinalDamage *= maxDamageMultiplier - Utils.Clamp(Vector2.Distance(npc.Center, target.Center) / distance, 0f, 0.5f);
                }
                else
                    modifiers.FinalDamage *= Utils.Clamp((float)projectile.timeLeft / Time(1), 0.5f, 1f);
            }
        }

        public static void NPCDistanceModifier(NPC npc, Projectile projectile, NPC target, ref NPC.HitModifiers modifiers, float maxDamageMultiplier = 1.5f, float distance = 1000f, bool noDistanceModifier = false)
        {
            if (noDistanceModifier) return;
            if (projectile.ModProjectile is TF2Projectile tf2Projectile)
            {
                if (npc.ModNPC is not PyroNPC || npc.ModNPC is not EnemyPyroNPC)
                {
                    if (!tf2Projectile.crit && !tf2Projectile.miniCrit)
                        modifiers.FinalDamage *= maxDamageMultiplier - Utils.Clamp(Vector2.Distance(npc.Center, target.Center) / distance, 0f, 1f);
                    else
                        modifiers.FinalDamage *= maxDamageMultiplier - Utils.Clamp(Vector2.Distance(npc.Center, target.Center) / distance, 0f, 0.5f);
                }
                else
                    modifiers.FinalDamage *= Utils.Clamp((float)projectile.timeLeft / Time(1), 0.5f, 1f);
            }
        }

        public static NPC FindBuilding(Player player, float maxDetectDistance)
        {
            NPC foundBuilding = null;
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                float sqrDistanceToTarget = Vector2.DistanceSquared(npc.Center, player.Center);
                if (sqrDistanceToTarget < sqrMaxDetectDistance && npc.ModNPC is Building building && building.Owner == player.whoAmI)
                {
                    sqrMaxDetectDistance = sqrDistanceToTarget;
                    foundBuilding = npc;
                }
            }
            return foundBuilding;
        }

        public static void SetPlayerDirection(Player player)
        {
            Vector2 pointPosition = player.RotatedRelativePoint(player.MountedCenter, reverseRotation: true);
            Vector2 value = Vector2.UnitX.RotatedBy(player.fullRotation);
            Vector2 vector = Main.MouseWorld - pointPosition;
            if (vector != Vector2.Zero)
                vector.Normalize();
            float num = Vector2.Dot(value, vector);
            player.ChangeDir((num > 0f) ? 1 : -1);
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

        public static void RemoveAllDebuffs(Player player)
        {
            for (int i = 0; i < Player.MaxBuffs; i++)
            {
                int buffTypes = player.buffType[i];
                if (Main.debuff[buffTypes] && player.buffTime[i] > 0 && !BuffID.Sets.NurseCannotRemoveDebuff[buffTypes] && !TF2BuffBase.cooldownBuff[buffTypes])
                {
                    player.DelBuff(i);
                    i = -1;
                }
            }
        }

        public static void RemoveAllDebuffs(NPC npc)
        {
            for (int i = 0; i < NPC.maxBuffs; i++)
            {
                int buffTypes = npc.buffType[i];
                if (Main.debuff[buffTypes] && npc.buffTime[i] > 0 && !BuffID.Sets.NurseCannotRemoveDebuff[buffTypes] && !TF2BuffBase.cooldownBuff[buffTypes])
                {
                    npc.DelBuff(i);
                    i = -1;
                }
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

        public static void AddAmmo(Player player, double percentage)
        {
            float multiplier = (float)percentage / 100f;
            foreach (Item item in player.inventory)
            {
                if (item.ModItem is TF2Weapon weapon)
                {
                    if (weapon.maxAmmoReserve > 0)
                    {
                        weapon.currentAmmoReserve += Round(weapon.maxAmmoReserve * multiplier);
                        weapon.currentAmmoReserve = Utils.Clamp(weapon.currentAmmoReserve, 0, weapon.maxAmmoReserve);
                    }
                    else
                    {
                        weapon.currentAmmoClip += Round(weapon.maxAmmoClip * multiplier);
                        weapon.currentAmmoClip = Utils.Clamp(weapon.currentAmmoClip, 0, weapon.maxAmmoClip);
                    }
                }
            }
        }

        public static void AddMoney(Player player, float amount, Vector2? spawnLocation = null)
        {
            player.GetModPlayer<TF2Player>().money += amount;
            AdvancedPopupRequest moneyText = new()
            {
                Color = Color.DarkOliveGreen,
                DurationInFrames = 30,
                Velocity = new Vector2(0, -5),
                Text = amount.ToString("C", CultureInfo.CurrentCulture)
            };
            PopupText.NewText(moneyText, spawnLocation ?? player.position);
        }

        public static void Overheal(Player player, int healAmount, float limit = 0.5f)
        {
            if (healAmount > 0)
            {
                TF2Player p = player.GetModPlayer<TF2Player>();
                int maxHealth = TF2Player.TotalHealth(player);
                if (p.overheal >= OverhealRound(maxHealth * limit * p.overhealMultiplier)) return;
                player.statLife += healAmount;
                if (player.statLife > maxHealth)
                {
                    int extraHealth = player.statLife - maxHealth - p.overheal;
                    p.overheal += extraHealth;
                    Maximum(ref p.overheal, OverhealRound(maxHealth * limit * p.overhealMultiplier));
                    player.statLife = Round((p.BaseHealth + p.healthBonus) * p.healthMultiplier + p.overheal);
                }
                player.HealEffect(healAmount);
            }
        }

        public static void OverhealNPC(NPC target, int healAmount, float limit = 0.5f)
        {
            if (target.ModNPC is not MercenaryBuddy) return;
            if (healAmount > 0)
            {
                MercenaryBuddy buddy = target.ModNPC as MercenaryBuddy;
                int maxHealth = buddy.finalBaseHealth;
                if (buddy.overheal >= OverhealRound(maxHealth * limit)) return;
                target.life += healAmount;
                if (target.life > maxHealth)
                {
                    int extraHealth = target.life - maxHealth - buddy.overheal;
                    buddy.overheal += extraHealth;
                    Maximum(ref buddy.overheal, OverhealRound(maxHealth * limit));
                    target.life = Round(buddy.finalBaseHealth + buddy.overheal);
                }
                target.HealEffect(healAmount);
            }
        }

        public static void OverhealMultiplayer(Player player, int healAmount, float limit = 0.5f)
        {
            if (!player.GetModPlayer<TF2Player>().ClassSelected || Main.netMode == NetmodeID.SinglePlayer) return;
            ModPacket packet = ModContent.GetInstance<TF2>().GetPacket();
            packet.Write((byte)MessageType.Overheal);
            packet.Write((byte)player.whoAmI);
            packet.Write(healAmount);
            packet.Write(limit);
            packet.Send(-1, Main.myPlayer);
        }

        public static void OverhealNPCMultiplayer(NPC target, int healAmount, float limit = 0.5f)
        {
            if (target.ModNPC is not MercenaryBuddy || Main.netMode == NetmodeID.SinglePlayer) return;
            ModPacket packet = ModContent.GetInstance<TF2>().GetPacket();
            packet.Write((byte)MessageType.OverhealNPC);
            packet.Write((byte)target.whoAmI);
            packet.Write(healAmount);
            packet.Write(limit);
            packet.Send(-1, Main.myPlayer);
        }

        public static void SpawnNPCMultiplayer(Player player, NPC npc, int type)
        {
            if (Main.netMode == NetmodeID.SinglePlayer) return;
            ModPacket packet = ModContent.GetInstance<TF2>().GetPacket();
            packet.Write((byte)MessageType.SpawnNPC);
            packet.Write((byte)player.whoAmI);
            packet.Write(type);
            packet.Write((byte)npc.whoAmI);
            packet.Send(-1, Main.myPlayer);
        }

        public static void SetFeignDeathSpy(NPC npc)
        {
            (npc.ModNPC as SpyNPC).temporaryBuddy = true;
            if (Main.netMode == NetmodeID.SinglePlayer) return;
            ModPacket packet = ModContent.GetInstance<TF2>().GetPacket();
            packet.Write((byte)MessageType.FeignDeath);
            packet.Write((byte)npc.whoAmI);
            packet.Send(-1, Main.myPlayer);
        }

        public static int OverwriteNPC(IEntitySource source, int X, int Y, int Type, int Start = 0, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f, float ai3 = 0f, int Target = 255)
        {
            if (Start >= 0)
            {
                Main.npc[Start] = new NPC();
                Main.npc[Start].SetDefaults(Type);
                Main.npc[Start].whoAmI = Start;
                Main.npc[Start].position.X = X - Main.npc[Start].width / 2;
                Main.npc[Start].position.Y = Y - Main.npc[Start].height;
                Main.npc[Start].active = true;
                Main.npc[Start].timeLeft = (int)(NPC.activeTime * 1.25);
                Main.npc[Start].wet = Collision.WetCollision(Main.npc[Start].position, Main.npc[Start].width, Main.npc[Start].height);
                Main.npc[Start].ai[0] = ai0;
                Main.npc[Start].ai[1] = ai1;
                Main.npc[Start].ai[2] = ai2;
                Main.npc[Start].ai[3] = ai3;
                Main.npc[Start].target = Target;
                Main.npc[Start].ModNPC?.OnSpawn(source);
                return Start;
            }
            return 200;
        }

        public static NPC OverwriteNPCDirect(IEntitySource source, int x, int y, int type, int start = 0, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f, float ai3 = 0f, int target = 255) => Main.npc[OverwriteNPC(source, x, y, type, start, ai0, ai1, ai2, ai3, target)];

        public static void KillNPC(NPC npc)
        {
            npc.life = 0;
            if (Main.netMode == NetmodeID.SinglePlayer) return;
            ModPacket packet = ModContent.GetInstance<TF2>().GetPacket();
            packet.Write((byte)MessageType.KillNPC);
            packet.Write((byte)npc.whoAmI);
            packet.Send(-1, Main.myPlayer);
        }

        public static SlotId PlaySound(SoundStyle? style, Vector2 position)
        {
            SlotId sound = SoundEngine.PlaySound(style, position);
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = ModContent.GetInstance<TF2>().GetPacket();
                packet.Write((byte)MessageType.SyncSound);
                packet.Write(style?.SoundPath ?? "");
                packet.Write(position.X);
                packet.Write(position.Y);
                packet.Send(-1, Main.myPlayer);
            }
            return sound;
        }

        public static void DropLoot(NPC npc, int type, int chanceDenominator = 1, int minimumDropped = 1, int maximumDropped = 1)
        {
            if (Main.rand.NextBool(chanceDenominator))
            {
                int item = Item.NewItem(npc.GetSource_Loot(), (int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, type, Main.rand.Next(minimumDropped, maximumDropped + 1), noBroadcast: true);
                if (Main.dedServ)
                {
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        if (Main.player[i].active)
                            NetMessage.SendData(MessageID.InstancedItem, i, -1, null, item);
                    }
                }
            }
        }

        public static bool BasicEnemiesThatCanDropMoney(NPC npc) => npc.TypeName == "Flower Fairy"
                || npc.TypeName == "Snow Fairy"
                || npc.TypeName == "Sand Fairy"
                || npc.TypeName == "Water Fairy"
                || npc.TypeName == "Spore Fairy"
                || npc.TypeName == "Stone Fairy"
                || npc.TypeName == "Metal Fairy"
                || npc.TypeName == "Blood Fairy"
                || npc.TypeName == "Bone Fairy"
                || npc.TypeName == "Thorn Fairy"
                || npc.TypeName == "Lava Fairy"
                || npc.TypeName == "Hell Raven"
                || npc.TypeName == "Yamawaro Scout"
                || npc.TypeName == "Yamawaro Vanguard"
                || npc.TypeName == "Sunflower Fairy"
                || npc.TypeName == "Crystal Fairy"
                || npc.TypeName == "Kappa Explorer"
                || npc.TypeName == "Kappa Pathfinder"
                || npc.TypeName == "Kappa Pioneer"
                || npc.TypeName == "Kappa Adventurer"
                || npc.TypeName == "Vengeful Spirit";

        public static void CreateSoulItem(NPC npc, float damage, float health, int pierce = 1)
        {
            SoulItem soulItem = Main.item[Item.NewItem(npc.GetSource_Loot(), (int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<SoulItem>())].ModItem as SoulItem;
            soulItem.damageMultiplier = damage;
            soulItem.healthMultiplier = health;
            soulItem.pierce = pierce;
            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendData(MessageID.SyncItem, number: soulItem.Item.whoAmI);
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
            if (Main.dedServ)
                NetMessage.SendData(25, -1, -1, NetworkText.FromLiteral(text), 255, color.R, color.G, color.B, 0, 0, 0);
        }

        public static bool IsTheSameAs(Item item, Item compareItem) => item.netID == compareItem.netID && item.type == compareItem.type;

        public struct WeaponSize(int x, int y)
        {
            public int X = x;
            public int Y = y;
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