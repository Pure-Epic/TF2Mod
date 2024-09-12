using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using TF2.Common;
using TF2.Content.UI.HUD.Demoman;
using TF2.Content.UI.HUD.Engineer;
using TF2.Content.UI.HUD.Heavy;
using TF2.Content.UI.HUD.Inventory;
using TF2.Content.UI.HUD.Medic;
using TF2.Content.UI.HUD.Pyro;
using TF2.Content.UI.HUD.Scout;
using TF2.Content.UI.HUD.Sniper;
using TF2.Content.UI.HUD.Soldier;
using TF2.Content.UI.HUD.Spy;
using TF2.Content.UI.MercenaryCreationMenu;

namespace TF2.Content.UI.HUD
{
    public class TF2HUDSystem : ModSystem
    {
        private TF2HUD[] hudType;
        private UserInterface[] userInterfaces;

        internal AmmoHUD ammoHUD = new AmmoHUD();
        internal UserInterface ammoHUDUserInterface = new UserInterface();
        internal HeavyAmmoHUD heavyAmmoHUD = new HeavyAmmoHUD();
        internal UserInterface heavyAmmoUserInterface = new UserInterface();

        internal SodaPopperHUD sodaPopperHUD = new SodaPopperHUD();
        internal UserInterface sodaPopperHUDUserInterface = new UserInterface();
        internal BonkAtomicPunchAmmoHUD bonkAtomicPunchAmmoHUD = new BonkAtomicPunchAmmoHUD();
        internal UserInterface bonkAtomicPunchAmmoHUDUserInterface = new UserInterface();
        internal BonkAtomicPunchChargeMeterHUD bonkAtomicPunchChargeMeterHUD = new BonkAtomicPunchChargeMeterHUD();
        internal UserInterface bonkAtomicPunchChargeMeterHUDUserInterface = new UserInterface();
        internal MadMilkAmmoHUD madMilkAmmoHUD = new MadMilkAmmoHUD();
        internal UserInterface madMilkAmmoHUDUserInterface = new UserInterface();
        internal MadMilkChargeMeterHUD madMilkChargeMeterHUD = new MadMilkChargeMeterHUD();
        internal UserInterface madMilkChargeMeterHUDUserInterface = new UserInterface();
        internal SandmanAmmoHUD sandmanAmmoHUD = new SandmanAmmoHUD();
        internal UserInterface sandmanAmmoHUDUserInterface = new UserInterface();
        internal SandmanChargeMeterHUD sandmanChargeMeterHUD = new SandmanChargeMeterHUD();
        internal UserInterface sandmanChargeMeterHUDUserInterface = new UserInterface();

        internal BuffBannerChargeMeterHUD buffBannerChargeMeterHUD = new BuffBannerChargeMeterHUD();
        internal UserInterface buffBannerChargeMeterHUDUserInterface = new UserInterface();

        internal FlareGunAmmoHUD flareGunAmmoHUD = new FlareGunAmmoHUD();
        internal UserInterface flareGunAmmoHUDUserInterface = new UserInterface();

        internal StickybombLauncherAmmoHUD stickybombLauncherAmmoHUD = new StickybombLauncherAmmoHUD();
        internal UserInterface stickybombLauncherAmmoHUDUserInterface = new UserInterface();
        internal StickybombAmountHUD stickybombAmountHUD = new StickybombAmountHUD();
        internal UserInterface stickybombAmountHUDUserInterface = new UserInterface();
        internal ShieldChargeMeterHUD shieldChargeMeterHUD = new ShieldChargeMeterHUD();
        internal UserInterface shieldChargeMeterHUDUserInterface = new UserInterface();
        internal EyelanderHeadAmountHUD eyelanderHeadAmountHUD = new EyelanderHeadAmountHUD();
        internal UserInterface eyelanderHeadAmountHUDUserInterface = new UserInterface();

        internal SandvichAmmoHUD sandvichAmmoHUD = new SandvichAmmoHUD();
        internal UserInterface sandvichAmmoHUDUserInterface = new UserInterface();
        internal SandvichChargeMeterHUD sandvichChargeMeterHUD = new SandvichChargeMeterHUD();
        internal UserInterface sandvichChargeMeterHUDUserInterface = new UserInterface();

