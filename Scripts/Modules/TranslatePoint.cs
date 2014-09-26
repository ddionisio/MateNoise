using UnityEngine;

namespace M8.Noise.Module {
    /// <summary>
    /// Noise module that moves the coordinates of the input value before
    /// returning the output value from a source module.
    /// 
    /// The GetValue() method moves the (x, y, z) coordinates of
    /// the input value by a translation amount before returning the output
    /// value from the source module.  To set the translation amount, call
    /// the SetTranslation() method.  To set the translation amount to
    /// apply to the individual x, y, or z coordinates, set the
    /// translate property.
    ///
    /// This noise module requires one source module.
    /// </summary>
    public class TranslatePoint : ModuleBase {
        public override int sourceModuleCount { get { return 1; } }

        public Vector3 translate = Vector3.zero;

        public override float GetValue(float x, float y, float z) {
            return mSourceModules[0].GetValue(x+translate.x, y+translate.y, z+translate.z);
        }
    }
}