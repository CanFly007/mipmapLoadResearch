using System.Runtime.InteropServices;
using System.Text;

public static class ProcessStartDLL
{

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    private const string libName = "PlatformDependentProcessDLL01";
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
    private const string libName = "libprocess";
#endif
    
    // 使用 libName 常量作为 DLL 名称
    [DllImport(libName, EntryPoint = "StartProcess", CharSet = CharSet.Auto)]
    public static extern int StartProcess(string path);

    [DllImport(libName, EntryPoint = "StartProcessWithCommand", CharSet = CharSet.Ansi)]
    public static extern int StartProcessWithCommand(string command);
}