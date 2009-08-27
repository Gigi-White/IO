// This file is part of re-vision (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh. www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful. 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not. see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Globalization;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Dms.Shared.Utilities;
using Remotion.Utilities;

namespace Remotion.Dms.UnitTests.Shared.Utilities
{
  [TestFixture]
  public class FileSizeTest
  {
    [SetUp]
    public void SetUp ()
    {
      new CultureScope(CultureInfo.InvariantCulture, CultureInfo.InvariantCulture);
    }

    [Test]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void ValueSmallerZero ()
    {
      new FileSize (-1);
    }

    [Test]
    public void Output ()
    {
      Assert.That (new FileSize (0).ToString (), Is.EqualTo ("0 B"));
      
      Assert.That (new FileSize (10).ToString (), Is.EqualTo ("10 B"));
      Assert.That (new FileSize (100).ToString (), Is.EqualTo ("100 B"));
      Assert.That (new FileSize (1000).ToString (), Is.EqualTo ("1000 B"));

      Assert.That (new FileSize (10000).ToString (), Is.EqualTo ("9.77 kB"));
      Assert.That (new FileSize (100000).ToString (), Is.EqualTo ("97.66 kB")); //should be: 97.7
      Assert.That (new FileSize (1000000).ToString (), Is.EqualTo ("976.56 kB")); //should be: 976.6

      Assert.That (new FileSize (10000000).ToString (), Is.EqualTo ("9.54 MB"));
      Assert.That (new FileSize (100000000).ToString (), Is.EqualTo ("95.37 MB")); //should be: 95.4
      Assert.That (new FileSize (1000000000).ToString (), Is.EqualTo ("953.67 MB")); //sould be: 953.7

      Assert.That (new FileSize (10000000000).ToString (), Is.EqualTo ("9.31 GB"));
      Assert.That (new FileSize (100000000000).ToString (), Is.EqualTo ("93.13 GB")); //should be: 93.1
      Assert.That (new FileSize (1000000000000).ToString (), Is.EqualTo ("931.32 GB"));   //should be: 931.3

      Assert.That (new FileSize (10000000000000).ToString (), Is.EqualTo ("9.09 TB"));
    }

    [Test]
    public void OutputWithKiloByte ()
    {
      var fileSize = new FileSize (128000);
      Assert.That (fileSize.ToString (), Is.EqualTo ("125.00 kB"));
    }

    [Test]
    public void OutputWithMB1 ()
    {
      var fileSize = new FileSize (1312000);
      Assert.That (fileSize.ToString (), Is.EqualTo ("1.25 MB"));
    }

    [Test]
    public void OutputWithMB2 ()
    {
      var fileSize = new FileSize (13107200);
      Assert.That (fileSize.ToString(), Is.EqualTo ("12.50 MB"));
    }

    [Test]
    public void OutputWithGB ()
    {
      var fileSize = new FileSize (13421772800);
      Assert.That (fileSize.ToString (), Is.EqualTo ("12.50 GB"));
    }

    [Test]
    public void OutputWithTB ()
    {
      var fileSize = new FileSize (13743895347200);
      Assert.That (fileSize.ToString (), Is.EqualTo ("12.50 TB"));
    }
    
  }
}