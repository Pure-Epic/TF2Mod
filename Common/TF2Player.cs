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
using TF2.Content.Items;
using TF2.Content.Items.Ammo;
using TF2.Content.Items.Consumables;
using TF2.Content.Items.Armor;
using TF2.Content.Items.Currencies;
using TF2.Content.Buffs;
using TF2.Content.Dusts;
using TF2.Content.Projectiles;
using TF2.Content.Items.Soldier;
using TF2.Content.Items.Sniper;
using TF2.Gensokyo.Common;

namespace TF2.Common
{
    public class TF2Player : ModPlayer
    {
        public float classMultiplier = 0.5f;
        public float healthMultiplier = 1f;
        public int pierce = 1;
        public float speedMultiplier = 1f;
        public float damageReduction;
        public bool focus;
        public bool disableFocusSlowdown;
        public int homingPower;
        public int mountSpeed;

        public bool initializedClass;
        public bool classAccessory;
        public bool classHideVanity;
        public bool classForceVanity;
        public bool classPower;
        public bool classSelected;
        public bool nullified;
        public bool activateUbercharge = false;

        public int extraJumps = 1;
        public int jumpsLeft;
        public bool allowExtraJumps;
        public bool releaseJump;
        public bool hasJumpOption_Scout;
        public bool canJumpAgain_Scout;
        public bool isPerformingJump_Scout;

        public bool buffBanner;
        public int bannerType = 0;
        public bool equalizer;
        public bool gunboats;

        public float stickybombCharge = 0;
        public float stickybombMaxCharge = 0;
        public float stickybombChargeTimer = 0;
        public int stickybombAmount = 0;
        public int stickybombMax;
        public bool shield;
        public int shieldTimer = 1;
        public int shieldType = 0;

        public int metal;
        public int maxMetal = 1000;

        public float ubercharge = 0;
        public float maxUbercharge = 100;
        public int uberchargeTime = 0;
        public int organs = 0;

        public float sniperCharge = 0;
        public float sniperMaxCharge = 0;
        public float sniperChargeTimer = 0;
        public bool sniperShield;

        public bool backStab;
        public bool invisWatchEquipped;
        public bool cloakandDaggerEquipped;
        public bool deadRingerEquipped;
        public bool brokenCloak;
        public int cloakImmuneTime;
        public bool lEtrangerEquipped;
        public bool yourEternalRewardEquipped;
     
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
            homingPower = tag.GetInt("homingPower");
            mountSpeed = tag.GetInt("mountSpeed");
        }

        public override void SaveData(TagCompound tag)
        {
            tag["homingPower"] = homingPower;
            tag["mountSpeed"] = mountSpeed;
        }

        // This is called before accessories' effects
        public override void OnEnterWorld(Player player) => allowExtraJumps = true;

        public override void UpdateDead()
        {
            metal = 0;
            if (organs == 0)
                ubercharge = 0;
        }

        public override void OnRespawn(Player player)
        {
            ubercharge = 0;
            initializedClass = false;
        }

        public override void ResetEffects()
        {
            CloakSound();
            Player.GetDamage<TF2DamageClass>() *= classMultiplier;
            classAccessory = classHideVanity = classForceVanity = classPower = false;
            currentClass = 0;
            classMultiplier = 0.5f;
            healthMultiplier = 1f;
            speedMultiplier = 1f;
            damageReduction = 0f;
            focus = false;
            disableFocusSlowdown = false;
            miniCrit = false;
            crit = false;
            critMelee = false;
            critMisc = false;
            maxMetal = 200;
            pierce = 1;
            classSelected = false;
            extraJumps = 0;
            hasJumpOption_Scout = false;
            buffBanner = false;
            bannerType = 0;
            equalizer = false;
            gunboats = false;
            sniperShield = false;
            shield = false;
            shieldTimer = 1;
            shieldType = 0;
            invisWatchEquipped = false;
            cloakandDaggerEquipped = false;
            deadRingerEquipped = false;
            brokenCloak = false;
            lEtrangerEquipped = false;
            yourEternalRewardEquipped = false;
        }

