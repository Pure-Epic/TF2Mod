using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Items.Materials;
using TF2.Content.Items.MultiClass;
using TF2.Content.Tiles.Crafting;

namespace TF2.Content.Items.Demoman
{
    public class UllapoolCaber : TF2Weapon
    {
        protected override void WeaponStatistics()
        {
            SetWeaponCategory(Demoman, Melee, Unique, Craft);
            SetSwingUseStyle();
            SetWeaponDamage(damage: 55, noRandomCriticalHits: true);
            SetWeaponAttackSpeed(0.96);
            SetWeaponAttackSound("TF2/Content/Sounds/SFX/melee_swing");
            SetWeaponAttackIntervals(deploy: 1);
            SetTimers(Time(5));
            SetWeaponPrice(weapon: 1, scrap: 3);
        }

        protected override void WeaponDescription(List<TooltipLine> description)
        {
            AddNegativeAttribute(description);
            AddNeutralAttribute(description);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture;
            if (timer[0] >= Time(5))
                texture = TextureAssets.Item[Item.type].Value;
            else
                texture = (Texture2D)ModContent.Request<Texture2D>("TF2/Content/Textures/UllapoolCaberDetonated");
            spriteBatch.Draw(texture, position, null, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture;
            if (timer[0] >= Time(5))
                texture = TextureAssets.Item[Item.type].Value;
            else
                texture = (Texture2D)ModContent.Request<Texture2D>("TF2/Content/Textures/UllapoolCaberDetonated");
            Vector2 position = Item.position - Main.screenPosition + new Vector2(Item.width / 2, Item.height - texture.Height * 0.5f + 2f);
            spriteBatch.Draw(texture, position, null, lightColor.MultiplyRGB(alphaColor), rotation, texture.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            return false;
        }

        protected override void WeaponPassiveUpdate(Player player)
        {
            if (!(player.ItemAnimationActive && player.HeldItem == Item))
            {
                timer[0]++;
                timer[0] = Utils.Clamp(timer[0], 0, Time(5));
            }
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage) => damage.Base = timer[0] >= 300 ? 75f : 55f;

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (timer[0] < Time(5)) return;
            timer[0] = 0;
            Explode(target);
            int i = Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<UllapoolCaberHitbox>(), damageDone, 0f, player.whoAmI);
            UllapoolCaberHitbox explosion = Main.projectile[i].ModProjectile as UllapoolCaberHitbox;
            explosion.sparedNPC = target;
            NetMessage.SendData(MessageID.SyncProjectile, number: i);
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            if (timer[0] < Time(5)) return;
            timer[0] = 0;
            Explode(target);
            int i = Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<UllapoolCaberHitbox>(), hurtInfo.Damage, 0f, player.whoAmI);
            UllapoolCaberHitbox explosion = Main.projectile[i].ModProjectile as UllapoolCaberHitbox;
            explosion.sparedPlayer = target;
            NetMessage.SendData(MessageID.SyncProjectile, number: i);
        }

        private static void Explode(Entity target)
        {
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/explode1"), target.position);
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/demoman_specialcompleted11"), target.position);
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

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<PainTrain>()
                .AddIngredient<ScrapMetal>(2)
                .AddTile<CraftingAnvil>()
                .Register();
        }
    }

    public class UllapoolCaberHitbox : ModProjectile
    {
        public NPC sparedNPC;
        public Player sparedPlayer;

        public override string Texture => "TF2/Content/Items/Demoman/UllapoolCaber";

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.timeLeft = 1;
        }

        public override bool? CanHitNPC(NPC target) => target != sparedNPC;

        public override bool CanHitPlayer(Player target) => target != sparedPlayer;

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Main.player[Projectile.owner].GetModPlayer<TF2Player>().crit)
                modifiers.SetCrit();
            else
                modifiers.DisableCrit();
        }

        public override void OnKill(int timeLeft)
        {
            if (TF2.FindPlayer(Projectile, 100f))
            {
                Player player = Main.player[Projectile.owner];
                Vector2 velocity = new Vector2(15f * player.direction, 15f);
                player.velocity -= velocity;
                if (player.immuneNoBlink) return;
                int selfDamage = Convert.ToInt32(Math.Floor(player.statLifeMax2 * 0.31428571428f));
                player.Hurt(PlayerDeathReason.ByCustomReason(player.name + " blew themself to smithereens."), selfDamage, 0);
            }
        }
    }
}