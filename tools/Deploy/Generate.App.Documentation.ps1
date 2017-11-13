[CmdletBinding()]
Param(
  [Parameter(Mandatory=$True)]
  [string]$OutputDirectory,
    
  [Parameter(Mandatory=$True)]
  [string]$InputDirectory,

  [Parameter(Mandatory=$True)]
  [string]$DocumentationCompiler
)

. "$PSScriptRoot\Documentation.Helpers.ps1"

function Run-It {        
    if(GuardAgainstEmpty $InputDirectory "InputDirectory") { return }
    if(GuardAgainstEmpty $OutputDirectory "OutputDirectory") { return }
    if(GuardAgainstEmpty $DocumentationCompiler "DocumentationCompiler") { return }

    if(EnsurePathExists $DocumentationCompiler "DocumentationCompiler") { return }
    if(EnsurePathExists $InputDirectory "InputDirectory") { return }

    Remove-Item $OutputDirectory -Force -recurse

    & $DocumentationCompiler $OutputDirectory $InputDirectory 
}

Run-It