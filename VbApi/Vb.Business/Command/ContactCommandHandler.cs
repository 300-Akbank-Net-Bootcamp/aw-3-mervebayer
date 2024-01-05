using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vb.Base.Response;
using Vb.Business.Cqrs;
using Vb.Data;
using Vb.Data.Entity;
using Vb.Schema;

namespace Vb.Business.Command;
public class ContactCommandHandler : IRequestHandler<CreateContactCommand, ApiResponse<ContactResponse>>, IRequestHandler<UpdateContactCommand, ApiResponse>, IRequestHandler<DeleteContactCommand, ApiResponse>
{
    private readonly VbDbContext _dbContext;
    private readonly IMapper _mapper;

    public ContactCommandHandler(VbDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ApiResponse<ContactResponse>> Handle(CreateContactCommand request, CancellationToken cancellationToken){
        var checkIfExist = await _dbContext.Set<Contact>().FirstOrDefaultAsync(x => x.Information == request.Model.Information, cancellationToken);
        if(checkIfExist != null){
            return new ApiResponse<ContactResponse>($"{request.Model.Information} is used by another customer.");
        }

        var entity = _mapper.Map<ContactRequest, Contact>(request.Model);
        var entityResult = await _dbContext.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var mapped = _mapper.Map<Contact, ContactResponse>(entityResult.Entity);
        return new ApiResponse<ContactResponse>(mapped);

    }
        
    public async Task<ApiResponse> Handle(UpdateContactCommand request, CancellationToken cancellationToken){
        var entity = await _dbContext.Set<Contact>().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null)
            {
                return new ApiResponse("Record not found");
            }
        entity.ContactType = request.Model.ContactType;
        entity.Information = request.Model.Information;
        entity.IsDefault = request.Model.IsDefault;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse();
    }

    public async Task<ApiResponse> Handle(DeleteContactCommand request, CancellationToken cancellationToken){
        var entity = await _dbContext.Set<Contact>().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (entity == null)
            {
                return new ApiResponse("Record not found");
            }
        entity.IsActive = false;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return new ApiResponse();
    }
}
