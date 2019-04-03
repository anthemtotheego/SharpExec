using System;
using System.IO;

namespace SharpExec
{
    class FileAddRemove
    {
        public void upload(string uploadPath, string executionPath, string rhost, string username, string password, string domain)
        {
            try
            {
                if (username == "" && password == "")
                {
                    string share = executionPath.Replace(":", "$");
                    string destpath = @"\\" + rhost + @"\" + share;
                    Console.WriteLine("[+] Grabbing file from " + uploadPath);
                    Console.WriteLine();
                    Console.WriteLine("[+] Uploading to " + destpath);
                    File.Copy(uploadPath, destpath);
                    Console.WriteLine("[+] File uploaded successfully");
                }
                else
                {
                    using (new Impersonation(domain, username, password))
                    {
                        string share = executionPath.Replace(":", "$");
                        string destpath = @"\\" + rhost + @"\" + share;
                        Console.WriteLine("[+] Grabbing file from " + uploadPath);
                        Console.WriteLine();
                        Console.WriteLine("[+] Uploading to " + destpath);
                        File.Copy(uploadPath, destpath);
                        Console.WriteLine("[+] File uploaded successfully");
                    }
                }
            }
            catch
            {
                Console.WriteLine("[-] File upload failed, check to make sure both directory paths and credentials are correct");
            }
        }
        //File Removal Method
        public void delete(string executionPath, string rhost, string username, string password, string domain)
        {
            try
            {                
                if (username == "" && password == "")
                {
                    string share = executionPath.Replace(":", "$");
                    string destpath = @"\\" + rhost + @"\" + share;
                    Console.WriteLine("[+] Deleting " + destpath);
                    File.Delete(destpath);
                    Console.WriteLine("[+] File removed successfully");
                    Console.WriteLine();
                }
                else
                {
                    using (new Impersonation(domain, username, password))
                    {
                        string share = executionPath.Replace(":", "$");
                        string destpath = @"\\" + rhost + @"\" + share;
                        Console.WriteLine("[+] Deleting " + destpath);
                        File.Delete(destpath);
                        Console.WriteLine("[+] File removed successfully");
                        Console.WriteLine();
                    }
                }
            }
            catch
            {
                Console.WriteLine("[-] File was not removed.  Please remove manually");
            }
        }

        public void get(string uploadPath, string executionPath, string rhost, string username, string password, string domain)
        {
            try
            {
                if (username == "" && password == "")
                {
                    string share = executionPath.Replace(":", "$");
                    string destpath = @"\\" + rhost + @"\" + share;
                    Console.WriteLine("[+] Grabbing file from " + destpath);
                    Console.WriteLine();
                    Console.WriteLine("[+] Download to " + uploadPath);
                    File.Copy(destpath, uploadPath);
                    Console.WriteLine("[+] File downloaded successfully");
                }
                else
                {
                    using (new Impersonation(domain, username, password))
                    {                        
                        string share = executionPath.Replace(":", "$");
                        string destpath = @"\\" + rhost + @"\" + share;
                        Console.WriteLine("[+] Grabbing file from " + destpath);
                        Console.WriteLine();
                        Console.WriteLine("[+] Download to " + uploadPath);
                        File.Copy(destpath, uploadPath);
                        Console.WriteLine("[+] File downloaded successfully");
                    }
                }
            }
            catch
            {
                Console.WriteLine("[-] File upload failed, check to make sure both directory paths and credentials are correct");
            }
        }
    }
}
