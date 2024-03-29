using Luban.OutputSaver;
using Luban.Utils;

namespace Luban.Unreal.FileSaver;

[OutputSaver("unreal")]
public class UnrealOutputSaver : OutputSaverBase
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    
    protected override void BeforeSave(OutputFileManifest outputFileManifest, string outputDir)
    {
        if (!EnvManager.Current.GetBoolOptionOrDefault($"{BuiltinOptionNames.OutputSaver}.{outputFileManifest.TargetName}", BuiltinOptionNames.CleanUpOutputDir,
                true, true))
        {
            return;
        }
        if(outputFileManifest.OutputType == OutputType.Data)
            FileCleaner.Clean(outputDir, outputFileManifest.DataFiles.Select(f => f.File).ToList());
        else if (outputFileManifest.OutputType == OutputType.Code)
        {
            var packageDir = EnvManager.Current.GetOptionOrDefault("", ConstStrings.PackageDirCfgName, true, ConstStrings.IncludePerfix);
            var headFileDir = Path.Join(outputDir, ConstStrings.HeaderFileRoot, packageDir);
            FileCleaner.Clean(headFileDir, outputFileManifest.DataFiles.Where(f => f.File.EndsWith(ConstStrings.HeaderSufix)).Select(t=>t.File).ToList());
            var cppFileDir = Path.Join(outputDir, ConstStrings.CppFileRoot, packageDir);
            FileCleaner.Clean(cppFileDir, outputFileManifest.DataFiles.Where(f => f.File.EndsWith(ConstStrings.CppSufix)).Select(t=>t.File).ToList());
        }
    }

    private string GetFullOutputPath(OutputFileManifest fileManifest, string outputDir, OutputFile outputFile)
    {
        string ret = $"{outputDir}/{outputFile.File}";
        if (fileManifest.OutputType == OutputType.Code)
        {
            var packageDir = EnvManager.Current.GetOptionOrDefault("", ConstStrings.PackageDirCfgName, true, ConstStrings.IncludePerfix);
            var headFileDir = Path.Join(outputDir, ConstStrings.HeaderFileRoot, packageDir);
            var cppFileDir = Path.Join(outputDir, ConstStrings.CppFileRoot, packageDir);
            if (outputFile.File.EndsWith(ConstStrings.CppSufix))
            {
                ret = $"{cppFileDir}/{outputFile.File}";

            }
            else if (outputFile.File.EndsWith(ConstStrings.HeaderSufix))
            {
                ret = $"{headFileDir}/{outputFile.File}";

            }
        }

        return ret;
    }
    public override void SaveFile(OutputFileManifest fileManifest, string outputDir, OutputFile outputFile)
    {
        string fullOutputPath = GetFullOutputPath(fileManifest, outputDir, outputFile);
        Directory.CreateDirectory(Path.GetDirectoryName(fullOutputPath));
        if (FileUtil.WriteAllBytes(fullOutputPath, outputFile.GetContentBytes()))
        {
            s_logger.Info("save file:{} ", fullOutputPath);
        }
    }
}
