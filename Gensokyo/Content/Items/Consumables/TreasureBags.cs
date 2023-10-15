using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using TF2.Content.Items;
using TF2.Gensokyo.Common;
using TF2.Gensokyo.Content.Items.Pyro;
using TF2.Gensokyo.Content.Items.Scout;
using TF2.Gensokyo.Content.Items.Sniper;
using TF2.Gensokyo.Content.Items.Soldier;
using TF2.Gensokyo.Content.Items.Spy;
using TF2.Gensokyo.Content.NPCs.Byakuren_Hijiri;

namespace TF2.Gensokyo.Content.Items.Consumables
{
    [ExtendsFromMod("Gensokyo")]
    public class GensokyoDLC_StarterBox : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 41;
            Item.height = 50;
            Item.consumable = true;
            Item.rare = ModContent.RarityType<NormalRarity>();
        }

        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {
            GensokyoDLC.Gensokyo.TryFind("LilyWhiteSpawner", out ModItem lilyWhiteSpawner);
            GensokyoDLC.Gensokyo.TryFind("RumiaSpawner", out ModItem rumiaSpawner);
            GensokyoDLC.Gensokyo.TryFind("EternityLarvaSpawner", out ModItem eternityLarvaSpawner);
            GensokyoDLC.Gensokyo.TryFind("NazrinSpawner", out ModItem nazrinSpawner);
            GensokyoDLC.Gensokyo.TryFind("HinaKagiyamaSpawner", out ModItem hinaKagiyamaSpawner);
            GensokyoDLC.Gensokyo.TryFind("SekibankiSpawner", out ModItem sekibankiSpawner);
            GensokyoDLC.Gensokyo.TryFind("SeiranSpawner", out ModItem seiranSpawner);
            GensokyoDLC.Gensokyo.TryFind("NitoriKawashiroSpawner", out ModItem nitoriKawashiroSpawner);
            GensokyoDLC.Gensokyo.TryFind("MedicineMelancholySpawner", out ModItem medicineMelancholySpawner);
            GensokyoDLC.Gensokyo.TryFind("CirnoSpawner", out ModItem cirnoSpawner);
            GensokyoDLC.Gensokyo.TryFind("MinamitsuMurasaSpawner", out ModItem minamitsuMurasaSpawner);
            GensokyoDLC.Gensokyo.TryFind("AliceMargatroidSpawner", out ModItem aliceMargatroidSpawner);
            GensokyoDLC.Gensokyo.TryFind("SakuyaIzayoiSpawner", out ModItem sakuyaIzayoiSpawner);
            GensokyoDLC.Gensokyo.TryFind("SeijaKijinSpawner", out ModItem seijaKijinSpawner);
            GensokyoDLC.Gensokyo.TryFind("MayumiJoutouguuSpawner", out ModItem mayumiJoutouguuSpawner);
            GensokyoDLC.Gensokyo.TryFind("ToyosatomimiNoMikoSpawner", out ModItem toyosatomimiNoMikoSpawner);
            GensokyoDLC.Gensokyo.TryFind("KaguyaHouraisanSpawner", out ModItem kaguyaHouraisanSpawner);
            GensokyoDLC.Gensokyo.TryFind("UtsuhoReiujiSpawner", out ModItem utsuhoReiujiSpawner);
            GensokyoDLC.Gensokyo.TryFind("TenshiHinanawiSpawner", out ModItem tenshiHinanawiSpawner);

            IEntitySource entitySource = player.GetSource_OpenItem(Type);

            player.QuickSpawnItem(entitySource, lilyWhiteSpawner.Type);
            player.QuickSpawnItem(entitySource, rumiaSpawner.Type);
            player.QuickSpawnItem(entitySource, eternityLarvaSpawner.Type);
            player.QuickSpawnItem(entitySource, nazrinSpawner.Type);
            player.QuickSpawnItem(entitySource, hinaKagiyamaSpawner.Type);
            player.QuickSpawnItem(entitySource, sekibankiSpawner.Type);
            player.QuickSpawnItem(entitySource, seiranSpawner.Type);
            player.QuickSpawnItem(entitySource, nitoriKawashiroSpawner.Type);
            player.QuickSpawnItem(entitySource, medicineMelancholySpawner.Type);
            player.QuickSpawnItem(entitySource, cirnoSpawner.Type);
            player.QuickSpawnItem(entitySource, minamitsuMurasaSpawner.Type);
            player.QuickSpawnItem(entitySource, aliceMargatroidSpawner.Type);
            player.QuickSpawnItem(entitySource, sakuyaIzayoiSpawner.Type);
            player.QuickSpawnItem(entitySource, seijaKijinSpawner.Type);
            player.QuickSpawnItem(entitySource, mayumiJoutouguuSpawner.Type);
            player.QuickSpawnItem(entitySource, toyosatomimiNoMikoSpawner.Type);
            player.QuickSpawnItem(entitySource, kaguyaHouraisanSpawner.Type);
            player.QuickSpawnItem(entitySource, utsuhoReiujiSpawner.Type);
            player.QuickSpawnItem(entitySource, tenshiHinanawiSpawner.Type);
        }
    }

    [ExtendsFromMod("Gensokyo")]
    public class ByakurenBossBag : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.BossBag[Type] = true;
            Item.ResearchUnlockCount = 3;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.consumable = true;
            Item.maxStack = Item.CommonMaxStack;
            if (ModLoader.TryGetMod("CalamityMod", out _))
                Item.rare = 12; // Turquoise
            else
                Item.rare = ItemRarityID.Purple;
            Item.expert = true;
        }

        public override bool CanRightClick() => true;

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            itemLoot.Add(ItemDropRule.OneFromOptions(1, new int[]
            {
                ModContent.ItemType<AdvancedScoutRifle>(),
                ModContent.ItemType<HeadhunterPistols>(),
                ModContent.ItemType<ManualInferno>(),
                ModContent.ItemType<HarshPunisher>(),
                ModContent.ItemType<OffensiveRocketSystem>()
            }));
            if (GensokyoDLC.gensokyoLoaded)
            {
                GensokyoDLC.Gensokyo.TryFind("PointItem", out ModItem pointItem);
                itemLoot.Add(ItemDropRule.Common(pointItem.Type, 1, 150, 200));
                GensokyoDLC.Gensokyo.TryFind("PowerItem", out ModItem powerItem);
                itemLoot.Add(ItemDropRule.Common(powerItem.Type, 1, 100, 120));
            }
            itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<ByakurenHijiri>()));
        }

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.4f);

        public override void PostUpdate()
        {
            // Spawn some light and dust when dropped in the world
            Lighting.AddLight(Item.Center, Color.White.ToVector3() * 0.4f);

            if (Item.timeSinceItemSpawned % 12 == 0)
            {
                Vector2 center = Item.Center + new Vector2(0f, Item.height * -0.1f);

                // This creates a randomly rotated vector of length 1, which gets it's components multiplied by the parameters
                Vector2 direction = Main.rand.NextVector2CircularEdge(Item.width * 0.6f, Item.height * 0.6f);
                float distance = 0.3f + Main.rand.NextFloat() * 0.5f;
                Vector2 velocity = new Vector2(0f, -Main.rand.NextFloat() * 0.3f - 1.5f);

                Dust dust = Dust.NewDustPerfect(center + direction * distance, DustID.GoldFlame, velocity);
                dust.scale = 0.5f;
                dust.fadeIn = 1.1f;
                dust.noGravity = true;
                dust.noLight = true;
                dust.alpha = 0;
            }
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            // Draw the periodic glow effect behind the item when dropped in the world (hence PreDrawInWorld)
            Texture2D texture = TextureAssets.Item[Item.type].Value;

            Rectangle frame;

            if (Main.itemAnimations[Item.type] != null)
            {
                // In case this item is animated, this picks the correct frame
                frame = Main.itemAnimations[Item.type].GetFrame(texture, Main.itemFrameCounter[whoAmI]);
            }
            else
                frame = texture.Frame();

            Vector2 frameOrigin = frame.Size() / 2f;
            Vector2 offset = new Vector2(Item.width / 2 - frameOrigin.X, Item.height - frame.Height);
            Vector2 drawPos = Item.position - Main.screenPosition + frameOrigin + offset;

            float time = Main.GlobalTimeWrappedHourly;
            float timer = Item.timeSinceItemSpawned / 240f + time * 0.04f;

            time %= 4f;
            time /= 2f;

            if (time >= 1f)
            {
                time = 2f - time;
            }

            time = time * 0.5f + 0.5f;

            for (float i = 0f; i < 1f; i += 0.25f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;
                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, new Color(255, 194, 0, 50), rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;
                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, new Color(255, 226, 0, 75), rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }

            return true;
        }
    }
}