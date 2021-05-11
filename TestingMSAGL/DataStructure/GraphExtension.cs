using System.Collections.Generic;
using System.Linq;
using Microsoft.Msagl.Drawing;
using TestingMSAGL.DataLinker;

namespace TestingMSAGL.DataStructure
{
    public class GraphExtension : Graph
    {

        private HashSet<IWithId> DataLinkerNodes = new HashSet<IWithId>();
        public GraphExtension(string label, string id) : base(label, id)
        {

        }

        /// <summary>
        /// May need catch if node is not found
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IWithId GetNodeById(string id)
        {
            return DataLinkerNodes.Single(x => x.NodeId.Equals(id));
        }

        /// <summary>
        /// Reduce casting
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NodeComplex GetComplexNodeById(string id)
        {
            return DataLinkerNodes.Single(x => x.NodeId.Equals(id)) as NodeComplex;
        }


        /// <summary>
        /// Returns false if item is already present.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool AddNodeWithId(IWithId item)
        {
            return DataLinkerNodes.Add(item);
        }
    }
}