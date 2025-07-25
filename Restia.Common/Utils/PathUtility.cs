﻿using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Text;

namespace Restia.Common.Utils
{
    public static class PathUtility
    {
        /// <summary>Path separators</summary>
        /// <remarks>Linux/Mac uses /, Windows uses \\</remarks>
        public static readonly char[] PathSeparators = new[] { '/', '\\' };
        /// <summary>Current directory token</summary>
        private const string CurrentDirectoryToken = ".";
        /// <summary>Parent directory token</summary>
        private const string ParentDirectoryToken = "..";

        /// <summary>
        /// Combines two path parts.
        /// </summary>
        /// <param name="path">The path</param>
        /// <param name="other">The other path</param>
        public static string Combine(string path, string other = null)
        {
            if (string.IsNullOrWhiteSpace(other))
            {
                return path;
            }

            if (other.Length > 0 && (other[0] == '/' || other[0] == '\\'))
            {
                // "other" is already an app-rooted path. Return it as-is.
                return other;
            }

            var index = path.LastIndexOfAny(PathSeparators);

            if (index != (path.Length - 1))
            {
                // If the first ends in a trailing slash e.g. "/Home/", assume it's a directory.
                return $"{path}/{other}";
            }

            return string.Concat(path.Substring(0, index + 1), other);
        }

        /// <summary>
        /// Combines multiple path parts.
        /// </summary>
        /// <param name="path">The path</param>
        /// <param name="others">The others path</param>
        public static string Combine(string path, params string[] others)
        {
            if (others == null || others.Length == 0)
            {
                return path;
            }

            var result = path;

            for (var i = 0; i < others.Length; i++)
            {
                result = Combine(result, others[i]);
            }

            return result;
        }

        /// <summary>
        /// Resolves relative segments in a path.
        /// </summary>
        /// <param name="path">The path</param>
        public static string ResolvePath(string path)
        {
            var pathSegment = new StringSegment(path);

            // Check if the path contains "." or ".." or empty segments (e.g. multiple slashes)
            if (!string.IsNullOrEmpty(path) &&
                (path[0] == PathSeparators[0] || path[0] == PathSeparators[1]))
            {
                // Leading slashes (e.g. "/Views/Index.cshtml") always generate an empty first token. Ignore these
                // for purposes of resolution.
                pathSegment = pathSegment.Subsegment(1);
            }

            var tokenizer = new StringTokenizer(pathSegment, PathSeparators);
            var requiresResolution = false;
            foreach (var segment in tokenizer)
            {
                // Determine if we need to do any path resolution.
                // We need to resolve paths with multiple path separators (e.g "//" or "\\") or, directory traversals e.g. ("../" or "./").
                if ((segment.Length == 0)
                    || segment.Equals(ParentDirectoryToken)
                    || segment.Equals(CurrentDirectoryToken))
                {
                    requiresResolution = true;
                    break;
                }
            }

            if (!requiresResolution)
            {
                return path;
            }

            var pathSegments = new List<StringSegment>();
            foreach (var segment in tokenizer)
            {
                if (segment.Length == 0)
                {
                    // Ignore multiple directory separators
                    continue;
                }

                if (segment.Equals(ParentDirectoryToken))
                {
                    if (pathSegments.Count == 0)
                    {
                        // Don't resolve the path if we ever escape the file system root. We can't reason about it in a
                        // consistent way.
                        return path;
                    }

                    pathSegments.RemoveAt(pathSegments.Count - 1);
                }
                else if (!segment.Equals(CurrentDirectoryToken))
                {
                    pathSegments.Add(segment);
                }
            }

            var builder = new StringBuilder();
            for (var i = 0; i < pathSegments.Count; i++)
            {
                var segment = pathSegments[i];
                builder.Append('/');
                builder.Append(segment.Buffer, segment.Offset, segment.Length);
            }

            return builder.ToString();
        }
    }
}
