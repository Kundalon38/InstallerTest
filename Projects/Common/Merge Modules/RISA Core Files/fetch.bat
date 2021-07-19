
echo -Copying Common Source Files
robocopy "C:\Advanced Installer\Source Files\Common Files\Databases" "C:\Advanced Installer\Source Files\Common Files\DatabasesBAK"  /X /TS /FP /NP /V
erase /Q "C:\Advanced Installer\Source Files\Common Files\Databases\*.*"

echo -Copying Common Source Files
robocopy "G:\Shared drives\Support Team\Install Sources\Common Files\CFS Schedules" "C:\Advanced Installer\Source Files\Common Files\CFS Schedules" /e /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\Common Files\Databases" "C:\Advanced Installer\Source Files\Common Files\Databases"   /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\Common Files\Database Utilities" "C:\Advanced Installer\Source Files\Common Files\Database Utilities" *.exe /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\Common Files\Design Lists" "C:\Advanced Installer\Source Files\Common Files\Design Lists" *.asc /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\Common Files\Detail Reports" "C:\Advanced Installer\Source Files\Common Files\Detail Reports" /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\Common Files\DXF" "C:\Advanced Installer\Source Files\Common Files\DXF"   /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\Common Files\INI" "C:\Advanced Installer\Source Files\Common Files\INI" *.ini /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\Common Files\Library" "C:\Advanced Installer\Source Files\Common Files\Library" *.dll /e /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\Common Files\License\Utilities" "C:\Advanced Installer\Source Files\Common Files\License\Utilities" /e /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\Common Files\LC_lists" "C:\Advanced Installer\Source Files\Common Files\LC_lists" *.xml  /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\Common Files\Manuals" "C:\Advanced Installer\Source Files\Common Files\Manuals"   /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\Common Files\RISA_Wood_Schedules" "C:\Advanced Installer\Source Files\Common Files\RISA_Wood_Schedules" /e   /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\Common Files\Silent Install" "C:\Advanced Installer\Source Files\Common Files\Silent Install" /e  /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\Common Files\Time History" "C:\Advanced Installer\Source Files\Common Files\Time History" /e  /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\Common Files\Yield Links" "C:\Advanced Installer\Source Files\Common Files\Yield Links" /e   /X /TS /FP /NP /V

robocopy "G:\Shared drives\Tech\Installer Staging\RISA Key Manager" "C:\Advanced Installer\Source Files\RISA Key Manager" /e /X /TS /FP /NP /V
rmdir "C:\Advanced Installer\Source Files\RISA License Manager" /s /q
robocopy "G:\Shared drives\Tech\Installer Staging\RISA License Manager" "C:\Advanced Installer\Source Files\RISA License Manager" /E /X /TS /FP /NP /V
robocopy "G:\Shared drives\Tech\Installer Staging\SafeNet\Sentinel RMS" "C:\Advanced Installer\Source Files\SafeNet\Sentinel RMS" /e  /X /TS /FP /NP /V
robocopy "G:\Shared drives\Tech\Installer Staging\SafeNet\Sentinel System Driver" "C:\Advanced Installer\Source Files\SafeNet\Sentinel System Driver" /e  /X /TS /FP /NP /V


robocopy "G:\Shared drives\Tech\Installer Staging\RISA-3D" "C:\Advanced Installer\Source Files\RISA-3D\Exes" /e  /X /TS /FP /NP /V

robocopy "G:\Shared drives\Support Team\Install Sources\RISA-3D\Examples" "C:\Advanced Installer\Source Files\RISA-3D\Examples" /MIR  /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\RISA-3D\Help" "C:\Advanced Installer\Source Files\RISA-3D\Help" /e  /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\RISA-3D\Help\RISA-3D" "C:\Advanced Installer\Source Files\RISA-3D\Help\RISA-3D"   /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\RISA-3D\Manuals" "C:\Advanced Installer\Source Files\RISA-3D\Manuals"   /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\RISA-3D\Region Defaults" "C:\Advanced Installer\Source Files\RISA-3D\Region Defaults" /e   /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\RISA-3D\Release Notes" "C:\Advanced Installer\Source Files\RISA-3D\Release Notes"   /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\RISA-3D\Silent Install" "C:\Advanced Installer\Source Files\RISA-3D\Silent Install"   /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\RISA-3D\Tutorials" "C:\Advanced Installer\Source Files\RISA-3D\Tutorials" /MIR  /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\RISAFoundation\Examples" "C:\Advanced Installer\Source Files\RISAFoundation\Examples" /MIR  /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\RISAFoundation\Help" "C:\Advanced Installer\Source Files\RISAFoundation\Help"   /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\RISAFoundation\Install Support Files" "C:\Advanced Installer\Projects\RISAFoundation\Assets" /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\RISAFoundation\Manuals" "C:\Advanced Installer\Source Files\RISAFoundation\Manuals"   /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\RISAFoundation\Region Defaults" "C:\Advanced Installer\Source Files\RISAFoundation\Region Defaults" /e   /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\RISAFoundation\Release Notes" "C:\Advanced Installer\Source Files\RISAFoundation\Release Notes"   /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\RISAFoundation\Silent Install" "C:\Advanced Installer\Source Files\RISAFoundation\Silent Install"   /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\RISAFoundation\Tutorials" "C:\Advanced Installer\Source Files\RISAFoundation\Tutorials" /MIR  /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\RISA-3D\STAAD" "C:\Advanced Installer\Source Files\RISA-3D\STAAD" /X /TS /FP /NP /V
robocopy "G:\Shared drives\Tech\Installer Staging\RISAFloor" "C:\Advanced Installer\Source Files\RISAFloor\Exes" /e  /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\RISAFloor\Examples" "C:\Advanced Installer\Source Files\RISAFloor\Examples" /MIR   /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\RISAFloor\Help" "C:\Advanced Installer\Source Files\RISAFloor\Help"   /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\RISAFloor\Install Support Files" "C:\Advanced Installer\Projects\RISAFloor\Assets" /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\RISAFloor\Manuals" "C:\Advanced Installer\Source Files\RISAFloor\Manuals"   /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\RISAFloor\Region Defaults" "C:\Advanced Installer\Source Files\RISAFloor\Region Defaults" /e   /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\RISAFloor\Release Notes" "C:\Advanced Installer\Source Files\RISAFloor\Release Notes"   /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\RISAFloor\RISA_Decks" "C:\Advanced Installer\Source Files\RISAFloor\RISA_Decks" *.xml  /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\RISAFloor\Silent Install" "C:\Advanced Installer\Source Files\RISAFloor\Silent Install"   /X /TS /FP /NP /V
robocopy "G:\Shared drives\Support Team\Install Sources\RISAFloor\Tutorials" "C:\Advanced Installer\Source Files\RISAFloor\Tutorials" /MIR   /X /TS /FP /NP /V

robocopy "G:\Shared drives\Support Team\Install Sources\RISACIS2Translator" "C:\Advanced Installer\Source Files\RISACIS2Translator" /X /TS /FP /NP /V 


copy "G:\Shared drives\Support Team\Install Sources\Common Files\Install Screens\install_splash_3d.bmp" "C:\Advanced Installer\Projects\RISA-3D_dotNET\Assets\Setup.bmp" /y 

robocopy "G:\Shared drives\Support Team\Install Sources\RISAFoundation\Help" "C:\Advanced Installer\Source Files\RISAFoundation\Help" risafnd.chm /X /TS /FP /NP /V



