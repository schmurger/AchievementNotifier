using AchievementNotifier.Parsers;
using AchievementNotifier.Parsers.Tenoke;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Graphics;
using WinRT.Interop;
using System.Diagnostics;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AchievementNotifier
{

    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private WindowWatcher windowWatcher;
        private static MainWindow mainWindow = null;

        public ObservableCollection<AchievementItem> GameAchievements = new ObservableCollection<AchievementItem>();
        public ObservableCollection<GameItem> Games = new ObservableCollection<GameItem>();
        public Dictionary<String, GameView> Storage = new Dictionary<String, GameView>();
        private string storageFile =System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AchievementNotifier", "achievements.dat");

        public MainWindow()
        {
            InitializeComponent();
            mainWindow = this;
            windowWatcher = new WindowWatcher();

            Center();
            LoadStorage();

           
        }


        private void TestNotificaton()
        {
            AchievementParser parser = new TenokeUserStatsParser();
            Achievement achievement = new Achievement();
            achievement.icon = "F:\\Games\\Dave The Diver\\icons\\02bdada9339c25640332b53188f1a6631fb515d2.jpg";
            achievement.name = "Test";
            achievement.description = "Description";
            parser.CreateNotification(achievement);

        }
            public static MainWindow getInstance()
        {
            return mainWindow;
        }

        private void SaveStorage()
        {
            FileOperations.WriteToFile(new List<String>(Storage.Keys), storageFile);
        }

        private void LoadStorage()
        {
            foreach (string line in FileOperations.readFile(storageFile))
            {
                EmuDetector.getInstance().DetectEmu(line);
            };
        }

        public void Add(GameItem gameItem, List<AchievementItem> achievementItems)
        {
            if (Storage.ContainsKey(gameItem.id)) return;

            Storage.Add(gameItem.id, new GameView(gameItem, achievementItems));
            Games.Add(gameItem);
            SaveStorage();
        }
        
        private void GameNavigation_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.InvokedItemContainer.AccessKey)) { 
                GameAchievements.Clear();
                Storage.GetValueOrDefault(args.InvokedItemContainer.AccessKey).achievementItems.ForEach(GameAchievements.Add);
            }
        }

        public void UpdateAchievement(String id, Achievement achievement)
        {
            if (!Storage.ContainsKey(id)) return;

            AchievementItem achievementItem = Storage.GetValueOrDefault(id).achievementItems.Find(a => a.id == achievement.id);

            if (achievementItem != null)
            {
                if (achievement.progressMax > 0 && achievement.progressMin > 0)
                {
                    achievementItem.percentage = (int)(achievement.progressMax / achievement.progressMin * 100);
                    achievementItem.percentageText = $"{achievementItem.percentage}%";
                    achievementItem.progress = $"{(int)achievement.progressMin}/{(int)achievement.progressMax}";
                    achievementItem.progressVisible = true;
                }

                if (achievement.achieved)
                {
                    achievementItem.icon = achievement.icon;
                    achievementItem.achievedAt = DateTimeOffset.FromUnixTimeSeconds(achievement.timestamp).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                }
                SaveStorage();
            }
        }

        private void Center()
        {
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);

            if (AppWindow.GetFromWindowId(windowId) is AppWindow appWindow &&
                DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Nearest) is DisplayArea displayArea)
            {
                SetTitleBarColors(appWindow);
                appWindow.Resize(new SizeInt32((int)(displayArea.WorkArea.Width * 0.3), (int)(displayArea.WorkArea.Height * 0.70)));
                appWindow.Title = "Achievement Notifier";
                PointInt32 CenteredPosition = appWindow.Position;
                
                CenteredPosition.X = (displayArea.WorkArea.Width - appWindow.Size.Width) / 2;
                CenteredPosition.Y = (displayArea.WorkArea.Height - appWindow.Size.Height) / 2;
                

                appWindow.Move(CenteredPosition);
        

                
            }

        }

        private bool SetTitleBarColors(AppWindow appWindow)
        {
            // Check to see if customization is supported.
            // The method returns true on Windows 10 since Windows App SDK 1.2,
            // and on all versions of Windows App SDK on Windows 11.
            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                AppWindowTitleBar m_TitleBar = appWindow.TitleBar;
                appWindow.SetIcon(@"Assets\trophy.ico");
                appWindow.Title = "Achievement Notifier";
                // Set active window colors.
                // Note: No effect when app is running on Windows 10
                // because color customization is not supported.
                m_TitleBar.ForegroundColor = Colors.White;
                m_TitleBar.BackgroundColor = Colors.Black;
                m_TitleBar.ButtonForegroundColor = Colors.White;
                m_TitleBar.ButtonBackgroundColor = Colors.Black;
                m_TitleBar.ButtonHoverForegroundColor = Colors.LightGray;
                m_TitleBar.ButtonHoverBackgroundColor = Colors.DarkGray;
                m_TitleBar.ButtonPressedForegroundColor = Colors.Gray;
                m_TitleBar.ButtonPressedBackgroundColor = Colors.Black;

                // Set inactive window colors.
                // Note: No effect when app is running on Windows 10
                // because color customization is not supported.
                m_TitleBar.InactiveForegroundColor = Colors.DarkGray;
                m_TitleBar.InactiveBackgroundColor = Colors.DarkGray;
                m_TitleBar.ButtonInactiveForegroundColor = Colors.DarkGray;
                m_TitleBar.ButtonInactiveBackgroundColor = Colors.DarkGray;
                return true;
            }
            return false;
        }
    }
}
