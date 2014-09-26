using UnityEngine;

namespace M8.Noise.Module {
    /// <summary>
    /// Noise module that outputs 3-dimensional Perlin noise.
    /// 
    /// Perlin noise is the sum of several coherent-noise functions of
    /// ever-increasing frequencies and ever-decreasing amplitudes.
    ///
    /// An important property of Perlin noise is that a small change in the
    /// input value will produce a small change in the output value, while a
    /// large change in the input value will produce a random change in the
    /// output value.
    ///
    /// This noise module outputs Perlin-noise values that usually range from
    /// -1.0 to +1.0, but there are no guarantees that all output values will
    /// exist within that range.
    ///
    /// For a better description of Perlin noise, see the links in the
    /// <i>References and Acknowledgments</i> section.
    ///
    /// This noise module does not require any source modules.
    ///
    /// <b>Octaves</b>
    ///
    /// The number of octaves control the <i>amount of detail</i> of the
    /// Perlin noise.  Adding more octaves increases the detail of the Perlin
    /// noise, but with the drawback of increasing the calculation time.
    ///
    /// An octave is one of the coherent-noise functions in a series of
    /// coherent-noise functions that are added together to form Perlin
    /// noise.
    ///
    /// An application may specify the frequency of the first octave by
    /// calling the SetFrequency() method.
    ///
    /// An application may specify the number of octaves that generate Perlin
    /// noise by calling the SetOctaveCount() method.
    ///
    /// These coherent-noise functions are called octaves because each octave
    /// has, by default, double the frequency of the previous octave.  Musical
    /// tones have this property as well; a musical C tone that is one octave
    /// higher than the previous C tone has double its frequency.
    ///
    /// <b>Frequency</b>
    ///
    /// An application may specify the frequency of the first octave by
    /// calling the SetFrequency() method.
    ///
    /// <b>Persistence</b>
    ///
    /// The persistence value controls the <i>roughness</i> of the Perlin
    /// noise.  Larger values produce rougher noise.
    ///
    /// The persistence value determines how quickly the amplitudes diminish
    /// for successive octaves.  The amplitude of the first octave is 1.0.
    /// The amplitude of each subsequent octave is equal to the product of the
    /// previous octave's amplitude and the persistence value.  So a
    /// persistence value of 0.5 sets the amplitude of the first octave to
    /// 1.0; the second, 0.5; the third, 0.25; etc.
    ///
    /// An application may specify the persistence value by calling the
    /// SetPersistence() method.
    ///
    /// <b>Lacunarity</b>
    ///
    /// The lacunarity specifies the frequency multipler between successive
    /// octaves.
    ///
    /// The effect of modifying the lacunarity is subtle; you may need to play
    /// with the lacunarity value to determine the effects.  For best results,
    /// set the lacunarity to a number between 1.5 and 3.5.
    ///
    /// <b>References &amp; acknowledgments</b>
    ///
    /// <a href=http://www.noisemachine.com/talk1/>The Noise Machine</a> -
    /// From the master, Ken Perlin himself.  This page contains a
    /// presentation that describes Perlin noise and some of its variants.
    /// He won an Oscar for creating the Perlin noise algorithm!
    ///
    /// <a
    /// href=http://freespace.virgin.net/hugo.elias/models/m_perlin.htm>
    /// Perlin Noise</a> - Hugo Elias's webpage contains a very good
    /// description of Perlin noise and describes its many applications.  This
    /// page gave me the inspiration to create libnoise in the first place.
    /// Now that I know how to generate Perlin noise, I will never again use
    /// cheesy subdivision algorithms to create terrain (unless I absolutely
    /// need the speed.)
    ///
    /// <a
    /// href=http://www.robo-murito.net/code/perlin-noise-math-faq.html>
    /// The Perlin noise math FAQ</a> - A good page that describes Perlin noise in
    /// plain English with only a minor amount of math.  During development of
    /// libnoise, I noticed that my coherent-noise function generated terrain
    /// with some "regularity" to the terrain features.  This page describes a
    /// better coherent-noise function called <i>gradient noise</i>.  This
    /// version of noise::module::Perlin uses gradient coherent noise to
    /// generate Perlin noise.
    /// </summary>
    public class Perlin : ModuleBase {
        public const int PERLIN_MAX_OCTAVE = 30;

        /// <summary>
        /// the frequency of the first octave.
        /// </summary>
        public float frequency = 1.0f;

        /// <summary>
        /// The lacunarity is the frequency multiplier between successive
        /// octaves.
        /// </summary>
        public float lacunarity = 2.0f;

        public Quality quality = Quality.Cubic;

        /// <summary>
        /// The number of octaves controls the amount of detail in the Perlin
        /// noise.
        /// </summary>
        public int octaveCount { get { return mOctaveCount; } set { mOctaveCount = Mathf.Clamp(value, 1, PERLIN_MAX_OCTAVE); } }

        /// <summary>
        /// The persistence value controls the roughness of the Perlin noise.
        /// </summary>
        public float persistence = 0.5f;

        /// <summary>
        /// The seed value used by the Perlin-noise function.
        /// </summary>
        public int seed = 0;

        public override float GetValue(float x, float y, float z) {
            float value = 0.0f;
            float signal = 0.0f;
            float curPersistence = 1.0f;
            long _seed;

            x *= frequency;
            y *= frequency;
            z *= frequency;

            for(int curOctave = 0; curOctave < mOctaveCount; curOctave++) {

                // Get the coherent-noise value from the input value and add it to the
                // final result.
                _seed = (seed + curOctave) & 0xffffffff;
                signal = Generate.GradientCoherent3D(x, y, z, (int)_seed, quality);
                value += signal * curPersistence;

                // Prepare the next octave.
                x *= lacunarity;
                y *= lacunarity;
                z *= lacunarity;
                curPersistence *= persistence;
            }

            return value;
        }

        private int mOctaveCount = 6;
    }
}