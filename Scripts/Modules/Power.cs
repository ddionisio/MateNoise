using UnityEngine;

namespace M8.Noise.Module {
    /// <summary>
    /// Noise module that raises the output value from a first source module
    /// to the power of the output value from a second source module.
    /// 
    /// The first source module must have an index value of 0.
    ///
    /// The second source module must have an index value of 1.
    ///
    /// This noise module requires two source modules.
    /// </summary>
    public class Power : ModuleBase {
        public override int sourceModuleCount { get { return 2; } }

        public override float GetValue(float x, float y, float z) {
            return Mathf.Pow(mSourceModules[0].GetValue(x, y, z), mSourceModules[1].GetValue(x, y, z));
        }
    }
}