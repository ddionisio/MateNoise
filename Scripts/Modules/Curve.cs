using UnityEngine;

namespace M8.Noise.Module {
    /// <summary>
    /// Noise module that maps the output value from a source module onto an
    /// arbitrary function curve.
    /// 
    /// This noise module maps the output value from the source module onto an
    /// application-defined curve.  This curve is defined by Unity's curve system.
    /// 
    /// Value is evaluated into the animation curve.
    /// 
    /// Ensure the control points lie within the range [-1, 1].
    /// 
    /// This noise module requires one source module.
    /// </summary>
    public class Curve : ModuleBase {
        public override int sourceModuleCount { get { return 1; } }

        public AnimationCurve curve { get { return mCurve; } set { mCurve = value; } }

        public bool normalizeSourceValue; //if true, value is converted from [-1, 1] to [0, 1]
        
        public override float GetValue(float x, float y, float z) {
            float val = mSourceModules[0].GetValue(x, y, z);

            if(normalizeSourceValue)
                val = (val+1.0f)*0.5f;

            return mCurve.Evaluate(val);
        }

        private AnimationCurve mCurve = new AnimationCurve();
    }
}