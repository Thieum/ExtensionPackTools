using System.Threading;
using System.Threading.Tasks;

namespace ExtensionManager.Manifest;

public interface IManifestService
{
    IManifest CreateNew();
    Task<IManifest> ReadAsync(string filePath);
    Task WriteAsync(string filePath, IManifest manifest);
}