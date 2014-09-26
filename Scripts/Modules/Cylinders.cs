using UnityEngine;

namespace M8.Noise.Module {
    /// <summary>
    /// Noise module that outputs concentric cylinders.
    /// 
    /// This noise module outputs concentric cylinders centered on the origin.
    /// These cylinders are oriented along the @a y axis similar to the
    /// concentric rings of a tree.  Each cylinder extends infinitely along
    /// the y axis.
    ///
    /// The first cylinder has a radius of 1.0.  Each subsequent cylinder has
    /// a radius that is 1.0 unit larger than the previous cylinder.
    ///
    /// The output value from this noise module is determined by the distance
    /// between the input value and the the nearest cylinder surface.  The
    /// input values that are located on a cylinder surface are given the
    /// output value 1.0 and the input values that are equidistant from two
    /// cylinder surfaces are given the output value -1.0.
    ///
    /// An application can change the frequency of the concentric cylinders.
    /// Increasing the frequency reduces the distances between cylinders.
    ///
    /// This noise module, modified with some low-frequency, low-power
    /// turbulence, is useful for generating wood-like textures.
    ///
    /// This noise module does not require any source modules.
    /// </summary>
    public class Cylinders : ModuleBase {
        /// <summary>
        /// Increasing the frequency increases the density of the concentric
        /// cylinders, reducing the distances between them.
        /// </summary>
        public float frequency = 1.0f;

        public override float GetValue(float x, float y, float z) {
            x *= frequency;
            z *= frequency;

            float distFromCenter = Mathf.Sqrt(x*x + z*z);
            float distFromSmallerSphere = distFromCenter - Mathf.Floor(distFromCenter);
            float distFromLargerSphere = 1.0f - distFromSmallerSphere;
            float nearestDist = Mathf.Min(distFromSmallerSphere, distFromLargerSphere);
            return 1.0f - nearestDist*4.0f; // Puts it in the -1.0 to +1.0 range.
        }
    }
}