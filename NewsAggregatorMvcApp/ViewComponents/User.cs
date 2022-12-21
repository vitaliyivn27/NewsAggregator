
using Microsoft.AspNetCore.Mvc;
using NewsAggregator.Data.Abstractions;
using Serilog;

namespace NewsAggregatorMvcApp.ViewComponents
{
    public class User : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public User(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> InvokeAsync()
        {
            try
            {
                if (HttpContext.User.Identity.Name != null)
                {
                    var userEmail = HttpContext.User.Identity.Name;
                    var user = _unitOfWork.Users.FindBy(user => user.Email.Equals(userEmail)).FirstOrDefault();
                    var userRoleName = (_unitOfWork.Roles.FindBy(role => role.Id.Equals(user.RoleId)).FirstOrDefault()).Name;

                    if (userRoleName == "Admin")
                    {
                        string userName = HttpContext.User.Identity.Name;
                        return $"Добро пожаловать - {userName}";
                    }
                    else if (userRoleName == "User")
                    {
                        string userName = HttpContext.User.Identity.Name;
                        return $"Добро пожаловать - {userName}";
                    }
                    else
                    {
                        return "Добро пожаловать - Гость";
                    }
                }
                else
                {
                    return "Добро пожаловать - Гость";
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception, "User InvokeAsync was not successful");
                throw;
            }
        }
    }
}