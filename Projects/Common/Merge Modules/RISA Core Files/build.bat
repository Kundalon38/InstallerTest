@pushd "c:\Program Files (x86)\Caphyon\Advanced Installer 18.3\bin\x86"
robocopy "c:\Advanced Installer\Source Files\RISA-3D\Region Defaults" "c:\Advanced Installer\Source Files\Region Defaults Merged" /E /XO /NFL /NDL
robocopy "c:\Advanced Installer\Source Files\RISAFoundation\Region Defaults" "c:\Advanced Installer\Source Files\Region Defaults Merged" /E /XO /NFL /NDL
robocopy "c:\Advanced Installer\Source Files\RISAFloor\Region Defaults" "c:\Advanced Installer\Source Files\Region Defaults Merged" /E /XO /NFL /NDL
@set proj="C:\Advanced Installer\Projects\Common\Merge Modules\RISA Core Files\RISA Core Files.aip"
advancedInstaller /rebuild %proj%
@popd