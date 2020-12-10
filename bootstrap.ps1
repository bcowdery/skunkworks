# Run as administartor
if (-NOT ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator"))  
{  
  $arguments = "& '" +$myinvocation.mycommand.definition + "'"
  Start-Process powershell -Verb runAs -ArgumentList $arguments
  Break
}

# Install chocolatey
$testchoco = powershell choco -v
if (-not($testchoco)) {
    Write-Output "Chocolatey is not installed, hang tight ..."
    Set-ExecutionPolicy Bypass -Scope Process -Force; iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))
}

# Install scoop
$testscoop = powershell scoop -v
if (-not($testscoop)) {
    Write-Output "Scoop is not installed, let me get that for you ..."
    Set-ExecutionPolicy Bypass -Scope Process -Force; iex ((New-Object System.Net.WebClient).DownloadString('https://get.scoop.sh'))
}

# Install docker desktop
choco install docker-desktop

# Install go-task runner
scoop bucket add extras
scoop install task