using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Core.Routing;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.Layout.Layered;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Microsoft.Msagl.WpfGraphControl;
using TestingMSAGL.DataLinker;
using TestingMSAGL.DataStructure;
using TestingMSAGL.DataStructure.RoutedOperation;
using Edge = Microsoft.Msagl.Drawing.Edge;
using Single = TestingMSAGL.DataStructure.RoutedOperation.Single;

namespace TestingMSAGL.ComplexEditor
{
    public class Editor
    {
        public readonly GraphViewer GraphViewer;
        private Brush _oldShapeFill = Brushes.Transparent;


        public Editor()
        {
            GraphViewer = new GraphViewer { LayoutEditingEnabled = true };
            //initGraph(null, null);
            //GraphViewer.GraphCanvas.Background = (SolidColorBrush) new BrushConverter().ConvertFromString("#4dd2ff");
        }


        public GraphExtension Graph { get; private set; } = new("root", "0");



        private int NodeCounter { get; set; }

        /// <summary>
        ///     triggers rerender process of graph
        /// </summary>
        public void refreshLayout()
        {
            GraphViewer.Graph = Graph;
        }


        /// <summary>
        ///     Static graph creating for testing purposes only.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void initGraph(object sender, RoutedEventArgs e)
        {
            GraphViewer.RunLayoutAsync = true;
            //todo rework initialization
            // Drawing Board
            var rootSubgraph = new Subgraph("rootSubgraph");
            // first element 
            var root = new NodeComplex(Graph, "Root");
            Graph.RootNode = root;
            rootSubgraph.AddSubgraph(root.Subgraph);

            //var ros = new Alternative(Graph, "Root Of Subgraph");
            //ros.Composite.Predecessor = root.Composite;

            //root.Composite.Successor = ros.Composite;
            //root.AddMember(ros);

            //var c1 = new Single(Graph, "Complex: 104");
            //var c2 = new Parallel(Graph, "Complex: 105");
            ////NodeComplex c3 = new Alternative(Graph, "Complex: 106");
            //ros.AddMember(c1); // layout Problem. - does not contain any nodes - that's why :)
            //ros.AddMember(c2);
            //ros.Composite.Successor = c2.Composite;
            //c2.Composite.Predecessor = ros.Composite;
            ////ros.AddMember(c3);
            ////c3.Composite.Predecessor = c2.Composite;
            ////c2.Composite.Successor = c3.Composite;


            //////
            //var e1 = new NodeElementary(Graph, "");
            //var e2 = new NodeElementary(Graph, "");
            //var e3 = new NodeElementary(Graph, "");
            //root.AddMember(e2);
            //root.AddMember(e3);
            //c2.AddMember(e1);
            //c2.AddMember(e2);
            //c2.AddMember(e3);
            //var c4 = new Fixed(Graph, "heya");
            ////var c5 = new Fixed(Graph, "fix it!");
            ////var c6 = new Parallel(Graph, "fix it!");

            ////c4.Composite.Successor = c6.Composite;
            ////c6.Composite.Predecessor = c4.Composite;
            //c2.AddMember(c4);
            //c2.AddMember(c5);
            //c2.AddMember(c6);
            //var e4 = new NodeElementary(Graph, "I am Inevitable!");
            //var e5 = new NodeElementary(Graph, "I am Ironman!");
            //c3.AddMember(e4);  
            //c3.AddMember(e5);

            // add edge for testing purposes

            //Graph.AddEdge(e2.NodeId, "Test", e3.NodeId);


            // if(c3 is Alternative alternative)


            /// Bulding for presentation ///
            /// preparation
            /// outer 
            var preparation = new Parallel(Graph, "");
            /// inner 
            var prepFirst = new Single(Graph, "");
            var prepSecond = new Single(Graph, "");

            var prepFirstOperationOne = new NodeElementary(Graph, "Zerspanen");
            var prepFirstOperationSecond = new NodeElementary(Graph, "Entgraten");
            var prepSecondOperationFirst = new NodeElementary(Graph, "Kanten");
            var prepSecondOperationSecond = new NodeElementary(Graph, "Gewindeschneiden");

            /// root
            root.AddMember(preparation);

            /// construct preparation node

            prepFirst.AddMember(prepFirstOperationOne);
            prepFirst.AddMember(prepFirstOperationSecond);

            prepSecond.AddMember(prepSecondOperationFirst);
            prepSecond.AddMember(prepSecondOperationSecond);

            preparation.AddMember(prepFirst);
            preparation.AddMember(prepSecond);

            /// assembly
            /// outer

            var assembly = new Alternative(Graph, "");
            root.AddMember(assembly);

            /// inner

            var weld = new Single(Graph, "Schwei??en");
            var fixate = new Single(Graph, "Fixieren");

            var weldExternal = new NodeElementary(Graph, "Extern Schwei??en");
            var spotWeld = new NodeElementary(Graph, "Punktschwei??en");

            var closeWeldseam = new NodeElementary(Graph, "Schwei??naht schlie??en");

            assembly.AddMember(weld);
            assembly.AddMember(weldExternal);

            weld.AddMember(closeWeldseam);
            fixate.AddMember(spotWeld);
            fixate.AddMember(new NodeElementary(Graph, "Punktschwei??en"));
            weld.AddMember(fixate);

            /// add edges

            Graph.AddEdge(prepFirstOperationOne.NodeId, prepFirstOperationSecond.NodeId);
            Graph.AddEdge(prepFirst.NodeId, prepSecond.NodeId);
            Graph.AddEdge(preparation.NodeId, assembly.NodeId);
            Graph.AddEdge(fixate.NodeId, closeWeldseam.NodeId);

            ///

            var composites = root.Composite.Members;


            Graph.RootSubgraph = rootSubgraph;


            NodeCounter = Graph.NodeCount;
            //Graph.Attr.BackgroundColor = Color.Orange;
            Graph.Directed = true;


            Graph.Attr.LayerDirection = LayerDirection.LR;
            Graph.LayoutAlgorithmSettings = new SugiyamaLayoutSettings
            {
                //    MinNodeWidth = 10,
                //    MinimalHeight = 10,
                ClusterMargin = 3,
                EdgeRoutingSettings = new EdgeRoutingSettings()
                {
                    BendPenalty = 10000,
                    EdgeRoutingMode = EdgeRoutingMode.StraightLine
                },
                PackingMethod = PackingMethod.Compact,
                PackingAspectRatio = 0.2,
                //    //LayeringOnly = true,
                MaxAspectRatioEccentricity = 1,
                
                //    LabelCornersPreserveCoefficient = 2,
                //    //LiftCrossEdges = true
            };

            var test3 = Graph.CreateLayoutSettings();



            // clear all complex labels
            foreach (var subgraph in root.Subgraph.AllSubgraphsDepthFirst())
            {
                subgraph.LabelText = "";
                subgraph.Label = null;
            }

            //var listOfSubgraphs = new List<Microsoft.Msagl.Drawing.Node>();

            //foreach(var subgraph in Graph.RootSubgraph.AllSubgraphsDepthFirst())
            //{
            //    listOfSubgraphs.Add(subgraph);
            //}
            //Graph.LayerConstraints.AddUpDownVerticalConstraints(listOfSubgraphs.ToArray());
            refreshLayout();
            //var subgraphs = root.Subgraph.AllSubgraphsDepthFirst();
            //var templist = new List<Microsoft.Msagl.Drawing.Node>();
            //try
            //{
            //foreach (var parentsubgraph in subgraphs)
            //{
            //    if (parentsubgraph.Nodes.Any())
            //    {
            //        foreach (var node in parentsubgraph.Nodes)
            //        {
            //            if (node.InEdges.Any() || node.OutEdges.Any() || node.SelfEdges.Any())
            //            {
            //                if (!templist.Contains(node))
            //                    templist.Add(node);
            //            }
            //        }
            //        if (parentsubgraph.Subgraphs.Any())
            //        {
            //            foreach (var subgraph in parentsubgraph.Subgraphs)
            //            {
            //                if (subgraph.InEdges.Any() || subgraph.OutEdges.Any() || subgraph.SelfEdges.Any())
            //                {
            //                    if (!templist.Contains(subgraph))
            //                        templist.Add(subgraph);
            //                }
            //            }

            //        }
            //        if (templist.Any())
            //            Graph.LayerConstraints.PinNodesToSameLayer(templist.ToArray());
            //    }

            //};

            //refreshLayout();

            //}
            //catch (Exception error)
            //{
            //    Console.WriteLine(error);
            //    throw;
            //}

            var test = GraphViewer.GraphCanvas.Children;
        }

      

