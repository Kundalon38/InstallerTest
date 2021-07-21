@call "C:\Advanced Installer\Projects\RISA License Manager\build.bat"
@call "C:\Advanced Installer\Projects\Common\Merge Modules\RISA Core Files\build.bat"
@if "%~1" == "" goto default
set builds=%1
goto st
:default
set builds="Standalone"
:st
@pushd "c:\Program Files (x86)\Caphyon\Advanced Installer 18.4\bin\x86"
@set proj="C:\Advanced Installer\Projects\RISAFoundation\Standalone\RISAFoundation.aip"
advancedinstaller /edit %proj% /SetProperty ProductVersion="13.0.0.1"
advancedinstaller /edit %proj% /SetProperty RISA_PRODUCT_TITLE2_INSTYPE="RISA Foundation 13.0"
advancedInstaller /rebuild %proj% -buildslist %builds%
@popd