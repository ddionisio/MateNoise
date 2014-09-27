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
    /// Noise module that outputs a checkerboard pattern.
    ///
    /// This noise module outputs unit-sized blocks of alternating values.
    /// The values of these blocks alternate between -1.0 and +1.0.
    ///
    /// This noise module is not really useful by itself, but it is often used
    /// for debugging purposes.
    ///
    /// This noise module does not require any source modules.
    /// </summary>
    public class CheckerBoard : ModuleBase {
        public override float GetValue(float x, float y, float z) {
            int ix = Mathf.FloorToInt(x);
            int iy = Mathf.FloorToInt(y);
            int iz = Mathf.FloorToInt(z);
            return (ix & 1 ^ iy & 1 ^ iz & 1) != 0 ? -1.0f : 1.0f;
        }
    }
}