using UnityEngine;

namespace M8.Noise.Module {
    /// <summary>
    /// Noise module that maps the output value from a source module onto an
    /// exponential curve.
    /// 
    /// Because most noise modules will output values that range from -1.0 to
    /// +1.0, this noise module first normalizes this output value (the range
    /// becomes 0.0 to 1.0), maps that value onto an exponential curve, then
    /// rescales that value back to the original range.
    ///
    /// This noise module requires one source module.
    /// </summary>
    public class Exponent : ModuleBase {
        public override int sourceModuleCount { get { return 1; } }

        /// <summary>
        /// Because most noise modules will output values that range from -1.0
        /// to +1.0, this noise module first normalizes this output value (the
        /// range becomes 0.0 to 1.0), maps that value onto an exponential
        /// curve, then rescales that value back to the original range.
        /// </summary>
        public float exponent = 1.0f;

        public override float GetValue(float x, float y, float z) {
            float value = mSourceModules[0].GetValue(x, y, z);
            return Mathf.Pow(Mathf.Abs((value + 1.0f)/2.0f), exponent)*2.0f - 1.0f;
        }
    }
}