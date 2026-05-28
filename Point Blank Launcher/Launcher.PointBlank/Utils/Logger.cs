using System;
using System.IO;
using System.Windows.Forms;

public class Logger
{
    public static void Log(string texto)
    {
        StreamWriter streamWriter = new StreamWriter(string.Concat(Application.StartupPath, "\\PBLauncher.log"), true);
        string[] strArrays = new string[] { "[", DateTime.Now.ToString("HH:mm:ss"), "] ", texto };
        if (texto == null)
        {
            streamWriter.WriteLine("");
        }
        else { streamWriter.WriteLine(string.Concat(strArrays)); }
        streamWriter.Flush();
        streamWriter.Close();
    }
}