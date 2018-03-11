namespace RestInPeace.Tests
{
    using System.Collections.Generic;
    using System.Net;
    using FluentAssertions;
    using Xunit;
    using static RestInPeace.RIP;

    public class RIPTest
    {
        [Fact]
        public void Test1()
        {
            Given()
                .BaseAddress("https://geo.api.gouv.fr")
                .Header("user-agent", "myagent")
                .Query("nom", "Guadeloupe")
           .When()
                .Get("regions")
           .Then()
                .AssertThat(response =>
                {
                    response.HttpStatusCode.Should().Be(HttpStatusCode.OK);
                    response.GetContent<List<dynamic>>().Should().HaveCount(1);
                });
        }
    }
}
