namespace Observability
{
    public class ObservabilityOptions
    {
        public string ServiceName { get; set; } = "unknown-service";
        public string ServiceVersion { get; set; } = "1.0.0";
        public string OtlpEndpoint { get; set; } = null!;
        public double SamplingRatio { get; set; } = 1.0;
    }
}
