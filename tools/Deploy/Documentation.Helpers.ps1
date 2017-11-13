function GuardAgainstEmpty {
    $value = $args[0]
    $name = $args[1]

    if([string]::IsNullOrEmpty($value)) 
    {
        Write-Host "The parameter $name is empty"
        return $true
    }
    return $false
}

function EnsurePathExists {
    $path = $args[0]
    $name = $args[1]

    if(!(Test-Path $path)) 
    {
        Write-Host "Parameter: $name, path does not exist: $path"
        return $true
    }
    return $false
}

function EnsureDirectoryIsNotEmpty {
    $path = $args[0]
    $name = $args[1]
    
    $InputContent = $path + "\*"

    if(!(Test-Path $InputContent))
    {
        Write-Host "Paramater: $name, $path cannot be empty"
        return $true
    }
    return $false
}