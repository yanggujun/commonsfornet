using System;

namespace Commons.Json.Mapper
{
	public class MapEngine<T> : IMapEngine<T>
	{
		private IJsonObjectMapper<T> mapper; 
		public MapEngine(IJsonObjectMapper<T> mapper)
		{
			this.mapper = mapper;
		}

		public T Map(JValue jsonValue)
		{
			throw new NotImplementedException();
		}


		public JValue Map(T target)
		{
			throw new NotImplementedException();
		}
	}
}
