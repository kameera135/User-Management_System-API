using System;

namespace SITCAMSClientIntegration.Requests
{
    public class CreatePlatformActivityLogRequest
    {
        public int PlatformId { get; set; }

        public string Type { get; set; }

        public string Description { get; set; }

        public object ExtraData { get; set; }

        public int UserId { get; set; }

        //public DateTime ActivityDoneAt { get; set; }

    }
}
