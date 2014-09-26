
namespace M8.Noise.Module {
    /// <summary>
    /// Noise module that outputs a constant value.
    /// 
    /// This noise module is not useful by itself, but it is often used as a
    /// source module for other noise modules.
    ///
    /// This noise module does not require any source modules.
    /// 
    /// Set value via property 'val'
    /// </summary>
    public class Const : ModuleBase {
        public float val { get { return mVal; } set { mVal = value; } }

        public override float GetValue(float x, float y, float z) {
            return mVal;
        }

        private float mVal = 0.0f;
    }
}