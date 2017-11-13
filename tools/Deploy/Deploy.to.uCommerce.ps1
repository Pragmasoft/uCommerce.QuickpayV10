Param(
  [Parameter(Mandatory=$False)]
  [string]$SourceDirectory
)

function GetDeploymentDirectories {
  return @(
    "C:\Repositories\XXX.Website"
  )
}

function GetAppName {
  $scriptPath = Get-ScriptDirectory;
  $nuspecFile = "$scriptPath\..\NuGet\App.Manifest.nuspec";

  [xml]$fileContents = Get-Content -Path $nuspecFile
  return $fileContents.package.metadata.id;
}

function Get-ScriptDirectory {
    Split-Path $script:MyInvocation.MyCommand.Path
}

function GetProjectFolder {
	$scriptPath = Get-ScriptDirectory;
	
	return "$scriptPath\..\..\src\Pragmasoft.QuickpayV10.Web"
}

function LocateAppsFolder($deployment_directory){
	$folderName = "Apps"
	$pathToAppsFolders = (gci -path $deployment_directory -filter $foldername -Recurse).FullName
		
	foreach($pathToAppsFolder in $pathToAppsFolders)
  {
		if($pathToAppsFolder -like '*uCommerce\Apps*'){			
			return $pathToAppsFolder;
		}
  }	 
}

function Run-It () {
  try {  
		if ($SourceDirectory.Equals(""))
		{
			$SourceDirectory = GetProjectFolder;
		}

		$scriptPath = Get-ScriptDirectory;
    Import-Module "$scriptPath\..\psake\4.3.0.0\psake.psm1"
		$deployment_directories = GetDeploymentDirectories;
		$appName = GetAppName;      

    foreach($deployment_directory in $deployment_directories)
    {
      $appsFolder = LocateAppsFolder($deployment_directory);
      $targetDir = "$appsFolder\" + $appName;
      $properties = @{
        "TargetDirectory" = $targetDir;
        "SourceDirectory" = $SourceDirectory;
      };
           
      Invoke-PSake "$ScriptPath\Extract.Files.For.App.ps1" "Run-It" -parameters $properties
		   
      #Copy nuspec file to Apps folder
      Copy-Item "$scriptPath\..\NuGet\App.Manifest.nuspec" $targetDir -Force		
    }	
  } catch {  
    Write-Error $_.Exception.Message -ErrorAction Stop  
  }
}

Run-It