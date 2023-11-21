#ifndef MYCALLBACK_H
#define MYCALLBACK_H




// 回调函数1指针
typedef void (*CallBack1)(void*);

// 回调函数2 C字符数组指针
typedef void (*CallBack2)(char*);

// 回调函数3 C结构体指针
typedef struct {
    int id;
    char name[20];
    char *data;
} CallbackEventData;



typedef void (*CallBack3)(CallbackEventData*);

// 定义注册委托函数
extern void RegisterCallBack1(CallBack1 cb);
extern void RegisterCallBack2(CallBack2 cb);
extern void RegisterCallBack3(CallBack3 cb);
extern void RegisterCallBack4(CallBack3 cb);

// 定义调用委托函数
extern void CallCallBack1(void* p);
extern void CallCallBack2(char* p);
extern void CallCallBack3(CallbackEventData* p);
extern void CallCallBack4(CallbackEventData* p);


// 定义全局变量 声明 
extern CallBack1 _cb1;
extern CallBack2 _cb2;
extern CallBack3 _cb3;
extern CallBack3 _cb4;


// 定义日志记录函数声明
extern void Log(char* msg);

// 定义 日志  回调
typedef void (*LogDelegate)(char* msg);

// 定义注册委托函数
extern void RegisterLogDelegate(LogDelegate cb);



#endif