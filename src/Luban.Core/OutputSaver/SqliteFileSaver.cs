using Luban.Utils;

namespace Luban.OutputSaver;

[OutputSaver("sqlite3")]
public class SqliteFileSaver : OutputSaverBase
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    protected override void BeforeSave(OutputFileManifest outputFileManifest, string outputDir)
    {
        //只对Code做处理
        if (outputFileManifest.OutputType == OutputType.Code)
        {
            if (!EnvManager.Current.GetBoolOptionOrDefault($"{BuiltinOptionNames.OutputSaver}.{outputFileManifest.TargetName}", BuiltinOptionNames.CleanUpOutputDir,
                    true, true))
            {
                return;
            }
            FileCleaner.Clean(outputDir, outputFileManifest.DataFiles.Select(f => f.File).ToList());
        }
    }

    public override void SaveFile(OutputFileManifest fileManifest, string outputDir, OutputFile outputFile)
    {
        //只对Code做处理
        if (fileManifest.OutputType == OutputType.Code)
        {
            string fullOutputPath = $"{outputDir}/{outputFile.File}";
            Directory.CreateDirectory(Path.GetDirectoryName(fullOutputPath));
            if (FileUtil.WriteAllBytes(fullOutputPath, outputFile.GetContentBytes()))
            {
                s_logger.Info("save file:{} ", fullOutputPath);
            }
        }
    }
}
