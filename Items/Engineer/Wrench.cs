using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using System.Linq;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.Chat;

namespace TF2.Items.Engineer
{
    public class Wrench : TF2WeaponNoAmmo
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Engineer's Starter Melee\n"
                             + "Right click to build a sentry gun\n"
                             + "Sentry guns attack the closest enemy\n"
                             + "Sentry gun level depends on the amount of metal the player currently has");
        }

        public override void SafeSetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.scale = 1f;
            Item.useTime = 48;
            Item.useAnimation = 48;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = false;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.shoot = ModContent.ProjectileType<WrenchHitbox>();

            Item.damage = 65;
            Item.knockBack = -1;
            Item.crit = 0;
        }

        int activePlayers = Main.player.Take(Main.maxPlayers).Count(x => x.active);

        int npc;

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse != 2)
            {
                SoundEngine.PlaySound(new SoundStyle("TF2/Sounds/SFX/wrench_swing"), player.Center);
            }
            return true;
        }

        public override bool AltFunctionUse(Player player)
        {
            
            if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("NPCs such as buildings and mercenaries are unsupported in multiplayer!"), Color.White);
                return false;
            }
            
            if (player.whoAmI == Main.myPlayer)
            {
                TFClass p = player.GetModPlayer<TFClass>();
                var source = player.GetSource_FromThis();
                if (NPC.CountNPCS(ModContent.NPCType<NPCs.SentryLevel1>()) <= activePlayers && NPC.CountNPCS(ModContent.NPCType<NPCs.SentryLevel2>()) <= activePlayers && NPC.CountNPCS(ModContent.NPCType<NPCs.SentryLevel3>()) <= activePlayers) //&& Main.netMode != NetmodeID.MultiplayerClient
                {
                    if (p.metal >= 130 && p.metal < 330)
                    {
                        npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<NPCs.SentryLevel1>(), player.whoAmI);
                        NPCs.SentryLevel1 spawnedModNPC = Main.npc[npc].ModNPC as NPCs.SentryLevel1;
                        spawnedModNPC.npcOwner = player.whoAmI;
                        NetMessage.SendData(MessageID.SyncNPC, number: npc);
                        p.metal -= 130;
                        SoundEngine.PlaySound(new SoundStyle("TF2/Sounds/SFX/spawn_item"), player.Center);
                    }
                    else if (p.metal >= 330 && p.metal < 530)
                    {
                        npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<NPCs.SentryLevel2>(), player.whoAmI);
                        NPCs.SentryLevel2 spawnedModNPC = Main.npc[npc].ModNPC as NPCs.SentryLevel2;
                        spawnedModNPC.npcOwner = player.whoAmI;
                        NetMessage.SendData(MessageID.SyncNPC, number: npc);
                        p.metal -= 330;
                        SoundEngine.PlaySound(new SoundStyle("TF2/Sounds/SFX/spawn_item"), player.Center);
                    }
                    else if (p.metal >= 530)
                    {
                        npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<NPCs.SentryLevel3>(), player.whoAmI);
                        NPCs.SentryLevel3 spawnedModNPC = Main.npc[npc].ModNPC as NPCs.SentryLevel3;
                        spawnedModNPC.npcOwner = player.whoAmI;
                        NetMessage.SendData(MessageID.SyncNPC, number: npc);
                        p.metal -= 530;
                        SoundEngine.PlaySound(new SoundStyle("TF2/Sounds/SFX/spawn_item"), player.Center);
                    }
                }
                else
                {
                    if (p.metal >= 130 && p.metal < 330)
                    {
                        if (NPC.CountNPCS(ModContent.NPCType<NPCs.SentryLevel1>()) == 0) { return false; }
                        NPCs.SentryLevel1 currentModNPC = Main.npc[npc].ModNPC as NPCs.SentryLevel1;
                        currentModNPC.Kill();

                        npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<NPCs.SentryLevel1>(), player.whoAmI);
                        NPCs.SentryLevel1 spawnedModNPC = Main.npc[npc].ModNPC as NPCs.SentryLevel1;
                        spawnedModNPC.npcOwner = player.whoAmI;
                        NetMessage.SendData(MessageID.SyncNPC, number: npc);
                        p.metal -= 65;
                        SoundEngine.PlaySound(new SoundStyle("TF2/Sounds/SFX/spawn_item"), player.Center);
                    }
                    else if (p.metal >= 330 && p.metal < 530)
                    {
                        if (NPC.CountNPCS(ModContent.NPCType<NPCs.SentryLevel2>()) == 0) { return false; }
                        NPCs.SentryLevel2 currentModNPC = Main.npc[npc].ModNPC as NPCs.SentryLevel2;
                        currentModNPC.Kill();

                        npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<NPCs.SentryLevel2>(), player.whoAmI);
                        NPCs.SentryLevel2 spawnedModNPC = Main.npc[npc].ModNPC as NPCs.SentryLevel2;
                        spawnedModNPC.npcOwner = player.whoAmI;
                        NetMessage.SendData(MessageID.SyncNPC, number: npc);
                        p.metal -= 165;
                        SoundEngine.PlaySound(new SoundStyle("TF2/Sounds/SFX/spawn_item"), player.Center);
                    }
                    else if (p.metal >= 530)
                    {
                        if (NPC.CountNPCS(ModContent.NPCType<NPCs.SentryLevel3>()) == 0) { return false; }
                        NPCs.SentryLevel3 currentModNPC = Main.npc[npc].ModNPC as NPCs.SentryLevel3;
                        currentModNPC.Kill();

                        npc = NPC.NewNPC(source, (int)player.position.X, (int)player.position.Y, ModContent.NPCType<NPCs.SentryLevel3>(), player.whoAmI);
                        NPCs.SentryLevel3 spawnedModNPC = Main.npc[npc].ModNPC as NPCs.SentryLevel3;
                        spawnedModNPC.npcOwner = player.whoAmI;
                        NetMessage.SendData(MessageID.SyncNPC, number: npc);
                        p.metal -= 265;
                        SoundEngine.PlaySound(new SoundStyle("TF2/Sounds/SFX/spawn_item"), player.Center);
                    }
                }
            }

            return false;
        }
    }

    public class WrenchHitbox : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.hostile = true;
            Projectile.Name = "Wrench_TF2";
            Projectile.alpha = 255;
            Projectile.damage = 0;
            Projectile.timeLeft = 1;
        }

        public override void AI()
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.velocity = Main.player[Main.myPlayer].velocity;
            }
        }

        public override bool CanHitPlayer(Player player)
        {
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return target.GetGlobalNPC<TF2GlobalNPC>().building;
        }
    }
}
