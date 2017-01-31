using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Net.Sockets;
using System.Reflection;

namespace Winwoof
{
    public class Winwoof
    {
        private readonly string filepath;
        private readonly string localAddress;
        private readonly int port = 1337;
        private readonly HttpListener httpListener = new HttpListener();

        public Winwoof(string[] args)
        {
            // setup some things
            localAddress = GetLocalAddress();

            // PrintStartupMessage with usage hint in case of malformed input
            switch (args.Length)
            {
                case 0:
                    PrintStartupMessage(true);
                    Environment.Exit(0);
                    break;

                case 1:
                    // start processing
                    break;

                case 2:
                    try
                    {
                        port = int.Parse(args[1]);
                    }
                    catch ( Exception e )
                    {
                        ErrorHandler("The Port must be a number!");
                    }
                    break;

                default:
                    PrintStartupMessage(true);
                    ErrorHandler("Wrong number of parameters!");
                    break;
            }
            
            // get file or dir
            filepath = GetAbsolutePath(args[0]);

            PrintStartupMessage(false);
        }

        private static void Main(string[] args)
        {
            Winwoof winwoof = new Winwoof(args);
            winwoof.ServeStart();
            Console.ReadKey();
            winwoof.ServeStop();
        }

        private string DirectoryHandler(string dir)
        {
            string tempZipPath = Path.GetTempPath() + @"woof.zip";

            if (File.Exists(tempZipPath))
            {
                File.Delete(tempZipPath);
            }

            ZipFile.CreateFromDirectory(dir, tempZipPath);

            return tempZipPath;
        }

        private void ErrorHandler(string error)
        {
            Console.WriteLine(error);
            Environment.Exit(0);
        }

        private string FileHandler(string file)
        {
            string path;

            if (file.Contains("\\") | file.Contains("/"))
            {
                // absolute path given
                path = file;
            }
            else
            {
                // filename given, get current path
                path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\" + file;
            }

            return path;
        }

        private string GetAbsolutePath(string arg)
        {
            if (Directory.Exists(arg))
            {
                return DirectoryHandler(arg);
            }

            if (File.Exists(arg))
            {
                return FileHandler(arg);
            }

            ErrorHandler("Can't open File or directory!");

            return string.Empty;
        }

        private string GetLocalAddress()
        {
            IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;

            if (addressList == null || !addressList.Any())
            {
                ErrorHandler("Could not determine local IP address!");
            }

            return addressList.First(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString();
        }

        private void PrintStartupMessage(bool showUsage)
        {
            Console.WriteLine(@"            _                           __ ");
            Console.WriteLine(@"           (_)                         / _|");
            Console.WriteLine(@"  __      ___ _ ____      _____   ___ | |_ ");
            Console.WriteLine(@"  \ \ /\ / / | '_ \ \ /\ / / _ \ / _ \|  _|");
            Console.WriteLine(@"   \ V  V /| | | | \ V  V / (_) | (_) | |  ");
            Console.WriteLine(@"    \_/\_/ |_|_| |_|\_/\_/ \___/ \___/|_|  ");

            Console.WriteLine("\n----------------------------------------------");

            Console.WriteLine(string.Empty);

            Console.WriteLine(" winwoof creates a small and simple webserver \n that can be used to share files or folders \n easily with people on the same network.");

            Console.WriteLine("\n (c) 2017 rbeier\n     https://github.com/rbeier/winwoof");

            if (showUsage)
            {
                Console.WriteLine("\n------------------- Usage -------------------- \n");
                Console.WriteLine("      winwoof <file|directory> [port]");
            }
        }

        private void ServeRequestAsync(IAsyncResult result)
        {
            HttpListener listener = result.AsyncState as HttpListener;

            if (listener == null)
            {
                Console.WriteLine("Error processing request!");
            }
            else
            {
                HttpListenerContext context = listener.EndGetContext(result);

                // start listening for request again
                listener.BeginGetContext(ServeRequestAsync, listener);

                SendFile(context);
            }
        }

        private void ServeStart()
        {
            // start listener
            httpListener.Prefixes.Add($"http://{localAddress}:{port}/");
            httpListener.Start();

            Console.WriteLine($"\nServer is now listening on: http://{localAddress}:{port}/\n");

            httpListener.BeginGetContext(ServeRequestAsync, httpListener);
        }

        private void ServeStop()
        {
            httpListener.Stop();
        }

        private void SendFile(HttpListenerContext listenerContext)
        {
            IPEndPoint requestEndPoint = listenerContext.Request.RemoteEndPoint;

            if (requestEndPoint != null)
            {
                Console.WriteLine($"Serving request from: {requestEndPoint.Address}");
            }

            HttpListenerResponse response = listenerContext.Response;

            using (FileStream fileStream = File.OpenRead(filepath))
            {
                response.AddHeader("Content-disposition", $"attachment; filename={Path.GetFileName(filepath)}");
                response.ContentLength64 = fileStream.Length;
                response.SendChunked = false;
                response.ContentType = MediaTypeNames.Application.Octet;

                byte[] buffer = new byte[64 * 1024];

                using (BinaryWriter binaryWriter = new BinaryWriter(response.OutputStream))
                {
                    int read;
                    while ((read = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        binaryWriter.Write(buffer, 0, read);
                        binaryWriter.Flush();
                    }

                    binaryWriter.Close();
                }

                response.StatusCode = (int)HttpStatusCode.OK;
                response.StatusDescription = "OK";
                response.OutputStream.Close();
            }
        }
    }
}