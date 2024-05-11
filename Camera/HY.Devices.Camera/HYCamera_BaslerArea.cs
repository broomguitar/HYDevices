namespace HY.Devices.Camera
{
    public class HYCamera_BaslerArea : AbstractHYCamera_Basler
    {
        public override CameraTypes CameraType => CameraTypes.Area;
        public HYCamera_BaslerArea(CameraConnectTypes connectType, uint cameraIndex) : base(connectType, cameraIndex)
        {

        }
        public HYCamera_BaslerArea(string serialNum) : base(serialNum)
        {

        }
    }
}
