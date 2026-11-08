using System;
using System.IO;
using System.Xml.Serialization;
using ViLa.Model;
using Xunit;

namespace ViLa.Test.Model;

/// <summary>
/// Unit tests for the <see cref="CommentItem"/> class.
/// </summary>
public class CommentItemTest
{
    /// <summary>
    /// Tests getting and setting all properties.
    /// </summary>
    [Fact]
    public void Properties_SetAndGet_ReturnsExpectedValues()
    {
        // Arrange
        var created = new DateTime(2026, 3, 15, 10, 30, 0);
        var updated = new DateTime(2026, 3, 15, 11, 45, 0);
        var comment = new CommentItem
        {
            Guid = "abc-123",
            Comment = "This is a comment",
            Created = created,
            Updated = updated
        };

        // Assert
        Assert.Equal("abc-123", comment.Guid);
        Assert.Equal("This is a comment", comment.Comment);
        Assert.Equal(created, comment.Created);
        Assert.Equal(updated, comment.Updated);
    }

    /// <summary>
    /// Tests XML serialization and deserialization of a comment item.
    /// </summary>
    [Fact]
    public void XmlSerialization_RoundTrip_PreservesProperties()
    {
        // Arrange
        var comment = new CommentItem
        {
            Guid = "guid-xyz",
            Comment = "Charging finished successfully",
            Created = new DateTime(2026, 5, 1, 8, 0, 0),
            Updated = new DateTime(2026, 5, 1, 9, 0, 0)
        };

        var serializer = new XmlSerializer(typeof(CommentItem));
        using var memoryStream = new MemoryStream();

        // Act
        serializer.Serialize(memoryStream, comment);
        memoryStream.Position = 0;
        var deserialized = (CommentItem?)serializer.Deserialize(memoryStream);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal("Charging finished successfully", deserialized.Comment);
        Assert.Equal(comment.Created, deserialized.Created);
        Assert.Equal(comment.Updated, deserialized.Updated);
    }
}
