using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Web;
using System.Web.Mvc;
using PlocyFlow.DAL.Entity;
using PlocyFlow.DAL.CommonUtility;
using PlocyFlow.DAL.Context;
namespace PlocyFlow.Models
{
    /// <summary>
    /// 权限验证,根据用户ID，页面ID验证是否可访问当前页面
    /// </summary>

    public static class PagePurview
    {
        public static bool verifyVisble(string uid, string pagename)
        {
            bool result = false;
            T_Page p = new T_Page(pagename);
            if (p.HasPage)
                result = p.Current_User_Visible(uid);
            return result;
        }
    }
    /// <summary>
    /// 业务页面对象类，包含了数据库中页面表的常用值，及逻辑方法
    /// </summary>
    public class T_Page
    {
        private string _id;
        public string ID { get { return _id; } }
        private string _page_name;
        public string PageName { get { return _page_name; } }
        private string _page_url;
        public string PageUrl { get { return _page_url; } }
        private bool _isleaf;
        public bool IsLeaf { get { return _isleaf; } }
        private string _parentid;
        public string ParentID { get { return _parentid; } }
        private string _vcode;
        private string _memo;
        private bool _haspage;
        public bool HasPage { get { return _haspage; } }
        private string _isshortcut;
        public bool IsShortCut { get { return _isshortcut == "1" ? true : false; } }
        public T_Page(int pageid)
        {
            _haspage = false;
            if (pageid.ToString() != "")
            {
                t_page p = t_pageDataContext.GetT_page(pageid);
                if (p != null)
                {
                    _id = CommonUtility.ReplaceNullString(p.id);
                    _page_name = CommonUtility.ReplaceNullString(p.page_name);
                    _page_url = CommonUtility.ReplaceNullString(p.page_url);
                    _parentid = CommonUtility.ReplaceNullString(p.parentid);
                    _vcode = CommonUtility.ReplaceNullString(p.vcode);
                    _memo = CommonUtility.ReplaceNullString(p.memo);
                    _isshortcut = CommonUtility.ReplaceNullString(p.is_shortcut);
                    string temp = CommonUtility.ReplaceNullString(p.isleaf);
                    _isleaf = false;
                    if (temp.Trim() != "")
                    {
                        if (Convert.ToInt32(temp) == 1)
                            _isleaf = true;
                    }
                    if (_id != "")
                        _haspage = true;
                }
            }
        }
        public T_Page(string pageUrl)
        {
            _haspage = false;
            if (pageUrl.ToString() != "")
            {
                t_page p = t_pageDataContext.GetT_page(pageUrl);
                if (p != null)
                {
                    _id = CommonUtility.ReplaceNullString(p.id);
                    _page_name = CommonUtility.ReplaceNullString(p.page_name);
                    _page_url = CommonUtility.ReplaceNullString(p.page_url);
                    _parentid = CommonUtility.ReplaceNullString(p.parentid);
                    _vcode = CommonUtility.ReplaceNullString(p.vcode);
                    _memo = CommonUtility.ReplaceNullString(p.memo);
                    string temp = CommonUtility.ReplaceNullString(p.isleaf);
                    _isleaf = false;
                    if (temp.Trim() != "")
                    {
                        if (Convert.ToInt32(temp) == 1)
                            _isleaf = true;
                    }
                    if (_id != "")
                        _haspage = true;
                }
            }
        }
        /// <summary>
        /// 当前用户是否可以使用该页面
        /// </summary>
        /// <param name="uid">登录用户ID</param>
        /// <returns></returns>
        public bool Current_User_Visible(string user_name)
        {
            bool result = false;
            if (_haspage && _vcode != "")
            {
                t_user u = t_userDataContext.Get_User(user_name);
                if (u != null)
                {
                    string uvcode = CommonUtility.ReplaceNullString(u.vcode);
                    if (uvcode != "")
                    {
                        int cmp = CommonUtility.Comp(Convert.ToUInt64(uvcode, 16), Convert.ToUInt64(_vcode, 16));
                        if (cmp > 0)
                            result = true;
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// 取得上一级页面对象
        /// </summary>
        /// <returns></returns>
        public T_Page GetParentPage()
        {
            return new T_Page(_parentid);
        }
    }
    /// <summary>
    /// 根据当前用户的权限生成该用户的逻辑菜单树
    /// </summary>
    public class T_Menu
    {
        private Logical_Menu_Tree _menutree;
        private string _uid;
        private bool _hasmenu;
        private string _vcode;
        private Logical_Menu_Tree _currentNode;
        public bool HasMenu { get { return _hasmenu; } }
        public Logical_Menu_Tree MenuTree { get { return _menutree; } }
        public T_Menu(string uid)
        {
            _uid = uid;
            _vcode = GeneralFun.GetVCode(_uid);
            _hasmenu = false;
            _currentNode = null;
            MakeTree();
        }
        public string GetJsonTree()
        {
            string result = "";


            return result;
        }
        private void MakeTree()
        {
            var menuData = getMenuData();
            _menutree = null;
            if (menuData != null && menuData.Count > 0)
            {
                _menutree = new Logical_Menu_Tree();
                _menutree.IsRoot = true;
                _hasmenu = true;
                foreach (var m in menuData)
                {
                    string id = CommonUtility.ReplaceNullString(m.id);
                    string vc = CommonUtility.ReplaceNullString(m.vcode);
                    string parentid = CommonUtility.ReplaceNullString(m.parentid);
                    Logical_Menu_Tree newNode = new Logical_Menu_Tree();
                    newNode.InitialNode(id);
                    if (newNode.NodeName != "")
                    {
                        if (parentid == "0")
                        {
                            _menutree.AddChildNode(newNode);
                        }
                        else
                        {
                            _currentNode = null;
                            Traversal(parentid, _menutree);
                            if (_currentNode != null)
                            {
                                _currentNode.AddChildNode(newNode);
                            }
                        }
                    }
                }
            }
        }
        private List<t_page> getMenuData()
        {
            var plist = t_pageDataContext.Get_ShowPage();
            if (plist != null && plist.Count > 0)
            {
                int idx = 0;
                while (idx < plist.Count)
                {
                    t_page t = plist[idx];
                    string current_code = CommonUtility.ReplaceNullString(t.vcode);
                    if (!isvisible(current_code))
                    {
                        plist.Remove(t);
                    }
                    else
                        idx++;
                }
            }
            return plist;
        }
        private bool isvisible(string current_vcode)
        {
            bool result = false;
            int cmp = CommonUtility.Comp(Convert.ToUInt64(_vcode, 16), Convert.ToUInt64(current_vcode, 16));
            if (cmp > 0)
                result = true;
            return result;
        }
        private void Traversal(string tree_id, Logical_Menu_Tree curNode)
        {
            if (curNode.ID == tree_id)
                _currentNode = curNode;
            else
            {
                if (curNode.HasChild)
                {
                    foreach (Logical_Menu_Tree lmt in curNode.Childs)
                    {
                        Traversal(tree_id, lmt);
                    }
                }
                else
                    return;
            }

        }
    }
    /// <summary>
    /// 逻辑菜单树结点
    /// </summary>
    public class Logical_Menu_Tree : IDisposable
    {
        private string _id;
        private string _nodename;
        private bool _isleaf;
        private string _pageurl;
        private int _childcount;
        private List<Logical_Menu_Tree> _children;
        private bool _isroot;
        private T_Page _curpage;
        public bool HasChild { get { return _childcount > 0 ? true : false; } }
        public bool IsLeaf { get { return _isleaf; } }
        public bool IsRoot { get { return _isroot; } set { _isroot = value; } }
        public List<Logical_Menu_Tree> Childs { get { return _children; } }
        public T_Page CurrentPage { get { return _curpage; } }
        public string PageUrl { get { return _pageurl; } }
        public string ID { get { return _id; } }
        public string NodeName { get { return _nodename; } }
        public Logical_Menu_Tree()
        {
            _nodename = "";
            _isleaf = false;
            _childcount = 0;
            _children = new List<Logical_Menu_Tree>();
            _isroot = false;
            _curpage = null;
        }
        public void InitialNode(string pageid)
        {
            _curpage = new T_Page(Convert.ToInt32(pageid));
            if (_curpage.HasPage)
            {
                _id = _curpage.ID;
                _nodename = _curpage.PageName;
                _isleaf = _curpage.IsLeaf;
                _pageurl = _curpage.PageUrl;
            }
        }
        public void AddChildNode(Logical_Menu_Tree child)
        {
            if (!_children.Contains(child))
            {
                _children.Add(child);
                _childcount = _children.Count;
            }
        }
        public void RemoveChildNode(Logical_Menu_Tree child)
        {
            if (_children.Contains(child))
            {
                _children.Remove(child);
                _childcount = _children.Count;
            }
        }
        public void Dispose()
        {
            _childcount = 0;
            _children.Clear();
            _children = null;
        }
    }
    public static class GeneralFun
    {
        
        public static string GetVCode(string user_name)
        {
            string vcode = CommonUtility.defaultCode;
            if (user_name != null && user_name != "")
            {
                var user = t_userDataContext.Get_User(user_name);
                if (user != null)
                {
                    string temp = CommonUtility.ReplaceNullString(user.vcode);
                    if (temp != "")
                        vcode = temp;
                }
            }
            return vcode;
        }
    }
}
