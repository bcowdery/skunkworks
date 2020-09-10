using System;
using System.Reflection;

namespace Shipyard.Service
{
    public static class Splash
    {
        public static readonly string SplashImage = @$"
   _____ _     _                           _ 
  / ____| |   (_)                         | |
 | (___ | |__  _ _ __  _   _  __ _ _ __ __| |
  \___ \| '_ \| | '_ \| | | |/ _` | '__/ _` |
  ____) | | | | | |_) | |_| | (_| | | | (_| |
 |_____/|_| |_|_| .__/ \__, |\__,_|_|  \__,_|
                | |     __/ |                
                |_|    |___/                 

Copyright © {DateTime.Now:yyyy} Symend Inc, Calgary AB
Shipyard Service, Version {Assembly.GetEntryAssembly()?.GetName().Version}    

";


        public static void Print(Action<string> printer) 
        {
            printer(SplashImage);
        }
    }
}