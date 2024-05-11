namespace HY.Devices.Camera
{
    /// <summary>
    /// 海康面阵相机
    /// </summary>
    public class HYCamera_HIKArea : AbstractHYCamera_HIK
    {
        public override CameraTypes CameraType => CameraTypes.Area;
        public HYCamera_HIKArea(CameraConnectTypes connectType, uint cameraIndex) : base(connectType, cameraIndex)
        {

        }
        public HYCamera_HIKArea(CameraConnectTypes connectType, string SerialNum) : base(connectType, SerialNum)
        {

        }
    }
}
