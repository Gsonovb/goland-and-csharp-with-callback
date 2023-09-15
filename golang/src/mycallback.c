
#include <stdio.h>
#include "mycallback.h"


// 定义全局变量
CallBack1 _cb1 = NULL;
CallBack2 _cb2 = NULL;
CallBack3 _cb3 = NULL;

LogDelegate _cbLog = NULL;



// 定义日志记录函数
void Log(char* msg) {
    //printf("C: %s\n", msg);
    if (_cbLog == NULL) {
        printf("C : LogDelegate is null\n");
        return;
    }

    // 调用委托函数
    (*_cbLog)(msg);
    
}


// 定义注册委托函数
void RegisterLogDelegate(LogDelegate cb)
{
    printf("C : OnRegisterLogDelegate \n");
    _cbLog = cb;
    printf("C : OnRegisterLogDelegate end \n");
}



// 定义注册委托函数
void RegisterCallBack1(CallBack1 cb) {
    Log("OnRegisterCallBack1");
    _cb1 = cb;
}

void RegisterCallBack2(CallBack2 cb) {
    Log("OnRegisterCallBack2");
    _cb2 = cb;
}

void RegisterCallBack3(CallBack3 cb) {
    Log("OnRegisterCallBack3");
    _cb3 = cb;
} 

// 定义调用委托函数

void CallCallBack1(void* p) {
    Log("OnCall_CallBack1 start");
    //检测指针是否分配
    if (_cb1 == NULL) {
        Log("CallBack1 is null");
        return;
    }

    //输出指针地址
    char str[100];
    sprintf(str, "OnCall_CallBack1 with p:%p", p);
    Log(str);


    //调用委托函数
    (*_cb1)(p);

    Log("OnCall_CallBack1 end");

}


void CallCallBack2(char* p) {
    Log("OnCall_CallBack2 start");
    //检测指针是否分配
    if (_cb2 == NULL) {
        Log("CallBack2 is null");
        return;
    }

        //输出指针地址
    char str[100];
    sprintf(str, "OnCall_CallBack2 with p:%p", p);
    Log(str);


    //调用委托函数
    (*_cb2)(p);
    Log("OnCall_CallBack2 end"); 
}

void CallCallBack3(CallbackEventData* p) {
    Log("OnCall_CallBack3 start");
    //检测指针是否分配
    if (_cb3 == NULL) {
        Log("CallBack3 is null");
        return;
    }

        //输出指针地址
    char str[100];
    sprintf(str, "OnCall_CallBack3 with p:%p", p);


    Log(str);
    //调用委托函数
    (*_cb3)(p);
    Log("OnCall_CallBack3 end"); 
}