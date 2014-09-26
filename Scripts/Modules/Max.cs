using UnityEngine;

namespace M8.Noise.Module {
    /// <summary>
    /// Noise module that outputs the larger of the two output values from two
    /// source modules.
    /// 
    /// This noise module requires two source modules.
    /// </summary>
    public class Max : ModuleBase {
        public override int sourceModuleCount { get { return 2; } }

        public override float GetValue(float x, float y, float z) {
            float v0 = mSourceModules[0].GetValue(x, y, z);
            float v1 = mSourceModules[1].GetValue(x, y, z);
            return Mathf.Max(v0, v1);
        }
    }
}