using OVRUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Valve.VR;

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

            var overlayImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\arrow.png");
            floorOverlay.SetTextureFromFile(overlayImagePath);
            floorOverlay.Show();

            Thread thread = new Thread(() => PrintAngle(app));
            thread.Start();
        }

        public void Shutdown()
        {
            app.Shutdown();
        }

        static void PrintAngle(Application application)
        {
            for (;;)
            {
                TrackedDevicePose_t[] trackedDevicePoseT = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];

                application.OVRSystem.GetDeviceToAbsoluteTrackingPose(
                    ETrackingUniverseOrigin.TrackingUniverseStanding, 0f, trackedDevicePoseT
                );

                DeviceLocation HmdLocation = new DeviceLocation(trackedDevicePoseT[0].mDeviceToAbsoluteTracking);

                ThreeDeeLine headsetToOrigin = new ThreeDeeLine(new Vector3(0, 0, 0), HmdLocation.Position);
                ThreeDeeLine headsetDirection = new ThreeDeeLine(HmdLocation.Position, HmdLocation.Rotation);
                Vector2 headsetDirectionAngle = headsetDirection.GetAngleFromPoints();  // Confirmed Correct
                Vector2 headsetOriginAngle = headsetToOrigin.GetAngleFromPoints();  // Confirmed correct

                // I think the solution is to split it up into 4 cases based on position
                
                Console.WriteLine("---------------");
                Console.WriteLine($"Headset Angle From Points = X: {((headsetDirectionAngle.X) * 180 / Math.PI)} | Y: {((headsetDirectionAngle.Y) * 180 / Math.PI)}");
                // Console.WriteLine($"Headset Angle From Headset = X: {(HmdLocation.Rotation.X) * 180 / Math.PI} | Y: {((HmdLocation.Rotation.Y) * 180 / Math.PI)}");
                Console.WriteLine($"Headset Origin Angle From Points = X: {(headsetOriginAngle.X) * 180 / Math.PI} | Y: {((headsetOriginAngle.Y) * 180 / Math.PI)}");
                Console.WriteLine($"Delta Angle From Points = X: {(Math.Abs(headsetDirectionAngle.X) - Math.Abs(headsetOriginAngle.X)) * 180 / Math.PI} | Y: {(Math.Abs((headsetDirectionAngle.Y) * 180 / Math.PI) - Math.Abs((headsetOriginAngle.Y) * 180 / Math.PI))}");
                // Console.WriteLine($"Delta Angle From Headset = X: {(HmdLocation.Rotation.X - headsetOriginAngle.X) * 180 / Math.PI} | Y: {((HmdLocation.Rotation.Y - headsetOriginAngle.Y) * 180 / Math.PI)}");
                Thread.Sleep(100);
            }
        }
        
        public class ThreeDeeLine
        {
            public ThreeDeeLine(Vector3 point1, Vector3 point2)
            {
                Point1 = point1;
                Point2 = point2;
            }

            public ThreeDeeLine(Vector3 point1, Vector2 rotation)
            {
                Point1 = point1;

                double a = Math.Tan(rotation.X);
                double b = Math.Tan(rotation.Y) * a;

                Point2 = new Vector3(point1.X - b, point1.Y - 1, point1.Z - a);
            }

            public Vector2 GetAngleFromPoints()
            {
                Vector3 deltaVector3 = new Vector3(Point1.X - Point2.X, Point1.Y - Point2.Y, Point1.Z - Point2.Z);
                return new Vector2(Math.Atan(deltaVector3.Z / deltaVector3.Y),
                    Math.Atan(deltaVector3.X / deltaVector3.Z));
            }

            public Vector3 Point1 { get; }
            public Vector3 Point2 { get; }
        }


        public class DeviceLocation
        {
            public DeviceLocation(HmdMatrix34_t matrix)
            {
                double w = Math.Sqrt(Math.Max(0, 1 + matrix.m0 + matrix.m5 + matrix.m10)) / 2;
                double x = Math.Sqrt(Math.Max(0, 1 + matrix.m0 - matrix.m5 - matrix.m10)) / 2;
                double y = Math.Sqrt(Math.Max(0, 1 - matrix.m0 + matrix.m5 - matrix.m10)) / 2;
                double z = Math.Sqrt(Math.Max(0, 1 - matrix.m0 - matrix.m5 + matrix.m10)) / 2;

                _copysign(ref x, -matrix.m9 - -matrix.m6);
                _copysign(ref y, -matrix.m2 - -matrix.m8);
                _copysign(ref z, matrix.m4 - matrix.m1);

                // up down
                double rX = Math.Atan2(2 * (w * x + y * z), 1 - 2 * (x * x + y * y));

                // left right
                double rY;
                if (Math.Abs(2 * (w * y - z * x)) >= 1)
                {
// use 90 degrees if out of range
                    rY = Math.CopySign(Math.PI / 2, 2 * (w * y - z * x));
                }
                else
                    rY = Math.Asin(2 * (w * y - z * x));

                // yaw (z-axis rotation)
                // double rZ = Math.Atan2(2 * (w * z + x * y), 1 - 2 * (y * y + z * z));

                Rotation = new DeviceRotation(rX, rY);

                Position = new DevicePosition(matrix.m3, matrix.m7, -matrix.m11);
            }

            public DevicePosition Position { get; }
            public DeviceRotation Rotation { get; }
        }

        public class DevicePosition : Vector3
        {
            public DevicePosition(double x, double y, double z) : base(x, y, z)
            {
            }

            public override string ToString()
            {
                return $"Position - X: {X * 180 / Math.PI} | Y: {Y * 180 / Math.PI} | Z: {Z * 180 / Math.PI}";
            }
        }

        public class DeviceRotation : Vector2
        {
            public DeviceRotation(double x, double y) : base(x, y)
            {
            }

            public override string ToString()
            {
                return $"Rotation - X: {X * 180 / Math.PI} | Y: {Y * 180 / Math.PI}";
            }
        }

        /// <summary>
        /// Used to store X, Y, and Z values.
        ///
        /// X is left and right
        /// Y is up and down
        /// Z is forward and back
        /// </summary>
        public class Vector3
        {
            public Vector3(double x, double y, double z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public double X { get; }
            public double Y { get; }
            public double Z { get; }
        }

        /// <summary>
        /// Used to store rotation information generally.
        ///
        /// X is up/down rotation, Y is left/right rotation. Z would be twisteyness but that's not important
        /// </summary>
        public class Vector2
        {
            public Vector2(double x, double y)
            {
                X = x;
                Y = y;
            }

            public double X { get; }
            public double Y { get; }
        }

        private static void _copysign(ref double sizeVal, double signVal)
        {
            if (signVal > 0 != sizeVal > 0)
                sizeVal = -sizeVal;
        }
    }
}