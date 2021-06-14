@call "C:\Advanced Installer\Projects\RISA License Manager\build.bat"
@pushd "c:\Program Files (x86)\Caphyon\Advanced Installer 18.3\bin\x86"
@set proj="C:\Advanced Installer\Projects\RISA License Manager\RISA License Manager Dev.aip"
advancedinstaller /edit %proj% /SetProperty ProductVersion="6.2.1.0"
advancedinstaller /edit %proj% /SetProperty RISA_PRODUCT_VERSION2="6.2"
advancedinstaller /edit %proj% /SetProperty RISA_PRODUCT_VERSION34="1.0"
advancedinstaller /edit %proj% /SetProperty RISA_PRODUCT_TITLE2_INSTYPE="RISA LIcense Manager 6.2.1.0"
advancedInstaller /rebuild %proj%
@popd