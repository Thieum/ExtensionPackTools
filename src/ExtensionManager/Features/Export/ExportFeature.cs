using ExtensionManager.Manifest;
using ExtensionManager.UI;
using ExtensionManager.UI.Worker;
using ExtensionManager.VisualStudio.Documents;
using ExtensionManager.VisualStudio.Extensions;
using ExtensionManager.VisualStudio.MessageBox;

namespace ExtensionManager.Features.Export;

public sealed class ExportFeature : ExportFeatureBase
{
    public ExportFeature(IThisVsixInfo vsixInfo, IVSDocuments documents, IVSMessageBox messageBox, IVSExtensions extensions, IDialogService dialogService, IManifestService manifestService)
        : base(vsixInfo, documents, messageBox, extensions, dialogService, manifestService)
    {
    }

    protected override async Task<string?> GetFilePathAsync()
        => await DialogService.ShowSaveVsextFileDialogAsync();

    protected override async Task ShowExportDialogAsync(IManifest manifest, IExportWorker worker, IReadOnlyCollection<IVSExtension> extensions)
        => await DialogService.ShowExportDialogAsync(worker, manifest, extensions);

    protected override async Task OnManifestWrittenAsync(string filePath)
        => await Documents.OpenAsync(filePath);
}
