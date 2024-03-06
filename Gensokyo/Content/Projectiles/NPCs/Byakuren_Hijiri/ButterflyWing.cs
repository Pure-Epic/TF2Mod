using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Gensokyo.Content.NPCs.Byakuren_Hijiri;

namespace TF2.Gensokyo.Content.Projectiles.NPCs.Byakuren_Hijiri
{
    [ExtendsFromMod("Gensokyo")]
    public class ButterflyWing : ModProjectile
    {
        public int ProjectileAI
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private int Owner => Projectile.owner;

        public bool projectileInitialized;
        private int timer;
        private Vector2 center;
        private Vector2 velocity = Vector2.Zero;
        private float rotation;
        private bool maxRotation;
        private int angle;
        private float offset;
        private bool maxOffset;
        private bool startTimer;
        private int burstCounter;

        public override void SetDefaults()
        {
            Projectile.width = 104;
            Projectile.height = 104;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 drawPos = Projectile.oldPos[i] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return true;
        }

        public override bool PreAI()
        {
            if ((ByakurenHijiri)Main.npc[Owner].ModNPC == null) return false;
            NPC npc = Main.npc[Owner];
            if (npc == null) return true;
            center = Projectile.Center;
            if (center == npc.Center + new Vector2(-200f, -200f))
                velocity = new Vector2(-2f, -2f);
            else if (center == npc.Center + new Vector2(200f, -200f))
                velocity = new Vector2(2f, -2f);
            else if (center == npc.Center + new Vector2(-500f, 375f))
                velocity = new Vector2(-5f, 3.75f);
            else if (center == npc.Center + new Vector2(500f, 375f))
                velocity = new Vector2(5f, 3.75f);
            projectileInitialized = true;
            return true;
        }

        public override void AI()
        {
            if (rotation >= 15f)
                maxRotation = true;
            else if (rotation <= -15f)
                maxRotation = false;
            if (rotation < 15f && !maxRotation)
                rotation += 0.5f;
            else if (maxRotation)
                rotation -= 0.5f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(rotation);
            ByakurenHijiri npc = (ByakurenHijiri)Main.npc[Owner].ModNPC;

            if (npc.State == 0 || !npc.NPC.active)
                Projectile.Kill();

            switch (npc.Stage)
            {
                case 1:
                    if (npc.Phase == ByakurenHijiri.GetBasicAttackPhase && timer % 45 == 0)
                    {
                        SoundEngine.PlaySound(new SoundStyle("TF2/Gensokyo/Content/Sounds/SFX/laser"), npc.NPC.Center);
                        if (Main.netMode == NetmodeID.MultiplayerClient) return;
                        Vector2 velocity = Projectile.DirectionTo(Main.player[npc.NPC.target].Center);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<PreMysticFragranceofaMakaiButterfly2>(), 35, 0f, npc.NPC.target);
                    }
                    break;

                case 2:
                    if (timer >= 500 && timer % 60 == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item9, npc.NPC.Center);
                        if (Main.netMode == NetmodeID.MultiplayerClient) return;
                        Vector2 velocity = Projectile.DirectionTo(Main.player[npc.NPC.target].Center) * 5f;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<StarMaelstrom2>(), 40, 0f, npc.NPC.target);
                    }
                    break;

