using UnityEngine;

namespace M8.Noise.Module {
    /// <summary>
    /// Noise module that inverts the output value from a source module.
    /// 
    /// This noise module requires one source module.
    /// </summary>
    public class Invert : ModuleBase {
        public override int sourceModuleCount { get { return 1; } }

        public override float GetValue(float x, float y, float z) {
            return -mSourceModules[0].GetValue(x, y, z);
        }
    }
}