using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Egghead.Models.Captcha
{
    public class CaptchaResult
    {
        [JsonProperty("success")] public bool Success { get; set; }
        [JsonProperty("hostname")] public string Hostname { get; set; }
        [JsonProperty("challenge_ts")] public DateTime ChallengeTimeStamp { get; set; }
        [JsonProperty("error-codes")] public List<string> ErrorCodes { get; set; }
    }
}