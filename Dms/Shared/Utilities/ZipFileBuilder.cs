// This file is part of re-vision (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace Remotion.Dms.Shared.Utilities
{
  /// <summary>
  /// Implements <see cref="IArchiveBuilder"/>. 
  /// At first all files of a directory are added to the generated zip file, then the directory itself is added.
  /// </summary>
  public class ZipFileBuilder : IArchiveBuilder
  {
    private readonly List<IFileSystemEntry> _files = new List<IFileSystemEntry>();

    public event EventHandler<StreamCopyProgressEventArgs> ArchiveProgress;

    public ZipFileBuilder ()
    {
    }

    public void AddDirectory (IDirectoryInfo directoryInfo)
    {
      ArgumentUtility.CheckNotNull ("directoryInfo", directoryInfo);
      _files.Add (directoryInfo);
    }

    public void AddFile (IFileInfo fileInfo)
    {
      ArgumentUtility.CheckNotNull ("fileInfo", fileInfo);
      _files.Add (fileInfo);
    }

    public Stream Build (string archiveFileName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("archiveFileName", archiveFileName);
      
      using (var zipOutputStream = new ZipOutputStream (File.Create (archiveFileName)))
      {
        StreamCopier streamCopier = new StreamCopier();
        foreach (var fileInfo in _files)
        {
          if (fileInfo.GetType() == typeof (InMemoryFileInfoWrapper))
            AddFilesToZipFile ((InMemoryFileInfoWrapper) fileInfo, ((InMemoryFileInfoWrapper) fileInfo).Name, zipOutputStream, streamCopier);
          else if (fileInfo.GetType() == typeof (FileInfoWrapper))
            AddFilesToZipFile ((FileInfoWrapper) fileInfo, ((FileInfoWrapper) fileInfo).Name, zipOutputStream, streamCopier);
          else if (fileInfo.GetType() == typeof (DirectoryInfoWrapper))
            AddDirectoryToZipFile ((DirectoryInfoWrapper) fileInfo, zipOutputStream, streamCopier, string.Empty);
        }
      }
      _files.Clear();
      return File.Open (archiveFileName, FileMode.Open, FileAccess.Read, FileShare.None);
    }

    private void AddFilesToZipFile (
        IFileInfo fileInfo,
        string path,
        ZipOutputStream zipOutputStream,
        StreamCopier streamCopier)
    {
      using (var fileStream = fileInfo.Open (FileMode.Open, FileAccess.Read, FileShare.Read))
      {
        ZipEntry zipEntry = new ZipEntry (path);
        zipOutputStream.PutNextEntry (zipEntry);
        streamCopier.TransferProgress += OnZippingProgress;
        streamCopier.CopyStream (fileStream, zipOutputStream, fileStream.Length);
      }
    }

    private void AddDirectoryToZipFile (
        DirectoryInfoWrapper directoryInfo,
        ZipOutputStream zipOutputStream,
        StreamCopier streamCopier,
        string parentDirectory)
    {
      parentDirectory = Path.Combine (parentDirectory, directoryInfo.Name);
      foreach (var file in directoryInfo.GetFiles())
        AddFilesToZipFile ((FileInfoWrapper) file, Path.Combine (parentDirectory, file.Name), zipOutputStream, streamCopier);
      foreach (var directory in directoryInfo.GetDirectories())
        AddDirectoryToZipFile ((DirectoryInfoWrapper) directory, zipOutputStream, streamCopier, parentDirectory);
    }

    private void OnZippingProgress (object sender, StreamCopyProgressEventArgs args)
    {
      if (ArchiveProgress != null)
        ArchiveProgress (this, args);
    }
  }
}