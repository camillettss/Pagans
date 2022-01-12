using UnityEditor;
using UnityEngine;
using System.IO;

public class ScreenshotGrabber
{
    [MenuItem("Screenshot/Grab")]
    public static void Grab()
    {
        ScreenCapture.CaptureScreenshot(Directory.GetCurrentDirectory() + "/Screenshots/", 1);
    }
}