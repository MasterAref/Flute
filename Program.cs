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
         StreamDownloader sound = new StreamDownloader();
         StreamSaver ss = new StreamSaver(sound.DownloadStream("http://www.linguee.com/mp3/DE/63/63ec45594aebdeb6075936e8e9d683b9-104"));
         ss.Save();
         //Console.ReadLine();
      }
   }

   /// <summary>
   /// After getting the url this class
   /// responsible to download it.
   /// </summary>
   class StreamDownloader
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
   class StreamSaver
   {
      private Stream _voiceStream;
      public string SaveTo { get; set; }


      public StreamSaver(Stream inputStream)
      {
         _voiceStream = inputStream ?? null;

         SaveTo = @"E:\Test.mp3";
      }

      public void Save()
      {
         if (_voiceStream == null)
         {
            Console.WriteLine("ERROR:\tThere is no stream to save as a mp3 file.");
            return;
         }

         using (FileStream fs = File.Create(SaveTo))
         {
            _voiceStream.CopyTo(fs);
         }
      }
      
   }


}
