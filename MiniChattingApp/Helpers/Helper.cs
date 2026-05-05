using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniChattingApp.Helpers
{
    public static class Helper
    {
        public static void ShowInformativeTextServer (this string txt)
        {
            Console.WriteLine(txt, Console.ForegroundColor = ConsoleColor.Cyan);
            Console.ResetColor();
        }

        public static void ShowErrorMessage(this string txt)
        {
            Console.WriteLine(txt, Console.ForegroundColor = ConsoleColor.Red);
            Console.ResetColor();
        }

        public static bool IsValidJson(string strInput)
        {
            // Source - https://stackoverflow.com/a/14977915
            if (string.IsNullOrWhiteSpace(strInput)) { return false; }
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

    }
}
