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
using System.Collections.Generic;

namespace M8.Noise.Module {
    /// <summary>
    /// Noise module that maps the output value from a source module onto an
    /// arbitrary function curve.
    /// 
    /// This noise module maps the output value from the source module onto an
    /// application-defined curve.
    /// 
    /// This noise module requires one source module.
    /// </summary>
    public class Curve : ModuleBase {
        /// Control points are used for defining splines.
        public struct ControlPoint {
            /// The input value.
            public float inputValue;

            /// The output value that is mapped from the input value.
            public float outputValue;

        };

        /// <summary>
        /// Use for deserialization.
        /// </summary>
        public string pointParams {
            set {
                mCtrlPts.Clear();

                //input:output, input:output...
                char[] delims = new char[] { ',' };
                string[] pairs = value.Split(delims, System.StringSplitOptions.RemoveEmptyEntries);
                for(int i = 0; i < pairs.Length; i++) {
                    string[] pair = pairs[i].Split(':');
                    AddControlPoint(System.Convert.ToSingle(pair[0].Trim()), System.Convert.ToSingle(pair[1].Trim()));
                }
            }
        }

        public List<ControlPoint> controlPoints { get { return mCtrlPts; } }

        /// Adds a control point to the curve.
        ///
        /// @param inputValue The input value stored in the control point.
        /// @param outputValue The output value stored in the control point.
        ///
        /// @pre No two control points have the same input value.
        ///
        /// @throw noise::ExceptionInvalidParam An invalid parameter was
        /// specified; see the preconditions for more information.
        ///
        /// It does not matter which order these points are added.
        public void AddControlPoint(float inputValue, float outputValue) {
            int ind = FindInsertionPos(inputValue);
            if(ind != -1)
                InsertAtPos(ind, inputValue, outputValue);
        }

        public override int sourceModuleCount { get { return 1; } }

        public override float GetValue(float x, float y, float z) {
            int count = mCtrlPts.Count;

            if(count < 4) {
                Debug.LogWarning("Requires at least 4 control points.");
                return -1;
            }

            // Get the output value from the source module.
            float sourceModuleValue = mSourceModules[0].GetValue(x, y, z);

            // Find the first element in the control point array that has an input value
            // larger than the output value from the source module.
            int indexPos;
            for(indexPos = 0; indexPos < count; indexPos++) {
                if(sourceModuleValue < mCtrlPts[indexPos].inputValue) {
                    break;
                }
            }

            // Find the four nearest control points so that we can perform cubic
            // interpolation.
            int index0 = Mathf.Clamp(indexPos - 2, 0, count - 1);
            int index1 = Mathf.Clamp(indexPos - 1, 0, count - 1);
            int index2 = Mathf.Clamp(indexPos, 0, count - 1);
            int index3 = Mathf.Clamp(indexPos + 1, 0, count - 1);

            // If some control points are missing (which occurs if the value from the
            // source module is greater than the largest input value or less than the
            // smallest input value of the control point array), get the corresponding
            // output value of the nearest control point and exit now.
            if(index1 == index2) {
                return mCtrlPts[index1].outputValue;
            }

            // Compute the alpha value used for cubic interpolation.
            float input0 = mCtrlPts[index1].inputValue;
            float input1 = mCtrlPts[index2].inputValue;
            float alpha = (sourceModuleValue - input0) / (input1 - input0);

            // Now perform the cubic interpolation given the alpha value.
            return Interpolate.Cubic(
              mCtrlPts[index0].outputValue,
              mCtrlPts[index1].outputValue,
              mCtrlPts[index2].outputValue,
              mCtrlPts[index3].outputValue,
              alpha);
        }

        /// Determines the array index in which to insert the control point
        /// into the internal control point array.
        ///
        /// @param inputValue The input value of the control point.
        ///
        /// @returns The array index in which to insert the control point.
        ///
        /// @pre No two control points have the same input value.
        ///
        /// @throw noise::ExceptionInvalidParam An invalid parameter was
        /// specified; see the preconditions for more information.
        ///
        /// By inserting the control point at the returned array index, this
        /// class ensures that the control point array is sorted by input
        /// value.  The code that maps a value onto the curve requires a
        /// sorted control point array.
        protected int FindInsertionPos(float inputValue) {
            int insertionPos;
            for(insertionPos = 0; insertionPos < mCtrlPts.Count; insertionPos++) {
                if(inputValue < mCtrlPts[insertionPos].inputValue) {
                    // We found the array index in which to insert the new control point.
                    // Exit now.
                    break;
                }
                else if(inputValue == mCtrlPts[insertionPos].inputValue) {
                    // Each control point is required to contain a unique input value
                    Debug.LogWarning("Invalid input: "+inputValue+" already placed at: "+insertionPos);
                    return -1;
                }
            }
            return insertionPos;
        }

        /// Inserts the control point at the specified position in the
        /// internal control point array.
        ///
        /// @param insertionPos The zero-based array position in which to
        /// insert the control point.
        /// @param inputValue The input value stored in the control point.
        /// @param outputValue The output value stored in the control point.
        ///
        /// To make room for this new control point, this method reallocates
        /// the control point array and shifts all control points occurring
        /// after the insertion position up by one.
        ///
        /// Because the curve mapping algorithm used by this noise module
        /// requires that all control points in the array must be sorted by
        /// input value, the new control point should be inserted at the
        /// position in which the order is still preserved.
        protected void InsertAtPos(int insertionPos, float inputValue, float outputValue) {

        }

        private List<ControlPoint> mCtrlPts = new List<ControlPoint>();
    }
}