using UnityEngine;

namespace M8.Noise.Module {
    /// <summary>
    /// Noise module that outputs the absolute value of the output value from
    /// a source module.
    /// </summary>
    public class Abs : ModuleBase {
        public override int sourceModuleCount { get { return 1; } }

        public override float GetValue(float x, float y, float z) {
            return Mathf.Abs(mSourceModules[0].GetValue(x, y, z));
        }
    }
}