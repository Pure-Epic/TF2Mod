using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using TF2.Content.UI;

namespace TF2.Common
{
    public class TF2System : ModSystem
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

        private UserInterface _uberchargeUserInterface;
        internal UberchargeUI UberchargeUI;

        private UserInterface _sniperUserInterface;
        internal SniperUI SniperUI;

        private UserInterface _invisWatchUserInterface;
        internal InvisWatchUI InvisWatchUI;
        private UserInterface _cloakandDaggerUserInterface;
        internal CloakandDaggerUI CloakandDaggerUI;
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

                UberchargeUI = new UberchargeUI();
                _uberchargeUserInterface = new UserInterface();
                _uberchargeUserInterface.SetState(UberchargeUI);

                SniperUI = new SniperUI();
                _sniperUserInterface = new UserInterface();
                _sniperUserInterface.SetState(SniperUI);

                InvisWatchUI = new InvisWatchUI();
                _invisWatchUserInterface = new UserInterface();
                _invisWatchUserInterface.SetState(InvisWatchUI);
                CloakandDaggerUI = new CloakandDaggerUI();
                _cloakandDaggerUserInterface = new UserInterface();
                _cloakandDaggerUserInterface.SetState(CloakandDaggerUI);
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
            _uberchargeUserInterface?.Update(gameTime);
            _sniperUserInterface?.Update(gameTime);
            _invisWatchUserInterface?.Update(gameTime);
            _cloakandDaggerUserInterface?.Update(gameTime);
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

            int uberchargeIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
            if (uberchargeIndex != -1)
            {
                layers.Insert(uberchargeIndex, new LegacyGameInterfaceLayer(
                    "TF2: Ubercharge",
                    delegate
                    {
                        _uberchargeUserInterface.Draw(Main.spriteBatch, new GameTime());
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

            int cloakandDaggerIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
            if (cloakandDaggerIndex != -1)
            {
                layers.Insert(cloakandDaggerIndex, new LegacyGameInterfaceLayer(
                    "TF2: Cloak and Dagger",
                    delegate
                    {
                        _cloakandDaggerUserInterface.Draw(Main.spriteBatch, new GameTime());
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
}