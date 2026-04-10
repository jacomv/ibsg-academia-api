using Academia.Domain.Enums;

namespace Academia.Application.Lessons.Validators;

public static class LessonContentValidator
{
    public record ValidationResult(bool IsValid, List<string> Errors);

    public static ValidationResult Validate(
        LessonType type, string? textContent, string? videoUrl,
        string? audioUrl, string? pdfFile)
    {
        var errors = new List<string>();

        switch (type)
        {
            case LessonType.Video:
                if (string.IsNullOrWhiteSpace(videoUrl))
                    errors.Add("Video lessons require a video URL.");
                break;

            case LessonType.Audio:
                if (string.IsNullOrWhiteSpace(audioUrl))
                    errors.Add("Audio lessons require an audio URL.");
                break;

            case LessonType.Text:
                if (string.IsNullOrWhiteSpace(textContent))
                    errors.Add("Text lessons require content.");
                break;

            case LessonType.Pdf:
                if (string.IsNullOrWhiteSpace(pdfFile))
                    errors.Add("PDF lessons require a PDF file.");
                break;

            case LessonType.Mixed:
                var hasAny = !string.IsNullOrWhiteSpace(textContent)
                    || !string.IsNullOrWhiteSpace(videoUrl)
                    || !string.IsNullOrWhiteSpace(audioUrl)
                    || !string.IsNullOrWhiteSpace(pdfFile);
                if (!hasAny)
                    errors.Add("Mixed lessons require at least one content field.");
                break;

            case LessonType.Section:
                // Sections are structural — no content required
                break;
        }

        return new ValidationResult(errors.Count == 0, errors);
    }
}
