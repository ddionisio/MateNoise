using UnityEngine;

namespace M8.Noise.Module {
    public class Simplex : ModuleBase {
        public override float GetValue(float x, float y, float z) {
            return SimplexGenerator.Sample(x, y, z);
        }
    }
}