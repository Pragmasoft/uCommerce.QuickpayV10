properties {
  $projects = $projects;
  $nuspecFile = $nuspecFile;
}

function LoadXmlFile ($filePath) {
  $packagesConfig = New-Object XML
  $packagesConfig.Load($filePath)
  return $packagesConfig;
}

function GetNugetDependencies {
	$dependencies = @();
	
	foreach ($project in $projects) {
    $filePath = $project + '\packages.config';
    $packagesConfig = LoadXmlFile $filePath
    
    foreach($package in $packagesConfig.packages.package)
    {
      if($dependencies -notcontains $package){ 
        $dependencies += $package
      }
    }
  }		
	return $dependencies
}

function RemoveDuplicates ($dependencies){
  $refactoredDependencies = @();
  
  foreach ($dependency in $nugetDependencies) {     
     $addDependency = $True;
     foreach ($refactoredDependency in $refactoredDependencies) {
       if($refactoredDependency.id -eq $dependency.id){ #Could check version and add difference versions. 
         $addDependency = $False;
       }   
     }
     
     if($addDependency){
      $refactoredDependencies += $dependency  
     }
  }
  return $refactoredDependencies
}

function RemoveAlreadyRegisteredDependencies ($nuspec) { 
  $nuspec = LoadXmlFile $nuspecFile
  $dependencies = $nuspec.selectSingleNode('package/metadata/dependencies')
  if($dependencies){
    $nuspec.package.metadata.removeChild($dependencies)
  }
  $nuspec.Save($nuspecFile)
}

function AddDependenciesToNuspec ($dependencies){
  $nuspec = LoadXmlFile $nuspecFile

  $dependenciesEle;
  if($nuspec.package.metadata.dependencies){
    $dependenciesEle = $nuspec.package.metadata.dependencies
  }
  else {
    write-host "*****************d**************"
    write-host $nuspec.package.metadata.dependencies
    write-host $nuspec.package.metadata.id
    $dependenciesEle = $nuspec.CreateElement('dependencies')  
  }
  
  foreach ($package in $nugetDependencies) {          
     $dependencyEle = $nuspec.CreateElement('dependency')
     $dependencyEle.SetAttribute('id', $package.id)
     $dependencyEle.SetAttribute('version', $package.version)
     
     $dependenciesEle.AppendChild($dependencyEle) 
  }
  $nuspec.package.metadata.AppendChild($dependenciesEle) 
  $nuspec.Save($nuspecFile)
}

task Run-It {        
	write-host 'Maintain Nuget dependencies';
  RemoveAlreadyRegisteredDependencies;
  $nugetDependencies = GetNugetDependencies;
  $nugetDependencies = RemoveDuplicates $nugetDependencies;
  AddDependenciesToNuspec $nugetDependencies;
}