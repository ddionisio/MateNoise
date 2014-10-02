using UnityEngine;
using System.Collections;

namespace M8.Noise {
    public class RollingParticle {
        public int startXBias;
        public int startYBias;
        public int life;
        public int population;

        public int width { get { return mWidth; } }
        public int height { get { return mHeight; } }

        public float Sample(int x, int y) {
            return mMap[x, y];
        }

        public float SampleScaled(int x, int y, int destWidth, int destHeight) {
            if(destWidth == mWidth && destHeight == mHeight)
                return Sample(x, y);

            float _x = x*((float)mWidth/(float)destWidth);
            float _y = y*((float)mHeight/(float)destHeight);

            int x0 = Mathf.FloorToInt(_x);
            int x1 = Mathf.Clamp(x0+1, 0, mWidth-1);

            int y0 = Mathf.FloorToInt(_y);
            int y1 = Mathf.Clamp(y0+1, 0, mHeight-1);

            float xt = _x - (float)x0;
            float yt = _y - (float)y0;

            float top = Mathf.Lerp(mMap[x0, y0], mMap[x1, y0], xt);
            float bottom = Mathf.Lerp(mMap[x0, y1], mMap[x1, y1], xt);

            return Mathf.Lerp(top, bottom, yt);
        }

        public void SetSize(int width, int height) {
            mWidth = width;
            mHeight = height;
        }

        public void Generate() {
            mMap = new float[mWidth, mHeight];    

            float maxValue = 0f;

            //start rolling
            for(int i = 0; i < population; i++) {
                int x = Random.Range(startXBias, mWidth-startXBias);
                int y = Random.Range(startYBias, mHeight-startYBias);

                for(int j = 0; j < life; j++) {
                    float val = mMap[x, y] += 1.0f;
                    if(val > maxValue)
                        maxValue = val;

                    PickNeighbor(x, y, out x, out y);
                }
            }

            if(maxValue > 0f) //should be greater than 0 at this point
                Normalize(maxValue);
        }

        public RollingParticle(int width, int height, int _startXBias, int _startYBias, int _life, int _population) {
            SetSize(width, height);

            startXBias = _startXBias;
            startYBias = _startYBias;
            life = _life;
            population = _population;
        }
                
        void PickNeighbor(int x, int y, out int adjX, out int adjY) {
            float val = mMap[x, y];

            mNeighborPickCount = 0;

            //west
            if(x > 0 && mMap[x-1, y] <= val) { mNeighborPicks[mNeighborPickCount] = Dir.West; mNeighborPickCount++; }
            //east
            if(x < mWidth-1 && mMap[x+1, y] <= val) { mNeighborPicks[mNeighborPickCount] = Dir.East; mNeighborPickCount++; }
            //north
            if(y > 0 && mMap[x, y-1] <= val) { mNeighborPicks[mNeighborPickCount] = Dir.North; mNeighborPickCount++; }
            //south
            if(y < mHeight-1 && mMap[x, y+1] <= val) { mNeighborPicks[mNeighborPickCount] = Dir.South; mNeighborPickCount++; }

            Dir nextDir = mNeighborPickCount > 0 ? mNeighborPicks[Random.Range(0, mNeighborPickCount)] : Dir.Invalid;

            switch(nextDir) {
                case Dir.North:
                    adjX = x; adjY = y-1;
                    break;
                case Dir.South:
                    adjX = x; adjY = y+1;
                    break;
                case Dir.East:
                    adjX = x+1; adjY = y;
                    break;
                case Dir.West:
                    adjX = x-1; adjY = y;
                    break;
                default: //shouldn't happen
                    adjX = x; adjY = y;
                    break;
            }
        }
        
        void Normalize(float maxValue) {
            for(int y = 0; y < mHeight; y++) {
                for(int x = 0; x < mWidth; x++) {
                    mMap[x, y] /= maxValue;
                }
            }
        }

        private enum Dir {
            North,
            South,
            East,
            West,

            Invalid
        }

        private float[,] mMap;

        private int mWidth;
        private int mHeight;

        private Dir[] mNeighborPicks = new Dir[4];
        private int mNeighborPickCount;
    }
}