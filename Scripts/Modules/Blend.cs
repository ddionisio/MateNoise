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
    /// Unlike most other noise modules, the index value assigned to a source
    /// module determines its role in the blending operation:
    /// - Source module 0 outputs one of the values to blend. (lhs)
    /// - Source module 1 outputs one of the values to blend. (rhs)
    /// - Source module 2 is known as the <i>control module</i>.  (control)
    ///   The control module determines the weight of the
    ///   blending operation.  Negative values weigh the blend towards the
    ///   output value from the source module with an index value of 0.
    ///   Positive values weigh the blend towards the output value from the
    ///   source module with an index value of 1.
    ///
    /// An application can pass the control module by blend[0, 2].  This may make the
    /// application code easier to read.
    ///
    /// This noise module uses linear interpolation to perform the blending
    /// operation.
    ///
    /// This noise module requires three source modules.
    /// </summary>
    public class Blend : ModuleBase {
        public override int sourceModuleCount { get { return 3; } }

        /// <summary>
        /// The control module determines the weight of the blending
        /// operation.  Negative values weigh the blend towards the output
        /// value from the source module with an index value of 0.  Positive
        /// values weigh the blend towards the output value from the source
        /// module with an index value of 1.
        /// </summary>
        public ModuleBase controlModule { get { return mSourceModules[2]; } set { mSourceModules[2] = value; } }

        public override float GetValue(float x, float y, float z) {
            float v0 = mSourceModules[0].GetValue(x, y, z);
            float v1 = mSourceModules[1].GetValue(x, y, z);
            float alpha = mSourceModules[2].GetValue(x, y, z);
            return Interpolate.Linear(v0, v1, alpha);
        }
    }
}