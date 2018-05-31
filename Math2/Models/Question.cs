using Math2.Enums;
using Newtonsoft.Json;

namespace Math2.Models
{
    public class Question
    {
        public string Id { get; set; }
        public string QuestionText { get; set; }
        public EnumQuestionStatus Status { get; set; }

        [JsonIgnore]
        public string IsCorrect { get; set; }
    }
}