using Xunit;

// Disable parallel test execution so singleton ViewModel.Instance state modifications don't race between tests.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
