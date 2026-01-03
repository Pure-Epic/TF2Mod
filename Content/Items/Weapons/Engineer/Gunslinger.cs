using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Projectiles.Engineer;

namespace TF2.Content.Items.Weapons.Engineer
{
    public class Gunslinger : TF2Weapon
    {
        public override Asset<Texture2D> WeaponActiveTexture => ModContent.Request<Texture2D>("TF2/Content/Textures/Nothing");

        protected override string ArmTexture => "TF2/Content/Textures/Items/Engineer/Gunslinger";

        protected override string ArmTextureReverse => "TF2/Content/Textures/Items/Engineer/GunslingerReverse";

        protected override int HealthBoost => 25;

        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Engineer, Melee, Unique, Unlock);
            SetSwingUseStyle();
            SetWeaponDamage(damage: 65, noRandomCriticalHits: true);
            SetWeaponAttackSpeed(0.8);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/gunslinger_swing");

        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddPositiveAttribute(description);
            AddNegativeAttribute(description);
        }

        protected override bool WeaponAddTextureCondition(Player player) => player.GetModPlayer<GunslingerPlayer>().gunslingerEquipped;

        protected override void WeaponPassiveUpdate(Player player) => player.GetModPlayer<GunslingerPlayer>().gunslingerEquipped = true;

        protected override bool? WeaponOnUse(Player player)
        {
            TF2.CreateProjectile(this, player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<WrenchHitbox>(), 1, 0f);
            return true;
        }

        protected override void WeaponHitPlayer(Player player, Player target, ref Player.HurtModifiers modifiers)
        {
            timer[0]++;
            if (timer[0] >= 3)
            {
                player.GetModPlayer<TF2Player>().crit = true;
                timer[0] = 0;
            }
        }

        protected override void WeaponHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            timer[0]++;
            if (timer[0] >= 3)
            {
                player.GetModPlayer<TF2Player>().crit = true;
                timer[0] = 0;
            }
        }
    }

    public class GunslingerPlayer : ModPlayer
    {
        public bool gunslingerEquipped;

        public override void ResetEffects() => gunslingerEquipped = false;
    }
}