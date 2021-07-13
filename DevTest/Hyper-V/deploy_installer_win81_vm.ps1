# guest services must be turned on in the VM
$VMName = "MyWin81"
$VMUser = "Tom"
# $SrcFolder = "C:\Advanced Installer\Projects\RISA-3D_dotNET\Standalone\Setup Files"
$DestFolder = "C:\Users\$VMUser\Desktop\"

$num = Read-Host "Enter an abbreviated product name"
Switch ($num)
{
    "3D" {
    	$SrcFolder = "C:\Advanced Installer\Projects\RISA-3D_dotNET\Standalone\Setup Files"
	$File = Get-ChildItem -Path $SrcFolder -Filter "*.exe" | Sort-Object LastWriteTime -Descending | Select-Object -First 1
	Write-Output "Copying $File to virtual machine $VMName..."
       	Copy-VMFile -VM (Get-VM $VMName) -SourcePath $SrcFolder"\"$File -DestinationPath "$DestFolder$file" -FileSource Host -CreateFullPath -Force
    }
    "FD" {
    	$SrcFolder = "C:\Advanced Installer\Projects\RISAFoundation\Standalone\Setup Files"
	$File = Get-ChildItem -Path $SrcFolder -Filter "*.exe" | Sort-Object LastWriteTime -Descending | Select-Object -First 1
	Write-Output "Copying $File to virtual machine $VMName..."
       	Copy-VMFile -VM (Get-VM $VMName) -SourcePath $SrcFolder"\"$File -DestinationPath "$DestFolder$file" -FileSource Host -CreateFullPath -Force
    }
    "FD.NET" {
    	$SrcFolder = "C:\Advanced Installer\Projects\RISAFoundation_dotNET\Standalone\Setup Files"
	$File = Get-ChildItem -Path $SrcFolder -Filter "*.exe" | Sort-Object LastWriteTime -Descending | Select-Object -First 1
	Write-Output "Copying $File to virtual machine $VMName..."
       	Copy-VMFile -VM (Get-VM $VMName) -SourcePath $SrcFolder"\"$File -DestinationPath "$DestFolder$file" -FileSource Host -CreateFullPath -Force
    }
    "FL" {
    	$SrcFolder = "C:\Advanced Installer\Projects\RISAFloor\Standalone\SetupFiles"
	$File = Get-ChildItem -Path $SrcFolder -Filter "*.exe" | Sort-Object LastWriteTime -Descending | Select-Object -First 1
	Write-Output "Copying $File to virtual machine $VMName..."
       	Copy-VMFile -VM (Get-VM $VMName) -SourcePath $SrcFolder"\"$File -DestinationPath "$DestFolder$file" -FileSource Host -CreateFullPath -Force
    }
    "CN" {
    	$SrcFolder = "C:\Advanced Installer\Projects\RISAConnection\Standalone\Setup Files"
	$File = Get-ChildItem -Path $SrcFolder -Filter "*.exe" | Sort-Object LastWriteTime -Descending | Select-Object -First 1
	Write-Output "Copying $File to virtual machine $VMName..."
       	Copy-VMFile -VM (Get-VM $VMName) -SourcePath $SrcFolder"\"$File -DestinationPath "$DestFolder$file" -FileSource Host -CreateFullPath -Force
    }
    "LM" {
    	$SrcFolder = "C:\Advanced Installer\Projects\RISA License Manager\RISA License Manager Dev-SetupFiles"
	$File = Get-ChildItem -Path $SrcFolder -Filter "*.exe" | Sort-Object LastWriteTime -Descending | Select-Object -First 1
	Write-Output "Copying $File to virtual machine $VMName..."
       	Copy-VMFile -VM (Get-VM $VMName) -SourcePath $SrcFolder"\"$File -DestinationPath "$DestFolder$file" -FileSource Host -CreateFullPath -Force
    }
    default { "Unknown product" }
}

