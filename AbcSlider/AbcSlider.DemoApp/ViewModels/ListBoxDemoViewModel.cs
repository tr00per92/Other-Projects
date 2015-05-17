namespace AbcSlider.DemoApp.ViewModels
{
    using System.Collections.Generic;

    public class ListBoxDemoViewModel : DemoViewModelBase
    {
        private const int ItemsCount = 10;

        public ListBoxDemoViewModel()
        {
            this.Minimum = 0;
            this.Value = 0;
            this.Maximum = ItemsCount - 1;
            this.Items = GenerateDummyItems(ItemsCount);
        }

        public IEnumerable<string> Items { get; set; }

        private static IEnumerable<string> GenerateDummyItems(int count)
        {
            var items = new HashSet<string>();
            for (var i = 1; i <= count; i++)
            {
                items.Add("Item " + i);
            }

            return items;
        } 
    }
}
