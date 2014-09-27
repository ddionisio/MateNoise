/*
-- Adapted from LuaJIT module; original license:
--
-- Based on code in "Simplex noise demystified", by Stefan Gustavson
-- www.itn.liu.se/~stegu/simplexnoise/simplexnoise.pdf
--
-- Thanks to Mike Pall for some cleanup and improvements (and for LuaJIT!)
--
-- Permission is hereby granted, free of charge, to any person obtaining
-- a copy of this software and associated documentation files (the
-- "Software"), to deal in the Software without restriction, including
-- without limitation the rights to use, copy, modify, merge, publish,
-- distribute, sublicense, and/or sell copies of the Software, and to
-- permit persons to whom the Software is furnished to do so, subject to
-- the following conditions:
--
-- The above copyright notice and this permission notice shall be
-- included in all copies or substantial portions of the Software.
--
-- THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
-- EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
-- MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
-- IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
-- CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
-- TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
-- SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
--
-- [ MIT license: http://www.opensource.org/licenses/mit-license.php ]
--
*/

using UnityEngine;
using System;

namespace M8.Noise {
//
public static class SimplexGenerator {

#region Static Data

	// Permutation of 0-255, replicated to allow easy indexing with sums of two bytes
	static byte[] Perms = {
		151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225,
		140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148,
		247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32,
		57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68,	175,
		74, 165, 71, 134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111,	229, 122,
		60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54,
		65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169,
		200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64,
		52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212,
		207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 170, 213,
		119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9,
		129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104,
		218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241,
		81,	51, 145, 235, 249, 14, 239,	107, 49, 192, 214, 31, 181, 199, 106, 157,
		184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93,
		222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180,

		// Duplicated
		151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225,
		140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148,
		247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32,
		57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68,	175,
		74, 165, 71, 134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111,	229, 122,
		60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54,
		65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169,
		200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64,
		52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212,
		207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 170, 213,
		119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9,
		129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104,
		218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241,
		81,	51, 145, 235, 249, 14, 239,	107, 49, 192, 214, 31, 181, 199, 106, 157,
		184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93,
		222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180,
	};

	// The above, mod 12 for each element
	static byte[] Perms12;

	// Gradients for 2D, 3D case
	static Vector3[] Grads3 = {
		new Vector3(1, 1, 0), new Vector3(-1, 1, 0), new Vector3(1, -1, 0), new Vector3(-1, -1, 0),
		new Vector3(1, 0, 1), new Vector3(-1, 0, 1), new Vector3(1, 0, -1), new Vector3(-1, 0, -1),
		new Vector3(0, 1, 1), new Vector3(0, -1, 1), new Vector3(0, 1, -1), new Vector3(0, -1, -1)
	};

    // Gradients for 4D case
	static Vector4[] Grads4 = {
		new Vector4(0, 1, 1, 1), new Vector4(0, 1, 1, -1), new Vector4(0, 1, -1, 1), new Vector4(0, 1, -1, -1),
		new Vector4(0, -1, 1, 1), new Vector4(0, -1, 1, -1), new Vector4(0, -1, -1, 1), new Vector4(0, -1, -1, -1),
		new Vector4(1, 0, 1, 1), new Vector4(1, 0, 1, -1), new Vector4(1, 0, -1, 1), new Vector4(1, 0, -1, -1),
		new Vector4(-1, 0, 1, 1), new Vector4(-1, 0, 1, -1), new Vector4(-1, 0, -1, 1), new Vector4(-1, 0, -1, -1),
		new Vector4(1, 1, 0, 1), new Vector4(1, 1, 0, -1), new Vector4(1, -1, 0, 1), new Vector4(1, -1, 0, -1),
		new Vector4(-1, 1, 0, 1), new Vector4(-1, 1, 0, -1), new Vector4(-1, -1, 0, 1), new Vector4(-1, -1, 0, -1),
		new Vector4(1, 1, 1, 0), new Vector4(1, 1, -1, 0), new Vector4(1, -1, 1, 0), new Vector4(1, -1, -1, 0),
		new Vector4(-1, 1, 1, 0), new Vector4(-1, 1, -1, 0), new Vector4(-1, -1, 1, 0), new Vector4(1, -1, -1, 0)
    };


