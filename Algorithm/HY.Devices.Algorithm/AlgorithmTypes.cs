using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HY.Devices.Algorithm
{
    public enum AlgorithmTypes
    {
        /// <summary>
        /// 二维码
        /// </summary>
        QRcode,
        /// <summary>
        /// 条形码
        /// </summary>
        Barcode,
        /// <summary>
        /// OCR
        /// </summary>
        OCR,
        /// <summary>
        /// 模板匹配
        /// </summary>
        TemplateMatching,
        /// <summary>
        /// 检测门对齐
        /// </summary>
        DetectDoorAlign,
        /// <summary>
        /// 检测门缝
        /// </summary>
        DetectDoorCrack,
        /// <summary>
        /// 色差
        /// </summary>
        ChromaticAberration,
        /// <summary>
        /// 检测目标
        /// </summary>
        DetectTarget,
        /// <summary>
        /// 推理分类
        /// </summary>
        Classification,
        /// <summary>
        /// 门平
        /// </summary>
        DoorPlane,
        /// <summary>
        /// 行为识别
        /// </summary>
        ActionRecognition,
        /// <summary>
        /// 边缘检测
        /// </summary>
        EdgeDetection,
    }
}
