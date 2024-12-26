using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Items.Materials;
using TF2.Content.Items.Weapons.MultiClass;
using TF2.Content.Projectiles;
using TF2.Content.Projectiles.Demoman;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Weapons.Demoman
{
    public class UllapoolCaber : TF2Weapon
    {
        public override Asset<Texture2D> WeaponActiveTexture => (timer[0] >= TF2.Time(5)) ? TextureAssets.Item[Type]: ModContent.Request<Texture2D>("TF2/Content/Textures/UllapoolCaberDetonated");

        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Demoman, Melee, Unique, Craft);
            SetSwingUseStyle();
            SetWeaponDamage(damage: 55, noRandomCriticalHits: true);
            SetWeaponAttackSpeed(0.96);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/Weapons/melee_swing");
            SetWeaponAttackIntervals(deploy: 1);
            SetTimers(TF2.Time(5));
            SetWeaponPrice(weapon: 1, scrap: 3);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddNegativeAttribute(description);
            AddNeutralAttribute(description);
        }

        protected override bool WeaponDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = (timer[0] >= TF2.Time(5)) ? TextureAssets.Item[Type].Value : ModContent.Request<Texture2D>("TF2/Content/Textures/UllapoolCaberDetonated").Value;
            spriteBatch.Draw(texture, position, null, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
            return false;
        }

        protected override bool WeaponDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = (timer[0] >= TF2.Time(5)) ? TextureAssets.Item[Type].Value : ModContent.Request<Texture2D>("TF2/Content/Textures/UllapoolCaberDetonated").Value;
            spriteBatch.Draw(texture, Item.position - Main.screenPosition + new Vector2(Item.width / 2, Item.height - texture.Height * 0.5f + 2f), null, lightColor.MultiplyRGB(alphaColor), rotation, texture.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            return false;
        }

        protected override void WeaponPassiveUpdate(Player player)
        {
            if (!(player.ItemAnimationActive && player.HeldItem == Item))
            {
                timer[0]++;
                timer[0] = Utils.Clamp(timer[0], 0, TF2.Time(5));
            }
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage) => damage.Base = (timer[0] >= TF2.Time(5)) ? 20f : 0f;

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (timer[0] < TF2.Time(5)) return;
            timer[0] = 0;
            Explode(target);
            TF2Projectile projectile = TF2.CreateProjectile(this, player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<UllapoolCaberHitbox>(), damageDone, 0f, player.whoAmI);
            (projectile as UllapoolCaberHitbox).sparedNPC = target;
            NetMessage.SendData(MessageID.SyncProjectile, number: projectile.Projectile.whoAmI);
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            if (timer[0] < TF2.Time(5)) return;
            timer[0] = 0;
            Explode(target);
            TF2Projectile projectile = TF2.CreateProjectile(this, player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<UllapoolCaberHitbox>(), hurtInfo.Damage, 0f, player.whoAmI);
            (projectile as UllapoolCaberHitbox).sparedPlayer = target;
            NetMessage.SendData(MessageID.SyncProjectile, number: projectile.Projectile.whoAmI);
        }

        private static void Explode(Entity target)
        {
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/explode"), target.position);
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Voicelines/demoman_specialcompleted11"), target.position);
            for (int i = 0; i < 50; i++)
            {
                Dust dust = Dust.NewDustDirect(target.position, target.width, target.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
                dust.velocity *= 1.4f;
            }
            for (int i = 0; i < 80; i++)
            {
                Dust dust = Dust.NewDustDirect(target.position, target.width, target.height, DustID.Torch, 0f, 0f, 100, default, 3f);
                dust.noGravity = true;
                dust.velocity *= 5f;
                dust = Dust.NewDustDirect(target.position, target.width, target.height, DustID.Torch, 0f, 0f, 100, default, 2f);
                dust.velocity *= 3f;
            }
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<PainTrain>().AddIngredient<ScrapMetal>(2).AddTile<AustraliumAnvil>().Register();
    }
}