    // A lookup table to traverse the simplex around a given point in 4D.
	// Details can be found where this table is used, in the 4D noise method.
	static int[,] SimplexData = {
		{ 0, 1, 2, 3 }, { 0, 1, 3, 2 }, { 0, 0, 0, 0 }, { 0, 2, 3, 1 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 1, 2, 3, 0 },
		{ 0, 2, 1, 3 }, { 0, 0, 0, 0 }, { 0, 3, 1, 2 }, { 0, 3, 2, 1 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 1, 3, 2, 0 },
		{ 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 },
		{ 1, 2, 0, 3 }, { 0, 0, 0, 0 }, { 1, 3, 0, 2 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 2, 3, 0, 1 }, { 2, 3, 1, 0 },
		{ 1, 0, 2, 3 }, { 1, 0, 3, 2 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 2, 0, 3, 1 }, { 0, 0, 0, 0 }, { 2, 1, 3, 0 },
		{ 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 },
		{ 2, 0, 1, 3 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 3, 0, 1, 2 }, { 3, 0, 2, 1 }, { 0, 0, 0, 0 }, { 3, 1, 2, 0 },
		{ 2, 1, 0, 3 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 3, 1, 0, 2 }, { 0, 0, 0, 0 }, { 3, 2, 0, 1 }, { 3, 2, 1, 0 }
    };

#endregion

#region Static Constructor

	//
	static SimplexGenerator ()
	{
		// Permutation values, mod 12
		Perms12 = new byte[512];

		for (int i = 0; i < 256; ++i)
		{
			byte x = (byte)(Perms[i] % 12);

			Perms12[i] = x;
			Perms12[i + 256] =  x;
		}

    	// Convert the above indices to masks that can be shifted / anded into offsets
	    for (int i = 0; i < 64; ++i)
        {
		    SimplexData[i, 0] = (1 << SimplexData[i, 0]) - 1;
		    SimplexData[i, 1] = (1 << SimplexData[i, 1]) - 1;
		    SimplexData[i, 2] = (1 << SimplexData[i, 2]) - 1;
		    SimplexData[i, 3] = (1 << SimplexData[i, 3]) - 1;
	    }
    }

#endregion

#region 2D Noise

    // 2D weight contribution
	static float GetN (int bx, int by, float x, float y)
    {
        float t = 0.5f - x * x - y * y;

        uint index = Perms12[bx + Perms[by]];

		return Math.Max(0, (t * t) * (t * t)) * (Grads3[index].x * x + Grads3[index].y * y);
    }

	//
	public static float Sample (float x, float y)
	{
        // 2D skew factors
        const float F = 0.366025403f; // (math.sqrt(3) - 1) / 2
        const float G = 0.211324865f; // (3 - math.sqrt(3)) / 6
        const float G2 = 2 * G - 1;

        // Skew the input space to determine which simplex cell we are in.
        float s = (x + y) * F;
        float ixf = Mathf.Floor(x + s);
        float iyf = Mathf.Floor(y + s);

        // Unskew the cell origin back to (x, y) space.
        float t = (ixf + iyf) * G;
        float x0 = x + t - ixf;
        float y0 = y + t - iyf;

        // Calculate the contribution from the two fixed corners.
        // A step of (1,0) in (i,j) means a step of (1-G,-G) in (x,y), and
        // A step of (0,1) in (i,j) means a step of (-G,1-G) in (x,y).
        int ix = Convert.ToInt32(ixf) & 0xFF;
        int iy = Convert.ToInt32(iyf) & 0xFF;

        float n0 = GetN(ix, iy, x0, y0);
        float n2 = GetN(ix + 1, iy + 1, x0 + G2, y0 + G2);

        /*
            Determine other corner based on simplex (equilateral triangle) we are in:
            if x0 > y0 then
                ix, x1 = ix + 1, x1 - 1
            else
                iy, y1 = iy + 1, y1 - 1
            end
        */
        int xi = x0 >= y0 ? 1 : 0;

        float n1 = GetN(ix + xi, iy + (1 - xi), x0 + G - xi, y0 + (G - 1) + xi);

        // Add contributions from each corner to get the final noise value.
        // The result is scaled to return values in the interval [-1,1].
        return 83.373f * (n0 + n1 + n2);
	}

#endregion

#region 3D Noise

    // 3D weight contribution
    static float GetN(int ix, int iy, int iz, float x, float y, float z)
    {
        float t = 0.6f - x * x - y * y - z * z;

        uint index = Perms12[ix + Perms[iy + Perms[iz]]];

        return Math.Max(0, (t * t) * (t * t)) * (Grads3[index].x * x + Grads3[index].y * y + Grads3[index].z * z);
    }

