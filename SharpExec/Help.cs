using System;

namespace SharpExec
{
    class Help
    {
        //Help menu
        public void help()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("SharpExec Flags");
            Console.WriteLine("---------------");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Flags                Required    Description");
            Console.WriteLine("-----                --------    -----------");
            Console.WriteLine("-m                   True        Set Module [psexec,wmi,wmiexec,smbexec], -m=psexec");
            Console.WriteLine("-i                   True        Set rhost, -i=192.168.0.10");
            Console.WriteLine("-e                   False       Set execution path, -e=C:\\Windows\\System32\\cmd.exe");
            Console.WriteLine("-d                   False       Set domain, -d=MyDomain");
            Console.WriteLine("-u                   False       Set username, -u=TargetUser");
            Console.WriteLine("-p                   False       Set password, -p=MyP@ssw0rd!");
            Console.WriteLine("-c                   False       Set command arguments, -c=" + "\"" + "All Your Args Here" + "\"");
            Console.WriteLine("-f                   False       Set local file location to upload, -f=" + "\"" + "C:\\Users\\My User\\Desktop\\Evil.exe" + "\"");
            Console.WriteLine("--serviceName        False       Set service name for psexec module, default set to Legit");
            Console.WriteLine("--serviceDisplay     False       Set service display name for psexec module, default set to Totes Legit Service");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Examples - more found on github page");
            Console.WriteLine("-----------------------------------");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Execute remote commands as nt/system user via native psexec functionality - requires admin");
            Console.WriteLine("------------------------------------------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine(@"c:\> SharpExec.exe -m=psexec -i=192.168.0.10 -d=MyDomain -u=MyUser -p=MyP@ssw0rd -e=C:\Windows\System32\cmd.exe -c=" + "\"" + "/c <Powershell Empire Payload>" + "\"");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Execute remote commands via WMI - requires admin");
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine(@"c:\> SharpExec.exe -m=wmi -i=192.168.0.10 -d=MyDomain -u=MyUser -p=MyP@ssw0rd -e=C:\Windows\System32\cmd.exe -c=" + "\"" + "/c <Powershell Empire Payload>" + "\"");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Upload local binary and execute it as nt/system user via native psexec functionality - requires admin");
            Console.WriteLine("-----------------------------------------------------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine(@"c:\> SharpExec.exe -m=psexec -i=192.168.0.10 -d=MyDomain -u=MyUser -p=MyP@ssw0rd -f=" + "\"" + @"C:\Users\My User\Desktop\EvilBinary.exe" + "\"" + @"-e=C:\Users\TargetUser\Desktop\EvilBinary.exe -c=" + "\"" + "My Args Here" + "\"");
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
