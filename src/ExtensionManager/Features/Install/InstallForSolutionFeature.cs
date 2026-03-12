using ExtensionManager.Installation;
using ExtensionManager.Manifest;
using ExtensionManager.UI;
using ExtensionManager.UI.Worker;
using ExtensionManager.VisualStudio.Extensions;
using ExtensionManager.VisualStudio.MessageBox;
using ExtensionManager.VisualStudio.Solution;

namespace ExtensionManager.Features.Install;

public sealed class InstallForSolutionFeature : InstallFeatureBase
{
    private readonly IVSSolutions _solutions;

    public InstallForSolutionFeature(IVSExtensions extensions, IVSMessageBox messageBox, IDialogService dialogService, IExtensionInstaller installer, IManifestService manifestService, IVSSolutions solutions)
        : base(extensions, messageBox, dialogService, installer, manifestService)
    {
        _solutions = solutions;
    }

    protected override async Task<string?> GetFilePathAsync()
        => await _solutions.GetCurrentSolutionExtensionsManifestFilePathAsync(MessageBox);

    protected override async Task<IEnumerable<VSExtensionToInstall>> CreateExtensionsToInstallListAsync(IEnumerable<IVSExtension> toInstall)
    {
        var extensions = await base.CreateExtensionsToInstallListAsync(toInstall);

        return extensions
            .Where(x => x.Status != VSExtensionStatus.Installed);
    }

    protected override async Task ShowInstallDialogAsync(IManifest manifest, IInstallWorker worker, IReadOnlyCollection<VSExtensionToInstall> extensions)
    {
        if (extensions.Count == 0)
            return;

        await DialogService.ShowInstallForSolutionDialogAsync(worker, manifest, extensions);
    }
}
