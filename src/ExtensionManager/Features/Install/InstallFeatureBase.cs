using ExtensionManager.Installation;
using ExtensionManager.Manifest;
using ExtensionManager.UI;
using ExtensionManager.UI.Worker;
using ExtensionManager.Utils;
using ExtensionManager.VisualStudio.Extensions;
using ExtensionManager.VisualStudio.MessageBox;

namespace ExtensionManager.Features.Install;

public abstract class InstallFeatureBase : IFeature, IInstallWorker
{
    private readonly IVSExtensions _extensions;
    private readonly IVSMessageBox _messageBox;
    private readonly IDialogService _dialogService;
    private readonly IExtensionInstaller _installer;
    private readonly IManifestService _manifestService;

    protected IVSExtensions Extensions => _extensions;
    protected IVSMessageBox MessageBox => _messageBox;
    protected IDialogService DialogService => _dialogService;
    protected IExtensionInstaller Installer => _installer;
    protected IManifestService ManifestService => _manifestService;

    protected InstallFeatureBase(IVSExtensions extensions, IVSMessageBox messageBox, IDialogService dialogService, IExtensionInstaller installer, IManifestService manifestService)
    {
        _extensions = extensions;
        _messageBox = messageBox;
        _dialogService = dialogService;
        _installer = installer;
        _manifestService = manifestService;
    }

    public async Task ExecuteAsync()
    {
        var filePath = await GetFilePathAsync().ConfigureAwait(false);

        if (filePath is null or { Length: 0 })
            return;

        if (!File.Exists(filePath))
            return;

        var manifest = await ManifestService.ReadAsync(filePath).ConfigureAwait(false);
        var extensionsToInstall = await CreateExtensionsToInstallListAsync(manifest.Extensions).ConfigureAwait(false);

        await ShowInstallDialogAsync(manifest, this, extensionsToInstall.ToList());
    }

    protected virtual async Task<IEnumerable<VSExtensionToInstall>> CreateExtensionsToInstallListAsync(IEnumerable<IVSExtension> toInstall)
    {
        var installed = await Extensions.GetInstalledExtensionsAsync().ConfigureAwait(false);
        var gallery = await Extensions.GetGalleryExtensionsAsync(toInstall.Select(x => x.Id)).ConfigureAwait(false);

        var statuses = new Dictionary<string, VSExtensionStatus>();

        foreach (var extension in toInstall)
            statuses[extension.Id] = VSExtensionStatus.NotSupported;

        foreach (var extension in gallery)
            statuses[extension.Id] = VSExtensionStatus.NotInstalled;

        foreach (var extension in installed.Intersect(toInstall, ExtensionEqualityComparer.Instance))
            statuses[extension.Id] = VSExtensionStatus.Installed;

        return toInstall
            .Distinct(ExtensionEqualityComparer.Instance)
            .Select(x => new VSExtensionToInstall(x, statuses[x.Id]));
    }

    async Task IInstallWorker.InstallAsync(IManifest manifest, IReadOnlyCollection<IVSExtension> extensions, bool systemWide, IProgress<ProgressStep<InstallStep>> progress, CancellationToken cancellationToken)
    {
        if (extensions.Count > 0)
            await Installer.InstallAsync(extensions, systemWide, progress, cancellationToken);
    }

    protected abstract Task<string?> GetFilePathAsync();
    protected abstract Task ShowInstallDialogAsync(IManifest manifest, IInstallWorker worker, IReadOnlyCollection<VSExtensionToInstall> extensions);
}
