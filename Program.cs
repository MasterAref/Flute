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
         Download sound = new Download();
         sound.byURL("http://www.linguee.com/mp3/DE/63/63ec45594aebdeb6075936e8e9d683b9-104");
         
         Console.ReadLine();
      }
   }

   class Download
   {

      public void byURL(string url)
      {
         HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
         HttpWebResponse response = (HttpWebResponse)request.GetResponse();
         Stream voice = response.GetResponseStream();



         byte[] buffer = new byte[32768];
         using (FileStream fileStream = File.Create(@"F:\me.mp3"))
         {
            while (true)
            {
               int read = voice.Read(buffer, 0, buffer.Length);
               if (read <= 0)
                  break;
               fileStream.Write(buffer, 0, read);
            }
         }


         Console.ReadLine();
      }

   }
}
