
namespace Commons.Json.Mapper
{
	public interface IMapEngine<T>
	{
		T Map(JValue jsonValue);
		JValue Map(T target);
	}
}
