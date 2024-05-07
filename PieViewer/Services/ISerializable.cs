using System.Text.Json.Nodes;

namespace PieViewer.Services;

public interface ISerializable
{
    string GetSerializedName();
    void Deserialize(JsonNode jsonObject);
}
