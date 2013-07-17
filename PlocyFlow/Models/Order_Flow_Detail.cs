using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PlocyFlow.DAL.CommonUtility;
namespace PlocyFlow.Models
{
    public class Order_Flow_Detail
    {
        public DateTime OperDate { get; set; }
        public string UserName { get; set; }
        public Oper_Type OperType { get; set; }
        public string OperMemo { get; set; }
        public string AttachUrl { get; set; }
        public double Elapsed { get; set; }
    }
}