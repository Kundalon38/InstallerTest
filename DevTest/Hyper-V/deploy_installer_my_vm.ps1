# guest services must be turned on in the VM
$VMName = "MyWin10"
$VMUser = "Tom"
# $SrcFolder = "C:\Advanced Installer\Projects\RISA-3D_dotNET\Standalone\Setup Files"
$DestFolder = "C:\Users\$VMUser\Desktop\"

$num = Read-Host "Enter an abbreviated product name"
Switch ($num)
{
    "3D" {
    	$SrcFolder = "C:\Advanced Installer\Projects\RISA-3D_dotNET\Standalone\Setup Files"
	$File = Get-ChildItem -Path $SrcFolder -Name *.exe | Sort-Object LastAccessTime -Descending | Select-Object -First 1
       	Copy-VMFile -VM (Get-VM $VMName) -SourcePath $SrcFolder"\"$File -DestinationPath "$DestFolder$file" -FileSource Host -CreateFullPath -Force
    }
    "FD" {
    	$SrcFolder = "C:\Advanced Installer\Projects\RISAFoundation\Standalone\Setup Files"
	$File = Get-ChildItem -Path $SrcFolder -Name *.exe | Sort-Object LastAccessTime -Descending | Select-Object -First 1
       	Copy-VMFile -VM (Get-VM $VMName) -SourcePath $SrcFolder"\"$File -DestinationPath "$DestFolder$file" -FileSource Host -CreateFullPath -Force
    }
    "FL" {
    	$SrcFolder = "C:\Advanced Installer\Projects\RISAFloor\Standalone\Setup Files"
	$File = Get-ChildItem -Path $SrcFolder -Name *.exe | Sort-Object LastAccessTime -Descending | Select-Object -First 1
       	Copy-VMFile -VM (Get-VM $VMName) -SourcePath $SrcFolder"\"$File -DestinationPath "$DestFolder$file" -FileSource Host -CreateFullPath -Force
    }
    "CN" {
    	$SrcFolder = "C:\Advanced Installer\Projects\RISAConnection\Standalone\Setup Files"
	$File = Get-ChildItem -Path $SrcFolder -Name *.exe | Sort-Object LastAccessTime -Descending | Select-Object -First 1
       	Copy-VMFile -VM (Get-VM $VMName) -SourcePath $SrcFolder"\"$File -DestinationPath "$DestFolder$file" -FileSource Host -CreateFullPath -Force
    }
    default { "Unknown product" }
}

