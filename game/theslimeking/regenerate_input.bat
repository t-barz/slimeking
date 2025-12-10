@echo off
cd /d g:\GameDev\slimeking\game\theslimeking
"C:\Program Files\Unity\Hub\Editor\2022.3.18f1\Editor\Unity.exe" -projectPath . -executeMethod "UnityEditor.AssetDatabase.ImportAsset" "Assets/Settings/InputSystem_Actions.inputactions" -quit -batchmode -nographics -logFile -
