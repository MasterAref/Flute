using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Flute
{
   class Program
   {
      


      const string _promptIndicator = ">";

      static void Main(string[] args)
      {
         Console.Title = "Flute v 0.1";

         //[NOTE] Testing Purpose

         //[SCENE-1]: getting fine configuration
         DefCommands o = new DefCommands();
         ConfigObject ConfigObject = o.ValidateConfig(o.ReadCfg(@"E:\IdealConfig.txt"));


         //[SCENE-2]: Download link exist in clipboard
         string copyLink = new UrlClipboard().CopyLink();


         //[SCENE-3]: Validate and detect url component.
         UrlValidate newUrl = new UrlValidate(copyLink, ConfigObject.sources);
         


         //[TODO] Uncomment
         //Run();

         Console.WriteLine("END OF LINE");
         Console.Read();
      }

      static void Run()
      {
         Dictionary<string, string> cmdsCfgNotFound = new Dictionary<string, string>();
         cmdsCfgNotFound["R"] = "To Recheck";
         cmdsCfgNotFound["N"] = "Check cfg in new path.";
         cmdsCfgNotFound["C"] = "To Create new Config file.";
         cmdsCfgNotFound["Q"] = "To Exit.";


         Dictionary<string, string> commandlist = new Dictionary<string, string>();
         commandlist["R"] = "IsCfgExist";
         commandlist["N"] = "IsCfgExist";
         commandlist["Q"] = "Exit";

         Console.WriteLine("To see available commands press 'L'");
         while (true)
         {
            var consoleInput = ReadUserCmd();
            if (String.IsNullOrWhiteSpace(consoleInput)) continue;

            if(!commandlist.ContainsKey(consoleInput)) continue;

            var assembly = Assembly.GetExecutingAssembly();
            var methodToCall = typeof(IDefCommands).GetMethod(commandlist[consoleInput]);
            switch (consoleInput)
            {
               case "R":
                  methodToCall.Invoke(new DefCommands(), new object[] { "E:\\" });
                  break;

               case "N":
                  string path = WriteToConsole("Type cfg path (e.g: C:\\htdocs\\myconfig.cfg");
                  methodToCall.Invoke(new DefCommands(), new object[] { path });
                  break;

               case "Q":
                  methodToCall.Invoke(new DefCommands(), null);
                  break;
            }
            
            

            try
            {
               

            }
            catch (TargetInvocationException ex)
            {
               throw ex.InnerException;
            }

            //Console.WriteLine(Console.ReadLine());

         }
      }

      static void ShowInstruction(Dictionary<string, string> instructions)
      {
         foreach (var instruction in instructions)
         {
            Console.WriteLine($"{instruction.Value} Press {instruction.Key}");
         }
      }

      static string WriteToConsole(string promptMessage = "")
      {
         Console.WriteLine(_promptIndicator + promptMessage);
         return Console.ReadLine();
      }

      static string ReadUserCmd()
      {
         Console.Write(">");
         return Console.ReadLine().ToUpper();
      }
   }

   struct ConfigObject
   {
      public string[] sources;
      public string[] prefixes;
      public string saveTo;
   }


   struct DownloadObject
   {
      public string saveTo;
      public string name;
      public string host;
      public string downloadUrl;
      public string hostPrefix;
      public string typePrefix;

   }

   interface IDefCommands
   {
      bool IsCfgExist(string path);

      void Exit();
   }

   class DefCommands : IDefCommands
   {
      public bool IsCfgExist(string path)
      {
         if (!File.Exists(path))
         {
            Console.WriteLine($"No Config file found in {path}");
            return false;
         }
         return true;
      }

      public Dictionary<string, string> ReadCfg(string path, char deliminator = ':')
      {
         // Proceed if file exist.
         //if(!IsCfgExist(path)) throw new ArgumentException("File Not Exist!");
         if (!IsCfgExist(path)) return null;

         // Reading File
         string[] cfgLinesString = File.ReadAllLines(path);


         // Create Dictionary
         Dictionary<string, string> configDictionary = new Dictionary<string, string>();
         foreach (var cfgLine in cfgLinesString)
         {
            string currentLine = cfgLine.Replace(" ", string.Empty);
            int seperatorLoc = currentLine.IndexOf(deliminator);
            string tempKey = currentLine.Substring(0, seperatorLoc);
            string tempValue = currentLine.Substring(seperatorLoc + 1);

            configDictionary.Add(tempKey, tempValue);
         }

         return configDictionary;
      }

      public ConfigObject ValidateConfig(Dictionary<string, string> configParams, char delimator = ',')
      {
         //TODO: validation rules!


         ConfigObject configO;
         configO.sources = configParams["sources"].Split(',');
         configO.prefixes = configParams["prefixes"].Split(',');
         configO.saveTo = configParams["saveTo"];
         return configO;
      }

      public void Exit()
      {
         Environment.Exit(0);
      }
   }


   class UrlClipboard
   {
      [DllImport("user32.dll")]
      internal static extern bool OpenClipboard(IntPtr hWndNewOwner);

      [DllImport("user32.dll")]
      internal static extern bool SetClipboardData(uint uFormat, IntPtr data);

      [DllImport("user32.dll")]
      private static extern IntPtr GetClipboardData(uint uFormat);

      [DllImport("user32.dll")]
      internal static extern bool CloseClipboard();

      [DllImport("user32.dll")]
      internal static extern bool EmptyClipboard();

      public string CopyLink()
      {
         OpenClipboard(IntPtr.Zero);
         var data = Marshal.PtrToStringAuto(GetClipboardData(13));
         EmptyClipboard();
         CloseClipboard();
         return data;
      }

      // Example from http://stackoverflow.com/questions/13571426/how-can-i-copy-a-string-to-clipboard-within-my-console-app-without-adding-a-refe
      //OpenClipboard(IntPtr.Zero);
      ////var yourString = "Hello World!";
      ////var ptr = Marshal.StringToHGlobalUni(yourString);
      ////SetClipboardData(13, ptr);
      //var t = Marshal.PtrToStringAuto(GetClipboardData(13));
      //CloseClipboard();
      ////Marshal.FreeHGlobal(ptr);
   }


   /// <summary>
   /// INPUT: string, url
   /// TASK: 
   /// OUTPUT: 
   /// </summary>
   class UrlValidate
   {
      // FIELDS & PROPERTIES
      private string _fileUrl;
      private string[] _hostsUrl;
      private string urlValidateHost;
      private string urlValidateDlLink;


      // CONSTRUCT
      public UrlValidate(string address, string[] hostsString)
      {
         this._fileUrl = address ?? null;
         this._hostsUrl = hostsString ?? null;

         if (string.IsNullOrWhiteSpace(_fileUrl) || _hostsUrl == null) return;  
      }


      // METHODS
      public DownloadObject SetupDownloadObject()
      {
         DownloadObject dlObject = new DownloadObject();

         // Setup 'DownloadObject.host'
         if (IsFullUrl(_fileUrl)) DetectFullUrlHost();
         if (IsFullUrl(_fileUrl)) DetectRelativeUrlHost();
         dlObject.host = urlValidateHost;

         // Setup 'DownloadObject.downloadUrl'
         if (!IsFullUrl(_fileUrl))
         {
            switch (urlValidateHost)
            {
               case "www.linguee.com":
                  urlValidateDlLink = urlValidateHost + "/mp3/" + _fileUrl;
                  break;

               //TODO: add other cases for other sources.
            }
         }
         dlObject.downloadUrl = urlValidateDlLink;


         //TODO: Setup other 'DownloadObject' fields

         //TODO: Error checking for each 'DownloadObject' fields
         if (urlValidateHost == null)
         {
            Console.WriteLine("ERRPR: cannot find out file's host.");
         }

         return dlObject;
      }

      //NOTE: If provided string was not Full path url then it assuemes its reletive. 
      // in above situation or returning true by IsFullUrl() we then identifying host
      // with DetectHost().
      private bool IsFullUrl(string check)
      {
         if (check.StartsWith("http://www.")) return true;
         if (check.StartsWith("https://www.")) return true;
         if (check.StartsWith("www.")) return true;
         if (check.StartsWith("http://")) return true;
         if (check.StartsWith("https://")) return true;

         return false;
      }

      private void DetectFullUrlHost()
      {

         if (_fileUrl == null)
         {
            Console.WriteLine("ERROR:\tThere is no downloading url.");
            return;
         }

         foreach (var url in _hostsUrl)
         {
            if (_fileUrl.StartsWith(url))
            {
               urlValidateHost = url;
            }
         }

      }

      private void DetectRelativeUrlHost()
      {
         
         if (_fileUrl == null)
         {
            Console.WriteLine("ERROR:\tThere is no downloading url.");
            return;
         }

         // www.Linguee.com use 'DE/' as relative path for voice files.
         if (urlValidateHost == null && _fileUrl.StartsWith("DE/"))
         {
            //NOTE: check if 'urlValidateHost' didn't set before. If setted it means we have multiple source! sth WRONG!!!
            urlValidateHost = _hostsUrl[Array.IndexOf(_hostsUrl, "www.linguee.com")];
         }

         //TODO: Add more if statement to check more sources!

      }

      
   }

   interface IStreamDownload
   {
      
   }


   /// <summary>
   /// Input: url
   /// Task: Downloading file
   /// Output: stream
   /// </summary>
   class StreamDownload
   {

      public Stream DownloadStream(string downloadLink)
      {
         HttpWebRequest request = (HttpWebRequest) WebRequest.Create(downloadLink);
         HttpWebResponse response = (HttpWebResponse)request.GetResponse();
         Stream stream = response.GetResponseStream();

         return stream;
      }
   }


   /// <summary>
   /// FileSaver responsible for saving stream input as a file.
   /// </summary>
   class StreamSave
   {
      private Stream _stream;
      public StreamSave(Stream inputStream)
      {
         this._stream = inputStream ?? null;
      }

      private IStreamDownload _streamDownload;
      public string SaveTo { get; set; }


      

      public StreamSave(IStreamDownload inputStream)
      {
         this._streamDownload = inputStream;
      }


      public void SaveStream()
      {
         SaveTo = @"E:\Test.mp3";

         if (_stream == null)
         {
            Console.WriteLine("ERROR:\tThere is no stream to save as a mp3 file.");
            return;
         }

         using (FileStream fs = File.Create(SaveTo))
         {
            _stream.CopyTo(fs);
         }
      }

   }


   interface IFileConfig
   {
      
   }

   /// <summary>
   /// A type that represent how(name, tag, etc) and where(path) the file will save.
   /// </summary>
   class FileConfig : IFileConfig
   {
      public string HostUrl { get; set; }
      public string SaveTo { get; set; }
      public string Prefix { get; set; }

      public FileConfig(string configPath)
      {
         if (!File.Exists(configPath))
         {
            return;
         }
         string[] allLines = File.ReadAllLines(configPath);
         string[] valueKey;

         foreach (var line in allLines)
         {
            valueKey = line.Split(' ');

            switch (valueKey[0])
            {
               case "Host:":
                  HostUrl = valueKey[1];
                  break;

               case "SaveTo:":
                  SaveTo = valueKey[1];
                  break;

               case "Prefix:":
                  Prefix = valueKey[1];
                  break;

               default:
                  return;
            }
         }
         
      }

   }
}
