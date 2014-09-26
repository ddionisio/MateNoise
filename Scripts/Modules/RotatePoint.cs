using UnityEngine;

namespace M8.Noise.Module {
    /// <summary>
    /// Noise module that rotates the input value around the origin before
    /// returning the output value from a source module.
    /// 
    /// The GetValue() method rotates the coordinates of the input value
    /// around the origin before returning the output value from the source
    /// module.  To set the rotation angles, use the rotation field.
    ///
    /// The coordinate system of the input value is assumed to be
    /// "left-handed" (x increases to the right, y increases upward,
    /// and z increases inward.)
    ///
    /// This noise module requires one source module.
    /// </summary>
    public class RotatePoint : ModuleBase {
        public override int sourceModuleCount { get { return 1; } }

        public Quaternion rotation = Quaternion.identity;

        public override float GetValue(float x, float y, float z) {
            return mSourceModules[0].GetValue(rotation*new Vector3(x, y, z));
        }
    }
}