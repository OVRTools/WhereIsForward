using OVRSharp;
using OVRSharp.Math;
using System;
using System.Numerics;

namespace WhereIsForward
{
    class WhereIsForwardOverlay : Overlay
    {
        public WhereIsForwardOverlay()
            : base("where_is_forward", "Where Is Forward????")
        {
            WidthInMeters = 0.1f;

            Transform = Matrix4x4
                .CreateRotationX(DegreesToRadians(90f))
                .ToHmdMatrix34_t();

            var overlayImagePath = Utils.PathToResource("arrow.png");
            SetTextureFromFile(overlayImagePath);
        }

        private static float DegreesToRadians(float degrees)
        {
            return (float)(degrees * (Math.PI / 180f));
        }
    }
}
