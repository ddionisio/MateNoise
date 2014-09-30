using UnityEngine;
using M8.Noise.Module;

namespace M8.Noise.Builder {
    public class CubeBuilder : BuilderBase3D {
        public bool seamless = false;
        public Bounds bounds;

        public void BuildPartialBegin() {
            destMap.SetSize(destWidth, destHeight, destDepth);
        }

        public void BuildPartial(int subX, int subY, int subZ, int subWidth, int subHeight, int subDepth) {
            Vector3 destSize = new Vector3(destWidth, destHeight, destDepth);
            Vector3 boundMin = bounds.min;
            Vector3 boundSize = bounds.size;
            Vector3 delta = new Vector3(boundSize.x/destSize.x, boundSize.y/destSize.y, boundSize.z/destSize.z);
            Vector3 curPos = Vector3.zero;

            curPos.y = boundMin.y + delta.y*(float)subY;
            for(int y = 0; y < subHeight; y++) {
                curPos.x = boundMin.z + delta.z*(float)subZ;
                for(int z = subZ; z < subDepth; z++) {
                    curPos.x = boundMin.x + delta.x*(float)subX;
                    for(int x = subX; x < subWidth; x++) {
                        float val;

                        if(seamless) {
                            float botValue = GetCornerValues(curPos.x, curPos.y, curPos.z, boundMin.x, boundMin.z, boundSize.x, boundSize.z);
                            float topValue = GetCornerValues(curPos.x, curPos.y + boundSize.y, curPos.z, boundMin.x, boundMin.z, boundSize.x, boundSize.z);
                            float yBlend = 1.0f - (y - boundMin.y)/boundSize.y;
                            val = Mathf.Lerp(botValue, topValue, yBlend);
                        }
                        else {
                            val = module.GetValue(curPos);
                        }

                        destMap.Set(x, y, z, val);
                        curPos.x += delta.x;
                    }

                    curPos.z += delta.z;
                }

                curPos.y += delta.y;
            }
        }

        public override void Build() {
            destMap.SetSize(destWidth, destHeight, destDepth);

            Vector3 destSize = new Vector3(destWidth, destHeight, destDepth);
            Vector3 boundMin = bounds.min;
            Vector3 boundSize = bounds.size;
            Vector3 delta = new Vector3(boundSize.x/destSize.x, boundSize.y/destSize.y, boundSize.z/destSize.z);
            Vector3 curPos = boundMin;

            for(int y = 0; y < destHeight; y++) {
                curPos.z = boundMin.z;
                for(int z = 0; z < destDepth; z++) {
                    curPos.x = boundMin.x;
                    for(int x = 0; x < destWidth; x++) {
                        float val;

                        if(seamless) {
                            float botValue = GetCornerValues(curPos.x, curPos.y, curPos.z, boundMin.x, boundMin.z, boundSize.x, boundSize.z);
                            float topValue = GetCornerValues(curPos.x, curPos.y + boundSize.y, curPos.z, boundMin.x, boundMin.z, boundSize.x, boundSize.z);
                            float yBlend = 1.0f - (y - boundMin.y)/boundSize.y;
                            val = Mathf.Lerp(botValue, topValue, yBlend);
                        }
                        else {
                            val = module.GetValue(curPos);
                        }

                        destMap.Set(x, y, z, val);
                        curPos.x += delta.x;
                    }

                    curPos.z += delta.z;
                }

                curPos.y += delta.y;
            }
        }

        private float GetCornerValues(float x, float y, float z, float xMin, float zMin, float xSize, float zSize) {
            float swValue = module.GetValue(x, y, z);
            float seValue = module.GetValue(x+xSize, y, z);
            float nwValue = module.GetValue(x, y, z+zSize);
            float neValue = module.GetValue(x+xSize, y, z+zSize);
            float xBlend = 1.0f - (x - xMin)/xSize;
            float zBlend = 1.0f - (z - zMin)/zSize;
            float v0 = Mathf.Lerp(swValue, seValue, xBlend);
            float v1 = Mathf.Lerp(nwValue, neValue, xBlend);
            return Mathf.Lerp(v0, v1, zBlend);
        }
    }
}