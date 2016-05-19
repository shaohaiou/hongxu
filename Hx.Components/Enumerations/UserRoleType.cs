using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hx.Components.Enumerations
{
    public enum UserRoleType
    {
        NoSet = 0,
        销售经理 = 1,
        销售员 = 2,
        人事专员 = 4,
        微信活动管理员 = 8,
        二手车估价器管理员 = 16,
        卡券活动管理员 = 32,
        广本61活动 = 64,
        投票活动管理员 = 128,
        场景二维码 = 256,
        财务出纳 =  512,
        总经理 = 1024,
        车型管理员 = 2048
    }
}
