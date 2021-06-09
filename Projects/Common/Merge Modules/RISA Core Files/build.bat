@pushd "c:\Program Files (x86)\Caphyon\Advanced Installer 18.3\bin\x86"
@set proj="C:\Advanced Installer\Projects\Common\Merge Modules\RISA Core Files\RISA Core Files.aip"
advancedInstaller /rebuild %proj%
@popd