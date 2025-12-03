using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items.Weapons.Demoman;
using TF2.Content.Items.Weapons.Engineer;
using TF2.Content.Items.Weapons.Sniper;
using TF2.Content.Items.Weapons.Spy;
using TF2.Content.Projectiles;
using TF2.Content.Projectiles.Demoman;
using TF2.Content.Projectiles.Pyro;
using TF2.Content.Projectiles.Spy;
using static TF2.TF2;

namespace TF2.Content.Items.Weapons
{
    public abstract class TF2Weapon : TF2Item
    {
        public bool Reloading
        {
            get => reload;
            set => reload = value;
        }

        protected float decimalDamage;
        protected int projectileAmount;
        protected float projectileAngle;
        protected int ammoCost;
        private double fireRate;
        protected double displayReloadSpeed;
        public int currentAmmoClip;
        public int maxAmmoClip;
        public int currentAmmoReserve;
        public int maxAmmoReserve;
        public bool noRandomCrits;
        public bool noRandomAmmoBoxes;
        public bool noRandomHealthKits;
        public int deploySpeed;
        public int holsterSpeed;
        protected int reloadRate;
        public int initialReloadRate;
        protected bool magazine;
        public bool noAmmoClip;
        protected bool noAmmoConsumption;
        protected bool usesAltClick;
        protected bool usesFocusShot;
        protected bool fullAutomatic;
        public SoundStyle reloadSound;

        protected WeaponSize meleeWeaponSize = new WeaponSize(50, 50);
        private bool noUseGraphic;
        public int cooldownTimer;
        public int deployTimer;
        public int holsterTimer;
        public bool lockWeapon;
        protected bool reload;
        protected int ammoReloadRateTimer;
        protected bool isCharging;
        protected bool isActive;
        protected bool hideAttackSpeed;

        protected bool weaponInitialized;
        protected float critChance;
        protected int critDuration;
        protected int critTimer;
        protected float ammoBoxChance;
        protected float healthKitChance;
        public bool noDistanceModifier;
        private SoundStyle? meleeHitSound;
        private bool isRocketLauncher;
        private bool isFlamethrower;
        protected int airblastCost;
        protected int airblastCooldown;
        protected int airblastTimer;
        protected SoundStyle flameThrowerAttackSound;
        protected SlotId flameThrowerAttackSoundSlot;
        private bool isGrenadeLauncher;
        protected bool startReloadSound;
        protected bool finishReloadSound;
        protected bool closeDrum;
        private bool isStickybombLauncher;
        public int stickybombsAmount;
        public int maxStickybombs;
        protected int armTime;
        public float chargeTime;
        public float maxChargeTime;
        protected float chargeInterval;
        protected SoundStyle stickbombLauncherAttackSound;
        protected float chargeUpRate;
        protected bool isSword;
        private bool isMinigun;
        protected int spinUpTime;
        protected int spinTimer;
        protected int minigunBulletCounter;
        protected SoundStyle minigunSpinSound;
        protected SoundStyle minigunSpinUpSound;
        protected SoundStyle minigunSpinDownSound;
        protected SoundStyle minigunAttackSound;
        protected SoundStyle minigunEmptySound;
        protected SlotId minigunSpinUpSoundSlot;
        protected SlotId minigunSpinDownSoundSlot;
        protected SlotId minigunSpinSoundSlot;
        protected SlotId minigunAttackSoundSlot;
        protected SlotId minigunEmptySoundSlot;
        protected bool endSpinUpSound;
        protected bool endSpinDownSound;
        protected int sandvichItem;
        private bool isSyringeGun;
        private bool isMediGun;
        public float uberCharge;
        public float uberChargeCapacity;
        private int uberChargeBuff;
        private int uberChargeDuration;
        protected SoundStyle mediGunHealSound;
        private bool endHealSound;
        private bool isSniperRifle;
        protected double chargeUpDamage;
        protected double additionalChargeUpDamage;
        protected float chargeUpDelay;
        protected float chargeUpDelayTimer;
        protected bool sniperReload;
        private bool isKnife;
        protected double speedPercentage;
        protected bool rightClick;
        protected bool autoUseAltClick;
        protected int spreadRecovery;
        public int singleReloadShotCooldown;
        public int singleReloadShotMaxCooldown;
        public bool singleReloadShotReload;
        protected const float StandardBulletSpeed = 10f;

        protected virtual void WeaponPostWeaponStatistics(Player player)
        { }

        protected override void WeaponSetSlot(int weaponCategory) => weaponType = weaponCategory;

        protected override bool WeaponModifyDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) => true;

        protected virtual void WeaponAttackAnimation(Player player) => Item.noUseGraphic = isKnife || noUseGraphic;

        protected virtual void WeaponSetAmmo(int maxAmmo, int maxReserve, int currentAmmo = 0)
        {
            currentAmmoClip = maxAmmoClip = maxAmmo;
            currentAmmoReserve = maxAmmoReserve = maxReserve;
        }

        protected virtual void WeaponEquip(Player player) => equipped = Item == GetWeapon(player, weaponType);

        public virtual bool WeaponCanBeUsed(Player player) => !magazine ? ((currentAmmoClip > 0 && currentAmmoClip >= ammoCost) || noAmmoClip || isMinigun) : WeaponMagazineCanBeUsed(player);

        protected virtual bool WeaponMagazineCanBeUsed(Player player) => !(currentAmmoClip <= 0 || currentAmmoClip < ammoCost || reload);

        protected virtual bool WeaponCanAltClick(Player player) => isMediGun ? ActivateUberCharge(player) : !player.HasBuff<BuffaloSteakSandvichBuff>() || weaponType == Melee;

        protected virtual bool WeaponCanConsumeAmmo(Player player)
        {
            if (isMinigun)
            {
                if (spinTimer >= spinUpTime)
                {
                    if (minigunBulletCounter >= 4)
                    {
                        minigunBulletCounter = 0;
                        return true;
                    }
                    return false;
                }
                else
                    return false;
            }
            else return currentAmmoClip > 0 && currentAmmoClip >= ammoCost && ammoCost > 0 && !ModContent.GetInstance<TF2ConfigClient>().InfiniteAmmo;
        }

        protected virtual bool WeaponResetHolsterTimeCondition(Player player) => player.controlUseItem;

        public virtual bool WeaponCriticalHits(Player player)
        {
            if (noRandomCrits) return false;
            Minimum(ref critChance, weaponType != Melee ? 2f : 15f);
            float damage = player.GetModPlayer<TF2Player>().damage;
            if (Main.rand.Next(1, 101) <= critChance)
            {
                critChance = weaponType != Melee ? 2f : 15f;
                if (Item.useTime <= Time(0.2))
                    critDuration = !isFlamethrower ? Time(2) : Time(0.5);
                player.GetModPlayer<TF2Player>().damage = 0;
                return true;
            }
            else
            {
                critChance = (weaponType != Melee ? 2f : 15f) + weaponType != Melee ? 0.0125f : 0.05625f * Utils.Clamp(damage, 0f, 800f);
                return false;
            }
        }

