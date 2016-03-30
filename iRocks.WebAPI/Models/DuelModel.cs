using iRocks.DataLayer;

namespace iRocks.WebAPI.Models
{
    public class DuelModel
    {

        public PublicationModel FirstPublication { get; set; }
        public PublicationModel SecondPublication { get; set; }
        public Vote DuelResult { get; set; }
        public string CategoryLabel { get; set; }
    }
}