using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Newtonsoft.Json;

namespace ColorManager.Json.Converters
{
    public class ImageConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Bitmap) || objectType == typeof(Image);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var m = new MemoryStream(Convert.FromBase64String((string) reader.Value ?? string.Empty));
            return (Bitmap) Image.FromStream(m);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var bmp = (Bitmap) value;
            using var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Bmp);

            writer.WriteValue(Convert.ToBase64String(ms.ToArray()));
        }
    }
}