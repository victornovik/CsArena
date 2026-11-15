using Xunit;
using Xunit.Sdk;
using Xunit.v3;

// xunit v4 native parallelism: ParallelMode.All also runs tests within the same class in parallel
// (replaces Meziantou.Xunit.v3.ParallelTestFramework). MaxThreads < 0 = no thread cap.
[assembly: Parallelization(Mode = ParallelMode.All, MaxThreads = -1)]
