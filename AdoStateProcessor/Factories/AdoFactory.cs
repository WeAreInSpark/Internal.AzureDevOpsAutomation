using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace AdoStateProcessor.Factories
{
    public class AdoFactory(IOptions<AdoOptions> options) : IAdoFactory
    {
        private static readonly string ADO_BASE_URL = "https://dev.azure.com/";
        public VssConnection Create()
        {
            var adoOptions = options.Value;
            var baseUri = new Uri(ADO_BASE_URL + adoOptions.Organization);

            var clientCredentials = new VssCredentials(new VssBasicCredential("username", adoOptions.Pat));
            var vssConnection = new VssConnection(baseUri, clientCredentials);
            return vssConnection;
        }
    }
}
