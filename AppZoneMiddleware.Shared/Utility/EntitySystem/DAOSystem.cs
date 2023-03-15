using AppZoneMiddleware.Shared.Utility.ConnectionProcessor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Utility.EntitySystem
{
    public class DAOSystem
    {
        IDbConnection cn = null;
        public DAOSystem(string ProcessorKey)
        {
            DataAccessComponent DAC = new DataAccessComponent(ProcessorKey);
            cn = DAC.Connection;
        }

        //static string PostCardProcessorKey = "PostCard";
        //static string CBAProcessorKey = "CBA";
        public string RetrieveSpecificProperty(IDbCommand cmd, string columnName)
        {
            string data = string.Empty;
            if (cmd != null)
            {
                cmd.Connection = cn;

                cn.Open();
                try
                {
                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        int i = reader.RecordsAffected;
                        if (reader.Read())
                        {
                            data = reader[columnName].ToString().Trim();
                        }
                        reader.Close();
                        reader.Dispose();
                    }
                }
                catch { throw; }
                finally
                {
                    cmd.Dispose();
                    cn.Close();
                    cn.Dispose();
                }
            }
            return data;
        }

        public T RetrieveSingle<T>(IDbCommand cmd)
        {
            T entity = default(T);
            if (cmd != null)
            {
                cmd.Connection = cn;

                cn.Open();
                try
                {
                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        PropertyInfo[] properties = typeof(T).GetProperties();
                        string columnNname = string.Empty;
                        object newObject = null;
                        while (reader.Read())
                        {
                            entity = Activator.CreateInstance<T>();
                            for (int i = 0; i < properties.Length; i++)
                            {
                                try
                                {
                                    // and then assign the column to the object property
                                    columnNname = properties[i].Name;
                                    if (reader[columnNname].GetType() != typeof(DBNull))
                                    {
                                        properties[i].SetValue(entity, reader[columnNname], null);
                                    }
                                }
                                catch
                                {
                                    try
                                    {
                                        newObject = System.Convert.ChangeType(reader[columnNname], properties[i].PropertyType);
                                        properties[i].SetValue(entity, newObject, null);
                                    }
                                    catch { }
                                }
                            }
                        }
                        reader.Close();
                        reader.Dispose();
                    }
                }
                catch { throw; }
                finally
                {
                    cmd.Dispose();
                    cn.Close();
                    cn.Dispose();
                }
            }
            return entity;

        }

        public IList<T> RetrieveList<T>(IDbCommand cmd)
        {
            List<T> list = new List<T>();
            if (cmd != null)
            {
                cmd.Connection = cn;
                cn.Open();
                try
                {
                    using (IDataReader reader = cmd.ExecuteReader())
                    {
                        PropertyInfo[] properties = typeof(T).GetProperties();
                        string columnNname = string.Empty;
                        object newObject = null;
                        while (reader.Read())
                        {
                            T entity = Activator.CreateInstance<T>();
                            for (int i = 0; i < properties.Length; i++)
                            {
                                try
                                {
                                    // and then assign the column to the object property
                                    columnNname = properties[i].Name;
                                    if (reader[columnNname].GetType() != typeof(DBNull))
                                    {
                                        properties[i].SetValue(entity, reader[columnNname], null);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    try
                                    {
                                        newObject = System.Convert.ChangeType(reader[columnNname], properties[i].PropertyType);
                                        properties[i].SetValue(entity, newObject, null);
                                    }
                                    catch (Exception exx) { }
                                }
                            }
                            list.Add(entity);
                        }
                        reader.Close();
                        reader.Dispose();
                    }
                }
                catch { throw; }
                finally
                {
                    cmd.Dispose();
                    cn.Close();
                    cn.Dispose();
                }
            }

            return list;
        }


        private IDbTransaction _trans;
        IDbCommand cmd = null;
        public void StartSQL(string commandText)
        {
            if (!string.IsNullOrEmpty(commandText))
            {
                cmd = new System.Data.OleDb.OleDbCommand();
                if (cn.State == ConnectionState.Closed)
                {
                    cn.Open();
                }
                try
                {
                    _trans = cn.BeginTransaction();
                    cmd.Connection = cn;
                    cmd.Transaction = _trans;
                    cmd.CommandText = commandText;

                    int i = cmd.ExecuteNonQuery();
                }
                catch
                {
                    cmd.Dispose();
                    throw;
                }
            }
        }

        public void ContinueSQL(string commandText)
        {
            if (!string.IsNullOrEmpty(commandText))
            {
                try
                {
                    cmd.Transaction = _trans;
                    cmd.CommandText = commandText;

                    int i = cmd.ExecuteNonQuery();
                }
                catch
                {
                    cmd.Dispose();
                    throw;
                }

            }
        }

        public void CommitSQL()
        {
            if (_trans != null)
            {
                _trans.Commit();

                if (_trans.Connection != null)
                {
                    _trans.Connection.Close();
                    _trans.Connection.Dispose();
                }
            }
            _trans = null;
            cmd.Dispose();
        }

        public string CommitSQLAndRetrieveScopeID()
        {
            string ID = "";

            if (_trans != null)
            {
                _trans.Commit();
                cmd.CommandText = "SELECT SCOPE_IDENTITY() as ID;";
                ID = Convert.ToString(cmd.ExecuteScalar());

                if (_trans.Connection != null)
                {
                    _trans.Connection.Close();
                    _trans.Connection.Dispose();
                }
            }
            _trans = null;
            cmd.Dispose();

            return ID;
        }


        public void RollbackSQL()
        {
            if (_trans != null)
            {
                _trans.Rollback();

                if (_trans.Connection != null)
                {
                    _trans.Connection.Close();
                    _trans.Connection.Dispose();
                }
            }
            _trans = null;
        }

        public object ExecuteStoredProcedure()
        {
            IDbCommand cmd = new System.Data.OleDb.OleDbCommand();
            System.Data.OleDb.OleDbParameter returnValueParam = null;

            try
            {
                cn.Open();
                cmd.Connection = cn;

                cmd.CommandText = "select A04VFTAA ( (select nvl(BRA_CODE,1) from map_acct where map_acc_no = '0701023347')," +
                                 "(select nvl(CUS_NUM,1) from map_acct where map_acc_no = '0701023347')," +
                                 "(select nvl(CUR_CODE,1) from map_acct where map_acc_no = '0701023347')," +
                               "(select nvl(LED_CODE,1) from map_acct where map_acc_no = '0701023347')," +
                        "(select nvl(SUB_ACCT_CODE,1) from map_acct where map_acc_no = '0701023347')) AVAILABLE_BALANCE FROM DUAL";

                cmd.CommandType = CommandType.StoredProcedure;


                returnValueParam = new System.Data.OleDb.OleDbParameter("AVAILABLE_BALANCE", System.Data.OleDb.OleDbType.Numeric);
                returnValueParam.Direction = ParameterDirection.ReturnValue;
                cmd.Parameters.Add(returnValueParam);


                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Trace.TraceInformation(ex.ToString());
                Trace.Flush();
            }
            finally
            {
                cmd.Dispose();
            }


            return returnValueParam.Value;
        }

    }
}
