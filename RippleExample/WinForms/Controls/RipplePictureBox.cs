using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using libWyvernzora.WinForms.Effects;

namespace RippleExample.WinForms.Controls
{
    public class RipplePictureBox : PictureBox
    {
        #region Win32 API
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Local

        private const int WM_PAINT = 0x000F;
        private const int WM_ERASEBKGND = 0x0014;
        private const int WM_PRINTCLIENT = 0x0318;

        // Win32 Structures
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct PAINTSTRUCT
        {
            public IntPtr hdc;
            public int fErase;
            public RECT rcPaint;
            public int fRestore;
            public int fIncUpdate;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] rgbReserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        // Win32 Functions
        [DllImport("user32.dll")]
        private static extern IntPtr BeginPaint(IntPtr hWnd,
                                        ref PAINTSTRUCT paintStruct);

        [DllImport("user32.dll")]
        private static extern bool EndPaint(IntPtr hWnd, ref PAINTSTRUCT paintStruct);

// ReSharper restore MemberCanBePrivate.Local
// ReSharper restore InconsistentNaming
        #endregion

        public const Int32 FPS = 60;
        public const Int32 SIZE = 600;

        private RippleEffect effect;
        private Timer timer;

        private Boolean dragging;

        public RipplePictureBox()
        {
            effect = new RippleEffect();
            dragging = false;

            timer = new Timer();
            timer.Interval = (int) (1000 / FPS);
            timer.Tick += (@s, a) => UpdateFrame();

            MouseDown += (@s, a) =>
                {
                    Splash(a.Location.X, a.Location.Y, ClickSplashRadius);
                    dragging = true;
                };
            MouseUp += (@s, a) => { dragging = false; };
            MouseMove += (@s, a) =>
                {
                    if (dragging)
                    {
                        Splash(a.Location.X, a.Location.Y, DragSplashRadius);
                    }
                };

            ClickSplashRadius = 10;
            DragSplashRadius = 5;

            timer.Start();
            timer.Enabled = true;
        }

        public Boolean AnimationEnabled
        {
            get { return timer.Enabled; }
            set
            {
                timer.Enabled = value;
                if (!value) effect.Clear();
            }
        }

        public RippleEffect Effect
        { get { return effect; } }

        /// <summary>
        /// Splash radius when clicked
        /// </summary>
        public Int32 ClickSplashRadius 
        { get; set; }

        /// <summary>
        /// Splash radius when dragging (drag trail)
        /// </summary>
        public Int32 DragSplashRadius 
        { get; set; }

        public new Image Image
        {
            get { return effect.Texture; }
            set { effect.Texture = new Bitmap(value); }
        }


        public void Splash(Int32 x, Int32 y, Int32 radius)
        {
            Point lc = new Point(x, y);
            lc = Translate(lc);
            effect.Splash(lc.X, lc.Y, radius);
        }

        public void Clear()
        {
            effect.Clear();
        }

        // Wrapper so that update and rendering comes together
        private void UpdateFrame()
        {
            if (!DesignMode) // disable animation in designer
            {
                effect.Update();
                Invalidate();
            }
        }

        // Actual rendering is here
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_ERASEBKGND:
                    return;
                case WM_PAINT:
                    PAINTSTRUCT paintStruct = new PAINTSTRUCT();
                    IntPtr screenHdc = BeginPaint(m.HWnd, ref paintStruct);

                    if (effect.Texture != null)
                    {
                        Bitmap f = effect.Render(null); // no need to supply new texture since image is statis. render current texture to bitmap
                        using (Graphics screen = Graphics.FromHdc(screenHdc))
                        {
                            screen.DrawImage(f, this.ClientRectangle); // Draw current texture to bitmap
                        }

                    }


                    EndPaint(m.HWnd, ref paintStruct);
                    return;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }


        // Handles different control and image sizes
        private Point Translate(Point point)
        {
            Double cx = (double)this.Image.Width / this.Width;
            Double cy = (double)this.Image.Height / this.Height;
            return new Point((int)(cx * point.X), (int)(cy * point.Y));
        }

    }
}
