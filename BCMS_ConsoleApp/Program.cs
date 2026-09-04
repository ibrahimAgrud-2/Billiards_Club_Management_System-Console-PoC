using System;

namespace testConsoleApp2
{
    public class Logger
    {

        public delegate void LogAction(string message);

        LogAction _LogAction;

        public Logger(LogAction logAction)
        {
            _LogAction = logAction;
        }

        public void Log(string message)
        {
            //içeriğinden bağımsız ilgili log fonksiyonunu çalıştırır.
            _LogAction(message);
        }



    }


    internal class Program
    {
        public static void LogToScreen(string message)
        {
            //ekrana basmak için ilgil kod
            Console.WriteLine(message);
        }

        public static void LogToTextFile(string message)
        {
            //dosyaya kayıt için ilgil kod
            Console.WriteLine(message);

        }
        public static void LogToDatabase(string message)
        {
            //DB'ye kayıt için ilgil kod
            Console.WriteLine(message);

        }

        static void Main(string[] args)
        {
            //delegate listesine LogToScreen fonk ekliyoruz. 
            //bunu programda 50 yerde kullandığımızı düşünelim ve artık ekrana 
            //değil de DB'e log yapmak istediğimde 50 yerde değil sadece bu satırda
            //LogToDatabase diyebilirdim
            Logger log = new Logger(LogToScreen);

            //sınıftaki Log fonksiyonu messagı alıyor ve delegate listesindeki fonksiyonu çağırıyor. Listede hangi fonksyionu olduğundan bağımsız olarak. Sadece delegate fonksiyonunu çağırıyor
            log.Log("This is the log message");


        }
    }
}
