using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Channels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.WpfGraphControl;
using Microsoft.Win32;
using TestingMSAGL.DataLinker;
using TestingMSAGL.DataStructure;
using TestingMSAGL.DataStructure.RoutedOperation;
using Color = Microsoft.Msagl.Drawing.Color;
using ModifierKeys = Microsoft.Msagl.Drawing.ModifierKeys;
using Node = Microsoft.Msagl.Drawing.Node;
using Point = Microsoft.Msagl.Core.Geometry.Point;
using Single = TestingMSAGL.DataStructure.RoutedOperation.Single;

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
            
            ViewerPanel.ClipToBounds = true;
            _graphViewer.BindToPanel(ViewerPanel);
            Graph.LayoutAlgorithmSettings.EdgeRoutingSettings.RouteMultiEdgesAsBundles = true;
            _graphViewer.LayoutEditingEnabled = true;
            //todo graphViewer props for pane moving
            _graphViewer.Graph = Graph;
            NodeTreeView();
            
        }

       

        private void NodeTreeView()
        {
            MenuItem root = new MenuItem() {Title = "Root"};
            
            
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
        /// <summary>
        /// Static graph creating for testing purposes only.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InitGraph(object sender, RoutedEventArgs e)
        {
            // Drawing Board
            Subgraph rootSubgraph = new Subgraph("rootSubgraph");

            // first element 
            var root = new NodeComplex(Graph, "Root");
            rootSubgraph.AddSubgraph(root.Subgraph);

            var ros = new Alternative(Graph, "Root Of Subgraph");
            ros.Composite.Predecessor = root.Composite;
            
            root.Composite.Successor = ros.Composite;
            root.AddMember(ros);

            var c1 = new Single(Graph, "Complex: 104");
            var c2 = new Fixed(Graph, "Complex: 105");
            //NodeComplex c3 = new Alternative(Graph, "Complex: 106");
            ros.AddMember(c1); // layout Problem. - does not contain any nodes - that's why :)
            ros.AddMember(c2);
            ros.Composite.Successor = c2.Composite;
            c2.Composite.Predecessor = ros.Composite;
            //ros.AddMember(c3);
            //c3.Composite.Predecessor = c2.Composite;
            //c2.Composite.Successor = c3.Composite;


            ////
            var e1 = new NodeElementary(Graph,"I am groot!");
            var e2 = new NodeElementary(Graph, "We are groot!");
            var e3 = new NodeElementary(Graph, "We are not groot!");
            c2.AddMember(e1);
            c2.AddMember(e2);
            c2.AddMember(e3);
            //
            //
            var c4 = new Fixed(Graph, "heya");
            //var c5 = new Fixed(Graph, "fix it!");
            //var c6 = new Parallel(Graph, "fix it!");

            //c4.Composite.Successor = c6.Composite;
            //c6.Composite.Predecessor = c4.Composite;
            c2.AddMember(c4);
            //c2.AddMember(c5);
            //c2.AddMember(c6);
            //var e4 = new NodeElementary(Graph, "I am Inevitable!");
            //var e5 = new NodeElementary(Graph, "I am Ironman!");
            //c3.AddMember(e4);  
            //c3.AddMember(e5);

           // if(c3 is Alternative alternative)
                Console.WriteLine("Test");
            var composites = root.Composite.Members;
            
            Graph.RootSubgraph = rootSubgraph;
            NodeCounter = Graph.NodeCount;
            _graphViewer.Graph = Graph;
            _graphViewer.RunLayoutAsync = true;
            


        }

       
        /// <summary>
        /// Method to create a new node in given Subgraph/Complex
        /// </summary>
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
                    var node = new NodeElementary(Graph, "New Node") ;
                    var nodeComplex = Graph.GetComplexNodeById(subgraph.Id);
                    //todo check if member exists?
                    if(!nodeComplex.AddMember(node))
                        MessageBox.Show("Could not add to member list");
                    //todo add method to detect if children are already present, sort new node as successor of last child
                   //node.Composite.Predecessor = nodeComplex.Composite;
                   //nodeComplex.Composite.Successor = node.Composite;
                   //todo fix issues
                    _graphViewer.Graph = Graph;
                    //_graphViewer.GraphCanvas.UpdateLayout();
                }
            }

            // todo repair exception handling and message

            catch (ArgumentNullException e)
            {
                MessageBox.Show("No complex selected!");
                // throw;
            }
            catch (InvalidOperationException invalid)
            {
                MessageBox.Show("More than one complex selected!");
            }

        } // todo buggy as hell
        //todo probably needs to be extracted to NodeComplex / Elementary 


        private void ConvertGroupOfElementariesToComplex(NodeComplex newComplex)
        {
            
            var forDragging = _graphViewer.Entities
                .Where(x => x.MarkedForDragging)
                .Cast<IViewerNode>()
                .ToArray();
            if (forDragging.Length < 1) return;
            
            var nodes = new List<IWithId>();
            
            foreach (var node in forDragging)
            {
                var nodeComplex = Graph.GetNodeById(node.Node.Id);
                
                nodes.Add(nodeComplex);
                node.Node.Attr.LineWidth = 1;
            }

            if (nodes.Any(x => x.ParentId == null))
            {
                MessageBox.Show("Error: Root can't be part of a Complex.");
                return;
            }

            var parentNode = Graph.GetComplexNodeById(nodes.First().ParentId);         
                
            
            var toBeDeleted = parentNode.Subgraph.Nodes.Where(node => nodes.Contains(Graph.GetNodeById(node.Id)))
                    .ToList();
            toBeDeleted.AddRange(parentNode.Subgraph.AllSubgraphsDepthFirstExcludingSelf());
            
            foreach (var entity in toBeDeleted)
            {
                parentNode.Subgraph.RemoveNode(entity);
                parentNode.Subgraph.RemoveNode(entity);

            }

            if (parentNode.AddMember(newComplex))
            {
                foreach (var member in nodes.Select(node => node))
                {
                    newComplex.AddMember(member);
                }
            }
            
                
            
            _graphViewer.Graph = Graph;
        }
        
        private void AddEdge()
        {
            //todo predecessor successor
            
            var forDragging = _graphViewer.Entities
                .Where(x => x.MarkedForDragging)
                .Cast<IViewerNode>()
                .ToArray();
            if (forDragging.Length < 1) return;
                            
            var nodes = new List<NodeComplex>();
            foreach (var node in forDragging)
            {
                nodes.Add(Graph.GetComplexNodeById(node.Node.Id));
                node.Node.Attr.LineWidth = 1;
            }
            
            foreach (var composite in nodes)
            {
                if (composite?.Composite.Successor != null)
                {
                    Graph.AddEdge(
                        composite.NodeId,
                        composite.Composite.Successor.DrawingNodeId
                    );
                    // adds nodes side-by-side (same layer left -> right)
                    Graph.LayerConstraints.AddSameLayerNeighbors(
                        Graph.FindNode(composite.Composite.DrawingNodeId),
                        Graph.FindNode(composite.Composite.Successor.DrawingNodeId)
                    );
                }
            }   

            Graph.Attr.LayerDirection = LayerDirection.LR;
            _graphViewer.Graph = Graph;
            Graph.Attr.LayerDirection = LayerDirection.TB;


            //todo magic for constraints?!
        }

         private void InsertSubgraph(IWithId complex)
         {
             // Possible to Insert into more then one ?
             try
             {
                 var forDragging = _graphViewer.Entities
                     .Single(x => x.MarkedForDragging);
                 var subgraph = ((IViewerNode) forDragging).Node;
                 subgraph.Attr.LineWidth = 1;
                 if (subgraph is Subgraph)
                 {
                     var nodeComplex = Graph.GetComplexNodeById(subgraph.Id);
                     nodeComplex.AddMember(complex);
                     

                 }else
                 {
                     MessageBox.Show("Error: You tried to insert into an elementary.");
                     
                 }
                 _graphViewer.Graph = Graph;

             }
             catch (Exception e)
             {
                 MessageBox.Show("More than one complex selected! " +e);
                 //throw;
             }

             // todo can't handle clicked elementary nodes
         } // todo buggy as hell

         private void DeleteNode()
         {
             try
             {
                 var forDragging = _graphViewer.Entities
                     .Single(x => x.MarkedForDragging);
                 var subgraph = ((IViewerNode) forDragging).Node;

                 
                 if (subgraph is Subgraph)
                 {
                     // todo get all inherent members and add them to the parent of parent of the node that is going to be deleted
                     // todo what will happen if the parent of parent is the root ? root undeletable?
                     var nodeComplex = Graph.GetComplexNodeById(subgraph.Id);
                     var parentNode = Graph.GetComplexNodeById(nodeComplex.Composite.ParentId);
                     
                     if (!parentNode.RemoveMember(nodeComplex ))
                         MessageBox.Show("Could not delete complex!", subgraph.Id);
                     _graphViewer.Graph = Graph;
                 }
                 
                 // todo delete elementary with same method
             } catch (Exception e)
             {
                 MessageBox.Show("Error in Delete()\n" +e);
                 //throw;
             }
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
         
         }

         private void DeleteSelectedNode_Click(object sender, RoutedEventArgs e)
         {
             DeleteNode();
             
         }

         private void cm_Opened(object sender, RoutedEventArgs e)
         {
         }

         private void cm_Closed(object sender, RoutedEventArgs e)
         {
         }


         private void Window_MouseMove(object sender, MouseEventArgs e)
         {
             MouseLabel.Content = e.GetPosition(this).ToString();
         }

         private void cm_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
         {
             _mMouseRightButtonDownPoint = new Point(e.GetPosition(this).X, e.GetPosition(this).Y);
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

         
         void ViewerPanel_OnMouseLeftButtonDown(object sender, MouseEventArgs e)
         {  
             // todo get this to work!
             var p0 = new Point(e.GetPosition(this).X, e.GetPosition(this).Y);
             var p1 = new Point(e.GetPosition(this).X, e.GetPosition(this).Y);
             if (e.LeftButton == MouseButtonState.Pressed)
             {
                p0.X = e.GetPosition(this).X;
                p0.Y = e.GetPosition(this).Y;
                 
             }

             if (e.LeftButton == MouseButtonState.Released)
             {
                 p1.X = e.GetPosition(this).X;
                 p1.Y = e.GetPosition(this).Y;

             }
            
             var rubberRec = new Rectangle(p0, p1);
         }

         private void AddAlternativMenuItem_OnClick(object sender, RoutedEventArgs e)
         {
             InsertSubgraph(new Alternative(Graph, "New Alternative"));
         }

         private void AddParallelMenuItem_OnClick(object sender, RoutedEventArgs e)
         {
             InsertSubgraph(new Parallel(Graph, "New Parallel"));
         }

         private void AddFixedMenuItem_OnClick(object sender, RoutedEventArgs e)
         {
             InsertSubgraph(new Fixed(Graph, "New Fixed"));
         }

         private void AddSingleMenuItem_OnClick(object sender, RoutedEventArgs e)
         {
             InsertSubgraph(new Single(Graph, "New Single"));

         }

         private void GroupAlternative_OnClick(object sender, RoutedEventArgs e)
         {
             ConvertGroupOfElementariesToComplex(new Alternative(Graph, "New Group Alternative"));
         }

         private void GroupParallel_OnClick(object sender, RoutedEventArgs e)
         {
             ConvertGroupOfElementariesToComplex(new Parallel(Graph, "New Group Parallel"));
         }

         private void GroupFixed_OnClick(object sender, RoutedEventArgs e)
         {
             ConvertGroupOfElementariesToComplex(new Fixed(Graph, "New Group Fixed"));
         }

         private void GroupSingle_OnClick(object sender, RoutedEventArgs e)
         {
             ConvertGroupOfElementariesToComplex(new Single(Graph, "New Group Single"));
         }
    }
}