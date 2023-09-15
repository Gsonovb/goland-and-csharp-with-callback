using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using Serilog;

namespace Golang.Ioc;

/// <summary>
/// 自定义String转换器(测试)
/// </summary>
/// <Remark>需要将非托管返回类型转换到托管类型</Remark>
[CustomMarshaller(typeof(string), MarshalMode.Default, typeof(CGoStringMarshaller))]
//[CustomMarshaller(typeof(string), MarshalMode.ManagedToUnmanagedIn, typeof(Utf8StringMarshaller.ManagedToUnmanagedIn))]

internal static unsafe class CGoStringMarshaller
{
#nullable enable

    /// <summary>
    /// Converts a string to an unmanaged version.
    /// </summary>
    /// <param name="managed">The managed string to convert.</param>
    /// <returns>An unmanaged string.</returns>
    public static byte* ConvertToUnmanaged(string? managed)
    {
        Trace.WriteLine("CGoStringMarshaller: OnConvertToUnmanaged start");
        Log.Information("CGoStringMarshaller: OnConvertToUnmanaged start");

        if (managed is null)
            return null;

        int exactByteCount = checked(Encoding.UTF8.GetByteCount(managed) + 1); // + 1 for null terminator
        byte* mem = (byte*)Marshal.AllocCoTaskMem(exactByteCount);
        Span<byte> buffer = new(mem, exactByteCount);

        int byteCount = Encoding.UTF8.GetBytes(managed, buffer);
        buffer[byteCount] = 0; // 设置为 0,表示C中的字符末尾

        // 获取 内存地址 值
        IntPtr ptr = (IntPtr)(mem);

        Trace.WriteLine("CGoStringMarshaller: OnConvertToUnmanaged end, ptr:" + ptr.ToString("X"));
        Log.Information("CGoStringMarshaller: OnConvertToUnmanaged end , ptr: {address}", ptr.ToString("X"));

        return mem;
    }

    /// <summary>
    /// Converts an unmanaged string to a managed version.
    /// </summary>
    /// <param name="unmanaged">The unmanaged string to convert.</param>
    /// <returns>A managed string.</returns>
    public static string? ConvertToManaged(byte* unmanaged)
    {
        // 获取 内存地址 值
        IntPtr ptr = (IntPtr)(unmanaged);

        Trace.WriteLine("CGoStringMarshaller: OnConvertToManaged start, prt:" + ptr.ToString("X"));
        Log.Information("CGoStringMarshaller: OnConvertToManaged start, unmanaged prt: {address}", ptr.ToString("X"));

        return Marshal.PtrToStringUTF8((IntPtr)unmanaged);
    }

    /// <summary>
    /// Free the memory for a specified unmanaged string.
    /// </summary>
    /// <param name="unmanaged">The memory allocated for the unmanaged string.</param>
    public static void Free(byte* unmanaged)
    {
        // 获取 内存地址 值
        IntPtr ptr = (IntPtr)(unmanaged);
        Trace.WriteLine("CGoStringMarshaller: OnFree start , prt: " + ptr.ToString("X"));
        Log.Information("CGoStringMarshaller: OnFree start, prt: {address}", ptr.ToString("X"));
        NativeLib.Free((IntPtr)unmanaged);
    }


}

/// <summary>
/// Marshaller for UTF-8 strings.
/// </summary>
[CustomMarshaller(typeof(string), MarshalMode.Default, typeof(CustomStringMarshaller))]
[CustomMarshaller(typeof(string), MarshalMode.ManagedToUnmanagedIn, typeof(ManagedToUnmanagedIn))]
[CustomMarshaller(typeof(string), MarshalMode.UnmanagedToManagedIn, typeof(NativeToManaged))]
internal static unsafe class CustomStringMarshaller
{
    /// <summary>
    /// Converts a string to an unmanaged version.
    /// 转换字符串为非托管版本
    /// </summary>
    /// <param name="managed">
    /// The managed string to convert.
    /// 要转换的托管字符串
    /// </param>
    /// <returns>
    /// An unmanaged string.
    /// 一个非托管字符串
    /// </returns>
    public static byte* ConvertToUnmanaged(string? managed)
    {
        Trace.WriteLine("CustomStringMarshaller: OnConvertToUnmanaged start");
        Log.Information("CustomStringMarshaller: OnConvertToUnmanaged start");

        if (managed is null)
            return null;

        int exactByteCount = checked(Encoding.UTF8.GetByteCount(managed) + 1); // + 1 for null terminator
        byte* mem = (byte*)Marshal.AllocCoTaskMem(exactByteCount);
        Span<byte> buffer = new(mem, exactByteCount);

        int byteCount = Encoding.UTF8.GetBytes(managed, buffer);
        buffer[byteCount] = 0; // null-terminate

        // 获取 内存地址 值
        IntPtr ptr = (IntPtr)(mem);

        Trace.WriteLine("CustomStringMarshaller: OnConvertToUnmanaged end, ptr:" + ptr.ToString("X"));
        Log.Information("CustomStringMarshaller: OnConvertToUnmanaged end , ptr: {address}", ptr.ToString("X"));

        return mem;
    }

