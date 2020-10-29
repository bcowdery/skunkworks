using System;
using System.Reflection;

namespace MissionControl.Web
{
    public static class Splash
    {
        public static readonly string SplashImage = @$"
        _         _               ___            _             _ 
  /\/\ (_)___ ___(_) ___  _ __   / __\___  _ __ | |_ _ __ ___ | |
 /    \| / __/ __| |/ _ \| '_ \ / /  / _ \| '_ \| __| '__/ _ \| |
/ /\/\ \ \__ \__ \ | (_) | | | / /__| (_) | | | | |_| | | (_) | |
\/    \/_|___/___/_|\___/|_| |_\____/\___/|_| |_|\__|_|  \___/|_|                                                                

Copyright © {DateTime.Now:yyyy}
MissionControl Admin App, Version {Assembly.GetEntryAssembly()?.GetName().Version}    

";

        public static void Print(Action<string> printer) 
        {
            printer(SplashImage);
        }
    }
}
