namespace Services.Utils;

using System;
using System.Collections.Generic;

public static class RedisKeyGenerator
{
    public const string ProjectInvitation = "ProjectInvitation";
    public static string GenerateKey(params object[] args)
    {
        return $"{string.Join(":", args)}";
    }

    /// <summary>
    /// Generate key for project invitation
    /// Pattern: ProjectStudent:projectId:studentId:token
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="studentId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static string GenerateProjectInvitationKey(Guid projectId, Guid studentId, string token) =>
        new RedisPatternBuilder()
            .AddExact(ProjectInvitation)
            .AddExact(projectId.ToString())
            .AddExact(studentId.ToString())
            .AddExact(token)
            .Build();

    /// <summary>
    /// Generate pattern for deleting all project invitations for a student
    /// Pattern: ProjectStudent:*:studentId:*
    /// </summary>
    /// <param name="studentId"></param>
    /// <returns></returns>
    public static string GenerateProjectInvitationDeletePattern(Guid studentId) =>
        new RedisPatternBuilder()
            .AddExact(ProjectInvitation)
            .AddWildcard()
            .AddExact(studentId.ToString())
            .AddWildcard()
            .Build();
}

public class RedisPatternBuilder
{
    private readonly List<string> _parts = [];

    public RedisPatternBuilder AddExact(string value)
    {
        _parts.Add(value);
        return this;
    }

    public RedisPatternBuilder AddWildcard()
    {
        _parts.Add("*");
        return this;
    }

    public RedisPatternBuilder AddOptional(string? value)
    {
        _parts.Add(value ?? "*");
        return this;
    }

    public string Build() => string.Join(":", _parts);
}