using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vb.Base.Response;
using Vb.Business.Cqrs;
using Vb.Data;
using Vb.Data.Entity;
using Vb.Schema;

namespace Vb.Business.Query;

public class AddressQueryHandler :
    IRequestHandler<GetAllAddressQuery, ApiResponse<List<AddressResponse>>>,
    IRequestHandler<GetAddressByIdQuery, ApiResponse<AddressResponse>>,
    IRequestHandler<GetAllAddressByParameterQuery, ApiResponse<List<AddressResponse>>>
{
    private readonly VbDbContext dbContext;
    private readonly IMapper mapper;

    public AddressQueryHandler(VbDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    public async Task<ApiResponse<List<AddressResponse>>> Handle(GetAllAddressQuery request,
        CancellationToken cancellationToken)
    {
        var list = await dbContext.Set<Address>().Include(x => x.Customer).Where(x => x.IsActive == true).ToListAsync(cancellationToken);
        
        var mappedList = mapper.Map<List<Address>, List<AddressResponse>>(list);
         return new ApiResponse<List<AddressResponse>>(mappedList);
    }

    public async Task<ApiResponse<AddressResponse>> Handle(GetAddressByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity =  await dbContext.Set<Address>()
            .Include(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.IsActive == true, cancellationToken);

        if (entity == null)
        {
            return new ApiResponse<AddressResponse>("Record not found");
        }
        
        var mapped = mapper.Map<Address, AddressResponse>(entity);
        return new ApiResponse<AddressResponse>(mapped);
    }

      public async Task<ApiResponse<List<AddressResponse>>> Handle(GetAllAddressByParameterQuery request,
        CancellationToken cancellationToken)
    {
        var entity =  await dbContext.Set<Address>()
            .Include(x => x.Customer)
             .Where(x =>
            x.Address1.ToUpper().Contains(request.Address1.ToUpper()) ||
            x.Address2.ToUpper().Contains(request.Address2.ToUpper()) ||
            x.City.ToUpper().Contains(request.City.ToUpper()) ||
            x.County.ToUpper().Contains(request.County.ToUpper()) ||
            x.PostalCode.ToUpper().Contains(request.PostalCode.ToUpper()) ||
            x.Country.ToUpper().Contains(request.Country.ToUpper())
            ).ToListAsync(cancellationToken);

     
         var mappedList = mapper.Map<List<Address>, List<AddressResponse>>(entity);
        return new ApiResponse<List<AddressResponse>>(mappedList);
    }
}