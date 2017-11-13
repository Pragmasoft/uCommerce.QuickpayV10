[CmdletBinding()]
Param(
  [Parameter(Mandatory=$False)]
  [string]$TargetDirectory = "C:\tmp\QuickpayV10",
    
  [Parameter(Mandatory=$False)]
  [string]$SourceDirectory, 
	
	[Parameter(Mandatory=$False)]
	[string]$NuspecFile = "App.Manifest.nuspec"
)

function GetScriptDirectory { 
  Split-Path -parent $PSCommandPath 
}

function GetProjectFolder {
	$scriptPath = GetScriptDirectory;
	
	return "$scriptPath\..\..\src\Pragmasoft.QuickpayV10.Web"
}

function MoveNuspecFile {
  $scriptPath = GetScriptDirectory;
  $nugetPath = $scriptPath + "\..\NuGet\" + $NuspecFile;

  Copy-Item -Path $nugetPath -Destination $TargetDirectory
}

function GetSolutionFile { 
   $scriptPath = GetScriptDirectory;
   $srcFolder = "$scriptPath\..\..\src";
   return Get-ChildItem -Path $srcFolder -Filter *.sln -Recurse;
}

function GetVersion {
  $scriptPath = GetScriptDirectory;
  $nuspecFile = "$scriptPath\..\NuGet\" + $NuspecFile;

  [xml]$fileContents = Get-Content -Path $nuspecFile
  return $fileContents.package.metadata.version;
}

function UpdateAssemblyInfos {
    $version = GetVersion;
    $version = GetUpdatedVersionString $version;
    
    $newVersion = 'AssemblyVersion("' + $version + '")';
    $newFileVersion = 'AssemblyFileVersion("' + $version + '")';
  
    foreach ($file in Get-ChildItem $SourceDirectory\..\ AssemblyInfo.cs -Recurse)  
    {      
        $TmpFile = $file.FullName + ".tmp"

        get-content $file.FullName | 
            %{$_ -replace 'AssemblyVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)', $newVersion } |
            %{$_ -replace 'AssemblyFileVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)', $newFileVersion } |
        set-content $TmpFile -force
        move-item $TmpFile $file.FullName -force
    }
}

function UpdateNuspecVersion {
    $scriptPath = GetScriptDirectory;
    $nuspecFile = $nuspecFilePath;

    [xml]$fileContents = Get-Content -Path $nuspecFile
    $fileContents.package.metadata.version = GetUpdatedVersionString $fileContents.package.metadata.version;
    
    $fileContents.Save($nuspecFile);
}

function GetUpdatedVersionString($version) {
    if($version.Substring($version.LastIndexOf(".") + 1) -eq "*") 
    {
        $versionDateNumberPart = (Get-Date).Year.ToString().Substring(2) + "" + (Get-Date).DayOfYear.ToString("000");
        return $version.Substring(0, $version.LastIndexOf("*")) + $versionDateNumberPart;
    }

    return $version;
}

function Run-It () {
  try {
    $scriptPath = GetScriptDirectory;

    if ($SourceDirectory.Equals(""))
    {
      $SourceDirectory = GetProjectFolder;
    }     

    #Step 01 update assembly version on projects in sln. 
    UpdateAssemblyInfos;
   
    Import-Module "$scriptPath\..\psake\4.3.0.0\psake.psm1"
                   
    #Step 02 rebuild solution
    $SolutionFile = GetSolutionFile;
    $rebuildProperties = @{
      "Solution_file" = $SolutionFile;
      "srcDir" = Resolve-Path "$scriptPath\..\..\src";
      "Configuration" = "Release"
    };

    Invoke-PSake "$ScriptPath\Rebuild.App.Solution.ps1" "Rebuild" -parameters $rebuildProperties   

    #Step 03 Maintain Nuget dependencies
    $maintainNugetDependenciesProperties = @{
      "projects" = @(
        $SourceDirectory
      );
      "nuspecFile" = "$scriptPath\..\NuGet\$NuspecFile"
    };
        
    Invoke-PSake "$ScriptPath\Maintain.Nuget.Dependencies.ps1" "Run-It" -parameters $maintainNugetDependenciesProperties

    #Step 04 Extract files
    $extractProperties = @{
      "TargetDirectory" = $TargetDirectory + "\Content";
      "SourceDirectory" = $SourceDirectory;
    };

    Invoke-PSake "$ScriptPath\Extract.Files.For.App.ps1" "Run-It" -parameters $extractProperties
   
    #Step 05 bin to ..\lib
    $pathToTargetBinDir = $TargetDirectory+ "\Content\bin"
    $pathToTargetLibDir = $TargetDirectory+ "\lib\net400"
        
    New-Item $pathToTargetLibDir -type directory
    Move-Item $pathToTargetBinDir\*.dll $pathToTargetLibDir
    Remove-Item $pathToTargetBinDir -recurse    

    # Step 05 add documentation to the package
    $DocumentationProperties = @{
      "TargetDirectory" = $TargetDirectory;
      "SourceDirectory" = $SourceDirectory + "\..\..\documentation";
    };

    Invoke-PSake "$ScriptPath\Add.Documentation.To.Package.ps1" "Run-It" -parameters $DocumentationProperties

    #6 Move the nuspec file   
    MoveNuspecFile;
    $nuget = $scriptPath + "\..\NuGet";
    $nuspecFilePath = $TargetDirectory + "\" + $NuspecFile;

    #7 Update the version in the nuspec file
    UpdateNuspecVersion;
    
    #Step 08 pack it up
    & "$nuget\nuget.exe" pack $nuspecFilePath -OutputDirectory $TargetDirectory;

    #Step 09 remove/delete files. 
    Remove-Item $TargetDirectory\* -exclude *.nupkg -recurse
  } catch {  
    Write-Error $_.Exception.Message -ErrorAction Stop  
  }
}

Run-It