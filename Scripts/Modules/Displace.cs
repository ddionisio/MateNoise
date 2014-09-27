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
    /// Noise module that uses three source modules to displace each
    /// coordinate of the input value before returning the output value from
    /// a source module.
    /// 
    /// Unlike most other noise modules, the index value assigned to a source
    /// module determines its role in the displacement operation:
    /// - Source module 0 outputs a value.
    /// - Source module 1 specifies the offset to apply to the x coordinate of the input value.
    /// - Source module 2 specifies the offset to apply to the y coordinate of the input value.
    /// - Source module 3 specifies the offset to apply to the z coordinate of the input value.
    ///
    /// The GetValue() method modifies the (x, y, z) coordinates of
    /// the input value using the output values from the three displacement
    /// modules before retrieving the output value from the source module.
    ///
    /// The Turbulence noise module is a special case of the
    /// displacement module; internally, there are three Perlin-noise modules
    /// that perform the displacement operation.
    ///
    /// This noise module requires four source modules.
    /// </summary>
    public class Displace : ModuleBase {
        public override int sourceModuleCount { get { return 4; } }

        public ModuleBase outputModule { get { return mSourceModules[0]; } set { mSourceModules[0] = value; } }
        public ModuleBase xDisplaceModule { get { return mSourceModules[1]; } set { mSourceModules[1] = value; } }
        public ModuleBase yDisplaceModule { get { return mSourceModules[2]; } set { mSourceModules[2] = value; } }
        public ModuleBase zDisplaceModule { get { return mSourceModules[3]; } set { mSourceModules[3] = value; } }

        public override float GetValue(float x, float y, float z) {
            // Get the output values from the three displacement modules.  Add each
            // value to the corresponding coordinate in the input value.
            float xDisplace = x + (mSourceModules[1].GetValue(x, y, z));
            float yDisplace = y + (mSourceModules[2].GetValue(x, y, z));
            float zDisplace = z + (mSourceModules[3].GetValue(x, y, z));

            // Retrieve the output value using the offsetted input value instead of
            // the original input value.
            return mSourceModules[0].GetValue(xDisplace, yDisplace, zDisplace);
        }
    }
}