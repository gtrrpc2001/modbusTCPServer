using lunar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deviceTest
{
    public class clsQuery
    {
        DMDB mysql;
        public clsQuery()
        {
            mysql = new DMDB();
            mysql.OpenConnection();
        }
        public DataTable GetDataTable()
        {            
            string sql = "SELECT * FROM tag_list ORDER BY IDX ASC";
            return mysql.sqlToDT(sql);
        }

        public void SetTag_ListUpdate(string group, string Tag, double value, string time)
        {
            var sql = $"UPDATE tag_list " +
                $" SET value = {value} , datatime = '{time}'" +
                $" WHERE 그룹 = '{group}' AND 태그 = '{Tag}'";
            mysql.sqlExec(sql);
        }

        public void SetDataInsert(string eq, string tag, string datatime, double value, string ext = "", int kind = 0)
        {            
            var sql = $"" +
            $"INSERT INTO raw (EQ,TAG,DATATIME,VALUE,EXT,KIND) " +
            $"VALUES('{eq}','{tag}','{datatime}',{value},'{ext}',{kind})";
            mysql.sqlExec(sql);
        }
    }
}
