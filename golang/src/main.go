package main

/*
#cgo CFLAGS: -I .
#cgo LDFLAGS: -L . -lmycallback

#include <unistd.h>
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <mycallback.h>

typedef struct CallBackEvent{
	char EventId[64];
	char EventName[32];
	char EventTime[64];
	char UserAppId[64];
	char *EventData;
}CallBackEvent;

typedef void (*CallBack)(void*);//函数指针
extern void C_callback(void*);
extern CallBack _cb;  //函数指针的类型

extern void LogMessage_cgo(char* msg); // Forward declaration.

*/
import "C"

import (
	"fmt"
	"io"
	"log"
	"os"
	"time"
	"unsafe"

	"github.com/gin-gonic/gin"
	"gopkg.in/Graylog2/go-gelf.v2/gelf"
)

func main() {
}

// 比较数值
//
//export Check
func Check(i1 int, i2 int) bool {
	return i1 > i2
}

// 生成字符串
//
//export GetSlogan
func GetSlogan(name *C.char) *C.char {

	// 转换成uintptr
	adr := uintptr(unsafe.Pointer(name))
	log.Print("GO: ", "GetSlogan name [in] adr:"+fmt.Sprintf("%x", adr))

	goName := C.GoString(name)
	goSlogan := "这里是是Go，你好!" + goName + "我们一起为武汉加油！"

	res := C.CString(goSlogan)

	adr2 := uintptr(unsafe.Pointer(res))

	log.Print("GO: ", "GetSlogan return  [out] adr:"+fmt.Sprintf("%x", adr2))

	return res
}

//export Free
func Free(p unsafe.Pointer) {
	// 转换成uintptr
	adr := uintptr(p)
	log.Print("GO: ", "OnFree adr:"+fmt.Sprintf("%x", adr))

	C.free(p)
}

// 事件数据
type EventData struct {
	EventId   string `json:"eventId"`   //事件ID
	EventName string `json:"eventName"` //事件名称
	EventTime string `json:"eventTime"` //事件时间
	UserAppId string `json:"userAppId"` //用户应用ID
	EventData string `json:"eventData"` //事件数据
}

type CallbackEventData struct {
	Id   int    `json:"id"`   //id
	Name string `json:"name"` //name
	Data string `json:"data"` //data
}

// 定义全局 Gin1 实例变量
var _gin1 *gin.Engine

// 定义全局 Gin2 实例变量
var _gin2 *gin.Engine

// 定义全局 Gin3 实例变量
var _gin3 *gin.Engine

// 初始化 日志
//
//export InitLog
func InitLog() {
	var logger, err = gelf.NewUDPWriter("localhost:12201")

	if err != nil {
		log.Fatalf("Failed to create UDP writer: %s", err)
	}

	// Don't prefix messages with a redundant timestamp etc.
	log.SetFlags(0)

	log.SetOutput(io.MultiWriter(os.Stdout, logger))

	log.Print("GO: ", "Init Server!")

	C.RegisterLogDelegate((C.LogDelegate)(unsafe.Pointer(C.LogMessage_cgo)))

}

//export LogMessage
func LogMessage(msg *C.char) {
	// 转换成Go字符串
	goMsg := C.GoString(msg)
	log.Print("C : ", goMsg)

}

// 初始化 回调函数1 并设置 Gin1 实例 和端口
//
//export InitCallBack1
func InitCallBack1(cb unsafe.Pointer, port int) {
	// log with port
	log.Print("GO: ", "InitCallBack1 port:"+fmt.Sprintf(":%d", port))

	if cb == nil {
		log.Print("GO: ", "Set cb prt  is nil !!")
		return
	} else {
		// 转换成uintptr
		adr := uintptr(cb)
		log.Print("GO: ", "Set cb prt  is not nil !! adr:"+fmt.Sprintf("%x", adr))

		C.RegisterCallBack1(C.CallBack1(cb))
	}

	// _gin1 = gin.Default()
	// _gin1.GET("/callback1", func(c *gin.Context) {
	// 	log.Print("GO: ","GET callback1 is Called")

	// 	c.JSON(200, gin.H{
	// 		"message": "callback1 is Called",
	// 	})
	// })

	// // 定义一个 POST 请求的路由 , 并获取BODY文本数据,并传入回调函数
	// _gin1.POST("/callback1", func(c *gin.Context) {

	// 	//check body is not null
	// 	if c.Request.Body == nil {
	// 		c.JSON(400, gin.H{
	// 			"message": "no body Request",
	// 		})
	// 		return
	// 	}

	// 	var eventData EventData

	// 	if c.ShouldBind(&eventData) != nil {
	// 		c.JSON(400, gin.H{
	// 			"message": "Bad Request error data",
	// 		})
	// 		return
	// 	}

	// 	jsonData, err := json.Marshal(eventData)
	// 	if err != nil {
	// 		c.JSON(400, gin.H{
	// 			"message": "Bad context r data",
	// 		})
	// 		return
	// 	}

	// 	json := string(jsonData)

	// 	//输出日志
	// 	log.Print("GO: ","POST callback1 is Called, data:" + json)

	// 	// 返回响应
	// 	c.JSON(200, gin.H{
	// 		"message": "OK",
	// 		"body":    json,
	// 	})

	// 	runCallBackFunc := func(json string) {

	// 		// 检查 _cb1 是否为空
	// 		if _cb1 == nil {
	// 			log.Print("GO: ","CallBack1 is nil")
	// 			return
	// 		}

	// 		var pdata = unsafe.Pointer(C.CString(json))

	// 		_cb1(pdata)

	// 		defer func() {

	// 			if err := recover(); err != nil {
	// 				log.Print("GO: ","CallBack1 error:", err)
	// 			}
	// 		}()
	// 	}

	// 	go runCallBackFunc(json)

	// })

	// go _gin1.Run(fmt.Sprintf(":%d", port))
}

