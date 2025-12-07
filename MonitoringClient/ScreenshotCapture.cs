using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

public class ScreenshotCapture
{
    private string screenshotDir;

    public ScreenshotCapture()
    {
        screenshotDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UnityMonitoring", "Screenshots");
        Directory.CreateDirectory(screenshotDir);
    }

    public string CaptureScreenshot()
    {
        try
        {
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                }

                string filename = $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
                string filepath = Path.Combine(screenshotDir, filename);
                bitmap.Save(filepath, ImageFormat.Jpeg);
                return filepath;
            }
        }
        catch
        {
            return null;
        }
    }

    public string CaptureScreenshotIfAnomaly(bool isAnomaly)
    {
        if (isAnomaly)
            return CaptureScreenshot();
        return null;
    }

    public string[] GetRecentScreenshots(int count = 10)
    {
        try
        {
            if (!Directory.Exists(screenshotDir))
                return new string[0];

            var files = Directory.GetFiles(screenshotDir, "screenshot_*.jpg")
                .OrderByDescending(f => new FileInfo(f).CreationTime)
                .Take(count)
                .ToArray();

            return files;
        }
        catch
        {
            return new string[0];
        }
    }
}

