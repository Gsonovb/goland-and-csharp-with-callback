﻿using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using Serilog;


namespace Golang.Ioc
{




    internal class Program
    {



        private static bool IsInitLog = false;

        private static void Main(string[] args)
        {

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.WithProperty("Lang", "C#")
                .WriteTo.Console()
                .WriteTo.Seq("http://localhost:5341")
                .CreateLogger();


            CheckDemo();
            StringDemo();
            StringDemo();
            //PrintGCInfo();
            //StringTest();
            //PrintGCInfo();

            InitLog();


            // Console.WriteLine("==================");
            // Console.WriteLine("输入任意字符开始测试回调函数1");

            // Console.ReadLine();

            // InitCallBack1();
            // DoTestCallBack1();




            // Console.WriteLine("==================");
            // Console.WriteLine("输入任意字符开始测试回调函数2");
            // Console.ReadLine();

            // InitCallBack2();


            // DoTestCallBack2();



            Console.WriteLine("==================");
            Console.WriteLine("输入任意字符开始测试回调函数3");
            Console.ReadLine();


            InitCallBack3();

            DoTestCallBack3();

            Console.WriteLine("==================");
            Console.WriteLine("输入任意字符开始测试回调函数4");
            Console.ReadLine();


            InitCallBack4();

            DoTestCallBack4();



            Console.WriteLine("按回车键结束");

            Console.ReadLine();

            Log.CloseAndFlush();
        }

        private static void DoTestCallBack1()
        {
            Log.Information("测试 TestCallBack1 开始 !");
            NativeLib.TestCallBack1();
            Log.Information("测试 TestCallBack1 完成 !");
        }



        private static void DoTestCallBack2()
        {
            Log.Information("测试 TestCallBack2 开始 !");
            NativeLib.TestCallBack2();
            Log.Information("测试 TestCallBack2 完成 !");
        }



        /// <summary>
        /// 初始化日志
        /// </summary>
        private static void InitLog()
        {
            if (IsInitLog)
            {
                return;
            }

            Log.Information("InitLog !");
            NativeLib.InitLog();
            IsInitLog = true;

            Log.Information("InitLog end !");
        }


        // 初始化 回调1

        private static CallBack1 _cb1;

        private static void InitCallBack1()
        {
            if (_cb1 != null)
            {
                return;
            }
            Log.Information("Call InitCallBack1");

            _cb1 = new CallBack1(OnCallBack1);
            NativeLib.InitCallBack1(_cb1, 9081);

            Log.Information("Call InitCallBack1 end");
        }


        private static void OnCallBack1(string data)
        {
            Log.Information("C#: OnCallBack1:{data}", data);
        }


        // 初始化 回调2
        private static CallBack2 _cb2;

        private static void InitCallBack2()
        {
            if (_cb2 != null)
            {
                return;
            }
            Log.Information("Call InitCallBack2");
            _cb2 = new CallBack2(OnCallBack2);
            NativeLib.InitCallBack2(_cb2, 9082);
            Log.Information("Call InitCallBack2 end");

        }


        private static void OnCallBack2(string data)
        {
            //Console.WriteLine($"C#: OnCallBack2:{dPtr}");
            Log.Information("C#: OnCallBack2:{data}", data);
        }


        // 初始化 回调3

        private static CallBack3 _cb3;

        private static void InitCallBack3()
        {
            if (_cb3 != null)
            {
                return;
            }
            Log.Information("Call InitCallBack3");
            _cb3 = new CallBack3(OnCallBack3);
            NativeLib.InitCallBack3(_cb3, 9083);
            Log.Information("Call InitCallBack3 end");
        }


        private static void OnCallBack3([MarshalUsing(typeof(CallBackEventDataMarshaller))] CallBackEventData data)
        {
            Log.Information("C#: OnCallBack3: {data}", data);

            // Console.WriteLine($"C#: OnCallBack3: ");

            // Console.WriteLine($"C#: EventId  :{dPtr.EventId}");
            // Console.WriteLine($"C#: EventName:{dPtr.EventName}");
            // Console.WriteLine($"C#: EventTime:{dPtr.EventTime}");
            // Console.WriteLine($"C#: UserAppId:{dPtr.UserAppId}");

            // Console.WriteLine($"C#: EventData:");
            // Console.WriteLine($"C#:      ANSI:{dPtr.GetEventData()}");
            // Console.WriteLine($"C#:   Unicode:{dPtr.GetEventDataUnicode()}");
            // Console.WriteLine($"C#:     UTF-8:{dPtr.GetEventDataUTF8()}");

        }

        private static void DoTestCallBack3()
        {
            Log.Information("测试 TestCallBack3 开始 !");
            NativeLib.TestCallBack3();
            Log.Information("测试 TestCallBack3 完成 !");
        }


