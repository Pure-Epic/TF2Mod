using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items.Ammo;
using TF2.Content.Items.Medic;
using TF2.Content.Items.Sniper;
using TF2.Content.Projectiles;
using TF2.Content.Projectiles.Demoman;
using TF2.Content.Projectiles.Pyro;
using static TF2.TF2;

namespace TF2.Content.Items
{
    public abstract class TF2Weapon : TF2Item
    {
        protected float decimalDamage;
        protected int projectileAmount;
        protected float projectileAngle;
        protected int ammoCost;
        private double fireRate;
        protected double displayReloadSpeed;
        public int currentAmmoClip; // protected
        protected int maxAmmoClip; // protected
        public bool noRandomCrits;
        protected int deploySpeed;
        public int holsterSpeed; // protected
        protected int reloadRate;
        public int initialReloadRate; // protected
        protected bool magazine;
        public bool noAmmoClip;
        protected bool noAmmoConsumption;
        protected bool usesAltClick;
        protected bool usesFocusShot;
        protected bool fullAutomatic;
        public SoundStyle reloadSound;

        protected WeaponSize meleeWeaponSize = new WeaponSize(50, 50);
        private Vector2 offset;
        public int cooldownTimer; // protected
        protected int deployTimer;
        protected int holsterTimer;
        public bool lockWeapon;
        protected bool reload;
        protected int ammoReloadRateTimer;
        protected bool isCharging;
        protected bool hideAttackSpeed;
        // private bool isFlamethrower;
        protected int airblastCooldown;
        private bool isGrenadeLauncher;
        protected bool startReloadSound;
        protected bool finishReloadSound;
        protected bool closeDrum;
        private bool isStickybombLauncher;
        protected int stickybombsAmount;
        protected int maxStickybombs;
        protected int armTime;
        protected float chargeTime;
        protected float maxChargeUp;
        protected float chargeInterval;
        protected SoundStyle stickbombLauncherAttackSound;
        protected float chargeUpRate;
        private bool isMinigun;
        protected int spinUpTime;
        protected int spinTimer;
        protected int minigunBulletCounter;
        protected int sandvichItem;
        protected bool eatingSandvich;
        private bool isMediGun;
        protected float uberCharge;
        protected int uberChargeCapacity;
        private int uberChargeBuff;
        private int uberChargeDuration;
        private bool isSniperRifle;
        protected double chargeUpDamage;
        protected double additionalChargeUpDamage;
        protected float chargeUpDelay;
        protected float chargeUpDelayTimer;
        private bool isKnife;
        protected double speedPercentage;
        protected bool rightClick;
        protected bool autoUseAltClick;
        protected int spreadRecovery;
        public int singleReloadShotCooldown;
        public int singleReloadShotMaxCooldown;
        public bool singleReloadShotReload;
        protected const float StandardBulletSpeed = 10f;

        protected virtual void WeaponAttackAnimation(Player player) => Item.noUseGraphic = (player.GetModPlayer<TF2Player>().classAccessory && !player.GetModPlayer<TF2Player>().classHideVanity && weaponType != Melee) || isKnife ||
            (
                Item.ModItem is Scout.MadMilk ||
                Item.ModItem is Jarate ||
                Item.ModItem is Spy.Sapper
            );

        protected virtual void WeaponSetAmmo(int maxAmmo, int currentAmmo = 0) => currentAmmoClip = maxAmmoClip = maxAmmo;

        public virtual bool WeaponCanBeUsed(Player player) => !magazine ? (currentAmmoClip > 0 && currentAmmoClip >= ammoCost) || player.HeldItem.ModItem is TF2WeaponNoAmmo || noAmmoClip : WeaponMagazineCanBeUsed(player);

        protected virtual bool WeaponMagazineCanBeUsed(Player player) => !(currentAmmoClip <= 0 || currentAmmoClip < ammoCost || reload);

        protected virtual bool WeaponCanAltClick(Player player) => isMediGun ? ActivateUberCharge(player) : !player.HasBuff<BuffaloSteakSandvichBuff>() || weaponType == Melee;

