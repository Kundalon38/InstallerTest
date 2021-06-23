@call "C:\Advanced Installer\Projects\RISA License Manager\build.bat"
@call "C:\Advanced Installer\Projects\Common\Merge Modules\RISA Core Files\build.bat"
set builds="Demo"
@pushd "c:\Program Files (x86)\Caphyon\Advanced Installer 18.3\bin\x86"
@set proj="C:\Advanced Installer\Projects\RISA-3D_dotNET\Standalone\RISA-3D_dotNET.aip"
@set mod="C:\Advanced Installer\Projects\RISA-3D_dotNET\Standalone\RISA-3D_dotNET_demo.aip"
advancedinstaller /edit %proj% /DuplicateProject %mod%
advancedinstaller /edit %mod% /SetProperty ProductVersion="19.0.2.63472"
advancedinstaller /edit %mod% /SetProperty RISA_PRODUCT_TITLE2_INSTYPE="RISA-3D 19.0"
advancedInstaller /edit %mod% /DelPrerequisite "Sentinel System Driver 7.6.0"
advancedInstaller /rebuild %mod% -buildslist %builds%
@popd