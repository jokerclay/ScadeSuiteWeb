using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScadeSuiteWeb.Shared.Utils;

public static class CommonHelper
{
    public static string SystemResourcesFilePath { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "Resources");

}
