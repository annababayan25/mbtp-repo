using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MBTP.Models
{
    public class BlackoutDate
    {
        public int BlackoutID { get; set; }

        public int PCID { get; set; }

        public string ProfitCenterName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Reason { get; set; }

    }
}
