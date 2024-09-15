using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Projectiles.Engineer;

namespace TF2.Content.Items.Weapons.Engineer
{
    public class Gunslinger : TF2Weapon
    {
        protected override string ArmTexture => "TF2/Content/Textures/Items/Engineer/Gunslinger";

        protected override string ArmTextureReverse => "TF2/Content/Textures/Items/Engineer/GunslingerReverse";

        protected override int HealthBoost => 25;

        public override string Texture => "TF2/Content/Textures/MeleeHitbox";

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

        protected override bool WeaponDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            spriteBatch.Draw(ModContent.Request<Texture2D>("TF2/Content/Items/Weapons/Engineer/Gunslinger").Value, position, null, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
            return false;
        }

        protected override bool WeaponDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = ModContent.Request<Texture2D>("TF2/Content/Items/Weapons/Engineer/Gunslinger").Value;
            spriteBatch.Draw(texture, Item.position - Main.screenPosition + new Vector2(Item.width / 2, Item.height - texture.Height * 0.5f + 2f), null, lightColor.MultiplyRGB(alphaColor), rotation, texture.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            return false;
        }

        protected override bool WeaponAddTextureCondition(Player player) => player.GetModPlayer<GunslingerPlayer>().gunslingerEquipped;

        protected override void WeaponPassiveUpdate(Player player) => player.GetModPlayer<GunslingerPlayer>().gunslingerEquipped = true;

        protected override bool? WeaponOnUse(Player player)
        {
            Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<WrenchHitbox>(), 1, 0f);
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