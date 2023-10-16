using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.Localization;
using Terraria.ModLoader;
using TF2.Gensokyo.Content.Items.BossSummons;
using TF2.Gensokyo.Content.NPCs.Byakuren_Hijiri;

namespace TF2.Gensokyo.Common
{
    [ExtendsFromMod("Gensokyo")]
    public class GensokyoBossChecklist : ModSystem
    {
        public override void PostSetupContent()
        {
            // Boss Checklist shows comprehensive information about bosses in its own UI. We can customize it:
            // https://forums.terraria.org/index.php?threads/.50668/
            DoBossChecklistIntegration();
        }

        private void DoBossChecklistIntegration()
        {
            // The mods homepage links to its own wiki where the calls are explained: https://github.com/JavidPack/BossChecklist/wiki/Support-using-Mod-Call
            // If we navigate the wiki, we can find the "AddBoss" method, which we want in this case
            // For some messages, mods might not have them at release, so we need to verify when the last iteration of the method variation was first added to the mod, in this case 1.3.1
            // Usually mods either provide that information themselves in some way, or it's found on the github through commit history/blame
            if (!ModLoader.TryGetMod("BossChecklist", out Mod bossChecklistMod) || bossChecklistMod.Version < new Version(1, 3, 1)) return;
            BossRush(bossChecklistMod);
            ByakurenHijiri(bossChecklistMod);
        }

        private void BossRush(Mod mod)
        {
            bool modActive = GensokyoDLC.gensokyoLoaded;
            if (!modActive) return;
            GensokyoDLC.Gensokyo.TryFind("LilyWhite", out ModNPC lily);
            GensokyoDLC.Gensokyo.TryFind("Rumia", out ModNPC rumia);
            GensokyoDLC.Gensokyo.TryFind("EternityLarva", out ModNPC larva);
            GensokyoDLC.Gensokyo.TryFind("Nazrin", out ModNPC nazrin);
            GensokyoDLC.Gensokyo.TryFind("HinaKagiyama", out ModNPC hina);
            GensokyoDLC.Gensokyo.TryFind("Sekibanki", out ModNPC sekibanki);
            GensokyoDLC.Gensokyo.TryFind("Seiran", out ModNPC seiran);
            GensokyoDLC.Gensokyo.TryFind("NitoriKawashiro", out ModNPC nitori);
            GensokyoDLC.Gensokyo.TryFind("MedicineMelancholy", out ModNPC medicine);
            GensokyoDLC.Gensokyo.TryFind("Cirno", out ModNPC cirno);
            GensokyoDLC.Gensokyo.TryFind("MinamitsuMurasa", out ModNPC minamitsu);
            GensokyoDLC.Gensokyo.TryFind("AliceMargatroid", out ModNPC alice);
            GensokyoDLC.Gensokyo.TryFind("SakuyaIzayoi", out ModNPC sakuya);
            GensokyoDLC.Gensokyo.TryFind("SeijaKijin", out ModNPC seija);
            GensokyoDLC.Gensokyo.TryFind("MayumiJoutouguu", out ModNPC mayumi);
            GensokyoDLC.Gensokyo.TryFind("ToyosatomimiNoMiko", out ModNPC miko);
            GensokyoDLC.Gensokyo.TryFind("KaguyaHouraisan", out ModNPC kaguya);
            GensokyoDLC.Gensokyo.TryFind("UtsuhoReiuji", out ModNPC okuu);
            GensokyoDLC.Gensokyo.TryFind("TenshiHinanawi", out ModNPC tenshi);

            List<int> bossList = new()
            {
                lily.Type,
                rumia.Type,
                larva.Type,
                nazrin.Type,
                hina.Type,
                sekibanki.Type,
                seiran.Type,
                nitori.Type,
                medicine.Type,
                cirno.Type,
                minamitsu.Type,
                alice.Type,
                sakuya.Type,
                seija.Type,
                mayumi.Type,
                miko.Type,
                kaguya.Type,
                okuu.Type,
                tenshi.Type
            };

            mod.Call(
                "LogEvent",
                Mod,
                "BossRush",
                18.99f,
                new Func<bool>(() => DownedGensokyoBoss.downedBossRush),
                bossList,
                new Dictionary<string, object>()
                {
                    ["displayName"] = Language.GetTextValue("Mods.TF2.BossChecklist.BossRush.DisplayName"),
                    ["spawnInfo"] = Language.GetTextValue("Mods.TF2.BossChecklist.BossRush.SpawnInfo"),
                    ["spawnItems"] = ModContent.ItemType<BossRushSummon>(),
                    ["availability"] = new Func<bool>(() => modActive), // The boss will only show up if the Gensokyo Mod is enabled
                    ["despawnMessage"] = Language.GetTextValue("Mods.TF2.BossChecklist.ByakurenHijiri.DespawnMessage"),
                    ["overrideHeadTextures"] = "TF2/Gensokyo/Content/Events/BossRushIcon",
                    ["customPortrait"] = (SpriteBatch sb, Rectangle rect, Color color) =>
                    {
                        Texture2D texture = ModContent.Request<Texture2D>("TF2/Gensokyo/Content/Textures/BossRushPortrait").Value;
                        Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
                        sb.Draw(texture, centered, color);
                    }
                }                                           
            );
        }

        private void ByakurenHijiri(Mod mod)
        {
            mod.Call(
                "LogBoss",
                Mod,
                nameof(ByakurenHijiri),
                19f, // Same weight as Providence
                new Func<bool>(() => DownedGensokyoBoss.downedByakurenHijiri),
                ModContent.NPCType<ByakurenHijiri>(),
                new Dictionary<string, object>()
                {
                    ["spawnInfo"] = Language.GetTextValue("Mods.TF2.BossChecklist.ByakurenHijiri.SpawnInfo"),
                    ["spawnItems"] = ModContent.ItemType<BossRushSummon>(),
                    ["availability"] = new Func<bool>(() => GensokyoDLC.gensokyoLoaded), // The boss will only show up if the Gensokyo Mod is enabled
                    ["despawnMessage"] = Language.GetTextValue("Mods.TF2.BossChecklist.ByakurenHijiri.DespawnMessage")
                }
            );
        }
    }
}