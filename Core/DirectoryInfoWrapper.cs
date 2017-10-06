// This file is part of the re-motion Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// re-motion is free software; you can redistribute it and/or modify it under 
// the terms of the GNU Lesser General Public License as published by the 
// Free Software Foundation; either version 2.1 of the License, 
// or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 

using System;
using System.IO;
using Remotion.Utilities;

namespace Remotion.IO
{
  /// <summary>
  /// The <see cref="DirectoryInfoWrapper"/> implements <see cref="IDirectoryInfo"/> and exposes methods for creating, moving, and enumerating 
  /// through directories via <see cref="DirectoryInfo"/>.
  /// </summary>
  public class DirectoryInfoWrapper : IDirectoryInfo
  {
    private readonly DirectoryInfo _wrappedInstance;

    public DirectoryInfoWrapper (DirectoryInfo directoryInfo)
    {
      ArgumentUtility.CheckNotNull ("directoryInfo", directoryInfo);
      _wrappedInstance = directoryInfo;
    }

    public string PhysicalPath
    {
      get { return _wrappedInstance.FullName; }
    }

    public string FullName
    {
      get { return _wrappedInstance.FullName; }
    }

    public string Extension
    {
      get { return _wrappedInstance.Extension; }
    }

    public string Name
    {
      get
      {
        var folders = _wrappedInstance.FullName.Split (Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        return folders.GetValue (folders.Length - 1).ToString();
      }
    }

    public bool Exists
    {
      get { return _wrappedInstance.Exists; }
    }

    public bool IsReadOnly
    {
      get { return _wrappedInstance.Attributes.Equals (FileAttributes.ReadOnly); }
    }

    public DateTime CreationTimeUtc
    {
      get { return _wrappedInstance.CreationTimeUtc; }
      set { _wrappedInstance.CreationTimeUtc = value; }
    }

    public DateTime LastAccessTimeUtc
    {
      get { return _wrappedInstance.LastAccessTimeUtc; }
      set { _wrappedInstance.LastAccessTimeUtc = value; }
    }

    public DateTime LastWriteTimeUtc
    {
      get { return _wrappedInstance.LastWriteTimeUtc; }
      set { _wrappedInstance.LastWriteTimeUtc = value; }
    }

    public IDirectoryInfo Parent
    {
      get { return this; }
    }

    public IFileInfo[] GetFiles ()
    {
      FileInfoWrapper[] fileInfo = new FileInfoWrapper[_wrappedInstance.GetFiles().Length];
      for (int i = 0; i < _wrappedInstance.GetFiles().Length; i++)
      {
        fileInfo[i] = new FileInfoWrapper(_wrappedInstance.GetFiles()[i]);
      }
      return fileInfo;
    }

    public IDirectoryInfo[] GetDirectories ()
    {
      DirectoryInfoWrapper[] directoryInfo = new DirectoryInfoWrapper[_wrappedInstance.GetDirectories().Length];

      for (int i = 0; i < _wrappedInstance.GetDirectories ().Length; i++)
      {
        directoryInfo[i] = new DirectoryInfoWrapper (_wrappedInstance.GetDirectories ()[i]);
      }
      return directoryInfo;
    }

    public void Refresh ()
    {
      _wrappedInstance.Refresh();
    }
  }
}