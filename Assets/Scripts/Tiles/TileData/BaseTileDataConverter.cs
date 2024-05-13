using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MyTiles
{
    public class BaseTileDataConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value, value.GetType());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            if (jo["$type"].Value<string>().Contains(nameof(BoxData)))
            {
                return jo.ToObject<BoxData>(serializer);
            }
            if (jo["$type"].Value<string>().Contains(nameof(WalkableTileData)))
            {
                return jo.ToObject<WalkableTileData>(serializer);
            }
            else
            {
                return jo.ToObject<NormalData>(serializer);
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(BaseTileData));
        }
    }
}