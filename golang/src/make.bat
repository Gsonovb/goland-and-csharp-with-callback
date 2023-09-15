REM 在Windows上编译需要安装go 和 msys2, 如果要编译32位版本,需要安装mingw32 686的编译工具链,并将 mingw32\bin设置到Path 路径中
set GOARCH=amd64
set CGO_ENABLED=1


gcc -c mycallback.c -o mycallback.o
ar cr libmycallback.a mycallback.o


@REM go build -ldflags "-s -w" -buildmode=c-shared -o Golang.Ioc.Interop.dll main.go


go build -ldflags "-s -w" -buildmode=c-shared -o Golang.Ioc.Interop.dll 