using ExtensionManager.VisualStudio.Documents;
using ExtensionManager.VisualStudio.Extensions;
using ExtensionManager.VisualStudio.MessageBox;
using ExtensionManager.VisualStudio.Solution;
using ExtensionManager.VisualStudio.StatusBar;
using ExtensionManager.VisualStudio.Themes;
using ExtensionManager.VisualStudio.Threads;

namespace ExtensionManager.VisualStudio;

/// <summary>
/// This class is an abstraction around the Visual Studio API and should have the same look and feel as the Community.VisualStudio.Toolkit.VS class.
/// </summary>
public sealed class VSFacade : IVSFacade
{
    private readonly IVSServiceFactory _factory;
    private IVSThemes? _themes;
    private IVSThreads? _threads;
    private IVSSolutions? _solutions;
    private IVSStatusBar? _statusBar;
    private IVSDocuments? _documents;
    private IVSMessageBox? _messageBox;
    private IVSExtensions? _extensions;

    /// <summary>Contains methods for WPF to deal with Visual Studio Themes.</summary>
    public IVSThemes Themes => _themes ??= _factory.CreateThemes();

    /// <summary> Contains methods for dealing with threads. </summary>
    public IVSThreads Threads => _threads ??= _factory.CreateThreads();

    /// <summary> A collection of services related to solutions. </summary>
    public IVSSolutions Solutions => _solutions ??= _factory.CreateSolutions();

    /// <summary> An API wrapper that makes it easy to work with the status bar. </summary>
    public IVSStatusBar StatusBar => _statusBar ??= _factory.CreateStatusBar();

    /// <summary> Contains helper methods for dealing with documents. </summary>
    public IVSDocuments Documents => _documents ??= _factory.CreateDocuments();

    /// <summary> Shows message boxes. </summary>
    public IVSMessageBox MessageBox => _messageBox ??= _factory.CreateMessageBox();

    /// <summary> Contains methods for dealing with Visual Studio Extensions. </summary>
    public IVSExtensions Extensions => _extensions ??= _factory.CreateExtensions();

    public VSFacade(IVSServiceFactory factory)
    {
        _factory = factory;
    }
}