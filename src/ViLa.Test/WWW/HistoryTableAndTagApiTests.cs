using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using ViLa.Model;
using ViLa.WWW.Api._1_.History;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebParameter;
using WebExpress.WebUI.WebIcon;
using Xunit;

namespace ViLa.Test.WWW;

public class HistoryTableAndTagApiTests
{
    private static IRequest CreateTestRequest(params (string Key, IParameter Param)[] parameters)
    {
        var req = (IRequest)RuntimeHelpers.GetUninitializedObject(typeof(Request));
        var paramField = typeof(RequestBase).GetField("_param", BindingFlags.NonPublic | BindingFlags.Instance);
        var dict = Activator.CreateInstance(paramField!.FieldType, true);
        paramField.SetValue(req, dict);

        if (parameters != null)
        {
            var addMethod = dict!.GetType().GetMethod("TryAdd", new[] { typeof(string), typeof(IParameter) });
            foreach (var (key, param) in parameters)
            {
                addMethod!.Invoke(dict, new object[] { key, param });
            }
        }

        return req;
    }

    private static IEnumerable<RestApiTableColumn> GetColumns(Table table, IRequest request)
    {
        var method = typeof(Table).GetMethod("RetrieveColums", BindingFlags.NonPublic | BindingFlags.Instance);
        return (IEnumerable<RestApiTableColumn>)method!.Invoke(table, new object[] { request })!;
    }

    private static IEnumerable<RestApiTableRow> GetRows(Table table, IRequest request)
    {
        var method = typeof(Table).GetMethod("RetrieveRows", BindingFlags.NonPublic | BindingFlags.Instance);
        return (IEnumerable<RestApiTableRow>)method!.Invoke(table, new object[] { null!, null!, null!, request })!;
    }

    [Fact]
    public void HistoryTable_RetrieveColums_HasIconsOnAllColumns()
    {
        // Arrange
        var table = new Table();

        // Act
        var columns = GetColumns(table, null!).ToList();

        // Assert
        Assert.Equal(5, columns.Count);

        var fromCol = columns.FirstOrDefault(c => c.Id == "from");
        Assert.NotNull(fromCol);
        Assert.Equal(new IconPlay().ToString(), fromCol.Icon);

        var tillCol = columns.FirstOrDefault(c => c.Id == "till");
        Assert.NotNull(tillCol);
        Assert.Equal(new IconStop().ToString(), tillCol.Icon);

        var powerCol = columns.FirstOrDefault(c => c.Id == "power");
        Assert.NotNull(powerCol);
        Assert.Equal(new IconBolt().ToString(), powerCol.Icon);

        var costCol = columns.FirstOrDefault(c => c.Id == "cost");
        Assert.NotNull(costCol);
        Assert.Equal(new IconEuroSign().ToString(), costCol.Icon);

        var tagCol = columns.FirstOrDefault(c => c.Id == "tag");
        Assert.NotNull(tagCol);
        Assert.Equal(new IconTag().ToString(), tagCol.Icon);
    }

    [Fact]
    public void HistoryTable_RetrieveColums_TagColumnHasEditableTemplate()
    {
        // Arrange
        var table = new Table();

        // Act
        var columns = GetColumns(table, null!).ToList();
        var tagCol = columns.FirstOrDefault(c => c.Id == "tag");

        // Assert
        Assert.NotNull(tagCol);
        Assert.NotNull(tagCol.Template);
        var tagTemplate = Assert.IsType<RestApiTableColumnTemplateTag>(tagCol.Template);
        Assert.True(tagTemplate.Editable);
        Assert.Equal("tag", tagTemplate.Type);
    }

