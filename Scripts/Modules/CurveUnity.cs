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
    public class CurveUnity : ModuleBase {
        public override int sourceModuleCount { get { return 1; } }

        public AnimationCurve curve { get { return mCurve; } }

        public override float GetValue(float x, float y, float z) {
            float val = mSourceModules[0].GetValue(x, y, z);

            return mCurve.Evaluate(val);
        }

        private AnimationCurve mCurve;

        public CurveUnity() : base() { mCurve = new AnimationCurve(); }

        public CurveUnity(AnimationCurve _curve) : base() { mCurve = _curve; }

        public CurveUnity(ModuleBase src, AnimationCurve _curve) : base() { mSourceModules[0] = src; mCurve = _curve; }
    }
}