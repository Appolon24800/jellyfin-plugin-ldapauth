using System;

namespace Jellyfin.Plugin.LDAP_Auth.Config
{
    /// <summary>
    /// Maps an LDAP role attribute value to library folder IDs.
    /// </summary>
    public class FolderRoleMap
    {
        /// <summary>
        /// Gets or sets the complete LDAP role attribute value.
        /// </summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the library folder IDs granted by this role.
        /// </summary>
        public string[] Folders { get; set; } = Array.Empty<string>();
    }
}