    [Fact]
    public void HistoryTable_RetrieveRows_SetsRestApiOnRows()
    {
        // Arrange
        var table = new Table();
        var testId = Guid.NewGuid().ToString();
        var log = new MeasurementLog
        {
            ID = testId,
            Tag = "TestTag"
        };
        log.Measurements.Add(new MeasurementItem { MeasurementTimePoint = DateTime.Now, Impulse = 10 });
        ViewModel.Instance.GetHistoryMeasurementLogs(); // initialize if needed
        var field = typeof(ViewModel).GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
            .FirstOrDefault(f => f.FieldType == typeof(List<MeasurementLog>));
        
        var list = field?.GetValue(ViewModel.Instance) as List<MeasurementLog>;
        list?.Add(log);

        try
        {
            // Act
            var rows = GetRows(table, null!).ToList();
            var testRow = rows.FirstOrDefault(r => r.Id.ToString() == testId);

            // Assert
            Assert.NotNull(testRow);
            Assert.NotNull(testRow.RestApi);
            Assert.Contains($"id={testId}", testRow.RestApi);
        }
        finally
        {
            list?.Remove(log);
        }
    }

    [Fact]
    public void HistoryTag_Update_MissingId_ReturnsBadRequest()
    {
        // Arrange
        var endpoint = new Tag();
        var request = CreateTestRequest();

        // Act
        var response = endpoint.Update(request);

        // Assert
        Assert.IsType<ResponseBadRequest>(response);
    }

    [Fact]
    public void HistoryTag_Update_NotFound_ReturnsNotFound()
    {
        // Arrange
        var endpoint = new Tag();
        var request = CreateTestRequest(("id", new ParameterId(Guid.NewGuid().ToString())));

        // Act
        var response = endpoint.Update(request);

        // Assert
        Assert.IsType<ResponseNotFound>(response);
    }

    [Fact]
    public void HistoryTag_Update_ValidId_UpdatesTagAndReturnsOk()
    {
        // Arrange
        var endpoint = new Tag();
        var testId = Guid.NewGuid().ToString();
        var log = new MeasurementLog
        {
            ID = testId,
            Tag = "OldTag"
        };
        log.Measurements.Add(new MeasurementItem { MeasurementTimePoint = DateTime.Now, Impulse = 5 });

        var field = typeof(ViewModel).GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
            .FirstOrDefault(f => f.FieldType == typeof(List<MeasurementLog>));
        var list = field?.GetValue(ViewModel.Instance) as List<MeasurementLog>;
        list?.Add(log);

        try
        {
            var request = CreateTestRequest(
                ("id", new ParameterId(testId)),
                ("tag", new Parameter("tag", "NewChipTag;SecondTag", ParameterScope.Parameter))
            );

            // Act
            var updateResponse = endpoint.Update(request);

            // Assert
            Assert.IsType<ResponseOK>(updateResponse);
            Assert.Equal("NewChipTag;SecondTag", log.Tag);

            // Verify Retrieve
            var getResponse = endpoint.Retrieve(request);
            Assert.IsType<ResponseOK>(getResponse);
            Assert.Contains("NewChipTag;SecondTag", getResponse.Content?.ToString() ?? "");
        }
        finally
        {
            list?.Remove(log);
        }
    }

    [Fact]
    public void ViewModel_Tags_SplitsBothSpacesAndSemicolons()
    {
        // Arrange
        var testId1 = Guid.NewGuid().ToString();
        var testId2 = Guid.NewGuid().ToString();
        var log1 = new MeasurementLog { ID = testId1, Tag = "TagA TagB" };
        var log2 = new MeasurementLog { ID = testId2, Tag = "TagC;TagD;TagA" };

        var field = typeof(ViewModel).GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
            .FirstOrDefault(f => f.FieldType == typeof(List<MeasurementLog>));
        var list = field?.GetValue(ViewModel.Instance) as List<MeasurementLog>;
        list?.Add(log1);
        list?.Add(log2);

        try
        {
            // Act
            var tags = ViewModel.Instance.Tags.ToList();

            // Assert
            Assert.Contains("TagA", tags);
            Assert.Contains("TagB", tags);
            Assert.Contains("TagC", tags);
            Assert.Contains("TagD", tags);
        }
        finally
        {
            list?.Remove(log1);
            list?.Remove(log2);
        }
    }
}
