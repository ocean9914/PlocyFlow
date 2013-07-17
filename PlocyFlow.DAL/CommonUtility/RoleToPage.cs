using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlocyFlow.DAL.CommonUtility
{
    public class RoleToPage
    {
        public string pageId { get; set; }
        public string pageName { get; set; }
        public string roleList { get; set; }
    }
    public class Product
    {
        public string Productid { get; set; }
        public string ProductName { get; set; }
        public string DepartId { get; set; }
        public string DepartName { get; set; }
        public string ProductTypeId { get; set; }
        public string ProductTypeName { get; set; }
        public string OBDate { get; set; }
        public string Game_TypeId { get; set; }
        public string Game_TypeName { get; set; }
        public string Statusid { get; set; }
        public string StatusName { get; set; }
        public string Manager { get; set; }
        public string PolicyId { get; set; }
        public string PolicyName { get; set; }
        public string IsDone { get; set; }
        public string Remark { get; set; }
    }
    public class Worked
    {
        public string OrderId { get; set; }
        public string WorkedType { get; set; }
        public string Staute { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string DeptId { get; set; }
        public string DeptName { get; set; }
        public string OperDate { get; set; }
        public string NextOper { get; set; }
        public string OperUser { get; set; }
    }
    
}
