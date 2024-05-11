namespace HY.Devices.Camera
{
    /// <summary>
    /// 海康线阵相机
    /// </summary>
    public class HYCamera_HIKLinear : AbstractHYCamera_HIK
    {
        public override CameraTypes CameraType => CameraTypes.Linear;
        public HYCamera_HIKLinear(CameraConnectTypes connectType, uint cameraIndex) : base(connectType, cameraIndex)
        {

        }
        public HYCamera_HIKLinear(CameraConnectTypes connectType, string SerialNum) : base(connectType, SerialNum)
        {

        }
    }
}