        // 初始化 回调4

        private static CallBack4 _cb4;

        private static void InitCallBack4()
        {
            if (_cb4 != null)
            {
                return;
            }
            Log.Information("Call InitCallBack4");
            _cb4 = new CallBack4(OnCallBack4);
            NativeLib.InitCallBack4(_cb4, 9084);
            Log.Information("Call InitCallBack4 end");
        }

        private unsafe static void OnCallBack4(IntPtr dPtr)  // CallBackEventData dPtr)
        {
            //log prt address
            Log.Information("C#: OnCallBack4 prt: {prt}", dPtr.ToString("X"));


            // get dPtr

            CallBackEventData eventData = Marshal.PtrToStructure<CallBackEventData>(dPtr);

            Log.Information("C#: OnCallBack4: {data}", eventData);



            int memsize = Marshal.SizeOf<CallBackEventData>();
            int memsize2 = Marshal.SizeOf<CallBackEventData2>();


            ReadOnlySpan<byte> databytes = new(dPtr.ToPointer(), memsize);

            //byte[] bytes = new byte[memsize];

           // byte[] bytes =databytes.ToArray();

            //    Marshal.Copy(dPtr, bytes, 0, bytes.Length);

            // dPtr 020000005465737443616C6C4261636B342CE6B58BE8AF9500000000000000004087694AFB7F0000C086694AFB7F00004086694AFB7F0000
            //Log.Information("C#: OnCallBack4: {data}", bytes);
            Log.Information("C#: OnCallBack4: {data}", databytes.ToArray());

            //read first int id from bytes 0 到sizeof(int)
            //int id = Marshal.ReadInt32(dPtr, 0);

            var index = 0;
            //第一个字段 id

            var part1 = databytes.Slice(index, sizeof(Int32));
            index += sizeof(Int32);

            int id = BitConverter.ToInt32(part1);


            //第二个字段 Name, 长度为 20个字节              


            var part2 = databytes.Slice(index, 20);

            index += 20;

            string name = System.Text.Encoding.UTF8.GetString(part2).TrimEnd('\0');

            // 第三个字段 Data ,类型为  字符串指针  ,数据位置在 sizeof(int)+20

            var part3 = databytes.Slice(index, sizeof(IntPtr));
            index += sizeof(IntPtr);

            var nDataPtr = BitConverter.ToInt64(part3);

            IntPtr dataPrt = new IntPtr(nDataPtr);

            string data = "";

            if (IntPtr.Zero != dataPrt)
            {
                data = Marshal.PtrToStringUTF8(dataPrt);
            }


            //log to output
            Log.Information("C#: OnCallBack4: id:{id},name:{name},dataprt:{dataPtr},Data:{data}", id, name, dataPrt, data);

        }


        private static void DoTestCallBack4()
        {
            Log.Information("测试 TestCallBack4 开始 !");
            NativeLib.TestCallBack4();
            Log.Information("测试 TestCallBack4 完成 !");
        }


        private static void CheckDemo()
        {
            Console.WriteLine($"Check(1,2):{NativeLib.Check(1, 2)}");
            Console.WriteLine($"Check(2,2):{NativeLib.Check(2, 2)}");
            Console.WriteLine($"Check(3,2):{NativeLib.Check(3, 2)}");
        }

        private static void StringDemo()
        {
            var r = NativeLib.GetSlogan("你好,我是C#:!");
            Console.WriteLine(r);
        }


        private static void PrintGCInfo()
        {
            //打印垃圾回收信息

            // Determine the maximum number of generations the system
            // garbage collector currently supports.
            Console.WriteLine("系统支持最高世代 {0}", GC.MaxGeneration);

            // Determine the best available approximation of the number
            // of bytes currently allocated in managed memory.
            Console.WriteLine("当前分配内存: {0}", GC.GetTotalMemory(false));

        }

        private static void StringTest()
        {
            var total = 0;
            Console.WriteLine($"输入每轮调用的次数");
            var line = Console.ReadLine();
            int num;
            int count = 10000;

            if (int.TryParse(line, out num))
            {
                count = num;
            }
            Console.WriteLine($"当前每轮调用 {count} 次, 开始第一轮测试");


            while (!line.Equals("q", StringComparison.CurrentCultureIgnoreCase))
            {
                for (int i = 0; i < count; i++)
                {
                    total++;
                    NativeLib.GetSlogan("你好,我是C#:!");
                }
                PrintGCInfo();
                Console.WriteLine($"调用 {total} 次,输入q 退出,其他继续测试");
                line = Console.ReadLine();
            }
        }
    }
}