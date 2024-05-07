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

public static class StbImageLoader
{
    [DllImport("stb_lib", EntryPoint = "MyImageLoadFunction")]
    public static extern int MyImageLoadFunction(string filename);

    [DllImport("stb_lib", EntryPoint = "MyStbirResizeUint8")]
    public static extern int MyStbirResizeUint8(
    byte[] input_pixels, int input_w, int input_h, int input_stride_in_bytes,
    byte[] output_pixels, int output_w, int output_h, int output_stride_in_bytes,
    int num_channels);
}

public struct BC7EncodingSettings
{
    // 根据实际需要定义字段
}