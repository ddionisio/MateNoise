using UnityEngine;

namespace M8.Noise.Module {
    /// <summary>
    /// Noise module that scales the coordinates of the input value before
    /// returning the output value from a source module.
    /// 
    /// The GetValue() method multiplies the (x, y, z) coordinates
    /// of the input value with a scaling factor before returning the output
    /// value from the source module.  To set the scaling factor, call the
    /// SetScale() method.  To set the scaling factor to apply to the
    /// individual x, y, or z coordinates, by setting scale field.
    ///
    /// This noise module requires one source module.
    /// </summary>
    public class ScalePoint : ModuleBase {
        public override int sourceModuleCount { get { return 1; } }

        public Vector3 scale = Vector3.one;

        public override float GetValue(float x, float y, float z) {
            return mSourceModules[0].GetValue(x*scale.x, y*scale.y, z*scale.z);
        }
    }
}