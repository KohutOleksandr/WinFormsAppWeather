using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsAppWeather
{
    public class tokenAPI
    {
        private static string myTokenApi = "c944cee8bcf4d3d8ef2486c50a1ae4a0";
        public static string getToken()
        {
            return myTokenApi;
        }
    }
}
