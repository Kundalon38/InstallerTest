echo -Fetch core dependencies
@call "C:\Advanced Installer\Projects\Common\Merge Modules\RISA Core Files\fetch.bat"

echo -Delete the Release Folders in Project Files
rmdir /Q /S "C:\Advanced Installer\Projects\RISAConnection\Standalone\SetupFiles"

echo -Copying RISAConnection Source Files
robocopy "G:\Shared drives\Tech\Installer Staging\RISAConnection" "C:\Advanced Installer\Source Files\RISAConnection\Exes" /e /X /TS /FP /NP /V 
robocopy "G:\Shared drives\Tech\Installer Staging\RISA-3D_dotNET_Demo" "C:\Advanced Installer\Source Files\RISA-3D_dotNET\Exes\DemoBackup" /e /X /TS /FP /NP /V 
copy "G:\Shared drives\Tech\Installer Staging\RISA-3D17\risa3dw.exe" "C:\Advanced Installer\Source Files\RISA-3D_dotNET\Exes\risa3dw17.exe"  /y

robocopy "G:\Shared drives\Support Team\Install Sources\Common Files\SAF" "C:\Advanced Installer\Source Files\Common Files\SAF" /e /X /TS /FP /NP /V 


robocopy "G:\Shared drives\Tech\Installer Staging\RISAConnection" "C:\Advanced Installer\Source Files\RISAConnection\Exes" /e /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\RISAConnection\Examples" "C:\Advanced Installer\Source Files\RISAConnection\Examples" /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\RISAConnection\Help" "C:\Advanced Installer\Source Files\RISAConnection\Help" /e /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\RISAConnection\INI" "C:\Advanced Installer\Source Files\RISAConnection\INI" /e /X /TS /FP /NP /V
rem robocopy "G:\Shared drives\Support Team\Install Sources\RISAConnection\Install Support Files" "C:\Advanced Installler\Projects\RISAConnection\Demo" /X /TS /FP /NP /V
rem robocopy "G:\Shared drives\Support Team\Install Sources\RISAConnection\Install Support Files" "C:\Advanced Installer\Projects\RISAConnection\Standalone" /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\RISAConnection\Manuals" "C:\Advanced Installer\Source Files\RISAConnection\Manuals" /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\RISAConnection\Region Defaults" "C:\Advanced Installer\Source Files\RISAConnection\Region Defaults" /e /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\RISAConnection\Release Notes" "C:\Advanced Installer\Source Files\RISAConnection\Release Notes" /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\RISAConnection\Silent Install" "C:\Advanced Installer\Source Files\RISAConnection\Silent Install"   /X /TS /FP /NP /V

robocopy "G:\Shared drives\Support Team\Install Sources\Common Files\DirectX" "C:\Advanced Installer\Source Files\Common Files\DirectX" /e  /X /TS /FP /NP /V

copy "G:\Shared drives\Support Team\Install Sources\Common Files\Install Screens\install_splash_cn.bmp" "C:\Advanced Installer\Projects\RISAConnection\Assets\Setup.bmp" /y 