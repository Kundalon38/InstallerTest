echo -Fetch core dependencies
@call "C:\Advanced Installer\Projects\Common\Merge Modules\RISA Core Files\fetch.bat"

echo -Delete the Release Folders in Project Files
rmdir /Q /S "C:\Advanced Installer\Projects\RISA-3D_dotNET\Standalone\SetupFiles"

echo -Copying RISA-3D Source Files
robocopy "G:\Shared drives\Tech\Installer Staging\RISA-3D_dotNET" "C:\Advanced Installer\Source Files\RISA-3D_dotNET\Exes" /e /X /TS /FP /NP /V 
robocopy "G:\Shared drives\Tech\Installer Staging\RISA-3D_dotNET_Demo" "C:\Advanced Installer\Source Files\RISA-3D_dotNET\Exes\DemoBackup" /e /X /TS /FP /NP /V 
copy "G:\Shared drives\Tech\Installer Staging\RISA-3D17\risa3dw.exe" "C:\Advanced Installer\Source Files\RISA-3D_dotNET\Exes\risa3dw17.exe"  /y

robocopy "G:\Shared drives\Support Team\Install Sources\Common Files\SAF" "C:\Advanced Installer\Source Files\Common Files\SAF" /e /X /TS /FP /NP /V 