        /// <summary>
        ///     Method to create a new node in given Subgraph/Complex
        ///     no refresh implemented!
        /// </summary>
        public void InsertNode(IWithId node)
        {
            //todo  Possible to Insert into more then one ?
            var selectedSubgraph = GetSelectedNode();
            if (selectedSubgraph == null) return;
            
            GraphViewer.LayoutEditor.RemoveObjDraggingDecorations(selectedSubgraph);
            if (selectedSubgraph.Node is Subgraph)
            {
                node ??= new NodeElementary(Graph, "");
                var nodeComplex = Graph.GetComplexNodeById(selectedSubgraph.Node.Id);
                //todo check if member exists?
                if (!nodeComplex.AddMember(node)) MessageBox.Show("Could not add to member list");
                //todo add method to detect if children are already present, sort new node as successor of last child
                //node.Composite.Predecessor = nodeComplex.Composite;
                //nodeComplex.Composite.Successor = node.Composite;
                //todo fix issues
            }
            //GraphViewer.Graph = Graph;

        } // todo buggy as hell

        /// <summary>
        /// Get one single selected node. If more than one or no node is selected this will show a descriptive
        /// error message to the user.
        /// </summary>
        /// <returns>The selected node casted to a IViewerNode</returns>
        private IViewerNode GetSelectedNode()
        {
            try
            {
                var selectedNode = findOneNodeSelected();
                if (selectedNode == null)
                {
                    MessageBox.Show("No Complex selected!");
                    // todo fix issues of straying nodes due to SingleOrDefault
                    return null;
                }

                return selectedNode as IViewerNode;
            }
            catch (InvalidOperationException exception)
            {
                MessageBox.Show("More than one complex selected!\n" + exception);
                return null;
            }
        }

