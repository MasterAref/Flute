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
         Internet sound = new Internet();
         sound.download("http://www.linguee.com/mp3/DE/63/63ec45594aebdeb6075936e8e9d683b9-104");
         
         Console.ReadLine();
      }
   }

   /// <summary>
   /// After getting the url this class
   /// responsible to download it.
   /// </summary>
   class Internet
   {

      public Stream download(string downloadLink)
      {
         HttpWebRequest request = (HttpWebRequest) WebRequest.Create(downloadLink);
         HttpWebResponse response = (HttpWebResponse)request.GetResponse();
         Stream stream = response.GetResponseStream();

         return stream;
      }




   }


}
