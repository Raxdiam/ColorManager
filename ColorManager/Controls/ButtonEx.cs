using System;
using System.Drawing;
using System.Windows.Forms;

namespace ColorManager.Controls
{
    public class ButtonEx : Button
    {
        private Graphics _g;
        private Rectangle _cr;

        private Color _borderColor = Color.FromArgb(180, 180, 180);
        private Color _borderColorHover = Color.FromArgb(170, 170, 170);
        private Color _borderColorPressed = Color.FromArgb(160, 160, 160);
        private Color _borderColorDisabled = Color.FromArgb(140, 140, 140);

        private Color _backColorHover = Color.FromArgb(250, 250, 250);
        private Color _backColorPressed = Color.FromArgb(240, 240, 240);
        private Color _backColorDisabled = Color.FromArgb(225, 225, 225);

        private Color _foreColorDisabled = Color.FromArgb(100, 100, 100);

        private Padding _textPadding = new(0, 0, 0, 1);

        private ButtonState _state = ButtonState.Normal;

        public ButtonEx()
        {
            Size = new Size(75, 22);
        }

        public Color BackColorHover
        {
            get => _backColorHover;
            set
            {
                _backColorHover = value;
                Invalidate();
            }
        }

        public Color BackColorPressed
        {
            get => _backColorPressed;
            set
            {
                _backColorPressed = value;
                Invalidate();
            }
        }

        public Color BackColorDisabled
        {
            get => _backColorDisabled;
            set
            {
                _backColorDisabled = value;
                Invalidate();
            }
        }

        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                Invalidate();
            }
        }

        public Color BorderColorHover
        {
            get => _borderColorHover;
            set
            {
                _borderColorHover = value;
                Invalidate();
            }
        }

        public Color BorderColorPressed
        {
            get => _borderColorPressed;
            set
            {
                _borderColorPressed = value;
                Invalidate();
            }
        }

        public Color BorderColorDisabled
        {
            get => _borderColorDisabled;
            set
            {
                _borderColorDisabled = value;
                Invalidate();
            }
        }

        public Color ForeColorDisabled
        {
            get => _foreColorDisabled;
            set
            {
                _foreColorDisabled = value;
                Invalidate();
            }
        }

        public Padding TextPadding
        {
            get => _textPadding;
            set
            {
                _textPadding = value;
                Invalidate();
            }
        }

        public new bool Enabled
        {
            get => base.Enabled;
            set
            {
                base.Enabled = value;
                _state = value ? ButtonState.Normal : ButtonState.Disabled;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            _g = e.Graphics;
            _cr = ClientRectangle;

            var (borderBrush, borderPen) = _state switch
            {
                ButtonState.Normal => (new SolidBrush(BackColor), new Pen(BorderColor)),
                ButtonState.Hover => (new SolidBrush(BackColorHover), new Pen(BorderColorHover)),
                ButtonState.Pressed => (new SolidBrush(BackColorPressed), new Pen(BorderColorPressed)),
                ButtonState.Disabled => (new SolidBrush(BackColorDisabled), new Pen(BorderColorDisabled)),
                _ => (new SolidBrush(BackColor), new Pen(BorderColor))
            };

            _g.FillRectangle(borderBrush, ClientRectangle);
            _g.DrawRectangle(borderPen, _cr.X, _cr.Y, _cr.Width - 1, _cr.Height - 1);
            borderBrush.Dispose();
            borderPen.Dispose();
            
            _g.DrawText(Text, Font,
                _cr.X - _textPadding.Left,
                _cr.Y - _textPadding.Top,
                _cr.Width - _textPadding.Right,
                _cr.Height - _textPadding.Bottom,
                Enabled ? ForeColor : _foreColorDisabled, TextAlign);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            SetButtonState(ButtonState.Hover);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            SetButtonState(ButtonState.Normal);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            SetButtonState(ButtonState.Pressed);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            SetButtonState(ClientRectangle.Contains(e.Location) ?
                ButtonState.Hover :
                ButtonState.Normal);
        }

        private void SetButtonState(ButtonState state)
        {
            if (!Enabled) return;

            _state = state;
            Invalidate();
        }

        private enum ButtonState
        {
            Normal,
            Hover,
            Pressed,
            Disabled
        }
    }
}
