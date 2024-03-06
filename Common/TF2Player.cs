using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TF2.Content.Buffs;
using TF2.Content.Dusts;
using TF2.Content.Items;
using TF2.Content.Items.Consumables;
using TF2.Content.Items.Currencies;
using TF2.Content.Items.Weapons;
using TF2.Content.Items.Weapons.Demoman;
using TF2.Content.Items.Weapons.Engineer;
using TF2.Content.Items.Weapons.Heavy;
using TF2.Content.Items.Weapons.Medic;
using TF2.Content.Items.Weapons.Pyro;
using TF2.Content.Items.Weapons.Scout;
using TF2.Content.Items.Weapons.Sniper;
using TF2.Content.Items.Weapons.Soldier;
using TF2.Content.Items.Weapons.Spy;
using TF2.Content.Mounts;
using TF2.Content.Projectiles;
using TF2.Content.Projectiles.Medic;
using TF2.Content.UI.Inventory;

namespace TF2.Common
{
    public class TF2Player : ModPlayer
    {
        public bool ClassSelected => currentClass != 0;

        public bool CanChangeClass => currentClass != 0 && !primaryEquipped && !secondaryEquipped && !pdaEquipped;

        public int BaseHealth => currentClass switch
        {
            TF2Item.Scout or TF2Item.Engineer or TF2Item.Sniper or TF2Item.Spy => 125,
            TF2Item.Soldier => 200,
            TF2Item.Pyro or TF2Item.Demoman => 175,
            TF2Item.Heavy => 300,
            TF2Item.Medic => 150,
            _ => Player.statLifeMax
        };

        public float classMultiplier = 0.5f;
        public float healthMultiplier = 1f;
        public float speedMultiplier = 1f;
        public float overhealMultiplier = 1f;
        public int healthBonus;
        public int overheal;
        public int overhealDecayTimer;
        public int pierce = 1;
        public float damageReduction;
        public float healReduction = 1f;
        public bool focus;
        public bool disableFocusSlowdown;
        public int homingPower;
        public int mountSpeed;
        public float damage;
        public float superDamage;

        public bool initializedClass;
        public bool primaryEquipped;
        public bool secondaryEquipped;
        public bool pdaEquipped;

        public int extraJumps;
        public int jumpsLeft;

        public bool hasBanner;
        public int bannerType;

        public bool hasShield;
        public int shieldType;

        public int metal;
        public int maxMetal = 200;
        public float sentryCostReduction = 1f;
        public float dispenserCostReduction = 1f;

        private int healTimer;
        public float regenMultiplier = 1f;
        public bool stopRegen;
        public bool activateUberCharge;
        public float uberCharge;
        public float maxUberCharge = 100;
        public int uberChargeTime;
        public int uberChargeDuration;
        public bool fullyCharged;
        public int organs;

        public bool backStab;
        public bool brokenCloak;
        public int cloakImmuneTime;

        public int currentClass;

        public bool crit;
        public bool miniCrit;
        public bool critMelee;
        public bool noRandomAmmoBoxes;
        public bool noRandomHealthKits;

        public int miningPower = 55;

        public int multiplayerHealCooldown;
        public int activePlayers = Main.player.Take(Main.maxPlayers).Count(x => x.active);
        public int shopRotation;
        private bool newPlayer;
        public int cachedHealth;

        public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath) => !(ModLoader.TryGetMod("Gensokyo", out Mod _) && Mod.TryFind("GensokyoDLC_StarterBox", out ModItem starterBox))
                ? (new Item[] {
                new Item(ModContent.ItemType<Australium>()),
                new Item(ModContent.ItemType<SaxtonHaleSummon>()) })
                : (IEnumerable<Item>)(new Item[] {
                new Item(ModContent.ItemType<Australium>()),
                new Item(ModContent.ItemType<SaxtonHaleSummon>()),
                new Item(starterBox.Type)});

        public override void Initialize()
        {
            homingPower = 0;
            mountSpeed = 0;
        }

        public override void LoadData(TagCompound tag)
        {
            currentClass = tag.GetInt("class");
            classMultiplier = tag.GetFloat("power");
            healthMultiplier = tag.GetFloat("health");
            miningPower = tag.GetInt("miningPower");
            cachedHealth = tag.GetInt("totalHealth");
            homingPower = tag.GetInt("homingPower");
            mountSpeed = tag.GetInt("mountSpeed");
            newPlayer = tag.GetBool("newPlayer");
        }

        public override void SaveData(TagCompound tag)
        {
            tag["class"] = currentClass;
            tag["power"] = classMultiplier;
            tag["health"] = healthMultiplier;
            tag["miningPower"] = miningPower;
            tag["totalHealth"] = cachedHealth;
            tag["homingPower"] = homingPower;
            tag["mountSpeed"] = mountSpeed;
            tag["newPlayer"] = newPlayer;
        }

        public override void OnEnterWorld()
        {
            if (ClassSelected)
            {
                TF2Menu.classSelected = currentClass;
                if (Player.difficulty != 0)
                    Player.difficulty = 0;
            }
            if (currentClass == TF2Item.Spy && !newPlayer)
                ModContent.GetInstance<PDASlot>().FunctionalItem.SetDefaults(ModContent.ItemType<InvisWatch>());
            newPlayer = true;
        }

        public override void OnRespawn()
        {
            uberCharge = 0;
            initializedClass = false;

            for (int i = 0; i < Player.inventory.Length; i++)
            {
                if (Player.inventory[i].ModItem is TF2Weapon item)
                {
                    item.ResetAmmo();
                    item.ResetUseTime();
                }
            }
        }

