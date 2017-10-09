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
using System.Linq;
using NUnit.Framework;

namespace Remotion.IO.UnitTests
{
  [TestFixture]
  public class DirectoryTreeResolverTest
  {
    private DirectoryTreeResolver _resolver;

    [SetUp]
    public void SetUp ()
    {
      _resolver = new DirectoryTreeResolver (new DateTime (2000, 1, 1));
    }

    [Test]
    public void GetLeafDirectoryInfo_WithOneLevelPath ()
    {
      var directory = _resolver.GetLeafDirectoryInfo ("Directory");

      Assert.That (directory, Is.Not.Null);
      Assert.That (directory.Name, Is.EqualTo ("Directory"));
      Assert.That (directory.FullName, Is.EqualTo ("Directory"));
      Assert.That (directory.Parent, Is.Null);
      Assert.That (directory.CreationTimeUtc, Is.EqualTo (new DateTime (2000, 1, 1)));
      Assert.That (directory.LastAccessTimeUtc, Is.EqualTo (new DateTime (2000, 1, 1)));
      Assert.That (directory.LastWriteTimeUtc, Is.EqualTo (new DateTime (2000, 1, 1)));
    }

    [Test]
    public void GetLeafDirectoryInfo_WithSameDirectoryTwice_ReturnsSameDirectory ()
    {
      var directory1 = _resolver.GetLeafDirectoryInfo ("Directory");
      var directory2 = _resolver.GetLeafDirectoryInfo ("Directory");

      Assert.That (directory1, Is.SameAs (directory2));
    }

    [Test]
    public void GetLeafDirectoryInfo_WithTwoLevelLevelPath ()
    {
      var directory = _resolver.GetLeafDirectoryInfo ("Parent\\Directory");

      Assert.That (directory, Is.Not.Null);
      Assert.That (directory.Name, Is.EqualTo ("Directory"));
      Assert.That (directory.FullName, Is.EqualTo ("Parent\\Directory"));
      Assert.That (directory.Parent, Is.Not.Null);
      Assert.That (directory.Parent.Name, Is.EqualTo ("Parent"));
    }

    [Test]
    public void GetRootDirectories ()
    {
      _resolver.GetLeafDirectoryInfo ("Root1\\Directory1");
      _resolver.GetLeafDirectoryInfo ("Root2\\Directory2");
      var roots = _resolver.GetRootDirectories();
      Assert.That (roots.Select (r => r.FullName).ToArray (), Is.EquivalentTo (new []{"Root1", "Root2"}));
    }

    [Test]
    public void GetLeafDirectoryInfo_WithMultiLevelLevelPath ()
    {
      var directory = _resolver.GetLeafDirectoryInfo ("GreatGrandParent\\GrandParent\\Parent\\Directory");

      Assert.That (directory, Is.Not.Null);
      Assert.That (directory.Name, Is.EqualTo ("Directory"));
      Assert.That (directory.FullName, Is.EqualTo ("GreatGrandParent\\GrandParent\\Parent\\Directory"));

      var parent = directory.Parent;
      Assert.That (parent, Is.Not.Null);
      Assert.That (parent.Name, Is.EqualTo ("Parent"));
      Assert.That (parent.FullName, Is.EqualTo ("GreatGrandParent\\GrandParent\\Parent"));
      Assert.That (parent.GetDirectories(), Is.EquivalentTo (new[] { directory }));

      var grandParent = parent.Parent;
      Assert.That (grandParent, Is.Not.Null);
      Assert.That (grandParent.Name, Is.EqualTo ("GrandParent"));
      Assert.That (grandParent.FullName, Is.EqualTo ("GreatGrandParent\\GrandParent"));
      Assert.That (grandParent.GetDirectories(), Is.EquivalentTo (new[] { parent }));

      var greatGrandParent = grandParent.Parent;
      Assert.That (greatGrandParent, Is.Not.Null);
      Assert.That (greatGrandParent.Name, Is.EqualTo ("GreatGrandParent"));
      Assert.That (greatGrandParent.FullName, Is.EqualTo ("GreatGrandParent"));
      Assert.That (greatGrandParent.GetDirectories(), Is.EquivalentTo (new[] { grandParent }));

      Assert.That (greatGrandParent.Parent, Is.Null);
    }

