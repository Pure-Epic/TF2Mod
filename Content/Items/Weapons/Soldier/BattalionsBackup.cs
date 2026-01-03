using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items.Materials;
using TF2.Content.NPCs.Buddies;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Soldier
{
    public class BattalionsBackup : TF2Weapon
    {
        public override Asset<Texture2D> WeaponActiveTexture => ModContent.Request<Texture2D>("TF2/Content/Textures/Items/Soldier/BattalionsBackup_Bugle");

        protected override string BackTexture => "TF2/Content/Textures/Items/Soldier/BattalionsBackup";

        protected override string BackTextureReverse => "TF2/Content/Textures/Items/Soldier/BattalionsBackupReverse";

        protected override int HealthBoost => 20;

        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Soldier, Secondary, Unique, Craft);
            SetWeaponSize(50, 50);
            SetWeaponOffset(4, 1);
            SetBannerUseStyle();
            SetWeaponAttackSpeed(2.645, hide: true);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/buff_banner_horn");
            SetWeaponPrice(weapon: 1, reclaimed: 1);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNeutralAttribute(description);
        }

        protected override bool WeaponAddTextureCondition(Player player) => player.GetModPlayer<TF2Player>().bannerType == 2;

        protected override Asset<Texture2D> WeaponBackTexture(Player player) => !player.GetModPlayer<BattalionsBackupPlayer>().bannerBuff ? base.WeaponBackTexture(player) : (player.direction == -1 ? ItemTextures.BattalionsBackupTextures[0] : ItemTextures.BattalionsBackupTextures[1]);

        protected override bool WeaponModifyHealthCondition(Player player) => player.GetModPlayer<TF2Player>().bannerType == 2;

        protected override void WeaponAttackAnimation(Player player)
        {
            float direction = -MathHelper.PiOver2 * player.direction;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, direction);
            player.itemLocation = player.MountedCenter;
        }

        public override bool WeaponCanBeUsed(Player player)
        {
            BattalionsBackupPlayer bannerPlayer = player.GetModPlayer<BattalionsBackupPlayer>();
            return bannerPlayer.rage >= bannerPlayer.MaxRage;
        }

        protected override void WeaponActiveUpdate(Player player)
        {
            if (isActive && player.ItemAnimationEndingOrEnded)
            {
                int buff = ModContent.BuffType<BattalionsBackupBuff>();
                foreach (Player targetPlayer in Main.ActivePlayers)
                    targetPlayer.AddBuff(buff, TF2.Time(10));
                foreach (NPC targetNPC in Main.ActiveNPCs)
                {
                    if (targetNPC.ModNPC is Buddy)
                        targetNPC.AddBuff(buff, TF2.Time(10));
                }
                isActive = false;
            }
        }

        protected override void WeaponPassiveUpdate(Player player) => player.GetModPlayer<TF2Player>().bannerType = 2;

        protected override bool? WeaponOnUse(Player player)
        {
            TF2.SetPlayerDirection(player);
            isActive = true;
            return true;
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
            if (bannerBuff && Player.HasBuff<BattalionsBackupBuff>())
            {
                rage = 0;
                int buffIndex = Player.FindBuffIndex(ModContent.BuffType<BattalionsBackupBuff>());
                buffDuration = Player.buffTime[buffIndex];
            }
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            if (bannerBuff)
                modifiers.FinalDamage *= 0.65f;
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (bannerBuff)
                modifiers.FinalDamage *= 0.5f;
        }
    }
}