using System;
using System.IO;
using System.Xml.Serialization;
using Jellyfin.Plugin.LDAP_Auth.Config;
using Jellyfin.Plugin.LDAP_Auth.Helpers;
using Xunit;

namespace Jellyfin.Plugin.LDAP_Auth.Tests;

public class FolderRoleMapperTests
{
    [Fact]
    public void MatchingRolesGrantUnionOfLibrariesWithoutDuplicates()
    {
        var mappings = new[]
        {
            new FolderRoleMap { Role = "movies", Folders = new[] { "movies-id", "shared-id" } },
            new FolderRoleMap { Role = "shows", Folders = new[] { "shows-id", "shared-id" } },
            new FolderRoleMap { Role = "other", Folders = new[] { "other-id" } },
        };

        Assert.Equal(new[] { "movies-id", "shared-id", "shows-id" }, FolderRoleMapper.GetEnabledFolders(new[] { "movies", "shows", "movies" }, mappings));
    }

    [Fact]
    public void FullGroupDnsMatchIgnoringCaseAndOuterWhitespace()
    {
        var mapping = new FolderRoleMap { Role = " CN=Movies,OU=Groups,DC=example,DC=com ", Folders = new[] { "movies-id" } };

        Assert.Equal(new[] { "movies-id" }, FolderRoleMapper.GetEnabledFolders(new[] { "cn=movies,ou=groups,dc=EXAMPLE,dc=COM" }, new[] { mapping }));
    }

    [Theory]
    [InlineData("Movies")]
    [InlineData("CN=Movies,OU=Other,DC=example,DC=com")]
    [InlineData("*")]
    public void PartialOrWildcardRolesDoNotMatch(string role)
    {
        var mapping = new FolderRoleMap { Role = "CN=Movies,OU=Groups,DC=example,DC=com", Folders = new[] { "movies-id" } };

        Assert.Empty(FolderRoleMapper.GetEnabledFolders(new[] { role }, new[] { mapping }));
    }

    [Fact]
    public void MissingRolesOrMappingsGrantNoLibraries()
    {
        var mapping = new FolderRoleMap { Role = "movies", Folders = new[] { "movies-id" } };

        Assert.Empty(FolderRoleMapper.GetEnabledFolders(null, new[] { mapping }));
        Assert.Empty(FolderRoleMapper.GetEnabledFolders(Array.Empty<string>(), new[] { mapping }));
        Assert.Empty(FolderRoleMapper.GetEnabledFolders(new[] { "movies" }, null));
    }

    [Fact]
    public void RemovedRoleNoLongerGrantsItsLibraries()
    {
        var mappings = new[]
        {
            new FolderRoleMap { Role = "movies", Folders = new[] { "movies-id" } },
            new FolderRoleMap { Role = "shows", Folders = new[] { "shows-id" } },
        };

        Assert.Equal(new[] { "movies-id", "shows-id" }, FolderRoleMapper.GetEnabledFolders(new[] { "movies", "shows" }, mappings));
        Assert.Equal(new[] { "shows-id" }, FolderRoleMapper.GetEnabledFolders(new[] { "shows" }, mappings));
    }

    [Fact]
    public void EmptyAndNullEntriesDoNotGrantLibraries()
    {
        var mappings = new[]
        {
            null,
            new FolderRoleMap { Role = "", Folders = new[] { "unexpected-id" } },
            new FolderRoleMap { Role = "movies", Folders = null },
            new FolderRoleMap { Role = "movies", Folders = new[] { null, "", " ", " movies-id " } },
        };

        Assert.Equal(new[] { "movies-id" }, FolderRoleMapper.GetEnabledFolders(new[] { null, " ", "movies" }, mappings));
    }

    [Fact]
    public void LegacyConfigurationKeepsFolderRolesDisabled()
    {
        var serializer = new XmlSerializer(typeof(PluginConfiguration));
        using var reader = new StringReader("<PluginConfiguration><EnableAllFolders>true</EnableAllFolders></PluginConfiguration>");
        var config = (PluginConfiguration)serializer.Deserialize(reader);

        Assert.False(config.EnableFolderRoles);
        Assert.True(config.EnableAllFolders);
        Assert.Equal("memberOf", config.LdapRoleAttribute);
        Assert.Empty(config.FolderRoleMapping);
    }

    [Fact]
    public void FolderMappingsRoundTripThroughConfigurationXml()
    {
        var config = new PluginConfiguration
        {
            EnableFolderRoles = true,
            LdapRoleAttribute = "memberOf",
            FolderRoleMapping = new[] { new FolderRoleMap { Role = "CN=Movies,DC=example,DC=com", Folders = new[] { "movies-id" } } },
        };
        var serializer = new XmlSerializer(typeof(PluginConfiguration));
        using var writer = new StringWriter();
        serializer.Serialize(writer, config);
        using var reader = new StringReader(writer.ToString());
        var restored = (PluginConfiguration)serializer.Deserialize(reader);

        Assert.True(restored.EnableFolderRoles);
        Assert.Equal(config.LdapRoleAttribute, restored.LdapRoleAttribute);
        Assert.Equal(config.FolderRoleMapping[0].Role, restored.FolderRoleMapping[0].Role);
        Assert.Equal(config.FolderRoleMapping[0].Folders, restored.FolderRoleMapping[0].Folders);
    }
}
