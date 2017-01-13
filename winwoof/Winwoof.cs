using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

// todo: file already exists C:\temp\woof.zip

class Winwoof
{

    private string filepath;
    private string localAddress;
    private int    port = 1337;

    HttpListener   listener = new HttpListener();


    public Winwoof(string[] args)
    {

        // setup some things
        this.localAddress = this.getLocalAddress();

        // print help text or change port number
        switch( args.Length )
        {

            case 0:
                this.printHelpText();
                break;

            case 1:
                // start processing
                break;

            case 2:
                this.port = Int32.Parse(args[1]);
                break;

            default:
                this.errorHandler("Wrong number of parameters!");
                break;

        }

        // get file or dir
        this.filepath = this.getAbsolutePath(args[0]);

        // start listener
        listener.Prefixes.Add("http://" + this.localAddress + ":" + this.port + "/");
        listener.Start();

    }


    private string getAbsolutePath(string arg)
    {

        if ( Directory.Exists(arg) )
            return this.directoryHandler(arg);

        if ( File.Exists(arg) )
            return this.fileHandler(arg);

        this.errorHandler("Can't open File or directory!");

        return "";

    }
    

    private string fileHandler(string file)
    {

        string path = "";

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


    public string directoryHandler(string dir)
    {

        string tempZipPath = Path.GetTempPath() + @"woof.zip";

        if (File.Exists(tempZipPath))
            File.Delete(tempZipPath);

        System.IO.Compression.ZipFile.CreateFromDirectory(dir, tempZipPath);

        return tempZipPath;

    }


    private void serve()
    {

        Console.Write("Server is listening on: http://" + this.localAddress + ":" + this.port + "/");

        Task.Factory.StartNew(() =>
        {

            while ( ! Console.KeyAvailable )
            {

                HttpListenerContext context = listener.GetContext();

                Task.Factory.StartNew((ctx) =>
                {

                    WriteFile( (HttpListenerContext) ctx );

                }, context, TaskCreationOptions.LongRunning);

            }

            listener.Stop();

        }, TaskCreationOptions.LongRunning);

        Console.ReadKey();

    }


    private void WriteFile(HttpListenerContext ctx)
    {

        var response = ctx.Response;

        using ( FileStream fs = File.OpenRead(this.filepath) )
        {

            response.AddHeader( "Content-disposition", "attachment; filename=" + Path.GetFileName(this.filepath) );
            response.ContentLength64 = fs.Length;
            response.SendChunked     = false;
            response.ContentType     = System.Net.Mime.MediaTypeNames.Application.Octet;

            byte[] buffer = new byte[64 * 1024];

            int read;

            using (BinaryWriter bw = new BinaryWriter(response.OutputStream))
            {

                while ((read = fs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    bw.Write(buffer, 0, read);
                    bw.Flush();
                }

                bw.Close();

            }

            response.StatusCode = (int) HttpStatusCode.OK;
            response.StatusDescription = "OK";
            response.OutputStream.Close();

        }

    }


    private String getLocalAddress()
    {

        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

        return host.AddressList
                   .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString();

    }


    private void errorHandler(string error)
    {

        Console.WriteLine(error);

        Environment.Exit(0);

    }


    private void printHelpText()
    {

        Console.WriteLine(@"          _                           __ ");
        Console.WriteLine(@"         (_)                         / _|");
        Console.WriteLine(@"__      ___ _ ____      _____   ___ | |_ ");
        Console.WriteLine(@"\ \ /\ / / | '_ \ \ /\ / / _ \ / _ \|  _|");
        Console.WriteLine(@" \ V  V /| | | | \ V  V / (_) | (_) | |  ");
        Console.WriteLine(@"  \_/\_/ |_|_| |_|\_/\_/ \___/ \___/|_|  ");

        Console.WriteLine("\n------------------------------------------");

        Console.WriteLine("");

        Console.WriteLine("Usage: \nwinwoof <file|directory> [port]");
        Console.WriteLine("[The path can be relative or absolute]\n");
        Console.WriteLine("Serves a file or a directory via http on \nyour local ip address.");

        Environment.Exit(0);

    }


    static void Main(string[] args)
    {

        new Winwoof(args).serve();

    }

}