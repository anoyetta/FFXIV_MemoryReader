# バージョン取得
$version = [System.Diagnostics.FileVersionInfo]::GetVersionInfo("Build\FFXIV_MemoryReader.dll").FileVersion

# フォルダ名
$buildFolder = ".\Build"
$fullFolder = ".\Distribute\FFXIV_MemoryReader-" + $version

# フォルダが既に存在するなら消去
if ( Test-Path $fullFolder -PathType Container ) {
	Remove-Item -Recurse -Force $fullFolder
}

# フォルダ作成
New-Item -ItemType directory -Path $fullFolder

# full
xcopy /Y /R /S "$buildFolder\*" "$fullFolder"

cd Distribute
$folder = "FFXIV_MemoryReader-" + $version

# アーカイブ
& "C:\Program Files\7-Zip\7z.exe" "a" "$folder.zip" "$folder"

pause
