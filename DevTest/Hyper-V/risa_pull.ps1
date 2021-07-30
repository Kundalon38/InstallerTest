md -Force "$env:USERPROFILE\Desktop\Pull"
md -Force "$env:USERPROFILE\Desktop\Pull\Registry"
md -Force "$env:USERPROFILE\Desktop\Pull\Documents"
md -Force "$env:USERPROFILE\Desktop\Pull\ProgramFiles"
md -Force "$env:USERPROFILE\Desktop\Pull\ProgramFilesDemo"
md -Force "$env:USERPROFILE\Desktop\Pull\ProgramData"

reg export "HKLM\SOFTWARE\RISA Technologies" "$env:USERPROFILE\Desktop\Pull\Registry\HKLM.reg" /y
reg export "HKCU\Software\RISA Technologies" "$env:USERPROFILE\Desktop\Pull\Registry\HKCU.reg" /y
reg export "HKCR\.asc" "$env:USERPROFILE\Desktop\Pull\Registry\ext_asc.reg" /y
reg export "HKCR\.fil" "$env:USERPROFILE\Desktop\Pull\Registry\ext_fil.reg" /y
reg export "HKCR\.r3d" "$env:USERPROFILE\Desktop\Pull\Registry\ext_r3d.reg" /y
reg export "HKCR\.rt3" "$env:USERPROFILE\Desktop\Pull\Registry\ext_rt3.reg" /y
reg export "HKCR\.rfl" "$env:USERPROFILE\Desktop\Pull\Registry\ext_rfl.reg" /y
reg export "HKCR\.fnd" "$env:USERPROFILE\Desktop\Pull\Registry\ext_fnd.reg" /y
reg export "HKCR\.rcn" "$env:USERPROFILE\Desktop\Pull\Registry\ext_rcn.reg" /y
reg export "HKCR\RISADesignList" "$env:USERPROFILE\Desktop\Pull\Registry\RISADesignList.reg" /y
reg export "HKCR\RISADatabase" "$env:USERPROFILE\Desktop\Pull\Registry\RISADatabase.reg" /y
reg export "HKCR\RISA-3DModel" "$env:USERPROFILE\Desktop\Pull\Registry\RISA3DModel.reg" /y
reg export "HKCR\RISAFloorModel" "$env:USERPROFILE\Desktop\Pull\Registry\RISAFloorModel.reg" /y
reg export "HKCR\RISAFoundationModel" "$env:USERPROFILE\Desktop\Pull\Registry\RISAFoundationModel.reg" /y
reg export "HKCR\RISAConnectionModel" "$env:USERPROFILE\Desktop\Pull\Registry\RISAConnectionModel.reg" /y
reg export "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths" "$env:USERPROFILE\Desktop\Pull\Registry\AppPaths.reg" /y


if (Test-Path -Path "$env:USERPROFILE\Desktop\Pull\Documents") {
    Copy-Item -Path "$env:USERPROFILE\Documents\RISA\*" -Destination "$env:USERPROFILE\Desktop\Pull\Documents" -PassThru -Recurse
}
if (Test-Path -Path "C:\Program Files\RISA") {
    Copy-Item -Path "C:\Program Files\RISA\*" -Destination "$env:USERPROFILE\Desktop\Pull\ProgramFiles" -PassThru -Recurse
}
if (Test-Path -Path "C:\Program Files\RISADemo") {
    Copy-Item -Path "C:\Program Files\RISADemo\*" -Destination "$env:USERPROFILE\Desktop\Pull\ProgramFilesDemo" -PassThru -Recurse
}
if (Test-Path -Path "C:\ProgramData\RISA") {
    Copy-Item -Path "C:\ProgramData\RISA\*" -Destination "$env:USERPROFILE\Desktop\Pull\ProgramData" -PassThru -Recurse
}
