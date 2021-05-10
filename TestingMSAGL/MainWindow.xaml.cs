using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.WpfGraphControl;
using Microsoft.Win32;
using TestingMSAGL.DataLinker;
using TestingMSAGL.DataStructure;
using Color = Microsoft.Msagl.Drawing.Color;
using Point = Microsoft.Msagl.Core.Geometry.Point;

namespace TestingMSAGL
{
    /// <summary>
    ///     Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly GraphViewer _graphViewer = new GraphViewer();
        private Point _mMouseRightButtonDownPoint;
        private Point _mMouseLeftButtonDownPoint;
        
        public MainWindow()
        {
            InitializeComponent();
            Loaded += InitGraph;
            viewerPanel.ClipToBounds = true;
            _graphViewer.BindToPanel(viewerPanel);
            Graph.LayoutAlgorithmSettings.EdgeRoutingSettings.RouteMultiEdgesAsBundles = true;
            _graphViewer.LayoutEditingEnabled = true;
            //todo graphViewer props for pane moving
            _graphViewer.Graph = Graph;
            NodeTreeView();
            
        }

        private void NodeTreeView()
        {
            MenuItem root = new MenuItem() {Title = "Menu"};
            MenuItem childItem1 = new MenuItem() {Title = "Child item #1"};
            childItem1.Items.Add(new MenuItem() {Title = "Child item #1.1"});
            childItem1.Items.Add(new MenuItem() {Title = "Child item #1.2"});
            root.Items.Add(childItem1);
            root.Items.Add(new MenuItem() {Title = "Child item #2"});
            NodeTree.Items.Add(root);
        }

        public class MenuItem
        {
            public MenuItem()
            {
                this.Items = new ObservableCollection<MenuItem>();
            }

            public string Title { get; set; }

            public ObservableCollection<MenuItem> Items { get; set; }
        }

        private GraphExtension Graph { get; } = new GraphExtension("root", "0");
        private int NodeCounter { get; set; }
        private int SubGraphCounter { get; set; }

        private void InitGraph(object sender, RoutedEventArgs e)
        {
            // Drawing Board
            Subgraph rootSubgraph = new Subgraph("rootSubgraph");
            
            
            // first element 
            var root = new NodeComplex(Graph, "Root");
            rootSubgraph.AddSubgraph(root.Subgraph);

            var ros = new NodeComplex(Graph, "Root Of Subgraph");
            root.AddMember(ros);

            var c1 = new NodeElementary(Graph, "Complex: 104");
            var c2 = new NodeComplex(Graph, "Complex: 105");
            var c3 = new NodeComplex(Graph, "Complex: 106");
            ros.AddMember(c1); // layout Problem.
            ros.AddMember(c2);
            ros.AddMember(c3);
            ////
            var e1 = new NodeElementary(Graph,"I am groot!");
            var e2 = new NodeElementary(Graph, "We are groot!");
            c2.AddMember(e1);
            c2.AddMember(e2);
            //
            //
            var e3 = new NodeElementary(Graph, "I am Inevitable!");
            var e4 = new NodeElementary(Graph, "I am Ironman!");
            c3.AddMember(e3);
            c3.AddMember(e4);

            
            Graph.RootSubgraph = rootSubgraph;
            NodeCounter = Graph.NodeCount;
            _graphViewer.Graph = Graph;
            
        }

        private void CreateDrawingBoard()
        {

        }

        private void UpdateNodeCount()
        {
            CounterText.Text = Graph.NodeCount.ToString();
        }

        private string IncrementNodeId()
        {
            NodeCounter++;
            UpdateNodeCount();
            return "ID: " + NodeCounter;
        }
        private string IncrementSubGraphId()
        {
            SubGraphCounter++;
            UpdateNodeCount();
            return ":SUBG: " + SubGraphCounter;
        }

        private string getNodeID_before()
        {
            var preNode = NodeCounter - 1;
            return "ID: " + preNode;
        }
       

