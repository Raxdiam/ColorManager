using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ColorManager
{
    public static class Extensions
    {
        public static string ToHex(this Color color)
        {
            return color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }

        public static Image FromBase64(this string value)
        {
            using var ms = new MemoryStream(Convert.FromBase64String(value));
            return Image.FromStream(ms);
        }

        public static Bitmap CropFromCenter(this Image image, int width, int height)
        {
            var x = image.Width / 2 - width / 2;
            var y = image.Height / 2 - height / 2;

            var cropArea = new Rectangle(x, y, width, height);

            var targetImage = ((Bitmap) image).Clone(cropArea, image.PixelFormat);

            return targetImage;
        }

        public static string ToRgbString(this Color color) => $"{color.R}, {color.G}, {color.B}";

        public static string ToSafeFileName(this string value)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            return new string(value.Where(c => !invalidChars.Contains(c)).ToArray());
        }
    }

    public static class ListViewExtensions
    {
        public static ColorItem FindColorItem(this ListView listView, string hex) =>
            listView.Items.Cast<ColorItem>().FirstOrDefault(i => i.Color.ToHex() == hex);

        public static void AddColorItem(this ListView listView, ColorItem item)
        {
            if (!listView.SmallImageList.Images.ContainsKey(item.Hex)) {
                var img = new Bitmap(16, 16, PixelFormat.Format32bppRgb);
                using var g = Graphics.FromImage(img);
                using var brush = new SolidBrush(item.Color);
                g.FillRectangle(brush, 0, 0, 16, 16);

                listView.SmallImageList.Images.Add(item.Hex, img);
            }

            listView.Items.Add(item);
        }

        public static void ClearColorItems(this ListView listView)
        {
            listView.Items.Clear();
            listView.SmallImageList.Images.Clear();
        }
    }

    public static class GraphicsExtensions
    {
        private static readonly Dictionary<ContentAlignment, TextFormatFlags> AlignmentFlagMap = new() {
            [ContentAlignment.BottomCenter] = TextFormatFlags.HorizontalCenter | TextFormatFlags.Bottom,
            [ContentAlignment.BottomLeft] = TextFormatFlags.Bottom | TextFormatFlags.Left,
            [ContentAlignment.BottomRight] = TextFormatFlags.Bottom | TextFormatFlags.Right,
            [ContentAlignment.MiddleCenter] = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            [ContentAlignment.MiddleLeft] = TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
            [ContentAlignment.MiddleRight] = TextFormatFlags.VerticalCenter | TextFormatFlags.Right,
            [ContentAlignment.TopCenter] = TextFormatFlags.HorizontalCenter | TextFormatFlags.Top,
            [ContentAlignment.TopLeft] = TextFormatFlags.Top | TextFormatFlags.Left,
            [ContentAlignment.TopRight] = TextFormatFlags.Top | TextFormatFlags.Right
        };

        public static void DrawText(this Graphics g, string text, Font font, int x, int y, Color foreColor) =>
            DrawText(g, text, font, new Point(x, y), foreColor);

        public static void DrawText(this Graphics g, string text, Font font, int x, int y, int width, int height, Color foreColor, ContentAlignment alignment) =>
            DrawText(g, text, font, new Rectangle(x, y, width, height), foreColor, alignment);

        public static void DrawText(this Graphics g, string text, Font font, Point location, Color foreColor) =>
            TextRenderer.DrawText(g, text, font, location, foreColor);

        public static void DrawText(this Graphics g, string text, Font font, Point location, Color foreColor, Color backColor) =>
            TextRenderer.DrawText(g, text, font, location, foreColor, backColor);

        public static void DrawText(this Graphics g, string text, Font font, Point location, Color foreColor, ContentAlignment alignment) =>
            TextRenderer.DrawText(g, text, font, location, foreColor, AlignmentFlagMap[alignment]);

        public static void DrawText(this Graphics g, string text, Font font, Point location, Color foreColor, Color backColor, ContentAlignment alignment) =>
            TextRenderer.DrawText(g, text, font, location, foreColor, backColor, AlignmentFlagMap[alignment]);

        public static void DrawText(this Graphics g, string text, Font font, Rectangle bounds, Color foreColor) =>
            TextRenderer.DrawText(g, text, font, bounds, foreColor);

        public static void DrawText(this Graphics g, string text, Font font, Rectangle bounds, Color foreColor, Color backColor) =>
            TextRenderer.DrawText(g, text, font, bounds, foreColor, backColor);

        public static void DrawText(this Graphics g, string text, Font font, Rectangle bounds, Color foreColor, ContentAlignment alignment) =>
            TextRenderer.DrawText(g, text, font, bounds, foreColor, AlignmentFlagMap[alignment]);

        public static void DrawText(this Graphics g, string text, Font font, Rectangle bounds, Color foreColor, ContentAlignment alignment, TextFormatFlags flags) =>
            TextRenderer.DrawText(g, text, font, bounds, foreColor, AlignmentFlagMap[alignment] | flags);

        public static void DrawText(this Graphics g, string text, Font font, Rectangle bounds, Color foreColor, Color backColor, ContentAlignment alignment) =>
            TextRenderer.DrawText(g, text, font, bounds, foreColor, backColor, AlignmentFlagMap[alignment]);

        public static void DrawText(this Graphics g, string text, Font font, Rectangle bounds, Color foreColor, Color backColor, ContentAlignment alignment,
            TextFormatFlags flags) => TextRenderer.DrawText(g, text, font, bounds, foreColor, backColor, AlignmentFlagMap[alignment] | flags);

        public static Size MeasureText(this Graphics g, string text, Font font) => TextRenderer.MeasureText(g, text, font);

        public static Size MeasureText(this Graphics g, string text, Font font, Size bounds) => TextRenderer.MeasureText(g, text, font, bounds);

        public static Size MeasureText(this Graphics g, string text, Font font, Size bounds, ContentAlignment alignment) =>
            TextRenderer.MeasureText(g, text, font, bounds, AlignmentFlagMap[alignment]);
    }
}
