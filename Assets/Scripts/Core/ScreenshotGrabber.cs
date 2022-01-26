using UnityEditor;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
public class ScreenshotGrabber
{
    [MenuItem("Exts/Grab")]
    public static void Grab()
    {
        ScreenCapture.CaptureScreenshot(Directory.GetCurrentDirectory() + "/Screenshots/", 1);
    }

    [MenuItem("Exts/Saves/Reset")]
    public static void Reset()
    {
        SaveSystem.Reset();
    }

    [MenuItem("Exts/Saves/Gameplay")]
    public static void Gameplay()
    {
        SaveSystem.Save_AfterTutorialState();
    }
}
#endif