using BusinessObject.Dtos.Quiz;
using BusinessObject.Dtos.LessonDTO;
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
            return $$"""
You are an expert English teacher creating a post-lesson quiz for students who have just finished this exact lesson.
Your quiz MUST test the LANGUAGE and SKILLS that the students actually learnt and practised today — never test the lesson plan document itself.

LESSON THAT THE STUDENTS JUST STUDIED:
{{lessonContent}}

STRICT RULES — BREAK ANY OF THESE → REFUSE AND RETURN ONLY THE ERROR JSON:

1. First silently check: the text above must be a real English lesson with real teaching content (vocabulary, grammar, skills practice, dialogues, texts, etc.).  
   If it is too short (< 120 words), harmful, spam, or clearly not a real lesson → return ONLY:
   {
     "error": "CONTENT_INVALID",
     "message": "Unable to generate quiz: The provided content is either insufficient, inappropriate, or not educational in nature."
   }

2. YOU ARE STRICTLY FORBIDDEN to create any question about:
   • the structure or names of activities (Activity 1, Activity 2, Warm-up, Consolidation…)
   • time allocation (how many minutes…)
   • interaction patterns (T → Ss, pair work, individual…)
   • teacher instructions or procedure steps
   • the title of the unit or lesson
   • anything that is ONLY in the lesson plan document but NOT actually taught to students as language

   → If you catch yourself thinking of such a question like “What is the topic of Unit 6?” or “What does the teacher do in Activity 3?” → delete it immediately.

3. ONLY create questions that test real student knowledge and ability to USE the language from today’s lesson, for example:
   ✓ Meaning, spelling, pronunciation or use of new vocabulary/phrases
   ✓ Correct grammar forms (Present Perfect Continuous, Future Perfect, etc.)
   ✓ Choosing the correct expression in a real situation
   ✓ Completing dialogues or sentences students practised
   ✓ Understanding the listening/reading text they studied
   ✓ Producing correct sentences with target structures

4. Requirements:
   • Generate maximum {{numberOfQuestions}} questions (or fewer if the lesson does not support that many strong application questions)
   • Every question and every option 100% in English
   • Exactly 4 choices (A, B, C, D) — exactly ONE is correct
   • Correct answer in random position
   • Difficulty mix: 30% easy, 50% medium, 20% difficult
   • Distractors must be very plausible (common student mistakes or similar items from the same lesson)

5. Output MUST be ONLY clean JSON (no markdown, no ```, no extra text whatsoever):

{
  "questions": [
    {
      "content": "Real question that tests what students actually learnt and can apply?",
      "answers": [
        { "content": "Option A", "isCorrect": false },
        { "content": "Option B", "isCorrect": true },
        { "content": "Option C", "isCorrect": false },
        { "content": "Option D", "isCorrect": false }
      ]
    }
    // ... more questions
  ]
}

Now generate {{numberOfQuestions}} (or fewer) REAL application questions based ONLY on the language content, vocabulary, grammar, and skills that the students actually studied and practised in the lesson above.
""";
        }

        // ========== PARSE RESPONSE ==========//
        private QuizGenerationResult ParseAndValidateQuizResponse(string text, int expectedQuestionCount)
        {
            if (string.IsNullOrEmpty(text))
                throw new InvalidOperationException("AI response is empty");

            // Clean markdown code blocks
            text = CleanMarkdownCodeBlocks(text);

            // Check for error response first
            if (text.Contains("\"error\"") && text.Contains("\"CONTENT_INVALID\""))
            {
                throw new InvalidOperationException(
                    "The provided lesson content is insufficient, inappropriate, or not educational. " +
                    "Please provide valid educational content with sufficient detail to generate quiz questions."
                );
            }

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

            // Validate parsed result - relaxed for content-based generation
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
            if (result == null)
                throw new InvalidOperationException("AI returned null result");

            if (result.Questions == null)
                throw new InvalidOperationException("AI response does not contain 'questions' array");

            if (!result.Questions.Any())
                throw new InvalidOperationException("AI returned empty questions list. The lesson content may be insufficient for quiz generation.");

            // VALIDATION: Check questions count (more flexible for content-based generation)
            var actualCount = result.Questions.Count;

            // Allow AI to generate fewer questions if content is limited
            if (actualCount == 0)
            {
                throw new InvalidOperationException(
                    "Could not generate any questions from the provided content. Please ensure the lesson contains sufficient educational material."
                );
            }

            // Warning instead of error if fewer questions generated
            if (actualCount < expectedQuestionCount)
            {
              
                // This allows the system to work with limited content
                Console.WriteLine($"Warning: Generated {actualCount} questions instead of {expectedQuestionCount} due to content limitations.");
            }

            if (actualCount > expectedQuestionCount + 2) // Allow slight over-generation
            {
                throw new InvalidOperationException(
                    $"AI generated too many questions. Expected: {expectedQuestionCount}, Got: {actualCount}"
                );
            }

            for (int i = 0; i < result.Questions.Count; i++)
            {
                ValidateQuestion(result.Questions[i], i + 1);
            }

            ValidateNoDuplicateQuestions(result.Questions);
        }

        private void ValidateQuestion(GeneratedQuestion question, int questionNumber)
        {
            // Validate content
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

        // ========== LESSON PLANNER AI GENERATION ==========

        public async Task<GeneratedLessonPlannerResult> GenerateLessonPlannerAsync(GenerateLessonPlannerRequest request)
        {
            var prompt = BuildLessonPlannerPrompt(request);

            try
            {
                var model = _googleAI.CreateGenerativeModel("models/gemini-2.5-flash");
                var response = await model.GenerateObjectAsync<GeneratedLessonPlannerResult>(prompt);

                // Parse and validate response
                ValidateLessonPlannerResult(response);

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating lesson planner with Gemini: {ex.Message}", ex);
            }
        }

        private string BuildLessonPlannerPrompt(GenerateLessonPlannerRequest request)
        {
            return $@"
You are an expert educational curriculum designer. Generate a comprehensive lesson plan based on the following requirements:

**Lesson Information:**
- Title: {request.Title}
- Topic: {request.Topic}
- Grade Level: {request.GradeLevel ?? "Not specified"}
- Duration: {request.DurationInMinutes ?? 45} minutes
- Learning Objectives: {request.LearningObjectives ?? "To be determined based on topic"}
- Additional Instructions: {request.AdditionalInstructions ?? "None"}

**Requirements:**
1. Create a detailed, well-structured lesson plan suitable for English language teaching
2. Include all necessary components: objectives, skills, attitudes, language focus, preparations, and activity stages
3. Design activities that are engaging, age-appropriate, and aligned with communicative language teaching principles
4. Ensure proper time allocation for each activity (total should be approximately {request.DurationInMinutes ?? 45} minutes)
5. Include varied interaction patterns (T-S, S-S, individual work, group work, etc.)

**Important Notes:**
- Ensure at least 3 objectives covering knowledge, skills, and attitudes
- Include at least 2-3 language focus items
- Create 4-5 activity stages following a logical teaching sequence
- Each activity stage should have 1-3 activity items
- Materials in Preparations is preparation's description (what to do, what to prepare)
- Vary interaction patterns throughout the lesson
- Total activity time should sum to approximately {request.DurationInMinutes ?? 45} minutes
- All text should be in English and professionally formatted
- Do NOT include any markdown formatting or code blocks in your response
";
        }

        private void ValidateLessonPlannerResult(GeneratedLessonPlannerResult? result)
        {
            if (result == null)
                throw new InvalidOperationException("AI returned null result");

            if (string.IsNullOrWhiteSpace(result.Title))
                throw new InvalidOperationException("Lesson planner must have a title");

            if (result.ActivityStages == null || !result.ActivityStages.Any())
                throw new InvalidOperationException("Lesson planner must have at least one activity stage");

            // Validate each activity stage
            foreach (var stage in result.ActivityStages)
            {
                if (string.IsNullOrWhiteSpace(stage.StageName))
                    throw new InvalidOperationException("Each activity stage must have a name");

                if (stage.ActivityItems == null || !stage.ActivityItems.Any())
                    throw new InvalidOperationException($"Activity stage '{stage.StageName}' must have at least one activity item");
            }
        }
    }

}