// Models.cs
using Newtonsoft.Json;

public class FanEventDto
{
    [JsonProperty("source")]   public string Source { get; set; }   // "scheduler" | "singer"
    [JsonProperty("role")]     public string Role { get; set; }     // "cheer" | "info" | "request" | "troll"
    [JsonProperty("fan_text")] public string FanText { get; set; }  // 출력 텍스트
    [JsonProperty("scene")]    public string Scene { get; set; }    // "MC"
    [JsonProperty("ts")]       public double Ts { get; set; }       // epoch seconds
}

public class PollRespDto
{
    [JsonProperty("events")] public FanEventDto[] Events { get; set; }
}