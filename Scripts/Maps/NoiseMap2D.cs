using UnityEngine;
using System.Collections;

namespace M8.Noise.Map {
    public class NoiseMap2D : IMap2D {
        public int width { get { return mData.GetLength(0); } }
        public int height { get { return mData.GetLength(1); } }

        public float this[int x, int y] {
            get {
                return mData[x, y];
            }
        }

        public float[,] data { get { return mData; } }

        public void Set(int x, int y, float value) {
            mData[x, y] = value;
        }

        public void SetSize(int width, int height) {
            if(mData == null || this.width != width || this.height != height) {
                mData = new float[width, height];
            }
        }

        public float SampleDim(int x, int y, int dimWidth, int dimHeight, Quality quality = Quality.Cubic) {
            return Utils.SampleDim(mData, x, y, dimWidth, dimHeight, quality);
        }

        public float SampleScaled(int x, int y, float sX, float sY, Quality quality = Quality.Cubic) {
            return Utils.SampleScaled(mData, x, y, sX, sY, quality);
        }

        private float[,] mData;
    }
}