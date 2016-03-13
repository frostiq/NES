using UnityEngine;

namespace Assets.Scripts.Lib
{
    public class ScreenshootMaker
    {
        private readonly Resolution _resolution;

        public ScreenshootMaker(Resolution resolution)
        {
            this._resolution = resolution;
        }

        public byte[] TakeScreenshoot(Camera camera)
        {
            RenderTexture rt = new RenderTexture(_resolution.width, _resolution.height, 24);
            camera.targetTexture = rt;
            Texture2D screenShot = new Texture2D(_resolution.width, _resolution.height, TextureFormat.RGB24, false);
            camera.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, _resolution.width, _resolution.height), 0, 0);
            camera.targetTexture = null;
            RenderTexture.active = null; // JC: added to avoid errors
            return screenShot.EncodeToPNG();
        }
    }
}
