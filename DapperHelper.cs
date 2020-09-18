using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;

namespace BestWoDP
{
    public class DapperHelper
    {


        /// <summary>
        /// 执行存储过程返回DataSet
        /// </summary>
        /// <param name="SPName"></param>
        /// <param name="parameters"></param>
        /// <param name="DBSetName"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public static DataSet RunProcedure_DataSet(string SPName, IDataParameter[] parameters, string DBSetName, DBConnection DB)
        {
            return RunProcedure_DataSet(SPName, parameters, DB);
        }

        /// <summary>
        /// 执行存储过程返回DataSet
        /// </summary>
        /// <param name="SPName"></param>
        /// <param name="parameters"></param>
        /// <param name="sqlconnection"></param>
        /// <returns></returns>
        public static DataSet RunProcedure_DataSet(string SPName, IDataParameter[] parameters, DBConnection DB)
        {
            DynamicParameters pars = new DynamicParameters();
            foreach (SqlParameter item in parameters)
            {
                pars.Add(item.ParameterName, item.Value, item.DbType, item.Direction, item.Size);
            }
            string StrConnection = GetConnectionStr(DB);
            using (IDbConnection DBcon = new SqlConnection(StrConnection))
            {
                DataSet ds = new XDataSet();
                var result = DBcon.ExecuteReader(SPName, pars, commandTimeout: 600, commandType: CommandType.StoredProcedure);
                try
                {
                    ds.Load(result, LoadOption.OverwriteChanges, null, new DataTable[] { });
                }
                finally { result.Close(); }

                return InitDS(ds);

            }
        }



        /// <summary>
        /// 执行存储过程返回DataTable
        /// </summary>
        /// <param name="SPName"></param>
        /// <param name="parameters"></param>
        /// <param name="sqlconnection"></param>
        /// <returns></returns>
        public static DataTable RunProcedure_DataTable(string SPName, IDataParameter[] parameters, DBConnection DB)
        {
            DynamicParameters pars = new DynamicParameters();
            foreach (SqlParameter item in parameters)
            {
                pars.Add(item.ParameterName, item.Value, item.DbType, item.Direction, item.Size);
            }
            string StrConnection = GetConnectionStr(DB);
            using (IDbConnection DBcon = new SqlConnection(StrConnection))
            {
                DataTable dt = new DataTable();
                var result = DBcon.ExecuteReader(SPName, pars, commandTimeout: 600, commandType: CommandType.StoredProcedure);
                try
                {
                    dt.Load(result);
                }
                finally { result.Close(); }

                foreach (SqlParameter item in parameters)
                {
                    if (item.Direction == ParameterDirection.Output)
                    {
                        item.Value = pars.Get<object>(item.ParameterName);
                    }
                }
                return InitDT(dt);
            }
        }

        /// <summary>
        /// 执行SP 返回SP返回值
        /// </summary>
        /// <param name="SPName"></param>
        /// <param name="parameters"></param>
        /// <param name="sqlconnection"></param>
        /// <returns></returns>
        public static int RunProcedure_Return(string SPName, IDataParameter[] parameters, DBConnection DB)
        {
            int AffectedCount = 0;
            int intResult = RunProcedure_Return(SPName, parameters, DB, out AffectedCount);
            return intResult;
        }

        /// <summary>
        /// 执行SP 返回SP返回值及受影响的行数
        /// </summary>
        /// <param name="SPName"></param>
        /// <param name="parameters"></param>
        /// <param name="sqlconnection"></param>
        /// <param name="AffectedCount"></param>
        /// <returns></returns>
        public static int RunProcedure_Return(string SPName, IDataParameter[] parameters, DBConnection DB, out int AffectedCount)
        {
            DynamicParameters pars = new DynamicParameters();
            foreach (SqlParameter item in parameters)
            {
                pars.Add(item.ParameterName, item.Value, item.DbType, item.Direction, item.Size);
            }

            pars.Add("ReturnValue", null, DbType.Int32, ParameterDirection.ReturnValue, null);
            string StrConnection = GetConnectionStr(DB);
            using (IDbConnection DBcon = new SqlConnection(StrConnection))
            {
                int result = DBcon.Execute(SPName, pars, commandTimeout: 600, commandType: CommandType.StoredProcedure);
                foreach (SqlParameter item in parameters)
                {
                    if (item.Direction == ParameterDirection.Output)
                    {
                        item.Value = pars.Get<object>(item.ParameterName) == null ? "" : pars.Get<object>(item.ParameterName);
                    }
                }

                AffectedCount = result;

                return pars.Get<int>("ReturnValue");
            }
        }


