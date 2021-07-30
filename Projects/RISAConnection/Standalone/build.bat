@call "C:\Advanced Installer\Projects\RISA License Manager\build.bat"
set builds="Standalone"
@pushd "c:\Program Files (x86)\Caphyon\Advanced Installer 18.4\bin\x86"
@set proj="C:\Advanced Installer\Projects\RISAConnection\Standalone\RISA Connection.aip"
advancedinstaller /edit %proj% /SetProperty ProductVersion="12.9.0.3"
advancedinstaller /edit %proj% /SetProperty RISA_PRODUCT_TITLE2_INSTYPE="RISAConnection 12.9"
advancedInstaller /rebuild %proj% -buildslist %builds%
@popd