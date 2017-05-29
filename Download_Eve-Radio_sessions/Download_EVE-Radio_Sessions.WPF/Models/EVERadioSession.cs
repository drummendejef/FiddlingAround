using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Download_EVE_Radio_Sessions.WPF.Models
{
    public class EVERadioSession
    {
        public string FileName { get; set; }
        public int Progress { get; set; }
        public string FilePath { get; set; }
    }
}
