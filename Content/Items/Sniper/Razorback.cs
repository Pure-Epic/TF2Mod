using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;

namespace TF2.Content.Items.Sniper
{
    public class Razorback : TF2AccessorySecondary
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Sniper's Unlocked Secondary");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.accessory = true;

            Item.value = Item.buyPrice(platinum: 1);
            Item.rare = ModContent.RarityType<UniqueRarity>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) // needs System.Linq
        {
            TooltipLine tt = tooltips.FirstOrDefault(x => x.Name == "Material" && x.Mod == "Terraria");
            tooltips.Remove(tt);

            var line = new TooltipLine(Mod, "Positive Attributes",
                "Blocks a single enemy hit attempt")
            {
                OverrideColor = new Color(153, 204, 255)
            };
            tooltips.Add(line);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            p.sniperShield = true;
            RazorbackPlayer razorback = player.GetModPlayer<RazorbackPlayer>();
            if (razorback.timer <= 0)
            {
                if (razorback.evadeHit)
                    razorback.timer = 1800;
                player.AddBuff(ModContent.BuffType<Buffs.RazorbackBuff>(), 1800);
            }
        }
    }

    public class RazorbackPlayer : ModPlayer
    {
        public bool evadeHit;
        public int timer = 0;

        public override void PostNurseHeal(NPC nurse, int health, bool removeDebuffs, int price) => timer = 0;

        public override void PreUpdate()
        {
            if (timer > 0)
                timer--;
        }

        public override void ResetEffects() => evadeHit = false;

        public void Dodge()
        {
            Player.SetImmuneTimeForAllTypes(80);
            if (Player.whoAmI != Main.myPlayer)
                return;
            Player.ClearBuff(ModContent.BuffType<Buffs.RazorbackBuff>());
            timer = 1800;
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spy_shield_break"), Player.Center);
            NetMessage.SendData(MessageID.Dodge, -1, -1, null, Player.whoAmI, 2f);
        }

        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource, ref int cooldownCounter)
        {
            if (!evadeHit) return true;
            Dodge();
            return false;
        }
    }
}