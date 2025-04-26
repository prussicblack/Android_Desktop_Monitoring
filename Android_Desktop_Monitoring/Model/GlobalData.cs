using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android_Desktop_Monitoring;

namespace Android_Desktop_Monitoring
{
    public static  class GlobalData
    {
        public static ConfigDataSet ConfigData { get; set; } = new ConfigDataSet();
        public static ReceiveDataSet ReceiveData { get; set; } = new ReceiveDataSet();

    }
}
