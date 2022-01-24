using UnityEditor;
using UnityEngine;
using System.IO;

public class ScreenshotGrabber
{
    [MenuItem("Exts/Grab")]
    public static void Grab()
    {
        ScreenCapture.CaptureScreenshot(Directory.GetCurrentDirectory() + "/Screenshots/", 1);
    }

    [MenuItem("Exts/Reset")]
    public static void Reset()
    {
        SaveSystem.Reset();
    }
}