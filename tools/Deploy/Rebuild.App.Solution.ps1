properties {
  $src = $srcDir
  $solution_file = $Solution_file
  $configuration = $configuration
}

task Compile -description "Compiles the complete solution" { 
  Push-Location "$src"
	
  # Set Visual Studio version explicitly so the proper build tasks can be found
  # Disable post build event to avoid deploying the solution as part of packaging
  Write-Host "Compiling solution with "$configuration
  Exec { msbuild "$solution_file" /p:Configuration=$configuration /m /p:VisualStudioVersion=12.0 /p:WarningLevel=0 /verbosity:quiet /p:PostBuildEvent= }
    
  Pop-Location
}

task CleanBinDirectory -description "Cleans the bin directory" {
  Push-Location "$src"
	if (Test-Path $src\bin)
	{
		Remove-Item -Recurse "$src\bin\*" -Force
	}
	Pop-Location
}

task CleanSolution -description "Cleans the complete solution" {
  Push-Location "$src"
  Exec { msbuild "$solution_file" /p:Configuration=$configuration /t:Clean /verbosity:quiet /p:VisualStudioVersion=12.0 }
  Pop-Location
}

task Rebuild -depend CleanSolution, CleanBinDirectory, Compile