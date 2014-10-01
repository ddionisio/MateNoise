using UnityEngine;
using System.Collections.Generic;
using M8.Noise.Module;
using M8.Noise.Builder;
using M8.Noise.Map;

namespace M8.Noise {
    public class RenderImage {
        public struct Gradient {
            public float pos; //[-1, 1]
            public Color color;
        }

        public bool lightEnabled = false;

        /// <summary>
        /// Returns the azimuth of the light source, in degrees.
        /// 0.0 degrees is east.
        /// 90.0 degrees is north.
        /// 180.0 degrees is west.
        /// 270.0 degrees is south.
        /// </summary>
        public float lightAzimuth {
            get { return mLightAzimuth; }
            set {
                mLightAzimuth = value;
                mRecalcLightValues = true;
            }
        }

        /// <summary>
        /// Set the brightness of the light source, 1.0f = full
        /// </summary>
        public float lightBrightness {
            get { return mLightBrightness; }
            set {
                mLightBrightness = value;
                mRecalcLightValues = true;
            }
        }

        /// <summary>
        /// The contrast determines the difference between areas in light and
        /// areas in shadow.  Determining the correct contrast amount requires
        /// some trial and error, but if your application interprets the noise
        /// map as a height map that has a spatial resolution of @a h meters
        /// and an elevation resolution of 1 meter, a good contrast amount to
        /// use is ( 1.0 / h ).
        /// </summary>
        public float lightContrast {
            get { return mLightContrast; }
            set {
                if(mLightContrast <= 0.0f) {
                    Debug.LogError("invalid light contrast, <= 0.0: "+value);
                }
                else {
                    mLightContrast = value;
                    mRecalcLightValues = true;
                }
            }
        }

        /// <summary>
        /// The elevation is the angle above the horizon:
        /// - 0 degrees is on the horizon.
        /// - 90 degrees is straight up.
        /// </summary>
        public float lightElevation {
            get { return mLightElev; }
            set {
                mLightElev = value;
                mRecalcLightValues = true;
            }
        }

        public float lightIntensity {
            get { return mLightIntensity; }
            set {
                if(mLightContrast < 0.0f) {
                    Debug.LogError("invalid light intensity, < 0.0: "+value);
                }
                else {
                    mLightIntensity = value;
                    mRecalcLightValues = true;
                }
            }
        }

        public Color lightColor = Color.white;

        public bool wrapEnabled = false;

        public NoiseMap2D sourceNoiseMap;

        public Texture2D backgroundTexture;

        /// <summary>
        /// The result after Render is called
        /// </summary>
        public Texture2D destTexture { get { return mDestTexture; } }

        public void AddGradientPoint(float pos, Color color) {
            Gradient newG = new Gradient() { pos=pos, color=color };
            if(mGradient.Count == 0) {
                mGradient.Add(newG);
            }
            else {
                for(int i = 0; i < mGradient.Count; i++) {
                    if(mGradient[i].pos == pos) {
                        mGradient[i] = newG;
                        return;
                    }
                    else if(mGradient[i].pos > pos) {
                        mGradient.Insert(i, newG);
                        return;
                    }
                }

                mGradient.Add(newG);
            }
        }

        public void BuildGrayscaleGradient() {
            mGradient.Clear();
            mGradient.Add(new Gradient() { pos = -1f, color = Color.black });
            mGradient.Add(new Gradient() { pos = 1f, color = Color.white });
        }

        public void BuildTerrainGradient() {
            mGradient.Clear();
            mGradient.Add(new Gradient() { pos = -1f, color = new Color(0f, 0f, 128f/255f) });
            mGradient.Add(new Gradient() { pos = -0.20f, color = new Color(32f/255f, 64f/255f, 128f/255f) });
            mGradient.Add(new Gradient() { pos = -0.04f, color = new Color(64f/255f, 96f/255f, 192f/255f) });
            mGradient.Add(new Gradient() { pos = -0.02f, color = new Color(192f/255f, 192f/255f, 128f/255f) });
            mGradient.Add(new Gradient() { pos = 0f, color = new Color(0f, 192f/255f, 0f) });
            mGradient.Add(new Gradient() { pos = 0.25f, color = new Color(192f/255f, 192f/255f, 0f) });
            mGradient.Add(new Gradient() { pos = 0.50f, color = new Color(160, 96f/255f, 64f/255f) });
            mGradient.Add(new Gradient() { pos = 0.75f, color = new Color(128f/255f, 1f, 1f) });
            mGradient.Add(new Gradient() { pos = 1f, color = Color.white });
        }

