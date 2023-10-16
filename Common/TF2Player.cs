using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TF2.Content.Buffs;
using TF2.Content.Dusts;
using TF2.Content.Items;
using TF2.Content.Items.Ammo;
using TF2.Content.Items.Armor;
using TF2.Content.Items.Consumables;
using TF2.Content.Items.Currencies;
using TF2.Content.Items.Medic;
using TF2.Content.Items.Scout;
using TF2.Content.Items.Sniper;
using TF2.Content.Items.Soldier;
using TF2.Content.Items.Spy;
using TF2.Content.Mounts;
using TF2.Content.Projectiles;
using TF2.Content.Projectiles.Scout;

namespace TF2.Common
{
    public class TF2Player : ModPlayer
    {
        public float classMultiplier = 0.5f;
        public float healthMultiplier = 1f;
        public float speedMultiplier = 1f;
        public int pierce = 1;
        public float damageReduction;
        public float healReduction = 1f;
        public bool focus;
        public bool disableFocusSlowdown;
        public int homingPower;
        public int mountSpeed;
        public int classIconID;

        public bool initializedClass;
        public bool classAccessory;
        public bool classHideVanity;
        public bool classForceVanity;
        public bool classPower;
        public bool classSelected;
        public bool nullified;
        public bool activateUberCharge = false;

        public int extraJumps;
        public int jumpsLeft;

        public bool hasBanner;
        public int bannerType;

        public float stickybombCharge;
        public float stickybombMaxCharge;
        public float stickybombChargeTimer;
        public int stickybombAmount;
        public int stickybombMax;
        public bool hasShield;
        public int shieldType;

        public int metal;
        public int maxMetal = 1000;
        public float sentryCostReduction = 1f;
        public float dispenserCostReduction = 1f;

        private int healTimer;
        public float regenMultiplier = 1f;
        public bool stopRegen;
        public float uberCharge;
        public float maxUberCharge = 100;
        public int uberChargeTime;
        public int uberChargeDuration;
        public int organs;

        public float sniperCharge;
        public float sniperMaxCharge;

        public bool backStab;
        public bool brokenCloak;
        public int cloakImmuneTime;

        public int currentClass; // Class selection

        public bool crit;
        public bool miniCrit;
        public bool critMelee;
        public bool critMisc; // For if crits are created elsewhere instead of CritPlayer;

        public int activePlayers = Main.player.Take(Main.maxPlayers).Count(x => x.active);

        public int shopRotation;

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
            classIconID = tag.GetInt("class");
            homingPower = tag.GetInt("homingPower");
            mountSpeed = tag.GetInt("mountSpeed");
        }

        public override void SaveData(TagCompound tag)
        {
            tag["class"] = classIconID;
            tag["homingPower"] = homingPower;
            tag["mountSpeed"] = mountSpeed;
        }

        public override void UpdateDead()
        {
            metal = 0;
            if (organs == 0)
                uberCharge = 0;

            for (int i = 0; i < Player.inventory.Length; i++)
            {
                if (Player.inventory[i].ModItem is TF2Weapon item)
                    item.StopReload();
            }
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
            DoubleJump();
            CloakSound();
            Player.GetDamage<TF2DamageClass>() *= classMultiplier;
            classAccessory = classHideVanity = classForceVanity = classPower = false;
            currentClass = 0;
            classMultiplier = 0.5f;
            healthMultiplier = 1f;
            speedMultiplier = 1f;
            damageReduction = 0f;
            healReduction = 1f;
            focus = false;
            disableFocusSlowdown = false;
            miniCrit = false;
            crit = false;
            critMelee = false;
            critMisc = false;
            maxMetal = 200;
            sentryCostReduction = 1f;
            dispenserCostReduction = 1f;
            pierce = 1;
            classSelected = false;
            extraJumps = 0;
            hasBanner = false;
            bannerType = 0;
            hasShield = false;
            shieldType = 0;
            regenMultiplier = 1f;
            brokenCloak = false;
        }

        // Checking player velocity prevents wall jumping
        public static bool Grounded(Player player) => player.TouchedTiles.Count >= 1 && (player.velocity.Y >= 0);

