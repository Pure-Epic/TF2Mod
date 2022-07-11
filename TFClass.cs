//using TF2.Buffs;
//using TF2.Dusts;
using TF2.Items.Armor;
using TF2.Items.Ammo;
//using TF2.NPCs;
//using TF2.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Audio;
using TF2.Items;
using TF2.NPCs;

namespace TF2
{
    public class TFClass : ModPlayer
    {
        public float classMultiplier = 0.5f;
        public int pierce = 1;

        public bool classAccessoryPrevious;
        public bool classAccessory;
        public bool classHideVanity;
        public bool classForceVanity;
        public bool classPower;
        public bool classSelected;
        public bool nullified;
        public bool activateUbercharge = false;

        public float stickybombCharge = 0;
        public float stickybombMaxCharge = 0;
        public float stickybombChargeTimer = 0;
        public int stickybombAmount = 0;
        public int stickybombMax;

        public int metal;
        public int maxMetal = 1000;

        public int healAmount;
        public float ubercharge = 0;
        public float maxUbercharge = 100;
        public int uberchargeTime = 0;

        public float sniperCharge = 0;
        public float sniperMaxCharge = 0;
        public float sniperChargeTimer = 0;

        public bool backStab;

        //class selection
        public int currentClass;

        public bool invisWatchEquipped;
        public bool cloakCooldown;

        public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath) => new Item[] { new Item(ModContent.ItemType<Australium>(), 1), new Item(ModContent.ItemType<Items.Consumables.SaxtonHaleSummon>(), 1) };

        // This is called before accessories' effects
        public override void OnEnterWorld(Player player) 
        {
            metal = maxMetal;
            player.statLife = player.statLifeMax2;
            
            // doesn't work in multiplayer
            /*
            if (Main.netMode == NetmodeID.Server) { return; }
            if (!SaxtonHaleSpawn.saxtonHaleSummoned)
            {
                var source = player.GetSource_FromThis();
                NetMessage.SendData(MessageID.SyncNPC, number: NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<NPCs.TownNPCs.SaxtonHale>(), player.whoAmI));
                SaxtonHaleSpawn.saxtonHaleSummoned = true;
            }
            */
        }

        public override void OnRespawn(Player player)
        {
            ubercharge = 0;
            metal = maxMetal;
            player.statLife = player.statLifeMax2;
        }

