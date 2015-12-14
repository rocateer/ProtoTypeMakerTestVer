using Microsoft.Win32;
using ProtoTypeMaker.Commons;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;

namespace ProtoTypeMaker
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        ScaleTransform scale = new ScaleTransform();
        TranslateTransform trans = new TranslateTransform();

        private List<InkItemList> _itemLIst = new List<InkItemList>();

        public List<InkItemList> ItemList
        {
            get { return _itemLIst; }
            set { _itemLIst  = value; }
        }

        private List<ColorInfoItem> _colorList = new List<ColorInfoItem>();

        public List<ColorInfoItem> ColorLIst
        {
            get { return _colorList; }
            set { _colorList = value; }
        }


        public MainWindow()
        {
            InitializeComponent();

            this.Drop += MainWindow_Drop;
            this.DragEnter += MainWindow_DragEnter;
            //icv_INk.MouseWheel += MainWindow_MouseWheel;
            sv_Main.PreviewMouseWheel += MainWindow_MouseWheel;
            //sv_Main.MouseWheel += MainWindow_MouseWheel;
            scale = new ScaleTransform(1.0, 1.0,0.5,0.5);
            trans = new TranslateTransform(0, 0);

            TransformGroup group = new TransformGroup();
            group.Children.Add(scale);
          //  group.Children.Add(trans);
            gd_Main.LayoutTransform = group;

            sv_Main.MouseRightButtonDown += MainWindow_MouseRightButtonDown;
            sv_Main.MouseMove += MainWindow_MouseMove;
            sv_Main.MouseRightButtonUp += MainWindow_MouseRightButtonUp;
            InkItemList item = new InkItemList();
            item.Seq = 0;
            item.SelVal = InkCanvasEditingMode.Ink;
            item.Display = "Draw";
            ItemList.Add(item);

            item = new InkItemList();
            item.Seq = 1;
            item.SelVal = InkCanvasEditingMode.EraseByStroke;
            item.Display = "Remove";
            ItemList.Add(item);


            cb_Select.ItemsSource = ItemList;

            cb_Select.DisplayMemberPath = "Display";
            cb_Select.SelectedValuePath = "SelVal";
            cb_Select.SelectedIndex = 0;


            ColorInfoItem colorItem = new ColorInfoItem();

            colorItem.Display = "Red";
            colorItem.SelColor = new SolidColorBrush(Colors.Red);
            colorItem.SelValue = new System.Windows.Ink.DrawingAttributes() { Color = Colors.Red, FitToCurve = true, Height = 25, Width = 25 };
            ColorLIst.Add(colorItem);

            colorItem = new ColorInfoItem();

            colorItem.Display = "Yellow";
            colorItem.SelColor = new SolidColorBrush(Colors.Yellow);
            colorItem.SelValue = new System.Windows.Ink.DrawingAttributes() { Color = Colors.Yellow, FitToCurve = true, Height = 25, Width = 25 };
            ColorLIst.Add(colorItem);

            colorItem = new ColorInfoItem();

            colorItem.Display = "Green";
            colorItem.SelColor = new SolidColorBrush(Colors.Green);
            colorItem.SelValue = new System.Windows.Ink.DrawingAttributes() { Color = Colors.Green, FitToCurve = true, Height = 25, Width = 25 };
            ColorLIst.Add(colorItem);

            colorItem = new ColorInfoItem();

            colorItem.Display = "Blue";
            colorItem.SelColor = new SolidColorBrush(Colors.Blue);
            colorItem.SelValue = new System.Windows.Ink.DrawingAttributes() { Color = Colors.Blue, FitToCurve = true, Height = 25, Width = 25 };
            ColorLIst.Add(colorItem);

            btn_Message.Click += btn_Message_Click;

            lb_ItemBox.ItemsSource = ColorLIst;

            lb_ItemBox.SelectedValuePath = "SelValue";

            lb_ItemBox.SelectedIndex = 0;

            btn_Save.Click += btn_Save_Click;


            btn_Remove.Click += btn_Remove_Click;
            btn_Load.Click += btn_Load_Click;


            btn_SaveXML.Click += btn_SaveXML_Click;
            btn_New.Click += btn_New_Click;
          //  icv_INk.EditingMode = InkCanvasEditingMode
        }

        void btn_New_Click(object sender, RoutedEventArgs e)
        {
            cv_ItemsContainer.Children.Clear();
            icv_INk.Strokes = new StrokeCollection();

        }

        void MainWindow_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            windowMove = false;
        
        }

        void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (windowMove)
            {
                Point newPoint =
                e.GetPosition(this);

               
                 
                //Point point = new Point(0.5,0.5);
                
                //gd_Main.RenderTransformOrigin = new Point(0, 0);

                sv_Main.ScrollToVerticalOffset(sv_Main.VerticalOffset - (newPoint.Y - oldWPoint.Y));
                sv_Main.ScrollToHorizontalOffset(sv_Main.HorizontalOffset - (newPoint.X- oldWPoint.X));
                //trans.X = trans.X+ (newPoint.X -oldWPoint.X) ;
                //trans.Y =trans.Y+(newPoint.Y -oldWPoint.Y)  ;

                oldWPoint = newPoint;
            }
        }

        bool windowMove = false;
        Point oldWPoint = new Point();
        void MainWindow_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            windowMove = true;

            //gd_Main.RenderTransformOrigin = new Point(0,0);
            oldWPoint = e.GetPosition(this);
        }

        void btn_SaveXML_Click(object sender, RoutedEventArgs e)
        {
            SetSaveXML();
        }

        async void btn_Load_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog opne = new OpenFileDialog();

            opne.Filter = "XML|*.xml";

            if (opne.ShowDialog() == true)
            {
               await LoadFile(opne.FileName);
            }
        }

        void btn_Remove_Click(object sender, RoutedEventArgs e)
        {
            if (oldItem != null)
            {

                cv_ItemsContainer.Children.Remove(oldItem);
                oldItem = null;
            }
        }

        void btn_Message_Click(object sender, RoutedEventArgs e)
        {
            MessageControl message = new MessageControl();
            message.Width = 200;
            message.Height = 300;
            message.MouseLeftButtonDown += message_MouseLeftButtonDown;
            message.MouseMove += message_MouseMove;
            message.MouseLeftButtonUp += message_MouseLeftButtonUp;
            message.GotFocus += message_GotFocus;
            cv_ItemsContainer.Children.Add(message);


            Canvas.SetLeft(message, 0);
            Canvas.SetTop(message, 0);
        }

        bool messageMove = false;
        void message_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            messageMove = false;
        }

        void message_MouseMove(object sender, MouseEventArgs e)
        {
            if (messageMove && oldItem == sender as MessageControl)
            {

                double xPosition1 = e.GetPosition(cv_ItemsContainer).X;
                double yPosition1 = e.GetPosition(cv_ItemsContainer).Y;
                Canvas.SetLeft(
                (sender as MessageControl)
                , -oldPoint.X +
               xPosition1 + Canvas.GetLeft((sender as MessageControl)));
                Canvas.SetTop(
                (sender as MessageControl)
                , -oldPoint.Y +
               yPosition1 + Canvas.GetTop((sender as MessageControl)));

                oldPoint = e.GetPosition(cv_ItemsContainer);
                

                
            }
 
            
        }

        void message_GotFocus(object sender, RoutedEventArgs e)
        {
            oldItem = sender as MessageControl;
        }

        void message_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            oldItem = sender as MessageControl;
            messageMove = true;
            oldPoint = e.GetPosition(cv_ItemsContainer);
        }

        void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            Snapshot(gd_Main, scale.ScaleX, 16);
        }

        Point oldCenter = new Point(0, 0);

        void MainWindow_MouseWheel(object sender, MouseWheelEventArgs e)
        {
         
            if (e.Delta > 0)
            {

                if (scale.ScaleX < 4.9)
                {
                    
                    scale.ScaleX += 0.1;
                    scale.ScaleY += 0.1;
                }
            }
            else{

                if (scale.ScaleX > 0.3)
                {
                    
                     
                    scale.ScaleX -= 0.1;
                    scale.ScaleY -= 0.1;
                }
            }
             ;



             //sv_Main.ScrollToVerticalOffset(sv_Main.VerticalOffset -(oldCenter.Y - e.GetPosition(gd_Main).Y));
             //sv_Main.ScrollToHorizontalOffset(sv_Main.HorizontalOffset - (oldCenter.X - e.GetPosition(gd_Main).X));

             //oldCenter = e.GetPosition(gd_Main);
            //Point point = ScaleP(e.GetPosition(gd_Main), new Point(this.ActualWidth / 2, this.ActualHeight / 2), scale.ScaleX, scale.ScaleY);



            //sv_Main.ScrollToHorizontalOffset(point.X);
            //sv_Main.ScrollToVerticalOffset(point.Y);
            e.Handled = true;  
        }
        public Point ScaleP(Point sourcePoint, Point centerPoint, double scaleX, double scaleY)
        {

            Point targetPoint = new Point();



            targetPoint.X = scaleX * sourcePoint.X + (centerPoint.X - scaleX * centerPoint.X);

            targetPoint.Y = scaleY * sourcePoint.Y + (centerPoint.Y - scaleY * centerPoint.Y);



            return targetPoint;

        }

        void MainWindow_DragEnter(object sender, DragEventArgs e)
        {
            
        }

        void MainWindow_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                for (int i = 0; i < files.Count(); i++)
                {
                    if (files[i].IndexOf(".png") > 0 ||
                        files[i].IndexOf(".jpg") > 0 ||
                        files[i].IndexOf(".gif") > 0 ||
                        files[i].IndexOf(".jpeg") > 0
                        )
                    {
                        Point position = e.GetPosition(cv_ItemsContainer);

                        HandleFileOpen(files[i], 0, position);
                    }
                   
                }
            }
        }
        


        private void HandleFileOpen(string v , int type , Point position)
        {

            
            if (type == 0)
            {

                AddItems image = new AddItems();

                image.Width = 200;
                image.Height = 300;
                image.SetSource(v);

                image.MouseLeftButtonDown += image_MouseLeftButtonDown;
                image.MouseMove += image_MouseMove;
                image.MouseLeftButtonUp += image_MouseLeftButtonUp;
                cv_ItemsContainer.Children.Add(image);
                image.Tag = DateTime.Now.ToString("yyyyMMddhhmmssfff");
                Canvas.SetLeft(image, position.X);
                Canvas.SetTop(image, position.Y);
 
            }
      
            

        }

        void image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            imageMove = false;
        }

        void image_MouseMove(object sender, MouseEventArgs e)
        {
           // Debug.WriteLine(e.GetPosition(sender as AddItems).X +" "+e.GetPosition(cv_ItemsContainer).X);


            if (imageMove && oldItem == sender as AddItems)
            {
                

                double xPosition1 = e.GetPosition(cv_ItemsContainer).X;
                double yPosition1 = e.GetPosition(cv_ItemsContainer).Y;
                Canvas.SetLeft(
                (sender as AddItems)
                ,-oldPoint.X+
               xPosition1 + Canvas.GetLeft((sender as AddItems)));
                Canvas.SetTop(
                (sender as AddItems)
                ,-oldPoint.Y+
               yPosition1 + Canvas.GetTop((sender as AddItems)));

                oldPoint = e.GetPosition(cv_ItemsContainer);
                Debug.WriteLine(Canvas.GetLeft(sender as AddItems));
            }
            
        }

        UIElement oldItem = null;
        bool imageMove = false;
        Point oldPoint = new Point(0, 0);

        void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            oldItem = sender as AddItems;
            imageMove = true;
            oldPoint = e.GetPosition(cv_ItemsContainer);
        }




        private void Snapshot(UIElement source, double scale, int quality)
        {

            double _width = 0;
            double _height = 0;

            foreach (UIElement item in cv_ItemsContainer.Children)
            {
                if (_width < Canvas.GetLeft(item) + 200)
                {
                    _width = Canvas.GetLeft(item) + 200;
                }
                if (_height < Canvas.GetTop(item) + 300)
                {
                    _height = Canvas.GetTop(item) + 300;
                }
            }
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(

                        (int)(_width * scale), (int)(_height * scale),

                        96d, 96d, PixelFormats.Default);




            renderBitmap.Render(gd_Main);

           

            SaveFileDialog file = new SaveFileDialog();
            file.Filter = "JPG|*.jpg";
            if (file.ShowDialog() == true)
            {
                using (Stream stream = new FileStream(file.FileName,

                    FileMode.Create, FileAccess.Write, FileShare.None))
                {

                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();

                    encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

                    encoder.Save(stream);

                }
                //using (FileStream stm = File.Create(file.FileName))
                //    jpgEncoder.Save(stm);
            }




            //double actualHeight = _height;
            //double actualWidth = _width;





            //double renderHeight = actualHeight * scale;
            //double renderWidth = actualWidth * scale;

            //RenderTargetBitmap renderTarget = new RenderTargetBitmap((int)renderWidth, (int)renderHeight, 96, 96, PixelFormats.Pbgra32);
            //VisualBrush sourceBrush = new VisualBrush(source);

            //DrawingVisual drawingVisual = new DrawingVisual();
            //DrawingContext drawingContext = drawingVisual.RenderOpen();

            //using (drawingContext)
            //{
            //    //drawingContext.PushTransform(new ScaleTransform(scale, scale));
            //    drawingContext.DrawRectangle(sourceBrush, null, new Rect(new Point(0, 0), new Point(actualWidth, actualHeight)));
            //}
            //renderTarget.Render(drawingVisual);

            
        }




        void SetSaveXML()
        {

           


            XmlDocument dom = new XmlDocument();
            XmlElement x;

            XmlComment dec = dom.CreateComment("This is data of all Employees");

            dom.AppendChild(dec);
            x = dom.CreateElement("Body");

            dom.AppendChild(x);
            foreach (object item in cv_ItemsContainer.Children)
            {
                if (item is MessageControl)
                {

                   
// GeneralTransform generalTransform1 = (item as MessageControl).TransformToAncestor(this);

                    // Retrieve the point value relative to the parent.
                    Point currentPoint = new Point(Canvas.GetLeft(item as MessageControl),Canvas.GetTop(item as MessageControl));




                    XmlElement x1 = dom.CreateElement("Control");
                    XmlElement xName = dom.CreateElement("LocationX");
                    xName.InnerText = (currentPoint.X ).ToString();
                    x1.AppendChild(xName);

                    XmlElement x11 = dom.CreateElement("LocationY");
                    x11.InnerText = (currentPoint.Y ).ToString();
                    x1.AppendChild(x11);
                    XmlElement x13 = dom.CreateElement("Message");
                    x13.InnerText = (item as MessageControl).GetMessage();
                    x1.AppendChild(x13);
                    XmlElement x12 = dom.CreateElement("Type");
                    x12.InnerText = "MessageControl";
                    x1.AppendChild(x12);
                    x.AppendChild(x1);
                }
                else if (item is AddItems)
                {
                   // GeneralTransform generalTransform1 = (item as AddItems).TransformToAncestor(this);

                    // Retrieve the point value relative to the parent.
                    Point currentPoint = new Point(Canvas.GetLeft(item as AddItems), Canvas.GetTop(item as AddItems));



                    XmlElement x1 = dom.CreateElement("Control");
                    XmlElement xName = dom.CreateElement("LocationX");
                    xName.InnerText = (currentPoint.X).ToString();
                    x1.AppendChild(xName);

                    XmlElement x11 = dom.CreateElement("LocationY");
                    x11.InnerText = (currentPoint.Y).ToString();
                    x1.AppendChild(x11);
                    XmlElement x13 = dom.CreateElement("Image");
                    x13.InnerText = (item as AddItems).strPath;
                    x1.AppendChild(x13);

                    XmlElement x12 = dom.CreateElement("Type");
                    x12.InnerText = "AddItems";
                    x1.AppendChild(x12);
                    x.AppendChild(x1);
                }                
            }


            if (icv_INk.Strokes.Count() > 0)
            {
                var xmlElements =
                      new XElement("Strokes", this.icv_INk.Strokes.Select(stroke =>
                              new XElement("Stroke",
                                  new XElement("Color",
                                      new XAttribute("A", stroke.DrawingAttributes.Color.A),
                                      new XAttribute("R", stroke.DrawingAttributes.Color.R),
                                      new XAttribute("G", stroke.DrawingAttributes.Color.G),
                                      new XAttribute("B", stroke.DrawingAttributes.Color.B)
                                  ),
                                  new XElement("Points", stroke.StylusPoints
                                      .Select(point =>
                                          new XElement("Point",
                                              new XAttribute("X", point.X),
                                              new XAttribute("Y", point.Y),
                                              new XAttribute("PressureFactor", point.PressureFactor)
                                          )
                                      )
                                  ),
                                  new XElement("Width", stroke.DrawingAttributes.Width),
                                  new XElement("Height", stroke.DrawingAttributes.Height)
                              )
                          )
                      );
                //xmlElements.ToString();
                if (xmlElements != null)
                {

                    XmlElement el = dom.CreateElement("Path"); 
                    el.InnerXml =
                    ToXmlElement(xmlElements).InnerXml;
                    x.AppendChild(el);
                }
            }

               SaveFileDialog file = new SaveFileDialog();
            file.Filter = "XML|*.xml";
            if (file.ShowDialog() == true)
            {
                using (FileStream stm = File.Create(file.FileName)){
                   dom.Save(stm);


                }

                 
            }

          
        }

        
        public  XmlElement ToXmlElement(XElement el ) 
        { 
            var doc = new XmlDocument(); 
            doc.Load(el.CreateReader()); 
             return doc.DocumentElement; 
         } 



        public async Task<object> LoadFile(string FileName)
        {
            cv_ItemsContainer.Children.Clear();
            XmlDocument dom = new XmlDocument();
            try
            {

                
                   Stream st = File.OpenRead(FileName);
                    XDocument data = XDocument.Load(st);
                   

                

                UIElement itemList = new UIElement();
                    foreach (var item in data.Descendants("Control"))
                    {
                        if (item.Element("Type").Value == "MessageControl")
                        {
                            MessageControl control = new MessageControl();
                            control.SetMessage(item.Element("Message").Value);
                            control.Width = 200;
                            control.Height = 300;
                            control.MouseLeftButtonDown += message_MouseLeftButtonDown;
                            control.MouseMove += message_MouseMove;
                            control.MouseLeftButtonUp += message_MouseLeftButtonUp;
                            cv_ItemsContainer.Children.Add(control);
                            Canvas.SetLeft(control, Convert.ToDouble(item.Element("LocationX").Value));
                            Canvas.SetTop(control, Convert.ToDouble(item.Element("LocationY").Value));

                        }
                        else if (item.Element("Type").Value == "AddItems")
                        {
                            AddItems control = new AddItems();

                            control.Width = 200;
                            control.Height = 300;
                            control.SetSource(item.Element("Image").Value);
                            control.MouseLeftButtonDown += image_MouseLeftButtonDown;
                            control.MouseMove += image_MouseMove;
                            control.MouseLeftButtonUp += image_MouseLeftButtonUp;
                            cv_ItemsContainer.Children.Add(control);
                            Canvas.SetLeft(control, Convert.ToDouble(item.Element("LocationX").Value));
                            Canvas.SetTop(control, Convert.ToDouble(item.Element("LocationY").Value));
                        }
                        else
                        {

                           

                        }


                    }

                    foreach (var item in data.Descendants("Path"))
                    {
                        icv_INk.Strokes = CreateStrokeCollectionfromXML(item.ToString());
                    }

                    return itemList;
                }

            catch{
                  
                return new object();
            
            }
        
          
        }



        private StrokeCollection CreateStrokeCollectionfromXML(string XMLStrokes)
        {
            XElement xmlElem;
            try
            {
                xmlElem = XElement.Parse(XMLStrokes);
            }
            catch (XmlException ex)
            {
                return new StrokeCollection();
            }
            StrokeCollection objStrokes = new StrokeCollection();
            //Query the XML to extract the Strokes
            var strokes = from s in xmlElem.Descendants("Stroke") select s;
            foreach (XElement strokeNodeElement in strokes)
            {
                var color = (from c in strokeNodeElement.Descendants("Color")
                             select c).FirstOrDefault();
                DrawingAttributes attributes = new DrawingAttributes();

                byte colorA = Convert.ToByte(color.Attribute("A").Value);
                byte colorR = Convert.ToByte(color.Attribute("R").Value);
                byte colorG = Convert.ToByte(color.Attribute("G").Value);
                byte colorB = Convert.ToByte(color.Attribute("B").Value);

                attributes.Color = Color.FromArgb(colorA, colorR, colorG, colorB);

                //var outlineColor = (from oc in strokeNodeElement.Descendants("OutlineColor")
                //                    select oc).FirstOrDefault();

                //byte outlineColorA = Convert.ToByte(outlineColor.Attribute("A").Value);
                //byte outlineColorR = Convert.ToByte(outlineColor.Attribute("R").Value);
                //byte outlineColorG = Convert.ToByte(outlineColor.Attribute("G").Value);
                //byte outlineColorB = Convert.ToByte(outlineColor.Attribute("B").Value);

                //attributes.OutlineColor = Color.FromArgb(outlineColorA, outlineColorR, outlineColorG, outlineColorB);

                attributes.Width = Convert.ToInt32(strokeNodeElement.Descendants("Width").FirstOrDefault().Value);
                attributes.Height = Convert.ToInt32(strokeNodeElement.Descendants("Height").FirstOrDefault().Value);

                var points = from p in strokeNodeElement.Descendants("Point")
                             select p;
                StylusPointCollection pointData = new System.Windows.Input.StylusPointCollection();

                foreach (XElement point in points)
                {
                    double Xvalue = Convert.ToDouble(point.Attribute("X").Value);
                    double Yvalue = Convert.ToDouble(point.Attribute("Y").Value);
                    pointData.Add(new StylusPoint(Xvalue, Yvalue));
                }
                StylusPointCollection col = new StylusPointCollection();
                col.Add(pointData);
                Stroke newstroke = new Stroke(col);
                newstroke.DrawingAttributes = attributes;
               // newstroke.StylusPoints.Add(pointData);
                //add the new stroke to the StrokeCollection
                objStrokes.Add(newstroke);
            }
            return objStrokes;
        }
    }

    public class InkItemList
    {
        private int _seq;

        public int Seq
        {
            get { return _seq; }
            set { _seq = value; }
        }

        private InkCanvasEditingMode _selVal;

        public InkCanvasEditingMode SelVal
        {
            get { return _selVal; }
            set { _selVal = value; }
        }

        private String _displayValue;

        public String Display
        {
            get { return _displayValue; }
            set { _displayValue = value; }
        }
        
        
        
    }

    public class ColorInfoItem
    {
        private SolidColorBrush _selColor;

        public SolidColorBrush SelColor
        {
            get { return _selColor; }
            set { _selColor = value; }
        }

        private String  _display;

        public String  Display
        {
            get { return _display; }
            set { _display = value; }
        }

        private 
            System.Windows.Ink.DrawingAttributes _selValue;

        public
            System.Windows.Ink.DrawingAttributes SelValue
        {
            get { return _selValue; }
            set { _selValue = value; }
        }
        
    }
}
