using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Web;
namespace PlocyFlow.DAL.CommonUtility
{
   public static class ExcelHelper
{
    // Fields
    public static int m_SheetMaxLen = 0xffff;

    // Methods
    public static string CreateExcel(this DataTable pDt, string pFileName, string pSheet, int pSheetIdx, int pStart)
    {
        int num;
        string str4;
        if (!Directory.Exists(Path.GetDirectoryName(pFileName)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(pFileName));
        }
        if (File.Exists(pFileName))
        {
            File.Delete(pFileName);
        }
        string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pFileName + ";Extended Properties='Excel 8.0;HDR=YES'";
        string str2 = "";
        OleDbConnection connection = new OleDbConnection(connectionString);
        connection.Open();
        OleDbTransaction transaction = connection.BeginTransaction();
        OleDbCommand command = new OleDbCommand("", connection);
        command.Transaction = transaction;
        pSheet = pSheet + ((pSheetIdx == 0) ? "" : ("(" + pSheetIdx.ToString() + ")"));
        str2 = " Create table " + pSheet + " (";
        for (num = 0; num < pDt.Columns.Count; num++)
        {
            pDt.Columns[num].ColumnName = pDt.Columns[num].Caption;
        }
        num = 0;
        while (num < pDt.Columns.Count)
        {
            str2 = str2 + "[" + pDt.Columns[num].Caption + "] NTEXT,";
            num++;
        }
        str2 = str2.Substring(0, str2.Length - 1) + ")";
        try
        {
            command.CommandText = str2;
            command.ExecuteNonQuery();
            int num2 = (pDt.Rows.Count > (pStart + m_SheetMaxLen)) ? (pStart + m_SheetMaxLen) : pDt.Rows.Count;
            for (int i = pStart; i < num2; i++)
            {
                str2 = "insert into [" + pSheet + "] values(";
                for (num = 0; num < pDt.Columns.Count; num++)
                {
                    string str3 = "";
                    if (pDt.Rows[i][num] != DBNull.Value)
                    {
                        if (pDt.Columns[num].DataType == Type.GetType("System.DateTime"))
                        {
                            str3 = DateTime.Parse(pDt.Rows[i][num].ToString()).ToShortDateString();
                        }
                        else if ((pDt.Columns[num].DataType == Type.GetType("System.Decimal")) || (pDt.Columns[num].DataType == Type.GetType("System.Double")))
                        {
                            str3 = decimal.Parse(pDt.Rows[i][num].ToString()).ToString("N3");
                        }
                        else
                        {
                            str3 = Convert.ToString(pDt.Rows[i][num]);
                        }
                    }
                    str2 = str2 + "'" + str3.ToString().Replace("'", "''") + "',";
                }
                str2 = str2.Substring(0, str2.Length - 1) + ")";
                command.CommandText = str2;
                command.ExecuteNonQuery();
            }
            transaction.Commit();
            connection.Close();
            if (pDt.Rows.Count > (pStart + m_SheetMaxLen))
            {
                pDt.CreateExcel(pFileName, pSheet, pSheetIdx + 1, num2);
            }
            str4 = pFileName;
        }
        catch (Exception exception)
        {
            transaction.Rollback();
            throw new Exception(exception.Message);
        }
        finally
        {
            if (connection.State != ConnectionState.Closed)
            {
                connection.Close();
            }
        }
        return str4;
    }

    public static void DownExcel( string serverfilpath, string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentNullException("fileName");
        }
        if (File.Exists(serverfilpath))
        {
            FileInfo info = new FileInfo(serverfilpath);
            HttpContext current = HttpContext.Current;
            try
            {
                current.Response.Clear();
                current.Response.ClearHeaders();
                current.Response.ClearContent();
                current.Response.Buffer = false;
                current.Response.Charset = "utf-8";
                current.Response.ContentEncoding = Encoding.UTF8;
                current.Response.ContentType = "application/vnd.ms-excel";
                if (current.Request.Browser.Browser == "IE")
                {
                    current.Response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, Encoding.UTF8));
                }
                else
                {
                    current.Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName);
                }
                current.Response.AddHeader("Content-Length", info.Length.ToString());
                current.Response.WriteFile(info.FullName, 0L, info.Length);
                current.Response.Flush();
                current.Response.End();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                File.Delete(serverfilpath);
            }
        }
    }
}

}
