using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hx.Components.Interface
{
    public interface IQuery
    {
        string Column { get; set; }                                 //列名
        string TableName { get; set; }                              //表名
        string OrderBy { get; set; }                                //查询的"OrderBy"语句，不包含OrderBy，例子：ID DESC
        string BulidQuery();                                        //查询的"where"语句
        string BulidSelect(string where, string tableName = "");     //查语句
    }
}
