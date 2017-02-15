using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Flute
{
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
         // Check file exist. Then proceed if YES.
         //if(!IsCfgExist(path)) throw new ArgumentException("File Not Exist!");
         if (!IsCfgExist(path)) return null;

         // Reading File
         string[] cfgLinesString = File.ReadAllLines(path);


         // Create Dictionary
         Dictionary<string, string> configDictionary = new Dictionary<string, string>();
         foreach (var cfgLine in cfgLinesString)
         {
            int seperatorLoc = cfgLine.IndexOf(deliminator);
            string tempKey = cfgLine.Substring(0, seperatorLoc).Replace(" ", string.Empty);
            string tempValue = cfgLine.Substring(seperatorLoc).Replace(" ", string.Empty);

            configDictionary.Add(tempKey, tempValue);
         }

         return configDictionary;
      }

      

      public void Exit()
      {
         Environment.Exit(0);
      }
   }

   class Program
   {
      const string _promptIndicator = ">";

      static void Main(string[] args)
      {
         Console.Title = "Flute v 0.1";

         //[NOTE] Testing Purpose
         DefCommands o = new DefCommands();
         o.ReadCfg(@"E:\config.txt");

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

   

   /// <summary>
   /// INPUT: string, url
   /// TASK: 
   /// OUTPUT: 
   /// </summary>
   class UrlCorrection
   {
      private string _sourceUrl;
      private string _hostUrl = "http://www.linguee.com/";
      public UrlCorrection(string address)
      {
         this._sourceUrl = address ?? null;
      }

      public string DetectType()
      {
         if (_sourceUrl == null)
         {
            Console.WriteLine("ERROR:\tThere is no downloading url.");
            return _sourceUrl;
         }

         if (_sourceUrl.StartsWith("http://www.linguee.com/"))
         {
            
         }
         else if (_sourceUrl.StartsWith("DE/"))
         {
            _sourceUrl = _hostUrl + _sourceUrl;
         }
         else if (_sourceUrl.StartsWith("/DE"))
         {
            _sourceUrl = _sourceUrl.Substring(1);
            _sourceUrl = _hostUrl + _sourceUrl;
         }


         if (!_sourceUrl.EndsWith(".mp3"))
         {
            if (_sourceUrl.Contains("mp3") && _sourceUrl.EndsWith("mp3"))
            {
               _sourceUrl = _sourceUrl.Substring(0, _sourceUrl.Length - 3);
            }
            _sourceUrl = _sourceUrl.Insert(_sourceUrl.Length, ".mp3");
         }

         return _sourceUrl;
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