        public void ClearGradient() {
            mGradient.Clear();
        }

        /// <summary>
        /// Render the noise map onto destTexture
        /// </summary>
        public void Render() {
            if(sourceNoiseMap == null) {
                Debug.LogError("No source noise map."); return;
            }

            int width = sourceNoiseMap.width;
            int height = sourceNoiseMap.height;

            if(backgroundTexture && backgroundTexture.width != width && backgroundTexture.height != height) {
                Debug.LogError("Background texture ("+backgroundTexture.width+", "+backgroundTexture.height+") != "+"("+width+", "+height+")"); return;
            }

            if(mDestTexture == null || mDestTexture.width != width || mDestTexture.height != height)
                mDestTexture = new Texture2D(width, height);

            for(int y = 0; y < height; y++) {
                for(int x = 0; x < width; x++) {
                    // Get the color based on the value at the current point in the noise
                    // map.
                    Color destClr = GetGradientColor(sourceNoiseMap[x, y]);

                    // If lighting is enabled, calculate the light intensity based on the
                    // rate of change at the current point in the noise map.
                    float lightIntensity;
                    if(lightEnabled) {
                        // Get the noise value of the current point in the source noise map
                        // and the noise values of its four-neighbors.
                        float nc = sourceNoiseMap[x, y];
                        float nl, nr, nd, nu;

                        if(wrapEnabled) {
                            if(x == 0) {
                                nl = sourceNoiseMap[width-1, y];
                                nr = sourceNoiseMap[x+1, y];
                            }
                            else if(x == width - 1) {
                                nl = sourceNoiseMap[x-1, y];
                                nr = sourceNoiseMap[0, y];
                            }
                            else {
                                nl = sourceNoiseMap[x-1, y];
                                nr = sourceNoiseMap[x+1, y];
                            }

                            if(y == 0) {
                                nu = sourceNoiseMap[x, height - 1];
                                nd = sourceNoiseMap[x, y+1];
                            }
                            else if(y == height - 1) {
                                nu = sourceNoiseMap[x, y-1];
                                nd = sourceNoiseMap[x, 0];
                            }
                            else {
                                nu = sourceNoiseMap[x, y-1];
                                nd = sourceNoiseMap[x, y+1];
                            }
                        }
                        else {
                            if(x == 0) {
                                nl = nc;
                                nr = sourceNoiseMap[x+1, y];
                            }
                            else if(x == width - 1) {
                                nl = sourceNoiseMap[x-1, y];
                                nr = nc;
                            }
                            else {
                                nl = sourceNoiseMap[x-1, y];
                                nr = sourceNoiseMap[x+1, y];
                            }

                            if(y == 0) {
                                nu = nc;
                                nd = sourceNoiseMap[x, y+1];
                            }
                            else if(y == height - 1) {
                                nu = sourceNoiseMap[x, y-1];
                                nd = nc;
                            }
                            else {
                                nu = sourceNoiseMap[x, y-1];
                                nd = sourceNoiseMap[x, y+1];
                            }
                        }

                        // Now we can calculate the lighting intensity.
                        lightIntensity = CalcLightIntensity(nc, nl, nr, nd, nu) * mLightBrightness;

                    }
                    else {
                        // These values will apply no lighting to the destination image.
                        lightIntensity = 1.0f;
                    }

                    // Get the current background color from the background image.
                    Color bkgrndClr = backgroundTexture ? backgroundTexture.GetPixel(x, y) : Color.white;

                    // Blend the destination color, background color, and the light
                    // intensity together, then update the destination image with that
                    // color.
                    mDestTexture.SetPixel(x, y, CalcDestColor(destClr, bkgrndClr, lightIntensity));
                }
            }
        }

