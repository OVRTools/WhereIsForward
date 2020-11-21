using System;
using Valve.VR;

namespace WhereIsForward
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0] == "--register")
                {
                    RegisterManifest();
                    return;
                }
                else if (args[0] == "--deregister")
                {
                    DeregisterManifest();
                    return;
                }
            }

            var app = new WhereIsForwardApp();

            Console.WriteLine("Hello gamer. You now have an arrow. Congratulation! :) Press enter to exit.");
            Console.ReadLine();

            app.Shutdown();
        }

        static void RegisterManifest()
        {
            EVRInitError initErr = EVRInitError.None;
            OpenVR.Init(ref initErr, EVRApplicationType.VRApplication_Utility);

            if (initErr != EVRInitError.None)
            {
                Console.WriteLine($"Error initializing OpenVR handle: {initErr}");
                Environment.Exit(1);
            }

            var err = OpenVR.Applications.AddApplicationManifest(Utils.PathToResource("manifest.vrmanifest"), false);

            if (err != EVRApplicationError.None)
            {
                Console.WriteLine($"Error registering manifest with OpenVR runtime: {err}");
                Environment.Exit(1);
            }

            Console.WriteLine("Application manifest registered with OpenVR runtime!");

            OpenVR.Shutdown();
        }

        static void DeregisterManifest()
        {
            EVRInitError initErr = EVRInitError.None;
            OpenVR.Init(ref initErr, EVRApplicationType.VRApplication_Utility);

            if (initErr != EVRInitError.None)
            {
                Console.WriteLine($"Error initializing OpenVR handle: {initErr}");
                Environment.Exit(1);
            }

            var err = OpenVR.Applications.RemoveApplicationManifest(Utils.PathToResource("manifest.vrmanifest"));

            if (err != EVRApplicationError.None)
            {
                Console.WriteLine($"Error de-registering manifest with OpenVR runtime: {err}");
                Environment.Exit(1);
            }

            Console.WriteLine("Application manifest de-registered with OpenVR runtime!");

            OpenVR.Shutdown();
        }
    }
}
