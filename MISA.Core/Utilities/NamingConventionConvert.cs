using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.Core.Utilities
{
    public static class NamingConventionConvert
    {
        /// <summary>
        /// Chuyển đổi chuỗi từ PascalCase/camelCase sang snake_case
        /// </summary>
        /// <param name="pascalCaseString">Chuỗi PascalCase/camelCase</param>
        /// <returns>Chuỗi snake_case</returns>
        /// Created by: LQThinh (04/12/2025)
        public static string ToSnakeCase(this string pascalCaseString)
        {
            if (string.IsNullOrEmpty(pascalCaseString))
            {
                return pascalCaseString;
            }
            var sb = new StringBuilder();
            sb.Append(char.ToLowerInvariant(pascalCaseString[0]));

            for (int i = 1; i < pascalCaseString.Length; i++)
            {
                char current = pascalCaseString[i];
                if (char.IsUpper(current))
                {
                    if (i + 1 < pascalCaseString.Length && char.IsLower(pascalCaseString[i + 1]))
                    {
                        sb.Append('_');
                    }
                    else if (i - 1 >= 0 && char.IsLower(pascalCaseString[i - 1]))
                    {
                        sb.Append('_');
                    }

                    sb.Append(char.ToLowerInvariant(current));
                }
                else
                {
                    sb.Append(current);
                }
            }

            return sb.ToString();
        }
    }
}
