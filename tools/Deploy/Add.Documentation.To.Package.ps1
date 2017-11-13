properties {
    $TargetDirectory;
    $SourceDirectory;
}

. ./Documentation.Helpers.ps1

Task Run-It {
    if(GuardAgainstEmpty $TargetDirectory "TargetDirectory") { return }
    if(GuardAgainstEmpty $SourceDirectory "SourceDirectory") { return }

    if(EnsurePathExists $TargetDirectory "TargetDirectory") { return }
    if(EnsurePathExists $SourceDirectory "SourceDirectory") { return }

    if(EnsureDirectoryIsNotEmpty $SourceDirectory "SourceDirectory") { return }

    Copy-Item $SourceDirectory $TargetDirectory -Force -recurse 
}