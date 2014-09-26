
namespace M8.Noise.Module {
    /// <summary>
    /// Noise module that outputs the value selected from one of two source
    /// modules chosen by the output value from a control module.
    /// 
    /// Unlike most other noise modules, the index value assigned to a source
    /// module determines its role in the selection operation:
    /// - Source module 0 outputs a value.
    /// - Source module 1 outputs a value.
    /// - Source module 2 is known as the <i>control module</i>.  The control 
    ///   module determines the value to select.  If
    ///   the output value from the control module is within a range of values
    ///   known as the <i>selection range</i>, this noise module outputs the
    ///   value from the source module with an index value of 1.  Otherwise,
    ///   this noise module outputs the value from the source module with an
    ///   index value of 0.
    ///
    /// To specify the bounds of the selection range, call the SetBounds()
    /// method.
    ///
    /// An application can pass the control module to the controlModule
    /// method instead of the this[].  This may make the
    /// application code easier to read.
    ///
    /// By default, there is an abrupt transition between the output values
    /// from the two source modules at the selection-range boundary.  To
    /// smooth the transition, pass a non-zero value to the SetEdgeFalloff()
    /// method.  Higher values result in a smoother transition.
    ///
    /// This noise module requires three source modules.
    /// </summary>
    public class Select : ModuleBase {
        public override int sourceModuleCount { get { return 3; } }

        /// <summary>
        /// The control module determines the output value to select.  If the
        /// output value from the control module is within a range of values
        /// known as the <i>selection range</i>, the GetValue() method outputs
        /// the value from the source module with an index value of 1.
        /// Otherwise, this method outputs the value from the source module
        /// with an index value of 0.
        ///
        /// This method assigns the control module an index value of 2.
        /// Passing the control module to this method produces the same
        /// results as passing the control module to the SetSourceModule()
        /// method while assigning that noise module an index value of 2.
        ///
        /// This control module must exist throughout the lifetime of this
        /// noise module unless another control module replaces that control
        /// module.
        /// </summary>
        public ModuleBase controlModule { get { return mSourceModules[2]; } set { mSourceModules[2] = value; } }

        /// <summary>
        /// If the output value from the control module is within the
        /// selection range, the GetValue() method outputs the value from the
        /// source module with an index value of 1.  Otherwise, this method
        /// outputs the value from the source module with an index value of 0.
        /// </summary>
        public float lowerBound { get { return mLowerBound; } }

        /// <summary>
        /// If the output value from the control module is within the
        /// selection range, the GetValue() method outputs the value from the
        /// source module with an index value of 1.  Otherwise, this method
        /// outputs the value from the source module with an index value of 0.
        /// </summary>
        public float upperBound { get { return mUpperBound; } }

        /// <summary>
        /// The falloff value is the width of the edge transition at either
        /// edge of the selection range.
        ///
        /// By default, there is an abrupt transition between the values from
        /// the two source modules at the boundaries of the selection range.
        ///
        /// For example, if the selection range is 0.5 to 0.8, and the edge
        /// falloff value is 0.1, then the GetValue() method outputs:
        /// - the output value from the source module with an index value of 0
        ///   if the output value from the control module is less than 0.4
        ///   ( = 0.5 - 0.1).
        /// - a linear blend between the two output values from the two source
        ///   modules if the output value from the control module is between
        ///   0.4 ( = 0.5 - 0.1) and 0.6 ( = 0.5 + 0.1).
        /// - the output value from the source module with an index value of 1
        ///   if the output value from the control module is between 0.6
        ///   ( = 0.5 + 0.1) and 0.7 ( = 0.8 - 0.1).
        /// - a linear blend between the output values from the two source
        ///   modules if the output value from the control module is between
        ///   0.7 ( = 0.8 - 0.1 ) and 0.9 ( = 0.8 + 0.1).
        /// - the output value from the source module with an index value of 0
        ///   if the output value from the control module is greater than 0.9
        ///   ( = 0.8 + 0.1).
        /// </summary>
        public float edgeFallOff {
            get { return mEdgeFalloff; }
            set {
                float boundSize = mUpperBound - mLowerBound;
                mEdgeFalloff = mEdgeFalloff > boundSize*0.5f ? boundSize*0.5f : mEdgeFalloff;
            }
        }

        /// <summary>
        /// Sets the lower and upper bounds of the selection range.
        /// 
        /// If the output value from the control module is within the
        /// selection range, the GetValue() method outputs the value from the
        /// source module with an index value of 1.  Otherwise, this method
        /// outputs the value from the source module with an index value of 0.
        /// </summary>
        public void SetBound(float lower, float upper) {
            mLowerBound = lower;
            mUpperBound = upper;

            // Make sure that the edge falloff curves do not overlap.
            edgeFallOff = mEdgeFalloff;
        }

        public override float GetValue(float x, float y, float z) {
            float controlValue = mSourceModules[2].GetValue(x, y, z);
            float alpha;
            if(mEdgeFalloff > 0.0f) {
                if(controlValue < (mLowerBound - mEdgeFalloff)) {
                    // The output value from the control module is below the selector
                    // threshold; return the output value from the first source module.
                    return mSourceModules[0].GetValue(x, y, z);

                }
                else if(controlValue < (mLowerBound + mEdgeFalloff)) {
                    // The output value from the control module is near the lower end of the
                    // selector threshold and within the smooth curve. Interpolate between
                    // the output values from the first and second source modules.
                    float lowerCurve = (mLowerBound - mEdgeFalloff);
                    float upperCurve = (mLowerBound + mEdgeFalloff);
                    alpha = Interpolate.CurveCubic((controlValue - lowerCurve) / (upperCurve - lowerCurve));
                    return Interpolate.Linear(mSourceModules[0].GetValue(x, y, z), mSourceModules[1].GetValue(x, y, z), alpha);

                }
                else if(controlValue < (mUpperBound - mEdgeFalloff)) {
                    // The output value from the control module is within the selector
                    // threshold; return the output value from the second source module.
                    return mSourceModules[1].GetValue(x, y, z);

                }
                else if(controlValue < (mUpperBound + mEdgeFalloff)) {
                    // The output value from the control module is near the upper end of the
                    // selector threshold and within the smooth curve. Interpolate between
                    // the output values from the first and second source modules.
                    float lowerCurve = (mUpperBound - mEdgeFalloff);
                    float upperCurve = (mUpperBound + mEdgeFalloff);
                    alpha = Interpolate.CurveCubic((controlValue - lowerCurve) / (upperCurve - lowerCurve));
                    return Interpolate.Linear(mSourceModules[1].GetValue(x, y, z), mSourceModules[0].GetValue(x, y, z), alpha);

                }
                else {
                    // Output value from the control module is above the selector threshold;
                    // return the output value from the first source module.
                    return mSourceModules[0].GetValue(x, y, z);
                }
            }
            else {
                if(controlValue < mLowerBound || controlValue > mUpperBound) {
                    return mSourceModules[0].GetValue(x, y, z);
                }
                else {
                    return mSourceModules[1].GetValue(x, y, z);
                }
            }
        }

        private float mLowerBound = -1.0f;
        private float mUpperBound = 1.0f;
        private float mEdgeFalloff = 0.0f;
    }
}