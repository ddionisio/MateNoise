// Copyright (C) 2003, 2004 Jason Bevins
//
// This library is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2.1 of the License, or (at
// your option) any later version.
//
// This library is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
// License (COPYING.txt) for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this library; if not, write to the Free Software Foundation,
// Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// The developer's email is jlbezigvins@gmzigail.com (for great email, take
// off every 'zig'.)
//
using UnityEngine;

namespace M8.Noise {
    public enum Quality {
        Linear,
        Cosine, //slightly less curvy than cubic, but with better performance
        Cubic,
        Quint
    }

    public struct Generate {
        const int X_NOISE_GEN = 1619;
        const int Y_NOISE_GEN = 31337;
        const int Z_NOISE_GEN = 6971;
        const int SEED_NOISE_GEN = 1013;
        const int SHIFT_NOISE_GEN = 8;

        /// <summary>
        /// Generates a gradient-coherent-noise value from the coordinates of a
        /// three-dimensional input value. 
        /// returns [-1, 1]
        /// </summary>
        public static float GradientCoherent3D(float x, float y, float z, int seed = 0, Quality quality = Quality.Cubic) {
            // Create a unit-length cube aligned along an integer boundary.  This cube
            // surrounds the input point.
            int x0 = Mathf.FloorToInt(x);
            int x1 = x0 + 1;
            int y0 = Mathf.FloorToInt(y);
            int y1 = y0 + 1;
            int z0 = Mathf.FloorToInt(z);
            int z1 = z0 + 1;

            // Map the difference between the coordinates of the input value and the
            // coordinates of the cube's outer-lower-left vertex onto an S-curve.
            float xs=0f, ys=0f, zs=0f;
            switch(quality) {
                case Quality.Linear:
                    xs = x - (float)x0;
                    ys = y - (float)y0;
                    zs = z - (float)z0;
                    break;
                case Quality.Cosine:
                    xs = Interpolate.CurveCos(x - (float)x0);
                    ys = Interpolate.CurveCos(y - (float)y0);
                    zs = Interpolate.CurveCos(z - (float)z0);
                    break;
                case Quality.Cubic:
                    xs = Interpolate.CurveCubic(x - (float)x0);
                    ys = Interpolate.CurveCubic(y - (float)y0);
                    zs = Interpolate.CurveCubic(z - (float)z0);
                    break;
                case Quality.Quint:
                    xs = Interpolate.CurveQuint(x - (float)x0);
                    ys = Interpolate.CurveQuint(y - (float)y0);
                    zs = Interpolate.CurveQuint(z - (float)z0);
                    break;
            }

            // Now calculate the noise values at each vertex of the cube.  To generate
            // the coherent-noise value at the input point, interpolate these eight
            // noise values using the S-curve value as the interpolant (trilinear
            // interpolation.)
            float n0, n1, ix0, ix1, iy0, iy1;
            n0   = Gradient3D(x, y, z, x0, y0, z0, seed);
            n1   = Gradient3D(x, y, z, x1, y0, z0, seed);
            ix0  = Interpolate.Linear(n0, n1, xs);
            n0   = Gradient3D(x, y, z, x0, y1, z0, seed);
            n1   = Gradient3D(x, y, z, x1, y1, z0, seed);
            ix1  = Interpolate.Linear(n0, n1, xs);
            iy0  = Interpolate.Linear(ix0, ix1, ys);
            n0   = Gradient3D(x, y, z, x0, y0, z1, seed);
            n1   = Gradient3D(x, y, z, x1, y0, z1, seed);
            ix0  = Interpolate.Linear(n0, n1, xs);
            n0   = Gradient3D(x, y, z, x0, y1, z1, seed);
            n1   = Gradient3D(x, y, z, x1, y1, z1, seed);
            ix1  = Interpolate.Linear(n0, n1, xs);
            iy1  = Interpolate.Linear(ix0, ix1, ys);

            return Interpolate.Linear(iy0, iy1, zs);
        }