        internal MetalHUD metalHUD = new MetalHUD();
        internal UserInterface metalHUDUserInterface = new UserInterface();
        internal SentryHUD sentryHUD = new SentryHUD();
        internal UserInterface sentryHUDUserInterface = new UserInterface();
        internal DispenserHUD dispenserHUD = new DispenserHUD();
        internal UserInterface dispenserHUDUserInterface = new UserInterface();
        internal TeleporterEntranceHUD teleporterEntranceHUD = new TeleporterEntranceHUD();
        internal UserInterface teleporterEntranceHUDUserInterface = new UserInterface();
        internal TeleporterExitHUD teleporterExitHUD = new TeleporterExitHUD();
        internal UserInterface teleporterExitHUDUserInterface = new UserInterface();
        internal ConstructionPDAHUD constructionPDAHUD = new ConstructionPDAHUD();
        internal UserInterface constructionPDAHUDUserInterface = new UserInterface();
        internal DestructionPDAHUD destructionPDAHUD = new DestructionPDAHUD();
        internal UserInterface destructionPDAHUDUserInterface = new UserInterface();
        internal RevengeAmountHUD revengeAmountHUD = new RevengeAmountHUD();
        internal UserInterface revengeAmountHUDUserInterface = new UserInterface();

        internal CrusadersCrossbowAmmoHUD crusadersCrossbowHUD = new CrusadersCrossbowAmmoHUD();
        internal UserInterface crusadersCrossbowAmmoHUDUserInterface = new UserInterface();
        internal UberchargeHUD uberchargeHUD = new UberchargeHUD();
        internal UserInterface uberchargeHUDUserInterface = new UserInterface();
        internal OrganAmountHUD organAmountHUD = new OrganAmountHUD();
        internal UserInterface organAmountHUDUserInterface = new UserInterface();

        internal SniperRifleAmmoHUD sniperRifleAmmoHUD = new SniperRifleAmmoHUD();
        internal UserInterface sniperRifleAmmoHUDUserInterface = new UserInterface();
        internal HuntsmanAmmoHUD huntsmanAmmoHUD = new HuntsmanAmmoHUD();
        internal UserInterface huntsmanAmmoHUDUserInterface = new UserInterface();
        internal BazaarBargainHeadAmountHUD bazaarBargainHeadAmountHUD = new BazaarBargainHeadAmountHUD();
        internal UserInterface bazaarBargainHeadAmountHUDUserInterface = new UserInterface();
        internal JarateAmmoHUD jarateAmmoHUD = new JarateAmmoHUD();
        internal UserInterface jarateAmmoHUDUserInterface = new UserInterface();
        internal JarateChargeMeterHUD jarateChargeMeterHUD = new JarateChargeMeterHUD();
        internal UserInterface jarateChargeMeterHUDUserInterface = new UserInterface();
        internal RazorbackChargeMeterHUD razorbackChargeMeterHUD = new RazorbackChargeMeterHUD();
        internal UserInterface razorbackChargeMeterHUDUserInterface = new UserInterface();

        internal InvisWatchChargeMeterHUD invisWatchChargeMeterHUD = new InvisWatchChargeMeterHUD();
        internal UserInterface invisWatchChargeMeterHUDUserInterface = new UserInterface();
        internal CloakAndDaggerChargeMeterHUD cloakAndDaggerChargeMeterHUD = new CloakAndDaggerChargeMeterHUD();
        internal UserInterface cloakAndDaggerChargeMeterHUDUserInterface = new UserInterface();
        internal DeadRingerChargeMeterHUD deadRingerChargeMeterHUD = new DeadRingerChargeMeterHUD();
        internal UserInterface deadRingerChargeMeterHUDUserInterface = new UserInterface();

        internal ItemDropHUD itemDropHUD = new ItemDropHUD();
        internal UserInterface itemDropHUDUserInterface = new UserInterface();

        internal static LocalizedText[] TF2HUDLocalization { get; private set; }

