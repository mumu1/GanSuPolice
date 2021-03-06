﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using Beyon.Common;
using Beyon.Domain.Zhdd.zjjg;


namespace Beyon.WebService.ZhddPlatform.zzjgInfo
{
    /// <summary>
    /// 宗教寺庙
    /// </summary>
    public class TempleManager
    {
        private OleDbConnectionStringBuilder zzjgDBConnectBuilder;

        public TempleManager()
        {
            this.zzjgDBConnectBuilder = new OleDbConnectionStringBuilder();

            zzjgDBConnectBuilder.Add("Provider", "MSDAORA");
            zzjgDBConnectBuilder.Add("Data Source", ConfigHelper.GetValueByKey("webservice.config", "zzjgDB"));
            zzjgDBConnectBuilder.Add("Persist Security Info", true);
            zzjgDBConnectBuilder.Add("User ID", ConfigHelper.GetValueByKey("webservice.config", "zzjgDBUser"));
            zzjgDBConnectBuilder.Add("Password", ConfigHelper.GetValueByKey("webservice.config", "zzjgDBPasswd"));
        }

        /// <summary>
        /// 获取坐标范围内宗教场所
        /// </summary>
        /// <param name="minX"></param>
        /// <param name="minY"></param>
        /// <param name="maxX"></param>
        /// <param name="maxY"></param>
        /// <returns></returns>
        public List<Temple> GetAllTempleByExtent(double minX, double minY, double maxX, double maxY)
        {
            List<Temple> result = new List<Temple>();
            try
            {
                using (OleDbConnection conn = new OleDbConnection(this.zzjgDBConnectBuilder.ConnectionString))
                {
                    String sql = "select bh, csmc, xxdz, xqpcs, jyqk, cszpid, zjcsjd, zjcswd, key_zd from ZZJGXT.csgl_zjcsinfo t where  zjcsjd is not null and zjcswd is not null and zjcsjd >= ? and zjcswd >= ? and zjcsjd <= ? and zjcswd <= ?";
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(sql, conn);
                    cmd.Parameters.Add(new OleDbParameter("@minX", OleDbType.VarChar));
                    cmd.Parameters[0].Value = minX;
                    cmd.Parameters.Add(new OleDbParameter("@minY", OleDbType.VarChar));
                    cmd.Parameters[1].Value = minY;
                    cmd.Parameters.Add(new OleDbParameter("@maxX", OleDbType.VarChar));
                    cmd.Parameters[2].Value = maxX;
                    cmd.Parameters.Add(new OleDbParameter("@maxY", OleDbType.VarChar));
                    cmd.Parameters[3].Value = maxY;
                    OleDbDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Temple info = new Temple();

                        //编号
                        if (!reader.IsDBNull(0))
                        {
                            info.Bh = reader[0].ToString();
                        }
                        //场所名称
                        if (!reader.IsDBNull(1))
                        {
                            info.Csmc = reader[1].ToString();
                        }
                        //详细地址
                        if (!reader.IsDBNull(2))
                        {
                            info.Xxdz = reader[2].ToString();
                        }
                        //辖区派出所
                        if (!reader.IsDBNull(3))
                        {
                            info.Xqpcs = reader[3].ToString();
                        }
                        //建院情况
                        if (!reader.IsDBNull(4))
                        {
                            info.Jyqk = reader[4].ToString();
                        }
                        //场所照片ID
                        if (!reader.IsDBNull(5))
                        {
                            info.Cszpid = reader[5].ToString();
                        }
                        //宗教场所经度 纬度
                        if (!reader.IsDBNull(6) && !reader.IsDBNull(7))
                        {
                            double x, y;
                            Double.TryParse(reader[6].ToString(), out x);
                            if (x > 0)
                            {
                                info.ZjcsJd = x;
                            }

                            Double.TryParse(reader[7].ToString(), out y);
                            if (y > 0)
                            {
                                info.ZjcsWd = y;
                            }
                        }

                        //key_zd
                        if(!reader.IsDBNull(8))
                        {
                            info.Key_zd = reader[8].ToString();
                        }

                        //选取屏幕坐标范围内宗教场所
                        if (info.ZjcsJd > 0 && info.ZjcsWd > 0 && info.ZjcsJd >= minX && info.ZjcsWd >= minY && info.ZjcsJd <= maxX && info.ZjcsWd <= maxY)
                        {
                            result.Add(info);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
    }
}
