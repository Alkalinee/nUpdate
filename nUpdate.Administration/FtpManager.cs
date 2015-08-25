﻿// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Security;
using nUpdate.Administration.Ftp;
using nUpdate.Administration.Ftp.Service;
using nUpdate.Administration.TransferInterface;

namespace nUpdate.Administration
{
// ReSharper disable once InconsistentNaming
    public class FTPManager
    {
        /// <summary>
        ///     Gets or sets the FTP-items that were listed by the <see cref="ListDirectoriesAndFiles" />-method.
        /// </summary>
        public IEnumerable<FtpItem> ListedFtpItems;

        private bool _disposed;
        private ITransferProvider _transferProvider;

        public FTPManager()
        {
            GetTransferProvider();
        }

        public FTPManager(string host, int port, string directory, string username, SecureString password,
            WebProxy proxy, bool usePassiveMode, string transferAssemblyFilePath, int protcol)
        {
            TransferAssemblyPath = transferAssemblyFilePath;
            GetTransferProvider();

            _transferProvider.Host = host;
            _transferProvider.Port = port;
            _transferProvider.Directory = directory;
            _transferProvider.Username = username;
            _transferProvider.Password = password.Copy();
            _transferProvider.Proxy = proxy;
            _transferProvider.UsePassiveMode = usePassiveMode;
            if (String.IsNullOrWhiteSpace(transferAssemblyFilePath))
                Protocol = (FtpSecurityProtocol) protcol;
        }

        /// <summary>
        ///     Sets the protocol to use.
        /// </summary>
        public FtpSecurityProtocol Protocol
        {
            get { return ((TransferService) _transferProvider).Protocol; }
            set
            {
                if (_transferProvider.GetType() == typeof (TransferService))
                    ((TransferService) _transferProvider).Protocol = value;
            }
        }

        /// <summary>
        ///     Sets if passive mode should be used.
        /// </summary>
        public bool UsePassiveMode
        {
            get { return _transferProvider.UsePassiveMode; }
            set { _transferProvider.UsePassiveMode = value; }
        }

        /// <summary>
        ///     The FTP-server.
        /// </summary>
        public string Host
        {
            get { return _transferProvider.Host; }
            set { _transferProvider.Host = value; }
        }

        /// <summary>
        ///     The port.
        /// </summary>
        public int Port
        {
            get { return _transferProvider.Port; }
            set { _transferProvider.Port = value; }
        }

        /// <summary>
        ///     The directory.
        /// </summary>
        public string Directory
        {
            get { return _transferProvider.Directory; }
            set { _transferProvider.Directory = value; }
        }

        /// <summary>
        ///     The username.
        /// </summary>
        public string Username
        {
            get { return _transferProvider.Username; }
            set { _transferProvider.Username = value; }
        }

        /// <summary>
        ///     The password.
        /// </summary>
        public SecureString Password
        {
            get { return _transferProvider.Password.Copy(); }
            set { _transferProvider.Password = value; }
        }

        /// <summary>
        ///     The proxy to use, if wished.
        /// </summary>
        public WebProxy Proxy
        {
            get { return _transferProvider.Proxy; }
            set { _transferProvider.Proxy = value; }
        }

        /// <summary>
        ///     Gets or sets the exception appearing during the package upload.
        /// </summary>
        public Exception PackageUploadException
        {
            get { return _transferProvider.PackageUploadException; }
            set { _transferProvider.PackageUploadException = value; }
        }

        /// <summary>
        ///     Gets or sets the path of the assembly containing the custom transfer handlers.
        /// </summary>
        public string TransferAssemblyPath { get; set; }

        private void GetTransferProvider()
        {
            if (String.IsNullOrWhiteSpace(TransferAssemblyPath))
            {
                var provider = GetDefaultServiceProvider();
                _transferProvider = (ITransferProvider) provider.GetService(typeof (ITransferProvider));
                ((TransferService) _transferProvider).Protocol = Protocol;
                    // Default integrated services define a protocol as there are multiple ones available
            }
            else
            {
                var assembly = Assembly.LoadFrom(TransferAssemblyPath);
                var provider = ServiceProviderHelper.CreateServiceProvider(assembly) ?? GetDefaultServiceProvider();
                _transferProvider = (ITransferProvider) provider.GetService(typeof (ITransferProvider));
            }
        }

