using UnityEngine;

public static class HapticsAdvanced
{
    public static void Light()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidVibrate(20, 50);
#elif UNITY_IOS && !UNITY_EDITOR
        Handheld.Vibrate();
#endif
    }

    public static void Medium()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidVibrate(35, 120);
#elif UNITY_IOS && !UNITY_EDITOR
        Handheld.Vibrate();
#endif
    }

    public static void Heavy()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidVibrate(60, 220);
#elif UNITY_IOS && !UNITY_EDITOR
        Handheld.Vibrate();
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    static void AndroidVibrate(long ms, int amplitude)
    {
        try
        {
            using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            using var vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator");

            if (vibrator == null) return;

            using var version = new AndroidJavaClass("android.os.Build$VERSION");
            int sdk = version.GetStatic<int>("SDK_INT");

            if (sdk >= 26)
            {
                using var effect = new AndroidJavaClass("android.os.VibrationEffect")
                    .CallStatic<AndroidJavaObject>("createOneShot", ms, amplitude);
                vibrator.Call("vibrate", effect);
            }
            else
            {
                vibrator.Call("vibrate", ms);
            }
        }
        catch
        {
            Handheld.Vibrate(); // fallback
        }
    }
#endif
}
