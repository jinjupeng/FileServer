using System;
using System.IO;
using JetBrains.Annotations;

namespace FileServer.Common.Helper
{
    /// <summary>
    /// A helper class for Directory operations.
    /// </summary>
    public static class DirectoryHelper
    {
        public static void CreateIfNotExists(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public static void DeleteIfExists(string directory)
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory);
            }
        }

        public static void DeleteIfExists(string directory, bool recursive)
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive);
            }
        }

        public static void CreateIfNotExists(DirectoryInfo directory)
        {
            if (!directory.Exists)
            {
                directory.Create();
            }
        }

        public static bool IsSubDirectoryOf([NotNull] string parentDirectoryPath, [NotNull] string childDirectoryPath)
        {
            ObjHelper.NotNull(parentDirectoryPath, nameof(parentDirectoryPath));
            ObjHelper.NotNull(childDirectoryPath, nameof(childDirectoryPath));

            return IsSubDirectoryOf(
                new DirectoryInfo(parentDirectoryPath),
                new DirectoryInfo(childDirectoryPath)
            );
        }

        public static bool IsSubDirectoryOf([NotNull] DirectoryInfo parentDirectory,
            [NotNull] DirectoryInfo childDirectory)
        {
            ObjHelper.NotNull(parentDirectory, nameof(parentDirectory));
            ObjHelper.NotNull(childDirectory, nameof(childDirectory));

            if (parentDirectory.FullName == childDirectory.FullName)
            {
                return true;
            }

            var parentOfChild = childDirectory.Parent;
            if (parentOfChild == null)
            {
                return false;
            }

            return IsSubDirectoryOf(parentDirectory, parentOfChild);
        }
    }
}
