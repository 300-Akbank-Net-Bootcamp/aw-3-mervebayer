using MediatR;
using Vb.Base.Response;
using Vb.Schema;

namespace Vb.Business.Cqrs;

public record CreateAddressCommand(AddressRequest Model) : IRequest<ApiResponse<AddressResponse>>;
public record UpdateAddressCommand(int Id, AddressRequest Model) : IRequest<ApiResponse>;
public record DeleteAddressCommand(int Id) :  IRequest<ApiResponse>;
public record GetAllAddressQuery() : IRequest<ApiResponse<List<AddressResponse>>>;
public record GetAddressByIdQuery(int Id) : IRequest<ApiResponse<AddressResponse>>;
public record GetAllAddressByParameterQuery(string Address1, string Address2,string Country,string City,string County,string PostalCode) : IRequest<ApiResponse<List<AddressResponse>>>;