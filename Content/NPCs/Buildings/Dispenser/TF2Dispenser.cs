using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TF2.Common;
using TF2.Content.Buffs;
using TF2.Content.Items;
using TF2.Content.Items.Weapons.Spy;
using static TF2.TF2;

namespace TF2.Content.NPCs.Buildings.Dispenser
{
    public abstract class TF2Dispenser : Building
    {
        public int ReservedMetal
        {
            get => (int)NPC.localAI[1];
            set => NPC.localAI[1] = value;
        }

        public virtual int MaxReservedMetal => 400;

        public override string BuildingName => Language.GetTextValue("Mods.TF2.NPCs.Dispenser");

        public override int BuildingCooldown => Time(21);

        public override int BuildingCooldownHauled => Time(5.25);

        protected override int ScrapMetalAmount => 50;

        protected bool playDispenserSound;

        protected virtual void DispenserSpawn()
        { }

        protected virtual void DispenserAI()
        { }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true
            });
            NPC.netAlways = true;
        }

        protected override void BuildingSpawn()
        {
            UpgradeCooldown = BuildingCooldown;
            DispenserSpawn();
        }

        protected override void BuildingAI()
        {
            if (!Initialized)
            {
                int buildDuration = !hauled ? BuildingCooldown : BuildingCooldownHauled;
                int health = Round(Utils.Lerp(InitialHealth * Player.GetModPlayer<TF2Player>().healthMultiplier, NPC.lifeMax, (float)(buildDuration - UpgradeCooldown) / buildDuration)) - preConstructedDamage;
                if (InitialHealth != Round(NPC.lifeMax / Player.GetModPlayer<TF2Player>().healthMultiplier))
                    NPC.life = health;
                if (NPC.life < 0)
                    NPC.checkDead();
                UpgradeCooldown -= constructionSpeed;
                if (UpgradeCooldown < 0)
                    UpgradeCooldown = 0;
                if (UpgradeCooldown <= 0)
                {
                    NPC.life = NPC.lifeMax - preConstructedDamage;
                    Initialized = true;
                    NPC.netUpdate = true;
                }
                return;
            }
            if (UpgradeCooldown > 0)
            {
                Timer = 0;
                Timer2 = 0;
                return;
            }
            UpgradeCooldown--;
            if (UpgradeCooldown < 0)
                UpgradeCooldown = 0;
            DispenserAI();
        }

        protected override void BuildingDestroy()
        {
            TF2Player p = Player.GetModPlayer<TF2Player>();
            if (p.currentClass == TF2Item.Engineer)
                p.dispenserWhoAmI = -1;
            SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Voicelines/engineer_autodestroyeddispenser01"), Player.Center);
        }

        public void HaulDispenser(int metal, int reservedMetal)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                UpgradeCooldown = BuildingCooldownHauled;
                hauled = true;
            }
            else
            {
                ModPacket packet = ModContent.GetInstance<TF2>().GetPacket();
                packet.Write((byte)MessageType.SyncDispenser);
                packet.Write((byte)NPC.whoAmI);
                packet.Write(metal);
                packet.Write(reservedMetal);
                packet.Send(-1, Main.myPlayer);
            }
        }

        public void SyncReservedMetal(int metalCost)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                ReservedMetal -= metalCost;
            else
            {
                ModPacket packet = ModContent.GetInstance<TF2>().GetPacket();
                packet.Write((byte)MessageType.SyncReservedMetal);
                packet.Write((byte)NPC.whoAmI);
                packet.Write(metalCost);
                packet.Send(-1, Main.myPlayer);
            }
        }

        protected override void BuildingSendExtraAI(BinaryWriter writer)
        {
            writer.Write(ReservedMetal);
            writer.Write(playDispenserSound);
        }

        protected override void BuildingReceiveExtraAI(BinaryReader binaryReader)
        {
            ReservedMetal = binaryReader.ReadInt32();
            playDispenserSound = binaryReader.ReadBoolean();
        }
    }

    public class DispenserStatistics
    {
        public int Type { get; set; }

        public int Metal { get; set; }

        public int ReservedMetal { get; set; }
    }

    public class DispenserLevel1 : TF2Dispenser
    {
        protected override Asset<Texture2D> BuildingTexture => BuildingTextures.DispenserLevel1Texture;

        public override void SetDefaults()
        {
            NPC.width = 34;
            NPC.height = 36;
            NPC.aiStyle = -1;
            NPC.lifeMax = 150;
            NPC.knockBackResist = 0f;
            NPC.friendly = true;
            NPC.HitSound = new SoundStyle("TF2/Content/Sounds/SFX/Weapons/wrench_hit_build_success");
            NPC.DeathSound = new SoundStyle("TF2/Content/Sounds/SFX/NPCs/dispenser_explode");
        }

        protected override void DispenserSpawn()
        {
            ReservedMetal = 25;
            Timer = Time(5);
        }

        protected override void DispenserAI()
        {
            UpgradeBuilding(ModContent.NPCType<DispenserLevel2>());
            Timer++;
            Timer2++;
            if (Timer >= Time(5) && ReservedMetal < MaxReservedMetal)
            {
                ReservedMetal += 40;
                ReservedMetal = Utils.Clamp(ReservedMetal, 0, MaxReservedMetal);
                Timer = 0;
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/NPCs/dispenser_generate_metal"), NPC.Center);
            }
            bool healed = false;
            if (Timer2 >= Time(1))
            {
                foreach (Player player in Main.ActivePlayers)
                {
                    if (NPC.Distance(player.Center) <= 100f && !player.dead && !player.hostile)
                    {
                        TF2Player p = player.GetModPlayer<TF2Player>();
                        player.Heal(GetHealth(player, 10));
                        AddAmmo(player, 20);
                        if (player.GetModPlayer<CloakPlayer>().invisWatchEquipped)
                            player.GetModPlayer<CloakPlayer>().cloakMeter = Time(0.5);
                        if (player.GetModPlayer<CloakAndDaggerPlayer>().cloakAndDaggerEquipped && !player.HasBuff<CloakAndDaggerBuff>())
                            player.GetModPlayer<CloakAndDaggerPlayer>().cloakMeter += Time(0.5);
                        if (player.GetModPlayer<FeignDeathPlayer>().deadRingerEquipped)
                            player.GetModPlayer<FeignDeathPlayer>().cloakMeter = Time(0.7);
                        if (p.currentClass == TF2Item.Engineer)
                        {
                            int metalCost = ReservedMetal >= 40 ? 40 : ReservedMetal;
                            if (metalCost > (200 - p.metal))
                                metalCost = 200 - p.metal;
                            p.metal += metalCost;
                            SyncReservedMetal(metalCost);
                        }
                        healed = true;
                    }
                }
                if (healed)
                {
                    if (!playDispenserSound)
                    {
                        SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/medigun_heal"), NPC.Center);
                        playDispenserSound = true;
                    }
                    Timer2 = 0;
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/ammo_pickup"), NPC.Center);
                }
                else
                    playDispenserSound = false;
            }
        }
    }

    public class DispenserLevel2 : TF2Dispenser
    {
        protected override Asset<Texture2D> BuildingTexture => BuildingTextures.DispenserLevel2Texture;

        public override int InitialHealth => 180;

        public override int BuildingCooldown => Time(1.6);

        public override void SetDefaults()
        {
            NPC.width = 34;
            NPC.height = 36;
            NPC.aiStyle = -1;
            NPC.lifeMax = 180;
            NPC.knockBackResist = 0f;
            NPC.friendly = true;
            NPC.HitSound = new SoundStyle("TF2/Content/Sounds/SFX/Weapons/wrench_hit_build_success");
            NPC.DeathSound = new SoundStyle("TF2/Content/Sounds/SFX/NPCs/dispenser_explode");
        }

        protected override void DispenserSpawn() => Timer = Time(5);

        protected override void DispenserAI()
        {
            UpgradeBuilding(ModContent.NPCType<DispenserLevel3>());
            Timer++;
            Timer2++;
            if (Timer >= Time(5) && ReservedMetal < MaxReservedMetal)
            {
                ReservedMetal += 50;
                ReservedMetal = Utils.Clamp(ReservedMetal, 0, MaxReservedMetal);
                Timer = 0;
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/NPCs/dispenser_generate_metal"), NPC.Center);
            }
            bool healed = false;
            if (Timer2 >= Time(1))
            {
                foreach (Player player in Main.ActivePlayers)
                {
                    if (NPC.Distance(player.Center) <= 100f && !player.dead && !player.hostile)
                    {
                        TF2Player p = player.GetModPlayer<TF2Player>();
                        player.Heal(GetHealth(player, 15));
                        AddAmmo(player, 30);
                        if (player.GetModPlayer<CloakPlayer>().invisWatchEquipped)
                            player.GetModPlayer<CloakPlayer>().cloakMeter = Time(1);
                        if (player.GetModPlayer<CloakAndDaggerPlayer>().cloakAndDaggerEquipped && !player.HasBuff<CloakAndDaggerBuff>())
                            player.GetModPlayer<CloakAndDaggerPlayer>().cloakMeter += Time(1);
                        if (player.GetModPlayer<FeignDeathPlayer>().deadRingerEquipped)
                            player.GetModPlayer<FeignDeathPlayer>().cloakMeter = Time(1.4);
                        if (p.currentClass == TF2Item.Engineer)
                        {
                            int metalCost = ReservedMetal >= 50 ? 50 : ReservedMetal;
                            if (metalCost > (200 - p.metal))
                                metalCost = 200 - p.metal;
                            p.metal += metalCost;
                            SyncReservedMetal(metalCost);
                        }
                        healed = true;
                    }
                }
                if (healed)
                {
                    if (!playDispenserSound)
                    {
                        SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/medigun_heal"), NPC.Center);
                        playDispenserSound = true;
                    }
                    Timer2 = 0;
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/ammo_pickup"), NPC.Center);
                }
                else
                    playDispenserSound = false;
            }
        }
    }

    public class DispenserLevel3 : TF2Dispenser
    {
        protected override Asset<Texture2D> BuildingTexture => BuildingTextures.DispenserLevel3Texture;

        public override int InitialHealth => 216;

        public override int BuildingCooldown => Time(1.6);

        public override void SetDefaults()
        {
            NPC.width = 42;
            NPC.height = 34;
            NPC.aiStyle = -1;
            NPC.lifeMax = 216;
            NPC.knockBackResist = 0f;
            NPC.friendly = true;
            NPC.HitSound = new SoundStyle("TF2/Content/Sounds/SFX/Weapons/wrench_hit_build_success");
            NPC.DeathSound = new SoundStyle("TF2/Content/Sounds/SFX/NPCs/dispenser_explode");
        }

        protected override void DispenserSpawn() => Timer = Time(5);

        protected override void DispenserAI()
        {
            Timer++;
            Timer2++;
            if (Timer >= Time(5) && ReservedMetal < MaxReservedMetal)
            {
                ReservedMetal += 60;
                ReservedMetal = Utils.Clamp(ReservedMetal, 0, MaxReservedMetal);
                Timer = 0;
                SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/NPCs/dispenser_generate_metal"), NPC.Center);
            }
            bool healed = false;
            if (Timer2 >= Time(1))
            {
                foreach (Player player in Main.ActivePlayers)
                {
                    if (NPC.Distance(player.Center) <= 100f && !player.dead && !player.hostile)
                    {
                        TF2Player p = player.GetModPlayer<TF2Player>();
                        player.Heal(GetHealth(player, 20));
                        AddAmmo(player, 40);
                        if (player.GetModPlayer<CloakPlayer>().invisWatchEquipped)
                            player.GetModPlayer<CloakPlayer>().cloakMeter = Time(1.5);
                        if (player.GetModPlayer<CloakAndDaggerPlayer>().cloakAndDaggerEquipped && !player.HasBuff<CloakAndDaggerBuff>())
                            player.GetModPlayer<CloakAndDaggerPlayer>().cloakMeter += Time(1.5);
                        if (player.GetModPlayer<FeignDeathPlayer>().deadRingerEquipped)
                            player.GetModPlayer<FeignDeathPlayer>().cloakMeter = Time(2.1);
                        if (p.currentClass == TF2Item.Engineer)
                        {
                            int metalCost = ReservedMetal >= 60 ? 60 : ReservedMetal;
                            if (metalCost > (200 - p.metal))
                                metalCost = 200 - p.metal;
                            p.metal += metalCost;
                            SyncReservedMetal(metalCost);
                        }
                        healed = true;
                    }
                }
                if (healed)
                {
                    if (!playDispenserSound)
                    {
                        SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/Weapons/medigun_heal"), NPC.Center);
                        playDispenserSound = true;
                    }
                    Timer2 = 0;
                    SoundEngine.PlaySound(new SoundStyle("TF2/Content/Sounds/SFX/ammo_pickup"), NPC.Center);
                }
                else
                    playDispenserSound = false;
            }
        }
    }
}