        /// <summary>
        /// 查询返回DataTable
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="FieldString"></param>
        /// <param name="WhereString"></param>
        /// <returns></returns>
        public static DataTable QueryGetDT(string TableName, string FieldString, string WhereString, DBConnection DB)
        {
            DataTable dt = new DataTable();
            string StrConnection = GetConnectionStr(DB);
            if (!WhereString.ToLower().Trim().StartsWith("and"))
            {
                WhereString = " and " + WhereString;
            }
            using (IDbConnection DBcon = new SqlConnection(StrConnection))
            {
                string sql = "select " + FieldString + " from " + TableName + " where 1=1 " + WhereString;
                var result = DBcon.ExecuteReader(sql, null, commandTimeout: 600);
                try
                {
                    dt.Load(result);
                }
                finally { result.Close(); }
                return InitDT(dt);
            }
        }


        /// <summary>
        /// 更新记录
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <param name="UpdateString">更新的列和值</param>
        /// <param name="WhereString">条件</param>
        /// <param name="Parameters">参数对象</param>
        /// <returns></returns>
        public static int Update(string TableName, string UpdateString, string WhereString, DBConnection DB)
        {
            string StrConnection = GetConnectionStr(DB);
            if (!WhereString.ToLower().Trim().StartsWith("and"))
            {
                WhereString = " and " + WhereString;
            }
            if (WhereString.Trim() == "")
            {
                return 0;
            }
            else
            {
                using (IDbConnection DBcon = new SqlConnection(StrConnection))
                {
                    string sql = "update " + TableName + " set " + UpdateString + " where 1=1 " + WhereString;
                    var result = DBcon.Execute(sql, null, commandTimeout: 600);
                    return result;
                }
            }
        }
        /// <summary>
        /// 执行语句
        /// </summary>
        /// <param name="strSQL"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public static int ExceSQL(string strSQL, DBConnection DB)
        {
            string StrConnection = GetConnectionStr(DB);
            using (IDbConnection DBcon = new SqlConnection(StrConnection))
            {
                var result = DBcon.Execute(strSQL, null, commandTimeout: 600);
                return result;
            }
        }

        /// <summary>
        /// 执行新增语句返回ID
        /// </summary>
        /// <param name="strSQL"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public static int ExceSQLReturnID(string strSQL, DBConnection DB)
        {
            strSQL += ";SELECT @id=SCOPE_IDENTITY()";
            string StrConnection = GetConnectionStr(DB);
            using (IDbConnection DBcon = new SqlConnection(StrConnection))
            {
                var p = new DynamicParameters();
                p.Add("@ID", dbType: DbType.Int32, direction: ParameterDirection.Output);
                var affectCount = DBcon.Execute(strSQL, p);
                return p.Get<int>("@ID");
            }
        }

        public static T GetModel<T>(string TableName, string FieldString, string WhereString, DBConnection DB)
        {
            string StrConnection = GetConnectionStr(DB);
            using (IDbConnection DBcon = new SqlConnection(StrConnection))
            {
                string sql = "select top 1 " + FieldString + " from " + TableName + " where 1=1 " + WhereString;
                var result = DBcon.Query<T>(sql, null).FirstOrDefault();
                return result;
            }
        }



        public class XDataSet : DataSet
        {
            public override void Load(IDataReader reader, LoadOption loadOption, FillErrorEventHandler handler, params DataTable[] tables)
            {
                XLoadAdapter adapter = new XLoadAdapter
                {
                    FillLoadOption = loadOption,
                    MissingSchemaAction = MissingSchemaAction.AddWithKey
                };
                if (handler != null)
                {
                    adapter.FillError += handler;
                }
                adapter.FillFromReader(this, reader, 0, 0);
                if (!reader.IsClosed && !reader.NextResult())
                {
                    reader.Close();
                }
            }
        }

        public class XLoadAdapter : DataAdapter
        {
            public XLoadAdapter()
            {
            }

