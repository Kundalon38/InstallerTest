@call "C:\Advanced Installer\Projects\RISA License Manager\build.bat"
@call "C:\Advanced Installer\Projects\Common\Merge Modules\RISA Core Files\build.bat"
set builds="Demo"
@pushd "c:\Program Files (x86)\Caphyon\Advanced Installer 18.4\bin\x86"
@set proj="C:\Advanced Installer\Projects\RISAFoundation\Standalone\RISAFoundation.aip"
@set mod="C:\Advanced Installer\Projects\RISAFoundation\Standalone\RISAFoundation_demo.aip"
advancedinstaller /edit %proj% /DuplicateProject %mod%
advancedinstaller /edit %mod% /SetProperty ProductVersion="13.0.0.1"
advancedinstaller /edit %mod% /SetProperty RISA_PRODUCT_TITLE2_INSTYPE="RISA Foundation 13.0 (Demo)"
advancedInstaller /edit %mod% /DelPrerequisite "Sentinel System Driver 7.6.0"
advancedInstaller /rebuild %mod% -buildslist %builds%
@popd