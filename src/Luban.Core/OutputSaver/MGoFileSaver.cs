using Luban.Utils;
namespace Luban.OutputSaver;

[OutputSaver("m_go")]
public class MGoFileSaver: OutputSaverBase
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();
    protected override void BeforeSave(OutputFileManifest outputFileManifest, string outputDir)
    {
        if (!EnvManager.Current.GetBoolOptionOrDefault($"{BuiltinOptionNames.OutputSaver}.{outputFileManifest.TargetName}", BuiltinOptionNames.CleanUpOutputDir,
                true, true))
        {
            return;
        }
        if(outputFileManifest.OutputType == OutputType.Code)
            FileCleaner.Clean(outputDir, outputFileManifest.DataFiles.Select(f => f.File).ToList(), "*.go", false);
        else
        {
            FileCleaner.Clean(outputDir, outputFileManifest.DataFiles.Select(f=>f.File).ToList());
        }
    }
    public override void SaveFile(OutputFileManifest fileManifest, string outputDir, OutputFile outputFile)
    {
        string fullOutputPath = $"{outputDir}/{outputFile.File}";
        Directory.CreateDirectory(Path.GetDirectoryName(fullOutputPath));
        if (FileUtil.WriteAllBytes(fullOutputPath, outputFile.GetContentBytes()))
        {
            s_logger.Info("save file:{} ", fullOutputPath);
        }
    }
}
