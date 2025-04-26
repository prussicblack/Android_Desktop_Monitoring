using Android_Desktop_Monitoring;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android_Desktop_Monitoring;

namespace Android_Desktop_Monitoring
{
    public class ConfigDataSet
    {
        public int Port { get; set; } = 50051;
        public string IP { get; set; } = "192.168.86.5";
    }


    public static class ConfigDataSetControl
    {
        public static string SystemDir = System.AppDomain.CurrentDomain.BaseDirectory;
        public static string SystemFile = SystemDir + System.IO.Path.DirectorySeparatorChar + "GlobalData.json";


        public static ConfigDataSet LoadData()
        {
            //전체 LoadData쪽에 Try Catch 넣을것.
            //Fail이므로 메세지창 띄우고.
            try
            {

                string sJsonData = System.IO.File.ReadAllText(SystemFile);
                ConfigDataSet sysData = JsonConvert.DeserializeObject<ConfigDataSet>(sJsonData);

                return sysData;
            }
            catch
            {
                return GlobalData.ConfigData;
            }
        }

        public static void SaveData(ConfigDataSet sysData)
        {
            try
            {
                if (System.IO.Directory.Exists(SystemDir) == false)
                {
                    System.IO.Directory.CreateDirectory(SystemDir);
                }

                var jsonString = JsonConvert.SerializeObject(sysData, Formatting.Indented);

                System.IO.File.WriteAllText(SystemFile, jsonString);
            }

            catch (Exception e)
            {
                //LogExtension.LogException(e, $"SystemDef Save Exception", OPLOG.Folder.Log, OPLOG.File.Log);
            }
        }

    }

}