        public override void ResetEffects()
        {
            CloakSound();
            primaryEquipped = false;
            //secondaryEquipped = false;
            //pdaEquipped = false;
            Player.GetDamage<TF2DamageClass>() *= classMultiplier;
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                healthBonus = 0;
                overhealMultiplier = 1f;
                healReduction = 1f;
                focus = false;
            }
            speedMultiplier = 1f;
            damageReduction = 0f;
            disableFocusSlowdown = false;
            miniCrit = false;
            crit = false;
            critMelee = false;
            noRandomAmmoBoxes = false;
            noRandomHealthKits = false;
            extraJumps = 0;
            hasBanner = false;
            bannerType = 0;
            hasShield = false;
            shieldType = 0;
            maxMetal = 200;
            sentryCostReduction = 1f;
            dispenserCostReduction = 1f;
            regenMultiplier = 1f;
            brokenCloak = false;
        }

        public void CloakSound()
        {
            if ((Player.GetModPlayer<CloakPlayer>().cloakBuff && Player.GetModPlayer<CloakPlayer>().invisWatchEquipped && Player.GetModPlayer<CloakPlayer>().cloakMeter <= 0) ||
                (Player.GetModPlayer<CloakAndDaggerPlayer>().cloakAndDaggerBuff && Player.GetModPlayer<CloakAndDaggerPlayer>().cloakAndDaggerEquipped && Player.GetModPlayer<CloakAndDaggerPlayer>().cloakMeter <= 0) ||
                (Player.GetModPlayer<FeignDeathPlayer>().feignDeath && Player.GetModPlayer<FeignDeathPlayer>().deadRingerEquipped && Player.GetModPlayer<FeignDeathPlayer>().cloakMeter <= 0))
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/spy_cloak"), Player.Center);
        }

        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            if (ClassSelected)
            {
                drawInfo.drawPlayer.hairColor = Color.Transparent;
                drawInfo.colorHead = drawInfo.colorEyes = Color.Multiply(Color.White, 0f);
            }
        }

        public override void HideDrawLayers(PlayerDrawSet drawInfo)
        {
            if (ClassSelected)
            {
                foreach (var layer in PlayerDrawLayerLoader.Layers)
                {
                    if (layer == PlayerDrawLayers.Skin
                        || layer == PlayerDrawLayers.Torso
                        || layer == PlayerDrawLayers.ArmOverItem
                        || layer == PlayerDrawLayers.Leggings
                        || layer == PlayerDrawLayers.Shoes
                        || layer.ToString() == "RaceHead"
                        || layer.ToString() == "RaceTorso"
                        || layer.ToString() == "RaceFrontArm"
                        || layer.ToString() == "RaceBackArm")
                        layer.Hide();
                }
            }
        }

        public override void PostUpdateEquips()
        {
            if (!initializedClass)
                jumpsLeft = extraJumps;
        }

        public override void PreUpdate()
        {
            if (Player.statLife < 1)
                Player.statLife = 0;
            if (Player.mount.Type != ModContent.MountType<TF2Mount>())
                focus = false;
            if (metal < 0)
                metal = 0;
            if (metal > maxMetal)
                metal = maxMetal;
            if (uberCharge >= maxUberCharge)
                uberCharge = maxUberCharge;
            if (Player.statLife < Player.statLifeMax && overheal > 0)
            {
                overheal -= Player.statLifeMax - Player.statLife;
                if (overheal < 0)
                    overheal = 0;
            }
            secondaryEquipped = hasBanner || hasShield || Player.GetModPlayer<GunboatsPlayer>().gunboatsEquipped || Player.GetModPlayer<RazorbackPlayer>().razorbackEquipped || Player.GetModPlayer<DarwinsDangerShieldPlayer>().darwinsDangerShieldEquipped;
            pdaEquipped = Player.GetModPlayer<CloakPlayer>().invisWatchEquipped || Player.GetModPlayer<CloakAndDaggerPlayer>().cloakAndDaggerEquipped;
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {             
                healthBonus = SaveHealth();
                var overheal = 1f;
                if (Player.GetModPlayer<FistsOfSteelPlayer>().fistsOfSteelEquipped)
                    overheal *= 0.6f;
                if (Player.GetModPlayer<RazorbackPlayer>().razorbackEquipped)
                    overheal = 0f;
                overhealMultiplier = overheal;
                var healResistance = 1f;
                if (Player.GetModPlayer<EqualizerPlayer>().equalizerEquipped)
                    healResistance *= 0.1f;
                if (Player.GetModPlayer<BackScratcherPlayer>().backScratcherEquipped)
                    healResistance *= 0.25f;
                if (Player.GetModPlayer<FistsOfSteelPlayer>().fistsOfSteelEquipped)
                    healResistance *= 0.6f;
                healReduction = healResistance;
                focus = Player.controlJump && Player.mount.Type == ModContent.MountType<TF2Mount>() && Player.HasBuff<TF2MountBuff>();
            }
            Player.buffImmune[BuffID.VortexDebuff] = true;
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamity))
            {
                if (calamity.TryFind("WeakPetrification", out ModBuff weakPetrification))
                    Player.buffImmune[weakPetrification.Type] = true;
            }
            if (ModLoader.TryGetMod("Gensokyo", out Mod gensokyo))
            {
                if (gensokyo.TryFind("Debuff_MovementInverted", out ModBuff movementInverted))
                    Player.buffImmune[movementInverted.Type] = true;
                if (gensokyo.TryFind("Debuff_GravityInverted", out ModBuff gravityInverted))
                    Player.buffImmune[gravityInverted.Type] = true;
            }
            cachedHealth = TF2.Round((BaseHealth + SaveHealth()) * healthMultiplier);
        }

        public override void PostUpdate()
        {
            if (!initializedClass && ClassSelected)
            {
                Player.statLife = Player.statLifeMax2;
                metal = maxMetal;
                initializedClass = true;
            }
            if (ClassSelected)
            {
                if (Player.hideMisc[0])
                {
                    foreach (int buff in Player.buffType)
                    {
                        if (Main.vanityPet[buff])
                            Player.ClearBuff(buff);
                    }
                    Player.hideMisc[0] = false;
                }
                if (Player.hideMisc[1])
                {
                    foreach (int buff in Player.buffType)
                    {
                        if (Main.lightPet[buff])
                            Player.ClearBuff(buff);
                    }
                    Player.hideMisc[1] = false;
                }
            }
            if (overheal > 0)
                overhealDecayTimer++;
            var healed = false;
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                foreach (Projectile projectile in Main.projectile)
                {
                    if ((projectile.type == ModContent.ProjectileType<HealingBeam>()
                        || projectile.type == ModContent.ProjectileType<HealingBeamKritzkrieg>())
                        && projectile.Hitbox.Intersects(Player.Hitbox)
                        && projectile.active
                        && projectile.owner != Main.myPlayer)
                        healed = true;
                }
            }
            if (overhealDecayTimer > TF2.Time(0.5) && !healed)
            {
                overhealDecayTimer = 0;
                overheal--;
            }
            if (cloakImmuneTime <= 0)
                cloakImmuneTime = 0;
            else
                cloakImmuneTime--;
            multiplayerHealCooldown--;
            multiplayerHealCooldown = Math.Clamp(multiplayerHealCooldown, 0, 5);
            if (Player.HeldItem.ModItem is TF2Weapon weapon)
            {
                if (damage > 0 && !weapon.Reloading)
                    damage--;
                if (weapon.uberCharge == weapon.uberChargeCapacity && weapon.uberChargeCapacity > 0 && !fullyCharged)
                {
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Voicelines/medic_fullycharged"), Player.Center);
                    fullyCharged = true;
                }
            }
            DoubleJump();
            MedicHealthRegeneration();
        }

        private void DoubleJump()
        {
            if (currentClass == TF2Item.Scout)
                Player.GetJumpState<ScoutDoubleJump>().Enable();
            else
                Player.GetJumpState<ScoutDoubleJump>().Disable();
        }

        private void MedicHealthRegeneration()
        {
            if (currentClass == TF2Item.Medic)
            {
                if (stopRegen) return;
                healTimer++;
                if (healTimer >= TF2.Time(1) && Player.statLife < Player.statLifeMax2)
                {
                    if (Player.GetModPlayer<BlutsaugerPlayer>().blutsaugerEquipped)
                        Player.GetModPlayer<TF2Player>().regenMultiplier *= 0.33f;
                    int healAmount = (int)(TF2.GetHealth(Player, 3) * regenMultiplier);
                    if (Player.HeldItem.ModItem is Amputator)
                        healAmount += TF2.GetHealth(Player, 3);
                    Player.Heal(healAmount);
                    healTimer = 0;
                }
            }
        }

        public override void NaturalLifeRegen(ref float regen)
        {
            if (ClassSelected)
                regen = 0;
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (KeybindSystem.Reload.JustPressed && Main.LocalPlayer.HeldItem.ModItem is TF2Weapon weapon && weapon.currentAmmoClip != weapon.maxAmmoClip && weapon.maxAmmoReserve > 0)
                weapon.Reloading = true;
            if (KeybindSystem.SoldierBuff.JustPressed && hasBanner)
            {
                TF2Player p = Main.LocalPlayer.GetModPlayer<TF2Player>();
                BannerPlayer bannerPlayer = p.bannerType switch
                {
                    1 => Main.LocalPlayer.GetModPlayer<BuffBannerPlayer>(),
                    2 => Main.LocalPlayer.GetModPlayer<BattalionsBackupPlayer>(),
                    3 => Main.LocalPlayer.GetModPlayer<ConcherorPlayer>(),
                    _ => Main.LocalPlayer.GetModPlayer<BuffBannerPlayer>()
                };
                if (bannerPlayer.rage >= bannerPlayer.maxRage)
                {
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/buff_banner_horn_red"), Player.Center);
                    int buff = p.bannerType switch
                    {
                        1 => ModContent.BuffType<Rage>(),
                        2 => ModContent.BuffType<DefenseRage>(),
                        3 => ModContent.BuffType<HealthRage>(),
                        _ => 0
                    };
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        Player.AddBuff(buff, TF2.Time(10));
                    else
                    {
                        foreach (Player targetPlayer in Main.player)
                            targetPlayer.AddBuff(buff, TF2.Time(10), false);
                    }
                }
            }
            if (KeybindSystem.Cloak.JustPressed && Player.GetModPlayer<CloakPlayer>().invisWatchEquipped && !Player.HasBuff<Cloaked>())
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/spy_cloak"), Player.Center);
                Player.AddBuff(ModContent.BuffType<Cloaked>(), TF2.Time(10));
            }
            else if (KeybindSystem.Cloak.JustPressed && Player.GetModPlayer<CloakPlayer>().invisWatchEquipped && Player.HasBuff<Cloaked>())
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/spy_cloak"), Player.Center);
                Player.ClearBuff(ModContent.BuffType<Cloaked>());
            }
            if (KeybindSystem.Cloak.JustPressed && Player.GetModPlayer<CloakAndDaggerPlayer>().cloakAndDaggerEquipped && !Player.HasBuff<CloakAndDaggerBuff>() && !brokenCloak)
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/spy_cloak"), Player.Center);
                Player.AddBuff(ModContent.BuffType<CloakAndDaggerBuff>(), TF2.Time(10));
            }
            else if (KeybindSystem.Cloak.JustPressed && Player.GetModPlayer<CloakAndDaggerPlayer>().cloakAndDaggerEquipped && Player.HasBuff<CloakAndDaggerBuff>())
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/spy_cloak"), Player.Center);
                Player.ClearBuff(ModContent.BuffType<CloakAndDaggerBuff>());
            }
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (ClassSelected)
                modifiers.DisableSound();
            damageReduction = Utils.Clamp(damageReduction, 0f, 1f);
            modifiers.FinalDamage -= damageReduction;
            if (!modifiers.PvP) return;
            Player opponent = Main.player[modifiers.DamageSource.SourcePlayerIndex];
            if (opponent.HeldItem.ModItem is TF2Weapon weapon && weapon.IsWeaponType(TF2Item.Melee) && weapon.WeaponCriticalHits(opponent))
                opponent.GetModPlayer<TF2Player>().crit = true;
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (proj.ModProjectile is TF2Projectile projectile && !projectile.noDistanceModifier)
                projectile.weapon?.WeaponDistanceModifier(Player, proj, target, ref modifiers);
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (!info.PvP) return;
            Player opponent = Main.player[info.DamageSource.SourcePlayerIndex];
            if (opponent.HeldItem.ModItem is TF2Weapon)
            {
                TF2Player p = opponent.GetModPlayer<TF2Player>();
                if (p.ClassSelected && !p.crit)
                    p.damage += info.Damage / p.classMultiplier;
                p.superDamage += info.Damage / p.classMultiplier;
            }
        }

        public override void PostHurt(Player.HurtInfo info)
        {
            if (ClassSelected)
                SoundEngine.PlaySound(new SoundStyle($"TF2/Content/Sounds/SFX/Voicelines/{(ClassesFileNames)currentClass}_painsevere0{Main.rand.Next(1, 4)}"), Player.Center);
        }

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (ClassSelected && item.ModItem is TF2Weapon)
            {
                if (!crit)
                    damage += damageDone / classMultiplier;
                superDamage += damageDone / classMultiplier;
            }

            if (ModContent.GetInstance<TF2Config>().NoTF2Loot || !ClassSelected || item.ModItem?.Mod is not TF2) return;
            if (target.friendly || NPCID.Sets.CountsAsCritter[target.type] || target.type == NPCID.TargetDummy || !target.boss) return;

            if (target.GetGlobalNPC<TF2GlobalNPC>().pickupCooldown <= 0 && Player.HeldItem.ModItem is TF2Weapon weapon && weapon.IsWeaponType(TF2Item.Melee))
            {
                if (weapon.WeaponAmmoBoxes(Player))
                {
                    IEntitySource ammoSource = target.GetSource_FromAI();
                    int type = Main.rand.NextBool(5) ? ModContent.ItemType<LargeAmmoPoint>() : (Main.rand.NextBool(2) ? ModContent.ItemType<MediumAmmoPoint>() : ModContent.ItemType<SmallAmmoPoint>());
                    int loot = Item.NewItem(ammoSource, target.Center, type);
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendData(MessageID.SyncItem, number: loot);
                    target.GetGlobalNPC<TF2GlobalNPC>().pickupCooldown = TF2.Time(1);
                }
                if (weapon.WeaponHealthKits(Player) && !ModContent.GetInstance<TF2Config>().FreeHealthPacks)
                {
                    IEntitySource healthSource = target.GetSource_FromAI();
                    int type = Main.rand.NextBool(5) ? ModContent.ItemType<LargeHealthPoint>() : (Main.rand.NextBool(2) ? ModContent.ItemType<MediumHealthPoint>() : ModContent.ItemType<SmallHealthPoint>());
                    int loot = Item.NewItem(healthSource, target.Center, type);
                    if (Main.netMode != NetmodeID.SinglePlayer)
                        NetMessage.SendData(MessageID.SyncItem, number: loot);
                    target.GetGlobalNPC<TF2GlobalNPC>().pickupCooldown = TF2.Time(1);
                }
            }
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (proj.ModProjectile is TF2Projectile projectile)
            {
                if (ClassSelected)
                {
                    if (!projectile.crit && !crit)
                        damage += damageDone / classMultiplier;
                    superDamage += damageDone / classMultiplier;
                }

                if (ModContent.GetInstance<TF2Config>().NoTF2Loot || !ClassSelected || proj.ModProjectile?.Mod is not TF2) return;
                if (target.friendly || NPCID.Sets.CountsAsCritter[target.type] || target.type == NPCID.TargetDummy || !target.boss) return;

                if (target.GetGlobalNPC<TF2GlobalNPC>().pickupCooldown <= 0)
                {
                    if (projectile.ammoShot)
                    {
                        IEntitySource ammoSource = target.GetSource_FromAI();
                        int type = Main.rand.NextBool(5) ? ModContent.ItemType<LargeAmmoPoint>() : (Main.rand.NextBool(2) ? ModContent.ItemType<MediumAmmoPoint>() : ModContent.ItemType<SmallAmmoPoint>());
                        int loot = Item.NewItem(ammoSource, target.Center, type);
                        if (Main.netMode != NetmodeID.SinglePlayer)
                            NetMessage.SendData(MessageID.SyncItem, number: loot);
                        target.GetGlobalNPC<TF2GlobalNPC>().pickupCooldown = TF2.Time(2);
                    }
                    if (projectile.healthShot && !ModContent.GetInstance<TF2Config>().FreeHealthPacks)
                    {
                        IEntitySource healthSource = target.GetSource_FromAI();
                        int type = Main.rand.NextBool(5) ? ModContent.ItemType<LargeHealthPoint>() : (Main.rand.NextBool(2) ? ModContent.ItemType<MediumHealthPoint>() : ModContent.ItemType<SmallHealthPoint>());
                        int loot = Item.NewItem(healthSource, target.Center, type);
                        if (Main.netMode != NetmodeID.SinglePlayer)
                            NetMessage.SendData(MessageID.SyncItem, number: loot);
                        target.GetGlobalNPC<TF2GlobalNPC>().pickupCooldown = TF2.Time(2);
                    }
                }
            }
        }

        public override bool CanBeHitByProjectile(Projectile proj)
        {
            // This is for the Mann's Anti Danmaku System
            if (focus)
                return proj.Colliding(proj.Hitbox, FocusShotHitbox());
            else return base.CanBeHitByProjectile(proj);
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            if (ClassSelected)
            {
                playSound = false;
                SoundEngine.PlaySound(new SoundStyle($"TF2/Content/Sounds/SFX/Voicelines/{(ClassesFileNames)currentClass}_paincriticaldeath0{Main.rand.Next(1, 4)}"), Player.Center);
            }
            return true;
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            metal = 0;
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                Item item = Player.inventory[i];
                if (item.ModItem is TF2Weapon weapon)
                {
                    weapon.StopReload();
                    if (weapon.GetWeaponMechanic("Medi Gun"))
                        weapon.uberCharge = 0f;
                }
            }
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            if (ClassSelected)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)TF2.MessageType.SyncPlayer);
                packet.Write((byte)Player.whoAmI);
                packet.Write((byte)currentClass);
                packet.Write(healthBonus);
                packet.Write(healthMultiplier);
                packet.Write(healReduction);
                packet.Write(overhealMultiplier);
                packet.Write(Player.itemAnimation);
                packet.Write(Player.itemAnimationMax);
                packet.Write(overheal);
                packet.Write(focus);
                packet.Send(toWho, fromWho);
            }
        }

        public override void CopyClientState(ModPlayer targetCopy)
        {
            TF2Player p = (TF2Player)targetCopy;
            targetCopy.Player.statLife = Player.statLife;
            targetCopy.Player.statLifeMax = Player.statLifeMax;
            p.currentClass = currentClass;
            p.healthBonus = healthBonus;
            p.healthMultiplier = healthMultiplier;
            p.healReduction = healReduction;
            p.overhealMultiplier = overhealMultiplier;
            targetCopy.Player.itemAnimation = Player.itemAnimation;
            targetCopy.Player.itemAnimationMax = Player.itemAnimationMax;
            p.overheal = overheal;
            p.focus = focus;
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            TF2Player p = (TF2Player)clientPlayer;
            if (currentClass != p.currentClass
                || healthBonus != p.healthBonus
                || healthMultiplier != p.healthMultiplier
                || healReduction != p.healReduction
                || overhealMultiplier != p.overhealMultiplier
                || overheal != p.overheal
                || Player.statLife != clientPlayer.Player.statLife
                || Player.statLifeMax != clientPlayer.Player.statLifeMax
                || Player.itemAnimation != clientPlayer.Player.itemAnimation
                || Player.itemAnimationMax != clientPlayer.Player.itemAnimationMax
                || focus != p.focus)
                SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
        }

        public int SaveHealth()
        {
            var health = 0;
            if (TF2.IsItemTypeInHotbar(Player, ModContent.ItemType<Sandman>()))
                health -= 15;
            if (bannerType == 2)
                health += 20;
            if (TF2.IsItemTypeInHotbar(Player, ModContent.ItemType<Eyelander>()))
                health += !Player.dead ? Player.GetModPlayer<EyelanderPlayer>().heads * 15 - 25 : -25;
            if (Player.HeldItem.ModItem is GlovesOfRunningUrgently && Player.inventory[58].ModItem is not GlovesOfRunningUrgently)
                health -= 20;
            if (Player.HasBuff<DalokohsBarBuff>())
                health += 50;
            if (TF2.IsItemTypeInHotbar(Player, ModContent.ItemType<Gunslinger>()))
                health += 25;
            if (TF2.IsItemTypeInHotbar(Player, ModContent.ItemType<VitaSaw>()))
                health -= 10;
            if (TF2.IsItemTypeInHotbar(Player, ModContent.ItemType<ConniversKunai>()))
                health -= 55;
            return health;
        }

        public static bool IsHealthFull(Player player) => player.statLife == TotalHealth(player);

        public static int TotalHealth(Player player)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            return TF2.Round((p.BaseHealth + p.SaveHealth()) * p.healthMultiplier);
        }

        public static float GetCurrentPlayerHealth(Player player) => (float)player.statLife / TotalHealth(player);

        public static int GetPlayerHealthFromPercentage(Player player, double percentage) => TF2.Round(TotalHealth(player) * (float)(percentage / 100f));

        public static float GetPlayerHealthFromPercentageRaw(Player player, double percentage) => (float)(TotalHealth(player) * (float)(percentage / 100f));

        public static void SetPlayerHealth(Player player, int health)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                player.GetModPlayer<TF2Player>().healthBonus += health;
        }

        public static void SetPlayerHealthMultiplier(Player player, double percentage) => player.GetModPlayer<TF2Player>().healthMultiplier *= (float)(percentage / 100f);

        public static void SetPlayerSpeed(Player player, double percentage)
        {
            ref float speed = ref player.GetModPlayer<TF2Player>().speedMultiplier;
            player.moveSpeed *= (float)(percentage / 100f);
            speed *= (float)(percentage / 100f);
        }

        public Rectangle FocusShotHitbox()
        {
            float scale = 1f;

            if (TF2.gensokyoLoaded)
                scale = (float)TF2.Gensokyo.Call("GetPlayerScale", Player.whoAmI);

            int width = (int)(10 * scale);
            int height = (int)(10 * scale);

            return new Rectangle(
                (int)Player.Center.X - width / 2,
                (int)Player.Center.Y - height / 2,
                width,
                height);
        }

        public enum ClassName
        {
            Classless,
            Scout,
            Soldier,
            Pyro,
            Demoman,
            Heavy,
            Engineer,
            Medic,
            Sniper,
            Spy
        }

        public enum ClassesFileNames
        {
            classless,
            scout,
            soldier,
            pyro,
            demoman,
            heavy,
            engineer,
            medic,
            sniper,
            spy
        }

        // Made with help from MrPlague
        public Asset<Texture2D> GetClassTexture(string texturePath)
        {
            string defaultTexturePath = $"TF2/Content/Textures/{(ClassName)currentClass}/{texturePath}";
            return ClassSelected && ModContent.HasAsset(defaultTexturePath) ? ModContent.Request<Texture2D>(defaultTexturePath, AssetRequestMode.ImmediateLoad) : ModContent.Request<Texture2D>("TF2/Content/Textures/Nothing");
        }

        public static void Draw(ref PlayerDrawSet drawInfo, Asset<Texture2D>[] texture, Asset<Texture2D>[,] textureHair, Vector2 position, Rectangle? sourceRect, float rotation, Vector2 origin, float scale, SpriteEffects effect)
        {
            DrawData drawData;
            Player player = drawInfo.drawPlayer;
            if (textureHair != null && textureHair[0, player.hair] != ModContent.Request<Texture2D>("TF2/Content/Textures/Nothing"))
            {
                drawData = new DrawData(textureHair[0, player.hair].Value, position, sourceRect, drawInfo.colorEyeWhites, rotation, origin, scale, effect);
                drawInfo.DrawDataCache.Add(drawData);
            }
            if (texture != null && texture[0] != ModContent.Request<Texture2D>("TF2/Content/Textures/Nothing"))
            {
                drawData = new DrawData(texture[0].Value, position, sourceRect, drawInfo.colorEyeWhites, rotation, origin, scale, effect);
                drawInfo.DrawDataCache.Add(drawData);
            }
        }

        public static void DrawBasic(ref PlayerDrawSet drawInfo, Asset<Texture2D> texture, Asset<Texture2D>[,] textureHair, Vector2 position, Rectangle? sourceRect, float rotation, Vector2 origin, float scale, SpriteEffects effect)
        {
            DrawData drawData;
            Player player = drawInfo.drawPlayer;
            if (textureHair != null && textureHair[0, player.hair] != ModContent.Request<Texture2D>("TF2/Content/Textures/Nothing"))
            {
                drawData = new DrawData(textureHair[0, player.hair].Value, position, sourceRect, drawInfo.colorEyeWhites, rotation, origin, scale, effect);
                drawInfo.DrawDataCache.Add(drawData);
            }
            if (texture != null && texture != ModContent.Request<Texture2D>("TF2/Content/Textures/Nothing"))
            {
                drawData = new DrawData(texture.Value, position, sourceRect, drawInfo.colorEyeWhites, rotation, origin, scale, effect);
                drawInfo.DrawDataCache.Add(drawData);
            }
        }
    }

    public class TF2HeadTexture : PlayerDrawLayer
    {
        private readonly Asset<Texture2D>[,] Hair = new Asset<Texture2D>[10, 165];
        private readonly Asset<Texture2D>[,] HairAlt = new Asset<Texture2D>[10, 165];
        private readonly Asset<Texture2D>[] Head = new Asset<Texture2D>[10];
        private readonly Asset<Texture2D>[] EyeLids = new Asset<Texture2D>[10];
        private readonly Asset<Texture2D>[] Eyes = new Asset<Texture2D>[10];

        public override bool IsHeadLayer => true;

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.skinVar < 10;

        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Head);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            TF2Player p = player.GetModPlayer<TF2Player>();

            if (p.ClassSelected)
            {
                Hair[0, player.hair] = player.direction == 1 ? p.GetClassTexture("Hair") : p.GetClassTexture("Hair_Reverse");
                HairAlt[0, player.hair] = player.direction == 1 ? p.GetClassTexture("Hair") : p.GetClassTexture("Hair_Reverse");
                Head[0] = p.GetClassTexture("Head");
                EyeLids[0] = p.GetClassTexture("EyeLids");
                Eyes[0] = p.GetClassTexture("Eyes");
                Vector2 headPosition = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - (player.bodyFrame.Width / 2) + (player.width / 2)), (int)(drawInfo.Position.Y - Main.screenPosition.Y + player.height - player.bodyFrame.Height + 4f)) + player.headPosition + drawInfo.headVect;
                TF2Player.Draw(ref drawInfo, Head, null, headPosition, player.bodyFrame, player.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect);
                TF2Player.Draw(ref drawInfo, Eyes, null, headPosition, player.bodyFrame, player.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect);
                Vector2 eyelidOffset = Main.OffsetsPlayerHeadgear[player.bodyFrame.Y / player.bodyFrame.Height];
                eyelidOffset.Y -= 2f;
                Rectangle eyelidFrame = EyeLids[0].Frame(1, 3, 0, player.eyeHelper.EyeFrameToShow);
                TF2Player.Draw(ref drawInfo, EyeLids, null, headPosition + eyelidOffset, eyelidFrame, player.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect);
                TF2Player.Draw(ref drawInfo, null, Hair, headPosition, drawInfo.hairFrontFrame, player.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect);
            }
        }
    }

    public class TF2TorsoTexture : PlayerDrawLayer
    {
        private readonly Asset<Texture2D>[] Torso = new Asset<Texture2D>[10];
        private readonly Asset<Texture2D>[] Legs = new Asset<Texture2D>[10];

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.skinVar < 10;

        public override Position GetDefaultPosition() => new Between(PlayerDrawLayers.Skin, PlayerDrawLayers.Leggings);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            TF2Player p = player.GetModPlayer<TF2Player>();

            if (p.ClassSelected)
            {
                player.body = -1;
                player.legs = -1;
                Torso[0] = player.direction == 1 ? p.GetClassTexture("Torso") : p.GetClassTexture("Torso_Reverse");
                Legs[0] = player.direction == 1 ? p.GetClassTexture("Legs") : p.GetClassTexture("Legs_Reverse");
                Vector2 bodyPosition = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - player.bodyFrame.Width / 2 + player.width / 2), (int)(drawInfo.Position.Y - Main.screenPosition.Y + player.height - player.bodyFrame.Height + 4f)) + player.bodyPosition + new Vector2(player.bodyFrame.Width / 2, player.bodyFrame.Height / 2);
                Vector2 legPosition = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - player.bodyFrame.Width / 2 + player.width / 2), (int)(drawInfo.Position.Y - Main.screenPosition.Y + player.height - player.bodyFrame.Height + 4f)) + player.bodyPosition + new Vector2(player.bodyFrame.Width / 2, player.bodyFrame.Height / 2);
                bodyPosition.Y += drawInfo.torsoOffset;
                Vector2 value = Main.OffsetsPlayerHeadgear[player.bodyFrame.Y / player.bodyFrame.Height];
                value.Y -= 2f;
                bodyPosition += value * (-drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically).ToDirectionInt());
                float bodyRotation = player.bodyRotation;
                TF2Player.Draw(ref drawInfo, Torso, null, bodyPosition, drawInfo.compTorsoFrame, bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect);
                if (!player.HasBuff<TF2MountBuff>())
                {
                    if (drawInfo.isSitting)
                        DrawSittingLegs(ref drawInfo, Legs[0].Value);
                    else
                        TF2Player.Draw(ref drawInfo, Legs, null, legPosition, player.legFrame, player.legRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect);
                }
                else
                {
                    Rectangle legFrame = player.legFrame;
                    legFrame.Y = player.legFrame.Height;
                    TF2Player.Draw(ref drawInfo, Legs, null, legPosition, legFrame, player.legRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect);
                }
            }
        }

        private static void DrawSittingLegs(ref PlayerDrawSet drawInfo, Texture2D texture)
        {
            DrawData drawData;
            Player player = drawInfo.drawPlayer;
            Vector2 legsOffset = drawInfo.legsOffset;
            Vector2 value = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - (player.legFrame.Width / 2) + (player.width / 2)), (int)(drawInfo.Position.Y - Main.screenPosition.Y + player.height - player.legFrame.Height + 4f)) + player.legPosition + drawInfo.legVect;
            Rectangle legFrame = player.legFrame;
            value.Y -= 2f;
            value.Y += drawInfo.seatYOffset;
            value += legsOffset;
            int num = 2;
            int num2 = 42;
            int num4 = 2;
            int num8 = 2;
            int num7 = 0;
            int num6 = 0;
            int num5 = 0;
            for (int num3 = num4; num3 >= 0; num3--)
            {
                Vector2 position = value + new Vector2(num, 2f) * new Vector2(player.direction, 1f);
                Rectangle value2 = legFrame;
                value2.Y += num3 * 2;
                value2.Y += num2;
                value2.Height -= num2;
                value2.Height -= num3 * 2;
                if (num3 != num4)
                    value2.Height = 2;
                position.X += player.direction * num8 * num3 + num6 * player.direction;
                if (num3 != 0)
                    position.X += num5 * player.direction;
                position.Y += num2;
                position.Y += num7;
                drawData = new DrawData(texture, position, value2, Color.White, player.legRotation, drawInfo.legVect, 1f, drawInfo.playerEffect);
                drawInfo.DrawDataCache.Add(drawData);
            }
        }
    }

    public class TF2FrontArmTexture : PlayerDrawLayer
    {
        private readonly Asset<Texture2D>[] Arm = new Asset<Texture2D>[10];
        private readonly Asset<Texture2D>[] Hand = new Asset<Texture2D>[10];

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.skinVar < 10;

        public override Position GetDefaultPosition() => new Between(PlayerDrawLayers.ArmOverItem, PlayerDrawLayers.HandOnAcc);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            TF2Player p = player.GetModPlayer<TF2Player>();
            if (p.ClassSelected)
            {
                Arm[0] = player.direction == 1 ? p.GetClassTexture("Arms") : p.GetClassTexture("Arms_Reverse");
                Hand[0] = player.direction == 1 ? p.GetClassTexture("Hands") : p.GetClassTexture("Hands_Reverse");
                Vector2 frontArmPosition = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - player.bodyFrame.Width / 2 + player.width / 2), (int)(drawInfo.Position.Y - Main.screenPosition.Y + player.height - player.bodyFrame.Height + 4f)) + player.bodyPosition + new Vector2(player.bodyFrame.Width / 2, player.bodyFrame.Height / 2);
                Vector2 value = Main.OffsetsPlayerHeadgear[player.bodyFrame.Y / player.bodyFrame.Height];
                value.Y -= 2f;
                frontArmPosition += value * -drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically).ToDirectionInt();
                float rotation = player.bodyRotation + drawInfo.compositeFrontArmRotation;
                Vector2 bodyVect = drawInfo.bodyVect;
                Vector2 frontArmOffset = new Vector2(-5 * (!drawInfo.playerEffect.HasFlag(SpriteEffects.FlipHorizontally) ? 1 : -1), 0f);
                bodyVect += frontArmOffset;
                frontArmPosition += frontArmOffset;
                if (drawInfo.compFrontArmFrame.X / drawInfo.compFrontArmFrame.Width >= 7)
                    frontArmPosition += new Vector2(!drawInfo.playerEffect.HasFlag(SpriteEffects.FlipHorizontally) ? 1 : -1, !drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically) ? 1 : -1);
                TF2Player.Draw(ref drawInfo, Arm, null, frontArmPosition, drawInfo.compFrontArmFrame, rotation, bodyVect, 1f, drawInfo.playerEffect);
                TF2Player.Draw(ref drawInfo, Hand, null, frontArmPosition, drawInfo.compFrontArmFrame, rotation, bodyVect, 1f, drawInfo.playerEffect);
            }
        }
    }

    public class TF2BackArmTexture : PlayerDrawLayer
    {
        private readonly Asset<Texture2D>[] Arm = new Asset<Texture2D>[10];
        private readonly Asset<Texture2D>[] Hand = new Asset<Texture2D>[10];

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.skinVar < 10;

        public override Position GetDefaultPosition() => new BeforeParent(ModContent.GetInstance<TF2TorsoTexture>());

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            TF2Player p = player.GetModPlayer<TF2Player>();

            if (p.ClassSelected)
            {
                Arm[0] = player.direction == 1 ? p.GetClassTexture("Arms") : p.GetClassTexture("Arms_Reverse");
                Hand[0] = player.direction == 1 ? p.GetClassTexture("Hands") : p.GetClassTexture("Hands_Reverse");
                Vector2 backArmPosition = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - player.bodyFrame.Width / 2 + player.width / 2), (int)(drawInfo.Position.Y - Main.screenPosition.Y + player.height - player.bodyFrame.Height + 4f)) + player.bodyPosition + new Vector2(player.bodyFrame.Width / 2, player.bodyFrame.Height / 2);
                Vector2 value = Main.OffsetsPlayerHeadgear[player.bodyFrame.Y / player.bodyFrame.Height];
                value.Y -= 2f;
                backArmPosition += value * -drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically).ToDirectionInt();
                Vector2 backArmOffset = new Vector2(6 * (!drawInfo.playerEffect.HasFlag(SpriteEffects.FlipHorizontally) ? 1 : -1), 2 * (!drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically) ? 1 : -1));
                backArmPosition.Y += drawInfo.torsoOffset;
                float bodyRotation = player.bodyRotation;
                backArmPosition += backArmOffset;
                Vector2 bodyVect2 = drawInfo.bodyVect;
                bodyVect2 += backArmOffset;
                float rotation = bodyRotation + drawInfo.compositeBackArmRotation;
                Rectangle arm = drawInfo.compBackArmFrame;
                if (player.ItemAnimationActive && player.HeldItem.useStyle == 15)
                    arm = new Rectangle(80, 112, 40, 56);
                TF2Player.Draw(ref drawInfo, Arm, null, backArmPosition, arm, rotation, bodyVect2, 1f, drawInfo.playerEffect);
                TF2Player.Draw(ref drawInfo, Hand, null, backArmPosition, arm, rotation, bodyVect2, 1f, drawInfo.playerEffect);
            }
        }
    }

    public class TF2BackTexture : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.skinVar < 10;

        public override Position GetDefaultPosition() => new BeforeParent(ModContent.GetInstance<TF2TorsoTexture>());

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            TF2Player p = player.GetModPlayer<TF2Player>();

            if (p.ClassSelected)
            {
                Vector2 bodyPosition = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - player.bodyFrame.Width / 2 + player.width / 2), (int)(drawInfo.Position.Y - Main.screenPosition.Y + player.height - player.bodyFrame.Height + 4f)) + player.bodyPosition + new Vector2(player.bodyFrame.Width / 2, player.bodyFrame.Height / 2);
                bodyPosition.Y += drawInfo.torsoOffset;
                Vector2 value = Main.OffsetsPlayerHeadgear[player.bodyFrame.Y / player.bodyFrame.Height];
                value.Y -= 2f;
                bodyPosition += value * (-drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically).ToDirectionInt());
                float bodyRotation = player.bodyRotation;
                TF2Player.DrawBasic(ref drawInfo, player.direction == 1 ? p.GetClassTexture("Back") : p.GetClassTexture("Back_Reverse"), null, bodyPosition, drawInfo.compTorsoFrame, bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect);
            }
        }
    }

    public class TF2CollarTexture : PlayerDrawLayer
    {
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.skinVar < 10;

        public override Position GetDefaultPosition() => new AfterParent(ModContent.GetInstance<TF2HeadTexture>());

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player player = drawInfo.drawPlayer;
            TF2Player p = player.GetModPlayer<TF2Player>();

            if (p.currentClass == TF2Item.Soldier || p.currentClass == TF2Item.Demoman)
            {
                Vector2 bodyPosition = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - player.bodyFrame.Width / 2 + player.width / 2), (int)(drawInfo.Position.Y - Main.screenPosition.Y + player.height - player.bodyFrame.Height + 4f)) + player.bodyPosition + new Vector2(player.bodyFrame.Width / 2, player.bodyFrame.Height / 2);
                bodyPosition.Y += drawInfo.torsoOffset;
                Vector2 value = Main.OffsetsPlayerHeadgear[player.bodyFrame.Y / player.bodyFrame.Height];
                value.Y -= 2f;
                bodyPosition += value * (-drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically).ToDirectionInt());
                float bodyRotation = player.bodyRotation;
                TF2Player.DrawBasic(ref drawInfo, p.GetClassTexture("Collar"), null, bodyPosition, drawInfo.compTorsoFrame, bodyRotation, drawInfo.bodyVect, 1f, drawInfo.playerEffect);
            }
        }
    }

    public class WeaponRestrictions : GlobalItem
    {
        public override bool InstancePerEntity => true;

        private static bool AllowedItems(Item item) => ((item.DamageType == ModContent.GetInstance<TF2DamageClass>() || item.DamageType == DamageClass.Default) && !item.potion && item.damage == 0 && item.shoot == ProjectileID.None && item.type != ItemID.RodofDiscord && item.type != ItemID.RodOfHarmony || item.ModItem?.Mod is TF2 || item.createTile != -1 || item.createWall != -1 || ItemID.Sets.BossBag[item.type] || item.ammo != AmmoID.None || item.type == ItemID.ShadowScale || item.shoot == ProjectileID.None);

        private bool canUse = true;

        public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (!canUse)
                spriteBatch.Draw(TextureAssets.Cd.Value, position - TextureAssets.InventoryBack9.Value.Size() / 4.225f * Main.inventoryScale, null, drawColor, 0f, new Vector2(0.5f, 0.5f), 0.8f * Main.inventoryScale, SpriteEffects.None, 0f);
        }

        public override bool CanUseItem(Item item, Player player) => !player.GetModPlayer<TF2Player>().ClassSelected || (item.mountType <= -1 || item.mountType == ModContent.MountType<TF2Mount>()) && AllowedItems(item);

        public override void UpdateInventory(Item item, Player player) => canUse = !player.GetModPlayer<TF2Player>().ClassSelected || AllowedItems(item);
    }

    public class CritPlayer : ModPlayer
    {
        public bool npcProjectileCrit;

        public override void PostUpdate()
        {
            if (Player.GetModPlayer<TF2Player>().crit)
                Player.GetModPlayer<TF2Player>().miniCrit = false;

            if (ModLoader.TryGetMod("CalamityMod", out Mod otherMod))
            {
                if (otherMod.TryFind("RageMode", out ModBuff rageBuff))
                    if (Player.HasBuff(rageBuff.Type))
                        Player.GetModPlayer<TF2Player>().miniCrit = true;
                if (otherMod.TryFind("AdrenalineMode", out ModBuff adrenalineBuff))
                    if (Player.HasBuff(adrenalineBuff.Type))
                        Player.GetModPlayer<TF2Player>().crit = true;
            }
        }

        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
        {
            bool miniCritDebuff = target.GetGlobalNPC<MarkedForDeathNPC>().markedForDeath || target.GetGlobalNPC<JarateNPC>().jarateDebuff;
            if (miniCritDebuff)
                Player.GetModPlayer<TF2Player>().miniCrit = true;
            if (miniCritDebuff && (item.ModItem is FanOWar || item.ModItem is Bushwacka))
                Player.GetModPlayer<TF2Player>().crit = true;

            if (Player.GetModPlayer<TF2Player>().crit)
            {
                modifiers.SetCrit();
                if (item.ModItem?.Mod is TF2)
                {
                    modifiers.CritDamage *= 1.5f;
                    Dust.NewDust(target.Center, 50, 28, ModContent.DustType<CriticalHit>(), 0 * 0.5f);
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/crit_hit"), Player.Center);
                    Player.GetModPlayer<TF2Player>().damage = 0;
                    if (Player.GetModPlayer<CandyCanePlayer>().candyCaneEquipped)
                    {
                        IEntitySource healthSource = target.GetSource_FromAI();
                        int type = ModContent.ItemType<SmallHealth>();
                        int health = Item.NewItem(healthSource, target.Center, type);
                        if (Main.netMode != NetmodeID.SinglePlayer)
                            NetMessage.SendData(MessageID.SyncItem, number: health);
                    }
                }
            }
            else if (Player.GetModPlayer<TF2Player>().miniCrit)
            {
                if (item.ModItem?.Mod is TF2)
                {
                    modifiers.SourceDamage *= 1.35f;
                    Dust.NewDust(target.Center, 40, 24, ModContent.DustType<MiniCrit>(), 0 * 0.5f);
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/crit_hit_mini"), Player.Center);
                }
            }
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (proj.ModProjectile is TF2Projectile projectile)
            {
                if (target.GetGlobalNPC<MarkedForDeathNPC>().markedForDeath)
                {
                    Player.GetModPlayer<TF2Player>().miniCrit = true;
                    if (projectile.spawnedFromNPC)
                        npcProjectileCrit = true;
                }
                if (target.GetGlobalNPC<JarateNPC>().jarateDebuff)
                {
                    Player.GetModPlayer<TF2Player>().miniCrit = true;
                    if (projectile.sniperCrit)
                        modifiers.SourceDamage *= 1.35f;
                    npcProjectileCrit = true;
                }
                if (Player.HasBuff<KritzkriegUberCharge>())
                {
                    if (projectile.spawnedFromNPC)
                        npcProjectileCrit = true;
                }
                if (Player.GetModPlayer<BuffBannerPlayer>().buffActive)
                {
                    if (projectile.spawnedFromNPC)
                        npcProjectileCrit = true;
                }

                if (projectile.spawnedFromNPC && !npcProjectileCrit) return;

                if (projectile.crit)
                {
                    modifiers.SetCrit();
                    modifiers.CritDamage *= 1.5f;
                    Dust.NewDust(target.Center, 50, 28, ModContent.DustType<CriticalHit>(), 0 * 0.5f);
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/crit_hit"), Player.Center);
                    Player.GetModPlayer<TF2Player>().damage = 0;
                    if (Player.GetModPlayer<CandyCanePlayer>().candyCaneEquipped)
                    {
                        IEntitySource healthSource = target.GetSource_FromAI();
                        int type = ModContent.ItemType<SmallHealth>();
                        int health = Item.NewItem(healthSource, target.Center, type);
                        if (Main.netMode != NetmodeID.SinglePlayer)
                            NetMessage.SendData(MessageID.SyncItem, number: health);
                    }
                }
                else if (projectile.miniCrit)
                {
                    modifiers.SourceDamage *= 1.35f;
                    Dust.NewDust(target.Center, 50, 28, ModContent.DustType<MiniCrit>(), 0 * 0.5f);
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/crit_hit_mini"), Player.Center);
                }
            }
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (!modifiers.PvP) return;
            bool miniCritDebuff = Player.GetModPlayer<MarkedForDeathPlayer>().markedForDeath || Player.GetModPlayer<JaratePlayer>().jarateDebuff;
            Player opponent = Main.player[modifiers.DamageSource.SourcePlayerIndex];
            if (miniCritDebuff)
                opponent.GetModPlayer<TF2Player>().miniCrit = true;
            if (miniCritDebuff && (Player.HeldItem.ModItem is FanOWar || Player.HeldItem.ModItem is Bushwacka))
                opponent.GetModPlayer<TF2Player>().crit = true;

            if (opponent.GetModPlayer<TF2Player>().crit)
            {
                if (opponent.HeldItem.ModItem?.Mod is TF2)
                {
                    modifiers.SourceDamage *= 1.5f;
                    Dust.NewDust(Player.Center, 50, 28, ModContent.DustType<CriticalHit>(), 0 * 0.5f);
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/crit_hit"), Player.Center);
                    if (opponent.GetModPlayer<CandyCanePlayer>().candyCaneEquipped)
                    {
                        IEntitySource healthSource = Player.GetSource_FromThis();
                        int type = ModContent.ItemType<SmallHealth>();
                        int health = Item.NewItem(healthSource, opponent.Center + new Vector2(0f, -25f), type);
                        if (Main.netMode != NetmodeID.SinglePlayer)
                            NetMessage.SendData(MessageID.SyncItem, number: health);
                    }
                }
            }
            else if (Player.GetModPlayer<TF2Player>().miniCrit)
            {
                if (opponent.HeldItem.ModItem?.Mod is TF2)
                {
                    modifiers.SourceDamage *= 1.35f;
                    Dust.NewDust(Player.Center, 40, 24, ModContent.DustType<MiniCrit>(), 0 * 0.5f);
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/crit_hit_mini"), Player.Center);
                }
            }
        }
    }

    public class ScoutDoubleJump : ExtraJump
    {
        public override Position GetDefaultPosition() => BeforeMountJumps;

        public override float GetDurationMultiplier(Player player) => 1.5f;

        public override void OnRefreshed(Player player)
        {
            TF2Player p = player.GetModPlayer<TF2Player>();
            p.jumpsLeft = p.extraJumps;
        }

        public override void OnStarted(Player player, ref bool playSound)
        {
            playSound = false;
            ref int jumps = ref player.GetModPlayer<TF2Player>().jumpsLeft;
            jumps--;
            if (jumps > 0)
                player.GetJumpState(this).Available = true;
        }
    }
}