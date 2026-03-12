using ExtensionManager.Installation;
using ExtensionManager.Manifest;
using ExtensionManager.UI;
using ExtensionManager.UI.Worker;
using ExtensionManager.VisualStudio.Extensions;
using ExtensionManager.VisualStudio.MessageBox;

namespace ExtensionManager.Features.Install;

public sealed class InstallFeature : InstallFeatureBase
{
    public InstallFeature(IVSExtensions extensions, IVSMessageBox messageBox, IDialogService dialogService, IExtensionInstaller installer, IManifestService manifestService)
        : base(extensions, messageBox, dialogService, installer, manifestService)
    {
    }

    protected override async Task<string?> GetFilePathAsync()
        => await DialogService.ShowOpenVsextFileDialogAsync();

    protected override async Task ShowInstallDialogAsync(IManifest manifest, IInstallWorker worker, IReadOnlyCollection<VSExtensionToInstall> extensions)
        => await DialogService.ShowInstallDialogAsync(worker, manifest, extensions);
}
