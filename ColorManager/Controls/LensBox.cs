using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ColorManager.Controls
{
    public class LensBox : PictureBox
    {
        public event Action<Color> FocusChanged;

        private static readonly Pen FocusPen = Pens.Magenta;

        private readonly Timer _timer;

        private int _lensDim;
        private int _lensZoomLevel = 14;
        private int _lensZoomDim;
        private Color _focusColor;
        private bool _disposeCurrentImage;

        public LensBox()
        {
            SizeMode = PictureBoxSizeMode.CenterImage;

            _timer = new Timer {Interval = 10};
            _timer.Tick += UpdateLens;

            _lensDim = 50;
            _lensZoomDim = _lensDim / _lensZoomLevel;
        }

        public int LensSize {
            get => _lensDim;
            set {
                _lensDim = value;
                _lensZoomDim = value / _lensZoomLevel;
            }
        }

        public int LensZoom {
            get => _lensZoomLevel;
            set {
                _lensZoomLevel = value;
                LensSize = _lensDim;
            }
        }

        public void Start() => _timer.Start();

        public void Stop() => _timer.Stop();

        public void SetImage(Image image, bool disposeLater)
        {
            Image = image;
            _disposeCurrentImage = disposeLater;
        }
        
        private void UpdateLens(object sender, EventArgs e)
        {
            if (_disposeCurrentImage) {
                Image?.Dispose();
                _disposeCurrentImage = false;
            }

            var mX = Cursor.Position.X;
            var mY = Cursor.Position.Y;
            
            using var bmp = new Bitmap(_lensZoomDim, _lensZoomDim, PixelFormat.Format32bppRgb);
            using var g = Graphics.FromImage(bmp);
            g.CopyFromScreen(
                mX - _lensZoomDim / 2, 
                mY - _lensZoomDim / 2, 
                0, 0, 
                new Size(_lensZoomDim, _lensZoomDim));
            g.Save();
            SetImage(GetZoom(bmp), true);

            _focusColor = bmp.GetPixel(_lensZoomDim / 2, _lensZoomDim / 2);
            FocusChanged?.Invoke(_focusColor);
        }

        private Bitmap GetZoom(Image original)
        {
            var res = new Bitmap(_lensDim, _lensDim);
            using var g = Graphics.FromImage(res);
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.DrawImage(original, 0, 0, _lensDim, _lensDim);
            g.DrawRectangle(FocusPen, res.Width / 2 - _lensZoomLevel - 1, res.Height / 2 - _lensZoomLevel - 1, _lensZoomLevel + 1, _lensZoomLevel + 1);
            return res;
        }
    }
}
