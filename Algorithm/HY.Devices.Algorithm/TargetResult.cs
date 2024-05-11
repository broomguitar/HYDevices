namespace HY.Devices.Algorithm
{
    /// <summary>
    /// 缺陷信息
    /// </summary>
    public class TargetResult
    {
        public int ID { set; get; }
        public string TypeName { set; get; }
        public double Row1 { set; get; }
        public double Row2 { set; get; }
        public double Column1 { set; get; }
        public double Column2 { set; get; }
        public double Area { set; get; }
        public double Score { get; set; }
    }
}
