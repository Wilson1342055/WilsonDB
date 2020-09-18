using BestWoDP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            DataSet ds = null;
            try
            {
                IDataParameter[] parameters ={ 
                                            new SqlParameter("@orgId",SqlDbType.Int),
                                            new SqlParameter("@customerOrderIds",SqlDbType.NVarChar,5000),
                                            new SqlParameter("@PageIndex",SqlDbType.Int),
                                            new SqlParameter("@PageSize",SqlDbType.Int),
                                            new SqlParameter("@StartTime",SqlDbType.NVarChar,50),
                                            new SqlParameter("@EndTime",SqlDbType.NVarChar,50),
                                         };
                parameters[0].Value = 35;
                parameters[1].Value = "AK20033100013";
                parameters[2].Value = 1;
                parameters[3].Value = 200;
                parameters[4].Value = "";
                parameters[5].Value = "";


                ds = DapperHelper.RunProcedure_DataSet("[UP_OrderAndPackage_Qurey]", parameters,DapperHelper.DBConnection.OrderHelper);

                //ds = BaseBll.RunProcedure_DataSet("[UP_OrderAndPackage_Qurey]", parameters, "TmepOrderInfo", Aukeys.Frame.Dao.Util.DbConnection.getOrderSConnection());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            MessageBox.Show(ds.Tables.Count.ToString());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                IDataParameter[] parameters = { 
                                            new SqlParameter("@OrgId",SqlDbType.NVarChar,10),
                                            new SqlParameter("@StoreHouseId", SqlDbType.Int), 
                                            new SqlParameter("@StartTime", SqlDbType.NVarChar,50 ), 
                                            new SqlParameter("@EndTime", SqlDbType.NVarChar,50 ), 
                                            new SqlParameter("@PageIndex",SqlDbType.Int),
                                            new SqlParameter("@PageSize",SqlDbType.Int),
                                            new SqlParameter("@PageCount",SqlDbType.Int ), //页码总数
                                            new SqlParameter("@RecordCount",SqlDbType.Int ) //记录总数
                                         };
                parameters[0].Value = 35;
                parameters[1].Value = 9999;
                parameters[2].Value = "2020-03-31";
                parameters[3].Value = "2020-04-10";
                parameters[4].Value = 1;
                parameters[5].Value = 50;
                parameters[6].Direction = ParameterDirection.Output;
                parameters[7].Direction = ParameterDirection.Output;
                DataTable dt = DapperHelper.RunProcedure_DataTable("UP_OrderPackageAbnormal_Page_Query", parameters, DapperHelper.DBConnection.OrderHelper);
                int intResult = (int)parameters[6].Value;
                string strResult = parameters[7].Value.ToString();
                MessageBox.Show(dt.Rows.Count.ToString()+intResult.ToString()+strResult);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //int result = -1;
            //string errorMessage = "";
            //try
            //{
            //    IDataParameter[] parameters ={ 
            //        new SqlParameter("@customerOrderId",SqlDbType.NVarChar),
            //        new SqlParameter("@Islock",SqlDbType.Int),
            //        new SqlParameter("@lastUpdateby",SqlDbType.NVarChar,100),
            //        new SqlParameter("@ErrorMessage",SqlDbType.NVarChar,100 )
            //    };
            //    parameters[0].Value = "99888881";
            //    parameters[1].Value = 1;
            //    parameters[2].Value = "lwf";
            //    parameters[3].Direction = ParameterDirection.Output;
            //    //int rowsAffected = 0;
            //    //result = DapperHelper.RunProcedure_Return("[UP_OrderPackage_IsSetLock]", parameters, DapperHelper.OrderConnStr, out rowsAffected);
            //    object resultO = DapperHelper.RunProcedure_Return("[UP_OrderPackage_IsSetLock]", parameters, DapperHelper.OrderConnStr);
            //    result = Convert.ToInt32(resultO);
            //    errorMessage = parameters[3].Value.ToString();

            //}
            //catch (Exception ex)
            //{
            //    errorMessage = ex.Message;
            //}
            


            int result = -1;
            string errorMessage = "";
            try
            {
                IDataParameter[] parameters ={ 
                                         new SqlParameter("@customerOrderId",SqlDbType.NVarChar),
                                         new SqlParameter("@orgId",SqlDbType.Int),
                                         new SqlParameter("@OrderStatus",SqlDbType.Int),
                                         new SqlParameter("@lastUpdateby",SqlDbType.NVarChar),
                                         new SqlParameter("@ErrorMessage",SqlDbType.NVarChar,100 )
                                         };
                parameters[0].Value = "99888899";
                parameters[1].Value = 1;
                parameters[2].Value = 500;
                parameters[3].Value = "OMS";
                parameters[4].Direction = ParameterDirection.Output;
                int rowsAffected = 0;
                result = DapperHelper.RunProcedure_Return("[UP_TempOrderStatus_Update]", parameters, DapperHelper.DBConnection.OrderHelper, out rowsAffected);
                errorMessage = parameters[4].Value.ToString();
                MessageBox.Show(result.ToString() + errorMessage);

            }
            catch (Exception ex)
            {
                errorMessage =  ex.Message;
            }
            MessageBox.Show(result.ToString() + errorMessage);

        }
    }
}
