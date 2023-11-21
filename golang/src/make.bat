REM 在Windows上编译需要安装go 和 msys2, 如果要编译32位版本,需要安装mingw32 686的编译工具链,并将 mingw32\bin设置到Path 路径中
set GOARCH=amd64
set CGO_ENABLED=1


gcc -g -c mycallback.c -o mycallback.o
ar cr libmycallback.a mycallback.o


@REM go build -ldflags "-s -w" -buildmode=c-shared -o Golang.Ioc.Interop.dll main.go


@REM go build -ldflags "-s -w" -g -buildmode=c-shared -o Golang.Ioc.Interop.dll 
@REM go build -ldflags "-s -w" -gcflags "-N -l" -buildmode=c-shared -o Golang.Ioc.Interop.dll
go build  -gcflags "-N -l" -buildmode=c-shared -o Golang.Ioc.Interop.dll

cv2pdb64 Golang.Ioc.Interop.dll