            public int FillFromReader(DataSet ds, IDataReader dataReader, int startRecord, int maxRecords)
            {
                return this.Fill(ds, "Table", dataReader, startRecord, maxRecords);
            }
        }


        private static DataSet InitDS(DataSet ds)
        {
            foreach (DataTable dt in ds.Tables)
            {
                foreach (DataColumn item in dt.Columns)
                {
                    item.ReadOnly = false;
                }
            }

            return ds;
        }

        private static DataTable InitDT(DataTable dt)
        {
            foreach (DataColumn item in dt.Columns)
            {
                item.ReadOnly = false;
            }
            return dt;
        }

        public enum DBConnection
        {
            SystemHelper = 1,
            TMSSplitHelper = 2,
            OrderHelper=3,
            LogHelper=4,
        }
        /// <summary>
        /// 开发
        /// </summary>
        /// <param name = "DB" ></ param >
        /// < returns ></ returns >
        private static string GetConnectionStr(DBConnection DB)
        {
            string strConnection = "";
            int intDBValue = (int)DB;
            switch (intDBValue)
            {
                case 1:
                    strConnection = "Data Source=120.77.233.164;Initial Catalog=SystemHelper;User Id=tms_dev;Password=tms_dev;";
                    break;
                case 2:
                    strConnection = "Data Source=120.77.233.164;Initial Catalog=TMSSplitHelper;User Id=tms_dev;Password=tms_dev;";
                    break;
                //case 3:
                //    strConnection = "Data Source=10.1.1.123;Initial Catalog=CommonHelper;User Id=WMSAdmin;Password=fdsoerjcmqwo2@a.23s!afds;";
                //    break;
                case 4:
                    strConnection = "Data Source=120.77.233.164;Initial Catalog=LogHelper;User Id=tms_dev;Password=tms_dev;";
                    break;
                    //case 5:
                    //    strConnection = "Data Source=10.1.1.123;Initial Catalog=LogHelper;User Id=WMSAdmin;Password=fdsoerjcmqwo2@a.23s!afds;";
                    //    break;
                    //case 6:
                    //    strConnection = "Data Source=10.1.1.123;Initial Catalog=OrderHelper;User Id=WMSAdmin;Password=fdsoerjcmqwo2@a.23s!afds;";
                    //    break;
                    //case 7:
                    //    strConnection = "Data Source=10.1.1.123;Initial Catalog=WareHouseHelper;User Id=WMSAdmin;Password=fdsoerjcmqwo2@a.23s!afds;";
                    //    break;
            }
            return strConnection;
        }

        /// <summary>
        /// 生产
        /// </summary>
        /// <param name="DB"></param>
        /// <returns></returns>
        //private static string GetConnectionStr(DBConnection DB)
        //{
        //    string strConnection = "";
        //    int intDBValue = (int)DB;
        //    switch (intDBValue)
        //    {
        //        case 1:
        //            strConnection = "Data Source=db228.bestwo.net,19433;Initial Catalog=OrderHelper;User Id=aukeys_tms;Password=;";
        //            break;
        //        case 2:
        //            strConnection = "Data Source=db228.bestwo.net,19433;Initial Catalog=BestWoStock;User Id=aukeys_tms;Password=;";
        //            break;
        //        case 3:
        //            strConnection = "Data Source=db228.bestwo.net,19433;Initial Catalog=CommonHelper;User Id=aukeys_tms;Password=;";
        //            break;
        //        case 4:
        //            strConnection = "Data Source=db228.bestwo.net,19433;Initial Catalog=ErpHelper;User Id=aukeys_tms;Password=;";
        //            break;
        //        case 5:
        //            strConnection = "Data Source=db228.bestwo.net,19433;Initial Catalog=LogHelper;User Id=aukeys_tms;Password=;";
        //            break;
        //        case 6:
        //            strConnection = "Data Source=db228.bestwo.net,19433;Initial Catalog=OrderHelper;User Id=aukeys_tms;Password=;";
        //            break;
        //        case 7:
        //            strConnection = "Data Source=db228.bestwo.net,19433;Initial Catalog=WareHouseHelper;User Id=aukeys_tms;Password=;";
        //            break;
        //    }
        //    return strConnection;
        //}




    }
}
