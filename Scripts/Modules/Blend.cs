
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