    /// <summary>
    /// Converts an unmanaged string to a managed version.
    /// 转换非托管字符串为托管版本
    /// </summary>
    /// <param name="unmanaged">
    /// The unmanaged string to convert.
    /// 要转换的非托管字符串
    /// </param>
    /// <returns>
    /// A managed string.
    /// 一个托管字符串
    /// </returns>
    public static string? ConvertToManaged(byte* unmanaged)
    {
        // 获取 内存地址 值
        IntPtr ptr = (IntPtr)(unmanaged);

        Trace.WriteLine("CustomStringMarshaller: OnConvertToManaged start, prt:" + ptr.ToString("X"));
        Log.Information("CustomStringMarshaller: OnConvertToManaged start, unmanaged prt: {address}", ptr.ToString("X"));

        return Marshal.PtrToStringUTF8((IntPtr)unmanaged);
    }

    /// <summary>
    /// Free the memory for a specified unmanaged string.
    /// 释放非托管字符串的内存
    /// </summary>
    /// <param name="unmanaged">
    /// The memory allocated for the unmanaged string.
    /// 为非托管字符串分配的内存地址
    /// </param>
    public static void Free(byte* unmanaged)
    {

        // 获取 内存地址 值
        IntPtr ptr = (IntPtr)(unmanaged);
        Trace.WriteLine("CustomStringMarshaller: OnFree start , prt: " + ptr.ToString("X"));
        Log.Information("CustomStringMarshaller: OnFree start, prt: {address}", ptr.ToString("X"));


        Marshal.FreeCoTaskMem((IntPtr)unmanaged);
    }

    /// <summary>
    /// custom marshaller to marshal a managed string as a UTF-8 unmanaged string.
    /// 自定义转换器,将托管字符串转换为非托管UTF-8字符串
    /// </summary>
    public static class NativeToManaged
    {

        /// <summary>
        /// Converts an unmanaged string to a managed version.
        /// 转换非托管字符串为托管版本
        /// </summary>
        /// <param name="unmanaged">
        /// The unmanaged string to convert.
        /// 要转换的非托管字符串
        /// </param>
        /// <returns>
        /// A managed string.
        /// 一个托管字符串
        /// </returns>
        public static string? ConvertToManaged(byte* unmanaged)
        {
            // 获取 内存地址 值
            IntPtr ptr = (IntPtr)(unmanaged);

            Trace.WriteLine("CustomStringMarshaller: NativeToManaged start, prt:" + ptr.ToString("X"));
            Log.Information("CustomStringMarshaller: NativeToManaged start, unmanaged prt: {address}", ptr.ToString("X"));

            return Marshal.PtrToStringUTF8((IntPtr)unmanaged);
        }

        /// <summary>
        /// Free the memory for a specified unmanaged string.
        /// 释放非托管字符串的内存
        /// </summary>
        /// <param name="unmanaged">
        /// The memory allocated for the unmanaged string.
        /// 为非托管字符串分配的内存地址
        /// </param>
        public static void Free(byte* unmanaged)
        {
            // 获取 内存地址 值
            IntPtr ptr = (IntPtr)(unmanaged);
            Trace.WriteLine("CustomStringMarshaller: NativeToManaged.Free start , prt: " + ptr.ToString("X"));
            Log.Information("CustomStringMarshaller: NativeToManaged.Free start, prt: {address}", ptr.ToString("X"));

            NativeLib.Free((IntPtr)unmanaged); // Here is native free from CGO ; 这里是CGO中的本地释放;
        }

    }


