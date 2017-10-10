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
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.IO
{
  /// <summary>
  /// In-memory implementation of the <see cref="IDirectoryInfo"/> interface.
  /// </summary>
  public class InMemoryDirectoryInfo : IDirectoryInfo
  {
    [NotNull]
    private readonly string _fullName;

    private readonly List<IDirectoryInfo> _directories = new List<IDirectoryInfo>();
    private readonly List<IFileInfo> _files = new List<IFileInfo>();

    [CanBeNull]
    private readonly IDirectoryInfo _parent;

    private readonly DateTime _creationTimeUtc;
    private readonly DateTime _lastAccessTimeUtc;
    private readonly DateTime _lastWriteTimeUtc;

    public InMemoryDirectoryInfo (
        [NotNull] string fullName,
        [CanBeNull] IDirectoryInfo parent,
        DateTime creationTimeUtc,
        DateTime lastAccessTimeUtc,
        DateTime lastWriteTimeUtc)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("fullName", fullName);

      _fullName = fullName;
      _parent = parent;
      _creationTimeUtc = creationTimeUtc;
      _lastAccessTimeUtc = lastAccessTimeUtc;
      _lastWriteTimeUtc = lastWriteTimeUtc;
    }

    public string PhysicalPath
    {
      get { return null; }
    }

    public string FullName
    {
      get { return _fullName; }
    }

    public string Name
    {
      get { return Path.GetFileName (_fullName); }
    }

    public string Extension
    {
      get { return string.Empty; }
    }

    public bool Exists
    {
      get { return true; }
    }

    public bool IsReadOnly
    {
      get { return true; }
    }

    public DateTime CreationTimeUtc
    {
      get { return _creationTimeUtc; }
    }

    public DateTime LastAccessTimeUtc
    {
      get { return _lastAccessTimeUtc; }
    }

    public DateTime LastWriteTimeUtc
    {
      get { return _lastWriteTimeUtc; }
    }

    public IDirectoryInfo Parent
    {
      get { return _parent; }
    }

    public IFileInfo[] GetFiles ()
    {
      return _files.ToArray();
    }

    public IDirectoryInfo[] GetDirectories ()
    {
      return _directories.ToArray();
    }

    public List<IFileInfo> Files
    {
      get { return _files; }
    }

    public List<IDirectoryInfo> Directories
    {
      get { return _directories; }
    }

    public void Refresh ()
    {
      // NOP
    }

    public override string ToString ()
    {
      return _fullName;
    }
  }
}