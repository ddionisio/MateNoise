using UnityEngine;
using System.Collections;

namespace M8.Noise {
    [System.Serializable]
    public class RollingParticle {
        public enum Area {
            Square,
            Circle
        }

        public int width = 100;
        public int height = 100;

        public Area spawnArea = Area.Square;
        public float spawnOffsetScale = 0.75f;

        public int life = 50;
        public int population = 4000;

        public bool autoNormalize = true;
        public float normalizeValue = 256.0f; //if autoNormalize is false

        public float Sample(int x, int y) {
            return mMap[x, y];
        }

        public float SampleScaled(int x, int y, int destWidth, int destHeight) {
            if(destWidth == width && destHeight == height)
                return Sample(x, y);

            float _x = x*((float)width/(float)destWidth);
            float _y = y*((float)height/(float)destHeight);

            int x0 = Mathf.FloorToInt(_x);
            int x1 = Mathf.Clamp(x0+1, 0, width-1);

            int y0 = Mathf.FloorToInt(_y);
            int y1 = Mathf.Clamp(y0+1, 0, height-1);

            float xt = _x - (float)x0;
            float yt = _y - (float)y0;

            float top = Mathf.Lerp(mMap[x0, y0], mMap[x1, y0], xt);
            float bottom = Mathf.Lerp(mMap[x0, y1], mMap[x1, y1], xt);

            return Mathf.Lerp(top, bottom, yt);
        }

        public void Generate() {
            mMap = new float[width, height];    

            float maxValue = 0f;
                        
            //start rolling
            for(int i = 0; i < population; i++) {
                int x, y;
                GetParticleStart(out x, out y);

                for(int j = 0; j < life; j++) {
                    float val = mMap[x, y] += 1.0f;
                    if(val > maxValue)
                        maxValue = val;

                    PickNeighbor(x, y, out x, out y);
                }
            }

            if(autoNormalize) {
                if(maxValue > 0f) //should be greater than 0 at this point
                    Normalize(maxValue);
            }
            else {
                if(normalizeValue > 0f)
                    Normalize(normalizeValue);
            }
        }

        public RollingParticle() { }

        public RollingParticle(int _width, int _height, Area _spawnArea, float _spawnOffsetScale, int _life, int _population) {
            width = _width;
            height = _height;
            spawnArea = _spawnArea;
            spawnOffsetScale = _spawnOffsetScale;
            life = _life;
            population = _population;
        }

        void GetParticleStart(out int x, out int y) {
            switch(spawnArea) {
                case Area.Circle:
                    float hw = width*0.5f, hh = height*0.5f;
                    Vector2 pos = new Vector2(hw, hh) + Random.insideUnitCircle*(spawnOffsetScale*Mathf.Min(hw, hh));
                    x = Mathf.RoundToInt(pos.x); if(x >= width) x = width - 1;
                    y = Mathf.RoundToInt(pos.y); if(y >= height) y = height - 1;
                    break;

                default:
                    x = Random.Range(Mathf.RoundToInt((1.0f-spawnOffsetScale)*width), Mathf.RoundToInt(spawnOffsetScale*width));
                    y = Random.Range(Mathf.RoundToInt((1.0f-spawnOffsetScale)*height), Mathf.RoundToInt(spawnOffsetScale*height));
                    break;
            }
        }
                
        void PickNeighbor(int x, int y, out int adjX, out int adjY) {
            float val = mMap[x, y];

            Dir[] mNeighborPicks = new Dir[4];
            int neighborPickCount = 0;

            //west
            if(x > 0 && mMap[x-1, y] <= val) { mNeighborPicks[neighborPickCount] = Dir.West; neighborPickCount++; }
            //east
            if(x < width-1 && mMap[x+1, y] <= val) { mNeighborPicks[neighborPickCount] = Dir.East; neighborPickCount++; }
            //north
            if(y > 0 && mMap[x, y-1] <= val) { mNeighborPicks[neighborPickCount] = Dir.North; neighborPickCount++; }
            //south
            if(y < height-1 && mMap[x, y+1] <= val) { mNeighborPicks[neighborPickCount] = Dir.South; neighborPickCount++; }

            Dir nextDir = neighborPickCount > 0 ? mNeighborPicks[Random.Range(0, neighborPickCount)] : Dir.Invalid;

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
            for(int y = 0; y < height; y++) {
                for(int x = 0; x < width; x++) {
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
    }
}