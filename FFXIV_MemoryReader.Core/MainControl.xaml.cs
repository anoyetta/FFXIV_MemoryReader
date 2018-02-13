using System.Windows.Controls;

namespace TamanegiMage.FFXIV_MemoryReader.Core
{
    /// <summary>
    /// MainControl.xaml の相互作用ロジック
    /// </summary>
    public partial class MainControl : UserControl
    {
        internal BindingData BindingData { get; set; } = new BindingData();

        public MainControl()
        {
            InitializeComponent();
            this.DataContext = BindingData;
        }
    }

    internal class BindingData
    {
        internal int ProcessId { get; set; } = 0;

        internal void ClearAll()
        {
            this.ProcessId = 0;
        }
    }
}
