using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
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
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Add_a_new_quiz_response_with_created_201_status()
    {
        var quiz = new QuizCreateModel("Test title");
        var client = _factory.CreateClient();
        var content = new StringContent(JsonConvert.SerializeObject(quiz));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var response = await client.PostAsync(new Uri($"{QuizAppFactory.QuizApiEndPoint}"),
            content);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
    }
    
    [Fact]
    public async Task Add_a_new_quiz_response_have_a_location_header()
    {
        var quiz = new QuizCreateModel("Test title");
        var client = _factory.CreateClient();
        var content = new StringContent(JsonConvert.SerializeObject(quiz));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var response = await client.PostAsync(new Uri($"{QuizAppFactory.QuizApiEndPoint}"),
            content);
        Assert.NotNull(response.Headers.Location);
    }

    [Fact]
    public async Task Get_an_existing_quiz_responds_with_ok_200_status()
    {
        var client = _factory.CreateClient();
        const long quizId = 1;
        var response = await client.GetAsync(new Uri($"{QuizAppFactory.QuizApiEndPoint}{quizId}"));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task Get_an_existing_quiz_response_contains_quiz_model()
    {
        var client = _factory.CreateClient();
        const long quizId = 1;
        var response = await client.GetAsync(new Uri($"{QuizAppFactory.QuizApiEndPoint}{quizId}"));
        var quiz = JsonConvert.DeserializeObject<QuizResponseModel>(await response.Content.ReadAsStringAsync());
        Assert.Equal(quizId, quiz.Id);
        Assert.Equal("My first quiz", quiz.Title);
    }

    [Fact]
    public async Task Request_for_non_existing_quiz_response_with_not_found_404_status()
    {
        var client = _factory.CreateClient();
        const long quizId = 999;
        var response = await client.GetAsync(new Uri($"{QuizAppFactory.QuizApiEndPoint}{quizId}"));
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Add_a_question_to_non_existing_quiz_response_with_not_found_404_status()
    {
        const string QuizApiEndPoint = "/api/quizzes/999/questions";

        var client = _factory.CreateClient();
        var question = new QuestionCreateModel("The answer to everything is what?");
        var content = new StringContent(JsonConvert.SerializeObject(question));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var response = await client.PostAsync(new Uri($"{QuizApiEndPoint}"),content);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}