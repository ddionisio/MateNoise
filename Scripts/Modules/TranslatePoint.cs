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
    /// Noise module that moves the coordinates of the input value before
    /// returning the output value from a source module.
    /// 
    /// The GetValue() method moves the (x, y, z) coordinates of
    /// the input value by a translation amount before returning the output
    /// value from the source module.  To set the translation amount, call
    /// the SetTranslation() method.  To set the translation amount to
    /// apply to the individual x, y, or z coordinates, set the
    /// translate property.
    ///
    /// This noise module requires one source module.
    /// </summary>
    public class TranslatePoint : ModuleBase {
        public override int sourceModuleCount { get { return 1; } }

        public Vector3 translate = Vector3.zero;

        public override float GetValue(float x, float y, float z) {
            return mSourceModules[0].GetValue(x+translate.x, y+translate.y, z+translate.z);
        }
    }
}