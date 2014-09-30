using UnityEngine;
using M8.Noise.Module;

namespace M8.Noise.Builder {
    public class PlaneBuilder : BuilderBase2D {
        public bool seamless = false;
        public Rect bounds;

        public void BuildPartialBegin() {
            destMap.SetSize(destWidth, destHeight);
        }

        public void BuildPartial(int subX, int subY, int subWidth, int subHeight) {
            Vector2 destSize = new Vector2(destWidth, destHeight);
            Vector2 boundMin = bounds.min;
            Vector2 boundSize = bounds.size;
            Vector2 delta = new Vector2(boundSize.x/destSize.x, boundSize.y/destSize.y);
            Vector2 curPos = Vector2.zero;

            curPos.y = boundMin.y + delta.y*(float)subY;
            for(int y = 0; y < subHeight; y++) {
                curPos.x = boundMin.x + delta.x*(float)subX;
                for(int x = subX; x < subWidth; x++) {
                    float val;

                    if(seamless) {
                        val = GetCornerValues(curPos.x, curPos.y, boundMin.x, boundMin.y, boundSize.x, boundSize.y);
                    }
                    else {
                        val = module.GetValue(curPos.x, 0f, curPos.y);
                    }

                    destMap.Set(x, y, val);
                    curPos.x += delta.x;
                }
                curPos.y += delta.y;
            }
        }

        public override void Build() {
            destMap.SetSize(destWidth, destHeight);

            Vector2 destSize = new Vector2(destWidth, destHeight);
            Vector2 boundMin = bounds.min;
            Vector2 boundSize = bounds.size;
            Vector2 delta = new Vector2(boundSize.x/destSize.x, boundSize.y/destSize.y);
            Vector2 curPos = boundMin;

            for(int y = 0; y < destHeight; y++) {
                curPos.y = boundMin.y;
                for(int x = 0; x < destWidth; x++) {
                    float val;

                    if(seamless) {
                        val = GetCornerValues(curPos.x, curPos.y, boundMin.x, boundMin.y, boundSize.x, boundSize.y);
                    }
                    else {
                        val = module.GetValue(curPos.x, 0f, curPos.y);
                    }

                    destMap.Set(x, y, val);
                    curPos.x += delta.x;
                }
                curPos.y += delta.y;
            }
        }

        private float GetCornerValues(float x, float y, float xMin, float yMin, float xSize, float ySize) {
            float swValue = module.GetValue(x, 0f, y);
            float seValue = module.GetValue(x+xSize, 0f, y);
            float nwValue = module.GetValue(x, 0f, y+ySize);
            float neValue = module.GetValue(x+xSize, 0f, y+ySize);
            float xBlend = 1.0f - (x - xMin)/xSize;
            float yBlend = 1.0f - (y - yMin)/ySize;
            float v0 = Mathf.Lerp(swValue, seValue, xBlend);
            float v1 = Mathf.Lerp(nwValue, neValue, xBlend);
            return Mathf.Lerp(v0, v1, yBlend);
        }
    }
}