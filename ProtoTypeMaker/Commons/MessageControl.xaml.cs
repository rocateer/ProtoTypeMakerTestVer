using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProtoTypeMaker
{
	/// <summary>
	/// MessageControl.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class MessageControl : UserControl
	{
		public MessageControl()
		{
			this.InitializeComponent();
		}


        public String GetMessage()
        {

            return txt_Message.Text;
        }

        public void SetMessage(String str)
        {
            txt_Message.Text = str;
        }
	}
}