        protected virtual bool WeaponCanConsumeAmmo(Player player)
        {
            if (isStickybombLauncher)
                return stickybombsAmount < maxStickybombs && ModContent.GetInstance<TF2ConfigClient>().Channel;
            else if (isMinigun)
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

        protected virtual bool WeaponResetHolsterTimeCondition(Player player) => false;

        protected virtual void WeaponActiveUpdate(Player player)
        { }

        protected virtual void WeaponActiveBonus(Player player)
        {
            if (isMinigun)
                MinigunUpdate(player, speedPercentage);
            if (isSniperRifle)
                SniperRifleUpdate(player, speedPercentage);
        }

        protected virtual void WeaponUpdateAmmo(Player player)
        {
            if (!noAmmoClip)
            {
                AmmoInterface clip = player.GetModPlayer<AmmoInterface>();
                clip.ammoCurrent = currentAmmoClip;
                clip.ammoMax = maxAmmoClip;
                if (currentAmmoClip <= 0 && !noAmmoClip && player.itemAnimation == 0)
                    reload = true;
                if (clip.startReload)
                {
                    reload = true;
                    clip.startReload = false;
                }
                UpdateAmmo(player);
            }
        }

        protected virtual void WeaponPassiveUpdate(Player player)
        { }

        protected virtual bool WeaponPreAttack(Player player)
        {
            if (isMinigun)
            {
                if (spinTimer >= spinUpTime && player.controlUseItem)
                {
                    minigunBulletCounter++;
                    SoundEngine.PlaySound(SoundID.Item11, player.Center);
                }
                else
                {
                    SoundEngine.PlaySound(SoundID.Item13, player.Center);
                    return false;
                }
            }
            return true;
        }

        protected virtual void WeaponAttack(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!isStickybombLauncher && !isKnife)
            {
                if (((currentAmmoClip <= 0 && player.ItemAnimationEndingOrEnded || (reload && magazine)) && !noAmmoClip) || !WeaponPreAttack(player)) return;
                if (!magazine)
                    reload = false;
                if (!noAmmoClip && WeaponCanConsumeAmmo(player))
                    currentAmmoClip -= ammoCost;
                Vector2 newVelocity = fullAutomatic && spreadRecovery >= 75 ? velocity : velocity.RotatedByRandom(MathHelper.ToRadians(2.5f));
                int newDamage = !isSniperRifle ? damage : (int)Math.Round(chargeUpDamage * player.GetModPlayer<TF2Player>().classMultiplier);
                WeaponFireProjectile(player, source, position, newVelocity, type, newDamage, knockback);
                if (fullAutomatic)
                    spreadRecovery = 0;
                if (isSniperRifle)
                {
                    bool isDamageAnInteger = decimalDamage == 0;
                    chargeUpDamage = isDamageAnInteger ? Item.damage : decimalDamage;
                    chargeTime = 0f;
                    chargeUpDelayTimer = 0;
                }
            }
            else if (isStickybombLauncher)
            {
                if (!ModContent.GetInstance<TF2ConfigClient>().Channel) return;
                if (stickybombsAmount < maxStickybombs && player.altFunctionUse != 2)
                {
                    if (currentAmmoClip <= 0) return;
                    reload = false;
                    currentAmmoClip -= ammoCost;

                    Vector2 newVelocity = new Vector2(velocity.X + chargeTime / 60, velocity.Y - chargeTime / 60);
                    SoundEngine.PlaySound(stickbombLauncherAttackSound, player.Center);
                    int i = Projectile.NewProjectile(source, position, newVelocity, ModContent.ProjectileType<Stickybomb>(), damage, knockback, player.whoAmI);
                    Stickybomb projectile = (Stickybomb)Main.projectile[i].ModProjectile;
                    projectile.owner = this;
                    NetMessage.SendData(MessageID.SyncProjectile, number: i);
                    chargeTime = 0f;
                    stickybombsAmount += 1;
                    rightClick = false;
                }
            }
            else if (isKnife)
            {
                TF2Player p = player.GetModPlayer<TF2Player>();
                // int newDamage = !player.controlUseTile ? damage : (int)Math.Round(damage * p.classMultiplier * 3);
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                if (player.controlUseTile)
                {
                    p.backStab = true;
                    player.velocity = velocity * 12.5f;
                    player.immuneTime += 24;
                }
            }
        }

        protected virtual void WeaponFireProjectile(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback = 0f) => FocusShot(player, source, position, velocity, type, damage, knockback, projectileAmount, projectileAngle, player.whoAmI);

