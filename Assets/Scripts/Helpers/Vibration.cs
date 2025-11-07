using System.Runtime.InteropServices;
using BloomLines.Managers;

namespace BloomLines.Helpers
{
    public static class Vibration
    {
#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void VibrateWeb(int ms);
#endif

        private static bool CanVibrate => SaveManager.GameState.Vibration;

        public static void Vibrate(int ms)
        {
            if (!CanVibrate)
                return;

#if UNITY_EDITOR
            return;
#endif

#if UNITY_WEBGL
            VibrateWeb(ms);
#else
            VibrationAssets.Vibration.Vibrate(ms);
#endif
        }

        public static void Stop()
        {
            if (!CanVibrate)
                return;

#if UNITY_EDITOR
            return;
#endif

#if UNITY_WEBGL
            VibrateWeb(0);
#else
            VibrationAssets.Vibration.Cancel();
#endif
        }
    }
}