// 发送回调函数1
func SendCallBack1(json string) {

	log.Print("GO: ", "SendCallBack1:  start  ", json)

	// 获取 字符串 C++ 指针
	var pdata = unsafe.Pointer(C.CString(json))
	log.Print("GO: ", "SendCallBack1: Get string Pointer ", fmt.Sprintf("%x", uintptr(pdata)))

	// 调用回调函数
	C.CallCallBack1(pdata)

	log.Print("GO: ", "SendCallBack1: Done	")

	defer func() {

		if err := recover(); err != nil {
			log.Print("GO: ", "SendCallBack1 error:", err)
		}
	}()
}

// 测试回调函数
//
//export TestCallBack1
func TestCallBack1() {

	log.Print("GO: ", "TestCallBack1 Start!")

	var str = "This is test, from TestCallBack1! "
	var str2 = " 这是来自 TestCallBack1 的测试！ "

	log.Print("GO: ", "TestCallBack1: Send Text :"+str)
	SendCallBack1(str)

	// 休眠3秒
	time.Sleep(3 * time.Second)

	log.Print("GO: ", "TestCallBack1: Send Text :"+str2)
	SendCallBack1(str2)

	time.Sleep(3 * time.Second)

	var str3 = str + "\n" + str2

	log.Print("GO: ", "TestCallBack1: Send Text :"+str3)
	SendCallBack1(str3)

	log.Print("GO: ", "TestCallBack1 End!")

}

// 初始化 回调函数2 并设置 Gin2 实例 和端口
//
//export InitCallBack2
func InitCallBack2(cb unsafe.Pointer, port int) {
	// log with port
	log.Print("GO: ", "InitCallBack2 port:"+fmt.Sprintf(":%d", port))

	if cb == nil {
		log.Print("GO: ", "Set cb prt  is nil !!")
		return
	} else {
		// 转换成uintptr
		adr := uintptr(cb)
		log.Print("GO: ", "Set cb prt  is not nil !! adr:"+fmt.Sprintf("%x", adr))

		C.RegisterCallBack2(C.CallBack2(cb))
	}

	// _gin2 = gin.Default()
	// _gin2.GET("/callback2", func(c *gin.Context) {
	// 	log.Print("GO: ", "GET callback2 is Called")

	// 	c.JSON(200, gin.H{
	// 		"message": "callback2 is Called",
	// 	})
	// })

	// // 定义一个 POST 请求的路由 , 并获取BODY文本数据,并传入回调函数
	// _gin2.POST("/callback2", func(c *gin.Context) {

	// 	//check body is not null
	// 	if c.Request.Body == nil {
	// 		c.JSON(400, gin.H{
	// 			"message": "no body Request",
	// 		})
	// 		return
	// 	}

	// 	//获取请求内容
	// 	var body string
	// 	c.Bind(&body)

	// 	//log +  body
	// 	log.Print("GO: ", "POST callback2 is Called,"+body)

	// 	// 调用回调函数
	// 	//	go _cb2(C.CString(body))

	// 	// 返回响应
	// 	c.JSON(200, gin.H{
	// 		"message": "OK",
	// 		"body":    body,
	// 	})
	// })

	// go _gin2.Run(fmt.Sprintf(":%d", port))
}

// 发送回调函数2
func SendCallBack2(json string) {

	log.Print("GO: ", "SendCallBack2:  start  ", json)

	// 获取 字符串 C++ 指针

	var pdata = C.CString(json)
	log.Print("GO: ", "SendCallBack2: use *C.char ")

	// 调用回调函数
	C.CallCallBack2(pdata)

	log.Print("GO: ", "SendCallBack2: Done	")

	defer func() {

		if err := recover(); err != nil {
			log.Print("GO: ", "SendCallBack2 error:", err)
		}
	}()
}