        public void CloakSound()
        {
            if ((Player.GetModPlayer<CloakPlayer>().cloakBuff && Player.GetModPlayer<CloakPlayer>().invisWatchEquipped && Player.GetModPlayer<CloakPlayer>().cloakMeter <= 0) ||
                (Player.GetModPlayer<CloakAndDaggerPlayer>().cloakAndDaggerBuff && Player.GetModPlayer<CloakAndDaggerPlayer>().cloakAndDaggerEquipped && Player.GetModPlayer<CloakAndDaggerPlayer>().cloakMeter <= 0) ||
                (Player.GetModPlayer<FeignDeathPlayer>().feignDeath && Player.GetModPlayer<FeignDeathPlayer>().deadRingerEquipped && Player.GetModPlayer<FeignDeathPlayer>().cloakMeter <= 0))
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spy_cloak"), Player.Center);
        }

        public override void UpdateVisibleVanityAccessories()
        {
            for (int n = 13; n < 18 + Player.extraAccessorySlots; n++)
            {
                Item item = Player.armor[n];
                if (item.type == ModContent.ItemType<ScoutClass>() ||
                    item.type == ModContent.ItemType<SoldierClass>() ||
                    item.type == ModContent.ItemType<PyroClass>() ||
                    item.type == ModContent.ItemType<DemomanClass>() ||
                    item.type == ModContent.ItemType<HeavyClass>() ||
                    item.type == ModContent.ItemType<EngineerClass>() ||
                    item.type == ModContent.ItemType<MedicClass>() ||
                    item.type == ModContent.ItemType<SniperClass>() ||
                    item.type == ModContent.ItemType<SpyClass>())
                {
                    classHideVanity = false;
                    classForceVanity = true;
                    classAccessory = false;
                }
            }
        }

