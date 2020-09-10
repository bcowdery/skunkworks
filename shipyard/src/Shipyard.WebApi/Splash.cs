using System;
using System.Reflection;

namespace Shipyard.WebApi
{
    public static class Splash
    {
        public static readonly string SplashImage = @$"
   _____ _     _                           ___          __  _     
  / ____| |   (_)                         | \ \        / / | |    
 | (___ | |__  _ _ __  _   _  __ _ _ __ __| |\ \  /\  / /__| |__  
  \___ \| '_ \| | '_ \| | | |/ _` | '__/ _` | \ \/  \/ / _ \ '_ \ 
  ____) | | | | | |_) | |_| | (_| | | | (_| |  \  /\  /  __/ |_) |
 |_____/|_| |_|_| .__/ \__, |\__,_|_|  \__,_|   \/  \/ \___|_.__/ 
                | |     __/ |                                     
                |_|    |___/                                      
                    
Copyright © {DateTime.Now:yyyy} Symend Inc, Calgary AB
Shipyard WebAPI, Version {Assembly.GetEntryAssembly()?.GetName().Version}    

";


        public static void Print(Action<string> printer) 
        {
            printer(SplashImage);
        }
    }
}