using UnityEngine;

namespace M8.Noise.Module {
    /// <summary>
    /// Noise module that outputs concentric spheres.
    /// 
    /// This noise module outputs concentric spheres centered on the origin
    /// like the concentric rings of an onion.
    ///
    /// The first sphere has a radius of 1.0.  Each subsequent sphere has a
    /// radius that is 1.0 unit larger than the previous sphere.
    ///
    /// The output value from this noise module is determined by the distance
    /// between the input value and the the nearest spherical surface.  The
    /// input values that are located on a spherical surface are given the
    /// output value 1.0 and the input values that are equidistant from two
    /// spherical surfaces are given the output value -1.0.
    ///
    /// An application can change the frequency of the concentric spheres.
    /// Increasing the frequency reduces the distances between spheres.  To
    /// specify the frequency, call the SetFrequency() method.
    ///
    /// This noise module, modified with some low-frequency, low-power
    /// turbulence, is useful for generating agate-like textures.
    ///
    /// This noise module does not require any source modules.  
    /// </summary>
    public class Spheres : ModuleBase {

        /// <summary>
        /// Increasing the frequency increases the density of the concentric
        /// spheres, reducing the distances between them.
        /// </summary>
        public float frequency = 1.0f;

        public override float GetValue(float x, float y, float z) {
            x *= frequency;
            y *= frequency;
            z *= frequency;

            float distFromCenter = Mathf.Sqrt(x*x + y*y + z*z);
            float distFromSmallerSphere = distFromCenter - Mathf.Floor(distFromCenter);
            float distFromLargerSphere = 1.0f - distFromSmallerSphere;
            float nearestDist = Mathf.Min(distFromSmallerSphere, distFromLargerSphere);
            return 1.0f - nearestDist*4.0f; // Puts it in the -1.0 to +1.0 range.
        }
    }
}