	//
	public static float Sample (float x, float y, float z)
	{
        // 3D skew factors
        const float F = 1.0f / 3;
        const float G = 1.0f / 6;
        const float G2 = 2 * G;
        const float G3 = 3 * G - 1;

		// Skew the input space to determine which simplex cell we are in.
        float s = (x + y + z) * F;
		float ixf = Mathf.Floor(x + s);
        float iyf = Mathf.Floor(y + s);
        float izf = Mathf.Floor(z + s);

		// Unskew the cell origin back to (x, y, z) space.
        float t = (ixf + iyf + izf) * G;
		float x0 = x + t - ixf;
		float y0 = y + t - iyf;
		float z0 = z + t - izf;

		// Calculate the contribution from the two fixed corners.
		// A step of (1,0,0) in (i,j,k) means a step of (1-G,-G,-G) in (x,y,z);
		// a step of (0,1,0) in (i,j,k) means a step of (-G,1-G,-G) in (x,y,z);
		// a step of (0,0,1) in (i,j,k) means a step of (-G,-G,1-G) in (x,y,z).
		int ix = Convert.ToInt32(ixf) & 0xFF;
        int iy = Convert.ToInt32(iyf) & 0xFF;
        int iz = Convert.ToInt32(izf) & 0xFF;

		float n0 = GetN(ix, iy, iz, x0, y0, z0);
		float n3 = GetN(ix + 1, iy + 1, iz + 1, x0 + G3, y0 + G3, z0 + G3);

		/*
			Determine other corners based on simplex (skewed tetrahedron) we are in:
			local ix2, iy2, iz2 = ix, iy, iz

			if x0 >= y0 then
				ix2, x2 = ix + 1, x2 - 1

				if y0 >= z0 then -- X Y Z
					ix, iy2, x1, y2 = ix + 1, iy + 1, x1 - 1, y2 - 1
				elseif x0 >= z0 then -- X Z Y
					ix, iz2, x1, z2 = ix + 1, iz + 1, x1 - 1, z2 - 1
				else -- Z X Y
					iz, iz2, z1, z2 = iz + 1, iz + 1, z1 - 1, z2 - 1
				end
			else
				iy2, y2 = iy + 1, y2 - 1

				if y0 < z0 then -- Z Y X
					iz, iz2, z1, z2 = iz + 1, iz + 1, z1 - 1, z2 - 1
				elseif x0 < z0 then -- Y Z X
					iy, iz2, y1, z2 = iy + 1, iz + 1, y1 - 1, z2 - 1
				else -- Y X Z
					iy, ix2, y1, x2 = iy + 1, ix + 1, y1 - 1, x2 - 1
				end
			end		
		*/
		int yx = x0 >= y0 ? 1 : 0;
		int zy = y0 >= z0 ? 1 : 0;
		int zx = x0 >= z0 ? 1 : 0;

		int i1 = yx & (zy | zx); // x >= y and (y >= z or x >= z)
		int j1 = (1 - yx) & zy; // x < y and y >= z
		int k1 = (1 - zy) & (1 - (yx & zx)); // y < z and not (x >= y and x >= z)

		int i2 = yx | (zy & zx); // x >= z or (y >= z and x >= z)
		int j2 = (1 - yx) | zy; // x < y or y >= z
		int k2 = yx ^ zy; // (x >= y and y < z) xor (x < y and y >= z)

		float n1 = GetN(ix + i1, iy + j1, iz + k1, x0 + G - i1, y0 + G - j1, z0 + G - k1);
		float n2 = GetN(ix + i2, iy + j2, iz + k2, x0 + G2 - i2, y0 + G2 - j2, z0 + G2 - k2);

		// Add contributions from each corner to get the final noise value.
		// The result is scaled to stay just inside [-1,1]
        return 33.542f * (n0 + n1 + n2 + n3);
	}

#endregion

#region 4D Noise

    // 4D weight contribution
	static float GetN (int ix, int iy, int iz, int iw, float x, float y, float z, float w)
    {
		float t = 0.6f - x * x - y * y - z * z - w * w;

        uint index = Perms[ix + Perms[iy + Perms[iz + Perms[iw]]]];

        index &= 0x1F;

        return Math.Max(0, (t * t) * (t * t)) * (Grads4[index].x * x + Grads4[index].y * y + Grads4[index].z * z + Grads4[index].w * w);
	}