        private void InsertNode()
        {
            // Possible to Insert into more then one ?
            try
            {
                IViewerObject forDragging = _graphViewer.Entities
                                                        .Single(x => x.MarkedForDragging);
                var subgraph = ((IViewerNode) forDragging).Node;
                subgraph.Attr.LineWidth = 1;
                if (subgraph is Subgraph)
                {
                    var node = new NodeElementary(Graph, "New Node");
                    var nodeComplex = Graph.GetComplexNodeById(subgraph.Id);
                    nodeComplex.AddMember(node);
                    _graphViewer.Graph = Graph;
                    //_graphViewer.GraphCanvas.UpdateLayout();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("More then one node Selected!" + e);
                throw;
            }
            // todo 
        } // todo buggy as hell

       
        //todo probably needs to be extracted to NodeComplex / Elementary 
        private void AddEdge()
        {
            //todo predecessor successor
            if (_graphViewer.Entities.Any())
            {
                var forDragging = _graphViewer.Entities
                    .Where(x => x.MarkedForDragging)
                    .Cast<IViewerNode>()
                    .ToArray();
                if (forDragging.Length > 1)
                {
                    foreach (var node in forDragging)
                    {
                        node.Node.Attr.LineWidth = 1;
                    }
                    
                    Graph.AddEdge(forDragging[0].Node.Id, forDragging[1].Node.Id);
                    Graph.LayerConstraints.AddSameLayerNeighbors(forDragging[0].Node,forDragging[1].Node); 
                }
                

            }
            Graph.Attr.LayerDirection = LayerDirection.LR;
            _graphViewer.Graph = Graph;
            Graph.Attr.LayerDirection = LayerDirection.TB;

            //todo magic for constraints?!
        }

         private void InsertSubgraph()
         {
             // Possible to Insert into more then one ?
             try
             {
                 IViewerObject forDragging = _graphViewer.Entities
                     .Single(x => x.MarkedForDragging);
                 var subgraph = ((IViewerNode) forDragging).Node;
                 subgraph.Attr.LineWidth = 1;
                 if (subgraph is Subgraph)
                 {
                     var node = new NodeComplex(Graph, "New Complex");
                     var nodeComplex = Graph.GetComplexNodeById(subgraph.Id);
                     nodeComplex.AddMember(node);
                     _graphViewer.Graph = Graph;
                     //_graphViewer.GraphCanvas.UpdateLayout();
                 }
             }
             catch (Exception e)
             {
                 MessageBox.Show("More then one complex selected!");
                 //throw;

             }
           
             // todo 
         } // todo buggy as hell

//todo repair delete method
         private void DeleteNode()
         {
             var arrayList = new ArrayList();
             foreach (var obj in _graphViewer.Entities)
                 if (obj.MarkedForDragging)
                     arrayList.Add(obj);
             foreach (IViewerObject obj in arrayList)
             {
                 var edge = obj.DrawingObject as IViewerEdge;
                 if (edge != null)
                 {
                     _graphViewer.RemoveEdge(edge, true);
                 }
                 else
                 {
                     var node = obj as IViewerNode;
                     if (node != null) _graphViewer.RemoveNode(node, true);
                 }
             }

             UpdateNodeCount();
             //todo visualize 
         }


         private string TagAStringToInt(int id, string tag = "ID")
         {
             if (tag.Length < 6)
                 return tag + ": " + id;
             return "ID: " + id;
         }

         private void MenuItem_OnClick(object sender, RoutedEventArgs e)
         {
             Close();
         }

         private void Open_OnClick(object sender, RoutedEventArgs e)
         {
             var openFileDlg = new OpenFileDialog();

             // Launch OpenFileDialog by calling ShowDialog method
             openFileDlg.ShowDialog();
             // Get the selected file name and display in a TextBox.
             // Load content of file in a TextBlock
         }


         private void Rectangle1_OnMouseEnter(object sender, MouseEventArgs e)
         {
             rectangle1.Fill = (SolidColorBrush) new BrushConverter().ConvertFromString("#4dd2ff");
         }

         private void Rectangle1_OnMouseLeave(object sender, MouseEventArgs e)
         {
             rectangle1.Fill = (SolidColorBrush) new BrushConverter().ConvertFromString("#00ace6");
         }

         private void Hexa_button_Click(object sender, RoutedEventArgs e)
         {
             if (Graph == null) return;
             var rnd = new Random(1024);
             var curCount = NodeCounter;
             Graph.AddNode(IncrementNodeId());
             Graph.AddEdge("ID: " + rnd.Next(curCount), "ID: " + NodeCounter);
             UpdateNodeCount();
             _graphViewer.Graph = Graph;
             _graphViewer.GraphCanvas.UpdateLayout();
         }

         private void DeleteSelectedNode_Click(object sender, RoutedEventArgs e)
         {
             DeleteNode();
             UpdateNodeCount();
         }

         private void DeleteNodeCM_Click(object sender, RoutedEventArgs e)
         {
             DeleteNode();
             UpdateNodeCount();
         }

         private void cm_Opened(object sender, RoutedEventArgs e)
         {
         }

         private void cm_Closed(object sender, RoutedEventArgs e)
         {
         }


         private void Window_MouseMove(object sender, MouseEventArgs e)
         {
             mouseLabel.Content = e.GetPosition(this).ToString();
         }


         private void AddToSelectedNodeCM_Click(object sender, RoutedEventArgs e)
         {
          
         }

         private void cm_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
         {
             _mMouseRightButtonDownPoint = new Point(e.GetPosition(this).X, e.GetPosition(this).Y);
             mouseLabel.Content = "Right Click: " + _mMouseRightButtonDownPoint;
         }

         private void AddSubgraphToSelectedNodeCM_Click(object sender, RoutedEventArgs e)
         {
             InsertSubgraph();
         }

         private void ResetGraph_Click(object sender, RoutedEventArgs e)
         {
         }

         private void EdgeSelectedNode_Click(object sender, RoutedEventArgs e)
         {
             AddEdge();
         }

         private void EditMode_Click(object sender, RoutedEventArgs e)
         {
             var defaultColor = EditMode.Background;
             if (_graphViewer.InsertingEdge)
             {
                 _graphViewer.InsertingEdge = false;
                 EditMode.Background = (SolidColorBrush) new BrushConverter().ConvertFromString(defaultColor.ToString());
             }
             else
             {
                 _graphViewer.InsertingEdge = true;
                 EditMode.Background = (SolidColorBrush) new BrushConverter().ConvertFromString("#4dd2ff");
             }
         }

         private void InsertNodeCM_Click(object sender, RoutedEventArgs e)
         {
             InsertNode();
         }

         private void ViewerPanel_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
         {
             _graphViewer.StartDrawingRubberLine(_mMouseLeftButtonDownPoint);
         }
    }
}