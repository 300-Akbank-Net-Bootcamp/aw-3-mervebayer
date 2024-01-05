using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vb.Base.Response;
using Vb.Business.Cqrs;
using Vb.Data;
using Vb.Data.Entity;
using Vb.Schema;

namespace Vb.Business.Query;
public class AccountQueryHandler :IRequestHandler<GetAllAccountQuery, ApiResponse<List<AccountResponse>>>,
    IRequestHandler<GetAccountByIdQuery, ApiResponse<AccountResponse>>,
    IRequestHandler<GetAccountByParameterQuery, ApiResponse<List<AccountResponse>>>
{
    private readonly VbDbContext _dbContext;
    private readonly IMapper _mapper;

    public AccountQueryHandler(VbDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<AccountResponse>>> Handle(GetAllAccountQuery request, CancellationToken cancellationToken){
        var list = await _dbContext.Set<Account>().Include(x => x.Customer)
            .ToListAsync(cancellationToken);
        
        var mappedList = _mapper.Map<List<Account>, List<AccountResponse>>(list);
        return new ApiResponse<List<AccountResponse>>(mappedList);
    }
    public async Task<ApiResponse<AccountResponse>> Handle(GetAccountByIdQuery request,
        CancellationToken cancellationToken)
        {
            var entity =  await _dbContext.Set<Account>().Include(x => x.Customer)
                .FirstOrDefaultAsync(x => x.AccountNumber == request.Id, cancellationToken);

            if (entity == null)
            {
                return new ApiResponse<AccountResponse>("Record not found");
            }
            
            var mapped = _mapper.Map<Account, AccountResponse>(entity);
            return new ApiResponse<AccountResponse>(mapped);
        }

    public async Task<ApiResponse<List<AccountResponse>>> Handle(GetAccountByParameterQuery request,
        CancellationToken cancellationToken)
    {
        var list =  await _dbContext.Set<Account>().Include(x => x.Customer)
            .Where(x =>
            x.IBAN.ToUpper().Contains(request.IBAN.ToUpper()) ||
              x.Name.ToUpper().Contains(request.Name.ToUpper()) 
        ).ToListAsync(cancellationToken);
        
        var mappedList = _mapper.Map<List<Account>, List<AccountResponse>>(list);
        return new ApiResponse<List<AccountResponse>>(mappedList);
    }
}
