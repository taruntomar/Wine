using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
namespace Wine.TreeViewModel
{
    public class HierarchicalObjectViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Tag { get; set; }
        public Visibility TextBlockVisible { get; set; }
        public Visibility TextBoxVisible { get; set; }
        public ContextMenu ContextMenuObj { get; set; }
        public ObservableCollection<HierarchicalObjectViewModel> HierarchicalObjects { get; set; }

        public HierarchicalObjectViewModel()
        {
            HierarchicalObjects = new ObservableCollection<HierarchicalObjectViewModel>();
        }
        public IEnumerable Items
        {
            get
            {
                var items = new CompositeCollection();
                items.Add(new CollectionContainer { Collection = HierarchicalObjects });
                return items;
            }
        }
    }
}
