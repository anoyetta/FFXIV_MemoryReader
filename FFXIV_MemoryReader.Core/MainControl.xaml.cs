using System.Windows.Controls;

namespace TamanegiMage.FFXIV_MemoryReader.Core
{
    /// <summary>
    /// MainControl.xaml の相互作用ロジック
    /// </summary>
    public partial class MainControl : UserControl
    {
        public MainControl()
        {
            InitializeComponent();
        }


        private void Button_ShowLiveLog_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("powershell.exe",
                    @"Get-ChildItem $env:APPDATA\TamanegiMage | Where-Object {$_.Name -match '^FFXIV_MemoryReader-.*\.log$'} | Sort-Object -Descending | Select-Object -First 1 | Get-Content -Wait -tail 20");
            }
            catch
            {
            }
        }

        private void Button_OpenLogDirectory_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                string appData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
                System.Diagnostics.Process.Start(System.IO.Path.Combine(appData, @"TamanegiMage"));
            }
            catch
            {
            }
        }
    }
}
