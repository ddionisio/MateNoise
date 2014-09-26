using UnityEngine;

namespace M8.Noise.Module {
    /// <summary>
    /// Noise module that outputs a checkerboard pattern.
    ///
    /// This noise module outputs unit-sized blocks of alternating values.
    /// The values of these blocks alternate between -1.0 and +1.0.
    ///
    /// This noise module is not really useful by itself, but it is often used
    /// for debugging purposes.
    ///
    /// This noise module does not require any source modules.
    /// </summary>
    public class CheckerBoard : ModuleBase {
        public override float GetValue(float x, float y, float z) {
            int ix = Mathf.FloorToInt(x);
            int iy = Mathf.FloorToInt(y);
            int iz = Mathf.FloorToInt(z);
            return (ix & 1 ^ iy & 1 ^ iz & 1) != 0 ? -1.0f : 1.0f;
        }
    }
}