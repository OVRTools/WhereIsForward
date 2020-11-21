using OVRUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WhereIsForward
{
    class WhereIsForwardApp
    {
        private readonly Application app;
        private readonly Overlay floorOverlay;

        public WhereIsForwardApp()
        {
            app = new Application(Application.ApplicationType.Overlay);

            floorOverlay = new Overlay("where_is_forward", "Where Is Forward????")
            {
                WidthInMeters = 0.1f,
                Transform = new Valve.VR.HmdMatrix34_t
                {
                    m0 = 1,
                    m1 = 0,
                    m2 = 0,
                    m3 = 0,
                    m4 = 0,
                    m5 = 0,
                    m6 = 1,
                    m7 = 0,
                    m8 = 0,
                    m9 = -1,
                    m10 = 0,
                    m11 = 0
                }
            };

            var overlayImagePath = Utils.PathToResource("arrow.png");
            floorOverlay.SetTextureFromFile(overlayImagePath);
            floorOverlay.Show();
        }

        public void Shutdown()
        {
            app.Shutdown();
        }
    }
}
