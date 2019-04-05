using System;
using System.Collections.Generic;

/*
Author: @anthemtotheego
License: BSD 3-Clause    
*/

namespace SharpExec
{
    class Program
    {
        static void Main(string[] args)
        {
            //Display help menu
            if (args.Length <= 0 || args[0] == "help" || args[0] == "?")
            {
                Help please = new Help();
                please.help();
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
                bool fileUploaded = false;

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
                    fileUploaded = true;
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
                //Upload file
                if (uploadPath != "" && executionPath != "" && domain != "" && username != "" && password != "")
                {
                    using (new Impersonation(domain, username, password))
                    {
                        try
                        {
                            FileAddRemove uploadFile = new FileAddRemove();
                            uploadFile.upload(uploadPath, executionPath, rhost, username, password, domain);
                            System.Threading.Thread.Sleep(3000);
                        }
                        catch
                        {
                            Console.WriteLine("[-] Something went wrong with file upload.  Please check syntax and run again");
                        }
                    }                    
                }
                else if (uploadPath != "" && executionPath != "")
                {
                    try
                    {
                        FileAddRemove uploadFile = new FileAddRemove();
                        uploadFile.upload(uploadPath, executionPath, rhost, username, password, domain);
                        System.Threading.Thread.Sleep(3000);
                    }
                    catch
                    {
                        Console.WriteLine("[-] Something went wrong with file upload.  Please check syntax and run again");
                    }
                }
                //PSExec
                if (module.ToLower() == "psexec")
                {
                    try
                    {
                        ManageService create = new ManageService();
                        create.PSExec(rhost, serviceName, serviceDisplayName, executionPath, cmdArgs, domain, username, password, fileUploaded);
                        //If file was uploaded remove it
                        if (uploadPath != "" && executionPath != "")
                        {
                            Console.WriteLine("[+] Sometimes, depending on what was executed your process/file may still be open");
                            Console.WriteLine("[+] Make sure process is not still running then press ENTER to remove file");
                            Console.ReadLine();
                            FileAddRemove deleteFile = new FileAddRemove();
                            deleteFile.delete(executionPath, rhost, username, password, domain);
                        }
                    }
                    catch
                    {
                        Console.WriteLine("[-] Something went wrong with PSEXEC module.  Please check syntax and run again.");
                    }
                }
                //WMI
                else if (module.ToLower() == "wmi")
                {
                    try
                    {
                        ManageProcess create = new ManageProcess();
                        create.wmi(rhost, executionPath, cmdArgs, domain, username, password);

                        //If file was uploaded remove it
                        if (uploadPath != "" && executionPath != "")
                        {
                            Console.WriteLine("[+] Sometimes, depending on what was executed your process/file may still be open");
                            Console.WriteLine("[+] Make sure process is not still running then press ENTER to remove file");
                            Console.ReadLine();
                            //Sleep allows time for execution
                            System.Threading.Thread.Sleep(2000);
                            FileAddRemove deleteFile = new FileAddRemove();
                            deleteFile.delete(executionPath, rhost, username, password, domain);
                        }
                    }
                    catch
                    {
                        Console.WriteLine("[-] Something went wrong with WMI module.  Please check syntax and run again.");
                    }
                }
                //WMIExec
                else if (module.ToLower() == "wmiexec")
                {
                    try
                    {
                        ManageProcess create = new ManageProcess();
                        create.wmiexec(rhost, executionPath, cmdArgs, domain, username, password);
                    }
                    catch
                    {
                      Console.WriteLine("[-] Something went wrong with WMIExec module.  Please check syntax and run again.");
                    }
                }
                //SMBEXEC
                else if (module.ToLower() == "smbexec")
                {
                    try
                    {
                        ManageService create = new ManageService();
                        create.smbExec(rhost, serviceName, serviceDisplayName, cmdArgs, domain, username, password);
                    }
                    catch
                    {
                        Console.WriteLine("[-] Something went wrong with SMBExec module.  Please check syntax and run again.");
                    }
                }
                else
                {
                    Console.WriteLine("[-] Something went wrong.  Please check syntax and run again.");
                }
            }
        }//End Main  
    }//End Program
}//End Namespace
