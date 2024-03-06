using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Materials;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Medic
{
    public class VitaSaw : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Medic, Melee, Unique, Unlock);
            SetSwingUseStyle();
            SetWeaponDamage(damage: 65);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/melee_swing");
            SetWeaponPrice(weapon: 1, scrap: 2);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
            AddNeutralAttribute(description);
        }

        protected override void WeaponPassiveUpdate(Player player) => TF2Player.SetPlayerHealth(player, -10);

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.type == NPCID.TargetDummy) return;
            player.GetModPlayer<TF2Player>().organs++;
            player.GetModPlayer<TF2Player>().organs = Utils.Clamp(player.GetModPlayer<TF2Player>().organs, 0, 4);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Ubersaw>()
                .AddIngredient<ScrapMetal>(2)
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }

    public class VitaSawPlayer : ModPlayer
    {
        public float deathUberCharge;

        public override void PostUpdate()
        {
            int organs = Player.GetModPlayer<TF2Player>().organs;
            if (!Player.dead && TF2.GetItemInHotbar(Player, new int[] { ModContent.ItemType<MediGun>(), ModContent.ItemType<Kritzkrieg>() })?.ModItem is TF2Weapon weapon)
                deathUberCharge = Utils.Clamp(weapon.uberCharge, 0f, weapon.uberChargeCapacity * 0.15f * organs);
        }
    }
}