        public RenderImage() {
            BuildGrayscaleGradient();
        }

        Color GetGradientColor(float pos) {
            if(mGradient.Count >= 2) {
                int ind = 0;
                for(; ind < mGradient.Count; ind++) {
                    if(pos < mGradient[ind].pos)
                        break;
                }

                int ind0 = Mathf.Clamp(ind-1, 0, mGradient.Count - 1);
                int ind1 = Mathf.Clamp(ind, 0, mGradient.Count - 1);

                if(ind0 == ind1)
                    return mGradient[ind1].color;

                float inp0 = mGradient[ind0].pos;
                float inp1 = mGradient[ind1].pos;
                float a = (pos - inp0)/(inp1 - inp0);

                return Color.Lerp(mGradient[ind0].color, mGradient[ind1].color, (float)a);
            }
            else
                return Color.clear;
        }

        Color CalcDestColor(Color srcClr, Color bkgrndClr, float lightValue) {
            // First, blend the source color to the background color using the alpha
            // of the source color.
            float red   = Mathf.Lerp(bkgrndClr.r, srcClr.r, srcClr.a);
            float green = Mathf.Lerp(bkgrndClr.g, srcClr.g, srcClr.a);
            float blue  = Mathf.Lerp(bkgrndClr.b, srcClr.b, srcClr.a);

            if(lightEnabled) {

                // Now calculate the light color.
                float lightRed   = (float)lightValue * lightColor.r;
                float lightGreen = (float)lightValue * lightColor.g;
                float lightBlue  = (float)lightValue * lightColor.b;

                // Apply the light color to the new color.
                red   *= lightRed;
                green *= lightGreen;
                blue  *= lightBlue;
            }

            // Clamp the color channels to the (0..1) range.
            red   = (red   < 0.0f)? 0.0f: red;
            red   = (red   > 1.0f)? 1.0f: red;
            green = (green < 0.0f)? 0.0f: green;
            green = (green > 1.0f)? 1.0f: green;
            blue  = (blue  < 0.0f)? 0.0f: blue;
            blue  = (blue  > 1.0f)? 1.0f: blue;

            return new Color(red, green, blue, Mathf.Max(srcClr.a, bkgrndClr.a));
        }

        float CalcLightIntensity(float center, float left, float right, float down, float up) {
            // Recalculate the sine and cosine of the various light values if
            // necessary so it does not have to be calculated each time this method is
            // called.
            if(mRecalcLightValues) {
                mCosAzimuth = Mathf.Cos(mLightAzimuth*Mathf.Deg2Rad);
                mSinAzimuth = Mathf.Sin(mLightAzimuth * Mathf.Deg2Rad);
                mCosElev    = Mathf.Cos(mLightElev    * Mathf.Deg2Rad);
                mSinElev    = Mathf.Sin(mLightElev    * Mathf.Deg2Rad);
                mRecalcLightValues = false;
            }

            // Now do the lighting calculations.
            const float I_MAX = 1.0f;
            float io = I_MAX * Utils.SQRT_2 * mSinElev / 2.0f;
            float ix = (I_MAX - io) * mLightContrast * Utils.SQRT_2 * mCosElev * mCosAzimuth;
            float iy = (I_MAX - io) * mLightContrast * Utils.SQRT_2 * mCosElev * mSinAzimuth;
            float intensity = (ix * (left - right) + iy * (down - up) + io);
            if(intensity < 0.0f) {
                intensity = 0.0f;
            }
            return intensity;
        }

        private float mLightAzimuth = 45f;
        private float mLightBrightness = 1f;
        private float mLightContrast = 1f;
        private float mLightElev = 45f;
        private float mLightIntensity = 1f;

        private bool mRecalcLightValues;
        private float mCosAzimuth;
        private float mSinAzimuth;
        private float mCosElev;
        private float mSinElev;

        private Texture2D mDestTexture;
        private List<Gradient> mGradient = new List<Gradient>();
    }
}