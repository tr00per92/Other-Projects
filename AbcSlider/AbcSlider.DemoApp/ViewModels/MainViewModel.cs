namespace AbcSlider.DemoApp.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Practices.Prism.Commands;
    using Microsoft.Practices.Prism.Mvvm;

    public class MainViewModel : BindableBase
    {
        private const string DefaultViewModel = "StackPanelDemoViewModel";
        private readonly IDictionary<string, DemoViewModelBase> viewModels;
        private DemoViewModelBase currentViewModel;

        public MainViewModel()
        {
            this.viewModels = new Dictionary<string, DemoViewModelBase>();
            this.ChangeCurrentViewModel(DefaultViewModel);
            this.ChangeCurrentViewModelCommand = new DelegateCommand<string>(this.ChangeCurrentViewModel);
        }

        public DemoViewModelBase CurrentViewModel
        {
            get
            {
                return currentViewModel;
            }
            set
            {
                currentViewModel = value;
                this.OnPropertyChanged(() => this.CurrentViewModel);
            }
        }

        public DelegateCommand<string> ChangeCurrentViewModelCommand { get; set; }

        private void ChangeCurrentViewModel(string viewModelName)
        {
            if (!this.viewModels.ContainsKey(viewModelName))
            {
                this.CreateViewModelInstance(viewModelName);
            }

            this.CurrentViewModel = this.viewModels[viewModelName];
        }

        private void CreateViewModelInstance(string viewModelName)
        {
            switch (viewModelName)
            {
                case "StackPanelDemoViewModel":
                    this.viewModels[viewModelName] = new StackPanelDemoViewModel();
                    break;
                case "GridImageDemoViewModel":
                    this.viewModels[viewModelName] = new GridImageDemoViewModel();
                    break;
                case "ListBoxDemoViewModel":
                    this.viewModels[viewModelName] = new ListBoxDemoViewModel();
                    break;
                case "TemplatesDemoViewModel":
                    this.viewModels[viewModelName] = new TemplatesDemoViewModel();
                    break;
                default:
                    throw new ArgumentException("Unsupported viewmodel type", "viewModelName");
            }
        }
    }
}