        // Rename
        protected void FocusShot(Player player, IEntitySource source, Vector2 position, Vector2 velocity, int type, int damage, float knockback, int projectileAmount = 1, float angle = 0f, int owner = -1)
        {
            for (int i = 0; i < projectileAmount; i++)
            {
                Vector2 newVelocity = angle == 0f ? velocity : velocity.RotatedByRandom(MathHelper.ToRadians(angle));
                int projectile = Projectile.NewProjectile(source, position, newVelocity, type, damage, knockback, owner);
                if (player.GetModPlayer<TF2Player>().focus && usesFocusShot)
                {
                    Main.projectile[projectile].GetGlobalProjectile<TF2ProjectileBase>().homing = true;
                    Main.projectile[projectile].GetGlobalProjectile<TF2ProjectileBase>().shootSpeed = Item.shootSpeed;
                    Main.projectile[projectile].netUpdate = true;
                }
                if (isSniperRifle && chargeTime == maxChargeUp)
                {
                    if (Item.ModItem is not SydneySleeper)
                        Main.projectile[projectile].GetGlobalProjectile<TF2ProjectileBase>().sniperCrit = true;
                    else
                        Main.projectile[projectile].GetGlobalProjectile<TF2ProjectileBase>().sniperMiniCrit = true;
                    Main.projectile[projectile].netUpdate = true;
                }
                WeaponPostFireProjectile(player, projectile);
            }
        }

        protected virtual void WeaponPostFireProjectile(Player player, int projectile)
        { }

        protected virtual void WeaponPostAttack(Player player)
        { }

        protected virtual bool? WeaponOnUse(Player player) => null;

        protected void SetWeaponSize(int width = 25, int height = 25) => Item.Size = weaponType != Melee ? (Vector2)new WeaponSize(width, height) : (Vector2)WeaponSize.MeleeWeaponSize;

        protected void SetWeaponOffset(float x = 0f, float y = 0f) => offset = new Vector2(x, y);

        protected void SetGunUseStyle(bool focus = false, bool automatic = false, bool grenadeLauncher = false, bool stickybombLauncher = false, bool minigun = false, bool mediGun = false)
        {
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.autoReuse = true;
            usesFocusShot = focus;
            fullAutomatic = automatic;
            isGrenadeLauncher = grenadeLauncher;
            isStickybombLauncher = stickybombLauncher;
            isMinigun = minigun;
            isMediGun = mediGun;
            if (isStickybombLauncher || isMinigun)
                usesAltClick = true;
            if (isMinigun)
                autoUseAltClick = true;
            if (isMediGun)
            {
                noAmmoConsumption = true;
                noAmmoClip = true;
            }
        }

        protected void SetSwingUseStyle(bool focus = false, bool sword = false)
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.autoReuse = true;
            usesFocusShot = focus;
            Item.scale = !sword ? 1f : 2f;
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

        protected void SetPDAUseStyle()
        {
            Item.useStyle = ItemUseStyleID.Thrust;
            Item.noMelee = true;
            noAmmoConsumption = true;
            noAmmoClip = true;
        }

        protected void SetWeaponDamage(double damage = 0, int projectile = ProjectileID.None, float projectileSpeed = StandardBulletSpeed, int projectileCount = 1, float shootAngle = 0f, float knockback = 0f, bool noRandomCriticalHits = false)
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
            if (!noAmmoConsumption)
            {
                switch (weaponType)
                {
                    case Primary:
                        Item.useAmmo = ModContent.ItemType<PrimaryAmmo>();
                        break;
                    case Secondary:
                        Item.useAmmo = ModContent.ItemType<SecondaryAmmo>();
                        break;
                    default:
                        break;
                }
            }
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

        protected void SetWeaponAttackSound(string attackSound) => Item.UseSound = new SoundStyle(attackSound);

        protected void SetWeaponAttackSound(SoundStyle soundStyle) => Item.UseSound = soundStyle;

        protected void SetWeaponAttackIntervals(double reloadTime = 0, int maxAmmo = 0, string reloadSoundPath = "", int cost = 1, int currentAmmo = 0, double initialReloadTime = 0, double customReloadTime = 0, bool altClick = false, bool noAmmo = false, bool usesMagazine = false, double deploy = 0, double holster = 0, double cooldown = 0)
        {
            deploySpeed = Time(deploy);
            holsterSpeed = Time(holster);
            cooldownTimer = customReloadTime <= 0 ? Time(cooldown) : Time(customReloadTime);
            ammoCost = cost;
            WeaponSetAmmo(maxAmmo, currentAmmo);
            reloadRate = Time(reloadTime);
            initialReloadRate = Time(initialReloadTime);
            displayReloadSpeed = customReloadTime <= 0 ? reloadTime : customReloadTime;
            noAmmoClip = noAmmo;
            usesAltClick = altClick;
            magazine = usesMagazine;
            reloadSound = new SoundStyle(reloadSoundPath);
        }