        public virtual bool WeaponAmmoBoxes(Player player)
        {
            if (noRandomAmmoBoxes || player.GetModPlayer<TF2Player>().noRandomAmmoBoxes) return false;
            Minimum(ref critChance, weaponType != Melee ? 2f : 15f);
            float damage = player.GetModPlayer<TF2Player>().superDamage;
            if (Main.rand.Next(1, 101) <= ammoBoxChance)
            {
                ammoBoxChance = weaponType != Melee ? 2f : 15f;
                player.GetModPlayer<TF2Player>().superDamage = 0;
                return true;
            }
            else
            {
                ammoBoxChance = (weaponType != Melee ? 2f : 15f) + weaponType != Melee ? 0.025f : 0.1125f * Utils.Clamp(damage, 0f, 1200f);
                return false;
            }
        }

        public virtual bool WeaponHealthKits(Player player)
        {
            if (noRandomHealthKits || player.GetModPlayer<TF2Player>().noRandomHealthKits) return false;
            Minimum(ref critChance, weaponType != Melee ? 2f : 15f);
            float damage = player.GetModPlayer<TF2Player>().superDamage;
            if (Main.rand.Next(1, 101) <= healthKitChance)
            {
                healthKitChance = weaponType != Melee ? 2f : 15f;
                player.GetModPlayer<TF2Player>().superDamage = 0;
                return true;
            }
            else
            {
                healthKitChance = (weaponType != Melee ? 2f : 15f) + weaponType != Melee ? 0.0125f : 0.05625f * Utils.Clamp(damage, 0f, 1600f);
                return false;
            }
        }

        protected virtual void WeaponActiveUpdate(Player player)
        { }

        protected virtual void WeaponActiveBonus(Player player)
        {
            if (isFlamethrower)
                FlameThrowerUpdate(player);
            if (isMinigun)
                MinigunUpdate(player, speedPercentage);
            if (isSniperRifle)
                SniperRifleUpdate(player, speedPercentage);
        }

        protected virtual void WeaponUpdateAmmo(Player player)
        {
            if (!noAmmoClip)
            {
                if (currentAmmoClip <= 0 && currentAmmoReserve > 0 && !noAmmoClip && player.itemAnimation == 0)
                    reload = true;
                if (Item != player.inventory[58] && Main.myPlayer == player.whoAmI)
                    UpdateAmmo(player);
            }
        }

        protected virtual void WeaponPassiveUpdate(Player player)
        { }

        protected virtual bool WeaponPreAttack(Player player)
        {
            if (isMinigun)
            {
                if (spinTimer >= spinUpTime && player.controlUseItem && player.altFunctionUse != 2)
                {
                    if (currentAmmoClip > 0)
                        minigunBulletCounter++;
                    if (SoundEngine.TryGetActiveSound(minigunSpinUpSoundSlot, out var spinUp))
                        spinUp.Stop();
                    return currentAmmoClip > 0;
                }
                else return false;
            }
            return true;
        }

