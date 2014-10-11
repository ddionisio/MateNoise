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
    /// Noise module that scales the coordinates of the input value before
    /// returning the output value from a source module.
    /// 
    /// The GetValue() method multiplies the (x, y, z) coordinates
    /// of the input value with a scaling factor before returning the output
    /// value from the source module.  To set the scaling factor, call the
    /// SetScale() method.  To set the scaling factor to apply to the
    /// individual x, y, or z coordinates, by setting scale field.
    ///
    /// This noise module requires one source module.
    /// </summary>
    public class ScalePoint : ModuleBase {
        public override int sourceModuleCount { get { return 1; } }

        public Vector3 scale = Vector3.one;

        public string rotateParam {
            set {
                //format: x, y, z
                string[] axis = value.Split(',');
                scale = new Vector3(System.Convert.ToSingle(axis[0].Trim()), System.Convert.ToSingle(axis[1].Trim()), System.Convert.ToSingle(axis[2].Trim()));
            }
        }

        public override float GetValue(float x, float y, float z) {
            return mSourceModules[0].GetValue(x*scale.x, y*scale.y, z*scale.z);
        }

        public ScalePoint() : base() { }

        public ScalePoint(Vector3 _scale) : base() { scale = _scale; }

        public ScalePoint(float s = 1.0f) : base() { scale = new Vector3(s, s, s); }

        public ScalePoint(ModuleBase src, float s = 1.0f) : base() { scale = new Vector3(s, s, s); mSourceModules[0] = src; }

        public ScalePoint(ModuleBase src, Vector3 _scale) : base() { scale = _scale; mSourceModules[0] = src; }
    }
}