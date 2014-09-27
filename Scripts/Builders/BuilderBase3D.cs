using UnityEngine;
using M8.Noise.Module;
using M8.Noise.Map;

namespace M8.Noise.Builder {
    public abstract class BuilderBase3D {
        public ModuleBase module;

        public IMap3D destMap;
        public int destWidth;
        public int destHeight;
        public int destDepth;

        public abstract void Build();
    }
}