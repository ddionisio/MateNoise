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
    /// Noise module that maps the output value from a source module onto an
    /// exponential curve.
    /// 
    /// Because most noise modules will output values that range from -1.0 to
    /// +1.0, this noise module first normalizes this output value (the range
    /// becomes 0.0 to 1.0), maps that value onto an exponential curve, then
    /// rescales that value back to the original range.
    ///
    /// This noise module requires one source module.
    /// </summary>
    public class Exponent : ModuleBase {
        public override int sourceModuleCount { get { return 1; } }

        /// <summary>
        /// Because most noise modules will output values that range from -1.0
        /// to +1.0, this noise module first normalizes this output value (the
        /// range becomes 0.0 to 1.0), maps that value onto an exponential
        /// curve, then rescales that value back to the original range.
        /// </summary>
        public float exponent = 1.0f;

        public override float GetValue(float x, float y, float z) {
            float value = mSourceModules[0].GetValue(x, y, z);
            return Mathf.Pow(Mathf.Abs((value + 1.0f)/2.0f), exponent)*2.0f - 1.0f;
        }
    }
}