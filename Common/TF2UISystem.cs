using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using TF2.Content.UI;

namespace TF2.Common
{
    public class TF2UISystem : ModSystem
    {
        private UserInterface _ammoBarUserInterface;
        internal AmmoUI AmmoUI;

        private UserInterface _buffBannerUserInterface;
        internal BuffBannerUI BuffBannerUI;

        private UserInterface _shieldChargeUserInterface;
        internal ShieldUI ShieldUI;

        internal StickybombUI StickybombUI;
        internal StickybombAmountUI StickybombAmountUI;
        private UserInterface _stickybombUserInterface;
        private UserInterface _stickybombAmountUserInterface;

        private UserInterface _metalUserInterface;
        internal MetalUI MetalUI;

        private UserInterface _uberChargeUserInterface;
        internal UberChargeUI UberChargeUI;

        private UserInterface _sniperUserInterface;
        internal SniperUI SniperUI;

        private UserInterface _invisWatchUserInterface;
        internal InvisWatchUI InvisWatchUI;
        private UserInterface _CloakAndDaggerUserInterface;
        internal CloakAndDaggerUI CloakAndDaggerUI;
        private UserInterface _deadRingerUserInterface;
        internal DeadRingerUI DeadRingerUI;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                AmmoUI = new AmmoUI();
                _ammoBarUserInterface = new UserInterface();
                _ammoBarUserInterface.SetState(AmmoUI);

                BuffBannerUI = new BuffBannerUI();
                _buffBannerUserInterface = new UserInterface();
                _buffBannerUserInterface.SetState(BuffBannerUI);

                ShieldUI = new ShieldUI();
                _shieldChargeUserInterface = new UserInterface();
                _shieldChargeUserInterface.SetState(ShieldUI);

                StickybombUI = new StickybombUI();
                StickybombAmountUI = new StickybombAmountUI();
                _stickybombUserInterface = new UserInterface();
                _stickybombUserInterface.SetState(StickybombUI);
                _stickybombAmountUserInterface = new UserInterface();
                _stickybombAmountUserInterface.SetState(StickybombAmountUI);

                MetalUI = new MetalUI();
                _metalUserInterface = new UserInterface();
                _metalUserInterface.SetState(MetalUI);

                UberChargeUI = new UberChargeUI();
                _uberChargeUserInterface = new UserInterface();
                _uberChargeUserInterface.SetState(UberChargeUI);

                SniperUI = new SniperUI();
                _sniperUserInterface = new UserInterface();
                _sniperUserInterface.SetState(SniperUI);

                InvisWatchUI = new InvisWatchUI();
                _invisWatchUserInterface = new UserInterface();
                _invisWatchUserInterface.SetState(InvisWatchUI);
                CloakAndDaggerUI = new CloakAndDaggerUI();
                _CloakAndDaggerUserInterface = new UserInterface();
                _CloakAndDaggerUserInterface.SetState(CloakAndDaggerUI);
                DeadRingerUI = new DeadRingerUI();
                _deadRingerUserInterface = new UserInterface();
                _deadRingerUserInterface.SetState(DeadRingerUI);
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            _ammoBarUserInterface?.Update(gameTime);
            _buffBannerUserInterface?.Update(gameTime);
            _shieldChargeUserInterface?.Update(gameTime);
            _stickybombUserInterface?.Update(gameTime);
            _stickybombAmountUserInterface?.Update(gameTime);
            _metalUserInterface?.Update(gameTime);
            _uberChargeUserInterface?.Update(gameTime);
            _sniperUserInterface?.Update(gameTime);
            _invisWatchUserInterface?.Update(gameTime);
            _CloakAndDaggerUserInterface?.Update(gameTime);
            _deadRingerUserInterface?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
            if (resourceBarIndex != -1)
            {
                layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
                    "TF2: Ammo Bar",
                    delegate
                    {
                        _ammoBarUserInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }

            int buffBannerIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
            if (buffBannerIndex != -1)
            {
                layers.Insert(buffBannerIndex, new LegacyGameInterfaceLayer(
                    "TF2: Buff Banner Bar",
                    delegate
                    {
                        _buffBannerUserInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }

            int shieldIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
            if (shieldIndex != -1)
            {
                layers.Insert(shieldIndex, new LegacyGameInterfaceLayer(
                    "TF2: Shield Charge Bar",
                    delegate
                    {
                        _shieldChargeUserInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }

            int stickybombIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
            if (stickybombIndex != -1)
            {
                layers.Insert(stickybombIndex, new LegacyGameInterfaceLayer(
                    "TF2: Stickybomb Charge",
                    delegate
                    {
                        _stickybombUserInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }

            int stickybombAmountIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
            if (stickybombAmountIndex != -1)
            {
                layers.Insert(stickybombAmountIndex, new LegacyGameInterfaceLayer(
                    "TF2: Stickybomb Amount",
                    delegate
                    {
                        _stickybombAmountUserInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }

            int metalIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
            if (metalIndex != -1)
            {
                layers.Insert(metalIndex, new LegacyGameInterfaceLayer(
                    "TF2: Metal",
                    delegate
                    {
                        _metalUserInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }

            int uberChargeIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
            if (uberChargeIndex != -1)
            {
                layers.Insert(uberChargeIndex, new LegacyGameInterfaceLayer(
                    "TF2: UberCharge",
                    delegate
                    {
                        _uberChargeUserInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }

            int sniperIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
            if (sniperIndex != -1)
            {
                layers.Insert(sniperIndex, new LegacyGameInterfaceLayer(
                    "TF2: Sniper Charge",
                    delegate
                    {
                        _sniperUserInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }

            int invisWatchIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
            if (invisWatchIndex != -1)
            {
                layers.Insert(invisWatchIndex, new LegacyGameInterfaceLayer(
                    "TF2: Invis Watch",
                    delegate
                    {
                        _invisWatchUserInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }

            int CloakAndDaggerIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
            if (CloakAndDaggerIndex != -1)
            {
                layers.Insert(CloakAndDaggerIndex, new LegacyGameInterfaceLayer(
                    "TF2: Cloak and Dagger",
                    delegate
                    {
                        _CloakAndDaggerUserInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }

            int deadRingerIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
            if (deadRingerIndex != -1)
            {
                layers.Insert(deadRingerIndex, new LegacyGameInterfaceLayer(
                    "TF2: Dead Ringer",
                    delegate
                    {
                        _deadRingerUserInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
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