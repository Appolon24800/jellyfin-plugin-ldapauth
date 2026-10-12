<h1 align="center">Jellyfin LDAP-Auth Plugin</h1>
<h3 align="center">Part of the <a href="https://jellyfin.org">Jellyfin Project</a></h3>

<p align="center">
<img alt="Plugin Banner" src="https://raw.githubusercontent.com/jellyfin/jellyfin-ux/master/plugins/SVG/jellyfin-plugin-ldapauth.svg?sanitize=true"/>
<br/>
<br/>
<a href="https://github.com/jellyfin/jellyfin-plugin-ldapauth/actions/workflows/test.yaml">
<img alt="GitHub Workflow Status" src="https://img.shields.io/github/actions/workflow/status/jellyfin/jellyfin-plugin-ldapauth/.github/workflows/test.yaml?branch=master">
</a>
<a href="https://github.com/jellyfin/jellyfin-plugin-ldapauth">
<img alt="MIT License" src="https://img.shields.io/github/license/jellyfin/jellyfin-plugin-ldapauth.svg"/>
</a>
<a href="https://github.com/jellyfin/jellyfin-plugin-ldapauth/releases">
<img alt="Current Release" src="https://img.shields.io/github/release/jellyfin/jellyfin-plugin-ldapauth.svg"/>
</a>
</p>

## About

LDAP authentication for Jellyfin Media Server. JelLDAP, if you will.

Authenticate your Jellyfin users against an LDAP database, and optionally create users who do not yet exist automatically.

Allows the administrator to customize most aspects of the LDAP authentication process, including customizable search attributes, username attribute, and a search filter for administrative users (set on user creation). The user, via the "Manual Login" process, can enter any valid attribute value, which will be mapped back to the specified username attribute automatically as well.

## Installation

[See the official documentation for install instructions](https://jellyfin.org/docs/general/server/plugins/index.html#installing).

## Folder role mapping

In the plugin settings, open **Jellyfin User Settings** and enable **Folder role mapping** to manage library access from LDAP groups or roles.

1. Set the LDAP role attribute. The default is `memberOf`. The bind user must be able to read this attribute on user entries.
2. Add a role mapping and enter a complete attribute value, such as `CN=JellyfinMovies,OU=Groups,DC=example,DC=com`.
3. Select the libraries granted by that role, then save the settings and restart Jellyfin.

Users receive the union of the libraries for all matching roles. Matching ignores case and leading or trailing whitespace. It does not interpret wildcards, shorten group DNs, or resolve nested groups. Directories that store membership only on group entries, such as `memberUid`, need a user-side membership attribute to use this feature.

When enabled, mappings replace the default library settings and synchronize access on every successful LDAP login for new and existing users. A missing role attribute or no matching role grants no libraries. Removing a role revokes its library access on the next login. Existing sessions are not explicitly revoked. Jellyfin administrators retain Jellyfin's administrator access rules.

The feature is disabled by default. When disabled, the existing library settings apply only to newly created users, and the plugin leaves existing users' library access unchanged.

## Build

This build targets Jellyfin 12.1.0. The Jellyfin dependency is pinned so builds do not pick up incompatible preview versions.

1. To build this plugin you will need [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).

2. Build plugin with following command
  ```
   dotnet publish LDAP-Auth/LDAP-Auth.csproj --configuration Release --output bin
  ```

3. Place the dll-file in the `plugins/ldapauth` folder (you might need to create the folders) of your JF install

Run the mapping and configuration tests with `dotnet test LDAP-Auth.sln --configuration Release`.

## Releasing

To release the plugin we recommend [JPRM](https://github.com/oddstr13/jellyfin-plugin-repository-manager) that will build and package the plugin.
For additional context and for how to add the packaged plugin zip to a plugin manifest see the [JPRM documentation](https://github.com/oddstr13/jellyfin-plugin-repository-manager) for more info.

## Contributing

We welcome all contributions and pull requests! If you have a larger feature in mind please open an issue so we can discuss the implementation before you start.
In general refer to our [contributing guidelines](https://github.com/jellyfin/.github/blob/master/CONTRIBUTING.md) for further information.

## Licence

This plugins code and packages are distributed under the MIT License. See [LICENSE](./LICENSE) for more information.
