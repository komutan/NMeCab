$ErrorActionPreference = "Stop"

if (Test-Path ./unidic-2.1.2)
{
    Write-Output "unidic-2.1.2 already exists."
    Exit
}

if (!(Test-Path ./unidic-mecab-2.1.2_bin.zip))
{
  Invoke-WebRequest https://unidic.ninjal.ac.jp/unidic_archive/cwj/2.1.2/unidic-mecab-2.1.2_bin.zip -OutFile ./unidic-mecab-2.1.2_bin.zip
}

Expand-Archive ./unidic-mecab-2.1.2_bin.zip -DestinationPath ../
Rename-Item ../unidic-mecab-2.1.2_bin unidic-2.1.2

Remove-Item  -Confirm ./unidic-mecab-2.1.2_bin.zip
