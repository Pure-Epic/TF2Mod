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
using TF2.Content.UI.HUD.Medic;
using TF2.Content.UI.HUD.Pyro;
using TF2.Content.UI.HUD.Scout;
using TF2.Content.UI.HUD.Sniper;
using TF2.Content.UI.HUD.Soldier;
using TF2.Content.UI.HUD.Spy;

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

        internal SandmanAmmoHUD sandmanAmmoHUD = new SandmanAmmoHUD();
        internal UserInterface sandmanAmmoHUDUserInterface = new UserInterface();
        internal SandmanChargeMeterHUD sandmanChargeMeterHUD = new SandmanChargeMeterHUD();
        internal UserInterface sandmanChargeMeterHUDUserInterface = new UserInterface();
        internal BonkAtomicPunchAmmoHUD bonkAtomicPunchAmmoHUD = new BonkAtomicPunchAmmoHUD();
        internal UserInterface bonkAtomicPunchAmmoHUDUserInterface = new UserInterface();
        internal BonkAtomicPunchChargeMeterHUD bonkAtomicPunchChargeMeterHUD = new BonkAtomicPunchChargeMeterHUD();
        internal UserInterface bonkAtomicPunchChargeMeterHUDUserInterface = new UserInterface();
        internal MadMilkAmmoHUD madMilkAmmoHUD = new MadMilkAmmoHUD();
        internal UserInterface madMilkAmmoHUDUserInterface = new UserInterface();
        internal MadMilkChargeMeterHUD madMilkChargeMeterHUD = new MadMilkChargeMeterHUD();
        internal UserInterface madMilkChargeMeterHUDUserInterface = new UserInterface();

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
        internal HeadAmountHUD headAmountHUD = new HeadAmountHUD();
        internal UserInterface headAmountHUDUserInterface = new UserInterface();

        internal SandvichAmmoHUD sandvichAmmoHUD = new SandvichAmmoHUD();
        internal UserInterface sandvichAmmoHUDUserInterface = new UserInterface();
        internal SandvichChargeMeterHUD sandvichChargeMeterHUD = new SandvichChargeMeterHUD();
        internal UserInterface sandvichChargeMeterHUDUserInterface = new UserInterface();

        internal MetalHUD metalHUD = new MetalHUD();
        internal UserInterface metalHUDUserInterface = new UserInterface();
        internal RevengeAmountHUD revengeAmountHUD = new RevengeAmountHUD();
        internal UserInterface revengeAmountHUDUserInterface = new UserInterface();

        internal UberchargeHUD uberchargeHUD = new UberchargeHUD();
        internal UserInterface uberchargeHUDUserInterface = new UserInterface();
        internal CrusadersCrossbowAmmoHUD crusadersCrossbowHUD = new CrusadersCrossbowAmmoHUD();
        internal UserInterface crusadersCrossbowAmmoHUDUserInterface = new UserInterface();
        internal OrganAmountHUD organAmountHUD = new OrganAmountHUD();
        internal UserInterface organAmountHUDUserInterface = new UserInterface();

        internal SniperRifleAmmoHUD sniperRifleAmmoHUD = new SniperRifleAmmoHUD();
        internal UserInterface sniperRifleAmmoHUDUserInterface = new UserInterface();
        internal HuntsmanAmmoHUD huntsmanAmmoHUD = new HuntsmanAmmoHUD();
        internal UserInterface huntsmanAmmoHUDUserInterface = new UserInterface();
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

        internal static LocalizedText[] TF2HUDLocalization { get; private set; }

        public static void DrawHUD(List<GameInterfaceLayer> layers, UserInterface ui)
        {
            int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
            if (resourceBarIndex != -1)
            {
                layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
                    "TF2: Ammo Bar",
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
            TF2HUDLocalization = new LocalizedText[]
            {
                Language.GetText("Mods.TF2.UI.HUD.Scout.Ball"),
                Language.GetText("Mods.TF2.UI.HUD.Scout.Drink"),
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
                Language.GetText("Mods.TF2.UI.HUD.Spy.Feign"),
            };
            if (!Main.dedServ)
            {
                hudType = new TF2HUD[]
                {
                    ammoHUD,
                    heavyAmmoHUD,
                    sandmanAmmoHUD,
                    sandmanChargeMeterHUD,
                    bonkAtomicPunchAmmoHUD,
                    bonkAtomicPunchChargeMeterHUD,
                    madMilkAmmoHUD,
                    madMilkChargeMeterHUD,
                    buffBannerChargeMeterHUD,
                    flareGunAmmoHUD,
                    stickybombLauncherAmmoHUD,
                    stickybombAmountHUD,
                    shieldChargeMeterHUD,
                    headAmountHUD,
                    sandvichAmmoHUD,
                    sandvichChargeMeterHUD,
                    metalHUD,
                    revengeAmountHUD,
                    uberchargeHUD,
                    crusadersCrossbowHUD,
                    organAmountHUD,
                    sniperRifleAmmoHUD,
                    huntsmanAmmoHUD,
                    jarateAmmoHUD,
                    jarateChargeMeterHUD,
                    razorbackChargeMeterHUD,
                    invisWatchChargeMeterHUD,
                    cloakAndDaggerChargeMeterHUD,
                    deadRingerChargeMeterHUD
                };
                userInterfaces = new UserInterface[]
                {
                    ammoHUDUserInterface,
                    heavyAmmoUserInterface,
                    sandmanAmmoHUDUserInterface,
                    sandmanChargeMeterHUDUserInterface,
                    bonkAtomicPunchAmmoHUDUserInterface,
                    bonkAtomicPunchChargeMeterHUDUserInterface,
                    madMilkAmmoHUDUserInterface,
                    madMilkChargeMeterHUDUserInterface,
                    buffBannerChargeMeterHUDUserInterface,
                    flareGunAmmoHUDUserInterface,
                    stickybombLauncherAmmoHUDUserInterface,
                    stickybombAmountHUDUserInterface,
                    shieldChargeMeterHUDUserInterface,
                    headAmountHUDUserInterface,
                    sandvichAmmoHUDUserInterface,
                    sandvichChargeMeterHUDUserInterface,
                    metalHUDUserInterface,
                    revengeAmountHUDUserInterface,
                    uberchargeHUDUserInterface,
                    crusadersCrossbowAmmoHUDUserInterface,
                    organAmountHUDUserInterface,
                    sniperRifleAmmoHUDUserInterface,
                    huntsmanAmmoHUDUserInterface,
                    jarateAmmoHUDUserInterface,
                    jarateChargeMeterHUDUserInterface,
                    razorbackChargeMeterHUDUserInterface,
                    invisWatchChargeMeterHUDUserInterface,
                    cloakAndDaggerChargeMeterHUDUserInterface,
                    deadRingerChargeMeterHUDUserInterface
                };
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