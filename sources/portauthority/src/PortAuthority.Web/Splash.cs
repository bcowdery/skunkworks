using System;
using System.Reflection;

namespace PortAuthority.Web
{
    public static class Splash
    {
        public static readonly string SplashImage = @$"
  _____           _                 _   _                _ _         
 |  __ \         | |     /\        | | | |              (_) |        
 | |__) |__  _ __| |_   /  \  _   _| |_| |__   ___  _ __ _| |_ _   _ 
 |  ___/ _ \| '__| __| / /\ \| | | | __| '_ \ / _ \| '__| | __| | | |
 | |  | (_) | |  | |_ / ____ \ |_| | |_| | | | (_) | |  | | |_| |_| |
 |_|   \___/|_|   \__/_/    \_\__,_|\__|_| |_|\___/|_|  |_|\__|\__, |
                                                                __/ |
                                                               |___/ 

Copyright © {DateTime.Now:yyyy}
Port Authority WebAPI, Version {Assembly.GetEntryAssembly()?.GetName().Version}
";

        public static void Print(Action<string> printer) 
        {
            printer(SplashImage);
        }
    }
}
