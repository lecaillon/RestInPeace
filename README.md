## RestInPeace
Testing REST services made easy. Inspired by [REST Assured](https://github.com/rest-assured/rest-assured).

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

### Design
The design of RIP is a call chain that consists of 3 parts that mimics the Arrange Act Assert pattern.

#### Given
Setup everything needed for running the tested REST service.

```c#
Given()
    // Add a JSON serialized object to the POST, PUT or DELETE query
    .Body(object)

    // Add a HTTP header
    .Header("string", "string")

     // Add query string parameters for any HTTP verb
    .Query("string", "string")

     // Provide a fully customized http client 
    .HttpClient(HttpClient)

    // Base address used when sending requests
    .BaseAddress("string")

    // Base address of URI used when sending requests
    .BaseAddress(Uri)
```

#### When
Invoke the REST service under test.
```c#
 .When()
    // Send a GET request to the specified Uri
    .Get("string")

    // Send a POST request to the specified Uri
    .Post("string")

     // Send a PUT request to the specified Uri
    .Put("string")

     // Sends a PATCH request to the specified Uri
    .Patch("string")

    // Send a DELETE request to the specified Uri
    .Delete("string")
```

#### Then
Specify the pass criteria for the test, which fails if not met.
```c#
 .Then()
    // Executes an assertion function that takes the `RIP.HttpResponse` in parameter
    .AssertThat(Action<RIP.HttpResponse>)

    // Returns the `RIP.HttpResponse` that wraps the original `HttpResponseMessage`
    .Retrieve()
```
