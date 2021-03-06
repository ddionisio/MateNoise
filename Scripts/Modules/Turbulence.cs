﻿// Copyright (C) 2003, 2004 Jason Bevins
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
namespace M8.Noise.Module {
    /// <summary>
    /// Noise module that randomly displaces the input value before
    /// returning the output value from a source module.
    /// 
    /// Turbulence is the pseudo-random displacement of the input value.
    /// The GetValue() method randomly displaces the (x, y, z)
    /// coordinates of the input value before retrieving the output value from
    /// the source module.  To control the turbulence, an application can
    /// modify its frequency, its power, and its roughness.
    ///
    /// The frequency of the turbulence determines how rapidly the
    /// displacement amount changes.
    ///
    /// The power of the turbulence determines the scaling factor that is
    /// applied to the displacement amount.
    ///
    /// The roughness of the turbulence determines the roughness of the
    /// changes to the displacement amount.  Low values smoothly change the
    /// displacement amount.  High values roughly change the displacement
    /// amount, which produces more "kinky" changes.
    ///
    /// Use of this noise module may require some trial and error.  Assuming
    /// that you are using a generator module as the source module, you
    /// should first:
    /// - Set the frequency to the same frequency as the source module.
    /// - Set the power to the reciprocal of the frequency.
    ///
    /// From these initial frequency and power values, modify these values
    /// until this noise module produce the desired changes in your terrain or
    /// texture.  For example:
    /// - Low frequency (1/8 initial frequency) and low power (1/8 initial
    ///   power) produces very minor, almost unnoticeable changes.
    /// - Low frequency (1/8 initial frequency) and high power (8 times
    ///   initial power) produces "ropey" lava-like terrain or marble-like
    ///   textures.
    /// - High frequency (8 times initial frequency) and low power (1/8
    ///   initial power) produces a noisy version of the initial terrain or
    ///   texture.
    /// - High frequency (8 times initial frequency) and high power (8 times
    ///   initial power) produces nearly pure noise, which isn't entirely
    ///   useful.
    ///
    /// Displacing the input values result in more realistic terrain and
    /// textures.  If you are generating elevations for terrain height maps,
    /// you can use this noise module to produce more realistic mountain
    /// ranges or terrain features that look like flowing lava rock.  If you
    /// are generating values for textures, you can use this noise module to
    /// produce realistic marble-like or "oily" textures.
    ///
    /// Internally, there are three noise::module::Perlin noise modules
    /// that displace the input value; one for the x, one for the y,
    /// and one for the z coordinate.
    ///
    /// This noise module requires one source module.
    /// </summary>
    public class Turbulence : ModuleBase {
        public override int sourceModuleCount { get { return 1; } }

        /// <summary>
        /// The power of the turbulence determines the scaling factor that is
        /// applied to the displacement amount.
        /// </summary>
        public float power = 1.0f;

        /// <summary>
        /// The roughness of the turbulence determines the roughness of the
        /// changes to the displacement amount.  Low values smoothly change
        /// the displacement amount.  High values roughly change the
        /// displacement amount, which produces more "kinky" changes.
        /// </summary>
        public int roughnessCount {
            get { return mXDistortModule.octaveCount; }
            set {
                mXDistortModule.octaveCount = value;
                mYDistortModule.octaveCount = value;
                mZDistortModule.octaveCount = value;
            }
        }

        /// <summary>
        /// The frequency of the turbulence determines how rapidly the
        /// displacement amount changes.
        /// </summary>
        public float frequency {
            get { return mXDistortModule.frequency; }
            set {
                mXDistortModule.frequency = value;
                mYDistortModule.frequency = value;
                mZDistortModule.frequency = value;
            }
        }

        public override float GetValue(float x, float y, float z) {
            // Get the values from the three noise::module::Perlin noise modules and
            // add each value to each coordinate of the input value.  There are also
            // some offsets added to the coordinates of the input values.  This prevents
            // the distortion modules from returning zero if the (x, y, z) coordinates,
            // when multiplied by the frequency, are near an integer boundary.  This is
            // due to a property of gradient coherent noise, which returns zero at
            // integer boundaries.
            float x0, y0, z0;
            float x1, y1, z1;
            float x2, y2, z2;
            x0 = x + (12414.0f / 65536.0f);
            y0 = y + (65124.0f / 65536.0f);
            z0 = z + (31337.0f / 65536.0f);
            x1 = x + (26519.0f / 65536.0f);
            y1 = y + (18128.0f / 65536.0f);
            z1 = z + (60493.0f / 65536.0f);
            x2 = x + (53820.0f / 65536.0f);
            y2 = y + (11213.0f / 65536.0f);
            z2 = z + (44845.0f / 65536.0f);
            float xDistort = x + mXDistortModule.GetValue(x0, y0, z0)*power;
            float yDistort = y + mYDistortModule.GetValue(x1, y1, z1)*power;
            float zDistort = z + mZDistortModule.GetValue(x2, y2, z2)*power;

            // Retrieve the output value at the offsetted input value instead of the
            // original input value.
            return mSourceModules[0].GetValue(xDistort, yDistort, zDistort);
        }

        public Turbulence() : base() {
            mXDistortModule = new Perlin();
            mYDistortModule = new Perlin(); mYDistortModule.seedOffset = 1;
            mZDistortModule = new Perlin(); mZDistortModule.seedOffset = 2;
        }

        public Turbulence(ModuleBase src) : base() { mSourceModules[0] = src; }

        protected Perlin mXDistortModule;
        protected Perlin mYDistortModule;
        protected Perlin mZDistortModule;
    }
}