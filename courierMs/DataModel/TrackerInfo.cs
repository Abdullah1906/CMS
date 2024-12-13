using courierMs.ViewModel;
using System.ComponentModel.DataAnnotations;

namespace courierMs.DataModel
{
    public class TrackerInfo
    {
        [Key]
        public int TrackId { get; set; }
        public string? ReceiverId { get; set; }
        public string? CustomerId { get; set; }
        public string? PercelId { get; set; }

        public string? TrackerName { get; set; }
        public string? Tracker_P_Number { get; set; }

        
    }
}
