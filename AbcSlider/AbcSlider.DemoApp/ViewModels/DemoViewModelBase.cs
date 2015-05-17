namespace AbcSlider.DemoApp.ViewModels
{
    using Microsoft.Practices.Prism.Mvvm;

    public abstract class DemoViewModelBase : BindableBase
    {
        protected double minimum;
        protected double value;
        protected double maximum;

        public double Minimum
        {
            get
            {
                return this.minimum;
            }
            set
            {
                this.minimum = value;
                this.OnPropertyChanged(() => this.Minimum);
            }
        }

        public double Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
                this.OnPropertyChanged(() => this.Value);
            }
        }

        public double Maximum
        {
            get
            {
                return this.maximum;
            }
            set
            {
                this.maximum = value;
                this.OnPropertyChanged(() => this.Maximum);
            }
        }
    }
}
