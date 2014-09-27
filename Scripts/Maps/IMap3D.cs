
namespace M8.Noise.Map {
    public interface IMap3D {
        void Set(int x, int y, int z, float value);
        void SetSize(int width, int height, int depth);
    }
}