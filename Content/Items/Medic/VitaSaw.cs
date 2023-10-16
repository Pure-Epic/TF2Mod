using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Materials;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Medic
{
    public class VitaSaw : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Medic, Melee, Unique, Unlock);
            SetSwingUseStyle();
            SetWeaponDamage(damage: 65);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/melee_swing");
            SetWeaponPrice(weapon: 1, scrap: 2);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
            AddNeutralAttribute(description);
        }

        protected override void WeaponPassiveUpdate(Player player) => SetPlayerHealth(player, 93);

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.type == NPCID.TargetDummy) return;
            player.GetModPlayer<TF2Player>().organs += 1;
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
        public int deathUberCharge;
        public bool giveUberChargeFromVitaSaw;
        public int timer;

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            TF2Player p = Player.GetModPlayer<TF2Player>();
            if (p.uberCharge >= 600)
                deathUberCharge = (int)(p.uberCharge * 0.6f);
            else
                deathUberCharge = Utils.Clamp((int)p.uberCharge, 0, 150 * p.organs);
        }

        public override void OnRespawn() => giveUberChargeFromVitaSaw = true;

        public override void PostUpdate()
        {
            TF2Player p = Player.GetModPlayer<TF2Player>();
            if (giveUberChargeFromVitaSaw)
            {
                timer++;
                if (timer >= 2) // MUST always be 2!
                {
                    giveUberChargeFromVitaSaw = false;
                    p.organs = 0;
                    deathUberCharge = 0;
                    timer = 0;
                }
            }
        }
    }
}