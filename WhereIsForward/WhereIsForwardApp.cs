using OVRSharp;

namespace WhereIsForward
{
    class WhereIsForwardApp : Application
    {
        private readonly Overlay floorOverlay;

        public WhereIsForwardApp()
            : base(ApplicationType.Overlay)
        {
            floorOverlay = new WhereIsForwardOverlay();
            floorOverlay.Show();
        }
    }
}
