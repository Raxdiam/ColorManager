using System;
using System.Drawing;
using System.Windows.Forms;
using Newtonsoft.Json;
using ImageConverter = ColorManager.Json.Converters.ImageConverter;

namespace ColorManager
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ColorItem : ListViewItem, IDisposable
    {
        public ColorItem(Color color, string description, Image thumbnail)
        {
            Color = color;
            Description = description;
            var hex = color.ToHex();
            Hex = hex;
            Text = hex.StartsWith("#") ? hex : $"#{hex}";
            ImageKey = hex;
            UseItemStyleForSubItems = false;
            SubItems.Add(description);
            Thumbnail = thumbnail;
        }

        [JsonProperty]
        public Color Color { get; }
        
        public string Hex { get; }
        
        [JsonProperty]
        public string Description { get; private set; }

        [JsonProperty]
        [JsonConverter(typeof(ImageConverter))]
        public Image Thumbnail { get; }

        public void SetDescription(string value)
        {
            SubItems[1].Text = value;
            Description = value;
        }

        public void Dispose()
        {
            Thumbnail?.Dispose();
        }
    }
}
