using System;
using System.IO;
using System.Management;

namespace SharpExec
{
    class ManageProcess
    {
        //WMI execute file or command
        public void wmi(string rhost, string executionPath, string cmdArgs, string domain, string username, string password)
        {
            object[] myProcess = { executionPath + " " + cmdArgs };

            if (username == "" && password == "")
            {
                ManagementScope myScope = new ManagementScope(String.Format("\\\\{0}\\root\\cimv2", rhost));
                ManagementClass myClass = new ManagementClass(myScope, new ManagementPath("Win32_Process"), new ObjectGetOptions());
                myClass.InvokeMethod("Create", myProcess);
                Console.WriteLine();
                Console.WriteLine("[+] Process created successfully");
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
                ManagementScope myScope = new ManagementScope(String.Format("\\\\{0}\\root\\cimv2", rhost), myConnection);
                ManagementClass myClass = new ManagementClass(myScope, new ManagementPath("Win32_Process"), new ObjectGetOptions());
                myClass.InvokeMethod("Create", myProcess);
                Console.WriteLine();
                Console.WriteLine("[+] Process created successfully");
            }
        }
        //Handles WMI semi-interactive shell
        public void wmiexec(string rhost, string executionPath, string cmdArgs, string domain, string username, string password)
        {
            Console.WriteLine();
            Console.WriteLine("[+] Using WMIExec module semi-interactive shell");
            Console.WriteLine("[+] Be careful what you execute");
            Console.WriteLine();           
            string pwd = @"C:\";
            string ln1 = "";

            if (username == "" && password == "")
            {
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
                            string remotePath;
                            if (pwd == @"C:\")
                            {
                                remotePath = pwd + put[2];
                            }
                            else
                            {
                                remotePath = pwd + @"\" + put[2];
                            }
                            FileAddRemove uploadFile = new FileAddRemove();
                            uploadFile.upload(localPath, remotePath, rhost, username, password, domain);
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
                            string remotePath;
                            if (pwd == @"C:\")
                            {
                                remotePath = pwd + put[1];
                            }
                            else
                            {
                                remotePath = pwd + @"\" + put[1];
                            }
                            FileAddRemove uploadFile = new FileAddRemove();
                            uploadFile.get(localPath, remotePath, rhost, username, password, domain);
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
                        Console.WriteLine("put                  Upload file from local directory to current shell directory, put fullLocalPath\\File.txt File.txt");
                        Console.WriteLine("get                  Download file from current shell directory to local directory, get File.txt fullLocalPath\\File.txt");
                        Console.WriteLine("help                 Show help menu");
                        Console.WriteLine("exit                 Exit shell");
                    }
                    else
                    {
                        ManagementScope myScope = new ManagementScope(String.Format("\\\\{0}\\root\\cimv2", rhost));
                        ManagementClass myClass = new ManagementClass(myScope, new ManagementPath("Win32_Process"), new ObjectGetOptions());
                        ManagementBaseObject myParams = myClass.GetMethodParameters("Create");
                        myParams["CurrentDirectory"] = pwd;
                        myParams["CommandLine"] = @"cmd /Q /c " + cmdArgs + @" > C:\__LegitFile 2>&1";
                        myClass.InvokeMethod("Create", myParams, null);

                        //Allows enough time to go elapse so output can be read
                        System.Threading.Thread.Sleep(2000);

                        //Handles reading output
                        string output = @"\\" + rhost + @"\C$\__LegitFile";
                        if (File.Exists(output))
                        {
                            using (StreamReader file = new StreamReader(output))
                            {
                                int counter = 0;
                                string ln;

                                //Reads output file
                                while ((ln = file.ReadLine()) != null)
                                {
                                    //Helps handle bad path
                                    if (ln.Contains("The system cannot find the path specified."))
                                    {
                                        ln1 = ln;

                                    }
                                    Console.WriteLine();
                                    Console.WriteLine(ln);
                                    counter++;
                                }
                                file.Close();
                                File.Delete(output);
                            }
                        }//End if file exits                    

                        //Handles changing directories
                        if (cmdArgs.ToLower().Contains("cd"))
                        {
                            //Handles if bad directory 
                            if (ln1.Contains("The system cannot find the path specified."))
                            {
                                ln1 = "";
                            }
                            else
                            {
                                /*Handles switching to full path - cd C:\Users\ATTE
                                Else handles new directory - cd Users\ATTE */
                                if (cmdArgs.ToLower().Contains(":"))
                                {
                                    pwd = cmdArgs.Split(' ')[1];
                                }
                                else
                                {
                                    string pwdOutput = pwd + @">";
                                    if (pwdOutput.Contains(@":\>"))
                                    {
                                        pwd = pwdOutput.Replace(">", cmdArgs.Split(' ')[1]);
                                    }
                                    else if (cmdArgs != "cd ..")
                                    {

                                        pwd = pwd + @">";
                                        pwd = pwd.Replace(">", @"\") + cmdArgs.Split(' ')[1];
                                    }
                                }
                            }

                            //Handles cd .. functionality
                            if (cmdArgs.ToLower().Contains(".."))
                            {
                                string input = pwd;
                                string backslash = @"\";

                                int index = input.LastIndexOf(@backslash);

                                if (index > 0)
                                {
                                    pwd = input.Substring(0, index);

                                    if (pwd == "C:")
                                    {
                                        pwd = @"C:\";
                                    }

                                }
                                else
                                {
                                    pwd = @"C:\";
                                }
                            }

                        }//End if cmdArgs contain cd
                    }
                            Console.WriteLine();
                            Console.Write(pwd + @">");                  
                            cmdArgs = Console.ReadLine();
                }
            }
            else
            {
                while (cmdArgs.ToLower() != "exit")
                {
                    //Handles uploading file to current remote directory
                    if (cmdArgs.ToLower().Contains("put "))
                    {
                        try                        { 

                            Char delimiter = ' ';
                            String[] put = cmdArgs.Split(delimiter);
                            string localPath = put[1];
                            string remotePath;
                            if (pwd == @"C:\")
                            {
                                remotePath = pwd + put[2];
                            }
                            else
                            {
                                remotePath = pwd + @"\" + put[2];
                            }
                            FileAddRemove uploadFile = new FileAddRemove();
                            uploadFile.upload(localPath, remotePath, rhost, username, password, domain);
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
                            string remotePath;
                            if (pwd == @"C:\")
                            {
                                remotePath = pwd + put[1];
                            }
                            else
                            {
                                remotePath = pwd + @"\" + put[1];
                            }
                            FileAddRemove uploadFile = new FileAddRemove();
                            uploadFile.get(localPath, remotePath, rhost, username, password, domain);
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
                        Console.WriteLine("put                  Upload file from local directory to current shell directory, put fullLocalPath\\File.txt File.txt");
                        Console.WriteLine("get                  Download file from current shell directory to local directory, get File.txt fullLocalPath\\File.txt");
                        Console.WriteLine("help                 Show help menu");
                        Console.WriteLine("exit                 Exit shell");
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
                    ManagementScope myScope = new ManagementScope(String.Format("\\\\{0}\\root\\cimv2", rhost), myConnection);
                    ManagementClass myClass = new ManagementClass(myScope, new ManagementPath("Win32_Process"), new ObjectGetOptions());
                    ManagementBaseObject myParams = myClass.GetMethodParameters("Create");
                    myParams["CurrentDirectory"] = pwd;
                    myParams["CommandLine"] = @"cmd /Q /c " + cmdArgs + @" > C:\__LegitFile 2>&1";
                    myClass.InvokeMethod("Create", myParams, null);

                    //Allows enough time to go elapse so output can be read
                    System.Threading.Thread.Sleep(2000);

                    using (new Impersonation(domain, username, password))
                    {
                        //Handles reading output
                        string output = @"\\" + rhost + @"\C$\__LegitFile";
                        if (File.Exists(output))
                        {
                            using (StreamReader file = new StreamReader(output))
                            {
                                int counter = 0;
                                string ln;

                                //Reads output file
                                while ((ln = file.ReadLine()) != null)
                                {
                                    //Helps handle bad path
                                    if (ln.Contains("The system cannot find the path specified."))
                                    {
                                        ln1 = ln;

                                    }
                                    Console.WriteLine();
                                    Console.WriteLine(ln);
                                    counter++;
                                }
                                file.Close();
                                File.Delete(output);
                            }
                        }//End if file exits
                    }//end impersonation

                    //Handles changing directories
                    if (cmdArgs.ToLower().Contains("cd"))
                    {
                        //Handles if bad directory 
                        if (ln1.Contains("The system cannot find the path specified."))
                        {
                            ln1 = "";
                        }
                        else
                        {
                            /*Handles switching to full path - cd C:\Users\ATTE
                            Else handles new directory - cd Users\ATTE */
                            if (cmdArgs.ToLower().Contains(":"))
                            {
                                pwd = cmdArgs.Split(' ')[1];
                            }
                            else
                            {
                                string pwdOutput = pwd + @">";
                                if (pwdOutput.Contains(@":\>"))
                                {
                                    pwd = pwdOutput.Replace(">", cmdArgs.Split(' ')[1]);
                                }
                                else if (cmdArgs != "cd ..")
                                {

                                    pwd = pwd + @">";
                                    pwd = pwd.Replace(">", @"\") + cmdArgs.Split(' ')[1];
                                }
                            }
                        }

                        //Handles cd .. functionality
                        if (cmdArgs.ToLower().Contains(".."))
                        {
                            string input = pwd;
                            string backslash = @"\";

                            int index = input.LastIndexOf(@backslash);

                            if (index > 0)
                            {
                                pwd = input.Substring(0, index);

                                if (pwd == "C:")
                                {
                                    pwd = @"C:\";
                                }

                            }
                            else
                            {
                                pwd = @"C:\";
                            }
                        }
                    }//End if cmdArgs contain cd
                }
                    Console.WriteLine();
                    Console.Write(pwd + @">");
                    cmdArgs = Console.ReadLine();
               
                }               
            }

        }
    }
}
