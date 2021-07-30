echo -Fetch core dependencies
@call "C:\Advanced Installer\Projects\Common\Merge Modules\RISA Core Files\fetch.bat"

echo -Delete the Release Folders in Project Files
rmdir /Q /S "C:\Advanced Installer\Projects\RISAFoundation\Standalone\SetupFiles"

echo -Copying RISAFoundation Source Files
robocopy "G:\Shared drives\Tech\Installer Staging\RISAFoundation" "C:\Advanced Installer\Source Files\RISAFoundation\Exes" /e /X /TS /FP /NP /V 
robocopy "G:\Shared drives\Tech\Installer Staging\RISAFoundation_Demo" "C:\Advanced Installer\Source Files\RISAFoundation\Exes\Demo" /e /X /TS /FP /NP /V 


