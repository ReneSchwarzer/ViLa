using System.IO;

namespace ViLa.Model
{
    /// <summary>
    /// Resolves persistent storage paths while preserving existing legacy directories.
    /// </summary>
    internal static class StorageDirectory
    {
        /// <summary>
        /// Selects existing storage or a new category beneath the mounted data directory.
        /// </summary>
        /// <param name="root">The application base directory.</param>
        /// <param name="category">The measurement or archive directory name.</param>
        /// <returns>The storage path without creating the directory.</returns>
        internal static string Resolve(string root, string category)
        {
            var data = Path.Combine(root, "data");
            var current = Path.Combine(data, category);
            var legacy = Path.Combine(root, category);
            if (Directory.Exists(current))
            {
                return current;
            }

            if (Directory.Exists(legacy))
            {
                return legacy;
            }

            return Directory.Exists(data) ? current : legacy;
        }
    }
}
