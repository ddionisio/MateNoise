using UnityEngine;
using M8.Noise.Module;
using M8.Noise.Map;

namespace M8.Noise.Builder {
    public abstract class BuilderBase2D {
        public ModuleBase module;

        public IMap2D destMap;
        public int destWidth;
        public int destHeight;

        public abstract void Build();
    }
}