        // Checking for player velocity prevents wall jumping
        public static bool Grounded(Player player) => player.TouchedTiles.Count >= 1 && (player.velocity.Y >= 0);
        
        public void CloakSound()
        {
            if ((Player.GetModPlayer<CloakPlayer>().cloakBuff && invisWatchEquipped && Player.GetModPlayer<CloakPlayer>().cloakMeter <= 0) ||
                (Player.GetModPlayer<CloakandDaggerPlayer>().cloakandDaggerBuff && cloakandDaggerEquipped && Player.GetModPlayer<CloakandDaggerPlayer>().cloakMeter <= 0) ||
                (Player.GetModPlayer<Content.Items.Spy.FeignDeathPlayer>().feignDeath && deadRingerEquipped && Player.GetModPlayer<Content.Items.Spy.FeignDeathPlayer>().cloakMeter <= 0)) // This namespace usage is necessary
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

        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource, ref int cooldownCounter)
        {
            damage -= (int)(damageReduction * damage);
            return true;
        }

        public override void FrameEffects()
        {
            if (classAccessory && !classSelected) //classAccessory == true && classSelected == false
            {
                switch (currentClass)
                {
                    case 1:
                        classSelected = true;
                        if (classHideVanity) return;
                        var scoutClass = ModContent.GetInstance<ScoutClass>();
                        Player.legs = EquipLoader.GetEquipSlot(Mod, scoutClass.Name, EquipType.Legs);
                        Player.body = EquipLoader.GetEquipSlot(Mod, scoutClass.Name, EquipType.Body);
                        Player.head = EquipLoader.GetEquipSlot(Mod, scoutClass.Name, EquipType.Head);
                        Player.wings = EquipLoader.GetEquipSlot(Mod, scoutClass.Name, EquipType.Wings);
                        break;

                    case 2:
                        classSelected = true;
                        if (classHideVanity) return;
                        var soldierClass = ModContent.GetInstance<SoldierClass>();
                        Player.legs = EquipLoader.GetEquipSlot(Mod, soldierClass.Name, EquipType.Legs);
                        Player.body = EquipLoader.GetEquipSlot(Mod, soldierClass.Name, EquipType.Body);
                        Player.head = EquipLoader.GetEquipSlot(Mod, soldierClass.Name, EquipType.Head);
                        Player.wings = EquipLoader.GetEquipSlot(Mod, soldierClass.Name, EquipType.Wings);
                        break;

                    case 3:
                        classSelected = true;
                        if (classHideVanity) return;
                        var pyroClass = ModContent.GetInstance<PyroClass>();
                        Player.legs = EquipLoader.GetEquipSlot(Mod, pyroClass.Name, EquipType.Legs);
                        Player.body = EquipLoader.GetEquipSlot(Mod, pyroClass.Name, EquipType.Body);
                        Player.head = EquipLoader.GetEquipSlot(Mod, pyroClass.Name, EquipType.Head);
                        Player.wings = EquipLoader.GetEquipSlot(Mod, pyroClass.Name, EquipType.Wings);
                        break;

                    case 4:
                        classSelected = true;
                        if (classHideVanity) return;
                        var demomanClass = ModContent.GetInstance<DemomanClass>();
                        Player.legs = EquipLoader.GetEquipSlot(Mod, demomanClass.Name, EquipType.Legs);
                        Player.body = EquipLoader.GetEquipSlot(Mod, demomanClass.Name, EquipType.Body);
                        Player.head = EquipLoader.GetEquipSlot(Mod, demomanClass.Name, EquipType.Head);
                        Player.wings = EquipLoader.GetEquipSlot(Mod, demomanClass.Name, EquipType.Wings);
                        break;

                    case 5:
                        classSelected = true;
                        if (classHideVanity) return;
                        var heavyClass = ModContent.GetInstance<HeavyClass>();
                        Player.legs = EquipLoader.GetEquipSlot(Mod, heavyClass.Name, EquipType.Legs);
                        Player.body = EquipLoader.GetEquipSlot(Mod, heavyClass.Name, EquipType.Body);
                        Player.head = EquipLoader.GetEquipSlot(Mod, heavyClass.Name, EquipType.Head);
                        Player.wings = EquipLoader.GetEquipSlot(Mod, heavyClass.Name, EquipType.Wings);
                        break;

                    case 6:
                        classSelected = true;
                        if (classHideVanity) return;
                        var engineerClass = ModContent.GetInstance<EngineerClass>();
                        Player.legs = EquipLoader.GetEquipSlot(Mod, engineerClass.Name, EquipType.Legs);
                        Player.body = EquipLoader.GetEquipSlot(Mod, engineerClass.Name, EquipType.Body);
                        Player.head = EquipLoader.GetEquipSlot(Mod, engineerClass.Name, EquipType.Head);
                        Player.wings = EquipLoader.GetEquipSlot(Mod, engineerClass.Name, EquipType.Wings);
                        break;

                    case 7:
                        classSelected = true;
                        if (classHideVanity) return;
                        var medicClass = ModContent.GetInstance<MedicClass>();
                        Player.legs = EquipLoader.GetEquipSlot(Mod, medicClass.Name, EquipType.Legs);
                        Player.body = EquipLoader.GetEquipSlot(Mod, medicClass.Name, EquipType.Body);
                        Player.head = EquipLoader.GetEquipSlot(Mod, medicClass.Name, EquipType.Head);
                        Player.wings = EquipLoader.GetEquipSlot(Mod, medicClass.Name, EquipType.Wings);
                        break;

                    case 8:
                        classSelected = true;
                        if (classHideVanity) return;
                        var sniperClass = ModContent.GetInstance<SniperClass>();
                        Player.legs = EquipLoader.GetEquipSlot(Mod, sniperClass.Name, EquipType.Legs);
                        Player.body = EquipLoader.GetEquipSlot(Mod, sniperClass.Name, EquipType.Body);
                        Player.head = EquipLoader.GetEquipSlot(Mod, sniperClass.Name, EquipType.Head);
                        Player.wings = EquipLoader.GetEquipSlot(Mod, sniperClass.Name, EquipType.Wings);
                        break;

                    case 9:
                        classSelected = true;
                        if (classHideVanity) return;
                        var spyClass = ModContent.GetInstance<SpyClass>();
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
            if (Player.mount.Type != ModContent.MountType<Content.Mounts.TF2Mount>())
                focus = false;
            if (metal < 0)
                metal = 0;
            if (metal > maxMetal)
                metal = maxMetal;
            if (ubercharge >= maxUbercharge)
                ubercharge = maxUbercharge;

            if (ModLoader.TryGetMod("CalamityMod", out Mod calamity))
            {
                if (calamity.TryFind("WeakPetrification", out ModBuff weakPetrification))
                {
                    if (Player.HasBuff(weakPetrification.Type) && classSelected)
                        Player.ClearBuff(weakPetrification.Type);
                    Player.buffImmune[weakPetrification.Type] = true;
                }
            }
            if (ModLoader.TryGetMod("Gensokyo", out Mod gensokyo))
            {
                if (gensokyo.TryFind("Debuff_MovementInverted", out ModBuff movementInverted))
                {
                    if (Player.HasBuff(movementInverted.Type) && classSelected)
                        Player.ClearBuff(movementInverted.Type);
                    Player.buffImmune[movementInverted.Type] = true;
                }
                if (gensokyo.TryFind("Debuff_GravityInverted", out ModBuff gravityInverted))
                {
                    if (Player.HasBuff(gravityInverted.Type) && classSelected)
                        Player.ClearBuff(gravityInverted.Type);
                    Player.buffImmune[gravityInverted.Type] = true;
                }
            }
        }

        public override void PostUpdate()
        {
            if (!initializedClass && classSelected)
            {
                Player.statLife = Player.statLifeMax2;
                metal = maxMetal;
                initializedClass = true;
            }

            if (cloakImmuneTime <= 0)
                cloakImmuneTime = 0;
            else
                cloakImmuneTime--;

            if (Player.mount.Active && Player.mount.BlockExtraJumps)
                canJumpAgain_Scout = false;
            else if ((Player.velocity.Y == 0f || Player.sliding || (Player.autoJump && Player.justJumped)) && hasJumpOption_Scout)
                canJumpAgain_Scout = true;
            else if (!hasJumpOption_Scout)
                canJumpAgain_Scout = false;

            if (Player.controlJump)
            {
                if (Player.sliding)
                    Player.autoJump = false;
                bool flag = false;
                if (Player.mount.Active && Player.mount.IsConsideredASlimeMount && Player.wetSlime > 0)
                {
                    Player.wetSlime = 0;
                    flag = true;
                }
                bool flag2 = Player.wet && Player.accFlipper;
                bool flag3 = !Player.mount.Active || !Player.mount.Cart;
                if (hasJumpOption_Scout && releaseJump && flag3)
                {
                    if (Player.sliding || Player.velocity.Y == 0f)
                        Player.justJumped = true;
                    if (canJumpAgain_Scout && !flag && !flag2)
                        canJumpAgain_Scout = false;
                    if ((Player.velocity.Y == 0f || Player.sliding || (Player.autoJump && Player.justJumped)) && hasJumpOption_Scout)
                        canJumpAgain_Scout = true;

                    if (Player.velocity.Y > 0f && jumpsLeft == extraJumps)
                        allowExtraJumps = false;
                    else if (jumpsLeft < extraJumps)
                        allowExtraJumps = true;
                    if (jumpsLeft >= 0 && allowExtraJumps)
                    {
                        isPerformingJump_Scout = true;
                        Player.velocity.Y = (0f - Player.jumpSpeed * 20f) * Player.gravDir;
                        Player.jump = Player.jumpHeight;
                        jumpsLeft--;
                    }
                }
                releaseJump = false;
            }
            else
                releaseJump = true;
            if (Grounded(Player))
            {
                allowExtraJumps = true;
                jumpsLeft = extraJumps;
            }
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (KeybindSystem.SoldierBuff.JustPressed && buffBanner && bannerType == 1 && Player.GetModPlayer<BuffBannerPlayer>().rage >= 600)
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/buff_banner_horn_red"), Player.Center);
                if (Main.netMode == NetmodeID.SinglePlayer)
                    Player.AddBuff(ModContent.BuffType<Rage>(), 600);
                else
                {
                    for (int i = 0; i < activePlayers; i++)
                    {
                        Player targetPlayer = Main.player[i];
                        targetPlayer.AddBuff(ModContent.BuffType<Rage>(), 600);
                    }
                }
            }
            if (KeybindSystem.SoldierBuff.JustPressed && buffBanner && bannerType == 2 && Player.GetModPlayer<BattalionsBackupPlayer>().rage >= 600)
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/buff_banner_horn_red"), Player.Center);
                if (Main.netMode == NetmodeID.SinglePlayer)
                    Player.AddBuff(ModContent.BuffType<DefenseRage>(), 600);
                else
                {
                    for (int i = 0; i < activePlayers; i++)
                    {
                        Player targetPlayer = Main.player[i];
                        targetPlayer.AddBuff(ModContent.BuffType<DefenseRage>(), 600);
                    }
                }
            }

            if (KeybindSystem.Cloak.JustPressed && invisWatchEquipped && !Player.HasBuff<Cloaked>())
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spy_cloak"), Player.Center);
                Player.AddBuff(ModContent.BuffType<Cloaked>(), 600);
            }
            else if (KeybindSystem.Cloak.JustPressed && invisWatchEquipped && Player.HasBuff<Cloaked>())
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spy_cloak"), Player.Center);
                Player.ClearBuff(ModContent.BuffType<Cloaked>());
            }
            if (KeybindSystem.Cloak.JustPressed && cloakandDaggerEquipped && !Player.HasBuff<CloakandDagger>() && !brokenCloak)
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spy_cloak"), Player.Center);
                Player.AddBuff(ModContent.BuffType<CloakandDagger>(), 600);
            }
            else if (KeybindSystem.Cloak.JustPressed && cloakandDaggerEquipped && Player.HasBuff<CloakandDagger>())
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/spy_cloak"), Player.Center);
                Player.ClearBuff(ModContent.BuffType<CloakandDagger>());
            }
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (ModContent.GetInstance<TF2Config>().NoTF2Loot || currentClass == 0) return;
            if (target.friendly || NPCID.Sets.CountsAsCritter[target.type] || target.type == NPCID.TargetDummy
                || proj.type == ModContent.ProjectileType<Content.Projectiles.Medic.HealingBeam>())
            return;

            if (Main.rand.NextBool(100) && currentClass == 6)
            {
                var metalSource = target.GetSource_FromAI();
                int type = ModContent.ItemType<Metal>();
                Item.NewItem(metalSource, target.Center, type);
            }

            if (ModContent.GetInstance<TF2Config>().FreeHealthPacks) return;

            if (Main.rand.NextBool(100) && currentClass > 0)
            {
                var healthSource = target.GetSource_FromAI();
                int type = ModContent.ItemType<SmallHealth>();
                Item.NewItem(healthSource, target.Center, type);
            }

            if (Main.rand.NextBool(250) && currentClass > 0)
            {
                var healthSource = target.GetSource_FromAI();
                int type = ModContent.ItemType<MediumHealth>();
                Item.NewItem(healthSource, target.Center, type);
            }

            if (Main.rand.NextBool(500) && currentClass > 0)
            {
                var healthSource = target.GetSource_FromAI();
                int type = ModContent.ItemType<LargeHealth>();
                Item.NewItem(healthSource, target.Center, type);
            }
        }

        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            if (ModContent.GetInstance<TF2Config>().NoTF2Loot || currentClass == 0) return;
            if (target.friendly || NPCID.Sets.CountsAsCritter[target.type] || target.type == NPCID.TargetDummy) return;

            if (Main.rand.NextBool(25) && currentClass == 6)
            {
                var metalSource = target.GetSource_FromAI();
                int type = ModContent.ItemType<Metal>();
                Item.NewItem(metalSource, target.Center, type);
            }

            if (ModContent.GetInstance<TF2Config>().FreeHealthPacks) return;

            if (Main.rand.NextBool(5) && currentClass > 0)
            {
                var healthSource = target.GetSource_FromAI();
                int type = ModContent.ItemType<SmallHealth>();
                Item.NewItem(healthSource, target.Center, type);
            }

            if (Main.rand.NextBool(10) && currentClass > 0)
            {
                var healthSource = target.GetSource_FromAI();
                int type = ModContent.ItemType<MediumHealth>();
                Item.NewItem(healthSource, target.Center, type);
            }

            if (Main.rand.NextBool(20) && currentClass > 0)
            {
                var healthSource = target.GetSource_FromAI();
                int type = ModContent.ItemType<LargeHealth>();
                Item.NewItem(healthSource, target.Center, type);
            }
        }

        // Rocket jumping won't instantly kill the player
        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            if ((proj.Name == "Rocket" || proj.Name == "Grenade" || proj.Name == "Stickybomb" || proj.Name == "Syringe") && proj.owner == Player.whoAmI)
                damage = 0;
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

        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            if (target.GetGlobalNPC<JarateNPC>().jarateDebuff)
                Player.GetModPlayer<TF2Player>().miniCrit = true;
            if (target.GetGlobalNPC<JarateNPC>().jarateDebuff && Player.HeldItem.ModItem is Bushwacka)
                Player.GetModPlayer<TF2Player>().crit = true;

            if (Player.GetModPlayer<TF2Player>().crit)
            {
                crit = true;
                if (item.ModItem?.Mod is TF2)
                {
                    damage = (int)(damage * 1.5f);
                    Dust.NewDust(target.Center, 50, 28, ModContent.DustType<CriticalHit>(), 0 * 0.5f, 0);
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/crit_hit"), Player.Center);
                }
            }
            else if (Player.GetModPlayer<TF2Player>().miniCrit)
            {
                if (item.ModItem?.Mod is TF2)
                {
                    damage = (int)(damage * 1.35f);
                    Dust.NewDust(target.Center, 40, 24, ModContent.DustType<MiniCrit>(), 0 * 0.5f, 0);
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/crit_hit_mini"), Player.Center);
                }
            }
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            // NPCs affected
            if (target.GetGlobalNPC<JarateNPC>().jarateDebuff)
            {
                Player.GetModPlayer<TF2Player>().miniCrit = true;
                if (proj.GetGlobalProjectile<TF2ProjectileBase>().sniperCrit)
                    damage = (int)(damage * 1.35f);
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
                crit = true;
                if (proj.ModProjectile?.Mod is TF2)
                {
                    damage = (int)(damage * 1.5f);
                    Dust.NewDust(target.Center, 50, 28, ModContent.DustType<CriticalHit>(), 0 * 0.5f, 0);
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/crit_hit"), Player.Center);
                }
            }
            else if (Player.GetModPlayer<TF2Player>().miniCrit)
            {
                if (proj.ModProjectile?.Mod is TF2)
                {
                    damage = (int)(damage * 1.5f);
                    Dust.NewDust(target.Center, 50, 28, ModContent.DustType<MiniCrit>(), 0 * 0.5f, 0);
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/crit_hit_mini"), Player.Center);
                }
            }
        }

        public override void ModifyHitPvp(Item item, Player target, ref int damage, ref bool crit)
        {
            if (target.GetModPlayer<JaratePlayer>().jarateDebuff)
                Player.GetModPlayer<TF2Player>().miniCrit = true;
            if (target.GetModPlayer<JaratePlayer>().jarateDebuff && Player.HeldItem.ModItem is Bushwacka)
                Player.GetModPlayer<TF2Player>().crit = true;

            if (Player.GetModPlayer<TF2Player>().crit)
            {
                crit = true;
                if (item.ModItem?.Mod is TF2)
                {
                    damage = (int)(damage * 1.5f);
                    Dust.NewDust(target.Center, 50, 28, ModContent.DustType<CriticalHit>(), 0 * 0.5f, 0);
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/crit_hit"), Player.Center);
                }
            }
            else if (Player.GetModPlayer<TF2Player>().miniCrit)
            {
                if (item.ModItem?.Mod is TF2)
                {
                    damage = (int)(damage * 1.35f);
                    Dust.NewDust(target.Center, 40, 24, ModContent.DustType<MiniCrit>(), 0 * 0.5f, 0);
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/crit_hit_mini"), Player.Center);
                }
            }
        }

        public override void ModifyHitPvpWithProj(Projectile proj, Player target, ref int damage, ref bool crit)
        {
            if (target.GetModPlayer<JaratePlayer>().jarateDebuff)
                Player.GetModPlayer<TF2Player>().miniCrit = true;
            if (proj.GetGlobalProjectile<TF2ProjectileBase>().spawnedFromNPC) return;
            if (Player.GetModPlayer<TF2Player>().crit)
            {
                crit = true;
                if (proj.ModProjectile?.Mod is TF2)
                {
                    damage = (int)(damage * 1.5f);
                    Dust.NewDust(target.Center, 50, 28, ModContent.DustType<CriticalHit>(), 0 * 0.5f, 0);
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/crit_hit"), Player.Center);
                }
            }
            else if (Player.GetModPlayer<TF2Player>().miniCrit)
            {
                if (proj.ModProjectile?.Mod is TF2)
                {
                    damage = (int)(damage * 1.5f);
                    Dust.NewDust(target.Center, 50, 28, ModContent.DustType<MiniCrit>(), 0 * 0.5f, 0);
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/crit_hit_mini"), Player.Center);
                }
            }
        }
    }
}