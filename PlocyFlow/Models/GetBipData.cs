using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Forms;
using PlocyFlow.DAL.Context;
using PlocyFlow.DAL.Entity;
using PlocyFlow.DAL.CommonUtility;
namespace PlocyFlow.Models
{
    public class GetBipData
    {
        private XDocument doc;
        private bool _err;
        private string _msg;
        public bool Error { get { return _err; } }
        public string Message { get { return _msg; } }
        public bool getData()
        {
            bool r = false;
            _err = false;
            _msg = "";
            try
            {
                net net = new net();
                string xml = net.getHTML(ConfigUtility.BipUrl);
                doc = XDocument.Parse(@xml);
                upDept();
                if (!_err)
                    upProductType();
                if (!_err)
                    upGameType();
                if (!_err)
                    upStatus();
                if (!_err)
                    upPolicy();
                if (!_err)
                    upBipData();
                if (!_err)
                    r = true;
            }
            catch(Exception ex) {
                _err = true;
                _msg = ex.Message;
            }
            finally
            {
                if (doc != null)
                    doc = null;
            }
            return r;
        }
        public bool getData(string xml)
        {
            bool r = false;
            _err = false;
            _msg = "";
            try
            {
                doc = XDocument.Parse(@xml);
                upDept();
                if (!_err)
                    upProductType();
                if (!_err)
                    upGameType();
                if (!_err)
                    upStatus();
                if (!_err)
                    upPolicy();
                if (!_err)
                    upBipData();
                if (!_err)
                    r = true;
            }
            catch { }
            finally
            {
                if (doc != null)
                    doc = null;
            }
            return r;
        }
        private void upDept()
        {
            if (doc != null)
            {
                try
                {
                    var dept = from d in doc.Descendants("Product")
                               select new { did = d.Element("DepartmentId").Value, dname = d.Element("DepartmentName").Value };
                    if (dept != null)
                    {
                        List<string> dlist = new List<string>();
                        foreach (var d in dept)
                        {
                            if (!dlist.Contains(d.did))
                            {
                                dlist.Add(d.did);
                                int did = -1;
                                if (int.TryParse(d.did, out did))
                                    deptDataContext.updateDept(did, d.dname.Trim());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _err = true;
                    _msg = ex.Message;
                }
            }
        }
        private void upProductType()
        {
            if (doc != null)
            {
                try
                {
                    var temp = from d in doc.Descendants("Product")
                               select new { id = d.Element("ProductTypeId").Value, name = d.Element("ProductTypeName").Value };
                    if (temp != null)
                    {
                        List<string> dlist = new List<string>();
                        foreach (var d in temp)
                        {
                            string id = d.id.Trim();
                            if (!dlist.Contains(id))
                            {
                                dlist.Add(id);
                                product_typeDataContext.updateProduct_type(id, d.name.Trim());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _err = true;
                    _msg = ex.Message;
                }
            }
        }
        private void upGameType()
        {
            if (doc != null)
            {
                try
                {
                    var temp = from d in doc.Descendants("Product")
                               select new { id = d.Element("GameTypeId").Value, name = d.Element("GameTypeName").Value };
                    if (temp != null)
                    {
                        List<string> dlist = new List<string>();
                        foreach (var d in temp)
                        {
                            string id = d.id.Trim();
                            if (!dlist.Contains(id))
                            {
                                dlist.Add(id);
                                game_typeDataContext.updateGame_type(id, d.name.Trim());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _err = true;
                    _msg = ex.Message;
                }
            }
        }
        private void upStatus()
        {
            if (doc != null)
            {
                try
                {
                    var temp = from d in doc.Descendants("Product")
                               select new { id = d.Element("StatusId").Value, name = d.Element("StatusName").Value };
                    if (temp != null)
                    {
                        List<string> dlist = new List<string>();
                        foreach (var d in temp)
                        {
                            int id = -1;
                            if (!dlist.Contains(d.id.Trim()))
                            {
                                dlist.Add(d.id.Trim());
                                if (int.TryParse(d.id, out id))
                                    game_statusDataContext.updateGame_status(id, d.name.Trim());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _err = true;
                    _msg = ex.Message;
                }
            }
        }
        private void upPolicy()
        {
            if (doc != null)
            {
                try
                {
                    var temp = from d in doc.Descendants("PolicyInfo")
                               select new { id = d.Element("PolicyId").Value, name = d.Element("PolicyName").Value };
                    if (temp != null)
                    {
                        List<string> dlist = new List<string>();
                        foreach (var d in temp)
                        {
                            string id = d.id.Trim();
                            if (!dlist.Contains(id))
                            {
                                dlist.Add(id);
                                policyDataContext.updatePolicy(id, d.name);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _err = true;
                    _msg = ex.Message;
                }
            }
        }
        private void upBipData()
        {
            if (doc != null)
            {
                try
                {
                    var temp = from d in doc.Descendants("Product")
                               select d;
                    if (temp != null)
                    {
                        List<bip_data> bipData = new List<bip_data>();
                        foreach (var p in temp)
                        {
                            var policy = from pi in p.Descendants("PolicyInfo") select pi;
                            foreach (var pi in policy)
                            {
                                bip_data b = new bip_data();
                                b.Productid = p.Element("ProductId").Value;
                                b.ProductName = p.Element("ProductName").Value;
                                b.Departmentid = CommonUtility.TryParseStringToInt32(p.Element("DepartmentId").Value);
                                b.ProductTypeid = p.Element("ProductTypeId").Value;
                                b.OBDate = CommonUtility.TryParseStringToDateTime(p.Element("OBDate").Value);
                                b.GameTypeid = p.Element("GameTypeId").Value;
                                b.Statusid = CommonUtility.TryParseStringToInt32(p.Element("StatusId").Value);
                                b.Manager = p.Element("Manager").Value;
                                b.Policyid = pi.Element("PolicyId").Value;
                                b.IsDone = bool.Parse(pi.Element("IsDone").Value);
                                b.Remark = pi.Element("Reamrk").Value;
                                bipData.Add(b);
                            }
                        }
                        bip_dataDataContext.updateBip_data(bipData);
                    }
                }
                catch (Exception ex)
                {
                    _err = true;
                    _msg = ex.Message;
                }
            }
        }
    }
    public class net
    {
        // Fields
        public string ContentType = "text/xml";
        public Encoding cs = Encoding.UTF8;
        private string html;
        private bool isOk = false;
        public string post;
        public string url;

        // Methods
        public string getHTML(string url)
        {
            this.isOk = false;
            this.url = url;
            new Thread(new ThreadStart(this.start)).Start();
            while (true)
            {
                if (this.isOk)
                {
                    return this.html;
                }
                Application.DoEvents();
                Thread.Sleep(100);
            }
        }
        public void start()
        {
            try
            {
                WebRequest request = WebRequest.Create(this.url);
                if (this.post != null)
                {
                    request.Method = "POST";
                    request.ContentType = this.ContentType;
                    Stream requestStream = request.GetRequestStream();
                    requestStream.Write(Encoding.Default.GetBytes(this.post), 0, this.post.Length);
                    requestStream.Close();
                }
                request.Credentials = CredentialCache.DefaultCredentials;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream, this.cs);
                this.html = reader.ReadToEnd();
                reader.Close();
                responseStream.Close();
                response.Close();
            }
            catch (Exception)
            {
                this.html = "";
            }
            this.isOk = true;
        }
    }
}