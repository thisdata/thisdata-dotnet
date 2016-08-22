using System.Runtime.Serialization;

namespace ThisData.Models
{
    [DataContract]
    public class VerifyResult 
    {
        [DataMember(Name = "score")]
        public double Score { get; set; }

        [DataMember(Name = "risk_level")]
        public string RiskLevel { get; set; }

        [DataMember(Name = "triggers")]
        public string[] Triggers { get; set; }

        [DataMember(Name = "messages")]
        public string[] Messages { get; set; }
    }
}
