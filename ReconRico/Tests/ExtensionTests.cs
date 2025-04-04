using NUnit.Framework;
using ReconRico.Extensions;

namespace ReconRico.Tests;

[TestFixture]
public class ExtensionTests
{
    [Test]
    public void EmptySource_IsSubsetOfAny_ReturnsTrue()
    {
        Assert.That(Array.Empty<int>()
            .IsSubsetOf([1, 2, 3]), Is.True);
    }

    [Test]
    public void EqualSets_ReturnsTrue()
    {
        Assert.That(new[] { 1, 2 }
            .IsSubsetOf([1, 2]), Is.True);
    }

    [Test]
    public void ProperSubset_ReturnsTrue()
    {
        Assert.That(new[] { 1 }
            .IsSubsetOf([1, 2]), Is.True);
    }

    [Test]
    public void NotASubset_ReturnsFalse()
    {
        Assert.That(new[] { 1, 4 }
            .IsSubsetOf(new[] { 1, 2, 3 }), Is.False);
    }

    [Test]
    public void EmptyTarget_NonEmptySource_ReturnsFalse()
    {
        Assert.That(new[] { 1 }
            .IsSubsetOf(Array.Empty<int>()), Is.False);
    }
}