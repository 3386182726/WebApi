using Microsoft.EntityFrameworkCore;
using NoteService.Modules.Notes.Model;
using NoteService.Test.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NoteService.Test
{
    public class NoteServiceTests(SqlServerFixture fixture) : IClassFixture<SqlServerFixture>
    {
    }
}