        protected void SetUtilityWeapon(bool altClick = false, int deploy = 0, int holster = 0)
        {
            deploySpeed = deploy;
            holsterSpeed = holster;
            usesAltClick = altClick;
            noAmmoClip = true;
        }

        protected void SetStickybombLauncher(int capacity = 0, double maxCharge = 0, double detonationTime = 0, float chargeRate = 1f, float interval = 1f, string attackSound = "TF2/Content/Sounds/SFX/stickybomblauncher_shoot")
        {
            isStickybombLauncher = true;
            maxStickybombs = capacity;
            armTime = Time(detonationTime);
            maxChargeUp = Time(maxCharge);
            chargeUpRate = chargeRate;
            chargeInterval = interval;
            stickbombLauncherAttackSound = new SoundStyle(attackSound);
        }

        protected void SetMinigun(double spinTime, double speed, string attackSound = "TF2/Content/Sounds/SFX/minigun_shoot")
        {
            isMinigun = true;
            spinUpTime = Time(spinTime);
            speedPercentage = speed;
            // minigunAttackSound = new SoundStyle(attackSound);
        }

        protected void SetMediGun(int buff, double duration, int capacity = 1000)
        {
            isMediGun = true;
            uberChargeBuff = buff;
            uberChargeDuration = Time(duration);
            uberChargeCapacity = capacity;
            noAmmoClip = true;
        }

        protected void SetSniperRifle(double chargeDamage = 100, double maxChargeTime = 2, double zoomDelay = 1.3, double speed = 27, float chargeRate = 1f, float interval = 1f)
        {
            isSniperRifle = true;
            additionalChargeUpDamage = chargeDamage;
            maxChargeUp = Time(maxChargeTime);
            chargeUpDelayTimer = chargeUpDelay = Time(zoomDelay);
            speedPercentage = speed;
            chargeUpRate = chargeRate;
            chargeInterval = interval;
        }

        private void AddStatisticTooltips(List<TooltipLine> tooltips)
        {
            float damageValue = decimalDamage == 0 ? Item.damage : decimalDamage;
            TooltipLine damage = new TooltipLine(Mod, "Damage",
                "Damage: " + Main.LocalPlayer.GetDamage<TF2DamageClass>().ApplyTo(damageValue))
            {
                OverrideColor = new Color(192, 192, 192)
            };
            if (damageValue > 0)
                tooltips.Add(damage);

            double attackSpeedValue = fireRate;
            TooltipLine attackSpeed = new TooltipLine(Mod, "Attack Speed",
                "Attack Speed: " + Math.Round(attackSpeedValue, 3))
            {
                OverrideColor = new Color(192, 192, 192)
            };
            if (attackSpeedValue > 0 && !hideAttackSpeed)
                tooltips.Add(attackSpeed);

            double reloadSpeedValue = displayReloadSpeed;
            TooltipLine reloadSpeed = new TooltipLine(Mod, "Reload Speed",
                "Reload Speed: " + Math.Round(reloadSpeedValue, 3))
            {
                OverrideColor = new Color(192, 192, 192)
            };
            if (reloadSpeedValue > 0)
                tooltips.Add(reloadSpeed);
        }

        protected void AddSwordAttribute(List<TooltipLine> description)
        {
            string text = (string)this.GetLocalization("Sword");
            TooltipLine line = new TooltipLine(Mod, "Sword Attribute", text)
            {
                OverrideColor = new Color(235, 226, 202)
            };
            description.Add(line);
        }

        public void ResetAmmo() => currentAmmoClip = maxAmmoClip;

        public void ResetUseTime() => deployTimer = holsterTimer = 0;

        protected void UpdateAmmo(Player player)
        {
            if (reload)
                cooldownTimer++;
            else
                cooldownTimer = 0;

            if (!startReloadSound && reload && isGrenadeLauncher)
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/grenade_launcher_drum_open"), Main.LocalPlayer.Center);
                startReloadSound = true;
            }

            if (reload)
            {
                if (cooldownTimer < (isGrenadeLauncher ? initialReloadRate - reloadRate : initialReloadRate)) return;

                if (!isGrenadeLauncher ? !finishReloadSound : !finishReloadSound && startReloadSound)
                {
                    SoundEngine.PlaySound(reloadSound, player.Center);
                    finishReloadSound = true;
                }

                if (ammoReloadRateTimer >= reloadRate)
                {
                    if (magazine)
                        currentAmmoClip = maxAmmoClip;
                    else
                    {
                        currentAmmoClip += 1;
                        finishReloadSound = false;
                    }
                    ammoReloadRateTimer = 0;
                }
            }

