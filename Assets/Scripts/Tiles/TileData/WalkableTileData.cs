namespace MyTiles
{
    [System.Serializable]
    public class WalkableTileData : BaseTileData
    {
        public bool Walkable;

        public WalkableTileData(TileBaseType tileBaseType) : base(tileBaseType)
        {
        }
    }
}