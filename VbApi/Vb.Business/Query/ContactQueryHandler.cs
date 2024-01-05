using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vb.Base.Response;
using Vb.Business.Cqrs;
using Vb.Data;
using Vb.Data.Entity;
using Vb.Schema;

namespace Vb.Business.Query;
public class ContactQueryHandler :IRequestHandler<GetAllContactQuery, ApiResponse<List<ContactResponse>>>,
    IRequestHandler<GetContactByIdQuery, ApiResponse<ContactResponse>>,
    IRequestHandler<GetContactByParameterQuery, ApiResponse<List<ContactResponse>>>
{
    private readonly VbDbContext _dbContext;
    private readonly IMapper _mapper;

    public ContactQueryHandler(VbDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<ContactResponse>>> Handle(GetAllContactQuery request, CancellationToken cancellationToken){
        var list = await _dbContext.Set<Contact>().Include(x => x.Customer).Where(x => x.IsActive == true)
            .ToListAsync(cancellationToken);
        
        var mappedList = _mapper.Map<List<Contact>, List<ContactResponse>>(list);
        return new ApiResponse<List<ContactResponse>>(mappedList);
    }
    public async Task<ApiResponse<ContactResponse>> Handle(GetContactByIdQuery request,
        CancellationToken cancellationToken)
        {
            var entity =  await _dbContext.Set<Contact>().Include(x => x.Customer)
                .FirstOrDefaultAsync(x => x.Id == request.Id  && x.IsActive == true, cancellationToken);

            if (entity == null)
            {
                return new ApiResponse<ContactResponse>("Record not found");
            }
            
            var mapped = _mapper.Map<Contact, ContactResponse>(entity);
            return new ApiResponse<ContactResponse>(mapped);
        }

    public async Task<ApiResponse<List<ContactResponse>>> Handle(GetContactByParameterQuery request,
        CancellationToken cancellationToken)
    {
        var list =  await _dbContext.Set<Contact>().Include(x => x.Customer)
            .Where(x =>
            x.ContactType.ToUpper().Contains(request.ContactType.ToUpper()) ||
            x.Information.ToUpper().Contains(request.Information.ToUpper()) 
        ).ToListAsync(cancellationToken);
        
        var mappedList = _mapper.Map<List<Contact>, List<ContactResponse>>(list);
        return new ApiResponse<List<ContactResponse>>(mappedList);
    }
}
