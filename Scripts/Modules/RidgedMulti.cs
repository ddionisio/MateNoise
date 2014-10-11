// Copyright (C) 2003, 2004 Jason Bevins
//
// This library is free software; you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2.1 of the License, or (at
// your option) any later version.
//
// This library is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
// License (COPYING.txt) for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this library; if not, write to the Free Software Foundation,
// Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// The developer's email is jlbezigvins@gmzigail.com (for great email, take
// off every 'zig'.)
//
using UnityEngine;

namespace M8.Noise.Module {
    /// <summary>
    /// Noise module that outputs 3-dimensional ridged-multifractal noise.
    /// 
    /// This noise module, heavily based on the Perlin-noise module, generates
    /// ridged-multifractal noise.  Ridged-multifractal noise is generated in
    /// much of the same way as Perlin noise, except the output of each octave
    /// is modified by an absolute-value function.  Modifying the octave
    /// values in this way produces ridge-like formations.
    ///
    /// Ridged-multifractal noise does not use a persistence value.  This is
    /// because the persistence values of the octaves are based on the values
    /// generated from from previous octaves, creating a feedback loop (or
    /// that's what it looks like after reading the code.)
    ///
    /// This noise module outputs ridged-multifractal-noise values that
    /// usually range from -1.0 to +1.0, but there are no guarantees that all
    /// output values will exist within that range.
    ///
    /// Note: For ridged-multifractal noise generated with only one octave,
    /// the output value ranges from -1.0 to 0.0.
    ///
    /// Ridged-multifractal noise is often used to generate craggy mountainous
    /// terrain or marble-like textures.
    ///
    /// This noise module does not require any source modules.
    ///
    /// <b>Octaves</b>
    ///
    /// The number of octaves control the <i>amount of detail</i> of the
    /// ridged-multifractal noise.  Adding more octaves increases the detail
    /// of the ridged-multifractal noise, but with the drawback of increasing
    /// the calculation time.
    ///
    /// An application may specify the number of octaves that generate
    /// ridged-multifractal noise by octaveCount field.
    ///
    /// <b>Frequency</b>
    ///
    /// An application may specify the frequency of the first octave by
    /// frequency field.
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
    /// <b>References &amp; Acknowledgments</b>
    ///
    /// <a href=http://www.texturingandmodeling.com/Musgrave.html>F.
    /// Kenton "Doc Mojo" Musgrave's texturing page</a> - This page contains
    /// links to source code that generates ridged-multfractal noise, among
    /// other types of noise.  The source file <a
    /// href=http://www.texturingandmodeling.com/CODE/MUSGRAVE/CLOUD/fractal.c> fractal.c</a> 
    /// contains the code I used in my ridged-multifractal class
    /// (see the @a RidgedMultifractal() function.)  This code was written by F.
    /// Kenton Musgrave, the person who created
    /// <a href=http://www.pandromeda.com/>MojoWorld</a>.  He is also one of
    /// the authors in <i>Texturing and Modeling: A Procedural Approach</i>
    /// (Morgan Kaufmann, 2002. ISBN 1-55860-848-6.)
    /// </summary>
    public class RidgedMulti : ModuleBase {
        public const int RIDGED_MAX_OCTAVE = 30;

        /// <summary>
        /// The frequency of the first octave.
        /// </summary>
        public float frequency = 1.0f;

        /// <summary>
        /// The lacunarity is the frequency multiplier between successive
        /// octaves.
        /// </summary>
        public float lacunarity {
            get { return mLacunarity; }
            set {
                mLacunarity = value;
                CalcSpectralWeights();
            }
        }

        /// <summary>
        /// The quality of the ridged-multifractal noise.
        /// </summary>
        public Quality quality = Quality.Cubic;

        /// <summary>
        /// The number of octaves controls the amount of detail in the ridged-multifractal noise.
        /// </summary>
        public int octaveCount {
            get { return mOctaveCount; }
            set {
                int prev = mOctaveCount;
                mOctaveCount = Mathf.Clamp(value, 1, RIDGED_MAX_OCTAVE);
                if(mOctaveCount > prev)
                    CalcSpectralWeights();
            }
        }

        public int seedOffset = 0;

        public override float GetValue(float x, float y, float z) {
            x *= frequency;
            y *= frequency;
            z *= frequency;

            float signal = 0.0f;
            float value  = 0.0f;
            float weight = 1.0f;

            // These parameters should be user-defined; they may be exposed in a
            // future version of libnoise.
            float offset = 1.0f;
            float gain = 2.0f;

            for(int curOctave = 0; curOctave < mOctaveCount; curOctave++) {

                // Get the coherent-noise value.
                int _seed = (Global.randomSeed + seedOffset + curOctave) & 0x7fffffff;
                signal = Generate.GradientCoherent3D(x, y, z, _seed, quality);

                // Make the ridges.
                signal = Mathf.Abs(signal);
                signal = offset - signal;

                // Square the signal to increase the sharpness of the ridges.
                signal *= signal;

                // The weighting from the previous octave is applied to the signal.
                // Larger values have higher weights, producing sharp points along the
                // ridges.
                signal *= weight;

                // Weight successive contributions by the previous signal.
                weight = signal * gain;
                if(weight > 1.0f) {
                    weight = 1.0f;
                }
                if(weight < 0.0f) {
                    weight = 0.0f;
                }

                // Add the signal to the output value.
                value += (signal * mSpectralWeights[curOctave]);

                // Go to the next octave.
                x *= mLacunarity;
                y *= mLacunarity;
                z *= mLacunarity;
            }

            return (value * 1.25f) - 1.0f;
        }

        public RidgedMulti()
            : base() {
            CalcSpectralWeights();
        }

        protected void CalcSpectralWeights() {
            // This exponent parameter should be user-defined; it may be exposed in a
            // future version of libnoise.
            float h = 1.0f;

            float _frequency = 1.0f;
            for(int i = 0; i < mOctaveCount; i++) {
                // Compute weight for each frequency.
                mSpectralWeights[i] = Mathf.Pow(_frequency, -h);
                _frequency *= lacunarity;
            }
        }

        protected float[] mSpectralWeights = new float[RIDGED_MAX_OCTAVE];

        private float mLacunarity = 2.0f;
        private int mOctaveCount = 6;
    }
}