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
    /// arbitrary function curve.
    /// 
    /// This noise module maps the output value from the source module onto an
    /// application-defined curve.  This curve is defined by Unity's curve system.
    /// 
    /// Value is evaluated into the animation curve.
    /// 
    /// Ensure the control points lie within the range [-1, 1].
    /// 
    /// This noise module requires one source module.
    /// </summary>
    public class Curve : ModuleBase {
        public override int sourceModuleCount { get { return 1; } }

        public AnimationCurve curve { get { return mCurve; } }
        
        public override float GetValue(float x, float y, float z) {
            float val = mSourceModules[0].GetValue(x, y, z);

            return mCurve.Evaluate(val);
        }

        private AnimationCurve mCurve;

        public Curve() : base() { mCurve = new AnimationCurve(); }

        public Curve(AnimationCurve _curve) : base() { mCurve = _curve; }

        public Curve(ModuleBase src, AnimationCurve _curve) : base() { mSourceModules[0] = src; mCurve = _curve; }
    }
}