        protected virtual void WeaponAttack(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!isStickybombLauncher && !isKnife && Item.ModItem is not Huntsman)
            {
                if ((currentAmmoClip <= 0 && player.ItemAnimationEndingOrEnded || reload && magazine) && !noAmmoClip || !WeaponPreAttack(player)) return;
                if (!magazine)
                    reload = false;
                if (!noAmmoClip && WeaponCanConsumeAmmo(player))
                    currentAmmoClip -= ammoCost;
                Vector2 newVelocity = fullAutomatic && spreadRecovery >= Time(1.25) ? velocity : velocity.RotatedByRandom(MathHelper.ToRadians(2.5f));
                int newDamage = !isSniperRifle ? damage : (int)Math.Round(chargeUpDamage * player.GetModPlayer<TF2Player>().damageMultiplier);
                WeaponFireProjectile(player, source, position, newVelocity, type, newDamage, knockback);
                if (fullAutomatic)
                    spreadRecovery = 0;
                if (isSniperRifle)
                {
                    bool isDamageAnInteger = decimalDamage == 0;
                    chargeUpDamage = isDamageAnInteger ? Item.damage : decimalDamage;
                    chargeTime = 0f;
                    chargeUpDelayTimer = 0;
                    if (noAmmoClip && !ModContent.GetInstance<TF2ConfigClient>().InfiniteAmmo)
                        currentAmmoClip--;
                }
            }
            else if (isKnife)
            {
                TF2Player p = player.GetModPlayer<TF2Player>();
                int i = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                KnifeProjectile projectile = Main.projectile[i].ModProjectile as KnifeProjectile;
                projectile.weapon = this;
                if (player.controlUseTile)
                {
                    projectile.crit = true;
                    p.backStab = true;
                    player.velocity = velocity * 12.5f;
                    player.immuneTime += 24;
                }
            }
        }

        protected virtual void WeaponFireProjectile(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback = 0f)
        {
            if (isFlamethrower)
                FlamethrowerProjectile(player, source, position, velocity, type, damage, knockback);
            else
                Shoot(player, source, position, velocity, type, damage, knockback, projectileAmount, projectileAngle, player.whoAmI);
        }

        protected virtual void WeaponPostFireProjectile(Player player, int projectile)
        { }

        protected virtual void WeaponMeleeHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
        {
            if (isSword)
            {
                if (player.ItemAnimationJustStarted)
                    noHitbox = true;
                hitbox = MeleeHitbox(player);
            }
        }

        protected virtual void WeaponPostAttack(Player player)
        { }

        protected virtual void WeaponHitPlayer(Player player, Player target, ref Player.HurtModifiers modifiers)
        { }

        protected virtual void WeaponHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        { }

        public virtual void WeaponDistanceModifier(Player player, Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (!noDistanceModifier && projectile.ModProjectile is TF2Projectile tf2Projectile)
            {
                if (!isFlamethrower)
                {
                    if ((!tf2Projectile.crit && !tf2Projectile.miniCrit) || tf2Projectile.weapon is Ambassador)
                        modifiers.FinalDamage *= (isRocketLauncher ? 1.25f : isSyringeGun ? 1.2f : 1.5f) - Utils.Clamp(Vector2.Distance(player.Center, target.Center) / (isRocketLauncher ? 750f : isSyringeGun ? 700f : 1000f), 0f, isRocketLauncher ? 0.75f : isSyringeGun ? 0.7f : 1f);
                    else
                        modifiers.FinalDamage *= (isRocketLauncher ? 1.25f : isSyringeGun ? 1.2f : 1.5f) - Utils.Clamp(Vector2.Distance(player.Center, target.Center) / (isRocketLauncher ? 750f : isSyringeGun ? 700f : 1000f), 0f, isRocketLauncher ? 0.25f : isSyringeGun ? 0.2f : 0.5f);
                }
                else if (!tf2Projectile.crit && !tf2Projectile.miniCrit)
                    modifiers.FinalDamage *= Utils.Clamp((float)projectile.timeLeft / Time(1), 0.5f, 1f);
            }
        }

        protected virtual bool? WeaponOnUse(Player player) => null;

        public virtual void WeaponReset()
        { }

        protected virtual void StickybombLauncherDetonate(Player player)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                Stickybomb stickybombProjectile = projectile.ModProjectile as Stickybomb;
                if (projectile.active && projectile.owner == player.whoAmI && projectile.type == Item.shoot && stickybombProjectile.weapon == this && stickybombProjectile.Timer >= armTime)
                {
                    projectile.timeLeft = 0;
                    stickybombsAmount--;
                }
            }
        }

        protected void SetWeaponSize(int width = 25, int height = 25) => Item.Size = weaponType != Melee ? (Vector2)new WeaponSize(width, height) : (Vector2)WeaponSize.MeleeWeaponSize;

        protected void SetGunUseStyle(bool focus = false, bool automatic = false, bool rocketLauncher = false, bool grenadeLauncher = false, bool stickybombLauncher = false, bool minigun = false, bool syringeGun = false, bool mediGun = false)
        {
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.autoReuse = true;
            usesFocusShot = focus;
            fullAutomatic = automatic;
            isRocketLauncher = rocketLauncher;
            isGrenadeLauncher = grenadeLauncher;
            isStickybombLauncher = stickybombLauncher;
            isMinigun = minigun;
            isSyringeGun = syringeGun;
            isMediGun = mediGun;
            if (isStickybombLauncher || isMinigun)
                usesAltClick = true;
            if (isMinigun || this is Wrangler)
                autoUseAltClick = true;
            if (isMediGun)
            {
                noAmmoConsumption = true;
                noAmmoClip = true;
            }
        }

        protected void SetSwingUseStyle(bool focus = false, bool sword = false, double scale = 100, bool noMelee = false)
        {
            Item.useStyle = !sword ? ItemUseStyleID.Swing : 15;
            Item.autoReuse = true;
            usesFocusShot = focus;
            Item.scale = !sword ? 1f : 2f;
            Item.scale *= (float)(scale / 100f);
            isSword = sword;
            Item.noMelee = noMelee;
        }

        protected void SetLungeUseStyle(bool knife = false)
        {
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            autoUseAltClick = true;
            isKnife = knife;
        }

        protected void SetThrowableUseStyle(bool focus = false)
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            noAmmoConsumption = true;
            usesFocusShot = focus;
        }

        protected void SetFoodUseStyle()
        {
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.noMelee = true;
            Item.autoReuse = true;
            noAmmoConsumption = true;
            noAmmoClip = true;
        }

        protected void SetDrinkUseStyle()
        {
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.noMelee = true;
            Item.autoReuse = true;
            noAmmoConsumption = true;
            noAmmoClip = true;
        }

        protected void SetBannerUseStyle()
        {
            Item.useStyle = 15;
            Item.noMelee = true;
            Item.autoReuse = true;
            noAmmoConsumption = true;
            noAmmoClip = true;
        }

        protected void SetPDAUseStyle()
        {
            Item.useStyle = ItemUseStyleID.None;
            Item.noMelee = true;
            noAmmoConsumption = true;
            noAmmoClip = true;
        }

        protected void SetWeaponDamage(double damage = 0, int projectile = ProjectileID.None, float projectileSpeed = StandardBulletSpeed, int projectileCount = 1, float shootAngle = 0f, float knockback = 0f, bool noRandomCriticalHits = false, bool distanceModifier = true)
        {
            damage = Utils.Clamp(damage, 0, int.MaxValue);
            bool isDamageAnInteger = Math.Abs(damage - (int)damage) < double.Epsilon;
            if (isDamageAnInteger)
                Item.damage = (int)damage;
            else
                decimalDamage = (float)damage;
            Item.shoot = projectile;
            Item.shootSpeed = projectileSpeed;
            projectileAmount = projectileCount;
            projectileAngle = shootAngle;
            Item.knockBack = knockback;
            noRandomCrits = noRandomCriticalHits;
            noDistanceModifier = !distanceModifier;
            if (isGrenadeLauncher)
                noDistanceModifier = true;
        }

        protected void SetWeaponAttackSpeed(double attackSpeed, double attackAnimation = 0f, bool hide = false)
        {
            fireRate = attackSpeed;
            if (Time(attackAnimation) <= 0)
                Item.useTime = Item.useAnimation = Time(attackSpeed);
            else
            {
                Item.useTime = Time(attackSpeed);
                Item.useAnimation = Time(attackAnimation);
            }
            hideAttackSpeed = hide;
        }

        protected void SetWeaponAttackSound(string attackSound, string hitSound = "")
        {
            Item.UseSound = new SoundStyle(attackSound);
            if (hitSound != "")
                meleeHitSound = new SoundStyle(hitSound);
        }

        protected void SetWeaponAttackSound(SoundStyle soundStyle) => Item.UseSound = soundStyle;

        protected void SetWeaponAttackIntervals(double reloadTime = 0, int maxAmmo = 0, int maxReserve = 0, string reloadSoundPath = "", int cost = 1, int currentAmmo = 0, double initialReloadTime = 0, double customReloadTime = 0, bool altClick = false, bool noAmmo = false, bool usesMagazine = false, double deploy = 0.5, double holster = 0.5, double cooldown = 0)
        {
            deploySpeed = Time(deploy);
            holsterSpeed = Time(holster);
            cooldownTimer = customReloadTime <= 0 ? Time(cooldown) : Time(customReloadTime);
            ammoCost = cost;
            WeaponSetAmmo(maxAmmo, maxReserve, currentAmmo);
            reloadRate = Time(reloadTime);
            initialReloadRate = Time(initialReloadTime);
            displayReloadSpeed = customReloadTime <= 0 ? reloadTime : customReloadTime;
            noAmmoClip = noAmmo;
            usesAltClick = altClick;
            magazine = usesMagazine;
            reloadSound = new SoundStyle(reloadSoundPath);
        }

        protected void SetUtilityWeapon(bool altClick = false, bool itemUseGraphic = true, double deploy = 0.5, double holster = 0.5)
        {
            deploySpeed = Time(deploy);
            holsterSpeed = Time(holster);
            usesAltClick = altClick;
            noDistanceModifier = true;
            noUseGraphic = !itemUseGraphic;
            noAmmoClip = true;
        }

        protected void SetFlamethrower(int cost = 20, int cooldown = 45, string attackSound = "TF2/Content/Sounds/SFX/Weapons/flame_thrower_loop")
        {
            isFlamethrower = true;
            airblastCost = cost;
            airblastCooldown = cooldown;
            flameThrowerAttackSound = new SoundStyle(attackSound);
        }

        protected void SetStickybombLauncher(int capacity = 0, double maxCharge = 0, double detonationTime = 0, float chargeRate = 1f, float interval = 1f, string attackSound = "TF2/Content/Sounds/SFX/Weapons/stickybomblauncher_shoot")
        {
            isStickybombLauncher = true;
            maxStickybombs = capacity;
            armTime = Time(detonationTime);
            maxChargeTime = Time(maxCharge);
            chargeUpRate = chargeRate;
            chargeInterval = interval;
            stickbombLauncherAttackSound = new SoundStyle(attackSound);
        }

        protected void SetMinigun(double spinTime, double speed, string spinSound = "TF2/Content/Sounds/SFX/Weapons/minigun_spin", string spinUpSound = "TF2/Content/Sounds/SFX/Weapons/minigun_wind_up", string spinDownSound = "TF2/Content/Sounds/SFX/Weapons/minigun_wind_down", string attackSound = "TF2/Content/Sounds/SFX/Weapons/minigun_shoot", string emptySound = "TF2/Content/Sounds/SFX/Weapons/minigun_empty")
        {
            isMinigun = true;
            spinUpTime = Time(spinTime);
            speedPercentage = speed;
            minigunSpinUpSound = new SoundStyle(spinUpSound);
            minigunSpinDownSound = new SoundStyle(spinDownSound);
            minigunSpinSound = new SoundStyle(spinSound)
            {
                IsLooped = true,
                SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest
            };
            minigunAttackSound = new SoundStyle(attackSound)
            {
                IsLooped = true,
                SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest
            };
            minigunEmptySound = new SoundStyle(emptySound)
            {
                IsLooped = true,
                SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest
            };
        }

        protected void SetMediGun(int buff, double duration, int capacity = 1000, string healSound = "TF2/Content/Sounds/SFX/Weapons/medigun_heal")
        {
            isMediGun = true;
            uberChargeBuff = buff;
            uberChargeDuration = Time(duration);
            uberChargeCapacity = capacity;
            noAmmoClip = true;
            mediGunHealSound = new SoundStyle(healSound);
        }

        protected void SetSniperRifle(double chargeDamage = 100, double maxChargeDuration = 2, double zoomDelay = 1.3, double speed = 27, float chargeRate = 1f, float interval = 1f)
        {
            isSniperRifle = true;
            additionalChargeUpDamage = chargeDamage;
            maxChargeTime = Time(maxChargeDuration);
            chargeUpDelayTimer = chargeUpDelay = Time(zoomDelay);
            speedPercentage = speed;
            chargeUpRate = chargeRate;
            chargeInterval = interval;
            noDistanceModifier = true;
        }

        private void AddStatisticTooltips(List<TooltipLine> tooltips)
        {
            float damageValue = decimalDamage == 0 ? Item.damage : decimalDamage;
            TooltipLine damage = new TooltipLine(Mod, "Damage",
                Language.GetText("Mods.TF2.UI.Items.Damage").Format(Main.LocalPlayer.GetDamage<MercenaryDamage>().ApplyTo(damageValue)))
            {
                OverrideColor = new Color(192, 192, 192)
            };
            if (damageValue > 0)
                tooltips.Add(damage);

            double attackSpeedValue = fireRate;
            TooltipLine attackSpeed = new TooltipLine(Mod, "Attack Speed",
                Language.GetText("Mods.TF2.UI.Items.AttackSpeed").Format(Math.Round(attackSpeedValue, 3)))
            {
                OverrideColor = new Color(192, 192, 192)
            };
            if (attackSpeedValue > 0 && !hideAttackSpeed)
                tooltips.Add(attackSpeed);

            double reloadSpeedValue = displayReloadSpeed;
            TooltipLine reloadSpeed = new TooltipLine(Mod, "Reload Speed",
                Language.GetText("Mods.TF2.UI.Items.ReloadSpeed").Format(Math.Round(reloadSpeedValue, 3)))
            {
                OverrideColor = new Color(192, 192, 192)
            };
            if (reloadSpeedValue > 0)
                tooltips.Add(reloadSpeed);
        }

        protected void AddSwordAttribute(List<TooltipLine> description)
        {
            string text = (string)this.GetLocalization("Sword");
            List<string> lines = TooltipSplitter(text);
            foreach (string tooltip in lines)
            {
                List<string> lines2 = StringSplitter(tooltip);
                for (int i = 0; i < lines2.Count; i++)
                {
                    TooltipLine line = new TooltipLine(Mod, "Sword Attribute", lines2[i])
                    {
                        OverrideColor = new Color(235, 226, 202)
                    };
                    description.Add(line);
                }
            }
        }

        public void ResetAmmo()
        {
            currentAmmoClip = maxAmmoClip;
            currentAmmoReserve = maxAmmoReserve;
        }

        public void ResetUseTime() => deployTimer = holsterTimer = 0;

        protected void UpdateAmmo(Player player)
        {
            if (reload)
                cooldownTimer++;
            else
                cooldownTimer = 0;
            if (reload)
            {
                ammoReloadRateTimer++;
                if (!isGrenadeLauncher && cooldownTimer >= (initialReloadRate - reloadRate) && !finishReloadSound)
                {
                    PlaySound(reloadSound, player.Center);
                    finishReloadSound = true;
                }
            }
            else
                ammoReloadRateTimer = 0;
            if (!startReloadSound && reload && isGrenadeLauncher)
            {
                PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/grenade_launcher_drum_open"), player.Center);
                startReloadSound = true;
            }
            if (reload && cooldownTimer >= (isGrenadeLauncher ? (initialReloadRate - reloadRate) : initialReloadRate))
            {
                if (isGrenadeLauncher && !finishReloadSound && startReloadSound)
                {
                    PlaySound(reloadSound, player.Center);
                    finishReloadSound = true;
                }
                if (ammoReloadRateTimer >= reloadRate && currentAmmoReserve > 0)
                {
                    if (magazine)
                    {
                        int ammo = (currentAmmoReserve >= maxAmmoClip) ? (maxAmmoClip - currentAmmoClip) : currentAmmoReserve;
                        currentAmmoClip = (currentAmmoReserve >= maxAmmoClip) ? maxAmmoClip : currentAmmoReserve;
                        currentAmmoReserve -= ammo;
                    }
                    else
                    {
                        currentAmmoClip++;
                        currentAmmoReserve--;
                        finishReloadSound = false;
                    }
                    ammoReloadRateTimer = 0;
                }
            }
            currentAmmoClip = Utils.Clamp(currentAmmoClip, 0, maxAmmoClip);
            if ((currentAmmoClip == maxAmmoClip) || currentAmmoReserve <= 0)
            {
                if (!finishReloadSound && reload && isGrenadeLauncher)
                    closeDrum = true;
                StopReload();
            }
            if (closeDrum && !reload && isGrenadeLauncher)
            {
                PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/grenade_launcher_drum_close"), player.Center);
                closeDrum = false;
            }
            if (!reload)
            {
                startReloadSound = false;
                finishReloadSound = false;
            }
            else
                spreadRecovery = Time(1.25);
        }

        public void StopReload() => reload = false;

        protected void EnforcePassiveEffects(Player player)
        {
            if (IsItemInHotbar(player, Item) && equipped)
                WeaponPassiveUpdate(player);
        }

        protected void UpdateDelay(Player player, bool resetDelayCondition)
        {
            deployTimer++;
            if (resetDelayCondition)
                holsterTimer = 0;
            if (deployTimer >= deploySpeed && !player.ItemAnimationActive)
                holsterTimer++;
        }

        protected void OutputDelay(Player player)
        {
            if (player.HeldItem != Item)
                ResetUseTime();
        }

        protected void Shoot(Player player, IEntitySource source, Vector2 position, Vector2 velocity, int type, int damage, float knockback, int projectileAmount = 1, float angle = 0f, int owner = -1)
        {
            bool randomCrit = (WeaponCriticalHits(player) && Item.useTime > Time(0.2)) || critDuration > 0;
            bool randomAmmo = WeaponAmmoBoxes(player);
            bool randomHealth = WeaponHealthKits(player);
            for (int i = 0; i < projectileAmount; i++)
            {
                Vector2 newVelocity = angle == 0f ? velocity : velocity.RotatedByRandom(MathHelper.ToRadians(angle));
                int projectile = Projectile.NewProjectile(source, position, newVelocity, type, damage, knockback, owner);
                TF2Projectile tf2Projectile = Main.projectile[projectile].ModProjectile as TF2Projectile;
                tf2Projectile.weapon = this;
                tf2Projectile.noDistanceModifier = noDistanceModifier;
                TF2Player p = player.GetModPlayer<TF2Player>();
                if (p.focus && usesFocusShot)
                {
                    tf2Projectile.homing = true;
                    tf2Projectile.shootSpeed = Item.shootSpeed;
                    Main.projectile[projectile].netUpdate = true;
                }
                if (randomCrit)
                    tf2Projectile.crit = true;
                if (p.crit)
                    tf2Projectile.crit = true;
                else if (p.miniCrit && !tf2Projectile.crit)
                    tf2Projectile.miniCrit = true;
                if (isSniperRifle && chargeTime == maxChargeTime)
                {
                    if (Item.ModItem is not SydneySleeper)
                    {
                        tf2Projectile.crit = true;
                        tf2Projectile.sniperCrit = true;
                    }
                    else
                    {
                        tf2Projectile.miniCrit = true;
                        tf2Projectile.sniperMiniCrit = true;
                    }
                    Main.projectile[projectile].netUpdate = true;
                }
                if (tf2Projectile is KnifeProjectile && p.backStab)
                {
                    tf2Projectile.backStab = true;
                    tf2Projectile.crit = true;
                }
                if (randomAmmo)
                    tf2Projectile.ammoShot = true;
                if (randomHealth)
                    tf2Projectile.healthShot = true;
                WeaponPostFireProjectile(player, projectile);
                Main.projectile[projectile].netUpdate = true;
            }
        }

        private void StickybombLauncherUpdate(Player player, int type)
        {
            chargeTime = Utils.Clamp(chargeTime, 0, maxChargeTime);
            TF2Player p = player.GetModPlayer<TF2Player>();
            if ((!player.controlUseItem || chargeTime == maxChargeTime) && isCharging && !player.dead && WeaponCanAltClick(player) && player.altFunctionUse != 2 && !rightClick)
            {
                Vector2 shootDirection = player.DirectionTo(Main.MouseWorld);
                reload = false;
                currentAmmoClip -= ammoCost;
                if (stickybombsAmount >= maxStickybombs)
                    RemoveOldestStickybomb(player);
                Vector2 newVelocity = new Vector2(shootDirection.X * Item.shootSpeed + chargeTime / 60, shootDirection.Y * Item.shootSpeed - chargeTime / 60);
                PlaySound(stickbombLauncherAttackSound, player.Center);
                int i = Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, newVelocity, type, (int)(Item.damage * p.damageMultiplier), Item.knockBack, player.whoAmI);
                Stickybomb projectile = (Stickybomb)Main.projectile[i].ModProjectile;
                projectile.weapon = this;
                NetMessage.SendData(MessageID.SyncProjectile, number: i);
                SetCustomItemTime(player);
                chargeTime = 0f;
                stickybombsAmount++;
                isCharging = false;
                if (currentAmmoClip <= 0 && currentAmmoReserve > 0)
                    reload = true;
            }
            if (player.controlUseTile && WeaponCanAltClick(player) && player.itemTime == 0 && !player.controlUseItem && !reload)
            {
                rightClick = true;
                SetCustomItemTime(player);
                StickybombLauncherDetonate(player);
                int direction = Main.MouseWorld.X > player.position.X ? 1 : -1;
                player.ChangeDir(direction);
                player.itemRotation = (Utils.DirectionTo(player.Center, Main.MouseWorld) * direction).ToRotation();
                PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/stickybomblauncher_detonate"), player.Center);
            }
            if (player.dead)
                rightClick = false;
            Minimum(ref stickybombsAmount, 0);
        }

        private void StickybombLauncherCharge()
        {
            chargeInterval++;
            if (chargeInterval >= chargeUpRate)
            {
                chargeTime++;
                chargeInterval = 0;
            }
            chargeTime = Utils.Clamp(chargeTime, 0, maxChargeTime);
        }

        private void RemoveOldestStickybomb(Player player)
        {
            int oldestTime = 0;
            Projectile oldestProjectile = null;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                Stickybomb stickybombProjectile = Main.projectile[i].ModProjectile as Stickybomb;
                if (projectile.active && projectile.owner == player.whoAmI && projectile.type == Item.shoot && stickybombProjectile.weapon == this && stickybombProjectile.Timer > oldestTime)
                {
                    oldestProjectile = Main.projectile[i];
                    oldestTime = stickybombProjectile.Timer;
                }
            }
            oldestProjectile.timeLeft = 0;
            stickybombsAmount--;
        }

        private void FlameThrowerActiveUpdate(Player player)
        {
            if (player.dead || !player.controlUseItem || currentAmmoClip <= 0)
            {
                if (SoundEngine.TryGetActiveSound(flameThrowerAttackSoundSlot, out var attackSound))
                    attackSound.Stop();
            }
        }

        private void FlameThrowerUpdate(Player player)
        {
            if (SoundEngine.TryGetActiveSound(flameThrowerAttackSoundSlot, out var attackSound))
                attackSound.Position = player.Center;
        }

        private void MinigunActiveUpdate(Player player)
        {
            if (player.dead || !WeaponCanAltClick(player))
            {
                if (SoundEngine.TryGetActiveSound(minigunSpinUpSoundSlot, out var spinUp))
                    spinUp.Stop();
                if (SoundEngine.TryGetActiveSound(minigunSpinDownSoundSlot, out var spinDown))
                    spinDown.Stop();
                if (SoundEngine.TryGetActiveSound(minigunSpinSoundSlot, out var spinSound))
                    spinSound.Stop();
                if (SoundEngine.TryGetActiveSound(minigunAttackSoundSlot, out var attackSound))
                    attackSound.Stop();
                if (SoundEngine.TryGetActiveSound(minigunEmptySoundSlot, out var emptySound))
                    emptySound.Stop();
                endSpinUpSound = endSpinDownSound = false;
                spinTimer = 0;
                return;
            }
            if (spinTimer >= spinUpTime && player.controlUseItem && player.altFunctionUse != 2)
            {
                if (currentAmmoClip > 0)
                {
                    if (SoundEngine.TryGetActiveSound(minigunSpinSoundSlot, out var spinSound))
                        spinSound.Stop();
                    if (SoundEngine.TryGetActiveSound(minigunEmptySoundSlot, out var emptySound))
                        emptySound.Stop();
                    if (!SoundEngine.TryGetActiveSound(minigunAttackSoundSlot, out var _))
                        minigunAttackSoundSlot = PlaySound(minigunAttackSound, player.Center);
                }
                else
                {
                    if (SoundEngine.TryGetActiveSound(minigunSpinSoundSlot, out var spinSound))
                        spinSound.Stop();
                    if (SoundEngine.TryGetActiveSound(minigunAttackSoundSlot, out var attackSound))
                        attackSound.Stop();
                    if (!SoundEngine.TryGetActiveSound(minigunEmptySoundSlot, out var _))
                        minigunEmptySoundSlot = PlaySound(minigunEmptySound, player.Center);
                }
            }
            else if (player.altFunctionUse == 2)
            {
                if (SoundEngine.TryGetActiveSound(minigunAttackSoundSlot, out var attackSound))
                    attackSound.Stop();
                if (SoundEngine.TryGetActiveSound(minigunEmptySoundSlot, out var emptySound))
                    emptySound.Stop();
            }
            if (player.controlUseItem || player.controlUseTile)
            {
                endSpinDownSound = false;
                if (!endSpinUpSound)
                {
                    if (SoundEngine.TryGetActiveSound(minigunSpinDownSoundSlot, out var spinDown))
                        spinDown.Stop();
                    if (!SoundEngine.TryGetActiveSound(minigunSpinUpSoundSlot, out var _))
                        minigunSpinUpSoundSlot = PlaySound(minigunSpinUpSound, player.Center);
                    endSpinUpSound = true;
                }
            }
            if (!player.controlUseItem && !player.controlUseTile)
            {
                if (spinTimer > 0)
                {
                    if (!endSpinDownSound)
                    {
                        if (!SoundEngine.TryGetActiveSound(minigunSpinDownSoundSlot, out var _))
                            minigunSpinDownSoundSlot = PlaySound(minigunSpinDownSound, player.Center);
                        endSpinDownSound = true;
                    }
                    spinTimer--;
                    player.itemAnimation = Item.useAnimation;
                }
                spinTimer = Utils.Clamp(spinTimer, 0, spinUpTime);
            }
            else
                spinTimer++;
        }

        protected void MinigunUpdate(Player player, double speedPercentage)
        {
            if (SoundEngine.TryGetActiveSound(minigunSpinSoundSlot, out var spinSound))
                spinSound.Position = player.Center;
            if (SoundEngine.TryGetActiveSound(minigunAttackSoundSlot, out var attackSound))
                attackSound.Position = player.Center;
            if (SoundEngine.TryGetActiveSound(minigunEmptySoundSlot, out var emptySound))
                emptySound.Position = player.Center;
            if (currentAmmoClip <= 0)
                attackSound?.Stop();
            if (player.itemAnimation > 0)
            {
                if (endSpinUpSound && !SoundEngine.TryGetActiveSound(minigunSpinUpSoundSlot, out var _) && !SoundEngine.TryGetActiveSound(minigunSpinSoundSlot, out var _) && !SoundEngine.TryGetActiveSound(minigunSpinDownSoundSlot, out var _) && !SoundEngine.TryGetActiveSound(minigunAttackSoundSlot, out var _) && !SoundEngine.TryGetActiveSound(minigunEmptySoundSlot, out var _))
                    minigunSpinSoundSlot = PlaySound(minigunSpinSound, player.position);
                TF2Player.SetPlayerSpeed(player, speedPercentage);
                if (!player.mount.Active && spinTimer > 0)
                {
                    player.jumpSpeedBoost = 0;
                    player.jumpSpeedBoost -= Player.jumpSpeed;
                }
            }
            if (!player.controlUseItem && !player.controlUseTile)
            {
                endSpinUpSound = false;
                if (SoundEngine.TryGetActiveSound(minigunSpinUpSoundSlot, out var spinUp))
                    spinUp.Stop();
                spinSound?.Stop();
                attackSound?.Stop();
                emptySound?.Stop();
            }
        }

        private void MediGunUpdate(Player player)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            p.uberCharge = uberCharge;
            p.maxUberCharge = uberChargeCapacity;
            if (!endHealSound && player.controlUseItem)
            {
                PlaySound(mediGunHealSound, player.Center);
                endHealSound = true;
            }
            if (uberCharge == uberChargeCapacity && uberChargeCapacity > 0 && !p.fullyCharged)
                PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/medigun_charged"), player.Center);
            lockWeapon = p.activateUberCharge;
        }

        private void MediGunPassiveUpdate(Player player)
        {
            uberCharge = Utils.Clamp(uberCharge, 0, uberChargeCapacity);
            if (!player.controlUseItem)
                endHealSound = false;
        }

        public void AddUberCharge(float rate = 1f) => uberCharge += rate;

        protected bool ActivateUberCharge(Player player)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (uberCharge >= uberChargeCapacity)
            {
                uberCharge = 0;
                p.activateUberCharge = true;
                p.fullyCharged = false;
                player.AddBuff(uberChargeBuff, uberChargeDuration, false);
                if (uberChargeBuff == ModContent.BuffType<QuickFixUberCharge>())
                    RemoveAllDebuffs(player);
                PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/invulnerable_on"), player.Center);
            }
            return false;
        }

        protected void SniperRifleActiveUpdate(Player player)
        {
            if (currentAmmoClip > 0 && chargeUpDelayTimer >= chargeUpDelay && !reload)
                player.scope = true;
            chargeTime = Round(Utils.Clamp(chargeTime, 0, maxChargeTime));
            chargeUpDelayTimer++;
            if (currentAmmoClip > 0 && player.controlUseTile)
                SniperRifleCharge();
            if (!player.controlUseTile)
            {
                chargeTime = 0f;
                isCharging = false;
            }
        }

        protected void SniperRifleUpdate(Player player, double speedPercentage)
        {
            if (isSniperRifle)
            {
                if (isCharging)
                {
                    TF2Player.SetPlayerSpeed(player, speedPercentage);
                    player.GetModPlayer<TF2Player>().disableFocusSlowdown = true;
                }
                chargeUpDamage = SniperRifleDamage(additionalChargeUpDamage);
            }
        }

        protected void SniperRifleCharge()
        {
            if (chargeUpDelayTimer >= chargeUpDelay && !reload)
            {
                isCharging = true;
                chargeInterval++;
                if (chargeInterval >= chargeUpRate)
                {
                    chargeTime++;
                    chargeInterval = 0f;
                }
                chargeTime = Utils.Clamp(chargeTime, 0, maxChargeTime);
            }
        }

        private int SniperRifleDamage(double damage) => (int)(Item.damage + damage * chargeTime / maxChargeTime);

        protected static int GetCustomItemTime(Player player) => player.itemTime;

        protected void SetCustomItemTime(Player player, int useTime = 0, int useAnimation = 0)
        {
            player.itemTime = useTime == 0 ? Item.useTime : useTime;
            player.itemAnimation = useAnimation == 0 ? Item.useAnimation : useAnimation;
        }

        protected void FlamethrowerProjectile(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                if (WeaponCanConsumeAmmo(player) && noAmmoClip)
                    currentAmmoClip -= ammoCost;
                Vector2 muzzleOffset = Vector2.Normalize(velocity) * 54f;
                if (Collision.CanHit(position, 6, 6, position + muzzleOffset, 6, 6))
                {
                    position += muzzleOffset;
                    if (!SoundEngine.TryGetActiveSound(flameThrowerAttackSoundSlot, out var _))
                        flameThrowerAttackSoundSlot = PlaySound(flameThrowerAttackSound, player.Center);
                    Shoot(player, source, position, velocity, type, damage, knockback, 1, 0f, player.whoAmI);
                }
            }
            else
            {
                if (WeaponCanConsumeAmmo(player))
                    currentAmmoClip -= airblastCost;
                player.itemAnimationMax = Item.useTime;
                PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/flame_thrower_airblast"), player.Center);
                Shoot(player, source, position, velocity, ModContent.ProjectileType<Airblast>(), 0, knockback, 1, 0f, player.whoAmI);
            }
        }

        public static bool CanSwitchWeapon(Player player)
        {
            if (player.HeldItem.ModItem is TF2Weapon)
            {
                TF2Weapon weapon = player.HeldItem.ModItem as TF2Weapon;
                return player.inventory[58] == weapon.Item || !weapon.equipped || (weapon.currentAmmoClip > 0 || weapon.currentAmmoReserve <= 0 || weapon.noAmmoClip) && weapon.holsterTimer >= weapon.holsterSpeed && !weapon.lockWeapon && !weapon.reload;
            }
            else return true;
        }

        public bool GetWeaponMechanic(string weapon) => weapon switch
        {
            "Flamethrower" => isFlamethrower,
            "Grenade Launcher" => isGrenadeLauncher,
            "Stickybomb Launcher" => isStickybombLauncher,
            "Minigun" => isMinigun,
            "Medi Gun" => isMediGun,
            "Sniper Rifle" => isSniperRifle,
            _ => false,
        };

        protected void DemomanMeleeCrit(Player player)
        {
            if (weaponType == Melee)
            {
                player.ClearBuff(ModContent.BuffType<ShieldBuff>());
                if (player.GetModPlayer<TF2Player>().crit && player.GetModPlayer<AliBabasWeeBootiesPlayer>().aliBabasWeeBootiesEquipped)
                {
                    ShieldPlayer shield = ShieldPlayer.GetShield(player);
                    shield.timer += Round(shield.ShieldRechargeTime * 0.25f);
                }
            }
        }

        public static bool HoldingWeapon<T>(Player player) => player.HeldItem.ModItem is T && player.inventory[58].ModItem is not T;

        public override sealed void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = WeaponResearchCost();
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = autoUseAltClick;
        }

        public override sealed void SetDefaults()
        {
            deploySpeed = holsterSpeed = Time(0.5);
            WeaponStatistics();
            ClampVariables();
            if (availability == Unlock)
                metalValue += Item.buyPrice(platinum: 1);
            Item.value = metalValue;
            Item.DamageType = ModContent.GetInstance<MercenaryDamage>();
            if (weaponType == Melee || maxAmmoClip <= 0)
                noAmmoClip = true;
            if (weaponType == Melee)
                noDistanceModifier = true;
            singleReloadShotMaxCooldown = initialReloadRate;
            if (ModContent.GetInstance<TF2ConfigClient>().InfiniteAmmo)
                ammoCost = 0;
        }

        public override sealed void ModifyTooltips(List<TooltipLine> tooltips)
        {
            AddName(tooltips);
            tooltips.Remove(tooltips.FirstOrDefault(x => x.Name == "ItemName" && x.Mod == "Terraria"));
            RemoveDefaultTooltips(tooltips);
            Player player = Main.LocalPlayer;
            WeaponEarlyUpdate(player);
            string weaponAvailability = !MannCoStoreActive ? availabilityNames[availability] : availabilityNames[5];
            string weaponCategory = categoryNames[weaponType];
            string category = Language.GetText("Mods.TF2.UI.Items.CategoryText").Format(classNames[classType], weaponAvailability, weaponCategory);
            if (classType == MultiClass)
                category = HasClass(Main.LocalPlayer.GetModPlayer<TF2Player>().currentClass) ? Language.GetText("Mods.TF2.UI.Items.CategoryText").Format(classNames[Main.LocalPlayer.GetModPlayer<TF2Player>().currentClass], weaponAvailability, weaponCategory) : Language.GetText("Mods.TF2.UI.Items.CategoryText").Format(Language.GetTextValue("Mods.TF2.UI.Items.MultiClass"), weaponAvailability, weaponCategory);
            tooltips.Insert(tooltips.FindLastIndex(x => x.Name == "Name" && x.Mod == "TF2") + 1, new TooltipLine(Mod, "Weapon Category", category)
            {
                OverrideColor = new Color(117, 107, 94)
            });
            AddStatisticTooltips(tooltips);
            WeaponDescription(tooltips);
            if (Item.favorited)
            {
                tooltips.Add(new TooltipLine(Mod, "Favorite", FontAssets.MouseText.Value.CreateWrappedText(Lang.tip[56].Value, 350f))
                {
                    OverrideColor = new Color(235, 226, 202)
                });
                tooltips.Add(new TooltipLine(Mod, "Favorite Description", FontAssets.MouseText.Value.CreateWrappedText(Lang.tip[57].Value, 350f))
                {
                    OverrideColor = new Color(235, 226, 202)
                });
                if (Main.LocalPlayer.chest > -1)
                {
                    ChestUI.GetContainerUsageInfo(out bool sync, out Item[] chestinv);
                    if (ChestUI.IsBlockedFromTransferIntoChest(Item, chestinv))
                    {
                        TooltipLine noTransfer = new TooltipLine(Mod, "No Transfer", FontAssets.MouseText.Value.CreateWrappedText(Language.GetTextValue("UI.ItemCannotBePlacedInsideItself"), 350f))
                        {
                            OverrideColor = new Color(235, 226, 202)
                        };
                        tooltips.Add(new TooltipLine(Mod, "Favorite", FontAssets.MouseText.Value.CreateWrappedText(Lang.tip[56].Value, 350f))
                        {
                            OverrideColor = new Color(235, 226, 202)
                        });
                    }
                }
            }
            TooltipLine priceTooltip = tooltips.FirstOrDefault(x => x.Name == "Price" && x.Mod == "Terraria");
            TooltipLine price = priceTooltip;
            tooltips.Add(price);
            tooltips.Remove(priceTooltip);
            TooltipLine specialPriceTooltip = tooltips.FirstOrDefault(x => x.Name == "SpecialPrice" && x.Mod == "Terraria");
            TooltipLine specialPrice = specialPriceTooltip;
            tooltips.Add(specialPrice);
            tooltips.Remove(specialPriceTooltip);
            TooltipLine journeyResearchTooltip = tooltips.FirstOrDefault(x => x.Name == "JourneyResearch" && x.Mod == "Terraria");
            TooltipLine journeyModeTooltip = journeyResearchTooltip;
            tooltips.Add(journeyModeTooltip);
            tooltips.Remove(journeyResearchTooltip);
        }

        public override sealed void ModifyWeaponCrit(Player player, ref float crit) => crit = 0;

        public override sealed void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (Main.myPlayer == player.whoAmI)
                WeaponAttackAnimation(player);
        }

        public override sealed bool CanUseItem(Player player)
        {
            if (player.whoAmI != Main.myPlayer || !equipped || Item == player.inventory[58] || reload && !noAmmoClip && !ModContent.GetInstance<TF2ConfigClient>().SingleReload || isSniperRifle && maxAmmoReserve <= 0 && currentAmmoClip <= 0) return false;
            if (!noAmmoClip && ModContent.GetInstance<TF2ConfigClient>().SingleReload)
            {
                if (singleReloadShotCooldown < singleReloadShotMaxCooldown && !magazine && singleReloadShotReload)
                    return false;
            }
            if (deployTimer >= deploySpeed)
                return WeaponCanBeUsed(player);
            else return false;
        }

        public override sealed void HoldItem(Player player)
        {
            if (!equipped || player.whoAmI != Main.myPlayer) return;
            if (!noAmmoClip && ModContent.GetInstance<TF2ConfigClient>().SingleReload)
            {
                if (currentAmmoClip <= 0 && currentAmmoReserve > 0)
                    singleReloadShotReload = true;
                if (singleReloadShotCooldown < initialReloadRate)
                {
                    singleReloadShotCooldown++;
                    cooldownTimer = 0;
                }
            }
            UpdateDelay(player, WeaponResetHolsterTimeCondition(player));
            WeaponUpdateAmmo(player);
            if (fullAutomatic)
                spreadRecovery++;
            if (isFlamethrower)
                FlameThrowerActiveUpdate(player);
            if (isStickybombLauncher)
                StickybombLauncherUpdate(player, Item.shoot);
            if (isMinigun)
                MinigunActiveUpdate(player);
            if (isMediGun)
                MediGunUpdate(player);
            if (isSniperRifle && Item.ModItem is not Huntsman)
                SniperRifleActiveUpdate(player);
            WeaponActiveUpdate(player);
            critTimer++;
            if (Item.useTime <= Time(0.2) && critTimer >= Time(1) && critDuration <= 0)
            {
                WeaponCriticalHits(player);
                critTimer = 0;
            }
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (weaponType == Primary && currentAmmoReserve > Round(maxAmmoReserve * p.primaryAmmoMultiplier))
                currentAmmoReserve = Round(maxAmmoReserve * p.primaryAmmoMultiplier);
            else if (weaponType == Secondary && currentAmmoReserve > Round(maxAmmoReserve * p.secondaryAmmoMultiplier))
                currentAmmoReserve = Round(maxAmmoReserve * p.secondaryAmmoMultiplier);
        }

        public override sealed void UpdateInventory(Player player)
        {
            WeaponEquip(player);
            if (!equipped || player.whoAmI != Main.myPlayer) return;
            if (!weaponInitialized)
            {
                WeaponPostWeaponStatistics(player);
                weaponInitialized = true;
            }
            EnforcePassiveEffects(player);
            if (player.HeldItem == Item)
                WeaponActiveBonus(player);
            if (isMediGun)
                MediGunPassiveUpdate(player);
            OutputDelay(player);
            if (critDuration > 0 && !reload)
                critDuration--;
        }

        protected override bool WeaponModifyHealthCondition(Player player) => IsItemTypeInHotbar(player, Item.type);

        public override sealed bool AltFunctionUse(Player player) => usesAltClick && WeaponCanAltClick(player);

        public override sealed bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Item.shoot != ProjectileID.None)
            {
                int damageValue = decimalDamage == 0 ? Item.damage : (int)Math.Round(decimalDamage);
                WeaponAttack(player, source, position, velocity, Item.shoot, (int)player.GetDamage<MercenaryDamage>().ApplyTo(damageValue), knockback);
                WeaponPostAttack(player);
            }
            if (ModContent.GetInstance<TF2ConfigClient>().SingleReload)
            {
                singleReloadShotCooldown = 0;
                singleReloadShotReload = false;
            }
            return false;
        }

        public sealed override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox) => WeaponMeleeHitbox(player, ref hitbox, ref noHitbox);

        public sealed override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            PlaySound(meleeHitSound, target.Center);
            if (weaponType == Melee)
            {
                TF2Player p = player.GetModPlayer<TF2Player>();
                if (p.critMelee)
                    p.crit = true;
            }
            WeaponHitNPC(player, target, ref modifiers);
            DemomanMeleeCrit(player);
        }

        public sealed override void ModifyHitPvp(Player player, Player target, ref Player.HurtModifiers modifiers)
        {
            if (meleeHitSound != null)
                PlaySound(meleeHitSound, target.Center);
            if (weaponType == Melee)
            {
                TF2Player p = player.GetModPlayer<TF2Player>();
                if (p.critMelee)
                    p.crit = true;
            }
            WeaponHitPlayer(player, target, ref modifiers);
            DemomanMeleeCrit(player);
        }

        public override sealed bool? UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                if (isStickybombLauncher)
                {
                    if (player.controlUseTile)
                    {
                        holsterTimer = 0;
                        return true;
                    }
                    else if (player.controlUseItem && GetCustomItemTime(player) == 0)
                    {
                        isCharging = true;
                        rightClick = false;
                        StickybombLauncherCharge();
                        holsterTimer = 0;
                        if (chargeTime == maxChargeTime) return WeaponOnUse(player);
                        return false;
                    }
                    else
                    {
                        holsterTimer = 0;
                        return WeaponOnUse(player);
                    }
                }
                holsterTimer = 0;
                return WeaponOnUse(player);
            }
            return base.UseItem(player);
        }
    }
}