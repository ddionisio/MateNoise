
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
    }
}