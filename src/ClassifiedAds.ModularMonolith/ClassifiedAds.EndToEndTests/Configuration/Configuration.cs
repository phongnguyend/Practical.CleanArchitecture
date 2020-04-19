using System;
using System.Collections.Generic;
using System.Text;

namespace ClassifiedAds.EndToEndTests
{
    public class Configuration
    {
        public static string ChromeDriverPath
        {
            get
            {
                return @"D:\Downloads\chromedriver_win32\72";
            }
        }

        public static string Login_Url
        {
            get
            {
                return "https://localhost:44364/Home/Login";
            }
        }

        public static string Login_UserName
        {
            get
            {
                return "phong@gmail.com";
            }
        }

        public static string Login_Password
        {
            get
            {
                return "v*7Un8b4rcN@<-RN";
            }
        }
    }
}
