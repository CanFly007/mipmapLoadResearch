using System;
using System.Runtime.InteropServices;

public static class TextureCompression
{
    [DllImport("ispc_texcomp")]
    public static extern void CompressBlocksBC1(IntPtr input, IntPtr output);

    [DllImport("ispc_texcomp")]
    public static extern void CompressBlocksBC3(IntPtr input, IntPtr output);

    [DllImport("ispc_texcomp")]
    public static extern void CompressBlocksBC7(IntPtr input, IntPtr output, ref BC7EncodingSettings settings);

    // 可以添加其他方法的声明
}

public struct BC7EncodingSettings
{
    // 根据实际需要定义字段
}