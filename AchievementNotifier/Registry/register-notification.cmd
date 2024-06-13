@echo off
reg add "HKEY_CLASSES_ROOT\AppUserModelId\AchievementNotifier" /f
reg add "HKEY_CLASSES_ROOT\AppUserModelId\AchievementNotifier" /v DisplayName /t REG_SZ /d "Achievement Unlocked!" /f
reg add "HKEY_CLASSES_ROOT\AppUserModelId\AchievementNotifier" /v IconUri /t REG_SZ /d "%cd%\Assets\trophy.ico" /f
reg add "HKEY_CLASSES_ROOT\AppUserModelId\AchievementNotifier" /v IconBackgroundColor /t REG_SZ /d "FFDDDDDD" /f
reg add "HKEY_CLASSES_ROOT\AppUserModelId\AchievementNotifier" /v ShowInSettings /t REG_DWORD /d 1 /f