        public static void DrawHUD(List<GameInterfaceLayer> layers, UserInterface ui)
        {
            int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
            if (resourceBarIndex > -1)
            {
                layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
                    "TF2 HUD",
                    delegate
                    {
                        ui.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        public override void Load()
        {
            TF2HUDLocalization =
            [
                Language.GetText("Mods.TF2.UI.HUD.Scout.Hype"),
                Language.GetText("Mods.TF2.UI.HUD.Scout.Boost"),
                Language.GetText("Mods.TF2.UI.HUD.Scout.Drink"),
                Language.GetText("Mods.TF2.UI.HUD.Scout.Ball"),
                Language.GetText("Mods.TF2.UI.HUD.Soldier.Rage"),
                Language.GetText("Mods.TF2.UI.HUD.Demoman.Charge"),
                Language.GetText("Mods.TF2.UI.HUD.Demoman.Heads"),
                Language.GetText("Mods.TF2.UI.HUD.Heavy.Food"),
                Language.GetText("Mods.TF2.UI.HUD.Engineer.Revenge"),
                Language.GetText("Mods.TF2.UI.HUD.Medic.Ubercharge"),
                Language.GetText("Mods.TF2.UI.HUD.Medic.Organs"),
                Language.GetText("Mods.TF2.UI.HUD.Sniper.Jar"),
                Language.GetText("Mods.TF2.UI.HUD.Sniper.Razorback"),
                Language.GetText("Mods.TF2.UI.HUD.Spy.Cloak"),
                Language.GetText("Mods.TF2.UI.HUD.Spy.Motion"),
                Language.GetText("Mods.TF2.UI.HUD.Spy.Feign")
            ];
            if (!Main.dedServ)
            {
                hudType =
                [
                    ammoHUD,
                    heavyAmmoHUD,
                    sodaPopperHUD,
                    bonkAtomicPunchAmmoHUD,
                    bonkAtomicPunchChargeMeterHUD,
                    madMilkAmmoHUD,
                    madMilkChargeMeterHUD,
                    sandmanAmmoHUD,
                    sandmanChargeMeterHUD,
                    buffBannerChargeMeterHUD,
                    flareGunAmmoHUD,
                    stickybombLauncherAmmoHUD,
                    stickybombAmountHUD,
                    shieldChargeMeterHUD,
                    eyelanderHeadAmountHUD,
                    sandvichAmmoHUD,
                    sandvichChargeMeterHUD,
                    metalHUD,
                    sentryHUD,
                    dispenserHUD,
                    teleporterEntranceHUD,
                    teleporterExitHUD,
                    constructionPDAHUD,
                    destructionPDAHUD,
                    revengeAmountHUD,
                    crusadersCrossbowHUD,
                    uberchargeHUD,
                    organAmountHUD,
                    sniperRifleAmmoHUD,
                    huntsmanAmmoHUD,
                    bazaarBargainHeadAmountHUD,
                    jarateAmmoHUD,
                    jarateChargeMeterHUD,
                    razorbackChargeMeterHUD,
                    invisWatchChargeMeterHUD,
                    cloakAndDaggerChargeMeterHUD,
                    deadRingerChargeMeterHUD,
                    itemDropHUD
                ];
                userInterfaces =
                [
                    ammoHUDUserInterface,
                    heavyAmmoUserInterface,
                    sodaPopperHUDUserInterface,
                    bonkAtomicPunchAmmoHUDUserInterface,
                    bonkAtomicPunchChargeMeterHUDUserInterface,
                    madMilkAmmoHUDUserInterface,
                    madMilkChargeMeterHUDUserInterface,
                    sandmanAmmoHUDUserInterface,
                    sandmanChargeMeterHUDUserInterface,
                    buffBannerChargeMeterHUDUserInterface,
                    flareGunAmmoHUDUserInterface,
                    stickybombLauncherAmmoHUDUserInterface,
                    stickybombAmountHUDUserInterface,
                    shieldChargeMeterHUDUserInterface,
                    eyelanderHeadAmountHUDUserInterface,
                    sandvichAmmoHUDUserInterface,
                    sandvichChargeMeterHUDUserInterface,
                    metalHUDUserInterface,
                    sentryHUDUserInterface,
                    dispenserHUDUserInterface,
                    teleporterEntranceHUDUserInterface,
                    teleporterExitHUDUserInterface,
                    constructionPDAHUDUserInterface,
                    destructionPDAHUDUserInterface,
                    revengeAmountHUDUserInterface,
                    crusadersCrossbowAmmoHUDUserInterface,
                    uberchargeHUDUserInterface,
                    organAmountHUDUserInterface,
                    sniperRifleAmmoHUDUserInterface,
                    huntsmanAmmoHUDUserInterface,
                    bazaarBargainHeadAmountHUDUserInterface,
                    jarateAmmoHUDUserInterface,
                    jarateChargeMeterHUDUserInterface,
                    razorbackChargeMeterHUDUserInterface,
                    invisWatchChargeMeterHUDUserInterface,
                    cloakAndDaggerChargeMeterHUDUserInterface,
                    deadRingerChargeMeterHUDUserInterface,
                    itemDropHUDUserInterface
                ];
                var fullHUD = hudType.Zip(userInterfaces, (hud, ui) => new { HUD = hud, UI = ui });
                foreach (var eachHUD in fullHUD)
                    eachHUD.UI.SetState(eachHUD.HUD);
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            foreach (UserInterface ui in userInterfaces)
                ui?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            foreach (UserInterface ui in userInterfaces)
                DrawHUD(layers, ui);
        }
    }

    public class TF2TipsSystem : ModSystem
    {
        public override void ModifyGameTipVisibility(IReadOnlyList<GameTipData> gameTips)
        {
            if (ModContent.GetInstance<TF2ConfigClient>().DefaultTips) return;
            foreach (GameTipData tip in gameTips)
            {
                if (tip.Mod is not TF2)
                    tip.Hide();
            }
        }
    }
}