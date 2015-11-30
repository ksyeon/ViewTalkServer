using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewTalkServer.Models
{
    public class ChattingData
    {
        public int ChatNumber { get; set; }

        public PPTData PPT { get; set; }

        public ChattingData(int chatNumber)
        {
            this.ChatNumber = chatNumber;
            PPT = new PPTData();
        }

        public ChattingData(int chatNumber, PPTData ppt)
        {
            this.ChatNumber = chatNumber;
            this.PPT = ppt;
        }
    }
}
