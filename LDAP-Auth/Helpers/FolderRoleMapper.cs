using System;
using System.Collections.Generic;
using System.Linq;
using Jellyfin.Plugin.LDAP_Auth.Config;

namespace Jellyfin.Plugin.LDAP_Auth.Helpers
{
    /// <summary>
    /// Resolves library access from LDAP role attribute values.
    /// </summary>
    public static class FolderRoleMapper
    {
        /// <summary>
        /// Gets the union of libraries granted by matching roles.
        /// </summary>
        /// <param name="roles">The values of the user's LDAP role attribute.</param>
        /// <param name="mappings">The configured folder role mappings.</param>
        /// <returns>Distinct library IDs, or an empty array if no roles match.</returns>
        public static string[] GetEnabledFolders(IEnumerable<string> roles, IEnumerable<FolderRoleMap> mappings)
        {
            var roleSet = new HashSet<string>(
                (roles ?? Array.Empty<string>()).Where(role => !string.IsNullOrWhiteSpace(role)).Select(role => role.Trim()),
                StringComparer.OrdinalIgnoreCase);

            return (mappings ?? Array.Empty<FolderRoleMap>())
                .Where(mapping => mapping != null && !string.IsNullOrWhiteSpace(mapping.Role) && roleSet.Contains(mapping.Role.Trim()))
                .SelectMany(mapping => mapping.Folders ?? Array.Empty<string>())
                .Where(folder => !string.IsNullOrWhiteSpace(folder))
                .Select(folder => folder.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }
    }
}
