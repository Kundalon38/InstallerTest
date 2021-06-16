@call "C:\Advanced Installer\Projects\RISA License Manager\build.bat"
@call "C:\Advanced Installer\Projects\Common\Merge Modules\RISA Core Files\build.bat"
set builds="Demo"
@pushd "c:\Program Files (x86)\Caphyon\Advanced Installer 18.3\bin\x86"
@set proj="C:\Advanced Installer\Projects\RISAFloor\Standalone\RISA Floor.aip"
@set mod="C:\Advanced Installer\Projects\RISAFloor\Standalone\RISA Floor_demo.aip"
advancedinstaller /edit %proj% /DuplicateProject %mod%
advancedinstaller /edit %mod% /SetProperty ProductVersion="15.0.2.27"
advancedinstaller /edit %mod% /SetProperty RISA_PRODUCT_VERSION2="15.0"
advancedinstaller /edit %mod% /SetProperty RISA_PRODUCT_VERSION34="2.27"
advancedinstaller /edit %mod% /SetProperty RISA_PRODUCT_TITLE2_INSTYPE="RISAFloor 15.0 (Demo)"
advancedInstaller /edit %mod% /DelPrerequisite "Sentinel System Driver 7.6.0"
advancedInstaller /rebuild %mod% -buildslist %builds%
@popd