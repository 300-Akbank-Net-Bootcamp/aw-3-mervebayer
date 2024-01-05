using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vb.Base.Response;
using Vb.Business.Cqrs;
using Vb.Data;
using Vb.Data.Entity;
using Vb.Schema;

namespace Vb.Business.Command;
public class AddressCommandHandler : IRequestHandler<CreateAddressCommand, ApiResponse<AddressResponse>>, 
                                     IRequestHandler<UpdateAddressCommand, ApiResponse>,
                                     IRequestHandler<DeleteAddressCommand, ApiResponse>
{
    private readonly VbDbContext _dbContext;
    private readonly IMapper _mapper;
    public AddressCommandHandler(VbDbContext dbContext, IMapper mapper)
    {
        _dbContext =  dbContext;
        _mapper = mapper;
    }

    public async Task<ApiResponse<AddressResponse>> Handle(CreateAddressCommand request, CancellationToken cancellationToken)
    {
        var check = await _dbContext.Set<Address>().Include(x => x.Customer).Where(x => x.Id == request.Model.Id).FirstOrDefaultAsync(cancellationToken);
        if (check != null)
        {
            return new ApiResponse<AddressResponse>($"{request.Model.Id} is used by another customer."); // check again Id??
        }

        var entity = _mapper.Map<AddressRequest, Address>(request.Model);

        var entityResult = await _dbContext.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var mapped = _mapper.Map<Address, AddressResponse>(entityResult.Entity);
        return new ApiResponse<AddressResponse>(mapped);
    }

    public async Task<ApiResponse> Handle (UpdateAddressCommand request, CancellationToken cancellationToken)
    {
       var check = await _dbContext.Set<Address>().Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken); 
       if( check == null){
            return new ApiResponse("Record not found");
       }

       check.Address1 = request.Model.Address1;
       check.Address2 = request.Model.Address2;
       check.Country = request.Model.Country;
       check.County = request.Model.County;
       check.PostalCode = request.Model.PostalCode;
       check.City = request.Model.City;
       check.IsDefault = request.Model.IsDefault;

       await _dbContext.SaveChangesAsync(cancellationToken);
       return new ApiResponse();
    }
    public async Task<ApiResponse> Handle (DeleteAddressCommand request, CancellationToken cancellationToken)
    {
       var check = await _dbContext.Set<Address>().Where(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken); 
       if( check == null){
            return new ApiResponse("Record not found");
       }

       check.IsActive = false;

       await _dbContext.SaveChangesAsync(cancellationToken);
       return new ApiResponse();
    }

}