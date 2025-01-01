using AutoMapper;

namespace Chatty.Core.Application.Mapping;

public interface IMapFrom<T>
{
    void Mapping(Profile mapper) => mapper.CreateMap(typeof(T), GetType());
}