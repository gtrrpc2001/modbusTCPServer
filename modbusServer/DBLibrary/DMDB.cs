#define DMDBMYSQL
//#define DMDBMSSQL

// 1111111111111111111111111111111111111111
#if (DMDBMYSQL)

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using static lunar.cs;

namespace lunar
{
    public class DMDB
    {
        private string server;
        public string db;
        private string uid;
        private string password;
        public string port;


        public static MySqlConnection connection = null;
        public DataGridView datagv;
        private MySqlDataAdapter mySqlDataAdapter;
        static public string loginname = "";
        ContextMenuStrip mchstr = new ContextMenuStrip();
        int selectcolumnh;
        DataGridView selectdgv;
        public string driver = "mysql";

        public static Dictionary<string, DataTable> dicCache = new Dictionary<string, DataTable>();

        public static Thread threadLoop = null;
        public static Boolean threadLoopIsRun = true;
        public static List<string> lSQLQueue = new List<string>();

        public DMDB(string dbname = "", string settingname = "setting.ini")
        {
            if(threadLoop == null)
            {
                threadLoop = new Thread(() => doLoop());
                threadLoop.Start();
            }

            //Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            if (cs.LocalParamGet("dbname") == "dair_ns")
            {
            }

            if (cs.LocalParamGet("trgencrypt") == "1")
            {
                try
                {
                    List<string> ll = new List<string>();
                    ll.Add("dbip");
                    ll.Add("dbid");
                    ll.Add("dbpw");

                    foreach (string s in ll)
                    {
                        string s1 = cs.LocalParamGet(s);
                        string s2 = cs.Encrypt(s1);
                        cs.LocalParamSet(s, s2);
                    }

                    cs.LocalParamSet("trgencrypt", "0");
                }
                catch (Exception E)
                {
                    cs.logError(E);
                }
            }
            db = cs.LocalParamGet("dbname", "", false, settingname);
            //connectinit("nms_kjfood");
            if (dbname != "")
            {
                db = dbname;
            }
            if (cs._APP == "")
            {
                cs._APP = db;
            }
            connectinit(db, settingname);// "nms_gn");

            try
            {
                if (cs.LocalParamGet("isdev") == "1")
                {
                    Dictionary<string, DataRow> dicSetting = getFieldInfo("_table_setting");
                    if (dicSetting.ContainsKey("필수") == false)
                    {
                        string sql = "alter table _table_setting add 필수 int(11)  DEFAULT 0;";
                        sqlExec(sql);
                    }
                    if (dicSetting.ContainsKey("표시형식") == false)
                    {
                        string sql = "alter table _table_setting add 표시형식 varchar(255)  DEFAULT '';";
                        sqlExec(sql);
                    }
                }
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
        }

        public static void OnApplicationExit(object sender, EventArgs e)
        {
            threadLoopIsRun = false;
        }

        public static void doLoop()
        {
            while (threadLoopIsRun)
            {
                try
                {
                    List<string> ll = null;
                    lock (lSQLQueue)
                    {
                        if (lSQLQueue.Count > 0)
                        {
                            ll = lSQLQueue;
                            lSQLQueue = new List<string>();
                        }
                    }

                    if (ll != null)
                    {
                        while (ll.Count > 0)
                        {
                            string sql = ll[0];
                            ll.RemoveAt(0);

                            DMDB db = new DMDB();
                            db.sqlExec(sql);
                        }
                    }
                }
                catch (Exception E)
                {
                    cs.logError(E);
                }
                Thread.Sleep(1);
            }
        }

        public static void sqlPush(string sql)
        {
            lock (lSQLQueue)
            {
                lSQLQueue.Add(sql);
            }
        }

        public void connectinit(string database, string settingname = "")
        {
            db = database;

            if (settingname != "")
            {
                server = cs.Decrypt(cs.LocalParamGet("dbip", "sEN8lBhubYhRr1w6mVDilQ==", false, settingname)); //"49.247.204.43";                                                                                                       //database = "testmes";
                uid = cs.Decrypt(cs.LocalParamGet("dbid", "hPDPIsRm/gDv+aHsU+c98Q==", false, settingname)); //"nsdev";
                password = cs.Decrypt(cs.LocalParamGet("dbpw", "L0GJ733MnzSLWtzGYrkWwqg2dAkIowEYsByHHFgLyyo=", false, settingname)); //"!eogksehrflqakstp!";
                db = cs.LocalParamGet("dbname", "L0GJ733MnzSLWtzGYrkWwqg2dAkIowEYsByHHFgLyyo=", false, settingname); //"!eogksehrflqakstp!";
            }
            else
            {
                if (cs.LocalParamGet("isdev") == "1")
                {
                    if (db == "kosmo")
                    {
                        server = "115.68.118.17";                                                                                             //database = "testmes";
                        uid = "sj";
                        password = "eogksalsrnr";
                    }
                    else
                    {
                        server = cs.LocalParamGet("dbip", "sEN8lBhubYhRr1w6mVDilQ=="); //"49.247.204.43";                                                                                                       //database = "testmes";
                        uid = cs.LocalParamGet("dbid", "hPDPIsRm/gDv+aHsU+c98Q=="); //"nsdev";
                        password = cs.LocalParamGet("dbpw", "L0GJ733MnzSLWtzGYrkWwqg2dAkIowEYsByHHFgLyyo="); //"!eogksehrflqakstp!";
                    }
                }
                else
                {

                    server = cs.LocalParamGet("dbip", "sEN8lBhubYhRr1w6mVDilQ=="); //"49.247.204.43";                                                                                                       //database = "testmes";
                    uid = cs.LocalParamGet("dbid", "hPDPIsRm/gDv+aHsU+c98Q=="); //"nsdev";
                    password = cs.LocalParamGet("dbpw", "L0GJ733MnzSLWtzGYrkWwqg2dAkIowEYsByHHFgLyyo="); //"!eogksehrflqakstp!";

                    //server = cs.Decrypt(cs.LocalParamGet("dbip", "sEN8lBhubYhRr1w6mVDilQ=="));//, cs._KEY_DM); //"49.247.204.43";
                    //uid = cs.Decrypt(cs.LocalParamGet("dbid", "hPDPIsRm/gDv+aHsU+c98Q=="));//, cs._KEY_DM); //"nsdev";
                    //password = cs.Decrypt(cs.LocalParamGet("dbpw", "L0GJ733MnzSLWtzGYrkWwqg2dAkIowEYsByHHFgLyyo="));//, cs._KEY_DM); //"!eogksehrflqakstp!";
                }
            }
            port = cs.LocalParamGet("dbport", "3306");

            string connectionString;
            //if (driver=="mssql")
            connectionString = "SERVER=" + server + ";Port=" + port + ";DATABASE=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password +
                ";Charset=utf8;Allow Zero Datetime=True;respect binary flags=false;SslMode=none;";
            //else
            //connectionString = "SERVER=" + server + ";Port=" + port + ";DATABASE=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";Charset=utf8;";
            if (connection == null)
            {
                connection = new MySqlConnection(connectionString);
            }
            else
            {

            }
        }

        public static Boolean isLoginCheck(string id, string pw)
        {
            try
            {
                DMDB db = new DMDB();
                string pwhash = db.sqlToText("select getHashedPW('" + id + "'); ");
                string pwsalt = db.sqlToText("select getSaltedPW('" + id + "'); ");
                if (HashSalt.VerifyPassword(pw, pwhash, pwsalt))
                {
                    db.sqlExec("update 인원_목록 set 잠금=0, 잠금시각=null where 아이디 = '" + id + "' ");
                    return true;
                }
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            return false;
        }

        public string doDupeRow(string table, string where, string newodcode = "")
        {
            try
            {
                List<string> lc = getFieldList(table);

                if (lc[0] == "idx")
                    lc.RemoveAt(0);

                string sql = "insert into " + table + "(";
                sql = sql + string.Join(",", lc);

                if (newodcode != "")
                {
                    int i = lc.IndexOf("수주번호");
                    if (i >= 0)
                        lc[i] = "'" + newodcode + "'";
                }

                sql = sql + ") select " + string.Join(",", lc) + " from " + table + " where " + where;

                string idx = sqlExec(sql);
                return idx;
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            return "";
        }

        public string sqlExecRaw(string sql)
        {
            string rr = "";
            Boolean r = false;

            Boolean isNeedMsg = false;

            string sMsg = "";

            //if (sql.Contains("\n"))
            //{
            //    sql = sql.Replace("\n", "");
            //}
            //if (sql.Contains("\r"))
            //{
            //    sql = sql.Replace("\r", "");
            //}
            //if (sql.Contains("\""))
            //{
            //    sql = sql.Replace("\"", "/");
            //}

            cs.logSQL(sql);

            lock (connection)
            {
                try
                {
                    MySqlCommand myCommand = new MySqlCommand(sql, connection);
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    myCommand.ExecuteNonQuery();
                    r = true;
                    rr = myCommand.LastInsertedId.ToString();
                }
                catch (Exception E)
                {
                    sMsg = E.Message;
                    cs.logError(E);
                    cs.LogErrorMsg(sql);
                    if (isNeedMsg)
                    {
                        isNeedMsg = true;
                        cs.LogDebug("[_table_setting] ERROR");
                        cs.LogDebug("[_table_setting] " + E.Message);
                        cs.LogDebug("[_table_setting] " + E.StackTrace);
                        cs.LogDebug("[_table_setting] " + sql);

                        MessageBox.Show("[관리자에게 문의 하세요]\r\n" + E.Message);
                    }
                }
            }

            return rr;
        }

        public string sqlExec(string sql)
        {
            string rr = "";
            Boolean r = false;

            Boolean isNeedMsg = false;

            string sMsg = "";

            if (sql.Contains("\n"))
            {
                sql = sql.Replace("\n", "");
            }
            if (sql.Contains("\r"))
            {
                sql = sql.Replace("\r", "");
            }
            if (sql.Contains("\""))
            {
                sql = sql.Replace("\"", "/");
            }

            cs.logSQL(sql);
            try
            {
                if (sql.Contains("delete from _table_setting") ||
                    sql.Contains("update _table_setting") ||
                    sql.Contains("delete from _table_link") ||
                    sql.Contains("update _table_link")
                    )
                {
                    isNeedMsg = true;
                    cs.LogDebug("[_table_setting] " + sql);
                }
            }
            catch (Exception E)
            {
                cs.logError(E);
            }

            lock (connection)
            {
                try
                {
                    MySqlCommand myCommand = new MySqlCommand(sql, connection);
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    myCommand.ExecuteNonQuery();
                    r = true;
                    rr = myCommand.LastInsertedId.ToString();
                }
                catch (Exception E)
                {
                    sMsg = E.Message;
                    cs.logError(E);
                    cs.LogErrorMsg(sql);
                    if (isNeedMsg)
                    {
                        isNeedMsg = true;
                        cs.LogDebug("[_table_setting] ERROR");
                        cs.LogDebug("[_table_setting] " + E.Message);
                        cs.LogDebug("[_table_setting] " + E.StackTrace);
                        cs.LogDebug("[_table_setting] " + sql);

                        MessageBox.Show("[관리자에게 문의 하세요]\r\n" + E.Message);
                    }
                }
            }

            //if (cs.LocalParamGet("istracesql") == "1")
            {
                try
                {
                    if (sql.Contains("delete") ||
                        sql.Contains("update")
                       )
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        dic.Add("ID", cs.empid);
                        dic.Add("내용", sql.Replace("'", "|"));
                        dic.Add("결과", sMsg.Replace("'", "|"));
                        insertDic("_log", dic);
                    }
                }
                catch (Exception E)
                {
                    cs.logError(E);
                }
            }

            return rr;
        }

        public string sqlExec(string sql, out string result)
        {
            string rr = "";
            Boolean r = false;

            Boolean isNeedMsg = false;

            string sMsg = "";

            result = "";

            if (sql.Contains("\n"))
            {
                sql = sql.Replace("\n", "");
            }
            if (sql.Contains("\r"))
            {
                sql = sql.Replace("\r", "");
            }
            if (sql.Contains("\""))
            {
                sql = sql.Replace("\"", "/");
            }

            cs.logSQL(sql);
            try
            {
                if (sql.Contains("delete from _table_setting") ||
                    sql.Contains("update _table_setting") ||
                    sql.Contains("delete from _table_link") ||
                    sql.Contains("update _table_link")
                    )
                {
                    isNeedMsg = true;
                    cs.LogDebug("[_table_setting] " + sql);
                }
            }
            catch (Exception E)
            {
                cs.logError(E);
            }

            lock (connection)
            {
                try
                {
                    MySqlCommand myCommand = new MySqlCommand(sql, connection);
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    myCommand.ExecuteNonQuery();
                    r = true;
                    rr = myCommand.LastInsertedId.ToString();
                }
                catch (Exception E)
                {
                    sMsg = E.Message;
                    cs.logError(E);
                    cs.LogErrorMsg(sql);
                    if (isNeedMsg)
                    {
                        isNeedMsg = true;
                        cs.LogDebug("[_table_setting] ERROR");
                        cs.LogDebug("[_table_setting] " + E.Message);
                        cs.LogDebug("[_table_setting] " + E.StackTrace);
                        cs.LogDebug("[_table_setting] " + sql);

                        MessageBox.Show("[관리자에게 문의 하세요]\r\n" + E.Message);
                    }

                    result = sMsg;
                }
            }

            if (cs.LocalParamGet("istracesql") == "1")
            {
                try
                {
                    if (sql.Contains("delete") ||
                        sql.Contains("update")
                       )
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        dic.Add("ID", cs.empid);
                        dic.Add("내용", sql.Replace("'", "|"));
                        dic.Add("결과", sMsg.Replace("'", "|"));
                        insertDic("_log", dic);
                    }
                }
                catch (Exception E)
                {
                    cs.logError(E);
                }
            }

            return rr;
        }

        //public Boolean sqlExec(string sql, out string result)
        //{
        //    result = "";

        //    if (sql.Contains("\n"))
        //    {
        //        sql = sql.Replace("\n", "");
        //    }
        //    if (sql.Contains("\r"))
        //    {
        //        sql = sql.Replace("\r", "");
        //    }
        //    if (sql.Contains("\""))
        //    {
        //        sql = sql.Replace("\"", "/");
        //    }

        //    Boolean r = false;
        //    cs.logSQL(sql);
        //    lock (connection)
        //    {
        //        try
        //        {
        //            MySqlCommand myCommand = new MySqlCommand(sql, connection);
        //            if (connection.State == ConnectionState.Closed)
        //                connection.Open();
        //            myCommand.ExecuteNonQuery();
        //            r = true;
        //        }
        //        catch (Exception E)
        //        {
        //            result = E.Message;
        //            cs.logError(E);
        //            cs.LogErrorMsg(sql);
        //        }
        //    }

        //    if (cs.LocalParamGet("istracesql") == "1")
        //    {
        //        try
        //        {
        //            if (sql.Contains("delete") ||
        //                sql.Contains("update")
        //               )
        //            {
        //                Dictionary<string, string> dic = new Dictionary<string, string>();
        //                dic.Add("ID", cs.empid);
        //                dic.Add("내용", sql.Replace("'", "|"));
        //                dic.Add("결과", result.Replace("'", "|"));
        //                insertDic("_log", dic);
        //            }
        //        }
        //        catch (Exception E)
        //        {
        //            cs.logError(E);
        //        }
        //    }
        //    return r;
        //}

        public Dictionary<string, DataRow> sqlToDic(string sql, string keycol)
        {
            Dictionary<string, DataRow> dic = new Dictionary<string, DataRow>();
            try
            {
                DataTable dt = sqlToDT(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    string key = dr[keycol].ToString();
                    if (dic.ContainsKey(key) == false)
                    {
                        dic.Add(key, dr);
                    }
                }
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            return dic;
        }

        public DataTable sqlToDT(string sql)
        {
            DataTable r = null;

            if (sql.Contains("FROM `information_schema`.`columns`"))
            {
                if (dicCache.TryGetValue(sql, out r))
                {
                    cs.logSQL("[USE CACHE] " + sql);
                    return r;
                }
            }
            cs.logSQL(sql);

            DataSet DS = null;
            lock (connection)
            {
                try
                {
                    mySqlDataAdapter = new MySqlDataAdapter(sql, connection);
                    DS = new DataSet();
                    mySqlDataAdapter.Fill(DS);

                    r = DS.Tables[0];
                    if (sql.Contains("FROM `information_schema`.`columns`"))
                    {
                        if (dicCache.ContainsKey(sql) == false)
                        {
                            dicCache.Add(sql, r);
                        }
                    }
                }
                catch (Exception E)
                {
                    cs.logError(E);
                    cs.LogErrorMsg(sql);                    
                }
            }
            return r;
        }

        public string sqlToText(string sql)
        {
            string rr = "";
            lock (connection)
            {
                try
                {
                    mySqlDataAdapter = new MySqlDataAdapter(sql, connection);
                    DataSet DS = new DataSet();
                    mySqlDataAdapter.Fill(DS);

                    DataRow dr = DS.Tables[0].Rows[0];
                    rr = dr[0].ToString(); 
                }
                catch
                {

                }
            }
            return rr;
        }

        public int sqlToInt(string sql)
        {
            cs.logSQL(sql);
            int i = -1;
            string rr = "";
            lock (connection)
            {
                try
                {
                    mySqlDataAdapter = new MySqlDataAdapter(sql, connection);
                    DataSet DS = new DataSet();
                    mySqlDataAdapter.Fill(DS);

                    DataRow dr = DS.Tables[0].Rows[0];
                    rr = dr[0].ToString();

                    Int32.TryParse(rr, out i);
                }
                catch
                {

                }
            }
            return i;
        }

        public double sqlToDouble(string sql, double ddefault = -1)
        {
            cs.logSQL(sql);
            double i = ddefault;
            string rr = "";
            lock (connection)
            {
                try
                {
                    mySqlDataAdapter = new MySqlDataAdapter(sql, connection);
                    DataSet DS = new DataSet();
                    mySqlDataAdapter.Fill(DS);

                    DataRow dr = DS.Tables[0].Rows[0];
                    rr = dr[0].ToString();

                    if (double.TryParse(rr, out i) == false)
                    {
                        i = ddefault;
                    }
                }
                catch
                {

                }
            }
            return i;
        }

        public long sqlToLong(string sql)
        {
            long i = 0;
            string rr = "";
            lock (connection)
            {
                try
                {
                    mySqlDataAdapter = new MySqlDataAdapter(sql, connection);
                    DataSet DS = new DataSet();
                    mySqlDataAdapter.Fill(DS);

                    DataRow dr = DS.Tables[0].Rows[0];
                    rr = dr[0].ToString();

                    long.TryParse(rr, out i);
                }
                catch
                {

                }
            }
            return i;
        }

        public DataRow sqlToDR(string sql)
        {
            DataRow rr = null;
            lock (connection)
            {
                try
                {
                    mySqlDataAdapter = new MySqlDataAdapter(sql, connection);
                    DataSet DS = new DataSet();
                    mySqlDataAdapter.Fill(DS);

                    if (DS.Tables[0].Rows.Count > 0)
                        rr = DS.Tables[0].Rows[0];
                }
                catch (Exception E)
                {
                    cs.LogErrorMsg(sql);
                    cs.logError(E);
                }
            }
            return rr;
        }

        public List<string> getFieldList(string table, Boolean AddWrap = false)
        {
            List<string> ll = new List<string>();
            try
            {
                string sql = "SELECT tbl.column_name as `필드명`,tbl.ORDINAL_POSITION AS `no` FROM `information_schema`.`columns` AS tbl ";
                sql += "where  `table_schema` = '" + db + "' and `table_name` = '" + table + "' ORDER BY tbl.ORDINAL_POSITION";

                DataTable dt = sqlToDT(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    if (AddWrap)
                        ll.Add("`" + dr["필드명"].ToString() + "`");
                    else
                        ll.Add(dr["필드명"].ToString());
                }
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            return ll;
        }

        public Dictionary<string, DataRow> getFieldInfo(string table)
        {
            Dictionary<string, DataRow> r = new Dictionary<string, DataRow>();
            try
            {
                string sql = "SELECT * FROM `information_schema`.`columns` AS tbl ";
                sql += "where  `table_schema` = '" + db + "' and `table_name` = '" + table + "' ";

                DataTable dt = sqlToDT(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    r.Add(dr["column_name"].ToString(), dr);
                }
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            return r;
        }

        public Boolean getFieldIsString(string table, string field)
        {
            Boolean r = true;
            try
            {
                string sql = "SELECT * FROM `information_schema`.`columns` AS tbl ";
                sql += "where  `table_schema` = '" + db + "' and `table_name` = '" + table + "' and `column_name` = '" + field + "' ";

                DataTable dt = sqlToDT(sql);
                if (dt != null)
                {
                    if (dt.Rows.Count == 1)
                    {
                        string DataType = dt.Rows[0]["data_type"].ToString();

                        if (DataType.Contains("varchar") || DataType == "text")
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            return r;
        }

        public Dictionary<string, DataRow> getTableSetting(string table)
        {
            Dictionary<string, DataRow> r = new Dictionary<string, DataRow>();
            try
            {
                string sql = "SELECT * from _table_setting where 테이블 = '" + table + "' ";

                DataTable dt = sqlToDT(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    r.Add(dr["필드"].ToString().ToUpper(), dr);
                }
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            return r;
        }

        public DataTable getTableInfo(string table)
        {
            DataTable dt = null;
            try
            {
                string sql = "SELECT * FROM `information_schema`.`columns` AS tbl ";
                sql += "where  `table_schema` = '" + db + "' and `table_name` = '" + table + "' ";

                dt = sqlToDT(sql);
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            return dt;
        }

        public int login(string id, string pw)
        {
            int myCount = 0;

            if (this.OpenConnection() == true)
            {
                string data = "select count(*)  from emp_list where id = '" + id + "' and pw = '" + pw + "'";
                MySqlCommand count = new MySqlCommand(data, connection);
                myCount = Convert.ToInt32(count.ExecuteScalar());

                if (myCount != 0)
                {
                    MySqlCommand cmd = new MySqlCommand("select emp_name from emp_list where emp_id = '" + id + "' and emp_pw = '" + pw + "'", connection);
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        //Console.WriteLine("{0}: {1}", rdr["Id"], rdr["Name"]);
                        loginname = rdr["emp_name"].ToString();
                    }
                    rdr.Close();
                }

                //close connection
                this.CloseConnection();
            }
            return myCount;
        }

        public bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("Cannot connect to server. Contact administrator");
                        break;
                    case 1045:
                        MessageBox.Show("Invalid username/password, please try again");
                        break;
                    default:
                        MessageBox.Show(ex.Message);
                        break;
                }
                return false;
            }
        }

        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private void datagv_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            DataTable changes = ((DataTable)datagv.DataSource).GetChanges();

            if (changes != null)
            {
                MySqlCommandBuilder mcb = new MySqlCommandBuilder(mySqlDataAdapter);
                mySqlDataAdapter.UpdateCommand = mcb.GetUpdateCommand();
                mySqlDataAdapter.Update(changes);
                ((DataTable)datagv.DataSource).AcceptChanges();
            }

        }

        public List<string> getTableList(string sw = "")
        {
            List<string> l = new List<string>();
            try
            {
                string s = "SHOW TABLES FROM " + db;
                if (sw != "")
                {
                    s = s + " " + sw;
                }

                DataTable dt = sqlToDT(s);
                foreach (DataRow dr in dt.Rows)
                {
                    l.Add(dr[0].ToString());
                }
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            return l;
        }

        public Boolean doIn(DateTime dtAdd, string kind, string code, double qty, string whfrom = "", string whto = "", string emp = "", string lot = "", string wpno = "", Dictionary<string, string> dicExtColValue = null)
        {
            Boolean r = false;
            try
            {
                if (emp == "")
                    emp = cs.empid;

                List<string> lCol = new List<string>();
                lCol.Add("종류");
                lCol.Add("품번");
                lCol.Add("수량"); // 수량
                lCol.Add("생산수량"); // 수량
                lCol.Add("기존창고");
                lCol.Add("이동창고");
                lCol.Add("등록자");
                lCol.Add("LOT");
                lCol.Add("연결번호");
                lCol.Add("수불일");

                List<string> lValue = new List<string>();
                lValue.Add("'" + kind + "' ");
                lValue.Add("'" + code + "' ");
                lValue.Add("" + qty.ToString() + " ");
                lValue.Add("" + qty.ToString() + " ");
                lValue.Add("'" + whfrom + "' ");
                lValue.Add("'" + whto + "' ");
                lValue.Add("'" + cs.empid + "' ");
                lValue.Add("'" + lot + "' ");
                lValue.Add("'" + wpno + "' ");
                lValue.Add("'" + dtAdd.ToString("yyyy-MM-dd HH:mm:ss") + "' ");

                if (dicExtColValue != null)
                {
                    foreach (KeyValuePair<string, string> kv in dicExtColValue)
                    {
                        lCol.Add(kv.Key);
                        lValue.Add(kv.Value);
                    }
                }

                string sql = "insert into item_inout_history (";

                sql += string.Join(",", lCol) + ") values (";

                sql += string.Join(",", lValue) + ")";

                string idx = sqlExec(sql);
                sqlExec("update item_inout_history set 명령='미적용' where idx_ioh = " + idx);
                r = true;
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            return r;
        }

        public void calcStock()
        {
            try
            {
                DataTable dtDate = sqlToDT("select 등록일 from 재고이력_목록 where 등록일 < current_date group by 등록일");

                Dictionary<string, string> dicIn = new Dictionary<string, string>();
                dicIn.Add("구매입고", "");

                Dictionary<string, string> dicOut = new Dictionary<string, string>();
                dicIn.Add("자재불출", "");

                foreach (DataRow drDate in dtDate.Rows)
                {
                    string sql = "select ";
                }
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
        }

        public Boolean doOut(DateTime dtAdd, string kind, string code, double qty, string whfrom = "", string whto = "", string emp = "", string lot = "", string wpno = "", Dictionary<string, string> dicExtColValue = null)
        {
            Boolean r = false;
            try
            {
                if (emp == "")
                    emp = cs.empid;

                List<string> lCol = new List<string>();
                lCol.Add("종류");
                lCol.Add("품번");
                lCol.Add("수량"); // 수량
                lCol.Add("생산수량"); // 수량
                lCol.Add("기존창고");
                lCol.Add("이동창고");
                lCol.Add("등록자");
                lCol.Add("LOT");
                lCol.Add("연결번호");
                lCol.Add("수불일");

                List<string> lValue = new List<string>();
                lValue.Add("'" + kind + "' ");
                lValue.Add("'" + code + "' ");
                lValue.Add("" + qty.ToString() + " ");
                lValue.Add("" + qty.ToString() + " ");
                lValue.Add("'" + whfrom + "' ");
                lValue.Add("'" + whto + "' ");
                lValue.Add("'" + cs.empid + "' ");
                lValue.Add("'" + lot + "' ");
                lValue.Add("'" + wpno + "' ");
                lValue.Add("'" + dtAdd.ToString("yyyy-MM-dd HH:mm:ss") + "' ");

                if (dicExtColValue != null)
                {
                    foreach (KeyValuePair<string, string> kv in dicExtColValue)
                    {
                        lCol.Add(kv.Key);
                        lValue.Add(kv.Value);
                    }
                }

                string sql = "insert into item_inout_history (";

                sql += string.Join(",", lCol) + ") values (";

                sql += string.Join(",", lValue) + ")";

                string idx = sqlExec(sql);
                sqlExec("update item_inout_history set 명령='미적용' where idx_ioh = " + idx);
                r = true;
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            return r;
        }

        public string makeCPTax(string org)
        {
            string r = "";
            try
            {
                r = org.Replace("-", "").Replace(" ", "");
                long l = 0;
                if (long.TryParse(r, out l))
                {
                    r = l.ToString();
                    r = r.Substring(0, 3) + "-" + r.Substring(3, 2) + "-" + r.Substring(5, 5);
                }
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            return r;
        }

        public string insertDic(string table, Dictionary<string, string> dic)
        {
            string r = "";
            string err = "";
            return insertDicWithErr(table, dic, out err);
        }

        public string insertDicWithErr(string table, Dictionary<string, string> dic, out string ErrMsg)
        {
            ErrMsg = "";
            string r = "";
            try
            {
                string sql = "insert into " + table;

                List<string> lCol = new List<string>();
                List<string> lValue = new List<string>();

                foreach (KeyValuePair<string, string> kv in dic)
                {
                    if (kv.Key.StartsWith("idx"))
                        continue;
                    if (kv.Key == "#ROW#")
                    {

                    }
                    else
                    {
                        lCol.Add("`" + kv.Key + "`");

                        if (kv.Value.Contains("#"))
                        {
                            string si = kv.Value.Replace("#", "");

                            if (si.Contains("."))
                            {
                                double dd = 0;
                                if (double.TryParse(si, out dd))
                                {
                                    lValue.Add(si);
                                }
                                else
                                {
                                    // 논리 버그
                                    lValue.Add("0");
                                    //lValue.Add("'" + kv.Value + "'");
                                }
                            }
                            else
                            {
                                long ii = 0;
                                if (long.TryParse(si, out ii))
                                {
                                    lValue.Add(ii.ToString());
                                }
                                else
                                {
                                    lValue.Add("0");
                                    //lValue.Add("'" + kv.Value + "'");
                                }
                            }
                        }

                        else
                        {
                            lValue.Add("'" + kv.Value + "'");
                        }
                    }
                }
                sql += ("(" + string.Join(",", lCol) + ")");
                sql += (" values (" + string.Join(",", lValue) + ");");

                //sql = sql.Replace("\\", "\\\\");

                cs.logSQL(sql);
                lock (connection)
                {
                    MySqlCommand myCommand = new MySqlCommand(sql, connection);
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    myCommand.ExecuteNonQuery();

                    r = myCommand.LastInsertedId.ToString();
                }
            }
            catch (Exception E)
            {
                ErrMsg = E.Message;
                cs.logError(E);
            }
            return r;
        }

        public string updateDic(string table, Dictionary<string, string> dic, string where)
        {
            string r = "";
            try
            {
                
                string sql = "update " + table + " set ";

                List<string> lCol = new List<string>();
                List<string> lValue = new List<string>();

                foreach (KeyValuePair<string, string> kv in dic)
                {
                    string sv = "";

                    if (kv.Key.StartsWith("idx"))
                        continue;
                    if (kv.Key == "#ROW#")
                    {

                    }
                    else
                    {
                        if (kv.Value.Contains("#"))
                        {
                            string si = kv.Value.Replace("#", "");
                            long ii = 0;
                            if (long.TryParse(si, out ii))
                            {
                                sv = ii.ToString();
                            }
                            else
                            {
                                double dd = 0;
                                if (double.TryParse(si, out dd))
                                {
                                    sv = dd.ToString();
                                }
                                else
                                {
                                    sv = "'" + kv.Value + "'";
                                }
                            }
                        }

                        else
                        {
                            sv = "'" + kv.Value + "'";
                        }

                        lCol.Add("`" + kv.Key + "` = " + sv);
                    }
                }
                sql += string.Join(",", lCol);
                sql += " where " + where;

                sqlExec(sql);
            }
            catch (Exception E)
            {
                r = E.Message;
                cs.logError(E);
            }
            return r;
        }

        public int getAutoIdx(string table)
        {
            try
            {
                DataTable dt = sqlToDT("show table status where name = '" + table + "'");
                if (dt.Rows.Count > 0)
                {
                    string s = dt.Rows[0]["auto_increment"].ToString();
                    int i = 0;
                    Int32.TryParse(s, out i);
                    return i;
                }
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            return 0;
        }

        public string makeDateKeyCode(string atable)
        {
            int i = getAutoIdx(atable);
            if (atable.StartsWith("v_"))
            {
                i = cs.strToInt(DateTime.Now.ToString("HHmmss"));
            }
            return DateTime.Now.ToString("yyyyMM") + (i + 1).ToString("000000");
        }

        public string makeDateIdxKeyCode(string atable)
        {
            int i = getAutoIdx(atable);            
            return DateTime.Now.ToString("yyyyMM") + (i + 1).ToString("000000");
        }

        public string makeKeyCode(string atable)
        {
            int i = getAutoIdx(atable);
            return (i + 1).ToString("000000");
        }

        public string makeIdxCode(string atable)
        {
            int i = getAutoIdx(atable);
            return "CD" + (i + 1).ToString("000000");
        }

        public string makeCPCode()
        {
            int i = getAutoIdx("업체_목록");
            return "CP" + (i + 1).ToString("000000");
        }
        public string makeODCode(string table = "수주_목록")
        {
            int i = getAutoIdx(table);
            return "OD" + DateTime.Now.ToString("yyMM") + (i + 1).ToString("000000");
        }
        public string makeIRCode(string table = "견적_목록")
        {
            int i = getAutoIdx(table);
            return "OD" + DateTime.Now.ToString("yyMM") + (i + 1).ToString("000000");
        }


        public string makeRCCode()
        {
            int i = getAutoIdx("거래관리_내용_목록");
            return "RC" + DateTime.Now.ToString("yyyyMM") + (i + 1).ToString("000000");
        }

        // GN
        public double GNGetItemPriceViaMonth(string itemcode, int month = 0)
        {
            double r = 0;

            if (month == 0)
                month = DateTime.Now.Month;

            r = sqlToInt("select " + month.ToString() + " from item_list_gn_product where 제품code = '" + itemcode + "' ");

            return r;
        }

        public void tableToCombo(string table, string col, ComboBox combo)
        {
            try
            {
                combo.Items.Clear();
                DataTable dt = sqlToDT("select " + col + " from " + table + " group by " + col + " order by " + col);
                foreach (DataRow dr in dt.Rows)
                {
                    string v = dr[col].ToString();

                    combo.Items.Add(v);
                }
                if (combo.Items.Count > 0)
                    combo.SelectedIndex = 0;
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
        }

        public string getSQL(string file)
        {
            string r = "";
            try
            {
                string sfile = Application.StartupPath + "\\sql\\" + file;
                if (File.Exists(sfile))
                {
                    r = File.ReadAllText(sfile);
                }
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            return r;
        }

        public DataTable fileToDT(string file)
        {
            return sqlToDT(getSQL(file));
        }
        
        public void sqlExecFile(string file)
        {
            sqlExec(getSQL(file));
        }

        public Dictionary<string, string> dupe(string sql)
        {
            Dictionary<string, string> r = new Dictionary<string, string>();
            try
            {
                DataRow dr = sqlToDR(sql);
                if (dr != null)
                {
                    List<string> lRemoveCol = new List<string>();
                    foreach (DataColumn col in dr.Table.Columns)
                    {
                        if (col.ColumnName.StartsWith("idx") == false)
                        {
                            if ((col.DataType == typeof(Int32)) ||
                                (col.DataType == typeof(double)) ||
                                (col.DataType == typeof(float)))
                            {
                                string s = dr[col].ToString();
                                double d;
                                if (double.TryParse(s, out d))
                                {
                                    r.Add(col.ColumnName, "#" + s);
                                }
                                else
                                {
                                    lRemoveCol.Add(col.ColumnName);
                                }
                            }

                            else if ((col.DataType == typeof(MySql.Data.Types.MySqlDateTime)))
                            {
                                string s = dr[col].ToString();
                                DateTime dtt;
                                if (DateTime.TryParse(s, out dtt))
                                {
                                    r.Add(col.ColumnName, s);
                                }
                                else
                                {
                                    lRemoveCol.Add(col.ColumnName);
                                }
                            }

                            else
                            {
                                r.Add(col.ColumnName, dr[col].ToString());
                            }
                        }
                    }
                    foreach(string col in lRemoveCol)
                    {
                        r.Remove(col);
                    }
                }
            }
            catch (Exception E)
            {
                cs.logError(E);
            }


            return r;
        }
        public String getSetting(string AKind, String ATag, String ADefault = "")
        {
            string r = ADefault;
            try
            {
                r = sqlToText("select 값 from _setting where 종류 = '" + AKind + "' and 항목 = '" + ATag + "'");
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            return r;
            //return .GetValue("CS_Param", "cs_param", AID, "cs_param_value", ADefault);
        }

        public void setSetting(string AKind, String ATag, String AValue, Boolean AInsert = false)
        {
            try
            {
                string sql = "";
                if (sqlToInt("select count(*) from _setting where 종류 = '" + AKind + "' and 항목 = '" + ATag + "'") == 0)
                {
                    sql = "insert into _setting (종류, 항목, 값) values ('" + AKind + "', '" + ATag + "', '" + AValue + "')";
                }
                else
                {
                    sql = "update _setting set 값 = '" + AValue + "' where 종류 = '" + AKind + "' and 항목 = '" + ATag + "'";
                }
                sqlExec(sql);
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            //Program.FormMainRef.FormDBManager.SetValue("CS_Param", "cs_param", AID, "cs_param_value", AValue);
        }

        public String DBParamGet(String ATag, String ADefault = "")
        {
            string r = ADefault;
            try
            {
                r = sqlToText("select value from _param where tag = '" + ATag + "'");
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            return r;
            //return .GetValue("CS_Param", "cs_param", AID, "cs_param_value", ADefault);
        }

        public void DBParamSet(String ATag, String AValue, Boolean AInsert = false, Boolean isQueue=false)
        {
            try
            {
                string sql = "";
                if (sqlToInt("select count(*) from _param where tag = '" + ATag + "'") == 0)
                {
                    sql = "insert into _param (tag, value) values ('" + ATag + "', '" + AValue + "')";
                }
                else
                {
                    sql = "update _param set value = '" + AValue + "' where tag = '" + ATag + "' ";
                }
                if (isQueue)
                {
                    sqlPush(sql);
                }
                else
                {
                    sqlExec(sql);
                }
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            //Program.FormMainRef.FormDBManager.SetValue("CS_Param", "cs_param", AID, "cs_param_value", AValue);
        }

        public static string paramGet(string ATag, string ADefault = "")
        {
            string r = ADefault;
            try
            {
                DMDB db = new DMDB();
                r = db.sqlToText("select value from _param where tag = '" + ATag + "'");
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            return r;
            //return .GetValue("CS_Param", "cs_param", AID, "cs_param_value", ADefault);
        }

        public static void paramSet(string ATag, string AValue, Boolean AInsert = false)
        {
            try
            {
                DMDB db = new DMDB();
                string sql = "";
                if (db.sqlToInt("select count(*) from _param where tag = '" + ATag + "'") == 0)
                {
                    sql = "insert into _param (tag, value) values ('" + ATag + "', '" + AValue + "')";
                }
                else
                {
                    sql = "update _param set value = '" + AValue + "' where tag = '" + ATag + "' ";
                }
                db.sqlExec(sql);
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            //Program.FormMainRef.FormDBManager.SetValue("CS_Param", "cs_param", AID, "cs_param_value", AValue);
        }

        public string checkDupe(string table, Dictionary<string, string> dic)
        {
            string r = "";

            try
            {
                string sql = "select count(*) from " + table + " ";

                List<string> lw = new List<string>();

                DataTable dt = sqlToDT("select * from _table_dupe_field where 테이블 = '" + table + "' and 포함 = 1 ");
                foreach (DataRow dr in dt.Rows)
                {
                    string field = dr["필드"].ToString();

                    if (dic.ContainsKey(field) == false)
                    {
                        r += "내용없음(" + field + ") ";
                    }
                    else
                    {
                        string v = dic[field];
                        if (v.StartsWith("#"))
                        {
                            v = v.Replace("#", "");
                        }
                        else
                        {
                            v = "'" + v + "'";
                        }
                        lw.Add("`" + field + "` = " + v);
                    }
                }

                if (r == "")
                {
                    sql = sql + " where " + string.Join(" AND ", lw);
                    int i = sqlToInt(sql);

                    if (i > 1)
                    {
                        r = "true";
                    }
                }
                else
                {

                }
            }
            catch (Exception E)
            {
                cs.logError(E);
            }

            return r;
        }

        public static void initGrid(DataGridView agv, string title = "", Form fParent = null)
        {
            DMDB db = new DMDB();

            if (title == "")
            {
                try
                {
                    title = agv.Parent.Name + "." + agv.Name;
                }
                catch (Exception E)
                {
                    cs.logError(E);
                }
            }

            agv.Tag = title;

            for (int i = 0; i < agv.Columns.Count; i++)
            {
                string key = title + "." + i.ToString() + ".width";
                int w = 100;

                // 1) 기존 버전
                string sw = db.DBParamGet(key, "100");

                if (Int32.TryParse(sw, out w))
                {
                    if (fParent != null)
                    {
                        fParent.Invoke(new Action(delegate () // this == Form 이다. Form이 아닌 컨트롤의 Invoke를 직접호출해도 무방하다.
                        {
                            agv.Columns[i].Width = w;
                        }));
                    }
                    else
                    {
                        agv.Columns[i].Width = w;
                    }
                }
            }

            if (fParent != null)
            {
                fParent.Invoke(new Action(delegate () // this == Form 이다. Form이 아닌 컨트롤의 Invoke를 직접호출해도 무방하다.
                {
                    agv.ColumnWidthChanged += gv_ColumnWidthChanged;
                }));
            }
            else
            {
                agv.ColumnWidthChanged += gv_ColumnWidthChanged;
            }
        }
        private static void gv_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            try
            {
                DataGridView gv = (DataGridView)sender;

                string title = (string)gv.Tag;

                string key = title + "." + e.Column.Index.ToString() + ".width";

                new DMDB().DBParamSet(key, e.Column.Width.ToString(), false, true);
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
        }

        public static Dictionary<DataGridView, DataTable> dicGridData = new Dictionary<DataGridView, DataTable>();

        public static void reloadGridData(DataGridView agv, DataTable dt)
        {
            try
            {
                if (dicGridData.ContainsKey(agv) == false)
                    dicGridData.Add(agv, dt);
                else
                    dicGridData[agv] = dt;
                //agv.DataSource = dt;
                agv.RowCount = dt.Rows.Count;
                agv.Invalidate();

                agv.CellValueNeeded -= gv_CellValueNeeded;
                agv.CellValueNeeded += gv_CellValueNeeded;
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
        }

        private static void gv_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            try
            {
                DataGridView gv = (DataGridView)sender;
                DataTable dt = null;
                if (dicGridData.TryGetValue(gv, out dt))
                {
                    int r = e.RowIndex;
                    int c = e.ColumnIndex;

                    string col = gv.Columns[c].ToolTipText;
                    if (col == "")
                        col = gv.Columns[c].HeaderText;

                    e.Value = dt.Rows[r][col].ToString();
                }
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
        }

        // 33333333333333333333333
        // 업체 특화
        public void doKP_IRCalc(string odcode)
        {
            string sql = "update 견적내용_목록 set ";
            sql += "  개별회배 = 폭 * 길이 / 1000000 ";
            sql += ", 총회배 = (폭 * 길이 / 1000000) * 수량 ";
            sql += ", 재료비 = (폭 * 길이 / 1000000) * 수량 * 기준단가 ";
            sql += ", 개별가공비 = (가공비 + 절단비) * 수량 ";
            sql += ", 공급가 = ((폭 * 길이 / 1000000) * 수량 * 기준단가) + ((가공비 + 절단비) * 수량) ";
            sql += ", 가공비절단비 = (가공비 + 절단비) ";
            sql += " where 수주번호 = '" + odcode + "' ";
            sqlExec(sql);

            sql = "update 견적내용_목록 set ";
            sql += "  개별회배 = 폭 * 길이 / 1000000 ";
            sql += ", 총회배 = (폭 * 길이 / 1000000) * 수량 ";
            sql += ", 재료비 = (폭 * 길이 / 1000000) * 수량 * 기준단가 ";
            sql += ", 개별가공비 = (가공비 + 절단비) * 수량 ";
            sql += ", 공급가 = ((폭 * 길이 / 1000000) * 수량 * 기준단가) + ((가공비 + 절단비) * 수량) ";
            sql += ", 가공비절단비 = (가공비 + 절단비) ";
            sql += " where 수주번호 = '" + odcode + "' and 계산구분 = '회배' ";
            sqlExec(sql);

            sql = "update 견적내용_목록 set ";
            sql += "  개별회배 = 폭 * 길이 / 1000000 ";
            sql += ", 총회배 = (폭 * 길이 / 1000000) * 수량 ";
            sql += ", 재료비 = 수량 * 기준단가 ";
            sql += ", 개별가공비 = (가공비 + 절단비) * 수량 ";
            sql += ", 공급가 = (수량 * 기준단가) + ((가공비 + 절단비) * 수량) ";
            sql += ", 가공비절단비 = (가공비 + 절단비) ";
            sql += " where 수주번호 = '" + odcode + "' and 계산구분 = '장' ";
            sqlExec(sql);
        }

        public void doKP_IRCalc(DataRow dr)
        {
            try
            {
                string idx = dr["idx"].ToString();

                string sql = "update 견적내용_목록 set ";
                sql += "  개별회배 = 폭 * 길이 / 1000000 ";
                sql += ", 총회배 = (폭 * 길이 / 1000000) * 수량 ";
                sql += ", 재료비 = (폭 * 길이 / 1000000) * 수량 * 기준단가 ";
                sql += ", 개별가공비 = (가공비 + 절단비) * 수량 ";
                sql += ", 공급가 = ((폭 * 길이 / 1000000) * 수량 * 기준단가) + ((가공비 + 절단비) * 수량) ";

                sql += ", 가공비절단비 = (가공비 + 절단비) ";

                sql += " where idx = " + idx + " ";
                sqlExec(sql);

                sql = "update 견적내용_목록 set ";
                sql += "  개별회배 = 폭 * 길이 / 1000000 ";
                sql += ", 총회배 = (폭 * 길이 / 1000000) * 수량 ";
                sql += ", 재료비 = (폭 * 길이 / 1000000) * 수량 * 기준단가 ";
                sql += ", 개별가공비 = (가공비 + 절단비) * 수량 ";
                sql += ", 공급가 = ((폭 * 길이 / 1000000) * 수량 * 기준단가) + ((가공비 + 절단비) * 수량) ";

                sql += ", 가공비절단비 = (가공비 + 절단비) ";

                sql += " where idx = " + idx + " and 계산구분 = '회배' ";
                sqlExec(sql);

                sql = "update 견적내용_목록 set ";
                sql += "  개별회배 = 폭 * 길이 / 1000000 ";
                sql += ", 총회배 = (폭 * 길이 / 1000000) * 수량 ";
                sql += ", 재료비 = 수량 * 기준단가 ";
                sql += ", 개별가공비 = (가공비 + 절단비) * 수량 ";
                sql += ", 공급가 = (수량 * 기준단가) + ((가공비 + 절단비) * 수량) ";

                sql += ", 가공비절단비 = (가공비 + 절단비) ";

                sql += " where idx = " + idx + " and 계산구분 = '장' ";
                sqlExec(sql);

                DataRow drNew = sqlToDR("select * from 견적내용_목록 where idx = " + idx);

                dr["개별회배"] = drNew["개별회배"].ToString();
                dr["총회배"] = drNew["총회배"].ToString();
                dr["재료비"] = drNew["재료비"].ToString();
                dr["개별가공비"] = drNew["개별가공비"].ToString();
                dr["공급가"] = drNew["공급가"].ToString();

                dr.AcceptChanges();

                //double dd1 = cs.strToDouble(dr["가공비"].ToString());
                //double dd2 = cs.strToDouble(dr["절단비"].ToString());

                //double dw = cs.strToDouble(dr["폭"].ToString());
                //double dh = cs.strToDouble(dr["길이"].ToString());
                //double dq = cs.strToDouble(dr["수량"].ToString());
                //double dp = cs.strToDouble(dr["기준단가"].ToString());

                //double d1 = (dw * dh / 1000000);

                //dr["개별회배"] = d1.ToString();
                //dr["총회배"] = (d1 * dq).ToString();
                //dr["재료비"] = (d1 * dp * dq).ToString();
                //dr["개별가공비"] = ((dd1 + dd2) * dq).ToString();

                //dr["공급가"] = cs.strToDouble(dr["재료비"].ToString()) + cs.strToDouble(dr["개별가공비"].ToString());

                ////dr.Table.AcceptChanges();

                //Dictionary<string, string> dic = new Dictionary<string, string>();
                //dic.Add("가공비", "#" + dr["가공비"].ToString());
                //dic.Add("절단비", "#" + dr["절단비"].ToString());
                //dic.Add("개별회배", "#" + dr["개별회배"].ToString());
                //dic.Add("총회배", "#" + dr["총회배"].ToString());
                //dic.Add("재료비", "#" + dr["재료비"].ToString());
                //dic.Add("개별가공비", "#" + dr["개별가공비"].ToString());
                //dic.Add("공급가", "#" + dr["공급가"].ToString());

                //updateDic("견적내용_목록", dic, "idx = " + dr["idx"].ToString());
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
        }

        public string getKP_IRCode(string cpname) //1=A,B,C 2=1,2,3 3=01,02,03 
        {
            string newod = "";
            try
            {
                // IR20220413_44A101
                string sql = "select * from 견적_목록 where 수주번호 like 'IR"
                    + DateTime.Now.ToString("yyyyMMdd") + "_%' "
                    + " and 업체명 = '" + cpname + "' "
                    + " order by 수주번호 desc";

                DataTable dt = sqlToDT(sql);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        string odlast = dt.Rows[0]["수주번호"].ToString();
                        var chars = odlast.ToCharArray();

                        char cc = chars[13];

                        //while (true)
                        {
                            cc++;
                            chars[13] = cc;
                            newod = new string(chars);

                            //if (sqlToInt("select coutn(*) 견적_목록 where substring(수주번호, 1, 14) = '" + newod + ))
                        }
                    }
                    else
                    {
                        // 44에 해당하는 해당 날짜의 번호는 항상 증가하여야 한다.
                        string sqlmax = "select SUBSTR(수주번호, 12, 2) from 견적_목록 where 수주번호 like 'IR"
                            + DateTime.Now.ToString("yyyyMMdd") + "_%' order by 수주번호 desc limit 1";

                        int nomax = sqlToInt(sqlmax);

                        string snomax = String.Format("{0:00}", nomax);
                        nomax++; 
                        snomax = String.Format("{0:00}", nomax);

                        newod = "IR" + DateTime.Now.ToString("yyyyMMdd") + "_"+ snomax + "A101";
                    }
                }
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            return newod;
        }

        public string getKP_IRCode2(string odcode) //1=A,B,C 2=1,2,3 3=01,02,03 
        {
            string newod = "";
            try
            {
                string odlast = odcode;
                var chars = odlast.ToCharArray();

                char cc = chars[14];
                cc++;

                chars[14] = cc;

                chars[15] = '0';
                chars[16] = '1';

                newod = new string(chars);                
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            return newod;
        }

        public string getKP_IRCode3(string odcode) //1=A,B,C 2=1,2,3 3=01,02,03 
        {
            string newod = "";
            try
            {
                string odlast = odcode;
                var chars = odlast.ToCharArray();

                char cc = chars[16];
                cc++;

                chars[16] = cc;

                newod = new string(chars);
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            return newod;
        }
    }
}

// 22222222222222222222222222222
#elif (DMDBMSSQL)

using System;
using System.Drawing;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Data.SqlClient;
using System.Net;
using System.Net.Sockets;
using cubemeslight.cubemes;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using System.IO;

namespace cubemeslight.cubemes
{
    public class DMDB
    {
        public string db;
        private string server;
        private string port;
        private string uid;
        private string password;

        public SqlConnection connection;
        public DataGridView datagv;
        private SqlDataAdapter SqlDataAdapter;
        static public string loginname = "";
        ContextMenuStrip mchstr = new ContextMenuStrip();
        int selectcolumnh;
        DataGridView selectdgv;

        public string driver = "mssql";

        public DMDB(string dbname = "" )
        {
            db = "ns_erp";

            if (cs._APP == "")
            {
                cs._APP = db;
            }

            connectinit("ns_erp");
        }

        public void connectinit(string database)
        {
            server = cs.LocalParamGet("sqldbserverip", "doc.cns2.com");
            port = cs.LocalParamGet("sqldbserverport", "14333");
            //database = "testmes";
            uid = cs.LocalParamGet("sqldbserverid", "ns_mes");
            password = cs.LocalParamGet("sqldbserverpw", "nserp@7056#");

            string connectionString;
            connectionString = "SERVER=" + server + "," + port + ";" + "DATABASE=" + database + ";" + "UID=" + uid + ";" + "pwd=" + password;// + ";Charset=utf8;";
             
            cs.logmsg("Try Connect : " + connectionString);
            connection = new SqlConnection(connectionString);
            //connection.ConnectionTimeout = 2000;
            try
            {
                connection.Open();
                cs.logmsg("OK : " + connectionString);
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
        }


        public string sqlExec(string sql)
        {
            string rr = "";
            Boolean r = false;

            Boolean isNeedMsg = false;

            if (driver == "mssql")
            {
                sql = sqlCheck(sql);
            }

            cs.logSQL(sql);
            try
            {
                if (sql.Contains("delete from _table_setting") ||
                    sql.Contains("update _table_setting") ||
                    sql.Contains("delete from _table_link") ||
                    sql.Contains("update _table_link")
                    )
                {
                    isNeedMsg = true;
                    cs.LogDebug("[_table_setting] " + sql);
                }
            }
            catch (Exception E)
            {
                cs.logError(E);
            }

            lock (connection)
            {
                try
                {
                    SqlCommand myCommand = new SqlCommand(sql, connection);
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    //myCommand.ExecuteNonQuery();

                    var obj = myCommand.ExecuteScalar();

                    r = true;
                    if (obj != null)
                    {
                        rr = ((int)obj).ToString();
                    }
                }
                catch (Exception E)
                {
                    cs.logError(E);
                    cs.LogErrorMsg(sql);
                    if (isNeedMsg)
                    {
                        isNeedMsg = true;
                        cs.LogDebug("[_table_setting] ERROR");
                        cs.LogDebug("[_table_setting] " + E.Message);
                        cs.LogDebug("[_table_setting] " + E.StackTrace);
                        cs.LogDebug("[_table_setting] " + sql);

                        MessageBox.Show("[관리자에게 문의 하세요]\r\n" + E.Message);
                    }
                }
            }
            return rr;
        }
        public Boolean sqlExec(string sql, out string result)
        {
            if (driver == "mssql")
            {
                sql = sqlCheck(sql);
            }

            result = "";

            if (sql.Contains("\n"))
            {
                sql = sql.Replace("\n", "");
            }
            if (sql.Contains("\r"))
            {
                sql = sql.Replace("\r", "");
            }
            if (sql.Contains("\""))
            {
                sql = sql.Replace("\"", "/");
            }

            Boolean r = false;
            cs.logSQL(sql);
            lock (connection)
            {
                try
                {
                    SqlCommand myCommand = new SqlCommand(sql, connection);
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    myCommand.ExecuteNonQuery();
                    r = true;
                }
                catch (Exception E)
                {
                    result = E.Message;
                    cs.logError(E);
                    cs.LogErrorMsg(sql);
                }
            }
            return r;
        }

        public DataTable sqlToDT(string sql)
        {
            if (driver == "mssql")
            {
                sql = sqlCheck(sql);
            }

            cs.logSQL(sql);
            DataSet DS;
            lock (connection)
            {
                SqlDataAdapter = new SqlDataAdapter(sql, connection);
                DS = new DataSet();
                SqlDataAdapter.Fill(DS);
            }
            return DS.Tables[0];
        }

        public double sqlToDouble(string sql)
        {
            cs.logSQL(sql);
            double i = -1;
            string rr = "";
            lock (connection)
            {
                try
                {
                    SqlDataAdapter = new SqlDataAdapter(sql, connection);
                    DataSet DS = new DataSet();
                    SqlDataAdapter.Fill(DS);

                    DataRow dr = DS.Tables[0].Rows[0];
                    rr = dr[0].ToString();

                    double.TryParse(rr, out i);
                }
                catch
                {

                }
            }
            return i;
        }

        public string sqlToText(string sql)
        {
            if (driver == "mssql")
            {
                sql = sqlCheck(sql);
            }

            string rr = "";
            lock (connection)
            {
                try
                {
                    SqlDataAdapter = new SqlDataAdapter(sql, connection);
                    DataSet DS = new DataSet();
                    SqlDataAdapter.Fill(DS);

                    DataRow dr = DS.Tables[0].Rows[0];
                    rr = dr[0].ToString(); ;
                }
                catch
                {

                }
            }
            return rr;
        }

        public int sqlToInt(string sql)
        {
            if (driver == "mssql")
            {
                sql = sqlCheck(sql);
            }

            cs.logSQL(sql);
            int i = -1;
            string rr = "";
            lock (connection)
            {
                try
                {
                    SqlDataAdapter = new SqlDataAdapter(sql, connection);
                    DataSet DS = new DataSet();
                    SqlDataAdapter.Fill(DS);

                    DataRow dr = DS.Tables[0].Rows[0];
                    rr = dr[0].ToString();

                    Int32.TryParse(rr, out i);
                }
                catch
                {

                }
            }
            return i;
        }

        public long sqlToLong(string sql)
        {
            if (driver == "mssql")
            {
                sql = sql.ToUpper().Replace("`", "").Replace("TABLE_SCHEMA", "TABLE_CATALOG");
            }
            long i = 0;
            string rr = "";
            lock (connection)
            {
                try
                {
                    SqlDataAdapter = new SqlDataAdapter(sql, connection);
                    DataSet DS = new DataSet();
                    SqlDataAdapter.Fill(DS);

                    DataRow dr = DS.Tables[0].Rows[0];
                    rr = dr[0].ToString();

                    long.TryParse(rr, out i);
                }
                catch
                {

                }
            }
            return i;
        }

        public DataRow sqlToDR(string sql)
        {
            if (driver == "mssql")
            {
                sql = sqlCheck(sql);
            }

            DataRow rr = null;
            lock (connection)
            {
                try
                {
                    SqlDataAdapter = new SqlDataAdapter(sql, connection);
                    DataSet DS = new DataSet();
                    SqlDataAdapter.Fill(DS);

                    if (DS.Tables[0].Rows.Count>0)
                        rr = DS.Tables[0].Rows[0];
                }
                catch (Exception E)
                {
                    cs.logError(E);
                }
            }
            return rr;
        }

        public void sqlExecFile(string file)
        {
            sqlExec(getSQL(file));
        }

        public List<string> getFieldList(string table, Boolean AddWrap=false)
        {
            List<string> ll = new List<string>();
            try
            {
                string sql = "SELECT tbl.column_name as [필드명] FROM [information_schema].[columns] AS tbl ";
                sql += "where  [table_schema] = '" + db + "' and [table_name] = '" + table + "' ";

                DataTable dt = sqlToDT(sql);
                foreach(DataRow dr in dt.Rows)
                {
                    if (AddWrap)
                    ll.Add("["+dr["필드명"].ToString()+"]");
                    else
                        ll.Add(dr["필드명"].ToString());
                }
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            return ll;
        }

        public Dictionary<string, DataRow> getFieldInfo(string table)
        {
            Dictionary<string, DataRow> r = new Dictionary<string, DataRow>();
            try
            {
                string sql = "SELECT * FROM [information_schema].[columns] AS tbl ";
                sql += "where  [table_schema] = '" + db + "' and [table_name] = '" + table + "' ";

                DataTable dt = sqlToDT(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    r.Add(dr["column_name"].ToString(), dr);
                }
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            return r;
        }

        public Dictionary<string, DataRow> getTableSetting(string table)
        {
            Dictionary<string, DataRow> r = new Dictionary<string, DataRow>();
            try
            {
                string sql = "SELECT * from _table_setting where 테이블 = '" + table + "' ";

                DataTable dt = sqlToDT(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    r.Add(dr["필드"].ToString(), dr);
                }
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            return r;
        }

        public DataTable getTableInfo(string table)
        {
            DataTable dt=null;
            try
            {
                string sql = "SELECT * FROM [information_schema].[columns] AS tbl ";
                sql += "where  [table_schema] = '" + db + "' and [table_name] = '" + table + "' ";

                dt = sqlToDT(sql);
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            return dt;
        }

        public int login(string id, string pw)
        {
            int myCount = 0;

            if(this.OpenConnection() == true)
            {
                string data = "select count(*)  from emp_list where id = '" + id + "' and pw = '" + pw + "'";
                SqlCommand count = new SqlCommand(data, connection);
                myCount = Convert.ToInt32(count.ExecuteScalar());

                if(myCount != 0)
                {
                    SqlCommand cmd = new SqlCommand("select emp_name from emp_list where emp_id = '" + id + "' and emp_pw = '" + pw + "'", connection);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        //Console.WriteLine("{0}: {1}", rdr["Id"], rdr["Name"]);
                        loginname = rdr["emp_name"].ToString();
                    }
                    rdr.Close();
                   }

                //close connection
                this.CloseConnection();
            }
            return myCount;
        }

        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (SqlException ex)
            {
                //When handling errors, you can your application's response based on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("Cannot connect to server. Contact administrator");
                        break;
                    case 1045:
                        MessageBox.Show("Invalid username/password, please try again");
                        break;
                    default:
                        MessageBox.Show(ex.Message);
                        break;
                }
                return false;
            }
        }

        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private void datagv_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            DataTable changes = ((DataTable)datagv.DataSource).GetChanges();

            if (changes != null)
            {
                SqlCommandBuilder mcb = new SqlCommandBuilder(SqlDataAdapter);
                SqlDataAdapter.UpdateCommand = mcb.GetUpdateCommand();
                SqlDataAdapter.Update(changes);
                ((DataTable)datagv.DataSource).AcceptChanges();
            }

        }

        public List<string> getTableList()
        {
            List<string> l = new List<string>();
            try
            {
                string s = "SHOW TABLES FROM " + db;

                DataTable dt = sqlToDT(s);
                foreach (DataRow dr in dt.Rows)
                {
                    l.Add(dr[0].ToString());
                }
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            return l;
        }

        public Boolean doIn(DateTime dtAdd, string kind, string code, double qty, string whfrom = "", string whto="", string emp="", string lot="", string wpno = "", Dictionary<string, string> dicExtColValue = null)
        {
            Boolean r = false;
            try
            {
                if (emp == "")
                    emp = cs.empid;

                List<string> lCol = new List<string>();
                lCol.Add("종류");
                lCol.Add("품번");
                lCol.Add("ioh_qty"); // 수량
                lCol.Add("기존창고");
                lCol.Add("이동창고");
                lCol.Add("등록자");
                lCol.Add("LOT");
                lCol.Add("연결번호");
                lCol.Add("수불일");

                List<string> lValue = new List<string>();
                lValue.Add("'" + kind + "' ");
                lValue.Add("'" + code + "' ");
                lValue.Add("" + qty.ToString()+ " ");
                lValue.Add("'" + whfrom + "' ");
                lValue.Add("'" + whto + "' ");
                lValue.Add("'" + cs.empid + "' ");
                lValue.Add("'" + lot + "' ");
                lValue.Add("'" + wpno + "' ");
                lValue.Add("'" + dtAdd.ToString("yyyy-MM-dd HH:mm:ss") + "' ");

                if (dicExtColValue != null)
                {
                    foreach(KeyValuePair<string, string> kv in dicExtColValue)
                    {
                        lCol.Add(kv.Key);
                        lValue.Add(kv.Value);
                    }
                }

                string sql = "insert into item_inout_history (";

                sql += string.Join(",", lCol) + ") values (";

                sql += string.Join(",", lValue) + ")";

                sqlExec(sql);
                r = true;
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            return r;
        }

        public void gridSettingLoad(DataTable dtGrid, GridView g, string table = "")
        {
            if (table != "")
            {
                try
                {
                    DataTable dt = sqlToDT("select * from _table_setting where 테이블 = '" + table + "' ");
                    
                    foreach (DataRow dr in dt.Rows)
                    {
                        string f = dr["필드"].ToString();
                        GridColumn c = g.Columns.ColumnByFieldName(f);

                        if (c != null)
                        {
                            if (c.FieldName == "idx")
                            {
                                c.Caption = "코드";
                                dtGrid.Columns[c.FieldName].Caption = "코드";
                            }

                            c.Visible = dr["표시"].ToString() == "1";

                            string col = dr["명칭"].ToString();
                            if (col != "")
                            {
                                c.Caption = col;
                                dtGrid.Columns[c.FieldName].Caption = col;
                            }
                        }
                        else
                        {

                        }
                    }

                    //g.Appearance.OddRow.BackColor = System.Drawing.ColorTranslator.FromHtml("#F2F2F2");
                    //g.OptionsView.EnableAppearanceOddRow = true;
					
					g.Appearance.HorzLine.BackColor = ColorTranslator.FromHtml("#ecf0f3");
                    g.Appearance.VertLine.BackColor = ColorTranslator.FromHtml("#ecf0f3");

                    g.Appearance.Row.Font = new Font("맑은 고딕", 9, FontStyle.Regular);

                    g.ColumnPanelRowHeight = 32;
                    g.RowHeight = 32;

                    //dtGrid.Columns[0].Caption = "코드";

                    //g.OptionsSelection.MultiSelect = true;
                    //g.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
                    //g.OptionsSelection.CheckBoxSelectorColumnWidth = 40;
                    //if (db == "hb")
                    {
                        try
                        {
                            DataTable dtcol = sqlToDT("select * from _param where tag like '%.width' and tag like '" + table + "%'");
                            if (dtcol != null)
                            {
                                foreach (DataRow dr in dtcol.Rows)
                                {
                                    string tag = dr["tag"].ToString();
                                    List<string> ll = tag.Split('.').ToList();
                                    if (ll.Count == 3)
                                    {
                                        string stable = ll[0];
                                        string scol = ll[1];
                                        string swidth = ll[2];
                                        string sv = dr["value"].ToString();
                                        int iw = 0;
                                        if (Int32.TryParse(sv, out iw))
                                        {
                                            if (g.Columns.ColumnByFieldName(scol)!=null)
                                            {
                                                g.Columns[scol].Width = iw;
                                            }
                                            // dtGrid.Columns[scol]
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception E)
                        {
                            cs.logError(E);
                        }
                    }
                }
                catch (Exception E)
                {
                    cs.logError(E);
                }
            }
            else
            {
                foreach(GridColumn col in g.Columns)
                {
                    if (col.FieldName.StartsWith("_"))
                    {
                        col.Visible = false;
                    }
                }
            }


            //try
            //{
            //    // UI 변경
            //    g.Appearance.HeaderPanel.Font = new Font("맑은 고딕", 9, FontStyle.Bold);
            //    g.Appearance.HeaderPanel.BackColor = Color.Transparent;

            //    g.Appearance.Row.Font = new Font("맑은 고딕", 9);
            //    g.Appearance.Row.ForeColor = Color.FromArgb(255, 64, 64, 64);
            //    //g.Appearance.Row.BorderColor = Color.White;

            //    g.Appearance.HorzLine.ForeColor = Color.White;// Color.FromArgb(255, 20, 20, 20);
            //    //g.Appearance.VertLine.ForeColor = Color.FromArgb(255, 20, 20, 20);

            //    g.OptionsView.ShowIndicator = false;
            //    //g.Appearance.VertLine.BackColor 
            //    g.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            //    g.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
                
            //    g.PaintStyleName = "Web";

            //}
            //catch (Exception E)
            //{
            //    cs.logError(E);
            //}
        }

        public string makeCPTax(string org)
        {
            string r = "";
            try
            {
                r = org.Replace("-", "").Replace(" ", "");
                long l = 0;
                if (long.TryParse(r, out l))
                {
                    r = l.ToString();
                    r = r.Substring(0, 3) + "-" + r.Substring(3, 2) + "-" + r.Substring(5, 5);
                }
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            return r;
        }
        public string makeKeyCode(string atable)
        {
            int i = getAutoIdx(atable);
            return (i + 1).ToString("000000");
        }

        public string makeIdxCode(string atable)
        {
            int i = getAutoIdx(atable);
            return "CD" + (i + 1).ToString("000000");
        }


        public string insertDic(string table, Dictionary<string,string> dic)
        {
            string r = "";
            try
            {
                string sql = "insert into " + table;

                List<string> lCol = new List<string>();
                List<string> lValue = new List<string>();

                foreach (KeyValuePair<string, string> kv in dic)
                {
                    if (kv.Key == "#ROW#")
                    {

                    }
                    else
                    {
                        lCol.Add("[" + kv.Key + "]");

                        if (kv.Value.Contains("#") == false)
                        {
                            lValue.Add("'" + kv.Value + "'");
                        }
                        else
                        {
                            lValue.Add(kv.Value.Replace("#", ""));
                        }
                    }
                }
                sql += ("(" + string.Join(",", lCol) + ")");
                sql += (" values (" + string.Join(",", lValue) + ");");

                cs.logSQL(sql);
                lock (connection)
                {
                    SqlCommand myCommand = new SqlCommand(sql, connection);
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    //int ir = 0;
                    object rr = myCommand.ExecuteScalar();

                    SqlDataAdapter = new SqlDataAdapter("SELECT @@IDENTITY;", connection);
                    DataSet DS = new DataSet();
                    SqlDataAdapter.Fill(DS);

                    DataRow dr = DS.Tables[0].Rows[0];
                    r = dr[0].ToString(); 
                }
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            return r;
        }

        public void updateDic(string table, Dictionary<string, string> dic, string where)
        {
            try
            {
                string sql = "update " + table + " set ";

                List<string> lCol = new List<string>();
                List<string> lValue = new List<string>();

                foreach (KeyValuePair<string, string> kv in dic)
                {
                    string sv = "";

                    if (kv.Value.Contains("#") == false)
                    {
                        sv = "'" + kv.Value + "'";
                    }
                    else
                    {
                        sv = kv.Value.Replace("#", "");
                    }

                    lCol.Add(kv.Key + " = " + sv);
                }
                sql += string.Join(",", lCol);
                sql += " where " + where;

                sqlExec(sql);
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
        }

        public int getAutoIdx(string table)
        {
            try
            {
                DataTable dt = sqlToDT("show table status where name = '" + table + "'");
                if (dt.Rows.Count > 0)
                {
                    string s = dt.Rows[0]["auto_increment"].ToString();
                    int i = 0;
                    Int32.TryParse(s, out i);
                    return i;
                }
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            return 0;
        }

        public string makeCPCode()
        {
            int i = getAutoIdx("업체_목록");
            return "CP" + (i + 1).ToString("000000");
        }
        public string makeODCode()
        {
            int i = getAutoIdx("order_list_kj");
            return "OD" + DateTime.Now.ToString("yyyyMM")+ (i + 1).ToString("000000");
        }
        public string makeRCCode()
        {
            int i = getAutoIdx("거래관리_내용_목록");
            return "RC" + DateTime.Now.ToString("yyyyMM") + (i + 1).ToString("000000");
        }

        // GN
        public double GNGetItemPriceViaMonth(string itemcode, int month = 0)
        {
            double r = 0;

            if (month == 0)
                month = DateTime.Now.Month;

            r = sqlToInt("select " + month.ToString() + " from item_list_gn_product where 제품code = '" + itemcode + "' ");

            return r;
        }

        public void tableToCombo(string table, string col, System.Windows.Forms.ComboBox combo)
        {
            try
            {
                combo.Items.Clear();
                DataTable dt = sqlToDT("select " + col + " from " + table + " group by " + col + " order by " + col);
                foreach (DataRow dr in dt.Rows)
                {
                    string v = dr[col].ToString();
                    
                    combo.Items.Add(v);
                }
                combo.Items.Insert(0, "");
                if (combo.Items.Count > 0)
                    combo.SelectedIndex = 0;
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
        }

        public string getSQL(string file)
        {
            string r = "";
            try
            {
                string sfile = Application.StartupPath + "\\sql\\" + file;
                if (File.Exists(sfile))
                {
                    r = File.ReadAllText(sfile);
                }
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            return r;
        }

        public Dictionary<string, DataRow> sqlToDic(string sql, string keycol)
        {
            Dictionary<string, DataRow> dic = new Dictionary<string, DataRow>();
            try
            {
                DataTable dt = sqlToDT(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    string key = dr[keycol].ToString();
                    if (dic.ContainsKey(key) == false)
                    {
                        dic.Add(key, dr);
                    }
                }
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            return dic;
        }

        public DataTable fileToDT(string file)
        {
            return sqlToDT(getSQL(file));
        }

        public Dictionary<string,string> dupe(string sql)
        { 
            Dictionary<string, string> r = new Dictionary<string, string>();
            try
            {
                DataRow dr = sqlToDR(sql);
                if (dr != null)
                {
                    foreach(DataColumn col in dr.Table.Columns)
                    {
                        if (col.ColumnName.StartsWith("idx") == false)
                        {
                            if ((col.DataType == typeof(Int32)) ||
                                (col.DataType == typeof(float)))
                            {
                                string s = dr[col].ToString();
                                if (s == "")
                                    s = "0";
                                r.Add(col.ColumnName, "#" + s);
                            }
                            else
                            {
                                r.Add(col.ColumnName, dr[col].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            

            return r;
        }

        public string checkDupe(string atable, Dictionary<string, string> dic)
        {
            return "";
        }

        public string sqlCheck(string sql)
        {
            sql = sql.Replace("`", "");
            sql = sql.Replace("TABLE_SCHEMA", "TABLE_CATALOG");
            sql = sql.Replace("table_schema", "TABLE_CATALOG");
            return sql;
        }

        public Boolean getFieldIsString(string table, string field)
        {
            return true;
        }
        public String DBParamGet(String ATag, String ADefault = "")
        {
            string r = ADefault;
            try
            {
                r = sqlToText("select value from _param where tag = '" + ATag + "'");
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            return r;
            //return .GetValue("CS_Param", "cs_param", AID, "cs_param_value", ADefault);
        }

        public void DBParamSet(String ATag, String AValue, Boolean AInsert = false)
        {
            try
            {
                string sql = "";
                if (sqlToInt("select count(*) from _param where tag = '" + ATag + "'") == 0)
                {
                    sql = "insert into _param (tag, value) values ('" + ATag + "', '" + AValue + "')";
                }
                else
                {
                    sql = "update _param set value = '" + AValue + "' where tag = '" + ATag + "' ";
                }
                sqlExec(sql);
            }
            catch (Exception E)
            {
                cs.logError(E);
            }
            //Program.FormMainRef.FormDBManager.SetValue("CS_Param", "cs_param", AID, "cs_param_value", AValue);
        }
    }
}
#endif