@echo off

:; Install the Azure Artifacts Credentials Provider
:; https://github.com/Microsoft/artifacts-credprovider

powershell -ExecutionPolicy Byepass -Command iex ((New-Object System.Net.WebClient).DownloadString('https://aka.ms/install-artifacts-credprovider.ps1')) 