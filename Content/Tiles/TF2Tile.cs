using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace TF2.Content.Tiles
{
    public static class TF2Tile
    {
        // Tile detection code by Rhoenicx
        public static bool IsDoor(Tile tile) => IsDoor(tile.TileType);

        public static bool IsDoor(int type) => (TileLoader.GetTile(type) != null) ? (TileID.Sets.OpenDoorID[type] > -1 || TileID.Sets.CloseDoorID[type] > -1) : (type == 10 || type == 11);

        public static bool IsSolidTile(Tile tile, bool ignoreSolidTop = false, bool noSolidTop = false)
        {
            if (tile == null || !tile.HasTile) return false;
            if (noSolidTop)
                return tile.HasUnactuatedTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType];
            if (ignoreSolidTop)
                return tile.HasUnactuatedTile && Main.tileSolid[tile.TileType];
            return tile.HasUnactuatedTile  && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType]);
        }

        public static bool IsFullBlock(Tile tile) => !tile.IsHalfBlock && !tile.TopSlope && !tile.BottomSlope && !tile.LeftSlope && !tile.RightSlope;
    }
}
