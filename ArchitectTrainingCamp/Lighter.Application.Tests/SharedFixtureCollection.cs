using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Lighter.Application.Tests
{
    [CollectionDefinition(nameof(SharedFixture))]
    public class SharedFixtureCollection: ICollectionFixture<SharedFixture>
    {

    }
}
