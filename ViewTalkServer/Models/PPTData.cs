using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewTalkServer.Models
{
    public class PPTData
    {
        public byte[] CurrentPPT { get; set; }
        public int CurrentPage { get; set; }
        public int LastPage { get; set; }

        public PPTData()
        {
            ResetPPT();
        }

        public PPTData(byte[] currentPPT, int currentPage, int lastPage)
        {
            this.CurrentPPT = currentPPT;
            this.CurrentPage = currentPage;
            this.LastPage = lastPage;
        }

        public void ResetPPT()
        {
            this.CurrentPPT = new byte[0];
            this.CurrentPage = 0;
            this.LastPage = 0;
        }
    }
}
