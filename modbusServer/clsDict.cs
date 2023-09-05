using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deviceTest
{
    public static class clsDict
    {        
        

        public static List<Dictionary<string, string>> GetDictList(DataTable dT)
        {

            var Tag_dictList = new List<Dictionary<string, string>>();
            foreach (DataRow dR in dT.Rows)
            {
                var dict = new Dictionary<string, string>();
                SetDictAdd(dict, dR);
                Tag_dictList.Add(dict);
            }
            return Tag_dictList;
        }
        private static void SetDictAdd(Dictionary<string, string> dict, DataRow dR)
        {

            dict.Add("idx", GetToString(dR["idx"]));
            dict.Add("사용", GetToString(dR["사용"]));
            dict.Add("설비", GetToString(dR["설비"]));
            dict.Add("그룹", GetToString(dR["그룹"]));
            dict.Add("태그", GetToString(dR["태그"]));
            dict.Add("addr", GetToString(dR["addr"]));
            dict.Add("comment", GetToString(dR["comment"]));
            dict.Add("value", GetToString(dR["value"]));
            dict.Add("datatime", GetToString(dR["datatime"]));
            dict.Add("vmin", GetToString(dR["vmin"]));
            dict.Add("vmax", GetToString(dR["vmax"]));
            dict.Add("vsum", GetToString(dR["vsum"]));
            dict.Add("vcnt", GetToString(dR["vcnt"]));
            dict.Add("vavg", GetToString(dR["vavg"]));
            dict.Add("scale", GetToString(dR["scale"]));
            dict.Add("offset", GetToString(dR["offset"]));
            dict.Add("alarm_hh", GetToString(dR["alarm_hh"]));
            dict.Add("alarm_h", GetToString(dR["alarm_h"]));
            dict.Add("alarm_l", GetToString(dR["alarm_l"]));
            dict.Add("alarm_ll", GetToString(dR["alarm_ll"]));
            dict.Add("alarm_result", GetToString(dR["alarm_result"]));
            dict.Add("설명", GetToString(dR["설명"]));
            dict.Add("단위", GetToString(dR["단위"]));
            dict.Add("UnitID", GetToString(dR["UnitID"]));
            dict.Add("Host", GetToString(dR["Host"]));
            dict.Add("Port", GetToString(dR["Port"]));
            dict.Add("CommSetting", GetToString(dR["CommSetting"]));
            dict.Add("CommTimeOutSec", GetToString(dR["CommTimeOutSec"]));
            dict.Add("SaveMode", GetToString(dR["SaveMode"]));
            dict.Add("SaveInterval", GetToString(dR["SaveInterval"]));
        }
        private static string GetToString(object value)
        {
            return value.ToString();
        }

        public static List<TagWithAddrModel> GetTagAddress(List<Dictionary<string,string>> Tag_dictList)
        {
            var list = new List<TagWithAddrModel>();
            foreach (var dic in Tag_dictList)
            {
                // 테스트용 -------------------------------
                //dic.TryGetValue("Host", out var host);
                //if (host != "127.0.0.1") break;
                // ----------------------------------------

                dic.TryGetValue("idx", out string idx);
                dic.TryGetValue("사용", out string used);
                dic.TryGetValue("설비", out string eq);
                dic.TryGetValue("그룹", out string group);
                dic.TryGetValue("태그", out string tag);
                dic.TryGetValue("addr", out string addr);
                dic.TryGetValue("vmin", out string vmin);
                dic.TryGetValue("vmax", out string vmax);
                dic.TryGetValue("vsum", out string vsum);
                dic.TryGetValue("vcnt", out string vcnt);
                dic.TryGetValue("vavg", out string vavg);
                dic.TryGetValue("scale", out string str_scale);
                double.TryParse(str_scale, out double scale);
                dic.TryGetValue("offset", out string offset);
                list.Add(new TagWithAddrModel(GetInt(idx), GetInt(used), eq, group, tag, GetInt(addr), GetInt(vmin), GetInt(vmax), GetInt(vsum), GetInt(vcnt), GetInt(vavg), scale, GetInt(offset)));
            }
            return list;
        }
        private static int GetInt(string value)
        {
            int num;
            int.TryParse(value, out num);
            return num;
        }
    }
}
