using BusinessObject.Dtos.Quiz;
using GenerativeAI;
using Microsoft.Extensions.Configuration;
using Service.QuizzInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service.QuizzMethod
{
    public class GeminiService : IGeminiService
    {

        private readonly GoogleAi _googleAI;
        private readonly string _apiKey;

        public GeminiService(IConfiguration configuration)
        {
            _apiKey = configuration["Gemini:ApiKey"]
                ?? throw new ArgumentNullException("Gemini API Key not found in configuration");

            _googleAI = new GoogleAi(_apiKey);
        }

        public async Task<QuizGenerationResult> GenerateQuizFromLessonAsync(
            string lessonContent,
            int numberOfQuestions = 5)
        {
            var prompt = BuildPrompt(lessonContent, numberOfQuestions);

            try
            {
                var model = _googleAI.CreateGenerativeModel("models/gemini-2.5-flash");
                var response = await model.GenerateContentAsync(prompt);
                var generatedText = response.Text();

                if (string.IsNullOrEmpty(generatedText))
                    throw new Exception("Empty response from Gemini API");

                // Parse và validate response
                return ParseAndValidateQuizResponse(generatedText, numberOfQuestions);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating quiz with Gemini: {ex.Message}", ex);
            }
        }

       private string BuildPrompt(string lessonContent, int numberOfQuestions)
{
    return $@"
Based on the following lesson content, especially focusing on section 'D. Procedures – Part 2: New Lesson',
generate {numberOfQuestions} multiple-choice questions (MCQs) in ENGLISH.

Lesson Content:
{lessonContent}

Requirements:
- Questions must be written entirely in English.
- Focus mainly on the 'D. Procedures – Part 2: New Lesson' section and its related concepts or activities.
- Each question must test understanding of the lesson content, grammar, vocabulary, or communicative context.
- Each question should have **4 answer options** (A, B, C, D).
- **Only ONE** correct answer per question.
- Randomize the order of answers — the correct answer should not always be the first one.
- Mix difficulty levels (easy, medium, difficult).
- Do NOT include any explanations, comments, or markdown (no ```json, no text before/after).
- Return ONLY the following pure JSON structure:

{{
  ""questions"": [
    {{
      ""content"": ""Question text in English?"",
      ""answers"": [
        {{
          ""content"": ""Answer 1"",
          ""isCorrect"": true
        }},
        {{
          ""content"": ""Answer 2"",
          ""isCorrect"": false
        }},
        {{
          ""content"": ""Answer 3"",
          ""isCorrect"": false
        }},
        {{
          ""content"": ""Answer 4"",
          ""isCorrect"": false
        }}
      ]
    }}
  ]
}}";
}

        // ========== BỔ SUNG: PARSE VÀ VALIDATE AI RESPONSE ==========
        private QuizGenerationResult ParseAndValidateQuizResponse(string text, int expectedQuestionCount)
        {
            if (string.IsNullOrEmpty(text))
                throw new InvalidOperationException("AI response is empty");

            // Clean markdown code blocks
            text = CleanMarkdownCodeBlocks(text);

            // Parse JSON
            QuizGenerationResult result;
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                result = JsonSerializer.Deserialize<QuizGenerationResult>(text, options);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException(
                    $"Failed to parse AI response as JSON. Error: {ex.Message}. Response text: {text.Substring(0, Math.Min(200, text.Length))}...",
                    ex
                );
            }

            // Validate parsed result
            ValidateQuizGenerationResult(result, expectedQuestionCount);

            return result;
        }

        private string CleanMarkdownCodeBlocks(string text)
        {
            text = text.Trim();

            // Remove ```json ... ```
            if (text.StartsWith("```json"))
            {
                text = text.Substring(7);
                var endIndex = text.LastIndexOf("```");
                if (endIndex > 0)
                    text = text.Substring(0, endIndex);
            }
            // Remove ``` ... ```
            else if (text.StartsWith("```"))
            {
                text = text.Substring(3);
                var endIndex = text.LastIndexOf("```");
                if (endIndex > 0)
                    text = text.Substring(0, endIndex);
            }

            return text.Trim();
        }

        // ========== VALIDATION METHODS ==========

        private void ValidateQuizGenerationResult(QuizGenerationResult result, int expectedQuestionCount)
        {
            // VALIDATION 1: Check null result
            if (result == null)
                throw new InvalidOperationException("AI returned null result");

            // VALIDATION 2: Check questions list exists
            if (result.Questions == null)
                throw new InvalidOperationException("AI response does not contain 'questions' array");

            // VALIDATION 3: Check questions list not empty
            if (!result.Questions.Any())
                throw new InvalidOperationException("AI returned empty questions list");

            // VALIDATION 4: Check questions count
            var actualCount = result.Questions.Count;
            if (actualCount < expectedQuestionCount - 1) // Allow -1 tolerance
            {
                throw new InvalidOperationException(
                    $"AI generated insufficient questions. Expected: {expectedQuestionCount}, Got: {actualCount}"
                );
            }
            if (actualCount > expectedQuestionCount)
            {
                throw new InvalidOperationException(
                    $"AI generated too many questions. Expected: {expectedQuestionCount}, Got: {actualCount}"
                );
            }

            // VALIDATION 5: Validate each question
            for (int i = 0; i < result.Questions.Count; i++)
            {
                ValidateQuestion(result.Questions[i], i + 1);
            }

            // VALIDATION 6: Check for duplicate questions
            ValidateNoDuplicateQuestions(result.Questions);
        }

        private void ValidateQuestion(GeneratedQuestion question, int questionNumber)
        {
            // Validate question content
            if (string.IsNullOrWhiteSpace(question.Content))
            {
                throw new InvalidOperationException(
                    $"Question #{questionNumber}: Content is empty or whitespace"
                );
            }

            var trimmedContent = question.Content.Trim();
            if (trimmedContent.Length < 10)
            {
                throw new InvalidOperationException(
                    $"Question #{questionNumber}: Content too short (minimum 10 characters). Content: '{trimmedContent}'"
                );
            }

            if (trimmedContent.Length > 500)
            {
                throw new InvalidOperationException(
                    $"Question #{questionNumber}: Content too long (maximum 500 characters)"
                );
            }

            // Validate answers list
            if (question.Answers == null || !question.Answers.Any())
            {
                throw new InvalidOperationException(
                    $"Question #{questionNumber}: Has no answers"
                );
            }

            if (question.Answers.Count != 4)
            {
                throw new InvalidOperationException(
                    $"Question #{questionNumber}: Must have exactly 4 answers, but has {question.Answers.Count}"
                );
            }

            // Validate each answer
            for (int i = 0; i < question.Answers.Count; i++)
            {
                ValidateAnswer(question.Answers[i], questionNumber, i + 1);
            }

            // Validate correct answers count
            var correctAnswersCount = question.Answers.Count(a => a.IsCorrect);
            if (correctAnswersCount == 0)
            {
                throw new InvalidOperationException(
                    $"Question #{questionNumber}: Has no correct answer. All answers are marked as incorrect."
                );
            }

            if (correctAnswersCount > 1)
            {
                throw new InvalidOperationException(
                    $"Question #{questionNumber}: Has {correctAnswersCount} correct answers. Only 1 correct answer is allowed."
                );
            }

            // Validate no duplicate answers
            ValidateNoDuplicateAnswers(question.Answers, questionNumber);
        }

        private void ValidateAnswer(GeneratedAnswer answer, int questionNumber, int answerNumber)
        {
            if (string.IsNullOrWhiteSpace(answer.Content))
            {
                throw new InvalidOperationException(
                    $"Question #{questionNumber}, Answer #{answerNumber}: Content is empty or whitespace"
                );
            }

            var trimmedContent = answer.Content.Trim();
            if (trimmedContent.Length < 1)
            {
                throw new InvalidOperationException(
                    $"Question #{questionNumber}, Answer #{answerNumber}: Content too short (minimum 1 character)"
                );
            }

            if (trimmedContent.Length > 500)
            {
                throw new InvalidOperationException(
                    $"Question #{questionNumber}, Answer #{answerNumber}: Content too long (maximum 500 characters)"
                );
            }
        }

        private void ValidateNoDuplicateAnswers(List<GeneratedAnswer> answers, int questionNumber)
        {
            var answerContents = answers
                .Select(a => a.Content.Trim().ToLowerInvariant())
                .ToList();

            var duplicates = answerContents
                .GroupBy(c => c)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicates.Any())
            {
                throw new InvalidOperationException(
                    $"Question #{questionNumber}: Has duplicate answers: '{string.Join("', '", duplicates)}'"
                );
            }
        }

        private void ValidateNoDuplicateQuestions(List<GeneratedQuestion> questions)
        {
            var questionContents = questions
                .Select((q, index) => new { Content = q.Content.Trim().ToLowerInvariant(), Index = index + 1 })
                .ToList();

            var duplicateGroups = questionContents
                .GroupBy(q => q.Content)
                .Where(g => g.Count() > 1)
                .ToList();

            if (duplicateGroups.Any())
            {
                var duplicateInfo = string.Join("; ", duplicateGroups.Select(g =>
                    $"'{g.Key}' appears in questions: {string.Join(", ", g.Select(x => $"#{x.Index}"))}"
                ));

                throw new InvalidOperationException(
                    $"Duplicate questions detected: {duplicateInfo}"
                );
            }
        }
    }

}