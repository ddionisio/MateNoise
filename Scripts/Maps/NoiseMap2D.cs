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

        public float SampleScaled(int x, int y, int destWidth, int destHeight, Quality quality = Quality.Cubic) {
            if(destWidth == width && destHeight == height)
                return mData[x, y];

            float _x = x*((float)width/(float)destWidth);
            float _y = y*((float)height/(float)destHeight);

            int x0 = Mathf.FloorToInt(_x);
            int x1 = Mathf.Clamp(x0+1, 0, width-1);

            int y0 = Mathf.FloorToInt(_y);
            int y1 = Mathf.Clamp(y0+1, 0, height-1);

            float xt=0f, yt=0f;
            switch(quality) {
                case Quality.Linear:
                    xt = _x - (float)x0;
                    yt = _y - (float)y0;
                    break;
                case Quality.Cosine:
                    xt = Interpolate.CurveCos(_x - (float)x0);
                    yt = Interpolate.CurveCos(_y - (float)y0);
                    break;
                case Quality.Cubic:
                    xt = Interpolate.CurveCubic(_x - (float)x0);
                    yt = Interpolate.CurveCubic(_y - (float)y0);
                    break;
                case Quality.Quint:
                    xt = Interpolate.CurveQuint(_x - (float)x0);
                    yt = Interpolate.CurveQuint(_y - (float)y0);
                    break;
            }
                        
            float top = Mathf.Lerp(mData[x0, y0], mData[x1, y0], xt);
            float bottom = Mathf.Lerp(mData[x0, y1], mData[x1, y1], xt);

            return Mathf.Lerp(top, bottom, yt);
        }

        private float[,] mData;
    }
}