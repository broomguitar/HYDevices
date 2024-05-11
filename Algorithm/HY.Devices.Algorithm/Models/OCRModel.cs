using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HY.Devices.Algorithm
{
   public  class OCRModel
    {
        private const int MaxObjects = 1000;
        [StructLayout(LayoutKind.Sequential)]
       public struct OCRResult
        {
            public int lt_x, lt_y, rt_x, rt_y, rb_x, rb_y, lb_x, lb_y;
            public IntPtr text;
            public float score;
            public float cls_score;
            public int cls_label;
        };
        [StructLayout(LayoutKind.Sequential)]
      public  struct OCRResultContainer
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxObjects)]
            public OCRResult[] candidates;
        }
    }
}
