using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using MySql.Data.MySqlClient;
using PlocyFlow.DAL.Entity;
using PlocyFlow.DAL.CommonUtility;
using PlocyFlow.DAL.Context;
using Tencent.OA.Framework.Messages;
using Tencent.OA.Framework.Messages.DataContract;
namespace PlocyFlow.Models
{
    public class Approve
    {
        private string _user_name;
        private string _orderId;
        private bool _hasAuthority;
        public bool HasAuthority { get { return _hasAuthority; } }
        private string _staute;
        public string Staute { get { return _staute; } }
        private declareorder _dOrder;
        public declareorder DeclareOrder { get { return _dOrder; } }
        private string _PreOper_user;
        private string _NextOper_user;
        public string PreOper_User { get { return _PreOper_user; } }
        public string NextOper_User { get { return _NextOper_user; } }
        private List<Order_Flow_Detail> _detailData;
        public List<Order_Flow_Detail> DetailData { get { return _detailData; } }
        public Approve(string user_name, string orderId)
        {
            _msg = "";
            _user_name = user_name;
            _orderId = orderId;
            _hasAuthority = HasApprove();
            GetOrderStaute();
        }
        private string _msg;
        public string Message { get { return _msg; } }
        public UrlHelper URL { get; set; }
        public void makeData()
        {
            if (_hasAuthority && _dOrder != null)
            {
                _PreOper_user = _dOrder.oper_user;
                if (_staute == "初审")
                {
                    _NextOper_user = apr_user_listDataContext.GetLastApproves(_dOrder.policyId);
                }
                if (_staute == "终审")
                {
                    _NextOper_user = "";
                }
                _detailData = new List<Order_Flow_Detail>();
                var deList = declareFlowDataContext.GetDeclareFlowByOrder(_orderId);
                if (deList != null)
                {
                    DateTime lastOperDate = DateTime.MinValue;
                    foreach (declareflow df in deList)
                    {
                        Order_Flow_Detail ofd = new Order_Flow_Detail();
                        ofd.OperDate = df.oper_date;
                        ofd.UserName = df.oper_user;
                        ofd.OperType = OperList.getOT_ByValue(df.oper_type);
                        ofd.OperMemo = df.oper_memo;
                        ofd.AttachUrl = _dOrder.attach_url;
                        if (lastOperDate == DateTime.MinValue)
                        {
                            ofd.Elapsed = 0f;
                        }
                        else
                        {
                            TimeSpan ts = new TimeSpan(df.oper_date.Ticks - lastOperDate.Ticks);
                            ofd.Elapsed = ts.TotalMinutes / 60f;
                        }
                        _detailData.Add(ofd);
                        lastOperDate = df.oper_date;
                    }
                }
            }
        }
        private bool HasApprove()
        {
            bool r = false;
            if (_user_name != "" && _orderId != "")
            {
                _dOrder = declareOrderDataContext.GetSingleDeclareOrder(_orderId);
                if (_dOrder != null)
                {
                    string next_opr = _dOrder.next_oper;
                    if (next_opr != "")
                    {
                        next_opr = next_opr.Replace(';', ',');
                        string[] users = CommonUtility.SplitStringArray(next_opr, new char[] { ',' }, true);
                        if (users.Length > 0)
                        {
                            foreach (string u in users)
                            {
                                if (u == _user_name)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return r;
        }
        private void GetOrderStaute()
        {
            if (_hasAuthority && _dOrder != null)
            {
                string sql = "select a.id,a.name from aprove_user u,approve_type a,policy p,t_user t where u.aid=a.id and u.pid=p.id and u.uid=t.id "
                    + " and p.p_id='" + _dOrder.policyId + "' and t.user_name='" + _user_name + "' ";
                sql = "select name from(" + sql + ")b order by id desc";
                _staute = CommonUtility.ReplaceNullString(DBHelper.GetObject(sql));

            }
        }
        public bool approve_save(string memo)
        {
            bool r = false;
            _msg = "";
            if (_hasAuthority)
            {
                using (DBHelperTran tran = new DBHelperTran(DBHelper.mysqlConnection))
                {
                    string sql = "update declareorder set next_oper=?next_oper,last_oper=?last_oper,last_oper_date=sysdate(),memo1=?last_memo where order_id=?order_id";
                    string nextoper = apr_user_listDataContext.GetLastApproves(_dOrder.policyId);
                    if (nextoper == _user_name)
                        return approve_close(memo);
                    MySqlParameter[] paras = new MySqlParameter[4];
                    paras[0] = new MySqlParameter("next_oper", MySqlDbType.VarChar); paras[0].Value = nextoper;
                    paras[1] = new MySqlParameter("last_oper", MySqlDbType.VarChar); paras[1].Value = _user_name;
                    paras[2] = new MySqlParameter("last_memo", MySqlDbType.VarChar); paras[2].Value = memo;
                    paras[3] = new MySqlParameter("order_id", MySqlDbType.VarChar); paras[3].Value = _orderId;
                    try
                    {
                        tran.ExcuteSQL(sql, paras);
                        string sn = BaseId.getDeclareFlowId();
                        sql = "insert into declareflow(sn,order_id,oper_type,oper_memo,oper_date,oper_user,memo) values(?sn,?order_id,?oper_type,?oper_memo,sysdate(),?oper_user,'0')";
                        paras = new MySqlParameter[5];
                        paras[0] = new MySqlParameter("sn", MySqlDbType.VarChar); paras[0].Value = sn;
                        paras[1] = new MySqlParameter("order_id", MySqlDbType.VarChar); paras[1].Value = _orderId;
                        paras[2] = new MySqlParameter("oper_type", OperList.Handle.Oper_Value);
                        paras[3] = new MySqlParameter("oper_memo", memo);
                        paras[4] = new MySqlParameter("oper_user", _user_name);
                        tran.ExcuteSQL(sql, paras);
                        tran.Commit();
                        r = true;
                        string policyName = policyDataContext.GetPolicyName(_dOrder.policyId);
                        string productName = bip_dataDataContext.GetProductName(_dOrder.product_Id);
                        char[] sp = { ',' };
                        string[] ss = CommonUtility.SplitStringArray(nextoper, sp, true);
                        foreach (string receiver in ss)
                        {
                            if (!string.IsNullOrEmpty(receiver))
                            {
                                string message = string.Format("[政策电子流－申报]{0}审批通过了关于<<{1}>>的{2}申报事件,备注内容：{3}", _user_name, productName, policyName, memo) ;
                                string title = string.Format("[政策电子流-申报{0}]", policyName);
                                sendRtxMessage(message, receiver, title, _user_name);
                            }
                        }
                    }
                    catch (Exception ex) { tran.Roback(); _msg = ex.Message; }

                }
            }
            return r;
        }
        public bool approve_reject(string memo)
        {
            bool r = false;
            _msg = "";
            if (_hasAuthority)
            {
                using (DBHelperTran tran = new DBHelperTran(DBHelper.mysqlConnection))
                {
                    string sql = "update declareorder set staute=1,next_oper=?next_oper,last_oper=?last_oper,last_oper_date=sysdate(),memo1=?last_memo where order_id=?order_id";
                    MySqlParameter[] paras = new MySqlParameter[4];
                    paras[0] = new MySqlParameter("next_oper", MySqlDbType.VarChar); paras[0].Value = "";
                    paras[1] = new MySqlParameter("last_oper", MySqlDbType.VarChar); paras[1].Value = _user_name;
                    paras[2] = new MySqlParameter("last_memo", MySqlDbType.VarChar); paras[2].Value = memo;
                    paras[3] = new MySqlParameter("order_id", MySqlDbType.VarChar); paras[3].Value = _orderId;
                    try
                    {
                        tran.ExcuteSQL(sql, paras);
                        string sn = BaseId.getDeclareFlowId();
                        sql = "insert into declareflow(sn,order_id,oper_type,oper_memo,oper_date,oper_user,memo) values(?sn,?order_id,?oper_type,?oper_memo,sysdate(),?oper_user,'0')";
                        paras = new MySqlParameter[5];
                        paras[0] = new MySqlParameter("sn", MySqlDbType.VarChar); paras[0].Value = sn;
                        paras[1] = new MySqlParameter("order_id", MySqlDbType.VarChar); paras[1].Value = _orderId;
                        paras[2] = new MySqlParameter("oper_type", OperList.Reject.Oper_Value);
                        paras[3] = new MySqlParameter("oper_memo", memo);
                        paras[4] = new MySqlParameter("oper_user", _user_name);
                        tran.ExcuteSQL(sql, paras);
                        tran.Commit();
                        r = true;
                        string policyName = policyDataContext.GetPolicyName(_dOrder.policyId);
                        string productName = bip_dataDataContext.GetProductName(_dOrder.product_Id);
                        string message = string.Format("[政策电子流－申报]{0}驳回了关于<<{1}>>的{2}申报事件,备注内容：{3}", _user_name, productName, policyName, memo);
                        string title = string.Format("[政策电子流-申报{0}]", policyName);
                        sendRtxMessage(message, _dOrder.oper_user, title, _user_name);

                    }
                    catch (Exception ex) { tran.Roback(); _msg = ex.Message; }

                }
            }
            return r;
        }
        public bool approve_close(string memo)
        {
            bool r = false;
            _msg = "";
            if (_hasAuthority)
            {
                using (DBHelperTran tran = new DBHelperTran(DBHelper.mysqlConnection))
                {
                    string sql = "update declareorder set staute=1,next_oper=?next_oper,last_oper=?last_oper,last_oper_date=sysdate(),memo1=?last_memo where order_id=?order_id";
                    MySqlParameter[] paras = new MySqlParameter[4];
                    paras[0] = new MySqlParameter("next_oper", MySqlDbType.VarChar); paras[0].Value = "";
                    paras[1] = new MySqlParameter("last_oper", MySqlDbType.VarChar); paras[1].Value = _user_name;
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
                        paras[4] = new MySqlParameter("oper_user", _user_name);
                        tran.ExcuteSQL(sql, paras);
                        tran.Commit();
                        r = true;
                        string policyName = policyDataContext.GetPolicyName(_dOrder.policyId);
                        string productName = bip_dataDataContext.GetProductName(_dOrder.product_Id);
                        string message = string.Format("[政策电子流－申报]关于您<<{0}>>的{1}申报事件已审批通过,备注内容：{2}",  productName, policyName, memo);
                        string title = string.Format("[政策电子流-申报{0}]", policyName);
                        sendRtxMessage(message, _dOrder.oper_user, title, _user_name);
                    }
                    catch (Exception ex) { tran.Roback(); _msg = ex.Message; }

                }
            }
            return r;
        }
        private void sendRtxMessage(string message, string receiver, string title, string sender)
        {
            string url ="http://"+ HttpContext.Current.Request.Url.Host+ URL.Content("~/MyGtasks/Index") + "?orderId=" + _dOrder.order_Id;
            RTXMessage rtxMessage = new RTXMessage()
            {
                MsgInfo = message + string.Format("点击处理链接:{0}", url),
                Receiver = receiver,
                Title = title,
                Priority = MessagePriority.Normal,
                Sender = sender
            };
            MessageHelper.SendRTXMessage(rtxMessage);
        }
        private void sendRtxMessage(string message,  string title)
        {
            RTXMessage rtxMessage = new RTXMessage()
            {
                MsgInfo = message + ";提示：可在我的已办中，查看明细。",
                Receiver = _dOrder.oper_user,
                Title = title,
                Priority = MessagePriority.Normal,
                Sender = _user_name
            };
            MessageHelper.SendRTXMessage(rtxMessage);
        }
    }
    public class ApproveComp
    {
        private string _user_name;
        private string _orderId;
        private bool _hasAuthority;
        public bool HasAuthority { get { return _hasAuthority; } }
        private string _staute;
        public string Staute { get { return _staute; } }
        private string _msg;
        public string Message { get { return _msg; } }
        private complainorder _cOrder;
        public complainorder ComplainOrder { get { return _cOrder; } }
        private List<Order_Flow_Detail> _detailData;
        public List<Order_Flow_Detail> DetailData { get { return _detailData; } }
        public UrlHelper URL { get; set; }
        public ApproveComp(string user_name, string orderId)
        {
            _msg = "";
            _user_name = user_name;
            _orderId = orderId;
            _hasAuthority = HasApprove();
            GetOrderStaute();
        }
        private bool HasApprove()
        {
            bool r = false;
            if (_user_name != "" && _orderId != "")
            {
                _cOrder = complianOrderDataContext.GetSingleComplainOrder(_orderId);
                if (_cOrder != null)
                {
                    string next_opr = _cOrder.next_oper;
                    if (next_opr != "")
                    {
                        next_opr = next_opr.Replace(';', ',');
                        string[] users = CommonUtility.SplitStringArray(next_opr, new char[] { ',' }, true);
                        if (users.Length > 0)
                        {
                            foreach (string u in users)
                            {
                                if (u == _user_name)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return r;
        }
        private void GetOrderStaute()
        {
            if (_hasAuthority && _cOrder != null)
            {
                string oper_user = _cOrder.oper_user;
                if (oper_user != _user_name)
                    _staute = "Right";
                else
                    _staute = "Close";

            }
        }
        public void makeData()
        {
            if (_hasAuthority && _cOrder != null)
            {

                _detailData = new List<Order_Flow_Detail>();
                var deList = complainFlowDataContext.GetComplainFlowByOrder(_orderId);
                if (deList != null)
                {
                    DateTime lastOperDate = DateTime.MinValue;
                    foreach (complainflow df in deList)
                    {
                        Order_Flow_Detail ofd = new Order_Flow_Detail();
                        ofd.OperDate = df.oper_date;
                        ofd.UserName = df.oper_user;
                        ofd.OperType = OperList.getOT_ByValue(df.oper_type);
                        ofd.OperMemo = df.oper_memo;
                        ofd.AttachUrl = df.attach_url;
                        if (lastOperDate == DateTime.MinValue)
                        {
                            ofd.Elapsed = 0f;
                        }
                        else
                        {
                            TimeSpan ts = new TimeSpan(df.oper_date.Ticks - lastOperDate.Ticks);
                            ofd.Elapsed = ts.TotalMinutes / 60f;
                        }
                        _detailData.Add(ofd);
                        lastOperDate = df.oper_date;
                    }
                }
            }
        }
        public bool approve_save(string memo, string pm, string attachUrl)
        {
            bool r = false;
            _msg = "";
            if (_hasAuthority)
            {
                using (DBHelperTran tran = new DBHelperTran(DBHelper.mysqlConnection))
                {
                    string sql = "update complainorder set next_oper=?next_oper,last_oper=?last_oper,last_oper_date=sysdate(),last_memo=?last_memo where order_id=?order_id";
                    MySqlParameter[] paras = new MySqlParameter[4];
                    paras[0] = new MySqlParameter("next_oper", MySqlDbType.VarChar); paras[0].Value = _cOrder.oper_user;
                    paras[1] = new MySqlParameter("last_oper", MySqlDbType.VarChar); paras[1].Value = _user_name;
                    paras[2] = new MySqlParameter("last_memo", MySqlDbType.VarChar); paras[2].Value = memo;
                    paras[3] = new MySqlParameter("order_id", MySqlDbType.VarChar); paras[3].Value = _orderId;
                    try
                    {
                        tran.ExcuteSQL(sql, paras);
                        string sn = BaseId.getCompFlowId();
                        sql = "insert into complainflow(sn,orderId,complainId,pm,attach_url,oper_type,oper_memo,oper_date,oper_user,memo) values(?sn,?order_id,?complainId,?pm,?attach_url,?oper_type,?oper_memo,sysdate(),?oper_user,'0')";
                        paras = new MySqlParameter[8];
                        paras[0] = new MySqlParameter("sn", MySqlDbType.VarChar); paras[0].Value = sn;
                        paras[1] = new MySqlParameter("order_id", MySqlDbType.VarChar); paras[1].Value = _orderId;
                        paras[2] = new MySqlParameter("complainId", _cOrder.complainId);
                        paras[3] = new MySqlParameter("pm", pm);
                        paras[4] = new MySqlParameter("attach_url", attachUrl);
                        paras[5] = new MySqlParameter("oper_type", OperList.Handle.Oper_Value);
                        paras[6] = new MySqlParameter("oper_memo", memo);
                        paras[7] = new MySqlParameter("oper_user", _user_name);
                        tran.ExcuteSQL(sql, paras);
                        tran.Commit();
                        if (!tran.HasErr)
                        {
                            r = true;
                            string message = string.Format("[政策电子流－投诉]{0}回复了您发起了关于<<{1}>>的{2}投诉事件,该回复内容：{3}", _user_name, _cOrder.productName, _cOrder.complainName, memo);
                            string title = string.Format("[政策电子流-投诉{0}]", _cOrder.complainName);
                            sendRtxMessage(message, pm, title, _user_name);
                        }
                    }
                    catch (Exception ex) { tran.Roback(); _msg = ex.Message; }

                }
            }
            return r;
        }
        public bool approve_to(string memo, string pm, string attachUrl)
        {
            bool r = false;
            _msg = "";
            if (_hasAuthority)
            {
                using (DBHelperTran tran = new DBHelperTran(DBHelper.mysqlConnection))
                {
                    string sql = "update complainorder set next_oper=?next_oper,last_oper=?last_oper,last_oper_date=sysdate(),last_memo=?last_memo where order_id=?order_id";
                    MySqlParameter[] paras = new MySqlParameter[4];
                    paras[0] = new MySqlParameter("next_oper", MySqlDbType.VarChar); paras[0].Value = pm;
                    paras[1] = new MySqlParameter("last_oper", MySqlDbType.VarChar); paras[1].Value = _user_name;
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
                        paras[2] = new MySqlParameter("complainId", _cOrder.complainId);
                        paras[3] = new MySqlParameter("pm", pm);
                        paras[4] = new MySqlParameter("attach_url", attachUrl);
                        paras[5] = new MySqlParameter("oper_type", OperList.To.Oper_Value);
                        paras[6] = new MySqlParameter("oper_memo", memo);
                        paras[7] = new MySqlParameter("oper_user", _user_name);
                        tran.ExcuteSQL(sql, paras);
                        tran.Commit();
                        if (!tran.HasErr)
                        {
                            r = true;
                            string message = string.Format("[政策电子流－投诉]{0}向您转交了关于<<{1}>>的{2}投诉事件,该转交留言：{3}", _user_name, _cOrder.productName, _cOrder.complainName, memo);
                            string title = string.Format("[政策电子流-投诉{0}]", _cOrder.complainName);
                            sendRtxMessage(message, pm, title, _user_name);
                        }
                    }
                    catch (Exception ex) { tran.Roback(); _msg = ex.Message; }

                }
            }
            return r;
        }
        public bool approve_close(string memo, string pm, string attachUrl)
        {
            bool r = false;
            _msg = "";
            if (_hasAuthority)
            {
                using (DBHelperTran tran = new DBHelperTran(DBHelper.mysqlConnection))
                {
                    string sql = "update complainorder set staute=1,next_oper=?next_oper,last_oper=?last_oper,last_oper_date=sysdate(),last_memo=?last_memo where order_id=?order_id";
                    MySqlParameter[] paras = new MySqlParameter[4];
                    paras[0] = new MySqlParameter("next_oper", MySqlDbType.VarChar); paras[0].Value = "";
                    paras[1] = new MySqlParameter("last_oper", MySqlDbType.VarChar); paras[1].Value = _user_name;
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
                        paras[2] = new MySqlParameter("complainId", _cOrder.complainId);
                        paras[3] = new MySqlParameter("pm", pm);
                        paras[4] = new MySqlParameter("attach_url", attachUrl);
                        paras[5] = new MySqlParameter("oper_type", OperList.Close.Oper_Value);
                        paras[6] = new MySqlParameter("oper_memo", memo);
                        paras[7] = new MySqlParameter("oper_user", _user_name);
                        tran.ExcuteSQL(sql, paras);
                        tran.Commit();
                        if (!tran.HasErr)
                            r = true;
                    }
                    catch (Exception ex) { tran.Roback(); _msg = ex.Message; }

                }
            }
            return r;
        }
        public bool approve_reject(string memo, string pm, string attachUrl)
        {
            bool r = false;
            _msg = "";
            if (_hasAuthority)
            {
                using (DBHelperTran tran = new DBHelperTran(DBHelper.mysqlConnection))
                {
                    string sql = "update complainorder set next_oper=?next_oper,last_oper=?last_oper,last_oper_date=sysdate(),last_memo=?last_memo where order_id=?order_id";
                    MySqlParameter[] paras = new MySqlParameter[4];
                    paras[0] = new MySqlParameter("next_oper", MySqlDbType.VarChar); paras[0].Value = pm;
                    paras[1] = new MySqlParameter("last_oper", MySqlDbType.VarChar); paras[1].Value = _user_name;
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
                        paras[2] = new MySqlParameter("complainId", _cOrder.complainId);
                        paras[3] = new MySqlParameter("pm", pm);
                        paras[4] = new MySqlParameter("attach_url", attachUrl);
                        paras[5] = new MySqlParameter("oper_type", OperList.Reject.Oper_Value);
                        paras[6] = new MySqlParameter("oper_memo", memo);
                        paras[7] = new MySqlParameter("oper_user", _user_name);
                        tran.ExcuteSQL(sql, paras);
                        tran.Commit();
                        if (!tran.HasErr)
                        {
                            r = true;
                            string message = string.Format("[政策电子流－投诉]{0}驳回了您关于<<{1}>>的{2}投诉事件,该驳回意见：{3}", _user_name, _cOrder.productName, _cOrder.complainName, memo);
                            string title = string.Format("[政策电子流-投诉{0}]", _cOrder.complainName);
                            sendRtxMessage(message, pm, title, _user_name);
                        }
                    }
                    catch (Exception ex) { tran.Roback(); _msg = ex.Message; }

                }
            }
            return r;
        }
        private void sendRtxMessage(string message, string receiver, string title, string sender)
        {
            string url = "http://" + HttpContext.Current.Request.Url.Host + URL.Content("~/MyCtasks/Index") + "?orderId=" + _cOrder.order_Id;
            RTXMessage rtxMessage = new RTXMessage()
            {
                MsgInfo = message + string.Format("点击处理链接:{0}", url),
                Receiver = receiver,
                Title = title,
                Priority = MessagePriority.Normal,
                Sender = sender
            };
            MessageHelper.SendRTXMessage(rtxMessage);
        }
    }
}