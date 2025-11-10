using System.Collections.ObjectModel;

namespace VistaControls
{
    public class CascaderOption
    {
        public string Label { get; set; } = string.Empty;
        public object? Value { get; set; }
        public bool Disabled { get; set; }
        public ObservableCollection<CascaderOption> Children { get; set; } = new ObservableCollection<CascaderOption>();

        public override string ToString()
        {
            return Label;
        }
    }
}


