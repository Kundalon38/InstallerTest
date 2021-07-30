@call "C:\Advanced Installer\Projects\RISA License Manager\build.bat"
@call "C:\Advanced Installer\Projects\Common\Merge Modules\RISA Core Files\build.bat"
@if "%~1" == "" goto default
set builds=%1
goto st
:default
set builds="Standalone"
:st
@pushd "c:\Program Files (x86)\Caphyon\Advanced Installer 18.4\bin\x86"
@set proj="C:\Advanced Installer\Projects\RISAFloor\Standalone\RISA Floor.aip"
advancedinstaller /edit %proj% /SetProperty ProductVersion="15.0.2.27"
advancedinstaller /edit %proj% /SetProperty RISA_PRODUCT_TITLE2_INSTYPE="RISAFloor 15.0"
advancedInstaller /rebuild %proj% -buildslist %builds%
@popd