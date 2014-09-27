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
    /// Noise module that outputs Voronoi cells.
    /// 
    /// In mathematics, a <i>Voronoi cell</i> is a region containing all the
    /// points that are closer to a specific <i>seed point</i> than to any
    /// other seed point.  These cells mesh with one another, producing
    /// polygon-like formations.
    ///
    /// By default, this noise module randomly places a seed point within
    /// each unit cube.  By modifying the <i>frequency</i> of the seed points,
    /// an application can change the distance between seed points.  The
    /// higher the frequency, the closer together this noise module places
    /// the seed points, which reduces the size of the cells.  To specify the
    /// frequency of the cells, call the SetFrequency() method.
    ///
    /// This noise module assigns each Voronoi cell with a random constant
    /// value from a coherent-noise function.  The <i>displacement value</i>
    /// controls the range of random values to assign to each cell.  The
    /// range of random values is +/- the displacement value.  Call the
    /// SetDisplacement() method to specify the displacement value.
    ///
    /// To modify the random positions of the seed points, call the SetSeed()
    /// method.
    ///
    /// This noise module can optionally add the distance from the nearest
    /// seed to the output value.  To enable this feature, call the
    /// EnableDistance() method.  This causes the points in the Voronoi cells
    /// to increase in value the further away that point is from the nearest
    /// seed point.
    ///
    /// Voronoi cells are often used to generate cracked-mud terrain
    /// formations or crystal-like textures
    ///
    /// This noise module requires no source modules.
    /// </summary>
    public class Voronoi : ModuleBase {
        /// <summary>
        /// Applying the distance from the nearest seed point to the output
        /// value causes the points in the Voronoi cells to increase in value
        /// the further away that point is from the nearest seed point.
        /// Setting this value to true (and setting the displacement to a
        /// near-zero value) causes this noise module to generate cracked mud
        /// formations.
        /// </summary>
        public bool enableDistance = false;

        /// <summary>
        /// This noise module assigns each Voronoi cell with a random constant
        /// value from a coherent-noise function.  The <i>displacement
        /// value</i> controls the range of random values to assign to each
        /// cell.  The range of random values is +/- the displacement value.
        /// </summary>
        public float displacement = 1.0f;

        /// <summary>
        /// The frequency determines the size of the Voronoi cells and the
        /// distance between these cells.
        /// </summary>
        public float frequency = 1.0f;

        /// <summary>
        /// The positions of the seed values are calculated by a
        /// coherent-noise function.  By modifying the seed value, the output
        /// of that function changes.
        /// </summary>
        public int seed = 0;

        public override float GetValue(float x, float y, float z) {
            // This method could be more efficient by caching the seed values.  Fix
            // later.

            x *= frequency;
            y *= frequency;
            z *= frequency;

            int xInt = Mathf.FloorToInt(x);
            int yInt = Mathf.FloorToInt(y);
            int zInt = Mathf.FloorToInt(z);

            float minDist = float.MaxValue;
            float xCandidate = 0f;
            float yCandidate = 0f;
            float zCandidate = 0f;

            // Inside each unit cube, there is a seed point at a random position.  Go
            // through each of the nearby cubes until we find a cube with a seed point
            // that is closest to the specified position.
            for(int zCur = zInt - 2; zCur <= zInt + 2; zCur++) {
                for(int yCur = yInt - 2; yCur <= yInt + 2; yCur++) {
                    for(int xCur = xInt - 2; xCur <= xInt + 2; xCur++) {

                        // Calculate the position and distance to the seed point inside of
                        // this unit cube.
                        float xPos = xCur + Generate.Value3D(xCur, yCur, zCur, seed);
                        float yPos = yCur + Generate.Value3D(xCur, yCur, zCur, seed + 1);
                        float zPos = zCur + Generate.Value3D(xCur, yCur, zCur, seed + 2);
                        float xDist = xPos - x;
                        float yDist = yPos - y;
                        float zDist = zPos - z;
                        float dist = xDist * xDist + yDist * yDist + zDist * zDist;

                        if(dist < minDist) {
                            // This seed point is closer to any others found so far, so record
                            // this seed point.
                            minDist = dist;
                            xCandidate = xPos;
                            yCandidate = yPos;
                            zCandidate = zPos;
                        }
                    }
                }
            }

            float value;
            if(enableDistance) {
                // Determine the distance to the nearest seed point.
                float xDist = xCandidate - x;
                float yDist = yCandidate - y;
                float zDist = zCandidate - z;

                value = (Mathf.Sqrt(xDist * xDist + yDist * yDist + zDist * zDist)
                  ) * Utils.SQRT_3 - 1.0f;
            }
            else {
                value = 0.0f;
            }

            // Return the calculated distance with the displacement value applied.
            return value + displacement*Generate.Value3D(
              (int)(Mathf.FloorToInt(xCandidate)),
              (int)(Mathf.FloorToInt(yCandidate)),
              (int)(Mathf.FloorToInt(zCandidate)));
        }

        public Voronoi(int _seed = 0, float _displacement = 1.0f, float _frequency = 1.0f, bool _enableDistance = false)
            : base() {
                seed = _seed;
                displacement = _displacement;
                frequency = _frequency;
                enableDistance = _enableDistance;
        }
    }
}