using System.Runtime.InteropServices;
using System.Text;

public static class ProcessStartDLL
{
    [DllImport("PlatformDependentProcessDLL01")]
    public static extern int StartProcess(string path);

    [DllImport("PlatformDependentProcessDLL01", CharSet = CharSet.Unicode)]
    public static extern int StartProcessWithCommand(string command);
}
