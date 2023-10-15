using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Materials;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Medic
{
    public class Amputator : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Medic, Melee, Unique, Unlock);
            SetSwingUseStyle();
            SetWeaponDamage(damage: 52);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/melee_swing");
            SetTimers(Time(4.2));
            SetWeaponPrice(weapon: 1, scrap: 3);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddHeader(description);
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        public override bool WeaponCanBeUsed(Player player) => timer[1] == 0;

        protected override void WeaponActiveUpdate(Player player)
        {
            if (timer[0] >= Time(4.2))
            {
                timer[1] = 0;
                timer[2] = 0;
                if (player.HeldItem.ModItem == this)
                {
                    lockWeapon = false;
                    player.GetModPlayer<TF2Player>().stopRegen = false;
                }
                if (player.controlUseTile && WeaponCanAltClick(player) && !player.ItemAnimationActive)
                {
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/amputator_heal"), player.Center);
                    timer[1] = 1;
                    timer[0] = 0;
                }
            }
            timer[0]++;
            if (timer[0] >= Time(4.2))
                timer[0] = Time(4.2);
            if (timer[1] == 1)
            {
                if (player.HeldItem.ModItem == this)
                {
                    lockWeapon = true;
                    player.GetModPlayer<TF2Player>().stopRegen = true;
                }
                timer[2]++;
                if (timer[2] >= Time(1))
                {
                    foreach (Player healedPlayer in Main.player)
                    {
                        if (healedPlayer.statLife < healedPlayer.statLifeMax2)
                        {
                            TF2Player p = healedPlayer.GetModPlayer<TF2Player>();
                            int healingAmount = (int)(25 * healedPlayer.statLifeMax2 / 500 * p.regenMultiplier);
                            healedPlayer.Heal(healingAmount);
                            if (Main.netMode != NetmodeID.SinglePlayer)
                                NetMessage.SendData(MessageID.PlayerHeal, number: healedPlayer.whoAmI, number2: healingAmount);
                        }
                    }
                    timer[2] = 0;
                }
            }
        }

        protected override void WeaponPassiveUpdate(Player player)
        {
            if (player.dead)
            {
                timer[0] = 0;
                lockWeapon = false;
                player.GetModPlayer<TF2Player>().stopRegen = false;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ScrapMetal>()
                .AddIngredient<VitaSaw>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }
}