        /// <summary>
        /// Get a list of all selected nodes. The nodes will be casted to IViewerNode.
        /// </summary>
        /// <returns></returns>
        private List<IViewerNode> GetSelectedNodes()
        {
            return GraphViewer.Entities
                .Where(x => x.MarkedForDragging)
                .Cast<IViewerNode>()
                .ToList();
        }

        //todo probably needs to be extracted to NodeComplex / Elementary 
        /// <summary>
        ///     Takes any type of a complex and merges all selected elementaries or complex into it
        /// </summary>
        /// <param name="complexType">The type of node that the elementaries should be grouped in</param>
        public void ConvertGroupOfElementariesToComplex(string complexType)
        {
            var selectedNodes = GetSelectedNodes();
            if (!selectedNodes.Any()) return;

            var selectedNodeIds = selectedNodes.Select(viewerNode => viewerNode.Node.Id).ToList();
            if (selectedNodeIds.Contains(Graph.RootNode.Subgraph.Id))
            {
                MessageBox.Show("The Root node cannot be part of a group!");
                return;
            }
            
            var complex = CreateIWithId(complexType);
            if (complex is not NodeComplex newComplex)
            {
                MessageBox.Show($"Cannot group elementaries in {{complexType}}");
                return;
            }
            
            // Get selected node with lowest depth in the graph. This is the node where we will insert the new complex in. 
            var targetNode = Graph.RootNode;
            var searchResult = Graph.RootNode.Composite.BreadthFirstSearch(composite => selectedNodeIds.Contains(composite.DrawingNodeId));
            if (searchResult != null)
            {
                targetNode = Graph.GetComplexNodeById(searchResult.ParentId);
            }
            
            foreach (var viewerNode in selectedNodes)
            {
                //workaround for select issue
                GraphViewer.LayoutEditor.RemoveObjDraggingDecorations(viewerNode);

                var node = Graph.GetNodeById(viewerNode.Node.Id);
                var parent = Graph.GetComplexNodeById(node.ParentId);

                parent.RemoveMember(node);
                newComplex.AddMember(node);
            }

            targetNode.AddMember(newComplex);
            refreshLayout();
        }

