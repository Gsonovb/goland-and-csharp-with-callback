using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Runtime.CompilerServices;

namespace Golang.Ioc;






// type EventData struct {
// 	EventId   string `json:"eventId"`   //事件ID
// 	EventName string `json:"eventName"` //事件名称
// 	EventTime string `json:"eventTime"` //事件时间
// 	UserAppId string `json:"userAppId"` //用户应用ID
// 	EventData string `json:"eventData"` //事件数据
// }

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public struct CallBackEvent
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
    public string EventId;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string EventName;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
    public string EventTime;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
    public string UserAppId;

    public IntPtr EventData;


    //Get EventData String 
    public string GetEventData()
    {
        return Marshal.PtrToStringAnsi(EventData);
    }

    //Get EventData String Unicode
    public string GetEventDataUnicode()
    {
        return Marshal.PtrToStringUni(EventData);
    }

    //Get EventData String UTF-8
    public string GetEventDataUTF8()
    {
        return Marshal.PtrToStringUTF8(EventData);
    }

}



[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public struct CallBackEventData
{
    public int Id;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
    public string Name;

}



// 回调函数1指针
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void CallBack1(
    [MarshalUsing(typeof(CustomStringMarshaller))]
    //[MarshalAs(UnmanagedType.LPTStr)]
    //[MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(CustomStringMarshaller))]
    string data
    );


// 回调函数2 C字符数组指针
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void CallBack2(
    [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(CustomStringMarshaller))]
    string data
    );
//[MarshalAs(UnmanagedType.LPStr)]


// 回调函数3 C结构体指针
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void CallBack3([MarshalAs(UnmanagedType.LPStr)] CallBackEventData data);




internal static partial class NativeLib
{
    private const string LibName = "Golang.Ioc.Interop.dll";

    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    [LibraryImport(LibName)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool Check(long i1, long i2);


    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    [LibraryImport(LibName, StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(CustomStringMarshaller))]
    //[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(CustomStringMarshaller))]
    //[MarshalUsing(typeof(CustomStringMarshaller))]
    internal static partial string GetSlogan(string name);


    [LibraryImport(LibName)]
    internal static partial void Free(IntPtr ptr);


    // 初始化 日志  
    //extern __declspec(dllexport) void InitLog();
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    [LibraryImport(LibName, StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(CustomStringMarshaller))]
    internal static partial void InitLog();


    // 初始化 回调函数1 并设置 Gin1 实例 和端口
    // extern __declspec(dllexport) void InitCallBack1(void* cb, GoInt port);
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    [LibraryImport(LibName, StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(CustomStringMarshaller))]
    internal static partial void InitCallBack1(
        CallBack1 cb,
        long port);


    // 初始化 回调函数2 并设置 Gin2 实例 和端口
    // extern __declspec(dllexport) void InitCallBack2(void* cb, GoInt port);
    [LibraryImport(LibName, StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(CustomStringMarshaller))]
    internal static partial void InitCallBack2(CallBack2 cb, long port);

    // 初始化 回调函数3 并设置 Gin3 实例 和端口
    // extern __declspec(dllexport) void InitCallBack3(void* cb, GoInt port);
    [LibraryImport(LibName, StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(CustomStringMarshaller))]
    internal static partial void InitCallBack3(CallBack3 cb, long port);


    // 测试回调函数
    // extern __declspec(dllexport) void TestCallBack1();
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    [LibraryImport(LibName, StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(CustomStringMarshaller))]
    internal static partial void TestCallBack1();


    // 测试回调函数
    // extern __declspec(dllexport) void TestCallBack1();
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    [LibraryImport(LibName, StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(CustomStringMarshaller))]
    internal static partial void TestCallBack2();


    // 测试回调函数
    // extern __declspec(dllexport) void TestCallBack1();
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    [LibraryImport(LibName, StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(CustomStringMarshaller))]
    internal static partial void TestCallBack3();



}
