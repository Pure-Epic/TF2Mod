using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items.Materials;
using TF2.Content.Projectiles;
using TF2.Content.Projectiles.Demoman;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Demoman
{
    public class SplendidScreen : TF2Accessory
    {
        protected override string ArmTexture => "TF2/Content/Textures/Items/Demoman/SplendidScreen";

        protected override string ArmTextureReverse => "TF2/Content/Textures/Items/Demoman/SplendidScreenReverse";

        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Demoman, Secondary, Unique, Craft);
            SetWeaponPrice(weapon: 1, reclaimed: 2);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            List<string> currentShieldChargeKey = KeybindSystem.ShieldCharge.GetAssignedKeys(0);
            if (currentShieldChargeKey.Count <= 0 || currentShieldChargeKey.Contains("None"))
                AddNeutralAttribute(description);
            else
                AddOtherAttribute(description, currentShieldChargeKey[0] + (string)this.GetLocalization("Notes2"));
        }

        protected override bool WeaponAddTextureCondition(Player player) => player.GetModPlayer<TF2Player>().shieldType == 2;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            p.shieldType = 2;
            SplendidScreenPlayer shield = player.GetModPlayer<SplendidScreenPlayer>();
            if (shield.chargeActive && !shield.chargeProjectileCreated)
            {
                Vector2 chargeDirection = player.DirectionTo(Main.MouseWorld);
                TF2Projectile projectile = TF2.CreateProjectile(null, player.GetSource_Accessory(Item), player.Center, chargeDirection * 2.5f, ModContent.ProjectileType<ShieldHitbox>(), TF2.Round((85f + player.GetModPlayer<EyelanderPlayer>().heads * 8.5f) * p.damageMultiplier), 0f, player.whoAmI);
                if (p.miniCrit)
                {
                    projectile.miniCrit = true;
                    projectile.Projectile.netUpdate = true;
                }
                TF2.SetPlayerDirection(player);
                shield.chargeProjectileCreated = true;
                for (int i = 0; i < Player.MaxBuffs; i++)
                {
                    int buffTypes = player.buffType[i];
                    if (Main.debuff[buffTypes] && player.buffTime[i] > 0 && !BuffID.Sets.NurseCannotRemoveDebuff[buffTypes] && !TF2BuffBase.cooldownBuff[buffTypes])
                    {
                        player.DelBuff(i);
                        i = -1;
                    }
                }
            }
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<CharginTarge>().AddIngredient<ReclaimedMetal>(2).AddTile<AustraliumAnvil>().Register();
    }

    public class SplendidScreenPlayer : ShieldPlayer
    {
        public override int ShieldRechargeTime => TF2.Time(8);

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            TF2Player p = Player.GetModPlayer<TF2Player>();
            if (p.shieldType == 1)
                modifiers.FinalDamage *= 0.8f;
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            TF2Player p = Player.GetModPlayer<TF2Player>();
            if (p.shieldType == 1)
                modifiers.FinalDamage *= 0.8f;
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (KeybindSystem.ShieldCharge.JustPressed && timer >= ShieldRechargeTime)
            {
                chargeActive = true;
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Voicelines/demoman_charge_windup1"), Player.Center);
            }
        }
    }
}