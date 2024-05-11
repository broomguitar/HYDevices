namespace HY.Devices.Camera
{
    public class HYCamera_DalsaLinear : AbstractHYCamera_Dalsa
    {
        public override CameraTypes CameraType => CameraTypes.Linear;
        public HYCamera_DalsaLinear(string serialNum, string configPath, int channel) : base(serialNum, configPath, channel)
        {

        }
        public HYCamera_DalsaLinear(string serialNum, int channel) : base(serialNum, channel)
        {

        }
    }
}
