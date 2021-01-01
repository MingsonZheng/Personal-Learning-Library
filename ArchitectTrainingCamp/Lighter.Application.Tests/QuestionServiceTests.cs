using FluentAssertions;
using Lighter.Application.Contracts.Dto;
using LighterApi.Data.Question;
using LighterApi.Dto;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Lighter.Application.Tests
{

    [Collection(nameof(SharedFixture))]
    public class QuestionServiceTests
    {
        private readonly SharedFixture _fixture;

        private readonly QuestionService _questionService;
        public QuestionServiceTests(SharedFixture fixture)
        {
            _fixture = fixture;
            _questionService = new QuestionService(_fixture.Client);
        }

        private async Task<Question> CreateOrGetOneQuestionWithNoAnswerAsync()
        {
            var collection = _fixture.Database.GetCollection<Question>("question");
            var filter = Builders<Question>.Filter.Size(q => q.Answers, 0);
            var question = await collection.Find(filter).FirstOrDefaultAsync();

            if (question != null)
                return question;

            question = new Question { Title = "问题一" };
            return await _questionService.CreateAsync(question, CancellationToken.None);
        }

        private async Task<QuestionAnswerReponse> CreateOrGetOneQuestionWithAnswerAsync()
        {
            var collection = _fixture.Database.GetCollection<Question>("question");
            var filter = Builders<Question>.Filter.SizeGt(q => q.Answers, 0);
            var question = await collection.Find(filter).FirstOrDefaultAsync();

            if (question != null)
                return await _questionService.GetWithAnswerAsync(question.Id, CancellationToken.None);

            // 不存在则创建一个没有回答的问题，再添加一个答案
            question = await CreateOrGetOneQuestionWithNoAnswerAsync();
            var answer = new AnswerRequest { Content = "问题一的回答一" };
            await _questionService.AnswerAsync(question.Id, answer, CancellationToken.None);

            return await _questionService.GetWithAnswerAsync(question.Id, CancellationToken.None);
        }


        [Fact]
        public async Task GetAsync_WrongId_ShoudReturnNull()
        {
            var result = await _questionService.GetAsync("empty", CancellationToken.None);
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_Right_ShouldBeOk()
        {
            var question = await CreateOrGetOneQuestionWithNoAnswerAsync();
            question.Should().NotBeNull();

            var result = await _questionService.GetAsync(question.Id, CancellationToken.None);
            question.Title.Should().Be(result.Title);
        }

        [Fact]
        public async Task AnswerAsync_Right_ShouldBeOk()
        {
            var question = await CreateOrGetOneQuestionWithNoAnswerAsync();
            question.Should().NotBeNull();

            var answer = new AnswerRequest { Content = "问题一的回答一" };
            await _questionService.AnswerAsync(question.Id, answer, CancellationToken.None);

            var questionWithAnswer = await _questionService.GetWithAnswerAsync(question.Id, CancellationToken.None);

            questionWithAnswer.Should().NotBeNull();
            questionWithAnswer.AnswerList.Should().NotBeEmpty();
            questionWithAnswer.AnswerList.First().Content.Should().Be(answer.Content);
        }

        [Fact]
        public async Task UpAsync_Right_ShouldBeOk()
        {
            var before = await CreateOrGetOneQuestionWithNoAnswerAsync();
            await _questionService.UpAsync(before.Id, CancellationToken.None);

            var after = await _questionService.GetAsync(before.Id, CancellationToken.None);
            after.Should().NotBeNull();
            after.VoteCount.Should().Be(before.VoteCount+1);
            after.VoteUps.Count.Should().Be(1);
        }

        [Fact]
        public async Task DownAsync_Right_ShouldBeOk()
        {
            var before = await CreateOrGetOneQuestionWithNoAnswerAsync();
            await _questionService.DownAsync(before.Id, CancellationToken.None);

            var after = await _questionService.GetAsync(before.Id, CancellationToken.None);
            after.Should().NotBeNull();
            after.VoteCount.Should().Be(before.VoteCount-1);
            after.VoteDowns.Count.Should().Be(1);
        }


        public async Task UpdateAsync_WithNoSummary_ShoudThrowException()
        {
            var before = await CreateOrGetOneQuestionWithNoAnswerAsync();
            var updateRequest = new QuestionUpdateRequest { Title = before.Title + "-updated" };
            await _questionService.UpdateAsync(before.Id, updateRequest, CancellationToken.None);

            var after = await _questionService.GetAsync(before.Id, CancellationToken.None);
            after.Should().NotBeNull();
            after.Title.Should().Be(updateRequest.Title);
        }


        [Fact]
        public async Task UpdateAsync_Right_ShoudBeOk()
        {
            var before = await CreateOrGetOneQuestionWithNoAnswerAsync();
            var updateRequest = new QuestionUpdateRequest { Title = before.Title + "-updated", Summary ="summary" };
            await _questionService.UpdateAsync(before.Id, updateRequest , CancellationToken.None);

            var after = await _questionService.GetAsync(before.Id, CancellationToken.None);
            after.Should().NotBeNull();
            after.Title.Should().Be(updateRequest.Title);
        }


        [Fact]
        public async Task UpdateAsync_Right_CommentsShouldAppend()
        {
            var before = await CreateOrGetOneQuestionWithNoAnswerAsync();
            var updateRequest = new QuestionUpdateRequest { Title = before.Title + "-updated", Summary = "summary" };
            await _questionService.UpdateAsync(before.Id, updateRequest, CancellationToken.None);

            var after = await _questionService.GetAsync(before.Id, CancellationToken.None);
            after.Comments.Should().NotBeEmpty();
            after.Comments.Count.Should().Be(before.Comments.Count+1);
        }
    }
}
