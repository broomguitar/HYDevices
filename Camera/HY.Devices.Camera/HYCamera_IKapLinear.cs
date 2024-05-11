using System;

namespace HY.Devices.Camera
{
    public class HYCamera_IKapLinear_BoardPCIE : AbstractHYCamera_IKap_BoardPCIE
    {
        public HYCamera_IKapLinear_BoardPCIE(uint cameraIndex, string configFilename) : base(cameraIndex, configFilename)
        {
        }
        public HYCamera_IKapLinear_BoardPCIE(string serialNum, string configFilename) : base(serialNum, configFilename)
        {
        }
        public override CameraTypes CameraType => CameraTypes.Linear;
        public override bool SaveImage(string savePath)
        {
            throw new NotImplementedException();
        }

        public override bool SoftWareTrigger()
        {
            throw new NotImplementedException();
        }
    }
    public class HYCamera_IKapLinear_BoardPCIE_New : AbstractHYCamera_IKap_BoardPCIE_New
    {
        public HYCamera_IKapLinear_BoardPCIE_New(uint cameraIndex, uint boardIndex, string configFilename, BoardInfoClasss boardInfoClasss= BoardInfoClasss.CameraLink) : base(cameraIndex, boardIndex, configFilename, boardInfoClasss)
        {
        }
        public HYCamera_IKapLinear_BoardPCIE_New(string serialNum, uint boardIndex, string configFilename, BoardInfoClasss boardInfoClasss = BoardInfoClasss.CameraLink) : base(serialNum, boardIndex,configFilename, boardInfoClasss)
        {
        }
        public override CameraTypes CameraType => CameraTypes.Linear;
        public override bool SaveImage(string savePath)
        {
            throw new NotImplementedException();
        }

        public override bool SoftWareTrigger()
        {
            throw new NotImplementedException();
        }
    }
    public class HYCamera_IKapLinear_Net : AbstractHYCamera_IKap_Net
    {
        public HYCamera_IKapLinear_Net(uint cameraIndex) : base(cameraIndex)
        {
        }
        public HYCamera_IKapLinear_Net(string serialNum) : base(serialNum)
        {
        }
        public override CameraTypes CameraType => CameraTypes.Linear;
        public override bool SaveImage(string savePath)
        {
            throw new NotImplementedException();
        }

        public override bool SoftWareTrigger()
        {
            throw new NotImplementedException();
        }
    }
}
