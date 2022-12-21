
using Microsoft.EntityFrameworkCore;
using NewsAggregator.Core.Abstractions;
using NewsAggregator.Data.Abstractions;
using Serilog;

namespace NewsAggregator.Business.ServicesImplementations;

public class RoleService : IRoleService
{
    private readonly IUnitOfWork _unitOfWork;

    public RoleService(IUnitOfWork unitOfWork)
    {
     
        _unitOfWork = unitOfWork;
    }

    public async Task<string> GetRoleNameByIdAsync(Guid id)
    {
        try
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(id);
            return role != null
                ? role.Name
                : string.Empty;
        }
        
        catch (Exception exception)
        {
            Log.Error(exception, "MethodName : GetRoleNameByIdAsync");
            throw;
        }
    }

    public async Task<Guid?> GetRoleIdByNameAsync(string name)
    {
        try
        {
            var role = await _unitOfWork.Roles.FindBy(role1 => role1.Name.Equals(name))
            .FirstOrDefaultAsync();
            return role?.Id;
        }
        
        catch (Exception exception)
        {
            Log.Error(exception, "MethodName : GetRoleIdByNameAsync");
            throw;
        }
    }
}