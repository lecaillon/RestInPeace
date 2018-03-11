## RestInPeace
Testing REST services made easy. Inspired by REST Assured.

`RIP` is a single class for arranging and formatting code that unit test REST services. The idea is to make them very readable and consistent.

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
