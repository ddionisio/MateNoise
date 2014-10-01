using UnityEngine;
using System.Collections;

namespace M8.Noise.Map {
    public class NoiseMap2D : IMap2D {
        public int width { get { return mWidth; } }
        public int height { get { return mHeight; } }

        public float this[int x, int y] {
            get {
                return mData[x, y];
            }
        }

        public void Set(int x, int y, float value) {
            mData[x, y] = value;
        }

        public void SetSize(int width, int height) {
            if(mData == null || mWidth != width || mHeight != height) {
                mData = new float[mWidth=width, mHeight=height];
            }
        }

        private float[,] mData;
        private int mWidth;
        private int mHeight;
    }
}