using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Client1
{
    public class CloudRoleNameTelemetryInitializer : ITelemetryInitializer
    {
        private readonly string _applicationName;

        public CloudRoleNameTelemetryInitializer(string applicationName) => _applicationName = applicationName;

        public void Initialize(ITelemetry telemetry) => telemetry.Context.Cloud.RoleName = _applicationName;
    }
}
