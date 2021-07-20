echo -Fetch core dependencies
@call "C:\Advanced Installer\Projects\Common\Merge Modules\RISA Core Files\fetch.bat"

echo -Delete the Release Folders in Project Files
rmdir /Q /S "C:\Advanced Installer\Projects\RISAFoundation_dotNet\Standalone\SetupFiles"

echo -Copying RISAFoundation Source Files
robocopy "G:\Shared drives\Tech\Installer Staging\RISAFoundation_dotNet" "C:\Advanced Installer\Source Files\RISAFoundation_dotNet\Exes" /e /MIR /X /TS /FP /NP /V
robocopy "G:\Shared drives\Tech\Installer Staging\RISAFoundation_dotNet_Demo" "C:\Advanced Installer\Source Files\RISAFoundation_dotNet\Exes\DemoBackup" /e /MIR /X /TS /FP /NP /V

copy "G:\Shared drives\Support Team\Install Sources\Common Files\Install Screens\install_splash_fd.bmp" "C:\Advanced Installer\Projects\RISAFoundation\Assets\Setup.bmp" /y 

