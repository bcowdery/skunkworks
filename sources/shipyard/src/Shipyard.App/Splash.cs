using System;
using System.Reflection;

namespace Shipyard.App
{
    public static class Splash
    {
        public static readonly string SplashImage = @$"
   _____ _     _                           _                      
  / ____| |   (_)                         | |   /\                
 | (___ | |__  _ _ __  _   _  __ _ _ __ __| |  /  \   _ __  _ __  
  \___ \| '_ \| | '_ \| | | |/ _` | '__/ _` | / /\ \ | '_ \| '_ \ 
  ____) | | | | | |_) | |_| | (_| | | | (_| |/ ____ \| |_) | |_) |
 |_____/|_| |_|_| .__/ \__, |\__,_|_|  \__,_/_/    \_\ .__/| .__/ 
                | |     __/ |                        | |   | |    
                |_|    |___/                         |_|   |_|    
           

Copyright © {DateTime.Now:yyyy} Symend Inc, Calgary AB
Shipyard Service, Version {Assembly.GetEntryAssembly()?.GetName().Version}    

";


        public static void Print(Action<string> printer) 
        {
            printer(SplashImage);
        }
    }
}