    [Test]
    public void GetLeafDirectoryInfo_IntergrationTest ()
    {
      var greatGrandParent1_GrandParent1_Parent1_Directory1 = _resolver.GetLeafDirectoryInfo ("GreatGrandParent1\\GrandParent1\\Parent1\\Directory1");
      var greatGrandParent1_GrandParent1_Parent1_Directory2 = _resolver.GetLeafDirectoryInfo ("GreatGrandParent1\\GrandParent1\\Parent1\\Directory2");
      _resolver.GetLeafDirectoryInfo ("GreatGrandParent1\\GrandParent1\\Parent2\\Directory1");
      _resolver.GetLeafDirectoryInfo ("GreatGrandParent1\\GrandParent1\\Parent2\\Directory2");
      _resolver.GetLeafDirectoryInfo ("GreatGrandParent1\\GrandParent1\\Parent1\\Directory3");
      var greatGrandParent1_GrandParent2_Parent1_Directory1 = _resolver.GetLeafDirectoryInfo ("GreatGrandParent1\\GrandParent2\\Parent1\\Directory1");
      _resolver.GetLeafDirectoryInfo ("GreatGrandParent1\\GrandParent2\\Parent1\\Directory2");
      var greatGrandParent1_GrandParent1 = _resolver.GetLeafDirectoryInfo ("GreatGrandParent1\\GrandParent1");
      var greatGrandParent2_GrandParent1_Parent1_Directory1 = _resolver.GetLeafDirectoryInfo ("GreatGrandParent2\\GrandParent1\\Paren1t\\Directory1");
      var greatGrandParent2 = _resolver.GetLeafDirectoryInfo ("GreatGrandParent2");

      var roots = _resolver.GetRootDirectories ();
      Assert.That (roots.Select (r => r.FullName).ToArray (), Is.EquivalentTo (new[] { "GreatGrandParent1", "GreatGrandParent2" }));

      Assert.That (greatGrandParent1_GrandParent1, Is.SameAs (greatGrandParent1_GrandParent1_Parent1_Directory1.Parent.Parent));
      Assert.That (greatGrandParent1_GrandParent1_Parent1_Directory1.Parent, Is.SameAs (greatGrandParent1_GrandParent1_Parent1_Directory2.Parent));
      Assert.That (greatGrandParent1_GrandParent1_Parent1_Directory1.Parent, Is.Not.SameAs (greatGrandParent1_GrandParent2_Parent1_Directory1));
      Assert.That (greatGrandParent2.Parent, Is.Not.SameAs (greatGrandParent1_GrandParent1.Parent));
      Assert.That (greatGrandParent2, Is.SameAs (greatGrandParent2_GrandParent1_Parent1_Directory1.Parent.Parent.Parent));
    }

    [Test]
    public void GetFileInfoWithPath_WithParentDirectory ()
    {
      var fileInfo = _resolver.GetFileInfoWithPath (
          "Parent\\Directory\\File",
          (fileName, directory) => new InMemoryFileInfo (fileName, new MemoryStream(), directory, DateTime.Now, DateTime.Now, DateTime.Now));

      Assert.That (fileInfo, Is.Not.Null);
      Assert.That (fileInfo.FullName, Is.EqualTo ("File"));
      Assert.That (fileInfo.Parent.FullName, Is.EqualTo ("Parent\\Directory"));
      Assert.That (fileInfo.Parent.GetFiles(), Is.EquivalentTo (new[] { fileInfo }));
    }

    [Test]
    public void GetFileInfoWithPath_WithoutParentDirectory ()
    {
      var fileInfo = _resolver.GetFileInfoWithPath (
          "File",
          (fileName, directory) => new InMemoryFileInfo (fileName, new MemoryStream(), directory, DateTime.Now, DateTime.Now, DateTime.Now));

      Assert.That (fileInfo, Is.Not.Null);
      Assert.That (fileInfo.FullName, Is.EqualTo ("File"));
      Assert.That (fileInfo.Parent, Is.Null);
    }
  }
}