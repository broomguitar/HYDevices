using System;

namespace HY.Devices.Camera
{
    public class HYCamera_IkapArea : AbstractHYCamera_IKap_BoardPCIE
    {
        public HYCamera_IkapArea(uint cameraIndex, string configFile) : base(cameraIndex, configFile)
        {

        }

        public override CameraTypes CameraType => CameraTypes.Area;
    }
    public class HYCamera_IkapArea_Net : AbstractHYCamera_IKap_Net
    {
        public HYCamera_IkapArea_Net(uint cameraIndex) : base(cameraIndex)
        {
        }
        public HYCamera_IkapArea_Net(string serialNum) : base(serialNum)
        {

        }
        public override CameraTypes CameraType => CameraTypes.Area;

        public override bool SoftWareTrigger()
        {
            throw new NotImplementedException();
        }
    }
}
