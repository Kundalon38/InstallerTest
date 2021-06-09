md -Force "$env:USERPROFILE\Desktop\Pull"
md -Force "$env:USERPROFILE\Desktop\Pull\Documents"
md -Force "$env:USERPROFILE\Desktop\Pull\ProgramFiles"
md -Force "$env:USERPROFILE\Desktop\Pull\ProgramData"
Copy-Item -Path "$env:USERPROFILE\Documents\RISA\*" -Destination "$env:USERPROFILE\Desktop\Pull\Documents" -PassThru -Recurse
Copy-Item -Path "C:\Program Files\RISA\*" -Destination "$env:USERPROFILE\Desktop\Pull\ProgramFiles" -PassThru -Recurse
Copy-Item -Path "C:\ProgramData\RISA\*" -Destination "$env:USERPROFILE\Desktop\Pull\ProgramData" -PassThru -Recurse