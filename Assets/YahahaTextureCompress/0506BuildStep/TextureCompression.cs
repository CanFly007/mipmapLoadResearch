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






public static class Astcenc
{
    [DllImport("astcenc-native")]
    public static extern astcenc_error astcenc_config_init(
        astcenc_profile profile,
        uint block_x,
        uint block_y,
        uint block_z,
        float quality,
        uint flags,
        out astcenc_config config);

    [DllImport("astcenc-native")]
    public static extern astcenc_error astcenc_context_alloc(
        ref astcenc_config config,
        uint thread_count,
        out IntPtr context);

    [DllImport("astcenc-native")]
    public static extern astcenc_error astcenc_compress_image(
        IntPtr context,
        IntPtr image,
        IntPtr swizzle,
        byte[] data_out,
        UIntPtr data_len,
        uint thread_index);

    [DllImport("astcenc-native")]
    public static extern astcenc_error astcenc_compress_reset(
        IntPtr context);

    [DllImport("astcenc-native")]
    public static extern astcenc_error astcenc_decompress_image(
        IntPtr context,
        byte[] data,
        UIntPtr data_len,
        IntPtr image_out,
        IntPtr swizzle,
        uint thread_index);

    [DllImport("astcenc-native")]
    public static extern astcenc_error astcenc_decompress_reset(
        IntPtr context);

    [DllImport("astcenc-native")]
    public static extern void astcenc_context_free(
        IntPtr context);

    [DllImport("astcenc-native")]
    public static extern astcenc_error astcenc_get_block_info(
        IntPtr context,
        byte[] data,
        out astcenc_block_info info);

    [DllImport("astcenc-native")]
    public static extern IntPtr astcenc_get_error_string(
        astcenc_error status);
}

// Enums and structs that might be used with the astcenc API
public enum astcenc_error
{
    /** @brief The call was successful. */
    ASTCENC_SUCCESS = 0,
    /** @brief The call failed due to low memory, or undersized I/O buffers. */
    ASTCENC_ERR_OUT_OF_MEM,
    /** @brief The call failed due to the build using fast math. */
    ASTCENC_ERR_BAD_CPU_FLOAT,
    /** @brief The call failed due to an out-of-spec parameter. */
    ASTCENC_ERR_BAD_PARAM,
    /** @brief The call failed due to an out-of-spec block size. */
    ASTCENC_ERR_BAD_BLOCK_SIZE,
    /** @brief The call failed due to an out-of-spec color profile. */
    ASTCENC_ERR_BAD_PROFILE,
    /** @brief The call failed due to an out-of-spec quality value. */
    ASTCENC_ERR_BAD_QUALITY,
    /** @brief The call failed due to an out-of-spec component swizzle. */
    ASTCENC_ERR_BAD_SWIZZLE,
    /** @brief The call failed due to an out-of-spec flag set. */
    ASTCENC_ERR_BAD_FLAGS,
    /** @brief The call failed due to the context not supporting the operation. */
    ASTCENC_ERR_BAD_CONTEXT,
    /** @brief The call failed due to unimplemented functionality. */
    ASTCENC_ERR_NOT_IMPLEMENTED,
    /** @brief The call failed due to an out-of-spec decode mode flag set. */
    ASTCENC_ERR_BAD_DECODE_MODE
}

public enum astcenc_profile
{
    /** @brief The LDR sRGB color profile. */
    ASTCENC_PRF_LDR_SRGB = 0,
    /** @brief The LDR linear color profile. */
    ASTCENC_PRF_LDR,
    /** @brief The HDR RGB with LDR alpha color profile. */
    ASTCENC_PRF_HDR_RGB_LDR_A,
    /** @brief The HDR RGBA color profile. */
    ASTCENC_PRF_HDR
}

[StructLayout(LayoutKind.Sequential)]
public struct astcenc_block_info
{
    // Define the structure fields as per the documentation
}

[StructLayout(LayoutKind.Sequential)]
public struct astcenc_swizzle
{
    public byte r, g, b, a;
}

[StructLayout(LayoutKind.Sequential)]
public struct astcenc_image
{
    // Define the structure fields as per the documentation
}

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void AstcencProgressCallback(float progress);

[StructLayout(LayoutKind.Sequential)]
public struct astcenc_config
{
    public astcenc_profile profile;
    public uint flags;
    public uint block_x;
    public uint block_y;
    public uint block_z;
    public float cw_r_weight;
    public float cw_g_weight;
    public float cw_b_weight;
    public float cw_a_weight;
    public uint a_scale_radius;
    public float rgbm_m_scale;
    public uint tune_partition_count_limit;
    public uint tune_2partition_index_limit;
    public uint tune_3partition_index_limit;
    public uint tune_4partition_index_limit;
    public uint tune_block_mode_limit;
    public uint tune_refinement_limit;
    public uint tune_candidate_limit;
    public uint tune_2partitioning_candidate_limit;
    public uint tune_3partitioning_candidate_limit;
    public uint tune_4partitioning_candidate_limit;
    public float tune_db_limit;
    public float tune_mse_overshoot;
    public float tune_2partition_early_out_limit_factor;
    public float tune_3partition_early_out_limit_factor;
    public float tune_2plane_early_out_limit_correlation;
    public float tune_search_mode0_enable;
    public IntPtr progress_callback;  // 使用 IntPtr 来表示指向委托的指针
}