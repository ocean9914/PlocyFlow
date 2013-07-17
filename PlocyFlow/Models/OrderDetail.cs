using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;
using PlocyFlow.DAL.Context;
using PlocyFlow.DAL.CommonUtility;
using PlocyFlow.DAL.Entity;
namespace PlocyFlow.Models
{
    public class OrderDetail:IDisposable
    {
        private string _orderId;
        public string OrderId { get { return _orderId; } }
        private string _workType;
        public string WorkedType { get { return _workType; } }
        private string _staute;
        public string Staute { get { return _staute; } }
        private string _productId;
        public string ProductId { get { return _productId; } }
        private string _productName;
        public string ProductName { get { return _productName; } }
        private string _deptId;
        public string DeptId { get { return _deptId; } }
        private string _deptName;
        public string DeptName { get { return _deptName; } }
        private string _operdate;
        public string OperDate { get { return _operdate; } }
        private string _attachUrl;
        public string AttachUrl { get { return _attachUrl; } }
        private bool _isAdmin;
        public bool IsAdmin { get { return _isAdmin; } }
        private List<Order_Flow_Detail> _detailData;
        public List<Order_Flow_Detail> DetailData { get { return _detailData; } }
        private bool _hasOrder;
        public bool HasOrder { get { return _hasOrder; } }
        private string _userName;
        private string _userlist;
        public string UserList { get { return _userlist; } }
        private string _elapsedlist;
        public string ElapsedList { get { return _elapsedlist; } }
        private string _complainId;
        public OrderDetail(string orderId,string user_name)
        {
            _orderId = orderId;
            _userName = user_name;
            _isAdmin = false;
            _userlist = "";
            _elapsedlist = "";
            if (_orderId != null && _orderId.Trim() != "")
            {
                string tp = _orderId.Trim().Substring(0, 1);
                switch (tp)
                {
                    case "1":
                        GetDeclareOrder();
                        break;
                    case "3":
                        GetComplainOrder();
                        break;
                }
                if (_hasOrder)
                {
                    t_user u = t_userDataContext.Get_User(_userName);
                    if (u!=null && u.role_id.HasValue && u.role_id.Value == 1)
                        _isAdmin = true;
                }
            }
        }
        private void GetDeclareOrder()
        {
            if (_orderId != null && _orderId != "")
            {
                string sql = "select o.order_Id,o.product_Id,b.ProductName, o.policyId,p.p_name as policyName,o.staute,b.Departmentid,d.d_name,o.oper_date,o.attach_url,o.next_oper "
                         + " from declareorder o,bip_data b, dept d,policy p "
                         + " where o.order_Id='"+_orderId+"' "
                         + " and o.product_Id=b.Productid and o.policyId=b.Policyid "
                         + " and b.Departmentid=d.d_id and b.Policyid=p.p_id ";
                DataTable dt = DBHelper.GetDataTable(sql);
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow tr = dt.Rows[0];
                    _workType = "申报" + tr[4].ToString();
                    _staute = CommonUtility.TryParseStringToInt32(tr[5].ToString()) > 0 ? "已关闭" : tr[10].ToString();
                    _productId = tr[1].ToString();
                    _productName = tr[2].ToString();
                    _deptId = tr[6].ToString();
                    _deptName = tr[7].ToString();
                    _operdate = Convert.ToDateTime(tr[8]).ToString("yyyy-MM-dd");
                    _attachUrl = tr[9].ToString().Trim();
                    _detailData = new List<Order_Flow_Detail>();
                    var deList = declareFlowDataContext.GetDeclareFlowByOrder(_orderId);
                    if (deList != null)
                    {
                        DateTime lastOperDate = DateTime.MinValue;
                        Queue<string> us = new Queue<string>();
                        Queue<string> es = new Queue<string>();
                        foreach (declareflow df in deList)
                        {
                            Order_Flow_Detail ofd = new Order_Flow_Detail();
                            ofd.OperDate = df.oper_date;
                            ofd.UserName = df.oper_user;
                            ofd.OperType = OperList.getOT_ByValue(df.oper_type);
                            ofd.OperMemo = df.oper_memo;
                            ofd.AttachUrl = "";
                            if (lastOperDate == DateTime.MinValue)
                            {
                                ofd.Elapsed = 0f;
                            }
                            else
                            {
                                TimeSpan ts = new TimeSpan(df.oper_date.Ticks - lastOperDate.Ticks);
                                ofd.Elapsed = ts.TotalMinutes / 60f;
                            }
                            us.Enqueue(df.oper_user);
                            es.Enqueue(ofd.Elapsed > 0 ? ofd.Elapsed.ToString("0.0") : "0");
                            _detailData.Add(ofd);
                            lastOperDate = df.oper_date;
                        }
                        if (us.Count > 0)
                        {
                            _userlist = us.Dequeue();
                            while (us.Count > 0)
                            {
                                _userlist += "," + us.Dequeue();
                            }
                        }
                        if (es.Count > 0)
                        {
                            _elapsedlist = es.Dequeue();
                            while (es.Count > 0)
                            {
                                _elapsedlist += "," + es.Dequeue();
                            }
                        }
                    }
                    _hasOrder = true;
                }
            }
        }
        
