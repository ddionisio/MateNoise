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

namespace M8.Noise.Module {
    /// <summary>
    /// The range of values in which to clamp the output value is called the
    /// <i>clamping range</i>.
    ///
    /// If the output value from the source module is less than the lower
    /// bound of the clamping range, this noise module clamps that value to
    /// the lower bound.  If the output value from the source module is
    /// greater than the upper bound of the clamping range, this noise module
    /// clamps that value to the upper bound.
    ///
    /// To specify the upper and lower bounds of the clamping range, set the properties: min, max.
    /// 
    /// This noise module requires one source module.
    /// </summary>
    public class Clamp : ModuleBase {
        public override int sourceModuleCount { get { return 1; } }
        public float min { get { return mMin; } set { mMin = value; } }
        public float max { get { return mMax; } set { mMax = value; } }

        public override float GetValue(float x, float y, float z) {
            float val = mSourceModules[0].GetValue(x, y, z);
            return Mathf.Clamp(val, mMin, mMax);
        }

        private float mMin = -1.0f;
        private float mMax = 1.0f;
    }
}