            if (reload)
                ammoReloadRateTimer++;
            else
                ammoReloadRateTimer = 0;

            currentAmmoClip = Utils.Clamp(currentAmmoClip, 0, maxAmmoClip);

            if (currentAmmoClip == maxAmmoClip)
            {
                if (!finishReloadSound && reload && isGrenadeLauncher)
                    closeDrum = true;
                StopReload();
            }
            if (closeDrum && !reload && isGrenadeLauncher)
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/grenade_launcher_drum_close"), Main.LocalPlayer.Center);
                closeDrum = false;
            }

            if (!reload)
            {
                startReloadSound = false;
                finishReloadSound = false;
            }
            else
                spreadRecovery = 75;
        }

        public void StopReload() => reload = false;

        protected void EnforcePassiveEffects(Player player)
        {
            if (!IsItemInHotbar(player, Item) && !ModContent.GetInstance<TF2ConfigClient>().InventoryStats) return;
            WeaponPassiveUpdate(player);
        }

        protected static void SetPlayerHealth(Player player, double percentage) => player.statLifeMax2 = (int)Math.Round(player.statLifeMax2 * (float)(percentage / 100f));

        public static void SetPlayerSpeed(Player player, double percentage)
        {
            ref float speed = ref player.GetModPlayer<TF2Player>().speedMultiplier;
            player.moveSpeed *= (float)(percentage / 100f);
            speed *= (float)(percentage / 100f);
        }

        // Convert to virtual
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

        private void StickybombLauncherUpdate(Player player, int type)
        {
            if (ModContent.GetInstance<TF2ConfigClient>().Channel)
                StickybombLauncherCharge();
            chargeTime = Utils.Clamp(chargeTime, 0, maxChargeUp);
            float maxCharge = !ModContent.GetInstance<TF2ConfigClient>().Channel ? maxChargeUp : maxChargeUp + Item.useTime;
            TF2Player p = player.GetModPlayer<TF2Player>();
            p.stickybombMaxCharge = maxCharge;
            p.stickybombCharge = chargeTime;
            p.stickybombChargeTimer = chargeInterval;
            p.stickybombAmount = stickybombsAmount;
            p.stickybombMax = maxStickybombs;

            if ((!player.controlUseItem || chargeTime == maxCharge) && isCharging && !ModContent.GetInstance<TF2ConfigClient>().Channel && !player.dead && WeaponCanAltClick(player))
            {
                if (stickybombsAmount < maxStickybombs && player.altFunctionUse != 2 && !rightClick)
                {
                    Vector2 shootDirection = player.DirectionTo(Main.MouseWorld);
                    reload = false;
                    currentAmmoClip -= ammoCost;

                    Vector2 newVelocity = new Vector2(shootDirection.X * Item.shootSpeed + chargeTime / 60, shootDirection.Y * Item.shootSpeed - chargeTime / 60);
                    SoundEngine.PlaySound(stickbombLauncherAttackSound, player.Center);
                    int i = Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, newVelocity, type, (int)(Item.damage * p.classMultiplier), Item.knockBack, player.whoAmI);
                    Stickybomb projectile = (Stickybomb)Main.projectile[i].ModProjectile;
                    projectile.owner = this;
                    NetMessage.SendData(MessageID.SyncProjectile, number: i);

                    SetCustomItemTime(player);
                    chargeTime = 0f;
                    stickybombsAmount += 1;
                    isCharging = false;
                    ChargeWeaponConsumeAmmo(player);

                    if (currentAmmoClip <= 0)
                        reload = true;
                }
            }

            if (player.controlUseTile && WeaponCanAltClick(player))
            {
                if (player.controlUseItem || reload) return;
                rightClick = true;
                SetCustomItemTime(player);
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile projectile = Main.projectile[i];
                    Stickybomb stickybombProjectile = Main.projectile[i].ModProjectile as Stickybomb;
                    if (projectile.active && projectile.owner == player.whoAmI && projectile.type == Item.shoot && stickybombProjectile.owner == this && stickybombProjectile.Timer >= armTime)
                    {
                        projectile.timeLeft = 0;
                        stickybombsAmount--;
                    }
                }
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/stickybomblauncher_det"), player.Center);
            }

            if (player.dead)
                rightClick = false;

            if (stickybombsAmount < 0)
                stickybombsAmount = 0;
        }

        private void StickybombLauncherCharge()
        {
            if (stickybombsAmount == maxStickybombs) return;
            chargeInterval++;
            float maxCharge = !ModContent.GetInstance<TF2ConfigClient>().Channel ? maxChargeUp : maxChargeUp + Item.useTime;
            if (chargeInterval >= chargeUpRate)
            {
                chargeTime += 1f;
                chargeInterval = 0;
            }
            chargeTime = Utils.Clamp(chargeTime, 0, maxCharge);
        }

        private void MinigunActiveUpdate(Player player)
        {
            if (player.dead || !WeaponCanAltClick(player))
            {
                spinTimer = 0;
                return;
            }

            if (!player.controlUseItem && !player.controlUseTile)
            {
                if (spinTimer > 0)
                {
                    spinTimer--;
                    player.itemAnimation = Item.useAnimation;
                }
                spinTimer = Utils.Clamp(spinTimer, 0, spinUpTime);
            }
            else
                spinTimer++;
        }

        protected static void MinigunUpdate(Player player, double speedPercentage)
        {
            if (player.itemAnimation > 0)
            {
                SetPlayerSpeed(player, speedPercentage);
                if (!player.mount.Active)
                    player.controlJump = false;
            }
        }

        private void MediGunUpdate(Player player)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            p.uberCharge = uberCharge;
            p.maxUberCharge = uberChargeCapacity;
        }

        private void MediGunPassiveUpdate(Player player)
        {
            uberCharge = Utils.Clamp(uberCharge, 0, uberChargeCapacity);
            if (player.GetModPlayer<UbersawPlayer>().ubersawHit)
                uberCharge += uberChargeCapacity / 4;
            if (player.GetModPlayer<VitaSawPlayer>().giveUberChargeFromVitaSaw)
                uberCharge = player.GetModPlayer<VitaSawPlayer>().deathUberCharge;
        }

        public void AddUberCharge(float rate = 1f) => uberCharge += rate;

        protected bool ActivateUberCharge(Player player)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (uberCharge >= uberChargeCapacity)
            {
                uberCharge = 0;
                p.activateUberCharge = true;
                player.AddBuff(uberChargeBuff, uberChargeDuration);
            }
            return false;
        }

        protected void SniperRifleActiveUpdate(Player player)
        {
            if (currentAmmoClip > 0 && chargeUpDelayTimer >= chargeUpDelay && !reload)
                player.scope = true;
            TF2Player p = player.GetModPlayer<TF2Player>();
            chargeTime = (int)Utils.Clamp(chargeTime, 0, maxChargeUp);
            p.sniperCharge = chargeTime;
            p.sniperMaxCharge = maxChargeUp;
            chargeUpDelayTimer++;
            if (currentAmmoClip > 0 && player.controlUseTile && !ModContent.GetInstance<TF2ConfigClient>().SniperAutoCharge)
                SniperRifleCharge();
            if (ModContent.GetInstance<TF2ConfigClient>().SniperAutoCharge)
                SniperRifleCharge();
            if (!player.controlUseTile && !ModContent.GetInstance<TF2ConfigClient>().SniperAutoCharge)
            {
                chargeTime = 0f;
                isCharging = false;
            }
            if (chargeTime == maxChargeUp)
            {
                if (Item.ModItem is not SydneySleeper)
                    p.crit = true;
                else
                    p.miniCrit = true;
            }
        }

        protected void SniperRifleUpdate(Player player, double speedPercentage)
        {
            if (isSniperRifle)
            {
                if (isCharging)
                {
                    SetPlayerSpeed(player, speedPercentage);
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
                    chargeTime += 1f;
                    chargeInterval = 0f;
                }
                chargeTime = Utils.Clamp(chargeTime, 0, maxChargeUp);
            }
        }

        private int SniperRifleDamage(double damage) => (int)(Item.damage + damage * chargeTime / maxChargeUp);

        protected static int GetCustomItemTime(Player player) => player.itemTime;

        protected void SetCustomItemTime(Player player)
        {
            player.itemTime = Item.useTime;
            player.itemAnimation = Item.useAnimation;
        }

        protected void FlamethrowerProjectile(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback, int cost = 20)
        {
            if (player.altFunctionUse != 2)
            {
                Vector2 muzzleOffset = Vector2.Normalize(velocity) * 54f;
                if (Collision.CanHit(position, 6, 6, position + muzzleOffset, 6, 6))
                {
                    position += muzzleOffset;
                    SoundEngine.PlaySound(SoundID.Item34, player.Center);
                    Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                }
            }
            else
            {
                player.itemAnimationMax = Item.useTime;
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/flame_thrower_airblast"), player.Center);
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<Airblast>(), 0, knockback, player.whoAmI);
                player.statMana -= cost;
                player.manaRegenDelay = 5 * cost;
            }
        }

        protected void ChargeWeaponConsumeAmmo(Player player, int ammoCost = 1)
        {
            if (ModContent.GetInstance<TF2ConfigClient>().InfiniteAmmo) return;
            bool ammoSlot = false;

            for (int i = 54; i < player.inventory.Length; i++)
            {
                Item item = player.inventory[i];
                if (item.type == Item.useAmmo)
                {
                    item.stack -= ammoCost;
                    ammoSlot = true;
                    break;
                }
            }
            if (!ammoSlot)
            {
                for (int i = 0; i < player.inventory.Length; i++)
                {
                    Item item = player.inventory[i];
                    if (item.type == Item.useAmmo)
                    {
                        item.stack -= ammoCost;
                        break;
                    }
                }
            }
        }

        public static bool CanSwitchWeapon(Player player)
        {
            if (player.HeldItem.ModItem is TF2Weapon)
            {
                TF2Weapon weapon = player.HeldItem.ModItem as TF2Weapon;
                if (player.inventory[58] == weapon.Item)
                    return true;
                else return (weapon.currentAmmoClip > 0 || weapon.noAmmoClip) && weapon.holsterTimer >= weapon.holsterSpeed && !weapon.lockWeapon && (!weapon.reload || ModContent.GetInstance<TF2ConfigClient>().EnableWeaponSwitch);
            }
            else return true;
        }

        public bool GetWeaponMechanic(string weapon)
        {
            return weapon switch
            {
                "Grenade Launcher" => isGrenadeLauncher,
                "Stickybomb Launcher" => isStickybombLauncher,
                "Minigun" => isMinigun,
                "Medi Gun" => isMediGun,
                "Sniper Rifle" => isSniperRifle,
                _ => false,
            };
        }

        protected void DemomanMeleeCrit(Player player)
        {
            if (weaponType == Melee)
            {
                if (player.GetModPlayer<TF2Player>().critMelee)
                    player.GetModPlayer<TF2Player>().crit = true;
                player.ClearBuff(ModContent.BuffType<MeleeCrit>());
            }
        }

        protected override void SetWeaponSlot(int weaponCategory)
        {
            if (!noAmmoConsumption)
            {
                switch (weaponCategory)
                {
                    case Primary:
                        weaponType = Primary;
                        Item.useAmmo = ModContent.ItemType<PrimaryAmmo>();
                        break;
                    case Secondary:
                        weaponType = Secondary;
                        Item.useAmmo = ModContent.ItemType<SecondaryAmmo>();
                        break;
                    default:
                        break;
                }
            }
        }

        public override sealed void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = WeaponResearchCost();
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = autoUseAltClick;
        }

        public override sealed void SetDefaults()
        {
            WeaponStatistics();
            ClampVariables();
            if (availability == Unlock)
                metalValue += Item.buyPrice(platinum: 1);
            Item.value = metalValue;
            Item.DamageType = ModContent.GetInstance<TF2DamageClass>();
            if (weaponType == Melee)
                noAmmoClip = true;
            singleReloadShotMaxCooldown = initialReloadRate;
            if (ModContent.GetInstance<TF2ConfigClient>().InfiniteAmmo)
                ammoCost = 0;
        }

        public override sealed Vector2? HoldoutOffset() => offset;

        public override sealed void ModifyTooltips(List<TooltipLine> tooltips)
        {
            RemoveDefaultTooltips(tooltips);
            Player player = Main.LocalPlayer;
            WeaponMultiClassUpdate(player);
            string weaponCategory = ((WeaponCategory)weaponType).ToString();
            if (weaponCategory == "PDA2")
                weaponCategory = "PDA";
            string category = ((Classes)classType).ToString() + "'s " + ((Availability)availability).ToString() + " " + weaponCategory;
            if (classType == MultiClass)
            {
                int currentClass = player.GetModPlayer<TF2Player>().currentClass;
                category = HasClass(currentClass) ? ((Classes)currentClass).ToString() + "'s " + ((Availability)availability).ToString() + " " + weaponCategory : "Multi-Class " + ((Availability)availability).ToString() + " " + weaponCategory;
            }
            TooltipLine line = new TooltipLine(Mod, "Weapon Category", category)
            {
                OverrideColor = new Color(117, 107, 94)
            };
            tooltips.Insert(1, line);
            AddStatisticTooltips(tooltips);
            WeaponDescription(tooltips);
        }

        public override sealed void ModifyWeaponCrit(Player player, ref float crit) => crit = 0;

        public override sealed void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.whoAmI == Main.myPlayer)
                WeaponAttackAnimation(player);
        }

        public override sealed bool CanUseItem(Player player)
        {
            if (Item == player.inventory[58] || player.whoAmI != Main.myPlayer) return false;
            if (reload && !noAmmoClip && !ModContent.GetInstance<TF2ConfigClient>().SingleReload) return false;
            if (!noAmmoClip && ModContent.GetInstance<TF2ConfigClient>().SingleReload)
            {
                if (singleReloadShotCooldown < singleReloadShotMaxCooldown && !magazine && singleReloadShotReload)
                    return false;
            }
            if (deployTimer >= deploySpeed)
                return WeaponCanBeUsed(player);
            else return false;
        }

        public override sealed bool CanConsumeAmmo(Item ammo, Player player) => WeaponCanConsumeAmmo(player);

        public override sealed void HoldItem(Player player)
        {
            if (!noAmmoClip && ModContent.GetInstance<TF2ConfigClient>().SingleReload)
            {
                if (currentAmmoClip <= 0)
                    singleReloadShotReload = true;
                if (singleReloadShotCooldown < initialReloadRate)
                {
                    singleReloadShotCooldown++;
                    cooldownTimer = 0;
                }
            }
            if (noAmmoClip)
                player.GetModPlayer<AmmoInterface>().ammoMax2 = 0;
            UpdateDelay(player, WeaponResetHolsterTimeCondition(player));
            WeaponUpdateAmmo(player);
            if (fullAutomatic)
                spreadRecovery++;
            if (isStickybombLauncher)
                StickybombLauncherUpdate(player, Item.shoot);
            if (isMinigun)
                MinigunActiveUpdate(player);
            if (isMediGun)
                MediGunUpdate(player);
            if (isSniperRifle && Item.ModItem is not Huntsman)
                SniperRifleActiveUpdate(player);
            WeaponActiveUpdate(player);
        }

        public override sealed void UpdateInventory(Player player)
        {
            EnforcePassiveEffects(player);
            if (player.HeldItem == Item)
                WeaponActiveBonus(player);
            if (isMediGun)
                MediGunPassiveUpdate(player);
            OutputDelay(player);
        }

        public override sealed bool AltFunctionUse(Player player) => usesAltClick && WeaponCanAltClick(player);

        public override sealed bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Item.shoot != ProjectileID.None)
            {
                int damageValue = decimalDamage == 0 ? Item.damage : (int)Math.Round(decimalDamage);
                WeaponAttack(player, source, position, velocity, Item.shoot, (int)player.GetDamage<TF2DamageClass>().ApplyTo(damageValue), knockback);
                WeaponPostAttack(player);
            }
            if (ModContent.GetInstance<TF2ConfigClient>().SingleReload)
            {
                singleReloadShotCooldown = 0;
                singleReloadShotReload = false;
            }
            return false;
        }

        public override sealed bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                if (isStickybombLauncher)
                {
                    if (player.controlUseTile)
                        return true;
                    else if (!ModContent.GetInstance<TF2ConfigClient>().Channel && player.controlUseItem && GetCustomItemTime(player) == 0)
                    {
                        isCharging = true;
                        rightClick = false;
                        StickybombLauncherCharge();
                        if (chargeTime == maxChargeUp) return WeaponOnUse(player);
                        return false;
                    }
                    else return WeaponOnUse(player);
                }
                return WeaponOnUse(player);
            }
            return base.UseItem(player);
        }

        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers) => DemomanMeleeCrit(player);

        public override void ModifyHitPvp(Player player, Player target, ref Player.HurtModifiers modifiers) => DemomanMeleeCrit(player);
    }

    public abstract class TF2WeaponNoAmmo : TF2Weapon
    { } // Weapons without ammo won't render the ammo ui

    public abstract class TF2WeaponMelee : TF2WeaponNoAmmo
    {
        // Chargin' Targe Support
        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers) => DemomanMeleeCrit(player);

        public override void ModifyHitPvp(Player player, Player target, ref Player.HurtModifiers modifiers) => DemomanMeleeCrit(player);
    }
}