        // todo needs complete rework
        public void AddEdge()
        {
            //todo predecessor successor
            var selectedNodes = GetSelectedNodes();
            if (selectedNodes.Count < 1) return;

            var nodes = new List<NodeComplex>();
            foreach (var node in selectedNodes)
            {
                nodes.Add(Graph.GetComplexNodeById(node.Node.Id));
                GraphViewer.LayoutEditor.RemoveObjDraggingDecorations(node);
            }

            foreach (var composite in nodes)
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

            GraphViewer.Graph = Graph;
            //todo magic for constraints?!
        }

        /// <summary>
        ///     Testing purpose only
        ///     Creates 10 Nodes, if a complex is marked.
        /// </summary>
        /// <param name="numberOfNodes"></param>
        public void CreateAnyAmountOfNodesForTesting(int numberOfNodes)
        {
            if (!GraphViewer.Entities.Any(x => x.MarkedForDragging)) return;

            for (var i = 0; i < numberOfNodes; i++) InsertNode(null);
        }

        /// <summary>
        ///     adds a subgraph into an existing subgraph and adds itself as a member of the parent complex
        /// </summary>
        /// <param name="complex"></param>
        /// <param name="subgraph"></param>
        public void InsertSubgraph(IWithId complex, IViewerNode subgraph)
        {
            // Possible to Insert into more then one ?
            try
            {
                GraphViewer.LayoutEditor.RemoveObjDraggingDecorations(subgraph);

                var nodeComplex = Graph.GetComplexNodeById(subgraph.Node.Id);
                nodeComplex.AddMember(complex);

                refreshLayout();
            }
            catch (Exception e)
            {
                MessageBox.Show("More than one complex selected! " + e);
            }
        }

