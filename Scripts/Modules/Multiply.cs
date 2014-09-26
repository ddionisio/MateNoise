
namespace M8.Noise.Module {
    /// <summary>
    /// Noise module that outputs the product of the two output values from
    /// two source modules.
    /// 
    /// This noise module requires two source modules.
    /// </summary>
    public class Multiply : ModuleBase {
        public override int sourceModuleCount { get { return 2; } }

        public override float GetValue(float x, float y, float z) {
            return mSourceModules[0].GetValue(x, y, z)*mSourceModules[1].GetValue(x, y, z);
        }
    }
}