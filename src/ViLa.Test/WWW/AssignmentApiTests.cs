using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using ViLa.Model;
using ViLa.WWW.Api._1_.History;
using WebExpress.WebCore.WebMessage;

namespace ViLa.Test.WWW;

/// <summary>Verifies assignment request validation and optimistic concurrency.</summary>
public sealed class AssignmentApiTests
{
    /// <summary>Refuses malformed or incomplete move requests.</summary>
    /// <param name="json">The invalid request body.</param>
    [Theory]
    [InlineData("{")]
    [InlineData("[]")]
    [InlineData("{}")]
    [InlineData("{\"cardId\":\"test\",\"columnId\":\"none\"}")]
    [InlineData("{\"cardId\":\"test\",\"columnId\":\"none\",\"source\":\"car\",\"action\":3}")]
    public void Update_InvalidPayload_ReturnsBadRequest(string json)
    {
        // arrange
        var endpoint = new Assignments();
        var request = CreateRequest(json);

        // act
        var response = endpoint.Update(request);

        // validation
        Assert.IsType<ResponseBadRequest>(response);
    }

    /// <summary>Refuses a stale source without changing the persisted vehicle assignment.</summary>
    [Fact]
    public void Update_StaleSource_PreservesAssignment()
    {
        // arrange
        var log = new MeasurementLog { ID = Guid.NewGuid().ToString(), VehicleId = "current-vehicle" };
        var field = typeof(ViewModel).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
            .Single(item => item.FieldType == typeof(List<MeasurementLog>));
        var history = (List<MeasurementLog>)field.GetValue(ViewModel.Instance)!;
        history.Add(log);
        try
        {
            var request = CreateRequest(JsonSerializer.Serialize(new { cardId = log.ID, source = "previous-vehicle", columnId = "none" }));

            // act
            var response = new Assignments().Update(request);

            // validation
            Assert.IsType<ResponseConflict>(response);
            Assert.Equal("current-vehicle", log.VehicleId);
        }
        finally
        {
            history.Remove(log);
        }
    }

    /// <summary>Creates an isolated request with a JSON body.</summary>
    /// <param name="json">The serialized request payload.</param>
    /// <returns>The request passed to the endpoint.</returns>
    private static Request CreateRequest(string json)
    {
        var request = (Request)RuntimeHelpers.GetUninitializedObject(typeof(Request));
        typeof(Request).GetProperty(nameof(Request.Content))!.SetValue(request, Encoding.UTF8.GetBytes(json));
        return request;
    }
}