        /// <summary>
        ///     Checks for any selected node (subgraph) and deletes the node and edges (subgraph and all children)
        /// </summary>
        public void DeleteNode()
        {
            try
            {
                var selectedNodes = GetSelectedNodes();
                if (selectedNodes.Count < 1) return;

                foreach (var viewerNode in selectedNodes)
                {
                    //GraphViewer.LayoutEditor.RemoveObjDraggingDecorations(viewerNode);
                    HandleNodeRelations(viewerNode);
                    
                    //todo extract as method to Graph Extension
                    if (viewerNode.Node is Subgraph subgraph)
                    {
                        if (subgraph.ParentSubgraph.ParentSubgraph == null)
                        {
                            MessageBox.Show("Error: Root can't be deleted!");
                            return;
                        }

                        //check for Children
                        if (subgraph.Subgraphs.Any())
                        {
                            var allChildren = subgraph.AllSubgraphsDepthFirst().ToList();
                            foreach (var child in allChildren) Graph.DeleteRecursive(child);
                        }
                        else
                        {
                            var allNodes = subgraph.Nodes.ToList();
                            foreach (var node in allNodes) Graph.RemoveNode(node);

                            subgraph.ParentSubgraph.RemoveSubgraph(subgraph);
                            Graph.DeleteById(subgraph.Id);
                            Graph.RemoveNode(subgraph);
                        }
                    }
                    else
                    {
                        //todo extract as method to Graph Extension
                        var childComposite = Graph.GetNodeById(viewerNode.Node.Id);
                        var parentComposite = Graph.GetComplexNodeById(childComposite.ParentId);
                        parentComposite.RemoveMember(childComposite);
                        // removes nodes from (root)subgraph into Graph
                        parentComposite.Subgraph.RemoveNode(viewerNode.Node);
                        // removes straying nodes from Graph
                        Graph.RemoveNode(viewerNode.Node);

                    }

                    refreshLayout();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error in Delete()\n" + e);
                //throw;
            }
        }
        /// <summary>
        /// does not function YET
        /// </summary>
        // TODO implement proper shallow delete
        public void ShallowDeleteNode()
        {
            try
            {
                var selectedNodes = GetSelectedNodes();
                if (selectedNodes.Count < 1) return;
                
                var compositeList = new List<IWithId>();
                foreach (var viewerNode in selectedNodes)
                {
                    
                    var markedComplex = Graph.GetComplexNodeById(viewerNode.Node.Id);
                    var parent = Graph.GetComplexNodeById(markedComplex.ParentId);
                    if (markedComplex.Composite.Members.Any())
                        foreach (var member in markedComplex.Composite.Members)
                        {
                            var node = Graph.GetNodeById(member.DrawingNodeId);
                            if ( node is NodeComplex nodeComplex)
                                compositeList.Add(nodeComplex);
                            if (node is NodeElementary nodeElementary)
                                compositeList.Add(nodeElementary);
                        }

                    DeleteNode();
                    bool isTrue;
                    foreach(var saveNodeWithId in compositeList)
                        {
                            isTrue =parent.AddMember(saveNodeWithId);
                        }
                    var test123 = parent.RemoveMember(markedComplex);
                    
                }
                refreshLayout();
                }
            catch (Exception e)
            {
                MessageBox.Show("Error in ShallowDelete()\n" + e);
                //throw;
            }
        }

        /// <summary>
        /// checks if Edges to and from that node exists, if so the predecessors now points to the successor of the deleted node
        /// </summary>
        /// <param name="viewerNode"></param>
        private void HandleNodeRelations(IViewerNode viewerNode)
        {
            // check for edges - if node within a relation is deleted, maintain the relation (parent -> grand child)
            var listOfInEdges = new List<Edge>();
            var listOfOutEdges = new List<Edge>();
            var listOfSelfEdges = new List<Edge>();
            var listOfSuccessors = new List<string>();
            var listOfPredecesors = new List<string>();
            // first delete all edges and remember all pre- and successors
            if (viewerNode.Node.InEdges.Any())
            {
                foreach (var edge in viewerNode.Node.InEdges)
                {
                    listOfPredecesors.Add(edge.Source);
                    listOfInEdges.Add(edge);
                }

            foreach (var edge in listOfInEdges)
                Graph.RemoveEdge(edge);
            }
            if (viewerNode.Node.OutEdges.Any())
            {
                foreach (var edge in viewerNode.Node.OutEdges)
                {
                    listOfSuccessors.Add(edge.Target);
                    listOfOutEdges.Add(edge);
                }

            foreach (var edge in listOfOutEdges)
                Graph.RemoveEdge(edge);
            }
            if (viewerNode.Node.SelfEdges.Any())
            {
                foreach (var edge in viewerNode.Node.SelfEdges)
                    listOfSelfEdges.Add(edge);

            foreach (var edge in listOfSelfEdges)
                Graph.RemoveEdge(edge);
            }

            // second recreate all relations

            foreach (var predecessor in listOfPredecesors)
            {
                foreach (var successor in listOfSuccessors)
                {
                    Graph.AddEdge(predecessor, successor);
                }
            }
        }

        public IViewerObject findOneNodeSelected()
        {
            return GraphViewer.Entities.SingleOrDefault(x => x.MarkedForDragging);
        }

        /// <summary>
        ///     creates types of NodeComplex, or NodeElementary - defaults to NodeElementary (routed operation)
        ///     add new types here
        /// </summary>
        /// <param name="routedOperation"></param>
        /// <returns></returns>
        public IWithId CreateIWithId(string routedOperation)
        {
            return routedOperation switch
            {
                "Alternative" => new Alternative(Graph, routedOperation),
                "Parallel" => new Parallel(Graph, routedOperation),
                "Fixed" => new Fixed(Graph, routedOperation),
                "Single" => new Single(Graph, routedOperation),
                "Node" => new NodeElementary(Graph, ""),
                _ => new NodeElementary(Graph, routedOperation)
            };
        }
    }
}