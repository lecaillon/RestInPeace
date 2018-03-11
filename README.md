## RestInPeace
Testing REST services made easy. Inspired by REST Assured.

RIP is a single [class](https://github.com/lecaillon/RestInPeace/blob/master/src/RestInPeace/RIP.cs) for arranging and formatting code that unit test REST services. The idea is to make them very readable and consistent. RIP does not intend to replace your favorite assertion framework, in fact it's quite the opposite. The example below shows how  RIP uses [Fluent Assertions](http://fluentassertions.com/) to unit test a simple REST service.

### Example
```c#
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
```

### Structure
The design of RIP consists of 3 parts that mimics the Arrange Act Assert pattern.

#### Given

#### When

#### Then
