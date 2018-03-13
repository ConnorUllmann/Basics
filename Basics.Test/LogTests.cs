using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Basics.Test
{
    public class LogTests
    {
        [Fact]
        public void PathParts()
        {
            var _filepath = "C:/path/to/directory/filename.extension";
            var parts = new System.IO.FileInfo(_filepath).PathParts();
            Assert.Equal("C:\\path\\to\\directory\\", parts.directory);
            Assert.Equal("filename", parts.filename);
            Assert.Equal(".extension", parts.extension);
        }
    }
}