	//
	public static float Sample (float x, float y, float z, float w)
	{
		// 4D skew factors
        const float F = 0.309016994f; //(math.sqrt(5) - 1) / 4 
        const float G = 0.138196601f; //(5 - math.sqrt(5)) / 20
        const float G2 = 2 * G;
        const float G3 = 3 * G;
        const float G4 = 4 * G - 1;

		// Skew the input space to determine which simplex cell we are in.
        float s = (x + y + z + w) * F;
        float ixf = Mathf.Floor(x + s);
        float iyf = Mathf.Floor(y + s);
        float izf = Mathf.Floor(z + s);
        float iwf = Mathf.Floor(w + s);

		// Unskew the cell origin back to (x, y, z) space.
        float t = (ixf + iyf + izf + iwf) * G;
		float x0 = x + t - ixf;
		float y0 = y + t - iyf;
		float z0 = z + t - izf;
		float w0 = w + t - iwf;

		// For the 4D case, the simplex is a 4D shape I won't even try to describe.
		// To find out which of the 24 possible simplices we're in, we need to
		// determine the magnitude ordering of x0, y0, z0 and w0.
		// The method below is a good way of finding the ordering of x,y,z,w and
		// then find the correct traversal order for the simplex we’re in.
		// First, six pair-wise comparisons are performed between each possible pair
		// of the four coordinates, and the results are used to add up binary bits
		// for an integer index.
		int c1 = x0 >= y0 ? 32 : 0;
		int c2 = x0 >= z0 ? 16 : 0;
		int c3 = y0 >= z0 ? 8 : 0;
		int c4 = x0 >= w0 ? 4 : 0;
		int c5 = y0 >= w0 ? 2 : 0;
		int c6 = z0 >= w0 ? 1 : 0;

		// Simplex[c] is a 4-vector with the numbers 0, 1, 2 and 3 in some order.
		// Many values of c will never occur, since e.g. x>y>z>w makes x<z, y<w and x<w
		// impossible. Only the 24 indices which have non-zero entries make any sense.
		// We use a thresholding to set the coordinates in turn from the largest magnitude.
		int c = c1 + c2 + c3 + c4 + c5 + c6;

		// The number 3 (i.e. bit 2) in the "simplex" array is at the position of the largest coordinate.
		int i1 = SimplexData[c, 0] >> 2;
		int j1 = SimplexData[c, 1] >> 2;
		int k1 = SimplexData[c, 2] >> 2;
		int l1 = SimplexData[c, 3] >> 2;

		// The number 2 (i.e. bit 1) in the "simplex" array is at the second largest coordinate.
		int i2 = (SimplexData[c, 0] >> 1) & 1;
		int j2 = (SimplexData[c, 1] >> 1) & 1;
		int k2 = (SimplexData[c, 2] >> 1) & 1;
		int l2 = (SimplexData[c, 3] >> 1) & 1;

		// The number 1 (i.e. bit 0) in the "simplex" array is at the second smallest coordinate.
		int i3 = SimplexData[c, 0] & 1;
		int j3 = SimplexData[c, 1] & 1;
		int k3 = SimplexData[c, 2] & 1;
		int l3 = SimplexData[c, 3] & 1;

		// Work out the hashed gradient indices of the five simplex corners
		// Sum up and scale the result to cover the range [-1,1]
        int ix = Convert.ToInt32(ixf) & 0xFF;
        int iy = Convert.ToInt32(iyf) & 0xFF;
        int iz = Convert.ToInt32(izf) & 0xFF;
        int iw = Convert.ToInt32(iwf) & 0xFF;

        float n0 = GetN(ix, iy, iz, iw, x0, y0, z0, w0);
		float n1 = GetN(ix + i1, iy + j1, iz + k1, iw + l1, x0 + G - i1, y0 + G - j1, z0 + G - k1, w0 + G - l1); // G
		float n2 = GetN(ix + i2, iy + j2, iz + k2, iw + l2, x0 + G2 - i2, y0 + G2 - j2, z0 + G2 - k2, w0 + G2 - l2); // G2
		float n3 = GetN(ix + i3, iy + j3, iz + k3, iw + l3, x0 + G3 - i3, y0 + G3 - j3, z0 + G3 - k3, w0 + G3 - l3); // G3
		float n4 = GetN(ix + 1, iy + 1, iz + 1, iw + 1, x0 + G4, y0 + G4, z0 + G4, w0 + G4); // G4

        return 40.654f * (n0 + n1 + n2 + n3 + n4);
	}

#endregion	
}
}