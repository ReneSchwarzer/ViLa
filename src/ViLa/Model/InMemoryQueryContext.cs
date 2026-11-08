using System;
using WebExpress.WebIndex.Queries;

namespace ViLa.Model
{
    /// <summary>
    /// Trivial in-memory implementation of IQueryContext. The actual
    /// filtering, sorting, and paging is performed by the endpoint's
    /// RetrieveRows override; this context only exists to satisfy
    /// the RestApiTable protocol.
    /// </summary>
    public sealed class InMemoryQueryContext : IQueryContext
    {
        /// <summary>
        /// Releases resources. No-op for the in-memory implementation.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
