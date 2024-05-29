using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ScadeSuiteWeb.Server.Models.User;

public class XUser : IdentityUser<int>
{

    [StringLength(50)]
    public string DisplayName { get; set; } = string.Empty;
}