        private void GetComplainOrder()
        {
            if (_orderId != null && _orderId != "")
            {
                string sql = "select distinct o.order_Id,o.productId,o.productName,o.complainId,o.complainName ,o.staute,b.Departmentid,d.d_name,o.oper_date,o.attach_url,o.next_oper"
                          + " from complainorder o,bip_data b, dept d"
                          + " where o.order_Id='" + _orderId + "' "
                          + " and o.productId=b.Productid and b.Departmentid=d.d_id ";
                DataTable dt = DBHelper.GetDataTable(sql);
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow tr = dt.Rows[0];
                    _workType = "投诉" + tr[4].ToString();
                    _staute = CommonUtility.TryParseStringToInt32(tr[5].ToString()) > 0 ? "已关闭" : tr[10].ToString();
                    _productId = tr[1].ToString();
                    _productName = tr[2].ToString();
                    _complainId = tr[3].ToString();
                    _deptId = tr[6].ToString();
                    _deptName = tr[7].ToString();
                    _operdate = Convert.ToDateTime(tr[8]).ToString("yyyy-MM-dd");
                    _attachUrl = tr[9].ToString().Trim();
                    _detailData = new List<Order_Flow_Detail>();
                    var deList = complainFlowDataContext.GetComplainFlowByOrder(_orderId);
                    if (deList != null)
                    {
                        DateTime lastOperDate = DateTime.MinValue;
                        Queue<string> us = new Queue<string>();
                        Queue<string> es = new Queue<string>();
                        foreach (complainflow df in deList)
                        {
                            Order_Flow_Detail ofd = new Order_Flow_Detail();
                            ofd.OperDate = df.oper_date;
                            ofd.UserName = df.oper_user;
                            ofd.OperType = OperList.getOT_ByValue(df.oper_type);
                            ofd.OperMemo = df.oper_memo;
                            ofd.AttachUrl = df.attach_url.Trim();
                            if (lastOperDate == DateTime.MinValue)
                            {
                                ofd.Elapsed = 0f;
                            }
                            else
                            {
                                TimeSpan ts = new TimeSpan(df.oper_date.Ticks - lastOperDate.Ticks);
                                ofd.Elapsed = ts.TotalMinutes / 60f;
                            }
                            us.Enqueue(df.oper_user);
                            es.Enqueue(ofd.Elapsed>0 ? ofd.Elapsed.ToString("0.0"):"0");
                            _detailData.Add(ofd);
                            lastOperDate = df.oper_date;
                        }
                        if (us.Count > 0)
                        {
                            _userlist = us.Dequeue();
                            while (us.Count > 0)
                            {
                                _userlist += "," + us.Dequeue();
                            }
                        }
                        if (es.Count > 0)
                        {
                            _elapsedlist = es.Dequeue();
                            while (es.Count > 0)
                            {
                                _elapsedlist += "," + es.Dequeue();
                            }
                        }
                    }
                    _hasOrder = true;
                }
            }
        }
        public void Dispose()
        {
            if (_detailData != null)
                _detailData.Clear();
            _detailData = null;
        }
        public void CloseOrder()
        {
            if (_hasOrder && _isAdmin)
            {
                if (_orderId != null && _orderId.Trim() != "")
                {
                    string tp = _orderId.Trim().Substring(0, 1);
                    switch (tp)
                    {
                        case "1":
                            closeDeclareOrder();
                            break;
                        case "3":
                            closeComplainOrder();
                            break;
                    }
                   
                }
            }
        }
        private void closeDeclareOrder()
        {
            bool r = false;
            using (DBHelperTran tran = new DBHelperTran(DBHelper.mysqlConnection))
            {
                string sql = "update declareorder set staute=1,next_oper=?next_oper,last_oper=?last_oper,last_oper_date=sysdate(),memo1=?last_memo where order_id=?order_id";
                MySqlParameter[] paras = new MySqlParameter[4];
                paras[0] = new MySqlParameter("next_oper", MySqlDbType.VarChar); paras[0].Value = "";
                paras[1] = new MySqlParameter("last_oper", MySqlDbType.VarChar); paras[1].Value = _userName;
                paras[2] = new MySqlParameter("last_memo", MySqlDbType.VarChar); paras[2].Value = "管理员关闭";
                paras[3] = new MySqlParameter("order_id", MySqlDbType.VarChar); paras[3].Value = _orderId;
                try
                {
                    tran.ExcuteSQL(sql, paras);
                    string sn = BaseId.getDeclareFlowId();
                    sql = "insert into declareflow(sn,order_id,oper_type,oper_memo,oper_date,oper_user,memo) values(?sn,?order_id,?oper_type,?oper_memo,sysdate(),?oper_user,'1')";
                    paras = new MySqlParameter[5];
                    paras[0] = new MySqlParameter("sn", MySqlDbType.VarChar); paras[0].Value = sn;
                    paras[1] = new MySqlParameter("order_id", MySqlDbType.VarChar); paras[1].Value = _orderId;
                    paras[2] = new MySqlParameter("oper_type", OperList.Close.Oper_Value);
                    paras[3] = new MySqlParameter("oper_memo", "管理员关闭");
                    paras[4] = new MySqlParameter("oper_user", _userName);
                    tran.ExcuteSQL(sql, paras);
                    tran.Commit();
                    r = true;
                }
                catch { tran.Roback(); }
            }
            if (r)
            {
                GetDeclareOrder();
            }
        }
        private void closeComplainOrder()
        {
            bool r = false;
            using (DBHelperTran tran = new DBHelperTran(DBHelper.mysqlConnection))
            {
                string sql = "update complainorder set staute=1,next_oper=?next_oper,last_oper=?last_oper,last_oper_date=sysdate(),last_memo=?last_memo where order_id=?order_id";
                MySqlParameter[] paras = new MySqlParameter[4];
                paras[0] = new MySqlParameter("next_oper", MySqlDbType.VarChar); paras[0].Value = "";
                paras[1] = new MySqlParameter("last_oper", MySqlDbType.VarChar); paras[1].Value = _userName;
                paras[2] = new MySqlParameter("last_memo", MySqlDbType.VarChar); paras[2].Value = "管理员关闭";
                paras[3] = new MySqlParameter("order_id", MySqlDbType.VarChar); paras[3].Value = _orderId;
                try
                {
                    tran.ExcuteSQL(sql, paras);
                    string sn = BaseId.getDeclareFlowId();
                    sql = "insert into complainflow(sn,orderId,complainId,pm,attach_url,oper_type,oper_memo,oper_date,oper_user,memo) values(?sn,?order_id,?complainId,?pm,?attach_url,?oper_type,?oper_memo,sysdate(),?oper_user,'0')";
                    paras = new MySqlParameter[8];
                    paras[0] = new MySqlParameter("sn", MySqlDbType.VarChar); paras[0].Value = sn;
                    paras[1] = new MySqlParameter("order_id", MySqlDbType.VarChar); paras[1].Value = _orderId;
                    paras[2] = new MySqlParameter("complainId", _complainId);
                    paras[3] = new MySqlParameter("pm", "");
                    paras[4] = new MySqlParameter("attach_url", "");
                    paras[5] = new MySqlParameter("oper_type", OperList.Close.Oper_Value);
                    paras[6] = new MySqlParameter("oper_memo", "管理员关闭");
                    paras[7] = new MySqlParameter("oper_user", _userName);
                    tran.ExcuteSQL(sql, paras);
                    tran.Commit();
                    if (!tran.HasErr)
                        r = true;
                }
                catch  { tran.Roback();}

            }
            if (r)
            {
                GetComplainOrder();
            }
        }
        public void updateNextOpr(string nextOperator)
        {
            if (_hasOrder && _isAdmin)
            {
                if (_orderId != null && _orderId.Trim() != "")
                {
                    string tp = _orderId.Trim().Substring(0, 1);
                    switch (tp)
                    {
                        case "1":
                            updateDeclareNextOpr(nextOperator);
                            break;
                        case "3":
                            updateComplainNextOpr(nextOperator);
                            break;
                    }

                }
            }
        }
        private void updateDeclareNextOpr(string nextOperator)
        {
            bool r = false;
            if (!string.IsNullOrEmpty(nextOperator))
            {
                using (DBHelperTran tran = new DBHelperTran(DBHelper.mysqlConnection))
                {
                    string sql = "update declareorder set next_oper=?next_oper,last_oper=?last_oper,last_oper_date=sysdate(),memo1=?last_memo where order_id=?order_id";
                    MySqlParameter[] paras = new MySqlParameter[4];
                    string memo = "此单由管理员转交给:" + nextOperator + "处理";
                    paras[0] = new MySqlParameter("next_oper", MySqlDbType.VarChar); paras[0].Value = nextOperator;
                    paras[1] = new MySqlParameter("last_oper", MySqlDbType.VarChar); paras[1].Value = _userName;
                    paras[2] = new MySqlParameter("last_memo", MySqlDbType.VarChar); paras[2].Value = memo;
                    paras[3] = new MySqlParameter("order_id", MySqlDbType.VarChar); paras[3].Value = _orderId;
                    try
                    {
                        tran.ExcuteSQL(sql, paras);
                        string sn = BaseId.getDeclareFlowId();
                        sql = "insert into declareflow(sn,order_id,oper_type,oper_memo,oper_date,oper_user,memo) values(?sn,?order_id,?oper_type,?oper_memo,sysdate(),?oper_user,'1')";
                        paras = new MySqlParameter[5];
                        paras[0] = new MySqlParameter("sn", MySqlDbType.VarChar); paras[0].Value = sn;
                        paras[1] = new MySqlParameter("order_id", MySqlDbType.VarChar); paras[1].Value = _orderId;
                        paras[2] = new MySqlParameter("oper_type", OperList.Close.Oper_Value);
                        paras[3] = new MySqlParameter("oper_memo", memo);
                        paras[4] = new MySqlParameter("oper_user", _userName);
                        tran.ExcuteSQL(sql, paras);
                        tran.Commit();
                        r = true;
                    }
                    catch { tran.Roback(); }
                }
            }
            if (r)
            {
                GetDeclareOrder();
            }
        }
        private void updateComplainNextOpr(string nextOperator)
        {
            bool r = false;
            if (!string.IsNullOrEmpty(nextOperator))
            {
                using (DBHelperTran tran = new DBHelperTran(DBHelper.mysqlConnection))
                {
                    string sql = "update complainorder set next_oper=?next_oper,last_oper=?last_oper,last_oper_date=sysdate(),last_memo=?last_memo where order_id=?order_id";
                    MySqlParameter[] paras = new MySqlParameter[4];
                    string memo = "此单由管理员转交给:" + nextOperator + "处理";
                    paras[0] = new MySqlParameter("next_oper", MySqlDbType.VarChar); paras[0].Value = nextOperator;
                    paras[1] = new MySqlParameter("last_oper", MySqlDbType.VarChar); paras[1].Value = _userName;
                    paras[2] = new MySqlParameter("last_memo", MySqlDbType.VarChar); paras[2].Value = memo;
                    paras[3] = new MySqlParameter("order_id", MySqlDbType.VarChar); paras[3].Value = _orderId;
                    try
                    {
                        tran.ExcuteSQL(sql, paras);
                        string sn = BaseId.getDeclareFlowId();
                        sql = "insert into complainflow(sn,orderId,complainId,pm,attach_url,oper_type,oper_memo,oper_date,oper_user,memo) values(?sn,?order_id,?complainId,?pm,?attach_url,?oper_type,?oper_memo,sysdate(),?oper_user,'0')";
                        paras = new MySqlParameter[8];
                        paras[0] = new MySqlParameter("sn", MySqlDbType.VarChar); paras[0].Value = sn;
                        paras[1] = new MySqlParameter("order_id", MySqlDbType.VarChar); paras[1].Value = _orderId;
                        paras[2] = new MySqlParameter("complainId", _complainId);
                        paras[3] = new MySqlParameter("pm", nextOperator);
                        paras[4] = new MySqlParameter("attach_url", "");
                        paras[5] = new MySqlParameter("oper_type", OperList.To.Oper_Value);
                        paras[6] = new MySqlParameter("oper_memo", memo);
                        paras[7] = new MySqlParameter("oper_user", _userName);
                        tran.ExcuteSQL(sql, paras);
                        tran.Commit();
                        if (!tran.HasErr)
                            r = true;
                    }
                    catch { tran.Roback(); }

                }
            }
            if (r)
            {
                GetComplainOrder();
            }
        }
    }
}