using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace TF2.Content.UI
{
    public class UITextures : ModSystem
    {
        internal static Asset<Texture2D>[] MainMenuBackgroundTextures { get; private set; }

        internal static Asset<Texture2D>[] MainMenuMercenaryTextures { get; private set; }

        internal static Asset<Texture2D>[] ClassIconTextures { get; private set; }

        internal static Asset<Texture2D>[] MannCoStoreBackgroundTextures { get; private set; }

        internal static Asset<Texture2D>[] MannCoStoreShopTextures { get; private set; }

        internal static Asset<Texture2D>[] MannCoStoreNavigationTextures { get; private set; }

        internal static Asset<Texture2D>[] MercenaryCreationMenuTextures { get; private set; }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                MainMenuBackgroundTextures =
                [
                    ModContent.Request<Texture2D>("TF2/Content/Textures/Background"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/BackgroundScreamFortress"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/BackgroundSmissmas")
                ];
                MainMenuMercenaryTextures =
                [
                    ModContent.Request<Texture2D>("TF2/Content/Textures/Main_Menu_Scout"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/Main_Menu_Soldier"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/Main_Menu_Pyro"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/Main_Menu_Demoman"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/Main_Menu_Heavy"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/Main_Menu_Engineer"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/Main_Menu_Medic"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/Main_Menu_Sniper"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/Main_Menu_Spy")
                ];
                ClassIconTextures =
                [
                    Main.Assets.Request<Texture2D>("Images/UI/PlayerBackground"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/ScoutIcon"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/SoldierIcon"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/PyroIcon"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/DemomanIcon"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/HeavyIcon"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/EngineerIcon"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/MedicIcon"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/SniperIcon"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/SpyIcon")
                ];
                MannCoStoreBackgroundTextures =
                [
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/MannCoStore/ItemBackground"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/MannCoStore/ItemBackgroundHovered")
                ];
                MannCoStoreShopTextures =
                [
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/MannCoStore/ShoppingCart"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/MannCoStore/ShoppingCartHovered"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/MannCoStore/ShoppingListBackground")
                ];
                MannCoStoreNavigationTextures =
                [
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/MannCoStore/First"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/MannCoStore/FirstHovered"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/MannCoStore/Previous"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/MannCoStore/PreviousHovered"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/MannCoStore/Next"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/MannCoStore/NextHovered"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/MannCoStore/Last"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/MannCoStore/LastHovered"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/MannCoStore/Scrollbar"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/MannCoStore/ScrollbarInner"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/MannCoStore/ScrollbarInnerHovered"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/MannCoStore/ClassFilters")
                ];
                MercenaryCreationMenuTextures =
                [
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/MercenaryCreationMenu/Scout"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/MercenaryCreationMenu/Soldier"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/MercenaryCreationMenu/Pyro"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/MercenaryCreationMenu/Demoman"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/MercenaryCreationMenu/Heavy"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/MercenaryCreationMenu/Engineer"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/MercenaryCreationMenu/Medic"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/MercenaryCreationMenu/Sniper"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/MercenaryCreationMenu/Spy"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/MercenaryCreationMenu/RandomClass"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/MercenaryCreationMenu/IconBorder"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/MercenaryCreationMenu/Mercenary"),
                    ModContent.Request<Texture2D>("TF2/Content/Textures/UI/MercenaryCreationMenu/Classless")
                ];
            }
        }
    }
}