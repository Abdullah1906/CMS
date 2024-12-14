using courierMs.ViewModel;
using System.ComponentModel.DataAnnotations;

namespace courierMs.DataModel
{
    public class TrackerInfo
    {
        [Key]
        public Guid TrackId { get; set; }
        public Guid? ReceiverId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? PercelId { get; set; }

        public string? TrackerName { get; set; }
        public string? Tracker_P_Number { get; set; }

        
    }
}
