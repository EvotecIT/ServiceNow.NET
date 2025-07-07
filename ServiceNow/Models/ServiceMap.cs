using System.Collections.Generic;
using System.Linq;

namespace ServiceNow.Models;

/// <summary>
/// Represents a service map consisting of nodes and dependencies.
/// </summary>
public class ServiceMap {
    /// <summary>
    /// Gets or sets the nodes that make up the map.
    /// </summary>
    public List<ServiceMapNode> Nodes { get; set; } = new();

    /// <summary>
    /// Gets or sets the dependencies between nodes.
    /// </summary>
    public List<ServiceMapDependency> Dependencies { get; set; } = new();

    /// <summary>
    /// Traverses dependencies starting from the specified node.
    /// </summary>
    /// <param name="rootId">The starting node id.</param>
    public IEnumerable<ServiceMapNode> Traverse(string rootId) {
        var visited = new HashSet<string>();
        var queue = new Queue<string>();
        queue.Enqueue(rootId);
        while (queue.Count > 0) {
            var current = queue.Dequeue();
            if (!visited.Add(current)) {
                continue;
            }
            var node = Nodes.FirstOrDefault(n => n.Id == current);
            if (node is not null) {
                yield return node;
            }
            foreach (var dep in Dependencies.Where(d => d.Parent == current)) {
                if (dep.Child is not null && !visited.Contains(dep.Child)) {
                    queue.Enqueue(dep.Child);
                }
            }
        }
    }
}
