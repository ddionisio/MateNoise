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
        public bool spawnOutside = false;
        public float spawnAreaScale = 0.75f;
        public float spawnAreaOuterScale = 0.1f;

        public int life = 50;
        public int population = 4000;

        public float valueInc = 1.0f;
        public float valueMax = 256.0f;
        public bool valueAutoMax = false; //if this is true, valueCapToMax is forced to false, valueMax is set to highest value after build
        public bool valueCapToMax = true;
        
        public float[,] Generate() {
            if(valueAutoMax)
                valueCapToMax = false;

            float[,] map = new float[width, height];
    
            //fill inner area if spawning outside
            if(spawnOutside) {
                float hw = width*0.5f, hh = height*0.5f;
                int minX = Mathf.RoundToInt(hw - spawnAreaScale*hw), maxX = Mathf.RoundToInt(hw + spawnAreaScale*width); if(maxX >= width) maxX = width - 1;
                int minY = Mathf.RoundToInt(hh - spawnAreaScale*hh), maxY = Mathf.RoundToInt(hh + spawnAreaScale*height); if(maxY >= height) maxY = height - 1;

                switch(spawnArea) {
                    case Area.Circle:
                        float size = Mathf.Min(hw, hh);
                        float r = spawnAreaScale*size;
                        Vector2 center = new Vector2(hw, hh);
                        for(int y = minY; y <= maxY; y++) {
                            for(int x = minX; x <= maxX; x++) {
                                Vector2 d = new Vector2(x - center.x, y - center.y);
                                if(d.sqrMagnitude <= r*r)
                                    map[x, y] = valueMax;
                            }
                        }
                        break;
                    case Area.Square:
                        for(int y = minY; y <= maxY; y++) {
                            for(int x = minX; x <= maxX; x++) {
                                map[x, y] = valueMax;
                            }
                        }
                        break;
                }
            }
            
            //start rolling
            for(int i = 0; i < population; i++) {
                int x, y;
                GetParticleStart(out x, out y);

                for(int j = 0; j < life; j++) {
                    float val = map[x, y];
                    if(!valueCapToMax || val < valueMax) {
                        val += valueInc;
                        if(valueCapToMax && val > valueMax)
                            val = valueMax;

                        map[x, y] = val;

                        if(valueAutoMax && val > valueMax)
                            valueMax = val;
                    }

                    PickNeighbor(map, x, y, out x, out y);
                }
            }

            Normalize(map, valueMax);

            return map;
        }

        public RollingParticle() { }

        public RollingParticle(int _width, int _height, Area _spawnArea, float _spawnOffsetScale, int _life, int _population) {
            width = _width;
            height = _height;
            spawnArea = _spawnArea;
            spawnAreaScale = _spawnOffsetScale;
            life = _life;
            population = _population;
        }

        void GetParticleStart(out int x, out int y) {
            switch(spawnArea) {
                case Area.Circle:
                    if(spawnOutside) {
                        float hw = width*0.5f, hh = height*0.5f;
                        float size = Mathf.Min(hw, hh);
                        float r = spawnAreaScale*size;
                        float rOuter = spawnAreaOuterScale*size;
                        Vector2 dir = new Vector2(M8.Noise.Generate.valueUnit, M8.Noise.Generate.valueUnit); dir.Normalize();
                        Vector2 pos = new Vector2(hw, hh) + dir*(r + M8.Noise.Generate.Range(0, rOuter));
                        x = Mathf.RoundToInt(pos.x); if(x >= width) x = width - 1;
                        y = Mathf.RoundToInt(pos.y); if(y >= height) y = height - 1;
                    }
                    else {
                        float hw = width*0.5f, hh = height*0.5f;
                        Vector2 pos = new Vector2(hw, hh) + new Vector2(M8.Noise.Generate.valueUnit, M8.Noise.Generate.valueUnit)*(spawnAreaScale*Mathf.Min(hw, hh));
                        x = Mathf.RoundToInt(pos.x); if(x >= width) x = width - 1;
                        y = Mathf.RoundToInt(pos.y); if(y >= height) y = height - 1;
                    }
                    break;

                default:
                    if(spawnOutside) {
                        float hw = width*0.5f, hh = height*0.5f;

                        int minX = Mathf.RoundToInt(hw - spawnAreaScale*hw), maxX = Mathf.RoundToInt(hw + spawnAreaScale*width); if(maxX >= width) maxX = width - 1;
                        int minY = Mathf.RoundToInt(hh - spawnAreaScale*hh), maxY = Mathf.RoundToInt(hh + spawnAreaScale*height); if(maxY >= height) maxY = height - 1;

                        float sumScale = spawnAreaScale+spawnAreaOuterScale;
                        int minX2 = Mathf.RoundToInt(hw - sumScale*hw), maxX2 = Mathf.RoundToInt(hw + sumScale*width); if(maxX2 >= width) maxX2 = width - 1;
                        int minY2 = Mathf.RoundToInt(hh - sumScale*hh), maxY2 = Mathf.RoundToInt(hh + sumScale*height); if(maxY2 >= height) maxY2 = height - 1;

                        x = M8.Noise.Generate.Range(0, 2) == 1 ? M8.Noise.Generate.Range(minX2, minX+1) : M8.Noise.Generate.Range(maxX, maxX2+1);
                        y = M8.Noise.Generate.Range(0, 2) == 1 ? M8.Noise.Generate.Range(minY2, minY+1) : M8.Noise.Generate.Range(maxY, maxY2+1);
                    }
                    else {
                        float hw = width*0.5f, hh = height*0.5f;
                        int minX = Mathf.RoundToInt(hw - spawnAreaScale*hw), maxX = Mathf.RoundToInt(hw + spawnAreaScale*width); if(maxX >= width) maxX = width - 1;
                        int minY = Mathf.RoundToInt(hh - spawnAreaScale*hh), maxY = Mathf.RoundToInt(hh + spawnAreaScale*height); if(maxY >= height) maxY = height - 1;

                        x = M8.Noise.Generate.Range(minX, maxX + 1);
                        y = M8.Noise.Generate.Range(minY, maxY + 1);
                    }
                    break;
            }
        }

        bool IsValid(float[,] map, int x, int y, float val) {
            if(spawnOutside) {
                float hw = width*0.5f, hh = height*0.5f;

                switch(spawnArea) {
                    case Area.Circle:
                        float r = spawnAreaScale*Mathf.Min(hw, hh);
                        Vector2 delta = new Vector2(x - hw, y - hh);
                        if(delta.sqrMagnitude < r*r)
                            return false;
                        break;
                    case Area.Square:
                        int minX = Mathf.RoundToInt(hw - spawnAreaScale*hw), maxX = Mathf.RoundToInt(hw + spawnAreaScale*width); if(maxX >= width) maxX = width - 1;
                        int minY = Mathf.RoundToInt(hh - spawnAreaScale*hh), maxY = Mathf.RoundToInt(hh + spawnAreaScale*height); if(maxY >= height) maxY = height - 1;

                        if(x >= minX && x <= maxX && y >= minY && y <= maxY)
                            return false;
                        break;
                }
            }

            return map[x, y] <= val && (!valueCapToMax || val < valueMax);
        }

        void PickNeighbor(float[,] map, int x, int y, out int adjX, out int adjY) {
            float val = map[x, y];

            Dir[] mNeighborPicks = new Dir[4];
            int neighborPickCount = 0;

            //west
            if(x > 0 && IsValid(map, x-1, y, val)) { mNeighborPicks[neighborPickCount] = Dir.West; neighborPickCount++; }
            //east
            if(x < width-1 && IsValid(map, x+1, y, val)) { mNeighborPicks[neighborPickCount] = Dir.East; neighborPickCount++; }
            //north
            if(y > 0 && IsValid(map, x, y-1, val)) { mNeighborPicks[neighborPickCount] = Dir.North; neighborPickCount++; }
            //south
            if(y < height-1 && IsValid(map, x, y+1, val)) { mNeighborPicks[neighborPickCount] = Dir.South; neighborPickCount++; }

            Dir nextDir = neighborPickCount > 0 ? mNeighborPicks[M8.Noise.Generate.Range(0, neighborPickCount)] : Dir.Invalid;

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
        
        void Normalize(float[,] map, float maxValue) {
            for(int y = 0; y < height; y++) {
                for(int x = 0; x < width; x++) {
                    map[x, y] /= maxValue;
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
    }
}