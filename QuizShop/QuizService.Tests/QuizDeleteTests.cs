using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace QuizService.Tests;

public class QuizDeleteTests : IClassFixture<QuizAppFactory>
{
    private readonly QuizAppFactory _factory;
    public QuizDeleteTests(QuizAppFactory factory)
    {
        _factory = factory;
    }
    
    [Fact]
    public async Task Delete_exising_quiz_responds_with_no_content_204_status()
    {
        // arrange
        var client = _factory.CreateClient();
        const long quizId = 1;
        
        // act
        var response = await client.DeleteAsync(new Uri($"{QuizAppFactory.QuizApiEndPoint}{quizId}"));
        
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task Delete_non_exising_quiz_responds_with_not_found_404_status()
    {
        // arrange
        var client = _factory.CreateClient();
        const long quizId = 999;
        
        // act
        var response = await client.DeleteAsync(new Uri($"{QuizAppFactory.QuizApiEndPoint}{quizId}"));
        
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}