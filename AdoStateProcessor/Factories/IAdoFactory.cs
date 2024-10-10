using Microsoft.VisualStudio.Services.WebApi;

namespace AdoStateProcessor.Factories
{
    public interface IAdoFactory
    {
        public VssConnection Create();
    }
}
