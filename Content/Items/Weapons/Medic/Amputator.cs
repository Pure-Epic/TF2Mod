using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items.Materials;
using TF2.Content.NPCs.Buddies;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Medic
{
    public class Amputator : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Medic, Melee, Unique, Unlock);
            SetSwingUseStyle();
            SetWeaponDamage(damage: 52);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/melee_swing");
            SetTimers(TF2.Time(4.2));
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
            if (timer[0] >= TF2.Time(4.2))
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
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/amputator_heal"), player.Center);
                    foreach (NPC healedNPC in Main.ActiveNPCs)
                    {
                        if (healedNPC.ModNPC is MercenaryBuddy)
                            healedNPC.AddBuff(ModContent.BuffType<AmputatorBuff>(), TF2.Time(4.2));                        
                    }
                    timer[1] = 1;
                    timer[0] = 0;
                }
            }
            timer[0]++;
            if (timer[0] >= TF2.Time(4.2))
                timer[0] = TF2.Time(4.2);
            if (timer[1] == 1)
            {
                if (player.HeldItem.ModItem == this)
                {
                    lockWeapon = true;
                    player.GetModPlayer<TF2Player>().stopRegen = true;
                }
                timer[2]++;
                if (timer[2] >= TF2.Time(0.25))
                {
                    foreach (Player healedPlayer in Main.ActivePlayers)
                    {
                        if (!TF2Player.IsHealthFull(healedPlayer) && !healedPlayer.dead)
                        {
                            TF2Player p = healedPlayer.GetModPlayer<TF2Player>();
                            int healingAmount = TF2.Round(TF2.GetHealth(healedPlayer, 6.25) * p.healReduction);
                            healedPlayer.Heal(healingAmount);
                            NetMessage.SendData(MessageID.SpiritHeal, number: healedPlayer.whoAmI, number2: healingAmount);
                            for (int j = 0; j < player.inventory.Length; j++)
                            {
                                Item item = player.inventory[j];
                                if (item.ModItem is TF2Weapon weapon && weapon.GetWeaponMechanic("Medi Gun"))
                                    weapon.uberCharge += TF2.Round(healingAmount * weapon.uberChargeCapacity / 100f * 0.1275510204f);
                            }
                        }
                    }
                    foreach (NPC healedNPC in Main.ActiveNPCs)
                    {
                        if (healedNPC.ModNPC is MercenaryBuddy buddy && healedNPC.life < buddy.finalBaseHealth)
                        {
                            for (int j = 0; j < player.inventory.Length; j++)
                            {
                                Item item = player.inventory[j];
                                if (item.ModItem is TF2Weapon weapon && weapon.GetWeaponMechanic("Medi Gun"))
                                    weapon.uberCharge += TF2.Round(6.25f * weapon.uberChargeCapacity / 100f * 0.1275510204f);
                            }
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

        public override void AddRecipes() => CreateRecipe().AddIngredient<VitaSaw>().AddIngredient<ScrapMetal>().AddTile<CraftingAnvil>().Register();
    }
}