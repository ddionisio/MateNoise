using UnityEngine;

namespace M8.Noise.Module {
    /// <summary>
    /// Get the summation of source modules.  The number of modules is dependent on what is provided to the constructor.
    /// This is a convenience for adding a series of modules, rather than having to deal with Add.
    /// </summary>
    public class Sum : ModuleBase {
        public override int sourceModuleCount { get { return mCount; } }

        public Sum(int _count)
            : base() {
            mSourceModules = new ModuleBase[_count];
            mCount = _count;
        }

        public Sum(params ModuleBase[] modules)
            : base() {
            mSourceModules = new ModuleBase[modules.Length];
            System.Array.Copy(modules, mSourceModules, mSourceModules.Length);
        }

        public override float GetValue(float x, float y, float z) {
            float val = 0;
            for(int i = 0; i < mSourceModules.Length; i++) {
                val += mSourceModules[i].GetValue(x, y, z);
            }
            return val;
        }

        private int mCount = 0;
    }
}