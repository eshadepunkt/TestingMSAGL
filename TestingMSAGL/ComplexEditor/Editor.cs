using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Core.Routing;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.Layout.Layered;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Windows;
using System.Windows.Media;
using Microsoft.Msagl.WpfGraphControl;
using TestingMSAGL.DataLinker;
using TestingMSAGL.DataStructure;
using Edge = Microsoft.Msagl.Drawing.Edge;
using OclAspectTest;
using TestingMSAGL.Constraints;
using TestingMSAGL.DataLinker.RoutedOperation;
using TestingMSAGL.DataStructure.Actions;
using TestingMSAGL.DataStructure.RoutingOperation;

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
            var constraints = new List<IConstraint> {
                new DifferentTasksConstraint(),
                new OrCombinedConstraint(new NotConstraint(new TypeConstraint(typeof(ParallelRoutingOperation))), new MinMemberSizeConstraint(2)),
                new MinMemberSizeConstraint(1),
                new SinglePredecessorConstraint(),
                new SingleSuccessorConstraint()
            };
            var ocl = ConstraintProvider.GenerateOcl(constraints);
            Console.WriteLine("Generated OCL:");
            Console.WriteLine(ocl);
            OclTestProvider.AddConstraints(new[] {"TestingMSAGL"}, ocl, false, true);
            
            // var constraints = File.ReadAllText("Constraints/Default.ocl");
            // OclTestProvider.AddConstraints(new[] {"TestingMSAGL"}, constraints, false, true);
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
            var root = new Sequential(Graph, "Root");
            Graph.RootNode = root;
            rootSubgraph.AddSubgraph(root.Subgraph);

            var tableParts = new Sequential(Graph, "Tischteile");
            var tableTopSawing = new NodeElementary(Graph, "Tischplatte sägen");
            tableParts.AddMember(tableTopSawing);
            
            var painting = new Alternative(Graph, "Lackierung");
            var color = new NodeElementary(Graph, "Farblack auftragen");
            var clear = new NodeElementary(Graph, "Klarlack auftragen");
            painting.AddMember(color);
            painting.AddMember(clear);
            
            tableParts.AddMember(painting);
            Graph.AddEdge(tableTopSawing.NodeId, painting.NodeId);
            
            var tableLegs = new Parallel(Graph, "Tischbeine");
            for (var i = 1; i < 5; i++)
            {
                var tableLeg = new Sequential(Graph, "Tischbein " + i);
                var tableLegSawing = new NodeElementary(Graph, "Tischbein " + i + " sägen");
                tableLeg.AddMember(tableLegSawing);
                painting = new Alternative(Graph, "Lackierung");
                color = new NodeElementary(Graph, "Farblack auftragen");
                clear = new NodeElementary(Graph, "Klarlack auftragen");
                painting.AddMember(color);
                painting.AddMember(clear);
            
                tableLeg.AddMember(painting);
                Graph.AddEdge(tableLegSawing.NodeId, painting.NodeId);
                
                tableLegs.AddMember(tableLeg);
            }
            
            tableParts.AddMember(tableLegs);
            
            var tableConstruction = new Parallel(Graph, "Tischteile zusammenschrauben");
            for (var i = 1; i < 5; i++)
            {
                var addLeg = new NodeElementary(Graph, "Tischbein " + i + " an Tischplatte schrauben");
                tableConstruction.AddMember(addLeg);
            }
            
            Graph.AddEdge(tableParts.NodeId, tableConstruction.NodeId);
            root.AddMember(tableParts);
            root.AddMember(tableConstruction);

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
                if (!nodeComplex.AddMember(node))
                {
                    if(node is NodeElementary elementary) 
                        Graph.RemoveNode(elementary.Node);
                    MessageBox.Show("Could not add to member list\n" + string.Join(", ", nodeComplex.Composite.ConsumeErrors()));
                    // nodeComplex.Composite.Errors.Clear();
                }
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

            var sequence = new ActionSequence();
            foreach (var viewerNode in selectedNodes)
            {
                var composite = GetComposite(Graph.GetNodeById(viewerNode.Node.Id));
                var parentComposite = Graph.GetComplexNodeById(composite.ParentId).Composite;
                sequence.Enqueue(new RemoveMemberAction(parentComposite, composite));
                sequence.Enqueue(new AddMemberAction(newComplex.Composite, composite));
            }
            
            sequence.Enqueue(new AddMemberAction(targetNode.Composite, newComplex.Composite));

            try
            {
                using var scope = new TransactionScope();
                sequence.Enlist();
                sequence.Execute();
                scope.Complete();
            }
            catch (TransactionException e)
            {
                MessageBox.Show("Cannot group because of the following error:\n" + sequence.Error);
                Graph.RemoveNode(newComplex.Subgraph);
                return;
            }


            foreach (var viewerNode in selectedNodes)
            {
                //workaround for select issue
                GraphViewer.LayoutEditor.RemoveObjDraggingDecorations(viewerNode);

                var node = Graph.GetNodeById(viewerNode.Node.Id);
                var parent = Graph.GetComplexNodeById(node.ParentId);

                if (node is NodeElementary elementary)
                {
                    parent.Subgraph.RemoveNode(elementary.Node);
                    newComplex.Subgraph.AddNode(elementary.Node);
                    elementary.Composite.ParentId = newComplex.Composite.DrawingNodeId;
                    elementary.ParentId = newComplex.Composite.DrawingNodeId;
                }
                else
                if(node is NodeComplex complexNode)
                {
                    parent.Subgraph.RemoveNode(complexNode.Subgraph);
                    newComplex.Subgraph.AddSubgraph(complexNode.Subgraph);
                    complexNode.Composite.ParentId = newComplex.Composite.DrawingNodeId;
                    complexNode.ParentId = newComplex.Composite.DrawingNodeId;
                }
            }

            targetNode.Subgraph.AddSubgraph(newComplex.Subgraph);
            newComplex.Composite.ParentId = targetNode.Composite.DrawingNodeId;
            newComplex.ParentId = targetNode.Composite.DrawingNodeId;
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

        public Composite GetComposite(IWithId withId)
        {
            if (withId is NodeComplex nodeComplex) return nodeComplex.Composite;
            if (withId is NodeElementary nodeElementary) return nodeElementary.Composite;
            return null;
        }

        public void RegisterEdge(Edge edge)
        {
            if (edge.Source != edge.Target)
            {
                var sourceComposite = GetComposite(Graph.GetNodeById(edge.SourceNode.Id));
                var targetComposite = GetComposite(Graph.GetNodeById(edge.TargetNode.Id));

                if (sourceComposite != null && targetComposite != null)
                {
                    var sequence = new ActionSequence();
                    sequence.Enqueue(new SetSuccessorAction(sourceComposite, targetComposite));
                    sequence.Enqueue(new SetPredecessorAction(targetComposite, sourceComposite));

                    try
                    {
                        using var scope = new TransactionScope();
                        sequence.Enlist();
                        sequence.Execute();
                        scope.Complete();
                    }
                    catch (TransactionException _)
                    {
                        Graph.RemoveEdge(edge);
                        refreshLayout();
                        MessageBox.Show("Could not add edge\n" + sequence.Error);
                    }
                }
            }
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
                if (!nodeComplex.AddMember(complex))
                {
                    if(complex is NodeComplex complexNode) 
                        Graph.RemoveNode(complexNode.Subgraph);
                    MessageBox.Show("Could not add to member list\n" + string.Join(", ", nodeComplex.Composite.ConsumeErrors()));
                }
                else
                {
                    refreshLayout();
                }
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
                        if (!parentComposite.RemoveMember(childComposite))
                        {
                            MessageBox.Show("Could not remove from member list\n" + string.Join(", ", parentComposite.Composite.ConsumeErrors()));
                        }
                        else
                        {
                            // removes straying nodes from Graph
                            Graph.RemoveNode(viewerNode.Node);
                        }
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
                "Sequential" => new Sequential(Graph, routedOperation),
                "Node" => new NodeElementary(Graph, ""),
                _ => new NodeElementary(Graph, routedOperation)
            };
        }
    }
}