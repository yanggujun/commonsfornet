using Commons.Utils;

namespace Commons.Json.Mapper
{
    public interface IJsonConverter<T>
    {
        JValue ToJson(T target);
        T Convert(JValue value);
    }
}
