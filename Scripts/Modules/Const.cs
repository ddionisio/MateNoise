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
    /// Noise module that outputs a constant value.
    /// 
    /// This noise module is not useful by itself, but it is often used as a
    /// source module for other noise modules.
    ///
    /// This noise module does not require any source modules.
    /// 
    /// Set value via property 'val'
    /// </summary>
    public class Const : ModuleBase {
        public float val { get { return mVal; } set { mVal = value; } }

        public override float GetValue(float x, float y, float z) {
            return mVal;
        }

        public Const() : base() { }

        public Const(float aVal)
            : base() {
                mVal = aVal;
        }

        private float mVal = 0.0f;
    }
}