                case 3:
                    if (npc.Phase == ByakurenHijiri.SpellcardAttackPhase)
                    {
                        if (timer == 0)
                        {
                            angle = 0;
                            offset = 0;
                        }
                        if (timer < 120)
                            Projectile.position += velocity;
                        if (timer >= 120 && timer <= 210)
                            Projectile.position.Y -= 5f;
                        if (timer >= 135)
                        {
                            angle++;
                            if (timer % 15 == 0)
                            {
                                SoundEngine.PlaySound(SoundID.Item9, npc.NPC.Center);
                                if (Main.netMode == NetmodeID.MultiplayerClient) return;
                                Vector2 velocity = Utils.RotatedBy(-Vector2.UnitY, MathHelper.ToRadians(angle + offset)) * 10f;
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<DevilsRecitation2>(), 25, 0f, npc.NPC.target);
                                velocity = Utils.RotatedBy(-Vector2.UnitY, MathHelper.ToRadians(-angle + offset)) * 10f;
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<DevilsRecitation2>(), 25, 0f, npc.NPC.target);
                            }
                            if (angle >= 110)
                            {
                                angle = 110;
                                if (offset >= 15)
                                    maxOffset = true;
                                else if (offset <= -15)
                                    maxOffset = false;
                                if (offset < 15f && !maxOffset)
                                    offset += 0.5f;
                                else if (maxOffset)
                                    offset -= 0.5f;
                                if (timer % 15 == 0)
                                {
                                    SoundEngine.PlaySound(SoundID.Item9, npc.NPC.Center);
                                    if (Main.netMode == NetmodeID.MultiplayerClient) return;
                                    Vector2 velocity = Utils.RotatedBy(Vector2.UnitY, MathHelper.ToRadians(offset)) * 10f;
                                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<DevilsRecitation2>(), 25, 0f, npc.NPC.target);
                                }
                            }
                        }
                        if (timer == 1200)
                        {
                            if (Main.netMode == NetmodeID.MultiplayerClient) return;
                            int projectile = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.UnitY, ModContent.ProjectileType<DevilsRecitation6>(), 75, 0f, npc.NPC.target);
                            Main.projectile[projectile].owner = npc.NPC.whoAmI;
                            DevilsRecitation6 laser = (DevilsRecitation6)Main.projectile[projectile].ModProjectile;
                            if (velocity == new Vector2(-2f, -2f))
                                ProjectileAI = 0;
                            else if (velocity == new Vector2(2f, -2f))
                                ProjectileAI = 1;
                            else if (velocity == new Vector2(-5f, 3.75f))
                                ProjectileAI = 2;
                            else if (velocity == new Vector2(5f, 3.75f))
                                ProjectileAI = 3;
                            laser.ProjectileAI = ProjectileAI;
                            NetMessage.SendData(MessageID.SyncProjectile, number: projectile);
                        }
                    }
                    else
                    {
                        center = Projectile.Center;
                        if ((center == npc.NPC.Center + new Vector2(-200f, -200f)) && (timer == 60 || timer == 300 || timer == 540 || timer == 780 || timer == 1020))
                        {
                            SoundEngine.PlaySound(new SoundStyle("TF2/Gensokyo/Content/Sounds/SFX/shoot"), npc.NPC.Center);
                            if (Main.netMode == NetmodeID.MultiplayerClient) return;
                            for (int i = 0; i < 32; i++)
                            {
                                Vector2 velocity = Utils.RotatedBy(Vector2.UnitX, MathHelper.ToRadians(i * 11.25f)) * 5f;
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<PreStarMaelstrom5>(), 30, 0f, npc.NPC.target);
                            }
                            startTimer = true;
                        }
                        else if ((center == npc.NPC.Center + new Vector2(200f, -200f)) && (timer == 120 || timer == 360 || timer == 600 || timer == 840 || timer == 1080))
                        {
                            SoundEngine.PlaySound(new SoundStyle("TF2/Gensokyo/Content/Sounds/SFX/shoot"), npc.NPC.Center);
                            if (Main.netMode == NetmodeID.MultiplayerClient) return;
                            for (int i = 0; i < 32; i++)
                            {
                                Vector2 velocity = Utils.RotatedBy(Vector2.UnitX, MathHelper.ToRadians(i * 11.25f)) * 5f;
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<PreStarMaelstrom5>(), 30, 0f, npc.NPC.target);
                            }
                            startTimer = true;
                        }
                        else if ((center == npc.NPC.Center + new Vector2(-500f, 375f)) && (timer == 180 || timer == 420 || timer == 660 || timer == 900 || timer == 1140))
                        {
                            SoundEngine.PlaySound(new SoundStyle("TF2/Gensokyo/Content/Sounds/SFX/shoot"), npc.NPC.Center);
                            if (Main.netMode == NetmodeID.MultiplayerClient) return;
                            for (int i = 0; i < 32; i++)
                            {
                                Vector2 velocity = Utils.RotatedBy(Vector2.UnitX, MathHelper.ToRadians(i * 11.25f)) * 5f;
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<PreStarMaelstrom6>(), 30, 0f, npc.NPC.target);
                            }
                            startTimer = true;
                        }
                        else if ((center == npc.NPC.Center + new Vector2(500f, 375f)) && (timer == 240 || timer == 480 || timer == 720 || timer == 960))
                        {
                            SoundEngine.PlaySound(new SoundStyle("TF2/Gensokyo/Content/Sounds/SFX/shoot"), npc.NPC.Center);
                            if (Main.netMode == NetmodeID.MultiplayerClient) return;
                            for (int i = 0; i < 32; i++)
                            {
                                Vector2 velocity = Utils.RotatedBy(Vector2.UnitX, MathHelper.ToRadians(i * 11.25f)) * 5f;
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<PreStarMaelstrom6>(), 30, 0f, npc.NPC.target);
                            }
                            startTimer = true;
                        }
                        if (startTimer && timer % 5 == 0 && burstCounter < 7)
                        {
                            if (Main.netMode == NetmodeID.MultiplayerClient) return;
                            Vector2 velocity = Projectile.DirectionTo(Main.player[npc.NPC.target].Center) * 15f;
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<DevilsRecitation4>(), 40, 0f, npc.NPC.target);
                            burstCounter++;
                            if (burstCounter == 7)
                            {
                                startTimer = false;
                                burstCounter = 0;
                            }
                        }
                    }
                    break;

                case 5:
                    int damage = 50;
                    if (timer == 0 || timer == 300 || timer == 600)
                    {
                        SoundEngine.PlaySound(new SoundStyle("TF2/Gensokyo/Content/Sounds/SFX/shoot"), npc.NPC.Center);
                        if (Main.netMode == NetmodeID.MultiplayerClient) return;
                        for (int i = 0; i < 25; i++)
                        {
                            Vector2 velocity = Utils.RotatedBy(Vector2.UnitX, MathHelper.ToRadians(i * 14.4f)) * 5f;
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<FlyingFantastica1>(), damage, 0f, npc.NPC.target);
                        }
                    }
                    else if (timer == 840)
                    {
                        SoundEngine.PlaySound(new SoundStyle("TF2/Gensokyo/Content/Sounds/SFX/shoot"), npc.NPC.Center);
                        if (Main.netMode == NetmodeID.MultiplayerClient) return;
                        for (int i = 0; i < 25; i++)
                        {
                            Vector2 velocity = Utils.RotatedBy(Vector2.UnitX, MathHelper.ToRadians(i * 14.4f)) * 5f;
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<FlyingFantastica2>(), damage, 0f, npc.NPC.target);
                        }
                    }
                    else if (timer == 1080)
                    {
                        SoundEngine.PlaySound(new SoundStyle("TF2/Gensokyo/Content/Sounds/SFX/shoot"), npc.NPC.Center);
                        if (Main.netMode == NetmodeID.MultiplayerClient) return;
                        for (int i = 0; i < 25; i++)
                        {
                            Vector2 velocity = Utils.RotatedBy(Vector2.UnitX, MathHelper.ToRadians(i * 14.4f)) * 5f;
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<FlyingFantastica3>(), damage, 0f, npc.NPC.target);
                        }
                    }
                    else if (timer == 1320 || timer == 1560 || timer == 1800)
                    {
                        SoundEngine.PlaySound(new SoundStyle("TF2/Gensokyo/Content/Sounds/SFX/shoot"), npc.NPC.Center);
                        if (Main.netMode == NetmodeID.MultiplayerClient) return;
                        for (int i = 0; i < 25; i++)
                        {
                            Vector2 velocity = Utils.RotatedBy(Vector2.UnitX, MathHelper.ToRadians(i * 14.4f)) * 5f;
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<FlyingFantastica4>(), damage, 0f, npc.NPC.target);
                        }
                    }
                    else if (timer == 2040 || timer == 2280 || timer == 2520 || timer == 2760)
                    {
                        SoundEngine.PlaySound(new SoundStyle("TF2/Gensokyo/Content/Sounds/SFX/shoot"), npc.NPC.Center);
                        if (Main.netMode == NetmodeID.MultiplayerClient) return;
                        for (int i = 0; i < 25; i++)
                        {
                            Vector2 velocity = Utils.RotatedBy(Vector2.UnitX, MathHelper.ToRadians(i * 14.4f)) * 5f;
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<FlyingFantastica5>(), damage, 0f, npc.NPC.target);
                        }
                    }
                    break;

                default:
                    break;
            }
            timer++;
            Projectile.netUpdate = true;
        }

        public override bool ShouldUpdatePosition() => false;
    }
}