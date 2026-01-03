using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Items.Modules
{
    public abstract class TF2Module : TF2Item
    {
        protected override string CustomCategory => Language.GetTextValue("Mods.TF2.UI.Items.Module");

        public virtual int ModuleBuff => 0;

        public virtual int ModuleTimeLimit => TF2.Time(60);

        public virtual bool AutomaticActivation => false;

        public virtual bool Unlocked => true;

        public virtual bool Passive => false;

        protected virtual void ModuleUpdate(Player player)
        { }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => DefaultTooltipsWithAvailability(tooltips);

        protected override bool WeaponModifyDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (!Main.LocalPlayer.GetModPlayer<TF2Player>().ClassSelected)
                spriteBatch.Draw(TextureAssets.Cd.Value, position - TextureAssets.InventoryBack9.Value.Size() / 4.225f * Main.inventoryScale, null, drawColor, 0f, new Vector2(0.5f, 0.5f), 0.8f * Main.inventoryScale, SpriteEffects.None, 0f);
            return false;
        }

        public override void UpdateEquip(Player player)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (!Passive)
            {
                if (Unlocked)
                {
                    if (p.moduleActivated)
                    {
                        p.moduleBuff = ModuleBuff;
                        player.AddBuff(p.moduleBuff, TF2.Time(60));
                    }
                    else
                    {
                        player.ClearBuff(p.moduleBuff);
                        p.moduleBuff = 0;
                    }
                }
                else
                {
                    int buff = player.FindBuffIndex(p.moduleBuff);
                    if (buff > -1)
                        TF2.Maximum(ref player.buffTime[buff], ModuleTimeLimit);
                    p.moduleActivated = false;
                    if (!player.HasBuff(p.moduleBuff))
                        p.moduleBuff = 0;
                }
            }
            ModuleUpdate(player);
        }
    }
}
