using System.Collections.Generic;
using System.Linq;
using MyTiles;
using Newtonsoft.Json;

namespace MyMapManager
{
    public static class MapIO
    {
        public static void SaveMapData(string path, CurrentState currentState)
        {
            var mapData = new MapData();
            mapData.TilemapInfoList = new List<TilemapInfo>();
            for (int i = 0; i < currentState.TilemapList.Count; i++)
            {
                var tilemapInfo = new TilemapInfo();
                tilemapInfo.TilemapIndex = i;
                tilemapInfo.TileCellPositions = currentState.TileDictList[i].Keys.ToList();
                tilemapInfo.TileDataset = currentState.TileDictList[i].Values
                    .Select(tileInfo => tileInfo.TileData).ToList();
                mapData.TilemapInfoList.Add(tilemapInfo);
            }

            var settings = GetSerializerSettings();
            var json = JsonConvert.SerializeObject(mapData, Formatting.Indented, settings);

            //把json字符串写入文件
            System.IO.File.WriteAllText(path, json);
        }

        private static JsonSerializerSettings GetSerializerSettings()
        {
            var settings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new BaseTileDataConverter() },
                TypeNameHandling = TypeNameHandling.Auto,
                NullValueHandling = NullValueHandling.Include,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            };
            return settings;
        }


        public static MapData LoadMapData(string path)
        {
            //读取文件中的json字符串
            var settings = GetSerializerSettings();
            System.IO.File.ReadAllText(path);
            //把json字符串转换成MapData
            string json = System.IO.File.ReadAllText(path);
            var mapData = JsonConvert.DeserializeObject<MapData>(json, settings);
            return mapData;
        }
    }
}