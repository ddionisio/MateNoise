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
    public struct Interpolate {

        /// <summary>
        /// Linear interpolation between two values.
        /// </summary>
        public static float Linear(float n0, float n1, float t) {
            return (1.0f - t)*n0 + t*n1;
        }

        /// <summary>
        /// Performs cubic interpolation between two values bound between two other values. n0 = value before n1, n1 = first value, n2 = second value, n3 = value after n2.
        /// </summary>
        public static float Cubic(float n0, float n1, float n2, float n3, float t) {
            float p = (n3 - n2) - (n0 - n1);
            float q = (n0 - n1) - p;
            float r = n2 - n0;
            float s = n1;
            return p*t*t*t + q*t*t + r*t + s;
        }

        /// <summary>
        /// Maps value onto a cosine s-curve, slightly less curvy than cubic/quint, but much better performance.
        /// </summary>
        public static float CurveCos(float t) {
            return 1.0f - Mathf.Cos(t*Mathf.PI)*0.5f;
        }

        /// <summary>
        /// Maps value onto a cubic s-curve.
        /// </summary>
        public static float CurveCubic(float t) {
            return (t*t*(3.0f - 2.0f*t));
        }

        /// <summary>
        /// Maps value onto a quintic s-curve.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float CurveQuint(float t) {
            float t3 = t*t*t;
            float t4 = t3*t;
            float t5 = t4*t;
            return 6.0f*t5 - 15.0f*t4 + 10.0f*t3;
        }
    }
}