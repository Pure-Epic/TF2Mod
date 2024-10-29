using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items.Materials;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Soldier
{
    public class BattalionsBackup : TF2Accessory
    {
        protected override string BackTexture => "TF2/Content/Textures/Items/Soldier/BattalionsBackup";
        
        protected override string BackTextureReverse => "TF2/Content/Textures/Items/Soldier/BattalionsBackupReverse";

        protected override int HealthBoost => 20;

        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Soldier, Secondary, Unique, Craft);
            SetWeaponPrice(weapon: 1, reclaimed: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNeutralAttribute(description);
        }

        protected override bool WeaponAddTextureCondition(Player player) => player.GetModPlayer<TF2Player>().bannerType == 2;

        protected override Asset<Texture2D> WeaponBackTexture(Player player) => !player.GetModPlayer<BattalionsBackupPlayer>().buffActive ? base.WeaponBackTexture(player) : (player.direction == -1 ? ItemTextures.BattalionsBackupTextures[0] : ItemTextures.BattalionsBackupTextures[1]);

        protected override bool WeaponModifyHealthCondition(Player player) => player.GetModPlayer<TF2Player>().bannerType == 2;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            p.bannerType = 2;
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<BuffBanner>().AddIngredient<ReclaimedMetal>().AddTile<AustraliumAnvil>().Register();
    }

    public class BattalionsBackupPlayer : BannerPlayer
    {
        public override int BannerID => 2;

        public override void PostUpdate()
        {
            rage = Utils.Clamp(rage, 0, MaxRage);
            if (!Player.GetModPlayer<TF2Player>().HasBanner)
                rage = 0;
            if (buffActive && Player.HasBuff<DefenseRage>())
            {
                rage = 0;
                int buffIndex = Player.FindBuffIndex(ModContent.BuffType<DefenseRage>());
                buffDuration = Player.buffTime[buffIndex];
            }
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            if (buffActive)
                modifiers.FinalDamage *= 0.65f;
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (buffActive)
                modifiers.FinalDamage *= 0.5f;
        }
    }
}