namespace AbcSlider.DemoApp.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;

    public class TemplatesDemoViewModel : DemoViewModelBase
    {
        private string selectedStyle;

        public TemplatesDemoViewModel()
        {
            this.Minimum = -200;
            this.Value = 0;
            this.Maximum = 200;
            this.Styles = new[] { "Default", "BlueGreenSlider", "RedMagentaSlider", "OrangeGreenSlider" };
            this.SelectedStyle = this.Styles.FirstOrDefault();
        }

        public IEnumerable<string> Styles { get; set; }

        public string SelectedStyle
        {
            get
            {
                return this.selectedStyle;
            }
            set
            {
                this.selectedStyle = value;
                this.OnPropertyChanged(() => this.SelectedStyle);
            }
        }
    }
}