        public override void FrameEffects()
        {
            if (classAccessory && !classSelected)
            {
                switch (currentClass)
                {
                    case 1:
                        classSelected = true;
                        if (classHideVanity) return;
                        ScoutClass scoutClass = ModContent.GetInstance<ScoutClass>();
                        Player.legs = EquipLoader.GetEquipSlot(Mod, scoutClass.Name, EquipType.Legs);
                        Player.body = EquipLoader.GetEquipSlot(Mod, scoutClass.Name, EquipType.Body);
                        Player.head = EquipLoader.GetEquipSlot(Mod, scoutClass.Name, EquipType.Head);
                        Player.wings = EquipLoader.GetEquipSlot(Mod, scoutClass.Name, EquipType.Wings);
                        break;

                    case 2:
                        classSelected = true;
                        if (classHideVanity) return;
                        SoldierClass soldierClass = ModContent.GetInstance<SoldierClass>();
                        Player.legs = EquipLoader.GetEquipSlot(Mod, soldierClass.Name, EquipType.Legs);
                        Player.body = EquipLoader.GetEquipSlot(Mod, soldierClass.Name, EquipType.Body);
                        Player.head = EquipLoader.GetEquipSlot(Mod, soldierClass.Name, EquipType.Head);
                        Player.wings = EquipLoader.GetEquipSlot(Mod, soldierClass.Name, EquipType.Wings);
                        break;

                    case 3:
                        classSelected = true;
                        if (classHideVanity) return;
                        PyroClass pyroClass = ModContent.GetInstance<PyroClass>();
                        Player.legs = EquipLoader.GetEquipSlot(Mod, pyroClass.Name, EquipType.Legs);
                        Player.body = EquipLoader.GetEquipSlot(Mod, pyroClass.Name, EquipType.Body);
                        Player.head = EquipLoader.GetEquipSlot(Mod, pyroClass.Name, EquipType.Head);
                        Player.wings = EquipLoader.GetEquipSlot(Mod, pyroClass.Name, EquipType.Wings);
                        break;

                    case 4:
                        classSelected = true;
                        if (classHideVanity) return;
                        DemomanClass demomanClass = ModContent.GetInstance<DemomanClass>();
                        Player.legs = EquipLoader.GetEquipSlot(Mod, demomanClass.Name, EquipType.Legs);
                        Player.body = EquipLoader.GetEquipSlot(Mod, demomanClass.Name, EquipType.Body);
                        Player.head = EquipLoader.GetEquipSlot(Mod, demomanClass.Name, EquipType.Head);
                        Player.wings = EquipLoader.GetEquipSlot(Mod, demomanClass.Name, EquipType.Wings);
                        break;

                    case 5:
                        classSelected = true;
                        if (classHideVanity) return;
                        HeavyClass heavyClass = ModContent.GetInstance<HeavyClass>();
                        Player.legs = EquipLoader.GetEquipSlot(Mod, heavyClass.Name, EquipType.Legs);
                        Player.body = EquipLoader.GetEquipSlot(Mod, heavyClass.Name, EquipType.Body);
                        Player.head = EquipLoader.GetEquipSlot(Mod, heavyClass.Name, EquipType.Head);
                        Player.wings = EquipLoader.GetEquipSlot(Mod, heavyClass.Name, EquipType.Wings);
                        break;

                    case 6:
                        classSelected = true;
                        if (classHideVanity) return;
                        EngineerClass engineerClass = ModContent.GetInstance<EngineerClass>();
                        Player.legs = EquipLoader.GetEquipSlot(Mod, engineerClass.Name, EquipType.Legs);
                        Player.body = EquipLoader.GetEquipSlot(Mod, engineerClass.Name, EquipType.Body);
                        Player.head = EquipLoader.GetEquipSlot(Mod, engineerClass.Name, EquipType.Head);
                        Player.wings = EquipLoader.GetEquipSlot(Mod, engineerClass.Name, EquipType.Wings);
                        break;

                    case 7:
                        classSelected = true;
                        if (classHideVanity) return;
                        MedicClass medicClass = ModContent.GetInstance<MedicClass>();
                        Player.legs = EquipLoader.GetEquipSlot(Mod, medicClass.Name, EquipType.Legs);
                        Player.body = EquipLoader.GetEquipSlot(Mod, medicClass.Name, EquipType.Body);
                        Player.head = EquipLoader.GetEquipSlot(Mod, medicClass.Name, EquipType.Head);
                        Player.wings = EquipLoader.GetEquipSlot(Mod, medicClass.Name, EquipType.Wings);
                        break;

                    case 8:
                        classSelected = true;
                        if (classHideVanity) return;
                        SniperClass sniperClass = ModContent.GetInstance<SniperClass>();
                        Player.legs = EquipLoader.GetEquipSlot(Mod, sniperClass.Name, EquipType.Legs);
                        Player.body = EquipLoader.GetEquipSlot(Mod, sniperClass.Name, EquipType.Body);
                        Player.head = EquipLoader.GetEquipSlot(Mod, sniperClass.Name, EquipType.Head);
                        Player.wings = EquipLoader.GetEquipSlot(Mod, sniperClass.Name, EquipType.Wings);
                        break;

                    case 9:
                        classSelected = true;
                        if (classHideVanity) return;
                        SpyClass spyClass = ModContent.GetInstance<SpyClass>();
                        Player.legs = EquipLoader.GetEquipSlot(Mod, spyClass.Name, EquipType.Legs);
                        Player.body = EquipLoader.GetEquipSlot(Mod, spyClass.Name, EquipType.Body);
                        Player.head = EquipLoader.GetEquipSlot(Mod, spyClass.Name, EquipType.Head);
                        Player.wings = EquipLoader.GetEquipSlot(Mod, spyClass.Name, EquipType.Wings);
                        break;

                    default:
                        break;
                }
            }
            if (nullified)
                Nullify();
        }

        private void Nullify()
        {
            Player.ResetEffects();
            Player.head = -1;
            Player.body = -1;
            Player.legs = -1;
            Player.handon = -1;
            Player.handoff = -1;
            Player.back = -1;
            Player.front = -1;
            Player.shoe = -1;
            Player.waist = -1;
            Player.shield = -1;
            Player.neck = -1;
            Player.face = -1;
            Player.balloon = -1;
            nullified = true;
        }

