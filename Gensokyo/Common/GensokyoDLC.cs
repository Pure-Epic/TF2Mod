using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using TF2.Gensokyo.Content.UI;

namespace TF2.Gensokyo.Common
{
    [ExtendsFromMod("Gensokyo")]
    public class GensokyoDLC : ModSystem
    {
        public static Mod Gensokyo;
        public readonly static bool gensokyoLoaded = ModLoader.TryGetMod("Gensokyo", out Gensokyo);
        private MethodInfo RumiaLimits = null;
        private MethodInfo OkuuLimits = null;
        public static bool bossRush;

        public override void Load()
        {
            // Gensokyo DLC code by Rhoenicx
            // "My brain hurts" - Pure_Epic

            if (Gensokyo != null)
            {
                Type gensokyoRumia = null;
                Type gensokyoUtsuhoReiuji = null;

                Assembly gensokyoAssembly = Gensokyo.GetType().Assembly;
                foreach (Type t in gensokyoAssembly.GetTypes())
                {
                    if (t.Name == "Rumia")
                        gensokyoRumia = t;
                    if (t.Name == "UtsuhoReiuji")
                        gensokyoUtsuhoReiuji = t;
                }

                if (gensokyoRumia != null)
                    RumiaLimits = gensokyoRumia.GetMethod("AI", BindingFlags.Instance | BindingFlags.Public);

                // The original method is private, therefore mark it with BindingFlags.NonPublic 
                if (gensokyoUtsuhoReiuji != null)
                    OkuuLimits = gensokyoUtsuhoReiuji.GetMethod("TargetIsValid_BossSpecific", BindingFlags.Instance | BindingFlags.NonPublic);

                if (RumiaLimits != null)
                    ModifyRumiaLimits += ILWeakReferences_ModifyRumiaLimits;
                if (OkuuLimits != null)
                    ModifyOkuuLimits += ILWeakReferences_ModifyOkuuLimits;

                Gensokyo.Call("RegisterShopAccess", Mod.Name);
            }
        }

        public override void Unload()
        {
            Gensokyo = null;

            if (RumiaLimits != null)
                ModifyRumiaLimits -= ILWeakReferences_ModifyRumiaLimits;
            if (OkuuLimits != null)
                ModifyOkuuLimits -= ILWeakReferences_ModifyOkuuLimits;
        }       

        private event ILContext.Manipulator ModifyRumiaLimits
        {
            add { HookEndpointManager.Modify(RumiaLimits, value); }
            remove { HookEndpointManager.Unmodify(RumiaLimits, value); }
        }

        private event ILContext.Manipulator ModifyOkuuLimits
        {
            add { HookEndpointManager.Modify(OkuuLimits, value); }
            remove { HookEndpointManager.Unmodify(OkuuLimits, value); }
        }

        public bool doPatch = true;

        // Since we are modifing a void, we cannot simply return a value
        private void ILWeakReferences_ModifyRumiaLimits(ILContext il)
        {
            if (doPatch)
            {
                var c = new ILCursor(il);
                ILLabel label = null;

                if (c.TryGotoNext(
                    x => x.MatchLdarg(0), // IL_0000: ldarg.0
                                          // The MatchCall being empty is not really a big deal since this piece of code is short anyway.
                    x => x.MatchCall(out _), // IL_0012: call instance int32 Gensokyo.NPCs.Boss::get_State()
                    x => x.MatchLdcI4(99), // IL_0017: ldc.i4.s 99
                    x => x.MatchBeq(out label) // IL_0019: beq.s IL_0069
                    ))

                {
                    // The reason why we increase the index by 4 is because by default, the cursor will be placed on the first line encountered by the TryGotoNext.
                    // In our case, we want to move past these lines to get inside the if statement.
                    int Index = c.Index + 4;

                    // For safety, we should actually also check the lines where we want to branch over, since we cannot guarantee this code stays the same in the future.
                    if (c.TryGotoNext(
                    x => x.MatchLdsfld<Main>("dayTime"), // IL_001B: ldsfld bool [tModLoader]Terraria.Main::dayTime
                    x => x.MatchBrfalse(out label), // IL_0020: brfalse.s IL_002C
                    x => x.MatchLdsfld<Main>("eclipse"), // IL_0022: ldsfld bool [tModLoader]Terraria.Main::eclipse
                    x => x.MatchLdcI4(0), // IL_0027: ldc.i4.0
                    x => x.MatchCeq(), // IL_0028: ceq
                    x => x.MatchBr(out _), // IL_002A: br.s IL_002D
                    x => x.MatchLdcI4(0), // IL_002C: ldc.i4.0
                    x => x.MatchBrfalse(out _) // IL_002D: brfalse IL_00B9
                    ))
                    {
                        c.Index = Index;
                        c.Emit(OpCodes.Br, label);
                    }
                }
            }
        }

        // This boolean returns true
        private void ILWeakReferences_ModifyOkuuLimits(ILContext il)
        {
            if (doPatch)
            {
                var c = new ILCursor(il);

                c.Emit(OpCodes.Ldc_I4_1);
                c.Emit(OpCodes.Ret);
            }
        }
    }

    [ExtendsFromMod("Gensokyo")]
    public class GensokyoDLC_UI : ModSystem
    {
        private UserInterface _harshPunisherChargeInterface;
        internal HarshPunisherChargeUI HarshPunisherChargeUI;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                HarshPunisherChargeUI = new HarshPunisherChargeUI();
                _harshPunisherChargeInterface = new UserInterface();
                _harshPunisherChargeInterface.SetState(HarshPunisherChargeUI);
            }
        }

        public override void UpdateUI(GameTime gameTime) => _harshPunisherChargeInterface?.Update(gameTime);

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int buffBannerIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
            if (buffBannerIndex != -1)
            {
                layers.Insert(buffBannerIndex, new LegacyGameInterfaceLayer(
                    "TF2: Harsh Punisher Charge",
                    delegate
                    {
                        _harshPunisherChargeInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }

    [ExtendsFromMod("Gensokyo")]
    public class GensokyoDLC_Item : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public bool gensokyoExclusiveItem;

        public override void UpdateInventory(Item item, Player player)
        {
            if (gensokyoExclusiveItem && !GensokyoDLC.gensokyoLoaded)
                item.type = ItemID.None;
        }
    }
}