        /// <summary>
        /// Generates a gradient-noise value from the coordinates of a
        /// three-dimensional input value and the integer coordinates of a
        /// nearby three-dimensional value.  (fx, fy, fz) corresponds to coordinates within (ix, iy, iz).
        /// The difference between (fx, fy, fz) and (ix, iy, iz) must be less than or equal to 1.
        /// returns [-1, 1]
        /// </summary>
        public static float Gradient3D(float fx, float fy, float fz, int ix, int iy, int iz, int seed = 0) {
            // Randomly generate a gradient vector given the integer coordinates of the
            // input value.  This implementation generates a random number and uses it
            // as an index into a normalized-vector lookup table.
            long vectorIndex = (
                X_NOISE_GEN*ix
              + Y_NOISE_GEN*iy
              + Z_NOISE_GEN*iz
              + SEED_NOISE_GEN*seed)
              & 0xffffffff;
            vectorIndex ^= (vectorIndex >> SHIFT_NOISE_GEN);
            vectorIndex &= 0xff;

            float xvGradient = Utils.vectors[(vectorIndex << 2)];
            float yvGradient = Utils.vectors[(vectorIndex << 2) + 1];
            float zvGradient = Utils.vectors[(vectorIndex << 2) + 2];

            // Set up us another vector equal to the distance between the two vectors
            // passed to this function.
            float xvPoint = (fx - (float)ix);
            float yvPoint = (fy - (float)iy);
            float zvPoint = (fz - (float)iz);

            // Now compute the dot product of the gradient vector with the distance
            // vector.  The resulting value is gradient noise.  Apply a scaling value
            // so that this noise value ranges from -1.0 to 1.0.
            return ((xvGradient * xvPoint)
                  + (yvGradient * yvPoint)
                  + (zvGradient * zvPoint)) * 2.12f;
        }

        /// <summary>
        /// Generates an integer-noise value from the coordinates of a
        /// three-dimensional input value. 
        /// returns [0, 2147483647]
        /// </summary>
        public static int IntValue3D(int x, int y, int z, int seed = 0) {
            // All constants are primes and must remain prime in order for this noise
            // function to work correctly.
            int n = (
                X_NOISE_GEN*x
              + Y_NOISE_GEN*y
              + Z_NOISE_GEN*z
              + SEED_NOISE_GEN*seed)
              & 0x7fffffff;
            n = (n >> 13) ^ n;
            return (n * (n * n * 60493 + 19990303) + 1376312589) & 0x7fffffff;
        }

        /// <summary>
        /// Generates a value-coherent-noise value from the coordinates of a
        /// three-dimensional input value.
        /// returns [-1, 1]
        /// </summary>
        public static float ValueCoherent3D(float x, float y, float z, int seed = 0, Quality quality = Quality.Cosine) {
            // Create a unit-length cube aligned along an integer boundary.  This cube
            // surrounds the input point.
            int x0 = Mathf.FloorToInt(x);
            int x1 = x0 + 1;
            int y0 = Mathf.FloorToInt(y);
            int y1 = y0 + 1;
            int z0 = Mathf.FloorToInt(z);
            int z1 = z0 + 1;

            // Map the difference between the coordinates of the input value and the
            // coordinates of the cube's outer-lower-left vertex onto an S-curve.
            float xs=0f, ys=0f, zs=0f;
            switch(quality) {
                case Quality.Linear:
                    xs = x - (float)x0;
                    ys = y - (float)y0;
                    zs = z - (float)z0;
                    break;
                case Quality.Cosine:
                    xs = Interpolate.CurveCos(x - (float)x0);
                    ys = Interpolate.CurveCos(y - (float)y0);
                    zs = Interpolate.CurveCos(z - (float)z0);
                    break;
                case Quality.Cubic:
                    xs = Interpolate.CurveCubic(x - (float)x0);
                    ys = Interpolate.CurveCubic(y - (float)y0);
                    zs = Interpolate.CurveCubic(z - (float)z0);
                    break;
                case Quality.Quint:
                    xs = Interpolate.CurveQuint(x - (float)x0);
                    ys = Interpolate.CurveQuint(y - (float)y0);
                    zs = Interpolate.CurveQuint(z - (float)z0);
                    break;
            }

            // Now calculate the noise values at each vertex of the cube.  To generate
            // the coherent-noise value at the input point, interpolate these eight
            // noise values using the S-curve value as the interpolant (trilinear
            // interpolation.)
            float n0, n1, ix0, ix1, iy0, iy1;
            n0   = Value3D(x0, y0, z0, seed);
            n1   = Value3D(x1, y0, z0, seed);
            ix0  = Interpolate.Linear(n0, n1, xs);
            n0   = Value3D(x0, y1, z0, seed);
            n1   = Value3D(x1, y1, z0, seed);
            ix1  = Interpolate.Linear(n0, n1, xs);
            iy0  = Interpolate.Linear(ix0, ix1, ys);
            n0   = Value3D(x0, y0, z1, seed);
            n1   = Value3D(x1, y0, z1, seed);
            ix0  = Interpolate.Linear(n0, n1, xs);
            n0   = Value3D(x0, y1, z1, seed);
            n1   = Value3D(x1, y1, z1, seed);
            ix1  = Interpolate.Linear(n0, n1, xs);
            iy1  = Interpolate.Linear(ix0, ix1, ys);
            return Interpolate.Linear(iy0, iy1, zs);
        }

        /// <summary>
        /// Generates a value-noise value from the coordinates of a
        /// three-dimensional input value.
        /// returns [-1, 1]
        /// </summary>
        public static float Value3D(int x, int y, int z, int seed = 0) {
            return 1.0f - ((float)IntValue3D(x, y, z, seed) / 1073741824.0f);
        }
    }
}