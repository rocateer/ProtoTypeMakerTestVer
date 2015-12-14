using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Linq;

namespace ProtoTypeMaker.Commons
{
    /// <summary>
    /// AddItems.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AddItems : UserControl
    {

        private List<ChildrenList> _hasChildren = new List<ChildrenList>();

        public List<ChildrenList> HasChildren
        {
            get { return _hasChildren; }
            set { _hasChildren = value; }
        }

        public string strPath = "";
        
        public AddItems()
        {
            InitializeComponent();
        }

        public void SetSource(string str)
        {
            BitmapImage logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource = new Uri(str);
            logo.EndInit();


            img_Item.Source = logo;
            strPath = str;
        }




        public void ChildrenAdd(AddItems item){
            ChildrenList itemChild = new ChildrenList();
            if (HasChildren.Count == 0)
            {
                itemChild.Seq = 0;
            }
            else
            {
                itemChild.Seq = HasChildren.Max(a => a.Seq) + 1;
            }
            itemChild.Child = item;
            HasChildren.Add(itemChild);
        }
        public void ChildrenRemove(AddItems item)
        {
            ChildrenList selectItem = HasChildren.Where(a => a.Child == item).FirstOrDefault();

            if (selectItem != null)
            {
                HasChildren.Remove(selectItem);
            }
        }
    }


    public class ChildrenList
    {
        private int _seq;

        public int Seq
        {
            get { return _seq; }
            set { _seq = value; }
        }

        private AddItems _child;

        public AddItems Child
        {
            get { return _child; }
            set { _child = value; }
        }
        
        
    }
}
