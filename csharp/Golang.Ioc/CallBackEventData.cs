using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Serilog;

namespace Golang.Ioc;


/// <summary>
/// 自定义回调数据结构
/// </summary>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
[NativeMarshalling(typeof(CallBackEventDataMarshaller))]
public struct CallBackEventData
{
#nullable enable

    public int Id;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
    public string Name;

    public string? Data;


    public override string ToString()
    {
        return $"Id:{Id},Name:{Name},Data:{Data ?? "null"}";
    }

}

[CustomMarshaller(typeof(CallBackEventData), MarshalMode.ManagedToUnmanagedIn, typeof(CallBackEventDataMarshaller))]
[CustomMarshaller(typeof(CallBackEventData), MarshalMode.ManagedToUnmanagedOut, typeof(CallBackEventDataManagedToNativeOut))]
[CustomMarshaller(typeof(CallBackEventData), MarshalMode.ElementOut, typeof(CallBackEventDataOut))]
internal static unsafe class CallBackEventDataMarshaller
{


    /// <summary>
    /// Unmanaged representation of <see cref="ErrorData"/>
    ///  <see cref="ErrorData"/> 非托管数据类型表示
    /// </summary>
    internal struct CallBackEventDataUnmanaged
    {
        public int Id;
        public byte* Name;
        public byte* Data;
    }



    public static CallBackEventDataUnmanaged ConvertToUnmanaged(CallBackEventData managed)
    {
        //LOG DEBUG
        Trace.WriteLine("CallBackEventDataMarshaller: ConvertToUnmanaged  Convert To Unmanaged struct");
        Log.Information("CallBackEventDataMarshaller: ConvertToUnmanaged  Convert To Unmanaged struct");


        return new CallBackEventDataUnmanaged
        {
            Id = managed.Id,
            Name = (byte*)Marshal.StringToCoTaskMemUTF8(managed.Name), //  CustomStringMarshaller.ConvertToUnmanaged(managed.Name) ,
            Data = CustomStringMarshaller.ConvertToUnmanaged(managed.Data)
        };
    }

    public static void Free(CallBackEventDataUnmanaged unmanaged)
    {
        //LOG DEBUG
        Trace.WriteLine("CallBackEventDataMarshaller: Free  Free Unmanaged struct");

        IntPtr prtName = (IntPtr)(unmanaged.Name);
        Log.Information("CallBackEventDataMarshaller: Free Unmanaged struct.Name  , prtName:{prtName}", prtName);
        Marshal.FreeCoTaskMem(prtName);

        IntPtr prtData = (IntPtr)(unmanaged.Data);
        Log.Information("CallBackEventDataMarshaller: Free Unmanaged struct.Data  , prtData:{prtData}", prtData);
        CustomStringMarshaller.Free(unmanaged.Data);

        //CustomStringMarshaller.Free(unmanaged.Data);
        //Marshal.ZeroFreeCoTaskMemUTF8((nint)unmanaged.Name);
    }

    /// <summary>
    /// 自定义转换器,将非托管数据转换为托管数据
    /// </summary>
    private static class CallBackEventDataManagedToNativeOut
    {
        public static CallBackEventData ConvertToManaged(CallBackEventDataUnmanaged unmanaged)
        {
            //LOG DEBUG
            Trace.WriteLine("CallBackEventDataManagedToNativeOut: ConvertToManaged  Convert To Managed struct");
            Log.Information("CallBackEventDataManagedToNativeOut: ConvertToManaged  Convert To Managed struct");

            return new CallBackEventData
            {
                Id = unmanaged.Id,
                Name = Marshal.PtrToStringUTF8((IntPtr)unmanaged.Name) ?? string.Empty,
                Data = CustomStringMarshaller.ConvertToManaged(unmanaged.Data)
            };
        }

        public static void Free(CallBackEventDataUnmanaged unmanaged)
        {
            //LOG DEBUG
            Trace.WriteLine("CallBackEventDataMarshaller: Free  Free Managed struct");
            Log.Information("CallBackEventDataMarshaller: Free  Free Managed struct");

            CallBackEventDataMarshaller.Free(unmanaged);
        }
    }


    /// <summary>
    /// 自定义转换器,将非托管数据转换为托管数据
    /// </summary> 
    private static class CallBackEventDataOut
    {

        public static CallBackEventDataUnmanaged ConvertToUnmanaged(CallBackEventData managed)
        {
            //LOG DEBUG
            Trace.WriteLine("CallBackEventDataOut: ConvertToUnmanaged  Convert To Unmanaged struct");
            Log.Information("CallBackEventDataOut: ConvertToUnmanaged  Convert To Unmanaged struct");


            return new CallBackEventDataUnmanaged
            {
                Id = managed.Id,
                Name = (byte*)Marshal.StringToCoTaskMemUTF8(managed.Name), //  CustomStringMarshaller.ConvertToUnmanaged(managed.Name) ,
                Data = CustomStringMarshaller.ConvertToUnmanaged(managed.Data)
            };
        }

        public static CallBackEventData ConvertToManaged(CallBackEventDataUnmanaged unmanaged)
        {
            //LOG DEBUG
            Trace.WriteLine("CallBackEventDataOut: ConvertToManaged  Convert To Managed struct");
            Log.Information("CallBackEventDataOut: ConvertToManaged  Convert To Managed struct");

            return new CallBackEventData
            {
                Id = unmanaged.Id,
                Name = Marshal.PtrToStringUTF8((IntPtr)unmanaged.Name) ?? string.Empty,
                Data = CustomStringMarshaller.ConvertToManaged(unmanaged.Data)
            };
        }


        public static void Free(CallBackEventDataUnmanaged unmanaged)
        {
            //LOG DEBUG
            Trace.WriteLine("CallBackEventDataOut: Free  Free Managed struct");
            Log.Information("CallBackEventDataOut: Free  Free Managed struct");

            CallBackEventDataMarshaller.Free(unmanaged);
        }
    }
}