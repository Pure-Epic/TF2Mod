using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Items.Ammo;
using Terraria.DataStructures;
using TF2.Projectiles.Demoman;
using Terraria.Audio;

namespace TF2.Items.Demoman
{
    public class StickybombLauncher : TF2Weapon
    {
        public int stickybombsAmount = 0;
        public int maxStickybombs = 0;

        public float chargeTime = 0;
        public float maxChargeUp;
        public float stickybombChargeInterval;
        public float chargeUpRate;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stickybomb Launcher");
            Tooltip.SetDefault("Demoman's Starter Secondary");
            ItemID.Sets.ItemsThatCountAsBombsForDemolitionistToSpawn[Item.type] = true;
        }
        public override void SafeSetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.scale = 1f;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.channel = true;

            Item.damage = 120; //Damage set on projectile
            Item.shoot = ModContent.ProjectileType<Stickybomb>();
            Item.shootSpeed = 10f;
            Item.useAmmo = ModContent.ItemType<SecondaryAmmo>();
            Item.knockBack = 20;
            Item.rare = ItemRarityID.White;

            maxStickybombs = 8;
            ammoCost = 1;
            maxAmmoClip = 8;
            ammoInClip = 8;
            reloadRate = 40f;
            maxChargeUp = 4f;
            stickybombChargeInterval = 1f;
            chargeUpRate = 60f;
        }
        public override void HoldItem(Player player)
        {
            WeaponSystem clip = player.GetModPlayer<WeaponSystem>();
            clip.ammoMax = maxAmmoClip;
            clip.ammoReloadRate = reloadRate;
            clip.ammoCurrent = ammoInClip;
            UpdateResource();
            Charge();
            TFClass p = player.GetModPlayer<TFClass>();
            p.stickybombMaxCharge = maxChargeUp;
            p.stickybombCharge = chargeTime;
            p.stickybombChargeTimer = stickybombChargeInterval;
            p.stickybombAmount = stickybombsAmount;
            p.stickybombMax = maxStickybombs;
        }
        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            TFClass p = player.GetModPlayer<TFClass>();
            if (p.classAccessory == true && !p.classHideVanity)
            {
                Item.scale = 0f;
            }
            else
            {
                Item.scale = 1f;
            }
        }
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (stickybombsAmount < maxStickybombs)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (stickybombsAmount < maxStickybombs && player.altFunctionUse != 2)
            {
                reload = false;
                ammoInClip -= 1;

                Vector2 newVelocity = new Vector2(velocity.X + chargeTime, velocity.Y - chargeTime).RotatedByRandom(MathHelper.ToRadians(0));
                SoundEngine.PlaySound(new SoundStyle("TF2/Sounds/SFX/grenade_launcher_shoot"), player.Center);
                Projectile.NewProjectile(source, position, newVelocity, ModContent.ProjectileType<Stickybomb>(), damage, knockback, player.whoAmI);
                chargeTime = 0;
                stickybombsAmount += 1;

                if (ammoInClip <= 0)
                {
                    reload = true;
                }
            }
            return false;
        }
        public override bool AltFunctionUse(Player player)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.owner == player.whoAmI && proj.type == ModContent.ProjectileType<Stickybomb>())
                {
                    proj.timeLeft = 0;
                }
            }
            SoundEngine.PlaySound(new SoundStyle("TF2/Sounds/SFX/stickybomblauncher_det"), player.Center);
            stickybombsAmount = 0;
            return true;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(0, 0);
        }

        public void Charge()
        {
            stickybombChargeInterval++;
            if (stickybombChargeInterval > chargeUpRate)
            {
                chargeTime += 1;
                stickybombChargeInterval = 0;
            }
            chargeTime = Utils.Clamp(chargeTime, 0, maxChargeUp);
        }
    }

}