    /// <summary>
    /// Custom marshaller to marshal a managed string as a UTF-8 unmanaged string.
    /// 自定义转换器,将托管字符串转换为非托管UTF-8字符串
    /// </summary>
    public ref struct ManagedToUnmanagedIn
    {
        /// <summary>
        /// Gets the requested buffer size for optimized marshalling.
        /// 获取优化转换的请求缓冲区大小
        /// </summary>
        public static int BufferSize => 0x100; // 256 bytes

        private byte* _unmanagedValue;
        private bool _allocated;

        /// <summary>
        /// Initializes the marshaller with a managed string and requested buffer.
        /// 使用托管字符串和请求的缓冲区初始化转换器
        /// </summary>
        /// <param name="managed">
        /// The managed string with which to initialize the marshaller.
        /// 要转换的托管字符串
        /// </param>
        /// <param name="buffer">
        /// The request buffer whose size is at least <see cref="BufferSize"/>.
        /// 请求的缓冲区,大小至少为<see cref="BufferSize"/>
        /// </param>
        public void FromManaged(string? managed, Span<byte> buffer)
        {

            Trace.WriteLine("CustomStringMarshaller: ManagedToUnmanagedIn.FromManaged start");
            Log.Information("CustomStringMarshaller: ManagedToUnmanagedIn.FromManaged start");


            _allocated = false;

            if (managed is null)
            {
                _unmanagedValue = null;
                return;
            }

            const int MaxUtf8BytesPerChar = 3;

            // >= for null terminator
            // Use the cast to long to avoid the checked operation
            // 大于等于(>=)空终止符
            // 使用long类型转换避免检查操作
            if ((long)MaxUtf8BytesPerChar * managed.Length >= buffer.Length)
            {
                // Calculate accurate byte count when the provided stack-allocated buffer is not sufficient
                // 当提供的栈分配缓冲区不足时,计算准确的字节数
                int exactByteCount = checked(Encoding.UTF8.GetByteCount(managed) + 1); // + 1 for null terminator ; +1 用于空终止符
                if (exactByteCount > buffer.Length)
                {
                    buffer = new Span<byte>((byte*)NativeMemory.Alloc((nuint)exactByteCount), exactByteCount);
                    _allocated = true;
                }
            }

            _unmanagedValue = (byte*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(buffer));

            int byteCount = Encoding.UTF8.GetBytes(managed, buffer);
            buffer[byteCount] = 0; // null-terminate 空终止符

            // 获取 内存地址 值
            IntPtr ptr = (IntPtr)(_unmanagedValue);

            Trace.WriteLine("CustomStringMarshaller: ManagedToUnmanagedIn.FromManaged end, ptr:" + ptr.ToString("X"));
            Log.Information("CustomStringMarshaller: ManagedToUnmanagedIn.FromManaged end , ptr: {address}", ptr.ToString("X"));

        }

        /// <summary>
        /// Converts the current managed string to an unmanaged string.
        /// 将当前托管字符串转换为非托管字符串
        /// </summary>
        /// <returns>
        /// An unmanaged string.
        /// 一个非托管字符串
        /// </returns>
        public byte* ToUnmanaged()
        {
            // 获取 内存地址 值
            IntPtr ptr = (IntPtr)(_unmanagedValue);

            Trace.WriteLine("CustomStringMarshaller: ManagedToUnmanagedIn.ToUnmanaged start, prt:" + ptr.ToString("X"));
            Log.Information("CustomStringMarshaller: ManagedToUnmanagedIn.ToUnmanaged start, unmanaged prt: {address}", ptr.ToString("X"));


            return _unmanagedValue;
        }

        /// <summary>
        /// Frees any allocated unmanaged memory.
        /// 释放任何分配的非托管内存
        /// </summary>
        public void Free()
        {
            Trace.WriteLine("CustomStringMarshaller: ManagedToUnmanagedIn.Free start");

            if (_allocated)
            {
                // 获取 内存地址 值
                IntPtr ptr = (IntPtr)(_unmanagedValue);
                Trace.WriteLine("CustomStringMarshaller: ManagedToUnmanagedIn.Free Free memory , prt: " + ptr.ToString("X"));
                Log.Information("CustomStringMarshaller: ManagedToUnmanagedIn.Free Free memory, prt: {address}", ptr.ToString("X"));

                NativeMemory.Free(_unmanagedValue);
            }
        }
    }
}
