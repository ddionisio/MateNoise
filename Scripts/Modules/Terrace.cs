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
    /// Noise module that maps the output value from a source module onto a
    /// terrace-forming curve.
    /// 
    /// This noise module maps the output value from the source module onto a
    /// terrace-forming curve.  The start of this curve has a slope of zero;
    /// its slope then smoothly increases.  This curve also contains
    /// <i>control points</i> which resets the slope to zero at that point,
    /// producing a "terracing" effect.
    /// 
    /// To add a control point to this noise module, call the
    /// AddControlPoint() method.
    /// 
    /// An application must add a minimum of two control points to the curve.
    /// If this is not done, the GetValue() method fails.  The control points
    /// can have any value, although no two control points can have the same
    /// value.  There is no limit to the number of control points that can be
    /// added to the curve.
    ///
    /// This noise module clamps the output value from the source module if
    /// that value is less than the value of the lowest control point or
    /// greater than the value of the highest control point.
    ///
    /// This noise module is often used to generate terrain features such as
    /// your stereotypical desert canyon.
    ///
    /// This noise module requires one source module.
    /// </summary>
    public class Terrace : ModuleBase {
        public override int sourceModuleCount { get { return 1; } }

        /// <summary>
        ///  Returns the number of control points on the terrace-forming curve.
        /// </summary>
        public int controlPointCount { get { return mCtrlPts.Count; } }

        /// <summary>
        /// Determines if the terrace-forming curve between the control
        /// points is inverted.
        /// </summary>
        public bool invert = false;

        /// <summary>
        /// Adds a control point to the terrace-forming curve.
        ///
        /// NOTE: No two control points have the same value.
        ///
        /// Two or more control points define the terrace-forming curve.  The
        /// start of this curve has a slope of zero; its slope then smoothly
        /// increases.  At the control points, its slope resets to zero.
        ///
        /// It does not matter which order these points are added.
        /// </summary>
        public void AddControlPoint(float value) {
            // Find the insertion point for the new control point and insert the new
            // point at that position.  The control point array will remain sorted by
            // value.
            int insertionPos = FindInsertionPos(value);
            if(insertionPos != -1)
                mCtrlPts.Insert(insertionPos, value);
        }

        /// <summary>
        /// Deletes all the control points on the terrace-forming curve.
        /// </summary>
        public void ClearControlPoints() {
            mCtrlPts.Clear();
        }

        /// <summary>
        /// Creates a number of equally-spaced control points that range from
        /// -1 to +1.
        ///
        /// controlPointCount The number of control points to generate.
        ///
        /// The number of control points must be greater than or equal to
        /// 2.
        ///
        /// Note: The previous control points on the terrace-forming curve are
        /// deleted.
        ///
        /// Two or more control points define the terrace-forming curve.  The
        /// start of this curve has a slope of zero; its slope then smoothly
        /// increases.  At the control points, its slope resets to zero.
        /// </summary>
        public void MakeControlPoints(int controlPointCount) {
            if(controlPointCount < 2) {
                Debug.LogError("There needs to be more than 1 control points. Count given: "+controlPointCount);
                return;
            }

            ClearControlPoints();

            float terraceStep = 2.0f / ((float)controlPointCount - 1.0f);
            float curValue = -1.0f;
            for(int i = 0; i < controlPointCount; i++) {
                AddControlPoint(curValue);
                curValue += terraceStep;
            }
        }

        public override float GetValue(float x, float y, float z) {
            if(mCtrlPts.Count < 2) {
                Debug.LogError("There needs to be more than 1 control points, current count: "+mCtrlPts.Count);
                return -1.0f;
            }

            // Get the output value from the source module.
            float sourceModuleValue = mSourceModules[0].GetValue(x, y, z);

            // Find the first element in the control point array that has a value
            // larger than the output value from the source module.
            int indexPos;
            for(indexPos = 0; indexPos < mCtrlPts.Count; indexPos++) {
                if(sourceModuleValue < mCtrlPts[indexPos])
                    break;
            }

            // Find the two nearest control points so that we can map their values
            // onto a quadratic curve.
            int index0 = Mathf.Clamp(indexPos - 1, 0, mCtrlPts.Count - 1);
            int index1 = Mathf.Clamp(indexPos, 0, mCtrlPts.Count - 1);

            // If some control points are missing (which occurs if the output value from
            // the source module is greater than the largest value or less than the
            // smallest value of the control point array), get the value of the nearest
            // control point and exit now.
            if(index0 == index1)
                return mCtrlPts[index1];

            // Compute the alpha value used for linear interpolation.
            float value0 = mCtrlPts[index0];
            float value1 = mCtrlPts[index1];
            float alpha = (sourceModuleValue - value0) / (value1 - value0);
            if(invert) {
                alpha = 1.0f - alpha;
                Utils.Swap(ref value0, ref value1);
            }

            // Squaring the alpha produces the terrace effect.
            alpha *= alpha;

            // Now perform the linear interpolation given the alpha value.
            return Interpolate.Linear(value0, value1, alpha);
        }

        /// Determines the array index in which to insert the control point
        /// into the internal control point array.
        ///
        /// @param value The value of the control point.
        ///
        /// @returns The array index in which to insert the control point.
        ///
        /// @pre No two control points have the same value.
        ///
        /// @throw noise::ExceptionInvalidParam An invalid parameter was
        /// specified; see the preconditions for more information.
        ///
        /// By inserting the control point at the returned array index, this
        /// class ensures that the control point array is sorted by value.
        /// The code that maps a value onto the curve requires a sorted
        /// control point array.
        protected int FindInsertionPos(float value) {
            int insertionPos;
            for(insertionPos = 0; insertionPos < mCtrlPts.Count; insertionPos++) {
                if(value < mCtrlPts[insertionPos]) {
                    // We found the array index in which to insert the new control point.
                    // Exit now.
                    break;
                }
                else if(value == mCtrlPts[insertionPos]) {
                    // Each control point is required to contain a unique value, so throw
                    // an exception.
                    Debug.LogError("Invalid value, it must not be equal to another control point.");
                    return -1;
                }
            }
            return insertionPos;
        }

        private List<float> mCtrlPts = new List<float>();
    }
}