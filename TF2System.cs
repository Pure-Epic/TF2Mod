using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.ID;
using TF2.UI;

namespace TF2
{
    //[Autoload(Side = ModSide.Client)]
    public class TF2System : ModSystem
    {

        private UserInterface _ammoBarUserInterface;
        internal AmmoUI AmmoUI;

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

        public override void Load()
        {
            if (!Main.dedServ) //can't run multiplayer
            {
                AmmoUI = new AmmoUI();
                _ammoBarUserInterface = new UserInterface();
                _ammoBarUserInterface.SetState(AmmoUI);

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
            }            
        }
        public override void UpdateUI(GameTime gameTime)
        {
            //if (!Main.dedServ && Main.netMode != NetmodeID.Server)
            //{
                _ammoBarUserInterface?.Update(gameTime);
                _stickybombUserInterface?.Update(gameTime);
                _stickybombAmountUserInterface?.Update(gameTime);
                _metalUserInterface?.Update(gameTime);
                _uberchargeUserInterface?.Update(gameTime);
                _sniperUserInterface?.Update(gameTime);
            //}

        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            //if (!(!Main.dedServ && Main.netMode != NetmodeID.Server)) { return; }

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
        }

    }
}