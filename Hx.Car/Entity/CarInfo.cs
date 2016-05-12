using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hx.Car.Entity
{
    [Serializable]
    public class CarInfo
    {
        public CarInfo()
        {
            cYhnr = string.Empty;
            cJzsj = string.Empty;
            xPic = string.Empty;
            dPic = string.Empty;
            fSscs = string.Empty;
            cKsjc = string.Empty;
            cXh = string.Empty;
            cQcys = string.Empty;
            cXntd = string.Empty;
            cRjjh = string.Empty;
        }

        public int id{ get; set; } 
        public int fPp{ get; set; } 
        public int fXhid{ get; set; } 
        public byte fYh{ get; set; } 
        public byte fZx{ get; set; } 
        public byte fTj{ get; set; } 
        public int fPx{ get; set; } 
        public string cYhnr{ get; set; } 
        public string cJzsj{ get; set; } 
        public string xPic{ get; set; } 
        public string dPic{ get; set; } 
        public string fSscs{ get; set; } 
        public string cKsjc{ get; set; } 
        public string cXh{ get; set; } 
        public string cQcys{ get; set; }
        public string cNsys { get; set; } 
        public string cXntd{ get; set; } 
        /// <summary>
        /// 车型名称
        /// </summary>
        public string cCxmc{ get; set; } 
        public decimal cJgqj{ get; set; } 
        /// <summary>
        /// 厂商指导价
        /// </summary>
        public decimal fZdj{ get; set; } 
        /// <summary>
        /// 厂商
        /// </summary>
        public string cChangs{ get; set; } 
        /// <summary>
        /// 级别
        /// </summary>
        public string cJibie{ get; set; }
        /// <summary>
        /// 发动机
        /// </summary>
        public string cFdj{ get; set; } 
        /// <summary>
        /// 变速箱
        /// </summary>
        public string cBsx{ get; set; }
        /// <summary>
        /// 长宽高
        /// </summary>
        public string cCkg{ get; set; } 
        /// <summary>
        /// 车身结构
        /// </summary>
        public string cCsjg{ get; set; } 
        /// <summary>
        /// 最高车速
        /// </summary>
        public string cZgcs{ get; set; } 
        /// <summary>
        /// 官方0-100km/h加速(s)
        /// </summary>
        public string cGfjs{ get; set; } 
        /// <summary>
        /// 实测0-100km/h加速(s)
        /// </summary>
        public string cScjs{ get; set; } 
        /// <summary>
        /// 实测100-0km/h制动(m)
        /// </summary>
        public string cSczd{ get; set; }
        /// <summary>
        /// 实测油耗(L/100km)
        /// </summary>
        public string cScyh{ get; set; } 
        /// <summary>
        /// 工信部综合油耗(L/100km)
        /// </summary>
        public string cGxbyh{ get; set; }
        /// <summary>
        /// 整车质保
        /// </summary>
        public string cZczb{ get; set; }
        /// <summary>
        /// 长度(mm)
        /// </summary>
        public string cChang{ get; set; }
        /// <summary>
        /// 宽度(mm)
        /// </summary>
        public string cKuan{ get; set; } 
        /// <summary>
        /// 高度(mm)
        /// </summary>
        public string cGao{ get; set; } 
        /// <summary>
        /// 轴距(mm)
        /// </summary>
        public string cZhouju{ get; set; }
        /// <summary>
        /// 前轮距(mm)
        /// </summary>
        public string cQnju{ get; set; } 
        /// <summary>
        /// 后轮距(mm)
        /// </summary>
        public string cHnju{ get; set; }
        /// <summary>
        /// 最小离地间隙(mm)
        /// </summary>
        public string cLdjx{ get; set; } 
        /// <summary>
        /// 整备质量(kg)
        /// </summary>
        public string cZbzl{ get; set; } 
        /// <summary>
        /// 车身结构
        /// </summary>
        public string cChesjg{ get; set; }
        /// <summary>
        /// 车门数(个)
        /// </summary>
        public string cCms{ get; set; } 
        /// <summary>
        /// 座位数(个)
        /// </summary>
        public string cZws{ get; set; } 
        /// <summary>
        /// 油箱容积(L)
        /// </summary>
        public string cYxrj{ get; set; } 
        /// <summary>
        /// 行李厢容积(L)
        /// </summary>
        public string cXlxrj{ get; set; } 
        /// <summary>
        /// 发动机型号
        /// </summary>
        public string cFdjxh{ get; set; } 
        /// <summary>
        /// 排量(mL)
        /// </summary>
        public string fPail{ get; set; }
        /// <summary>
        /// 进气形式
        /// </summary>
        public string cJqxs{ get; set; }
        /// <summary>
        /// 气缸排列形式
        /// </summary>
        public string cQgpl{ get; set; } 
        /// <summary>
        /// 气缸数(个)
        /// </summary>
        public int fQgs{ get; set; } 
        /// <summary>
        /// 每缸气门数(个)
        /// </summary>
        public string cQms{ get; set; } 
        /// <summary>
        /// 压缩比
        /// </summary>
        public string cYsb{ get; set; } 
        /// <summary>
        /// 配气机构
        /// </summary>
        public string cPqjg{ get; set; } 
        /// <summary>
        /// 缸径(mm)
        /// </summary>
        public string cGangj{ get; set; } 
        /// <summary>
        /// 冲程
        /// </summary>
        public string cChongc{ get; set; }
        /// <summary>
        /// 最大马力(Ps)
        /// </summary>
        public string cZdml{ get; set; }
        /// <summary>
        /// 最大功率(kW)
        /// </summary>
        public string cZdgl{ get; set; } 
        /// <summary>
        /// 最大功率转速(rpm)
        /// </summary>
        public string cZhuans{ get; set; }
        /// <summary>
        /// 最大扭矩(N·m)
        /// </summary>
        public string cZdlz{ get; set; }
        /// <summary>
        /// 最大扭矩转速(rpm)
        /// </summary>
        public string cLzzs{ get; set; } 
        /// <summary>
        /// 发动机特有技术
        /// </summary>
        public string cTyjs{ get; set; } 
        /// <summary>
        /// 燃料形式
        /// </summary>
        public string cRlxs{ get; set; } 
        /// <summary>
        /// 燃油标号
        /// </summary>
        public string cRybh{ get; set; } 
        /// <summary>
        /// 供油方式
        /// </summary>
        public string cGyfs{ get; set; } 
        /// <summary>
        /// 缸盖材料
        /// </summary>
        public string cGgcl{ get; set; } 
        /// <summary>
        /// 缸体材料
        /// </summary>
        public string cGtcl{ get; set; } 
        /// <summary>
        /// 环保标准
        /// </summary>
        public string cHbbz{ get; set; }
        /// <summary>
        /// 变速箱简称
        /// </summary>
        public string cJianc{ get; set; }
        /// <summary>
        /// 挡位个数
        /// </summary>
        public string cDwgs{ get; set; }
        /// <summary>
        /// 变速箱类型
        /// </summary>
        public string cBsxlx{ get; set; }
        /// <summary>
        /// 驱动方式
        /// </summary>
        public string cQdfs{ get; set; } 
        /// <summary>
        /// 前悬挂类型
        /// </summary>
        public string cQxglx{ get; set; }
        /// <summary>
        /// 后悬挂类型
        /// </summary>
        public string cHxglx{ get; set; } 
        /// <summary>
        /// 助力类型
        /// </summary>
        public string cZllx{ get; set; }
        /// <summary>
        /// 车体结构
        /// </summary>
        public string cCtjg{ get; set; }
        /// <summary>
        /// 前制动器类型
        /// </summary>
        public string cQzdq{ get; set; }
        /// <summary>
        /// 后制动器类型
        /// </summary>
        public string cHzdq{ get; set; } 
        /// <summary>
        /// 驻车制动类型
        /// </summary>
        public string cZczd{ get; set; } 
        /// <summary>
        /// 前轮胎规格
        /// </summary>
        public string cQnt{ get; set; }
        /// <summary>
        /// 后轮胎规格
        /// </summary>
        public string cHnt{ get; set; }
        /// <summary>
        /// 备胎规格
        /// </summary>
        public string cBetai{ get; set; }
        /// <summary>
        /// 驾驶座安全气囊
        /// </summary>
        public string cJszqls{ get; set; } 
        /// <summary>
        /// 副驾驶安全气囊
        /// </summary>
        public string cFjsqls{ get; set; } 
        /// <summary>
        /// 前排侧气囊
        /// </summary>
        public string cQpcql{ get; set; } 
        /// <summary>
        /// 后排侧气囊
        /// </summary>
        public string cHpcql{ get; set; } 
        /// <summary>
        /// 前排头部气囊(气帘)
        /// </summary>
        public string cQptb{ get; set; } 
        /// <summary>
        /// 后排头部气囊(气帘)
        /// </summary>
        public string cHptb{ get; set; } 
        /// <summary>
        /// 膝部气囊
        /// </summary>
        public string cQbql{ get; set; } 
        /// <summary>
        /// 胎压监测装置
        /// </summary>
        public string cTyjc{ get; set; } 
        /// <summary>
        /// 零胎压继续行驶
        /// </summary>
        public string cLty{ get; set; } 
        /// <summary>
        /// 安全带未系提示
        /// </summary>
        public string cHqdts{ get; set; }
        /// <summary>
        /// ISO FIX儿童座椅接口
        /// </summary>
        public string cIso{ get; set; } 
        /// <summary>
        /// LATCH儿童座椅接口
        /// </summary>
        public string cLatch{ get; set; } 
        /// <summary>
        /// 发动机电子防盗
        /// </summary>
        public string cFdjfd{ get; set; } 
        /// <summary>
        /// 车内中控锁
        /// </summary>
        public string cCnzks{ get; set; } 
        /// <summary>
        /// 遥控钥匙
        /// </summary>
        public string cYkys{ get; set; } 
        /// <summary>
        /// 无钥匙启动系统
        /// </summary>
        public string cWysqd{ get; set; }
        /// <summary>
        /// ABS防抱死
        /// </summary>
        public string cAbs{ get; set; } 
        /// <summary>
        /// 制动力分配(EBD/CBC等)
        /// </summary>
        public string cCbc{ get; set; } 
        /// <summary>
        /// 刹车辅助(EBA/BAS/BA等)
        /// </summary>
        public string cBa{ get; set; } 
        /// <summary>
        /// 牵引力控制(ASR/TCS/TRC等)
        /// </summary>
        public string cTrc{ get; set; }
        /// <summary>
        /// 车身稳定控制(ESP/DSC/VSC等)
        /// </summary>
        public string cVsc{ get; set; }
        /// <summary>
        /// 自动驻车/上坡辅助
        /// </summary>
        public string cZdzc{ get; set; } 
        /// <summary>
        /// 陡坡缓降
        /// </summary>
        public string cDphj{ get; set; } 
        /// <summary>
        /// 可变悬挂
        /// </summary>
        public string cKbxg{ get; set; } 
        /// <summary>
        /// 空气悬挂
        /// </summary>
        public string cKqxg{ get; set; } 
        /// <summary>
        /// 可变转向比
        /// </summary>
        public string cKbzxb{ get; set; } 
        /// <summary>
        /// 电动天窗
        /// </summary>
        public string cZdtc{ get; set; } 
        /// <summary>
        /// 全景天窗
        /// </summary>
        public string cQjtc{ get; set; } 
        /// <summary>
        /// 运动外观套件
        /// </summary>
        public string cYdwg{ get; set; } 
        /// <summary>
        /// 铝合金轮毂
        /// </summary>
        public string cLhjn{ get; set; } 
        /// <summary>
        /// 电动吸合门
        /// </summary>
        public string cDdxhm{ get; set; } 
        /// <summary>
        /// 真皮方向盘
        /// </summary>
        public string cZpfxp{ get; set; } 
        /// <summary>
        /// 方向盘上下调节
        /// </summary>
        public string cSxtj{ get; set; } 
        /// <summary>
        /// 方向盘前后调节
        /// </summary>
        public string cQhtj{ get; set; } 
        /// <summary>
        /// 方向盘电动调节
        /// </summary>
        public string cDdtj{ get; set; } 
        /// <summary>
        /// 多功能方向盘
        /// </summary>
        public string cDgnfxp{ get; set; }
        /// <summary>
        /// 方向盘换挡
        /// </summary>
        public string cFxphd{ get; set; } 
        /// <summary>
        /// 定速巡航
        /// </summary>
        public string cDsxh{ get; set; } 
        /// <summary>
        /// 泊车辅助
        /// </summary>
        public string cBcfz{ get; set; } 
        /// <summary>
        /// 倒车视频影像
        /// </summary>
        public string cDcsp{ get; set; } 
        /// <summary>
        /// 行车电脑显示屏
        /// </summary>
        public string cDnxsp{ get; set; } 
        /// <summary>
        /// HUD抬头数字显示
        /// </summary>
        public string cHud{ get; set; } 
        /// <summary>
        /// 真皮/仿皮座椅
        /// </summary>
        public string cZpzy{ get; set; } 
        /// <summary>
        /// 运动座椅
        /// </summary>
        public string cYdzy{ get; set; }
        /// <summary>
        /// 座椅高低调节
        /// </summary>
        public string cZygd{ get; set; } 
        /// <summary>
        /// 腰部支撑调节
        /// </summary>
        public string cYbzc{ get; set; } 
        /// <summary>
        /// 肩部支撑调节
        /// </summary>
        public string cJbzc{ get; set; } 
        /// <summary>
        /// 前排座椅电动调节
        /// </summary>
        public string cQpddtj{ get; set; }
        /// <summary>
        /// 第二排靠背角度调节
        /// </summary>
        public string cEpjdtj{ get; set; }
        /// <summary>
        /// 第二排座椅移动
        /// </summary>
        public string cEpzyyd{ get; set; }
        /// <summary>
        /// 后排座椅电动调节
        /// </summary>
        public string cHptj{ get; set; } 
        /// <summary>
        /// 电动座椅记忆
        /// </summary>
        public string cDdjy{ get; set; } 
        /// <summary>
        /// 前排座椅加热
        /// </summary>
        public string cQpzyjr{ get; set; }
        /// <summary>
        /// 后排座椅加热
        /// </summary>
        public string cHpzyjr{ get; set; }
        /// <summary>
        /// 座椅通风
        /// </summary>
        public string cZytf{ get; set; } 
        /// <summary>
        /// 座椅按摩
        /// </summary>
        public string cZyam{ get; set; } 
        /// <summary>
        /// 后排座椅整体放倒
        /// </summary>
        public string cHpztpf{ get; set; } 
        /// <summary>
        /// 后排座椅比例放倒
        /// </summary>
        public string cHpblpf{ get; set; }
        /// <summary>
        /// 第三排座椅
        /// </summary>
        public string cSpzy{ get; set; } 
        /// <summary>
        /// 前座中央扶手
        /// </summary>
        public string cQzfs{ get; set; } 
        /// <summary>
        /// 后座中央扶手
        /// </summary>
        public string cHzfs{ get; set; } 
        /// <summary>
        /// 后排杯架
        /// </summary>
        public string cHphj{ get; set; } 
        /// <summary>
        /// 电动后备厢
        /// </summary>
        public string cDdhbx{ get; set; } 
        /// <summary>
        /// GPS导航系统
        /// </summary>
        public string cGps{ get; set; } 
        /// <summary>
        /// 定位互动服务
        /// </summary>
        public string cDwfw{ get; set; }
        /// <summary>
        /// 中控台彩色大屏
        /// </summary>
        public string cCsdp{ get; set; }
        /// <summary>
        /// 人机交互系统
        /// </summary>
        public string cRjjh{ get; set; } 
        /// <summary>
        /// 内置硬盘
        /// </summary>
        public string cNzyp{ get; set; } 
        /// <summary>
        /// 蓝牙/车载电话
        /// </summary>
        public string cCzdh{ get; set; } 
        /// <summary>
        /// 车载电视
        /// </summary>
        public string cCzds{ get; set; } 
        /// <summary>
        /// 后排液晶屏
        /// </summary>
        public string cHpyjp{ get; set; }
        /// <summary>
        /// 外接音源接口(AUX/USB/iPod等)
        /// </summary>
        public string cIpod{ get; set; } 
        /// <summary>
        /// CD支持MP3/WMA
        /// </summary>
        public string cMp3{ get; set; } 
        /// <summary>
        /// 单碟CD
        /// </summary>
        public string cDdcd{ get; set; } 
        /// <summary>
        /// 虚拟多碟CD
        /// </summary>
        public string cXndd{ get; set; } 
        /// <summary>
        /// 多碟CD系统
        /// </summary>
        public string cDuodcd{ get; set; }
        /// <summary>
        /// 单碟DVD
        /// </summary>
        public string cDddvd{ get; set; } 
        /// <summary>
        /// 多碟DVD系统
        /// </summary>
        public string cDuodvd{ get; set; } 
        /// <summary>
        /// 2-3喇叭扬声器系统
        /// </summary>
        public string c23lb{ get; set; }
        /// <summary>
        /// 4-5喇叭扬声器系统
        /// </summary>
        public string c45lb{ get; set; } 
        /// <summary>
        /// 6-7喇叭扬声器系统
        /// </summary>
        public string c67lb{ get; set; } 
        /// <summary>
        /// ≥8喇叭扬声器系统
        /// </summary>
        public string c8lb{ get; set; } 
        /// <summary>
        /// 氙气大灯
        /// </summary>
        public string cXqdd{ get; set; }
        /// <summary>
        /// LED大灯
        /// </summary>
        public string cLed{ get; set; }
        /// <summary>
        /// 日间行车灯
        /// </summary>
        public string cRjxcd{ get; set; } 
        /// <summary>
        /// 自动头灯
        /// </summary>
        public string cZdtd{ get; set; } 
        /// <summary>
        /// 转向头灯(辅助灯)
        /// </summary>
        public string cZxtd{ get; set; } 
        /// <summary>
        /// 前雾灯
        /// </summary>
        public string cQwd{ get; set; } 
        /// <summary>
        /// 大灯高度可调
        /// </summary>
        public string cGdkt{ get; set; } 
        /// <summary>
        /// 大灯清洗装置
        /// </summary>
        public string cQxzz{ get; set; } 
        /// <summary>
        /// 车内氛围灯
        /// </summary>
        public string cCnfwd{ get; set; } 
        /// <summary>
        /// 前电动车窗
        /// </summary>
        public string cQddcc{ get; set; }
        /// <summary>
        /// 后电动车窗
        /// </summary>
        public string cHddcc{ get; set; }
        /// <summary>
        /// 车窗防夹手功能
        /// </summary>
        public string cFjs{ get; set; } 
        /// <summary>
        /// 防紫外线/隔热玻璃
        /// </summary>
        public string cFzwx{ get; set; } 
        /// <summary>
        /// 后视镜电动调节
        /// </summary>
        public string cHsjtj{ get; set; } 
        /// <summary>
        /// 后视镜加热
        /// </summary>
        public string cHsjjr{ get; set; }
        /// <summary>
        /// 后视镜自动防眩目
        /// </summary>
        public string cFxm{ get; set; } 
        /// <summary>
        /// 后视镜电动折叠
        /// </summary>
        public string cZdzd{ get; set; } 
        /// <summary>
        /// 后视镜记忆
        /// </summary>
        public string cHsjjy{ get; set; } 
        /// <summary>
        /// 后风挡遮阳帘
        /// </summary>
        public string cHfdz{ get; set; } 
        /// <summary>
        /// 后排侧遮阳帘
        /// </summary>
        public string cHpcz{ get; set; } 
        /// <summary>
        /// 遮阳板化妆镜
        /// </summary>
        public string cHzj{ get; set; } 
        /// <summary>
        /// 后雨刷
        /// </summary>
        public string cHys{ get; set; } 
        /// <summary>
        /// 感应雨刷
        /// </summary>
        public string cGyys{ get; set; } 
        /// <summary>
        /// 手动空调
        /// </summary>
        public string cSdkt{ get; set; }
        /// <summary>
        /// 自动空调
        /// </summary>
        public string cZdkt{ get; set; }
        /// <summary>
        /// 后排独立空调
        /// </summary>
        public string cDlkt{ get; set; } 
        /// <summary>
        /// 后座出风口
        /// </summary>
        public string cHzcfk{ get; set; } 
        /// <summary>
        /// 温度分区控制
        /// </summary>
        public string cWdkz{ get; set; } 
        /// <summary>
        /// 空气调节/花粉过滤
        /// </summary>
        public string cKqtj{ get; set; } 
        /// <summary>
        /// 车载冰箱
        /// </summary>
        public string cCzbx{ get; set; } 
        /// <summary>
        /// 自动泊车入位
        /// </summary>
        public string cPcrw{ get; set; } 
        /// <summary>
        /// 并线辅助
        /// </summary>
        public string cBxfz{ get; set; } 
        /// <summary>
        /// 主动刹车/主动安全系统
        /// </summary>
        public string cZdsc{ get; set; }
        /// <summary>
        /// 整体主动转向系统
        /// </summary>
        public string cZtzx{ get; set; } 
        /// <summary>
        /// 夜视系统
        /// </summary>
        public string cYsxt{ get; set; }
        /// <summary>
        /// 中控液晶屏分屏显示
        /// </summary>
        public string cFpxs{ get; set; } 
        /// <summary>
        /// 自适应巡航
        /// </summary>
        public string cZsyxh{ get; set; } 
        /// <summary>
        /// 全景摄像头
        /// </summary>
        public string cQjsxt{ get; set; } 
        public int fHit{ get; set; }
        public byte fDel { get; set; }
    
    }
}
