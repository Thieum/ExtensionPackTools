using ExtensionManager.Manifest;
using ExtensionManager.UI;
using ExtensionManager.UI.Worker;
using ExtensionManager.VisualStudio.Documents;
using ExtensionManager.VisualStudio.Extensions;
using ExtensionManager.VisualStudio.MessageBox;

namespace ExtensionManager.Features.Export;

public abstract class ExportFeatureBase : IFeature, IExportWorker
{
    private readonly IThisVsixInfo _vsixInfo;
    private readonly IVSDocuments _documents;
    private readonly IVSMessageBox _messageBox;
    private readonly IVSExtensions _extensions;
    private readonly IDialogService _dialogService;
    private readonly IManifestService _manifestService;

    protected IThisVsixInfo VsixInfo => _vsixInfo;
    protected IVSDocuments Documents => _documents;
    protected IVSMessageBox MessageBox => _messageBox;
    protected IVSExtensions Extensions => _extensions;
    protected IDialogService DialogService => _dialogService;
    protected IManifestService ManifestService => _manifestService;

    protected ExportFeatureBase(IThisVsixInfo vsixInfo, IVSDocuments documents, IVSMessageBox messageBox, IVSExtensions extensions, IDialogService dialogService, IManifestService manifestService)
    {
        _vsixInfo = vsixInfo;
        _documents = documents;
        _messageBox = messageBox;
        _extensions = extensions;
        _dialogService = dialogService;
        _manifestService = manifestService;
    }

    public async Task ExecuteAsync()
    {
        var manifest = ManifestService.CreateNew();
        var installedExtensions = await Extensions.GetInstalledExtensionsAsync().ConfigureAwait(false);

        var installedExtensionsList = installedExtensions as List<IVSExtension>
            ?? installedExtensions.ToList();

        installedExtensionsList.RemoveAll(vsix => vsix.Id == VsixInfo.Id);

        await ShowExportDialogAsync(manifest, this, installedExtensions);
    }

    async Task IExportWorker.ExportAsync(IManifest manifest, IProgress<ProgressStep<ExportStep>> progress, CancellationToken cancellationToken)
    {
        var filePath = await GetFilePathAsync().ConfigureAwait(false);

        if (filePath is null or { Length: 0 })
            return;

        progress.Report(null, ExportStep.SaveManifest);
        await ManifestService.WriteAsync(filePath, manifest, cancellationToken).ConfigureAwait(false);

        progress.Report(null, ExportStep.Finish);
        await OnManifestWrittenAsync(filePath);
    }

    protected abstract Task<string?> GetFilePathAsync();
    protected abstract Task ShowExportDialogAsync(IManifest manifest, IExportWorker worker, IReadOnlyCollection<IVSExtension> installedExtensions);
    protected abstract Task OnManifestWrittenAsync(string filePath);
}
