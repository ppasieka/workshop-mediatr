using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using QuizService.Model;
using Xunit;

namespace QuizService.Tests;

public class QuizzesControllerTest : IClassFixture<QuizAppFactory>
{
    const string QuizApiEndPoint = "/api/quizzes/";

    private readonly QuizAppFactory _factory;
    public QuizzesControllerTest(QuizAppFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PostNewQuizAddsQuiz()
    {
        var quiz = new QuizCreateModel("Test title");
        var client = _factory.CreateClient();
        var content = new StringContent(JsonConvert.SerializeObject(quiz));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var response = await client.PostAsync(new Uri($"{QuizApiEndPoint}"),
            content);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
    }

    [Fact]
    public async Task AQuizExistGetReturnsQuiz()
    {
        var client = _factory.CreateClient();
        const long quizId = 1;
        var response = await client.GetAsync(new Uri($"{QuizApiEndPoint}{quizId}"));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Content);
        var quiz = JsonConvert.DeserializeObject<QuizResponseModel>(await response.Content.ReadAsStringAsync());
        Assert.Equal(quizId, quiz.Id);
        Assert.Equal("My first quiz", quiz.Title);
    }

    [Fact]
    public async Task AQuizDoesNotExistGetFails()
    {
        var client = _factory.CreateClient();
        const long quizId = 999;
        var response = await client.GetAsync(new Uri($"{QuizApiEndPoint}{quizId}"));
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AQuizDoesNotExists_WhenPostingAQuestion_ReturnsNotFound()
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