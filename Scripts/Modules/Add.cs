
namespace M8.Noise.Module {
    /// <summary>
    /// Noise module that outputs the sum of the two output values from two
    /// source modules.
    /// </summary>
    public class Add : ModuleBase {
        public override int sourceModuleCount { get { return 2; } }

        public override float GetValue(float x, float y, float z) {
            return mSourceModules[0].GetValue(x, y, z) + mSourceModules[1].GetValue(x, y, z);
        }
    }
}
