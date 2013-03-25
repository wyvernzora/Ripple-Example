// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
// libWyvernzora/WinForms/Effects/RippleEffect.cs
// --------------------------------------------------------------------------------
// Copyright (C) 2013, Jieni Luchijinzhou a.k.a Aragorn Wyvernzora
// All rights reserved.
// 
// This file is part of libWyvernzora.
// 
//     libWyvernzora is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     libWyvernzora is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with libWyvernzora.  If not, see <http://www.gnu.org/licenses/>.
// 
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-

using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace libWyvernzora.WinForms.Effects
{
    /// <summary>
    /// 2D Simulater Ripple Effect
    /// </summary>
    public unsafe class RippleEffect
    {
        // Back Buffer
        private Int32[] backBuffer;     // Wave Height Frame
        private Bitmap frame;           // Animation Frame

        // Front Buffer
        private Int32[] frontBuffer;    // Wave Height Frame
        private Bitmap texture;         // Animation Frame

        /// <summary>
        /// Overloaded.
        /// Initializes a new instance of RippleEffect
        /// </summary>
        /// <remarks>
        /// NOTE: You have to call Initialize() if you use this constructor overload!
        /// </remarks>
        public RippleEffect()
        {
        }

        /// <summary>
        /// Overloaded.
        /// Initializes a new instance of RippleEffect
        /// </summary>
        /// <param name="w">Width of the buffer</param>
        /// <param name="h">Height of the buffer</param>
        public RippleEffect(int w, int h)
        {
            // allocate buffers
            frontBuffer = new Int32[w * h];
            backBuffer = new Int32[frontBuffer.Length];
            frame = new Bitmap(w, h);
        }

        /// <summary>
        /// Initializes the ripple effect according to the texture assigned.
        /// </summary>
        private void Initialize()
        {
            if (texture == null)
                throw new Exception();

            // allocate buffers
            frontBuffer = new Int32[texture.Width * texture.Height];
            backBuffer = new Int32[frontBuffer.Length];

            frame = new Bitmap(texture);
        }

        #region Properties

        /// <summary>
        /// Gets or sets the base texture without effects applied.
        /// </summary>
        public Bitmap Texture
        {
            get { return texture; }
            set
            {
                texture = value;
                Initialize();
            }
        }
        /// <summary>
        /// Gets the base texture with effects applied.
        /// </summary>
        public Bitmap Frame
        {
            get { return frame; }
        }

        #endregion

        #region Update/Render

        /// <summary>
        /// Updates buffers to the next animation frame.
        /// Does not render the new animation frame, however.
        /// </summary>
        public void Update()
        {
            // accessing properties involves calling methods!
            int w = texture.Width;
            int h = texture.Height;

            // fix front buffer so that .net runtime doesn't move it
            fixed (int* pF = frontBuffer, pB = backBuffer)
            {
                // top/bottom edge pixels are excluded from the loop
                for (int i = w; i < w * h - w; i++)
                {
                    // if current pixel is a left/right edge pixel, don't process it
                    if (i % w == 1 || i % w == w - 1) continue;

                    // calculate wave height
                    pB[i] = ((
                                 pF[i - 1] +
                                 pF[i + 1] +
                                 pF[i - w] +
                                 pF[i + w]) >> 1) - pB[i];
                    pB[i] -= pB[i] >> 5;
                }
            }

            // flip buffers
            int[] tmp = frontBuffer;
            frontBuffer = backBuffer;
            backBuffer = tmp;
        }

        /// <summary>
        /// Renders the next animation frame.
        /// </summary>
        /// <param name="ntxt">New base texture; null if the same texture as previous frame</param>
        /// <returns>Bitmap: rendered next frame</returns>
        public Bitmap Render(Bitmap ntxt)
        {
            // apply new texture if needed
            if (ntxt != null)
            {
                texture = ntxt;
            }

            // avoid using properties
            int w = texture.Width;
            int h = texture.Height;

            // copy current texture
            //frame = new Bitmap(texture);

            // lock bitmap data
            BitmapData fdat = frame.LockBits(new Rectangle(0, 0, texture.Width, texture.Height),
                                             ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData tdat = texture.LockBits(new Rectangle(0, 0, texture.Width, texture.Height),
                                               ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            // fix front buffer
            fixed (int* buffer = frontBuffer)
            {
                // get bitmap data pointers
                byte* faddr = (byte*) fdat.Scan0;
                byte* taddr = (byte*) tdat.Scan0;

                for (int i = w; i < w * h - w; i++)
                {
                    // calculate x and y offsets
                    int xo = buffer[i - 1] - buffer[i + 1];
                    int yo = buffer[i - w] - buffer[i + w];

                    // shader
                    int shade = (xo - yo) / 4;

                    // temporary variables to avoid redundant calculation
                    int pxi = i * 3;
                    int fxi = pxi + xo * 3 + yo * tdat.Stride;
                    if (fxi < 0) fxi = pxi;
                    if (fxi >= w * h * 3) fxi = pxi;

                    // get pixel from texture
                    byte r = taddr[fxi];
                    byte g = taddr[fxi + 1];
                    byte b = taddr[fxi + 2];

                    // apply ripple effect
                    r = (byte) CoerceValue(r + shade, 0, 255);
                    g = (byte) CoerceValue(g + shade, 0, 255);
                    b = (byte) CoerceValue(b + shade, 0, 255);

                    // write pixel to frame buffer
                    faddr[pxi] = r;
                    faddr[pxi + 1] = g;
                    faddr[pxi + 2] = b;
                }

                // copy edge rows
                for (int i = 0; i < w * 3; i++)
                {
                    faddr[(w * h) * 3 - i] = taddr[(w * h) * 3 - i];
                    faddr[i] = taddr[i];
                }
            }

            // unlock bitmap data
            frame.UnlockBits(fdat);
            texture.UnlockBits(tdat);

            return frame;
        }

        #endregion

        #region Interaction

        /// <summary>
        /// Create a "Splash" at a target location
        /// </summary>
        /// <param name="x">X coordinate of the splash</param>
        /// <param name="y">Y coordinate of the splash</param>
        /// <param name="r">Radius of the splash in pixels</param>
        public void Splash(int x, int y, Int32 r)
        {
            if (x < 0 || x >= texture.Width || y < 0 || y >= texture.Height) return;

            Rectangle effectRect = new Rectangle(0, 0, texture.Width, texture.Height);

            for (int iy = y - r; iy < y + r; iy++)
            {
                for (int ix = x - r; ix < x + r; ix++)
                {
                    Double d = Math.Sqrt(Math.Pow(ix - x, 2) + Math.Pow(iy - y, 2));
                    Point p = new Point(ix, iy);
                    if (d < r && effectRect.Contains(p))
                        frontBuffer[ix + iy * effectRect.Width] = (int) (255 - (768 * (1 - d / r / 2)));
                }
            }
        }

        /// <summary>
        /// Clears buffers, i.e. resets animation to the original state
        /// </summary>
        public void Clear()
        {
            if (texture != null)
            {
                frontBuffer = new Int32[texture.Width * texture.Height];
                backBuffer = new Int32[frontBuffer.Length];
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Makes sure that the raw is within defined range
        /// </summary>
        /// <param name="raw"></param>
        /// <param name="lo"></param>
        /// <param name="hi"></param>
        /// <returns></returns>
        private int CoerceValue(int raw, int lo, int hi)
        {
            if (raw <= lo) return lo;
            else if (raw >= hi) return hi;
            else return raw;
        }

        #endregion
    }
}