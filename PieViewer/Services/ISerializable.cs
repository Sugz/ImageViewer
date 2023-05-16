using System.Text.Json.Nodes;

namespace ImageViewer.Services;

public interface ISerializable
{
    string GetSerializedName();
    void Deserialize(JsonNode jsonObject);
}