        public override void ResetEffects()
        {
            Player.GetDamage<TF2DamageClass>() *= classMultiplier;
            classAccessoryPrevious = classAccessory;
            classAccessory = classHideVanity = classForceVanity = classPower = false;
            currentClass = 0;
            classMultiplier = 0.5f;
            maxMetal = 200;
            pierce = 1;
            classSelected = false;
            invisWatchEquipped = false;
            cloakCooldown = false;
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
            if (classAccessory == true && classSelected == false) //classAccessory == true && classSelected == false
            {
                switch (currentClass)
                {
                    case 1:
                        classSelected = true;
                        if (classHideVanity) { return; }
                        var scoutClass = ModContent.GetInstance<ScoutClass>();
                        Player.legs = EquipLoader.GetEquipSlot(Mod, scoutClass.Name, EquipType.Legs);
                        Player.body = EquipLoader.GetEquipSlot(Mod, scoutClass.Name, EquipType.Body);
                        Player.head = EquipLoader.GetEquipSlot(Mod, scoutClass.Name, EquipType.Head);
                        Player.wings = EquipLoader.GetEquipSlot(Mod, scoutClass.Name, EquipType.Wings);
                        break;

                    case 2:
                        classSelected = true;
                        if (classHideVanity) { return; }
                        var soldierClass = ModContent.GetInstance<SoldierClass>();
                        Player.legs = EquipLoader.GetEquipSlot(Mod, soldierClass.Name, EquipType.Legs);
                        Player.body = EquipLoader.GetEquipSlot(Mod, soldierClass.Name, EquipType.Body);
                        Player.head = EquipLoader.GetEquipSlot(Mod, soldierClass.Name, EquipType.Head);
                        Player.wings = EquipLoader.GetEquipSlot(Mod, soldierClass.Name, EquipType.Wings);
                        break;

                    case 3:
                        classSelected = true;
                        if (classHideVanity) { return; }
                        var pyroClass = ModContent.GetInstance<PyroClass>();
                        Player.legs = EquipLoader.GetEquipSlot(Mod, pyroClass.Name, EquipType.Legs);
                        Player.body = EquipLoader.GetEquipSlot(Mod, pyroClass.Name, EquipType.Body);
                        Player.head = EquipLoader.GetEquipSlot(Mod, pyroClass.Name, EquipType.Head);
                        Player.wings = EquipLoader.GetEquipSlot(Mod, pyroClass.Name, EquipType.Wings);
                        break;

                    case 4:
                        classSelected = true;
                        if (classHideVanity) { return; }
                        var demomanClass = ModContent.GetInstance<DemomanClass>();
                        Player.legs = EquipLoader.GetEquipSlot(Mod, demomanClass.Name, EquipType.Legs);
                        Player.body = EquipLoader.GetEquipSlot(Mod, demomanClass.Name, EquipType.Body);
                        Player.head = EquipLoader.GetEquipSlot(Mod, demomanClass.Name, EquipType.Head);
                        Player.wings = EquipLoader.GetEquipSlot(Mod, demomanClass.Name, EquipType.Wings);
                        break;

                    case 5:
                        classSelected = true;
                        if (classHideVanity) { return; }
                        var heavyClass = ModContent.GetInstance<HeavyClass>();
                        Player.legs = EquipLoader.GetEquipSlot(Mod, heavyClass.Name, EquipType.Legs);
                        Player.body = EquipLoader.GetEquipSlot(Mod, heavyClass.Name, EquipType.Body);
                        Player.head = EquipLoader.GetEquipSlot(Mod, heavyClass.Name, EquipType.Head);
                        Player.wings = EquipLoader.GetEquipSlot(Mod, heavyClass.Name, EquipType.Wings);
                        break;

                    case 6:
                        classSelected = true;
                        if (classHideVanity) { return; }
                        var engineerClass = ModContent.GetInstance<EngineerClass>();
                        Player.legs = EquipLoader.GetEquipSlot(Mod, engineerClass.Name, EquipType.Legs);
                        Player.body = EquipLoader.GetEquipSlot(Mod, engineerClass.Name, EquipType.Body);
                        Player.head = EquipLoader.GetEquipSlot(Mod, engineerClass.Name, EquipType.Head);
                        Player.wings = EquipLoader.GetEquipSlot(Mod, engineerClass.Name, EquipType.Wings);
                        break;

                    case 7:
                        classSelected = true;
                        if (classHideVanity) { return; }
                        var medicClass = ModContent.GetInstance<MedicClass>();
                        Player.legs = EquipLoader.GetEquipSlot(Mod, medicClass.Name, EquipType.Legs);
                        Player.body = EquipLoader.GetEquipSlot(Mod, medicClass.Name, EquipType.Body);
                        Player.head = EquipLoader.GetEquipSlot(Mod, medicClass.Name, EquipType.Head);
                        Player.wings = EquipLoader.GetEquipSlot(Mod, medicClass.Name, EquipType.Wings);
                        break;

                    case 8:
                        classSelected = true;
                        if (classHideVanity) { return; }
                        var sniperClass = ModContent.GetInstance<SniperClass>();
                        Player.legs = EquipLoader.GetEquipSlot(Mod, sniperClass.Name, EquipType.Legs);
                        Player.body = EquipLoader.GetEquipSlot(Mod, sniperClass.Name, EquipType.Body);
                        Player.head = EquipLoader.GetEquipSlot(Mod, sniperClass.Name, EquipType.Head);
                        Player.wings = EquipLoader.GetEquipSlot(Mod, sniperClass.Name, EquipType.Wings);
                        break;

                    case 9:
                        classSelected = true;
                        if (classHideVanity) { return; }
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
            {
                Nullify();
            }

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
            healAmount++;
            if (nullified)
            {
                Nullify();
            }
            if (healAmount >= 60 && currentClass == 7)
            {
                Player.statLife += (int)(3 * classMultiplier);
                healAmount = 0;
            }
        }

        public override void PreUpdate()
        {
            if (ubercharge >= maxUbercharge)
            {
                ubercharge = maxUbercharge;
            }
            if (Player.statLife < 1)
            {
                Player.statLife = 0;
            }
            if (metal < 0)
            {
                metal = 0;
            }
            if (metal > maxMetal)
            {
                metal = maxMetal;
            }
        }
        public override void PostUpdate()
        {
            if (activateUbercharge == true)
            {
                uberchargeTime++;
                Player.AddBuff(ModContent.BuffType<Buffs.Ubercharge>(), 1);
                if (uberchargeTime >= 600)
                {
                    uberchargeTime = 0;
                    ubercharge = 0;
                    activateUbercharge = false;
                }
            }
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (KeybindSystem.Cloak.JustPressed && invisWatchEquipped && !cloakCooldown)
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Sounds/SFX/spy_cloak"), Player.Center);
                Player.AddBuff(ModContent.BuffType<Buffs.Cloaked>(), 600);
                Player.AddBuff(ModContent.BuffType<Buffs.CloakCooldown>(), 1800);
            }
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (target.friendly || NPCID.Sets.CountsAsCritter[target.type] || target.type == NPCID.TargetDummy
                || proj.type == ModContent.ProjectileType<Projectiles.Medic.HealingBeam>())
            { return; }

            if (Main.rand.NextBool(100) && currentClass == 6)
            {
                var metalSource = target.GetSource_FromAI();
                int type = ModContent.ItemType<Metal>();
                Item.NewItem(metalSource, target.Center, type);
            }

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
            if (target.friendly || NPCID.Sets.CountsAsCritter[target.type] || target.type == NPCID.TargetDummy) { return; }

            if (Main.rand.NextBool(25) && currentClass == 6)
            {
                var metalSource = target.GetSource_FromAI();
                int type = ModContent.ItemType<Metal>();
                Item.NewItem(metalSource, target.Center, type);
            }

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
            if (proj.Name == "Rocket" || proj.Name == "Grenade" || proj.Name == "Stickybomb")
            {
                damage = 0;
            }
        }
    }
}
