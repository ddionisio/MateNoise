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
namespace M8.Noise.Module {
    /// <summary>
    /// Noise module that applies a scaling factor and a bias to the output
    /// value from a source module.
    /// 
    /// The GetValue() method retrieves the output value from the source
    /// module, multiplies it with a scaling factor, adds a bias to it, then
    /// outputs the value.
    ///
    /// This noise module requires one source module.
    /// </summary>
    public class ScaleBias : ModuleBase {
        public override int sourceModuleCount { get { return 1; } }

        /// <summary>
        /// The GetValue() method retrieves the output value from the source
        /// module, multiplies it with the scaling factor, adds the bias to
        /// it, then outputs the value.
        /// </summary>
        public float bias = 0.0f;

        /// <summary>
        /// The GetValue() method retrieves the output value from the source
        /// module, multiplies it with the scaling factor, adds the bias to
        /// it, then outputs the value.
        /// </summary>
        public float scale = 1.0f;

        public override float GetValue(float x, float y, float z) {
            return mSourceModules[0].GetValue(x, y, z)*scale + bias;
        }

        public ScaleBias() : base() { }

        public ScaleBias(ModuleBase src, float _scale = 1.0f, float _bias = 0.0f) : base() { mSourceModules[0] = src; scale = _scale; bias = _bias; }
    }
}