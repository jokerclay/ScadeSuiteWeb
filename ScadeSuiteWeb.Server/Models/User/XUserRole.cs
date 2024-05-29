using Microsoft.AspNetCore.Identity;

namespace ScadeSuiteWeb.Server.Models.User;

public class XUserRole : IdentityUserRole<int>
{
    public virtual XUser User { get; private set; }
    public virtual XRole Role { get; private set; }
        
    /*
    public XUserRole(){}
    public XUserRole(int? roleId, int? userId = null)
    {
        if (roleId.HasValue)
        {
            RoleId = roleId.Value;
        }

        if (userId.HasValue)
        {
            UserId = userId.Value;
        }
    }
*/
}