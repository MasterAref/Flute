using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Flute
{
   class Program
   {

      static void Main(string[] args)
      {
         Dictionary<String, string> commands = new Dictionary<string, string>();
         commands["R"] = "to Recheck";
         commands["N"] = "to Insert New path.";
         commands["C"] = "to Create new Config file.";
         commands["Q"] = "to Exit.";

         
         Console.WriteLine("Type default configuration file path:");
         string defConfigPath = Console.ReadLine();

         if (!File.Exists(defConfigPath))
         {
            string nextcmd;
            Console.WriteLine($"No Config file found in {defConfigPath}.");

            // NON-Recursive
            ShowInstruction(commands);
            do
            {
               nextcmd = GetCommand(commands);
            } while (nextcmd == null);


            // Validate & Call proper Method
            switch (nextcmd.ToUpper())
            {
               case "r":
               case "R":
                  // Call method
                  break;

               case "n":
               case "N":
                  // Call method
                  break;

               case "c":
               case "C":
                  // Call method
                  break;

               case "q":
               case "Q":
                  Environment.Exit(0);
                  break;

               case null:
               default:
                  Console.WriteLine("Please enter proper command!");
                  break;
            }
         }

         FileConfig configObject = new FileConfig(defConfigPath);

         



         //StreamDownload sound = new StreamDownload();
         //StreamSave ss = new StreamSave(sound.DownloadStream("http://www.linguee.com/mp3/DE/63/63ec45594aebdeb6075936e8e9d683b9-104"));
         //ss.SaveStream();



         Console.WriteLine(configObject.HostUrl);
         Console.ReadLine();
      }

      static void ShowInstruction(Dictionary<string, string> cmds)
      {
         foreach (var command in cmds)
         {
            Console.WriteLine($"Press {command.Key.PadRight(5)}{command.Value}");
         }
      }

      static string GetCommand(Dictionary<string, string> validCommands)
      {
         //[NOTE] userInput is Key not value such as "R","A","D", ...
         string userInput = Console.ReadLine().ToUpper();
         if (validCommands.ContainsKey(userInput))
         {
            return userInput;
         }
         else
         {
            string errorMsg = "ERROR: Please enter proper Command!\n";
            Console.WriteLine(errorMsg + new String('-', errorMsg.Length));
            return null;
         }
      }

      static string ShowInstructionGetCmd(Dictionary<string, string> cmds)
      {
         foreach (var command in cmds)
         {
            Console.WriteLine($"Press {command.Key.PadRight(5)}{command.Value}");
         }

         return GetCommandRecursive(cmds);
      }

      static string GetCommandRecursive(Dictionary<string, string> validCommands)
      {
         //[TODO] Make it recursive
         //[PROBLEM] Now if we have sequence like "w1" "w2" "c" which "c" is correct and others are wrong 
         // it will return "w1" at end. But we want "c"
         //[NOTE] userInput is Key not value such as "R","A","D", ...
         string userInput = Console.ReadLine().ToUpper();
         if (!validCommands.ContainsKey(userInput))
         {
            string errorMsg = "ERROR: Please enter proper Command!\n";
            Console.WriteLine(errorMsg + new String('-', errorMsg.Length));
            GetCommandRecursive(validCommands);
            
         }
         return userInput;
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