// 测试回调函数2
//
//export TestCallBack2
func TestCallBack2() {

	log.Print("GO: ", "TestCallBack2 Start!")

	var str = "This is test, from TestCallBack2!"
	var str2 = " 这是来自 TestCallBack2 的测试！"
	var str3 = str + "\n" + str2

	log.Print("GO: ", "TestCallBack2: Send Text :"+str)
	SendCallBack2(str)
	time.Sleep(3 * time.Second)

	SendCallBack2(str2)
	log.Print("GO: ", "TestCallBack2: Send Text :"+str2)

	time.Sleep(3 * time.Second)

	SendCallBack2(str3)
	log.Print("GO: ", "TestCallBack2: Send Text :"+str3)

	log.Print("GO: ", "TestCallBack2 End!")
}

// 初始化 回调函数3 并设置 Gin3 实例 和端口
//
//export InitCallBack3
func InitCallBack3(cb unsafe.Pointer, port int) {

	// log with port
	log.Print("GO: ", "InitCallBack3 port:"+fmt.Sprintf(":%d", port))

	if cb == nil {
		log.Print("GO: ", "Set cb prt  is nil !!")
		return
	} else {
		// 转换成uintptr
		adr := uintptr(cb)
		log.Print("GO: ", "Set cb prt  is not nil !! adr:"+fmt.Sprintf("%x", adr))

		C.RegisterCallBack3(C.CallBack3(cb))
	}

	//_cb3 = cb
	// _gin3 = gin.Default()
	// _gin3.GET("/callback3", func(c *gin.Context) {
	// 	log.Print("GO: ", "GET callback3 is Called")

	// 	c.JSON(200, gin.H{
	// 		"message": "callback3 is Called",
	// 	})
	// })

	// // 定义一个 POST 请求的路由 , 并获取BODY文本数据,并传入回调函数
	// _gin3.POST("/callback3", func(c *gin.Context) {

	// 	//check body is not null
	// 	if c.Request.Body == nil {
	// 		c.JSON(400, gin.H{
	// 			"message": "no body Request",
	// 		})
	// 		return
	// 	}

	// 	//获取请求内容
	// 	var event EventData
	// 	err := c.BindJSON(&event)

	// 	if err != nil {
	// 		c.JSON(400, gin.H{
	// 			"message": "Bad Request error data",
	// 		})
	// 		return
	// 	}

	// 	//log + event
	// 	log.Print("GO: ", "POST callback3 is Called,"+event.EventName+",data:"+event.EventData)

	// 	// // C结构体
	// 	// var cEvent C.CallBackEvent

	// 	// GoStringToCChar(event.EventId, &cEvent.EventId[0], 64)
	// 	// GoStringToCChar(event.EventName, &cEvent.EventName[0], 32)
	// 	// GoStringToCChar(event.EventTime, &cEvent.EventTime[0], 64)
	// 	// GoStringToCChar(event.UserAppId, &cEvent.UserAppId[0], 64)

	// 	// cEvent.EventData = C.CString(event.EventData)

	// 	// 创建指针
	// 	//var pEvent = &cEvent

	// 	// 调用回调函数
	// 	//go _cb3(pEvent)

	// 	// 返回响应
	// 	c.JSON(200, gin.H{
	// 		"message": "OK",
	// 	})
	// })

	// go _gin3.Run(fmt.Sprintf(":%d", port))
}

// 发送回调函数3
func SendCallBack3(event CallbackEventData) {

	log.Print("GO: ", "SendCallBack3:  start  ")

	// C结构体
	var cEvent C.CallbackEventData

	cEvent.id = C.int(event.Id)

	GoStringToCChar(event.Name, &cEvent.name[0], 20)

	// 创建指针
	var pEvent = &cEvent

	// 调用回调函数
	C.CallCallBack3(pEvent)

	log.Print("GO: ", "SendCallBack3: Done	")

	defer func() {

		if err := recover(); err != nil {
			log.Print("GO: ", "SendCallBack3 error:", err)
		}
	}()
}

// 测试回调函数3
//
//export TestCallBack3
func TestCallBack3() {

	log.Print("GO: ", "TestCallBack3 Start!")

	var event CallbackEventData
	event.Id = 1
	event.Name = "TestCallBack3,测试回调3"
	event.Data = "这是来自 TestCallBack3 的测试！"

	SendCallBack3(event)

	log.Print("GO: ", "TestCallBack3 End!")
}

// 定义函数 将Go字符串转换成C固定长度字符数组
func GoStringToCChar(str string, p *C.char, len int) {
	// 转换成C++ string
	cstr := C.CString(str)
	defer C.free(unsafe.Pointer(cstr))

	// 拷贝到C++ 固定长度字符数组
	C.strncpy(p, cstr, C.size_t(len))

}