        public override void PostUpdateEquips()
        {
            if (nullified)
                Nullify();
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

            // if (Player.HasBuff(BuffID.VortexDebuff))
            // Player.ClearBuff(BuffID.VortexDebuff);
            Player.buffImmune[BuffID.VortexDebuff] = true;
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamity))
            {
                if (calamity.TryFind("WeakPetrification", out ModBuff weakPetrification))
                {
                    // if (Player.HasBuff(weakPetrification.Type) && classSelected)
                    // Player.ClearBuff(weakPetrification.Type);
                    Player.buffImmune[weakPetrification.Type] = true;
                }
            }
            if (ModLoader.TryGetMod("Gensokyo", out Mod gensokyo))
            {
                if (gensokyo.TryFind("Debuff_MovementInverted", out ModBuff movementInverted))
                {
                    // if (Player.HasBuff(movementInverted.Type) && classSelected)
                    // Player.ClearBuff(movementInverted.Type);
                    Player.buffImmune[movementInverted.Type] = true;
                }
                if (gensokyo.TryFind("Debuff_GravityInverted", out ModBuff gravityInverted))
                {
                    // if (Player.HasBuff(gravityInverted.Type) && classSelected)
                    // Player.ClearBuff(gravityInverted.Type);
                    Player.buffImmune[gravityInverted.Type] = true;
                }
            }
        }

        public override void PostUpdate()
        {
            classIconID = currentClass;

            if (!initializedClass && classSelected)
            {
                Player.statLife = Player.statLifeMax2;
                metal = maxMetal;
                initializedClass = true;
            }
            else if (!initializedClass)
                initializedClass = true;

            if (currentClass < 0)
                Player.lifeRegen = 0;

            if (cloakImmuneTime <= 0)
                cloakImmuneTime = 0;
            else
                cloakImmuneTime--;

            DoubleJump();
            MedicHealthRegeneration();
        }

        private void DoubleJump()
        {
            if (currentClass == 1)
                Player.GetJumpState<ScoutDoubleJump>().Enable();
            else
                Player.GetJumpState<ScoutDoubleJump>().Disable();
        }

        private void MedicHealthRegeneration()
        {
            if (currentClass == 7)
            {
                if (stopRegen) return;
                healTimer++;
                if (healTimer >= 60 && Player.statLife < Player.statLifeMax2)
                {
                    if (Player.GetModPlayer<BlutsaugerPlayer>().blutsaugerEquipped)
                        Player.GetModPlayer<TF2Player>().regenMultiplier *= 0.33f;
                    int healAmount = (int)(0.02f * regenMultiplier * Player.statLifeMax2);
                    if (Player.HeldItem.ModItem is Amputator)
                        healAmount += (int)(0.02f * Player.statLifeMax2);
                    Player.Heal(healAmount);
                    healTimer = 0;
                }
            }
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (KeybindSystem.SoldierBuff.JustPressed && hasBanner)
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/buff_banner_horn_red"), Player.Center);
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
                    int buff = p.bannerType switch
                    {
                        1 => ModContent.BuffType<Rage>(),
                        2 => ModContent.BuffType<DefenseRage>(),
                        3 => ModContent.BuffType<HealthRage>(),
                        _ => 0
                    };
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        Player.AddBuff(buff, 600);
                    else
                    {
                        foreach (Player targetPlayer in Main.player)
                        {
                            targetPlayer.AddBuff(buff, 600);
                            NetMessage.SendData(MessageID.AddPlayerBuff, number: targetPlayer.whoAmI, number2: buff, number3: 600);
                        }
                    }
                }
            }
            if (KeybindSystem.Cloak.JustPressed && Player.GetModPlayer<CloakPlayer>().invisWatchEquipped && !Player.HasBuff<Cloaked>())
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spy_cloak"), Player.Center);
                Player.AddBuff(ModContent.BuffType<Cloaked>(), 600);
            }
            else if (KeybindSystem.Cloak.JustPressed && Player.GetModPlayer<CloakPlayer>().invisWatchEquipped && Player.HasBuff<Cloaked>())
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spy_cloak"), Player.Center);
                Player.ClearBuff(ModContent.BuffType<Cloaked>());
            }
            if (KeybindSystem.Cloak.JustPressed && Player.GetModPlayer<CloakAndDaggerPlayer>().cloakAndDaggerEquipped && !Player.HasBuff<CloakAndDaggerBuff>() && !brokenCloak)
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spy_cloak"), Player.Center);
                Player.AddBuff(ModContent.BuffType<CloakAndDaggerBuff>(), 600);
            }
            else if (KeybindSystem.Cloak.JustPressed && Player.GetModPlayer<CloakAndDaggerPlayer>().cloakAndDaggerEquipped && Player.HasBuff<CloakAndDaggerBuff>())
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spy_cloak"), Player.Center);
                Player.ClearBuff(ModContent.BuffType<CloakAndDaggerBuff>());
            }
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            damageReduction = Utils.Clamp(damageReduction, 0f, 1f);
            modifiers.FinalDamage -= damageReduction;
            if (!modifiers.PvP) return;
            Player opponent = Main.player[modifiers.DamageSource.SourcePlayerIndex];
            if (opponent.HeldItem.ModItem is TF2Weapon)
            {
                TF2Weapon weapon = opponent.HeldItem.ModItem as TF2Weapon;
                if (opponent.HeldItem.ModItem is TF2WeaponMelee || weapon.IsWeaponType(TF2Weapon.Melee))
                {
                    if (Main.rand.NextBool(6) && !weapon.noRandomCrits)
                        opponent.GetModPlayer<TF2Player>().crit = true;
                }
                else
                {
                    if (Main.rand.NextBool(50) && !weapon.noRandomCrits)
                        opponent.GetModPlayer<TF2Player>().crit = true;
                }
            }
        }

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (ModContent.GetInstance<TF2Config>().NoTF2Loot || currentClass == 0 || item.ModItem?.Mod is not TF2) return;
            if (target.friendly || NPCID.Sets.CountsAsCritter[target.type] || target.type == NPCID.TargetDummy) return;

            if (Main.rand.NextBool(25) && currentClass == 6)
            {
                IEntitySource metalSource = target.GetSource_FromAI();
                int type = ModContent.ItemType<Metal>();
                int loot = Item.NewItem(metalSource, target.Center, type);
                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendData(MessageID.SyncItem, number: loot);
            }

            if (ModContent.GetInstance<TF2Config>().FreeHealthPacks) return;

            if (Main.rand.NextBool(5) && currentClass > 0)
            {
                IEntitySource healthSource = target.GetSource_FromAI();
                int type = ModContent.ItemType<SmallHealth>();
                int loot = Item.NewItem(healthSource, target.Center, type);
                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendData(MessageID.SyncItem, number: loot);
            }

            if (Main.rand.NextBool(10) && currentClass > 0)
            {
                IEntitySource healthSource = target.GetSource_FromAI();
                int type = ModContent.ItemType<MediumHealth>();
                int loot = Item.NewItem(healthSource, target.Center, type);
                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendData(MessageID.SyncItem, number: loot);
            }

            if (Main.rand.NextBool(20) && currentClass > 0)
            {
                IEntitySource healthSource = target.GetSource_FromAI();
                int type = ModContent.ItemType<LargeHealth>();
                int loot = Item.NewItem(healthSource, target.Center, type);
                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendData(MessageID.SyncItem, number: loot);
            }
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (ModContent.GetInstance<TF2Config>().NoTF2Loot || currentClass == 0 || proj.ModProjectile?.Mod is not TF2) return;
            if (target.friendly || NPCID.Sets.CountsAsCritter[target.type] || target.type == NPCID.TargetDummy)
                return;

            if (Main.rand.NextBool(100) && currentClass == 6)
            {
                IEntitySource metalSource = target.GetSource_FromAI();
                int type = ModContent.ItemType<Metal>();
                int loot = Item.NewItem(metalSource, target.Center, type);
                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendData(MessageID.SyncItem, number: loot);
            }

            if (ModContent.GetInstance<TF2Config>().FreeHealthPacks) return;

            if (Main.rand.NextBool(100) && currentClass > 0)
            {
                IEntitySource healthSource = target.GetSource_FromAI();
                int type = ModContent.ItemType<SmallHealth>();
                int loot = Item.NewItem(healthSource, target.Center, type);
                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendData(MessageID.SyncItem, number: loot);
            }

            if (Main.rand.NextBool(250) && currentClass > 0)
            {
                IEntitySource healthSource = target.GetSource_FromAI();
                int type = ModContent.ItemType<MediumHealth>();
                int loot = Item.NewItem(healthSource, target.Center, type);
                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendData(MessageID.SyncItem, number: loot);
            }

            if (Main.rand.NextBool(500) && currentClass > 0)
            {
                IEntitySource healthSource = target.GetSource_FromAI();
                int type = ModContent.ItemType<LargeHealth>();
                int loot = Item.NewItem(healthSource, target.Center, type);
                if (Main.netMode != NetmodeID.SinglePlayer)
                    NetMessage.SendData(MessageID.SyncItem, number: loot);
            }
        }

        public override bool CanBeHitByProjectile(Projectile proj)
        {
            // This is for the Mann's Anti Danmaku System
            if (focus)
                return proj.Colliding(proj.Hitbox, FocusShotHitbox());
            else return base.CanBeHitByProjectile(proj);
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
    }

    public class CritPlayer : ModPlayer
    {
        public bool npcProjectileCrit;

        public override void PostUpdate()
        {
            if (Player.GetModPlayer<TF2Player>().crit)
                Player.GetModPlayer<TF2Player>().miniCrit = false;
            if (Player.GetModPlayer<TF2Player>().critMisc)
                Player.GetModPlayer<TF2Player>().crit = false;

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
                    Dust.NewDust(target.Center, 50, 28, ModContent.DustType<CriticalHit>(), 0 * 0.5f, 0);
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/crit_hit"), Player.Center);
                }
            }
            else if (Player.GetModPlayer<TF2Player>().miniCrit)
            {
                if (item.ModItem?.Mod is TF2)
                {
                    modifiers.SourceDamage.Base *= 1.35f;
                    Dust.NewDust(target.Center, 40, 24, ModContent.DustType<MiniCrit>(), 0 * 0.5f, 0);
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/crit_hit_mini"), Player.Center);
                }
            }
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            // Temporary solution
            if (proj.ModProjectile is ShoveHitbox) return;

            // NPCs can crit (with the right conditions)
            if (target.GetGlobalNPC<MarkedForDeathNPC>().markedForDeath)
            {
                Player.GetModPlayer<TF2Player>().miniCrit = true;
                if (proj.GetGlobalProjectile<TF2ProjectileBase>().spawnedFromNPC)
                    npcProjectileCrit = true;
            }
            if (target.GetGlobalNPC<JarateNPC>().jarateDebuff)
            {
                Player.GetModPlayer<TF2Player>().miniCrit = true;
                if (proj.GetGlobalProjectile<TF2ProjectileBase>().sniperCrit)
                    modifiers.SourceDamage *= 1.35f;
                npcProjectileCrit = true;
            }
            if (Player.HasBuff<KritzkriegUberCharge>())
            {
                if (proj.GetGlobalProjectile<TF2ProjectileBase>().spawnedFromNPC)
                    npcProjectileCrit = true;
            }
            if (Player.GetModPlayer<BuffBannerPlayer>().buffActive)
            {
                if (proj.GetGlobalProjectile<TF2ProjectileBase>().spawnedFromNPC)
                    npcProjectileCrit = true;
            }

            if (proj.GetGlobalProjectile<TF2ProjectileBase>().spawnedFromNPC && !npcProjectileCrit) return;

            if (Player.GetModPlayer<TF2Player>().crit)
            {
                modifiers.SetCrit();
                if (proj.ModProjectile?.Mod is TF2)
                {
                    modifiers.CritDamage *= 1.5f;
                    Dust.NewDust(target.Center, 50, 28, ModContent.DustType<CriticalHit>(), 0 * 0.5f, 0);
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/crit_hit"), Player.Center);
                }
            }
            else if (Player.GetModPlayer<TF2Player>().miniCrit)
            {
                if (proj.ModProjectile?.Mod is TF2)
                {
                    modifiers.SourceDamage *= 1.35f;
                    Dust.NewDust(target.Center, 50, 28, ModContent.DustType<MiniCrit>(), 0 * 0.5f, 0);
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
                    Dust.NewDust(Player.Center, 50, 28, ModContent.DustType<CriticalHit>(), 0 * 0.5f, 0);
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/crit_hit"), Player.Center);
                }
            }
            else if (Player.GetModPlayer<TF2Player>().miniCrit)
            {
                if (opponent.HeldItem.ModItem?.Mod is TF2)
                {
                    modifiers.SourceDamage *= 1.35f;
                    Dust.NewDust(Player.Center, 40, 24, ModContent.DustType<MiniCrit>(), 0 * 0.5f, 0);
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