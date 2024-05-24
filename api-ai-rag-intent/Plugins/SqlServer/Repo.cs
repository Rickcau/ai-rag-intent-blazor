// Copyright (c) Microsoft. All rights reserved.

using System;
using System.IO;

namespace api_ai_rag_intent.Plugins.SqlServer;

/// <summary>
/// Utility class to assist in resolving file-system paths.
/// </summary>
internal static class Repo
{
    private static string RootFolder { get; } = GetRoot();

    public static string RootConfigFolder { get; } = $@"{RootFolder}\api-ai-rag-intent";

    private static string GetRoot()
    {
        var current = Environment.CurrentDirectory;

        var folder = new DirectoryInfo(current);

        while (!Directory.Exists(Path.Combine(folder.FullName, ".git")))
        {
            folder =
                folder.Parent ??
                throw new DirectoryNotFoundException($"Unable to locate repo root folder: {current}");
        }

        return folder.FullName;
    }
}