        private IServiceProvider GetDefaultServiceProvider()
        {
            var assembly = Assembly.GetExecutingAssembly();
            return ServiceProviderHelper.CreateServiceProvider(assembly);
        }

        public void TestConnection()
        {
            _transferProvider.TestConnection();
        }

        /// <summary>
        ///     Deletes a file on the server.
        /// </summary>
        /// <param name="fileName">The name of the file to delete.</param>
        public void DeleteFile(string fileName)
        {
            _transferProvider.DeleteFile(fileName);
        }

        /// <summary>
        ///     Deletes a file on the server which is located at the specified path.
        /// </summary>
        /// <param name="directoryPath">The path of the directory where the file is located.</param>
        /// <param name="fileName">The name of the file to delete.</param>
        public void DeleteFile(string directoryPath, string fileName)
        {
            _transferProvider.DeleteFile(directoryPath, fileName);
        }

        /// <summary>
        ///     Deletes a directory on the server.
        /// </summary>
        /// <param name="directoryPath">The name of the directory to delete.</param>
        public void DeleteDirectory(string directoryPath)
        {
            _transferProvider.DeleteDirectory(directoryPath);
        }

        /// <summary>
        ///     Lists the directories and files of the current FTP-directory.
        /// </summary>
        public IEnumerable<FtpItem> ListDirectoriesAndFiles(string path, bool recursive)
        {
            return _transferProvider.ListDirectoriesAndFiles(path, recursive);
        }

        public void RenameDirectory(string oldName, string newName)
        {
            _transferProvider.RenameDirectory(oldName, newName);
        }

        public void MakeDirectory(string name)
        {
            _transferProvider.MakeDirectory(name);
        }

        /// <summary>
        ///     Moves all files and subdirectories from the current FTP-directory to the given aim directory.
        /// </summary>
        /// <param name="aimPath">The aim directory to move the files and subdirectories to.</param>
        public void MoveContent(string aimPath)
        {
            _transferProvider.MoveContent(aimPath);
        }

        /// <summary>
        ///     Returns if a file or directory is existing on the server.
        /// </summary>
        /// <param name="destinationName">The name of the file or folder to check.</param>
        /// <returns>Returns "true" if the file or folder exists, otherwise "false".</returns>
        public bool IsExisting(string destinationName)
        {
            return _transferProvider.IsExisting(destinationName);
        }

        /// <summary>
        ///     Returns if a file or directory is existing on the server.
        /// </summary>
        /// <param name="directoryPath">The directory in which the file should be existing.</param>
        /// <param name="destinationName">The name of the file or folder to check.</param>
        /// <returns>Returns "true" if the file or folder exists, otherwise "false".</returns>
        public bool IsExisting(string directoryPath, string destinationName)
        {
            return _transferProvider.IsExisting(directoryPath, destinationName);
        }

        /// <summary>
        ///     Uploads a file to the server.
        /// </summary>
        /// <param name="filePath">The local path of the file to upload.</param>
        public void UploadFile(string filePath)
        {
            _transferProvider.UploadFile(filePath);
        }

        /// <summary>
        ///     Uploads an update package to the server.
        /// </summary>
        /// <param name="packagePath">The local path of the package..</param>
        /// <param name="packageVersion">The package version for the directory name.</param>
        public void UploadPackage(string packagePath, string packageVersion)
        {
            _transferProvider.UploadPackage(packagePath, packageVersion);
        }

        /// <summary>
        ///     Terminates the package upload process.
        /// </summary>
        public void CancelPackageUpload()
        {
            _transferProvider.CancelPackageUpload();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!disposing || _disposed)
                return;

            Password.Dispose();
            _disposed = true;
        }

        public event EventHandler<TransferProgressEventArgs> ProgressChanged
        {
            add { _transferProvider.ProgressChanged += value; }
            remove { _transferProvider.ProgressChanged -= value; }
        }

        public event EventHandler<EventArgs> CancellationFinished
        {
            add { _transferProvider.CancellationFinished += value; }
            remove { _transferProvider.CancellationFinished -= value; }
        }
    }
}