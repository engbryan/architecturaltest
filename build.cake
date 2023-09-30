#tool nuget:?package=ReportGenerator&version=4.8.8

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////
 
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
 
//////////////////////////////////////////////////////////////////////
///    Build Variables
/////////////////////////////////////////////////////////////////////
var solutionFile = "Proof.Auth.sln";       

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////
 
Task("Build")
    .Does(() => 
    {
        DotNetBuild(solutionFile);
    });
 
Task("Test")
    .IsDependentOn("Build")
    .Does(() => 
    {
        if (System.IO.Directory.Exists("testresults"))
            System.IO.Directory.Delete("testresults", true);

        var settings = new DotNetTestSettings
        {
            NoRestore = true,
            ArgumentCustomization = x => x.Append("/p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:ExcludeByFile=\\\"**/Program.cs,**/Migrations/*.cs\\\""),
            Verbosity = DotNetVerbosity.Quiet
        };
 
        DotNetTest(solutionFile, settings);
    });

Task("Coverage")
    .IsDependentOn("Test")
    .Does(() => 
    {
        if (System.IO.Directory.Exists("coverageoutput"))
            System.IO.Directory.Delete("coverageoutput", true);

        GlobPattern covegareFiles = "./test/**/*.opencover.xml";

        ReportGenerator(covegareFiles, "./coverageoutput", new ReportGeneratorSettings { ReportTypes = new []
        {
            ReportGeneratorReportType.TextSummary,
            ReportGeneratorReportType.HtmlInline_AzurePipelines_Dark
        }});
    });


//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////
 
RunTarget(target);