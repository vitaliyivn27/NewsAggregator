
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NewsAggregator.Core.Abstractions;
using NewsAggregator.Core.DataTransferObjects;
using NewsAggregator.Data.Abstractions;
using NewsAggregator.Data.CQS.Queries;
using NewsAggregator.DataBase.Entities;
using Serilog;

namespace NewsAggregator.Business.ServicesImplementations;

public class UserService : IUserService
{
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;


    public UserService(IMapper mapper,
        IConfiguration configuration,
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _mapper = mapper;
        _configuration = configuration;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }


    public async Task<bool> IsUserExists(Guid userId)
    {
        try
        {
            return await _unitOfWork.Users.Get().AnyAsync(user => user.Id.Equals(userId));
        }

        catch (Exception exception)
        {
            Log.Error(exception, "MethodName : IsUserExists");
            throw;
        }
    }

    public async Task<bool> IsUserExists(string email)
    {
        try
        {
            return await _unitOfWork.Users.Get().AnyAsync(user => user.Email.Equals(email));
        }

        catch (Exception exception)
        {
            Log.Error(exception, "MethodName : IsUserExists");
            throw;
        }
    }

    public async Task<IEnumerable<UserDto>> GetAllUsers()
    {
        try
        {
            return (await _unitOfWork.Users.GetAllAsync()).Select(user => _mapper.Map<UserDto>(user)).ToArray();
        }

        catch (Exception exception)
        {
            Log.Error(exception, "MethodName : GetAllUsers");
            throw;
        }
    }

    public async Task<bool> CheckUserPassword(string email, string password)
    {
        try
        {
            var dbPasswordHash = (await _unitOfWork.Users
                .Get().FirstOrDefaultAsync(user => user.Email.Equals(email)))
                ?.PasswordHash;

            return
                dbPasswordHash != null
                && CreateMd5($"{password}.{_configuration["Secrets:PasswordSalt"]}").Equals(dbPasswordHash);
        }

        catch (Exception exception)
        {
            Log.Error(exception, "MethodName : CheckUserPassword");
            throw;
        }
    }

    public async Task<bool> CheckUserPassword(Guid userId, string password)
    {
        try
        {
            var dbPasswordHash = (await _unitOfWork.Users.GetByIdAsync(userId))?.PasswordHash;

            return
                dbPasswordHash != null
                && CreateMd5($"{password}.{_configuration["Secrets:PasswordSalt"]}").Equals(dbPasswordHash);
        }

        catch (Exception exception)
        {
            Log.Error(exception, "MethodName : CheckUserPassword");
            throw;
        }
    }

    public async Task<int> RegisterUser(UserDto dto, string password)
    {
        try
        {
            var user = _mapper.Map<User>(dto);

            user.PasswordHash = CreateMd5($"{password}.{_configuration["Secrets:PasswordSalt"]}");

            await _unitOfWork.Users.AddAsync(user);
            return await _unitOfWork.Commit();
        }

        catch (Exception exception)
        {
            Log.Error(exception, "MethodName : RegisterUser");
            throw;
        }
    }


    public async Task<UserDto> GetUserByEmailAsync(string email)
    {
        try
        {
            var user = await _unitOfWork.Users
                .FindBy(us => us.Email.Equals(email),
                    us => us.Role)
                .FirstOrDefaultAsync();

            return
                user != null ?
                _mapper.Map<UserDto>(user) :
                null;
        }

        catch (Exception exception)
        {
            Log.Error(exception, "MethodName : GetUserByEmailAsync");
            throw;
        }
    }

    private string CreateMd5(string password)
    {
        try
        {
            var passwordSalt = _configuration["Secrets:PasswordSalt"];

            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                var inputBytes = System.Text.Encoding.UTF8.GetBytes(password + passwordSalt);
                var hashBytes = md5.ComputeHash(inputBytes);

                return Convert.ToHexString(hashBytes);
            }
        }

        catch (Exception exception)
        {
            Log.Error(exception, "MethodName : CreateMd5");
            throw;
        }
    }

    public async Task<UserDto?> GetUserByRefreshTokenAsync(Guid token)
    {
        var user = await _mediator.Send(new GetUserByRefreshTokenQuery() { RefreshToken = token });

        return user;
    }



}