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
    /// Noise module that rotates the input value around the origin before
    /// returning the output value from a source module.
    /// 
    /// The GetValue() method rotates the coordinates of the input value
    /// around the origin before returning the output value from the source
    /// module.  To set the rotation angles, use the rotation field.
    ///
    /// The coordinate system of the input value is assumed to be
    /// "left-handed" (x increases to the right, y increases upward,
    /// and z increases inward.)
    ///
    /// This noise module requires one source module.
    /// </summary>
    public class RotatePoint : ModuleBase {
        public override int sourceModuleCount { get { return 1; } }

        public Quaternion rotation = Quaternion.identity;

        public string rotateParam {
            set {
                //format: x, y, z
                string[] axis = value.Split(',');
                rotation = Quaternion.Euler(System.Convert.ToSingle(axis[0].Trim()), System.Convert.ToSingle(axis[1].Trim()), System.Convert.ToSingle(axis[2].Trim()));
            }
        }

        public override float GetValue(float x, float y, float z) {
            return mSourceModules[0].GetValue(rotation*new Vector3(x, y, z));
        }
    }
}