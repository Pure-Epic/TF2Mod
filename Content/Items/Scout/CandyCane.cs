using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Consumables;
using TF2.Content.Items.Medic;
using TF2.Content.Items.MultiClass;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Scout
{
    public class CandyCane : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Scout, Melee, Unique, Craft);
            SetSwingUseStyle();
            SetWeaponDamage(damage: 35);
            SetWeaponAttackSpeed(0.5);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/melee_swing");
            SetWeaponPrice(weapon: 2, scrap: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override void WeaponPassiveUpdate(Player player) => player.GetModPlayer<CandyCanePlayer>().candyCaneEquipped = true;

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.Crit)
            {
                IEntitySource healthSource = target.GetSource_FromAI();
                int type = ModContent.ItemType<SmallHealth>();
                int item = Item.NewItem(healthSource, target.Center, type);
                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendData(MessageID.SyncItem, number: item);
            }
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            if (player.GetModPlayer<TF2Player>().crit)
            {
                IEntitySource healthSource = target.GetSource_FromAI();
                int type = ModContent.ItemType<SmallHealth>();
                int item = Item.NewItem(healthSource, target.Center, type);
                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendData(MessageID.SyncItem, number: item);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Kritzkrieg>()
                .AddIngredient<PainTrain>()
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }

    public class CandyCanePlayer : ModPlayer
    {
        public bool candyCaneEquipped;

        public override void ResetEffects() => candyCaneEquipped = false;

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (candyCaneEquipped)
                modifiers.FinalDamage *= 1.25f;
        }
    }
}