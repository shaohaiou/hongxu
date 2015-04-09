using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HX.CheShangBao
{
    [System.Runtime.InteropServices.ComVisible(true)]
    public class ServerJsToClient
    {
        public ServerJsToClient() { }

        public ServerJsToClient(Default frmDefault)
        {
            CurrentDefault = frmDefault;
        }

        public Default CurrentDefault { get; set; }

        /// <summary>
        /// 营销推广
        /// </summary>
        /// <param name="cid"></param>
        public void Yxtg(string cid)
        {
            int id = int.Parse(cid);
            Yxtg formYxtg = new Yxtg();
            formYxtg.defaultform = CurrentDefault;
            formYxtg.carid = id;
            formYxtg.Show();
        }

        /// <summary>
        /// 一键营销
        /// </summary>
        /// <param name="cid"></param>
        public void Yjyx(string cid)
        {
            int id = int.Parse(cid);
            Yjyx formYjyx = new Yjyx();
            formYjyx.defaultform = CurrentDefault;
            formYjyx.carid = id;
            formYjyx.Show();
        }

        /// <summary>
        /// 修改信息
        /// </summary>
        /// <param name="cid"></param>
        public void Xgxx(string cid)
        {
            int id = int.Parse(cid);
            AddCar formAddCar = new AddCar();
            formAddCar.defaultform = CurrentDefault;
            formAddCar.carid = id;
            formAddCar.Show();
        }

        /// <summary>
        /// 进入后台
        /// </summary>
        /// <param name="aid"></param>
        public void AutoLogin(string aid)
        {
            if (aid.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length == 1)
            {
                int id = int.Parse(aid);
                AutoLogin formAddCar = new AutoLogin();
                formAddCar.accountid = id;
                formAddCar.Show();
            }
            else if (aid.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length > 1)
            {
                AccountSel frmAccountSel = new AccountSel();
                frmAccountSel.accountids = aid;
                frmAccountSel.Show();
            }
        }

        /// <summary>
        /// 帐号管理
        /// </summary>
        /// <param name="sitetype"></param>
        public void AccountConfig(string sitetype)
        {
            int sitetypeval = int.Parse(sitetype);
            AccountConfig formAccountMg = new AccountConfig();
            formAccountMg.defaultform = CurrentDefault;
            formAccountMg.sitetypeval = sitetypeval;
            formAccountMg.Show();
        }

        public void Reload()
        {
            if (CurrentDefault != null)
                CurrentDefault.RefreshPage();
        }
    }
}
