@call "C:\Advanced Installer\Projects\RISA License Manager\build.bat"
set builds="Demo"
@pushd "c:\Program Files (x86)\Caphyon\Advanced Installer 18.3\bin\x86"
@set proj="C:\Advanced Installer\Projects\RISAConnection\Standalone\RISA Connection.aip"
@set mod="C:\Advanced Installer\Projects\RISAConnection\Standalone\RISA Connection_demo.aip"
advancedinstaller /edit %proj% /DuplicateProject %mod%
advancedinstaller /edit %proj% /SetProperty RISA_PRODUCT_VERSION2="12.9"
advancedinstaller /edit %proj% /SetProperty RISA_PRODUCT_VERSION34="0.3"
advancedinstaller /edit %proj% /SetProperty RISA_PRODUCT_TITLE2_INSTYPE="RISAConnection 12.9 (Demo)"
advancedInstaller /edit %mod% /DelPrerequisite "Sentinel System Driver 7.6.0"
advancedInstaller /rebuild %mod% -buildslist %builds%
@popd