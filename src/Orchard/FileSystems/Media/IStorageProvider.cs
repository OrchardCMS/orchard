﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Orchard.FileSystems.Media {
    public interface IStorageProvider : IDependency {
        /// <summary>
        /// Checks if the given file exists within the storage provider.
        /// </summary>
        /// <param name="path">The relative path within the storage provider.</param>
        /// <returns>True if the file exists; False otherwise.</returns>
        bool FileExists(string path);

        /// <summary>
        /// Retrieves the public URL for a given file within the storage provider.
        /// </summary>
        /// <param name="path">The relative path within the storage provider.</param>
        /// <returns>The public URL.</returns>
        string GetPublicUrl(string path);

        /// <summary>
        /// Retrieves the path within the storage provider for a given public url.
        /// </summary>
        /// <param name="url">The virtual or public url of a media.</param>
        /// <returns>The storage path or <value>null</value> if the media is not in a correct format.</returns>
        string GetStoragePath(string url);

        /// <summary>
        /// Retrieves a file within the storage provider.
        /// </summary>
        /// <param name="path">The relative path to the file within the storage provider.</param>
        /// <returns>The file.</returns>
        /// <exception cref="ArgumentException">If the file is not found.</exception>
        IStorageFile GetFile(string path);

        /// <summary>
        /// Lists the files within a storage provider's path.
        /// </summary>
        /// <param name="path">The relative path to the folder which files to list.</param>
        /// <returns>The list of files in the folder.</returns>
        IEnumerable<IStorageFile> ListFiles(string path);

        /// <summary>
        /// Checks if the given folder exists within the storage provider.
        /// </summary>
        /// <param name="path">The relative path within the storage provider.</param>
        /// <returns>True if the folder exists; False otherwise.</returns>
        bool FolderExists(string path);

        /// <summary>
        /// Lists the folders within a storage provider's path.
        /// </summary>
        /// <param name="path">The relative path to the folder which folders to list.</param>
        /// <returns>The list of folders in the folder.</returns>
        IEnumerable<IStorageFolder> ListFolders(string path);

        /// <summary>
        /// Tries to create a folder in the storage provider.
        /// </summary>
        /// <param name="path">The relative path to the folder to be created.</param>
        /// <returns>True if success; False otherwise.</returns>
        bool TryCreateFolder(string path);

        /// <summary>
        /// Creates a folder in the storage provider.
        /// </summary>
        /// <param name="path">The relative path to the folder to be created.</param>
        /// <exception cref="ArgumentException">If the folder already exists.</exception>
        void CreateFolder(string path);

        /// <summary>
        /// Deletes a folder in the storage provider.
        /// </summary>
        /// <param name="path">The relative path to the folder to be deleted.</param>
        /// <exception cref="ArgumentException">If the folder doesn't exist.</exception>
        void DeleteFolder(string path);

        /// <summary>
        /// Renames a folder in the storage provider.
        /// </summary>
        /// <param name="oldPath">The relative path to the folder to be renamed.</param>
        /// <param name="newPath">The relative path to the new folder.</param>
        void RenameFolder(string oldPath, string newPath);

        /// <summary>
        /// Deletes a file in the storage provider.
        /// </summary>
        /// <param name="path">The relative path to the file to be deleted.</param>
        /// <exception cref="ArgumentException">If the file doesn't exist.</exception>
        void DeleteFile(string path);

        /// <summary>
        /// Renames a file in the storage provider.
        /// </summary>
        /// <param name="oldPath">The relative path to the file to be renamed.</param>
        /// <param name="newPath">The relative path to the new file.</param>
        void RenameFile(string oldPath, string newPath);

        /// <summary>
        /// Copies a file in the storage provider.
        /// </summary>
        /// <param name="originalPath">The relative path to the file to be copied.</param>
        /// <param name="duplicatePath">The relative path to the new file.</param>
        void CopyFile(string originalPath, string duplicatePath);

        /// <summary>
        /// Creates a file in the storage provider.
        /// </summary>
        /// <param name="path">The relative path to the file to be created.</param>
        /// <exception cref="ArgumentException">If the file already exists.</exception>
        /// <returns>The created file.</returns>
        IStorageFile CreateFile(string path);

        /// <summary>
        /// Tries to save a stream in the storage provider.
        /// </summary>
        /// <param name="path">The relative path to the file to be created.</param>
        /// <param name="inputStream">The stream to be saved.</param>
        /// <returns>True if success; False otherwise.</returns>
        bool TrySaveStream(string path, Stream inputStream);

        /// <summary>
        /// Saves a stream in the storage provider.
        /// </summary>
        /// <param name="path">The relative path to the file to be created.</param>
        /// <param name="inputStream">The stream to be saved.</param>
        /// <exception cref="ArgumentException">If the stream can't be saved due to access permissions.</exception>
        void SaveStream(string path, Stream inputStream);

        /// <summary>
        /// Combines two paths.
        /// </summary>
        /// <param name="path1">The parent path.</param>
        /// <param name="path2">The child path.</param>
        /// <returns>The combined path.</returns>
        string Combine(string path1, string path2);
    }
}