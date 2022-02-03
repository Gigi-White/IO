using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Build.Tasks;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.NUnit;
using Nuke.Common.Tools.Xunit;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;

[CheckBuildProjectConfigurations]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build> (x => x.Compile);

    [Parameter ("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion] readonly GitVersion GitVersion;

    //AbsolutePath SourceDirectory => RootDirectory / "source";
    List<AbsolutePath> SourceDirectories => new() { RootDirectory / "Archive.Zip", RootDirectory / "Core",  };
    List<AbsolutePath> TestsDirectories => new() { RootDirectory / "Archive.Zip.UnitTests", RootDirectory / "UnitTests", };
    AbsolutePath OutputDirectory => RootDirectory / "output";

    List<AbsolutePath> NugetDirectories => SourceDirectories;

    Target Clean => _ => _
    .Before(Restore)
    .Executes(() =>
    {
        Serilog.Log.Write(0, "Second Hello!");
        SourceDirectories.ForEach (x=>x.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory));
    //SourceDirectoryGlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
    //TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
    EnsureCleanDirectory(OutputDirectory);

});

    Target NugetTest => _ => _.Executes (() =>
    {
        
        NugetDirectories.ForEach (path =>
        {
            MSBuild(s => s
                    .SetTargetPath(path)
                    .SetTargets("Pack")
                    .SetPackageOutputPath (RootDirectory + "/outputNuget")
            );
        });
    });

    [PackageExecutable(
            packageId: "nunit.consolerunner",
            packageExecutable: "nunit3-console.exe")]
    readonly Tool Nunit;

    Target RunUnitTests => _ => _.Executes(() =>
    {
        String NunitParams = $".";
        TestsDirectories.ForEach(path =>
        {
            Nunit(NunitParams, path);
        });
    });

    Target Test => _ => _
            .Executes (() =>
            {
                Serilog.Log.Write (0, "Hello World!");
                Serilog.Log.Write (0, RootDirectory + " / ");
                ScheduledTargets.ForEach (x =>
                {
                    Serilog.Log.Write (Serilog.Events.LogEventLevel.Debug, x.ToString() + "");
                });
                
                
            });

    Target Restore => _ => _
        .Executes(() =>
        {
            MSBuild(s => s
                .SetTargetPath(Solution)
                .SetTargets("Restore"));
        });

    Target Compile => _ => _
        .DependsOn(Restore,Test,Clean)
        .Executes(() =>
        {
            MSBuild(s => s
                .SetTargetPath(Solution)
                .SetTargets("Rebuild")
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .SetMaxCpuCount(Environment.ProcessorCount)
                .SetNodeReuse(IsLocalBuild));
        });
    Target Publish => _ => _
            .Executes(() =>
            {
                
            });

    Target Update => _ => _
            .TriggeredBy(Publish)
            .Executes(() =>
            {
                Serilog.Log.Write(0, "Update World!");
            });
}
