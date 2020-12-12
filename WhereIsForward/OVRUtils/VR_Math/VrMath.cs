using OVRUtils;
using System;
using System.Xml.Schema;
using Valve.VR;

namespace OVRUtils.VR_Math
{
    /// <summary>
    /// This class dose random math stuff for VR, mostly conversions between the wacky datatypes OpenVR gives us and
    /// actually useful things like Euler angles
    /// </summary>
    public class VrMath
    {
        /// <summary>
        /// Turns a HmdMatrix34_t into 3d X, Y, and Z positions as well as euler rX, rY, and rZ. 
        /// 
        /// This is a bit of a shitshow. Positions work perfectly, but there's some issues with rotation sometimes.
        ///   rY works very well, and the math checks out for X and Z, but for some reason (pretty sure it's the
        ///   asymptotes on asin?) X and Z don't work all that reliably. I'm also not all that good at linear algebra so
        ///   there could also be something else at fault.
        /// 
        /// If you're good at this stuff and see what I did wrong, let me know!! - Cobular
        /// </summary>
        /// <param name="matrix">The mDeviceToAbsoluteTracking rotation matrix from the headset. Gotten from
        /// GetDeviceToAbsoluteTrackingPose</param>
        /// <returns>A tuple of Position, Rotation Vector3s</returns>
        public static Tuple<Vector3, Vector3> DeviceLocation(HmdMatrix34_t matrix)
        {
            // This all came from the wikipedia page on Rotation Matrices basically. I took the combined rX, rY, and rZ
            // matrix then backsolved for each euler angle. 
            double rY = -Math.Asin(matrix.m8 % (Math.PI/2));

            double rX = Math.Asin(matrix.m9 / Math.Cos(rY));

            double rZ = Math.Acos(matrix.m0 / Math.Cos(rY));
            
            return new Tuple<Vector3, Vector3>(new Vector3(matrix.m3, matrix.m7, -matrix.m11), new Vector3(rX, rY, rZ));
        }

        /// <summary>
        /// This is pretty simple and barely needs to be a function. Basically subtracts the two vector2s to get the delta.
        /// This does throw away location data, not really sure what the consequences of that are but I don't think that matters?
        /// </summary>
        /// <param name="angle1">A vector2 representing and angle.</param>
        /// <param name="angle2">A vector2 representing and angle in the same coordinate space.</param>
        /// <returns>The subtracted absolute values of the angles.</returns>
        public static Vector2 GetDeltaAngle(Vector2 angle1, Vector2 angle2)
        {
            return new Vector2(Math.Abs(angle1.X) - Math.Abs(angle2.X), 
                Math.Abs(angle1.Y) - Math.Abs(angle2.Y));
        }


        /// <summary>
        /// This one does what it says on the tin. Takes the input, multiplies it by (180/Pi) but wraps it up nicely
        /// </summary>
        /// <returns>The given angle, but in radians!</returns>
        public static double RadToDeg(double radians)
        {
            return radians * 180 / Math.PI;
        }

        /// <summary>
        /// This is used to test everything in concert and prints out the delta angles between the headset's view
        /// direction and the given position.
        /// </summary>
        /// <param name="openVrDeviceId">The ID of the device to pay attention to. Usually 0 for HMDs</param>
        /// <param name="position">The point that you are calculating the angle to.</param>
        /// <param name="application">The OVRSharp application context to be used.</param>
        public static void Debug(int openVrDeviceId, Vector3 position, Application application)
        {
            TrackedDevicePose_t[] trackedDevicePoseT = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
            application.OVRSystem.GetDeviceToAbsoluteTrackingPose(
                ETrackingUniverseOrigin.TrackingUniverseStanding, 0f, trackedDevicePoseT
            );
            
            Tuple<Vector3, Vector3> hmdLocation = DeviceLocation(trackedDevicePoseT[0].mDeviceToAbsoluteTracking);
            Vector2 headsetOriginAngle = GetAngleFromTwoPoints(new Vector3(0, 0, 0), hmdLocation.Item1);


            Vector2 deltaAngle = GetDeltaAngle(hmdLocation.Item2, headsetOriginAngle);

            Console.WriteLine($"Delta Angle From Points = X: {RadToDeg(deltaAngle.X)} | Y: {deltaAngle.Y}");
        }

            /// <summary>
        /// Returns the X and Y angles between two points in 3d space. This is a bit hard to explain, so I'll make a
        /// video or something eventually. If you're reading this and there isn't a video, make an issue and I'll
        /// probably get around to it.
        ///
        /// Quickly though, X is the up/down angle, and Y is the left/right angle if that makes sense.
        ///
        /// The way this works is to basically draw two triangles using the delta between the points, then do trig on
        /// that to find the angles. Again, this makes more sense with a diagram that is coming soon :tm:
        /// </summary>
        /// <param name="point1">The first point to use</param>
        /// <param name="point2">The second point to use</param>
        /// <returns>The X (up/down) and Y (left/right) angle between the two points in worldspace.</returns>
        public static Vector2 GetAngleFromTwoPoints(Vector3 point1, Vector3 point2)
        {
            return new Vector2(Math.Atan(point1.Z - point2.Z / point1.Y - point2.Y),
                Math.Atan(point1.X - point2.X / point1.Z - point2.Z));
        }

        /// <summary>
        /// Used to store X, Y, and Z values in a convenient little bundle. Used for position and rotation.
        ///
        /// X is left/right, Y is up/down, and Z is forward/back
        /// </summary>
        public class Vector3 : Vector2
        {
            public Vector3(double x, double y, double z) : base(x, y)
            {
                Z = z;
            }
            
            public double Z { get; }
        }

        /// <summary>
        /// Used to store X and Y values in a convenient little bundle. Used for rotation data when one does not care
        ///   about Z, because come on who really cares about Z anyway.
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
    }
}