md -Force "$env:USERPROFILE\Desktop\Pull"
md -Force "$env:USERPROFILE\Desktop\Pull\Documents"
md -Force "$env:USERPROFILE\Desktop\Pull\ProgramFiles"
md -Force "$env:USERPROFILE\Desktop\Pull\ProgramFilesDemo"
md -Force "$env:USERPROFILE\Desktop\Pull\ProgramData"


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
