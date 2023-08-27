using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace QuizService.Tests;

public class QuizUpdateTest : IClassFixture<QuizAppFactory>
{
    private readonly QuizAppFactory _factory;
    public QuizUpdateTest(QuizAppFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Update_existing_quiz_with_valid_title_returns_204()
    {
        // arrange
        var client = _factory.CreateClient();
        const long quizId = 1;
        var newTitle = "New Title";
        var content = new StringContent(JsonConvert.SerializeObject(new { Title = newTitle }), Encoding.UTF8, "application/json");

        // act
        var response = await client.PutAsync(new Uri($"{QuizAppFactory.QuizApiEndPoint}{quizId}"), content);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Update_non_existing_quiz_returns_404()
    {
        // arrange
        var client = _factory.CreateClient();
        const long quizId = 999;
        var newTitle = "New Title";
        var content = new StringContent(JsonConvert.SerializeObject(new { Title = newTitle }), Encoding.UTF8, "application/json");

        // act
        var response = await client.PutAsync(new Uri($"{QuizAppFactory.QuizApiEndPoint}{quizId}"), content);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_quiz_with_empty_title_returns_400()
    {
        // arrange
        var client = _factory.CreateClient();
        const long quizId = 1;
        var newTitle = "";
        var content = new StringContent(JsonConvert.SerializeObject(new { Title = newTitle }), Encoding.UTF8, "application/json");

        // act
        var response = await client.PutAsync(new Uri($"{QuizAppFactory.QuizApiEndPoint}{quizId}"), content);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Update_quiz_with_same_title_returns_204()
    {
        // arrange
        var client = _factory.CreateClient();
        const long quizId = 1;
        var newTitle = "Quiz 1";
        var content = new StringContent(JsonConvert.SerializeObject(new { Title = newTitle }), Encoding.UTF8, "application/json");

        // act
        var response = await client.PutAsync(new Uri($"{QuizAppFactory.QuizApiEndPoint}{quizId}"), content);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Update_quiz_with_special_characters_in_title_returns_400()
    {
        // arrange
        var client = _factory.CreateClient();
        const long quizId = 1;
        var newTitle = "Quiz #1";
        var content = new StringContent(JsonConvert.SerializeObject(new { Title = newTitle }), Encoding.UTF8, "application/json");

        // act
        var response = await client.PutAsync(new Uri($"{QuizAppFactory.QuizApiEndPoint}{quizId}"), content);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Update_quiz_with_alpha_numeric_title_returns_204()
    {
        // arrange
        var client = _factory.CreateClient();
        const long quizId = 1;
        var newTitle = "Quiz 999";
        var content = new StringContent(JsonConvert.SerializeObject(new { Title = newTitle }), Encoding.UTF8, "application/json");

        // act
        var response = await client.PutAsync(new Uri($"{QuizAppFactory.QuizApiEndPoint}{quizId}"), content);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Update_quiz_title_reflected_in_get_request()
    {
        // arrange
        var client = _factory.CreateClient();
        const long quizId = 1;
        var newTitle = "New Title";
        var content = new StringContent(JsonConvert.SerializeObject(new { Title = newTitle }), Encoding.UTF8, "application/json");

        // act
        await client.PutAsync(new Uri($"{QuizAppFactory.QuizApiEndPoint}{quizId}"), content);
        var response = await client.GetAsync(new Uri($"{QuizAppFactory.QuizApiEndPoint}{quizId}"));

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        JObject quiz = JObject.Parse(await response.Content.ReadAsStringAsync());
        var title = quiz.GetValue("title", StringComparison.OrdinalIgnoreCase)?.Value<string>();
        title.Should().Be(newTitle);
    }
}