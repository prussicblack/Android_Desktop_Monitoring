using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_Application
{
    public class ReceiveDataSet
    {
        public double CPU { get; set; }
        public double memory { get; set; }
        public double Network { get; set; }
        public double GPU { get; set; }

    }

    public static class ReceiveDataSetControl
    {
        public static string SystemDir = System.AppDomain.CurrentDomain.BaseDirectory;
        public static string SystemFile = SystemDir + System.IO.Path.DirectorySeparatorChar + "GlobalData.json";


        public static ReceiveDataSet LoadData()
        {
            //전체 LoadData쪽에 Try Catch 넣을것.
            //Fail이므로 메세지창 띄우고.
            try
            {

                string sJsonData = System.IO.File.ReadAllText(SystemFile);
                ReceiveDataSet sysData = JsonConvert.DeserializeObject<ReceiveDataSet>(sJsonData);

                return sysData;
            }
            catch
            {
                return GlobalData.ReceiveData;
            }
        }

        public static void SaveData(ReceiveDataSet sysData)
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
