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
    /// Noise module that outputs the larger of the two output values from two
    /// source modules.
    /// 
    /// This noise module requires two source modules.
    /// </summary>
    public class Max : ModuleBase {
        public override int sourceModuleCount { get { return 2; } }

        public override float GetValue(float x, float y, float z) {
            float v0 = mSourceModules[0].GetValue(x, y, z);
            float v1 = mSourceModules[1].GetValue(x, y, z);
            return Mathf.Max(v0, v1);
        }
    }
}