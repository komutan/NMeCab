$ErrorActionPreference = "Stop"

if (Test-Path ./unidic-cwj-2.2.0)
{
    Write-Output "unidic-cwj-2.2.0 already exists."
    Exit
}

if (!(Test-Path ./unidic-cwj-2.2.0.zip))
{
  Invoke-WebRequest https://unidic.ninjal.ac.jp/unidic_archive/cwj/2.2.0/unidic-cwj-2.2.0.zip -OutFile ./unidic-cwj-2.2.0.zip
}

Expand-Archive ./unidic-cwj-2.2.0.zip -DestinationPath ../
Remove-Item ../unidic-cwj-2.2.0/*.def
Remove-Item ../unidic-cwj-2.2.0/*.csv

Remove-Item  -Confirm ./unidic-cwj-2.2.0.zip
