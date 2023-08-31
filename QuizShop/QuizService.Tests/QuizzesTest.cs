using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using QuizService.Model;
using Xunit;

namespace QuizService.Tests;

public class QuizzesTest : IClassFixture<QuizAppFactory>
{

    private readonly QuizAppFactory _factory;
    public QuizzesTest(QuizAppFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_all_quizzes()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync(new Uri($"{QuizAppFactory.QuizApiEndPoint}"));
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Add_a_new_quiz_response_with_created_201_status()
    {
        var quiz = new { Title = $"Test title {Guid.NewGuid():N}" };
        var client = _factory.CreateClient();
        var content = new StringContent(JsonConvert.SerializeObject(quiz));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var response = await client.PostAsync(new Uri($"{QuizAppFactory.QuizApiEndPoint}"),
            content);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
    
    [Fact]
    public async Task Add_a_new_quiz_response_have_a_location_header()
    {
        var quiz = new { Title = $"Test title {Guid.NewGuid():N}" };
        var client = _factory.CreateClient();
        var content = new StringContent(JsonConvert.SerializeObject(quiz));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var response = await client.PostAsync(new Uri($"{QuizAppFactory.QuizApiEndPoint}"),
            content);
        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task Get_an_existing_quiz_responds_with_ok_200_status()
    {
        var client = _factory.CreateClient();
        const long quizId = 1;
        var response = await client.GetAsync(new Uri($"{QuizAppFactory.QuizApiEndPoint}{quizId}"));
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task Get_an_existing_quiz_response_contains_quiz_model()
    {
        var client = _factory.CreateClient();
        const long quizId = 1;
        var response = await client.GetAsync(new Uri($"{QuizAppFactory.QuizApiEndPoint}{quizId}"));
        var quiz = JsonConvert.DeserializeObject<QuizItemResponse>(await response.Content.ReadAsStringAsync());
        quiz.Should().NotBeNull();
        quiz!.Id.Should().Be(quizId);
        quiz.Title.Should().Be("My first quiz");
    }

    [Fact]
    public async Task Request_for_non_existing_quiz_response_with_not_found_404_status()
    {
        var client = _factory.CreateClient();
        const long quizId = 999;
        var response = await client.GetAsync(new Uri($"{QuizAppFactory.QuizApiEndPoint}{quizId}"));
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Add_a_question_to_non_existing_quiz_response_with_not_found_404_status()
    {
        const string quizApiEndPoint = "/api/quizzes/999/questions";

        var client = _factory.CreateClient();
        var question = new AddQuestionRequest("The answer to everything is what?");
        var content = new StringContent(JsonConvert.SerializeObject(question));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var response = await client.PostAsync(new Uri($"{quizApiEndPoint}"),content);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}