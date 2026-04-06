using System.Text.Json;
using Academia.Application.Common.Interfaces;
using Academia.Domain.Entities;
using Academia.Domain.Enums;
using Academia.Infrastructure.Persistence;
using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Academia.Infrastructure.DataMigration;

/// <summary>
/// Migrates data from the Directus PostgreSQL database to the new .NET schema.
/// Run once, from a safe environment, before cutover.
/// </summary>
public class DirectusMigrationService : IMigrationService
{
    private readonly AcademiaDbContext _target;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<DirectusMigrationService> _logger;

    // Directus role UUIDs → new role enum mapping (from .env)
    private readonly Dictionary<string, UserRole> _roleMap = new()
    {
        ["ca75757c-a6c0-4a8f-a5cd-71e06341d797"] = UserRole.Administrator,
        ["939b84da-70ec-40e8-bb0b-4317e6fa033d"] = UserRole.Teacher,
        ["f93326ee-8b51-415f-baae-2db0556b1f3d"] = UserRole.Student,
    };

    public DirectusMigrationService(
        AcademiaDbContext target,
        IPasswordHasher passwordHasher,
        ILogger<DirectusMigrationService> logger)
    {
        _target = target;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<MigrationResult> MigrateFromDirectusAsync(
        string directusConnectionString, CancellationToken ct = default)
    {
        var errors = new List<string>();
        var stats = new MigrationStats(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

        await using var source = new NpgsqlConnection(directusConnectionString);
        await source.OpenAsync(ct);
        _logger.LogInformation("Connected to Directus database. Starting migration...");

        try
        {
            // Order matters: respect foreign key dependencies
            stats = stats with { Users = await MigrateUsersAsync(source, errors, ct) };
            stats = stats with { Courses = await MigrateCoursesAsync(source, errors, ct) };
            stats = stats with { Chapters = await MigrateChaptersAsync(source, errors, ct) };
            stats = stats with { Lessons = await MigrateLessonsAsync(source, errors, ct) };
            stats = stats with { LearningPaths = await MigrateLearningPathsAsync(source, errors, ct) };
            stats = stats with { Exams = await MigrateExamsAsync(source, errors, ct) };
            stats = stats with { Questions = await MigrateQuestionsAsync(source, errors, ct) };
            stats = stats with { Enrollments = await MigrateEnrollmentsAsync(source, errors, ct) };
            stats = stats with { UserProgress = await MigrateProgressAsync(source, errors, ct) };
            stats = stats with { ExamAnswers = await MigrateExamAnswersAsync(source, errors, ct) };
            stats = stats with { Grades = await MigrateGradesAsync(source, errors, ct) };
            stats = stats with { Notifications = await MigrateNotificationsAsync(source, errors, ct) };

            _logger.LogInformation("Migration completed. Stats: {@Stats}", stats);
            return new MigrationResult(true, "Migration completed successfully.", stats, errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Migration failed");
            errors.Add($"Fatal error: {ex.Message}");
            return new MigrationResult(false, "Migration failed.", stats, errors);
        }
    }

    // ─── Users ──────────────���───────────────────────────────────���─────────────

    private async Task<int> MigrateUsersAsync(
        NpgsqlConnection source, List<string> errors, CancellationToken ct)
    {
        _logger.LogInformation("Migrating users...");
        var rows = await source.QueryAsync(
            "SELECT id, first_name, last_name, email, password, role, status " +
            "FROM directus_users WHERE role IS NOT NULL");

        var count = 0;
        foreach (var row in rows)
        {
            try
            {
                string id = row.id.ToString();
                string roleId = row.role?.ToString() ?? "";

                if (!_roleMap.TryGetValue(roleId, out var role))
                {
                    errors.Add($"User {id}: unknown role UUID {roleId}, defaulting to Student");
                    role = UserRole.Student;
                }

                var existing = await _target.Users.FindAsync(new object[] { Guid.Parse(id) }, ct);
                if (existing is not null) continue; // skip already migrated

                // Handle Directus Argon2 passwords — keep as-is, app will re-hash on first login
                var passwordHash = row.password?.ToString() ?? _passwordHasher.Hash("ChangeMe123!");

                var user = new User(
                    row.first_name?.ToString() ?? "Unknown",
                    row.last_name?.ToString() ?? "User",
                    row.email?.ToString() ?? $"user_{id}@migrated.local",
                    passwordHash,
                    role);

                // Preserve original ID from Directus
                SetId(user, Guid.Parse(id));

                _target.Users.Add(user);
                count++;
            }
            catch (Exception ex)
            {
                errors.Add($"User row error: {ex.Message}");
            }
        }
        await _target.SaveChangesAsync(ct);
        _logger.LogInformation("Migrated {Count} users", count);
        return count;
    }

    // ─── Courses ────────────────────────────��──────────────────────────��──────

    private async Task<int> MigrateCoursesAsync(
        NpgsqlConnection source, List<string> errors, CancellationToken ct)
    {
        _logger.LogInformation("Migrating courses...");
        var rows = await source.QueryAsync(
            "SELECT id, titulo, descripcion, imagen, estado, tipo_acceso, precio, " +
            "duracion_estimada, maestro FROM cursos");

        var count = 0;
        foreach (var row in rows)
        {
            try
            {
                var id = Guid.Parse(row.id.ToString());
                var existing = await _target.Courses.FindAsync(new object[] { id }, ct);
                if (existing is not null) continue;

                var course = new Course(
                    title: row.titulo?.ToString() ?? "Untitled",
                    description: row.descripcion?.ToString(),
                    status: MapStatus(row.estado?.ToString()),
                    accessType: MapAccessType(row.tipo_acceso?.ToString()),
                    price: row.precio is not null ? (decimal?)Convert.ToDecimal(row.precio) : null,
                    estimatedDuration: row.duracion_estimada is not null
                        ? (int?)Convert.ToInt32(row.duracion_estimada) : null,
                    teacherId: row.maestro is not null
                        ? (Guid?)Guid.Parse(row.maestro.ToString()) : null);

                if (row.imagen is not null)
                    course.SetImage(row.imagen.ToString());

                SetId(course, id);
                _target.Courses.Add(course);
                count++;
            }
            catch (Exception ex) { errors.Add($"Course {row.id}: {ex.Message}"); }
        }
        await _target.SaveChangesAsync(ct);
        _logger.LogInformation("Migrated {Count} courses", count);
        return count;
    }

    // ─── Chapters ─────────────────────────────────────────────────────────────

    private async Task<int> MigrateChaptersAsync(
        NpgsqlConnection source, List<string> errors, CancellationToken ct)
    {
        _logger.LogInformation("Migrating chapters...");
        var rows = await source.QueryAsync(
            "SELECT id, curso_id, titulo, descripcion, orden, fecha_liberacion FROM capitulos");

        var count = 0;
        foreach (var row in rows)
        {
            try
            {
                var id = Guid.Parse(row.id.ToString());
                var existing = await _target.Chapters.FindAsync(new object[] { id }, ct);
                if (existing is not null) continue;

                var chapter = new Chapter(
                    courseId: Guid.Parse(row.curso_id.ToString()),
                    title: row.titulo?.ToString() ?? "Chapter",
                    description: row.descripcion?.ToString(),
                    order: Convert.ToInt32(row.orden),
                    availableFrom: row.fecha_liberacion is not null
                        ? (DateTime?)Convert.ToDateTime(row.fecha_liberacion) : null);

                SetId(chapter, id);
                _target.Chapters.Add(chapter);
                count++;
            }
            catch (Exception ex) { errors.Add($"Chapter {row.id}: {ex.Message}"); }
        }
        await _target.SaveChangesAsync(ct);
        _logger.LogInformation("Migrated {Count} chapters", count);
        return count;
    }

    // ─── Lessons ───────────────────────────────���────────────────────────────���─

    private async Task<int> MigrateLessonsAsync(
        NpgsqlConnection source, List<string> errors, CancellationToken ct)
    {
        _logger.LogInformation("Migrating lessons...");
        var rows = await source.QueryAsync(
            "SELECT id, capitulo_id, titulo, tipo, contenido_texto, video_url, audio_url, " +
            "archivo_pdf, duracion_minutos, orden, requiere_completar_anterior, fecha_liberacion " +
            "FROM lecciones");

        var count = 0;
        foreach (var row in rows)
        {
            try
            {
                var id = Guid.Parse(row.id.ToString());
                var existing = await _target.Lessons.FindAsync(new object[] { id }, ct);
                if (existing is not null) continue;

                var lessonType = MapLessonType(row.tipo?.ToString());
                var lesson = new Lesson(
                    chapterId: Guid.Parse(row.capitulo_id.ToString()),
                    title: row.titulo?.ToString() ?? "Lesson",
                    type: lessonType,
                    order: Convert.ToInt32(row.orden),
                    requiresCompletingPrevious: Convert.ToBoolean(row.requiere_completar_anterior));

                lesson.Update(
                    row.titulo?.ToString() ?? "Lesson",
                    lessonType,
                    row.contenido_texto?.ToString(),
                    row.video_url?.ToString(),
                    row.audio_url?.ToString(),
                    row.archivo_pdf?.ToString(),
                    row.duracion_minutos is not null ? (int?)Convert.ToInt32(row.duracion_minutos) : null,
                    Convert.ToInt32(row.orden),
                    Convert.ToBoolean(row.requiere_completar_anterior),
                    row.fecha_liberacion is not null
                        ? (DateTime?)Convert.ToDateTime(row.fecha_liberacion) : null);

                SetId(lesson, id);
                _target.Lessons.Add(lesson);
                count++;
            }
            catch (Exception ex) { errors.Add($"Lesson {row.id}: {ex.Message}"); }
        }
        await _target.SaveChangesAsync(ct);
        _logger.LogInformation("Migrated {Count} lessons", count);
        return count;
    }

    // ─── Learning Paths ──────────────────────────────��────────────────────────

    private async Task<int> MigrateLearningPathsAsync(
        NpgsqlConnection source, List<string> errors, CancellationToken ct)
    {
        _logger.LogInformation("Migrating learning paths...");
        var rows = await source.QueryAsync(
            "SELECT id, nombre, descripcion, imagen, estado, tipo_acceso, precio, orden_global " +
            "FROM rutas");

        var count = 0;
        foreach (var row in rows)
        {
            try
            {
                var id = Guid.Parse(row.id.ToString());
                var existing = await _target.LearningPaths.FindAsync(new object[] { id }, ct);
                if (existing is not null) continue;

                var path = new LearningPath(
                    name: row.nombre?.ToString() ?? "Path",
                    description: row.descripcion?.ToString(),
                    status: MapStatus(row.estado?.ToString()),
                    accessType: MapAccessType(row.tipo_acceso?.ToString()),
                    price: row.precio is not null ? (decimal?)Convert.ToDecimal(row.precio) : null,
                    globalOrder: Convert.ToInt32(row.orden_global ?? 0));

                if (row.imagen is not null)
                    path.Update(path.Name, path.Description, row.imagen.ToString(),
                        path.Status, path.AccessType, path.Price, path.GlobalOrder);

                SetId(path, id);
                _target.LearningPaths.Add(path);
                count++;
            }
            catch (Exception ex) { errors.Add($"LearningPath {row.id}: {ex.Message}"); }
        }
        await _target.SaveChangesAsync(ct);

        // rutas_cursos
        var junctionRows = await source.QueryAsync(
            "SELECT id, ruta_id, curso_id, orden, es_requisito FROM rutas_cursos");
        var jCount = 0;
        foreach (var row in junctionRows)
        {
            try
            {
                var id = Guid.Parse(row.id.ToString());
                var existing = await _target.LearningPathCourses.FindAsync(new object[] { id }, ct);
                if (existing is not null) continue;

                var lpc = new LearningPathCourse(
                    Guid.Parse(row.ruta_id.ToString()),
                    Guid.Parse(row.curso_id.ToString()),
                    Convert.ToInt32(row.orden),
                    Convert.ToBoolean(row.es_requisito));
                SetId(lpc, id);
                _target.LearningPathCourses.Add(lpc);
                jCount++;
            }
            catch (Exception ex) { errors.Add($"LearningPathCourse {row.id}: {ex.Message}"); }
        }
        await _target.SaveChangesAsync(ct);
        _logger.LogInformation("Migrated {Count} learning paths + {JCount} course links", count, jCount);
        return count;
    }

    // ─── Exams + Questions ───────────────────────────────────���────────────────

    private async Task<int> MigrateExamsAsync(
        NpgsqlConnection source, List<string> errors, CancellationToken ct)
    {
        var rows = await source.QueryAsync(
            "SELECT id, titulo, curso_id, capitulo_id, nota_minima, intentos_permitidos, " +
            "tiempo_limite_minutos, orden FROM examenes");
        var count = 0;
        foreach (var row in rows)
        {
            try
            {
                var id = Guid.Parse(row.id.ToString());
                if (await _target.Exams.FindAsync(new object[] { id }, ct) is not null) continue;

                var exam = new Exam(
                    title: row.titulo?.ToString() ?? "Exam",
                    courseId: row.curso_id is not null ? (Guid?)Guid.Parse(row.curso_id.ToString()) : null,
                    chapterId: row.capitulo_id is not null ? (Guid?)Guid.Parse(row.capitulo_id.ToString()) : null,
                    passingScore: Convert.ToDecimal(row.nota_minima ?? 70),
                    maxAttempts: Convert.ToInt32(row.intentos_permitidos ?? 0),
                    timeLimitMinutes: row.tiempo_limite_minutos is not null
                        ? (int?)Convert.ToInt32(row.tiempo_limite_minutos) : null,
                    order: Convert.ToInt32(row.orden ?? 0));

                SetId(exam, id);
                _target.Exams.Add(exam);
                count++;
            }
            catch (Exception ex) { errors.Add($"Exam {row.id}: {ex.Message}"); }
        }
        await _target.SaveChangesAsync(ct);
        _logger.LogInformation("Migrated {Count} exams", count);
        return count;
    }

    private async Task<int> MigrateQuestionsAsync(
        NpgsqlConnection source, List<string> errors, CancellationToken ct)
    {
        var rows = await source.QueryAsync(
            "SELECT id, examen_id, tipo, enunciado, opciones, respuesta_correcta, puntos, orden " +
            "FROM preguntas");
        var count = 0;
        foreach (var row in rows)
        {
            try
            {
                var id = Guid.Parse(row.id.ToString());
                if (await _target.Questions.FindAsync(new object[] { id }, ct) is not null) continue;

                var options = new List<string>();
                if (row.opciones is not null)
                {
                    var raw = row.opciones.ToString();
                    try { options = JsonSerializer.Deserialize<List<string>>(raw) ?? new List<string>(); }
                    catch { /* ignore malformed */ }
                }

                var question = new Question(
                    examId: Guid.Parse(row.examen_id.ToString()),
                    type: MapQuestionType(row.tipo?.ToString()),
                    text: row.enunciado?.ToString() ?? "",
                    options: options,
                    correctAnswer: row.respuesta_correcta?.ToString(),
                    points: Convert.ToDecimal(row.puntos ?? 1),
                    order: Convert.ToInt32(row.orden ?? 0));

                SetId(question, id);
                _target.Questions.Add(question);
                count++;
            }
            catch (Exception ex) { errors.Add($"Question {row.id}: {ex.Message}"); }
        }
        await _target.SaveChangesAsync(ct);
        _logger.LogInformation("Migrated {Count} questions", count);
        return count;
    }

    // ─── Enrollments, Progress, Answers, Grades, Notifications ───────────────

    private async Task<int> MigrateEnrollmentsAsync(
        NpgsqlConnection source, List<string> errors, CancellationToken ct)
    {
        var rows = await source.QueryAsync(
            "SELECT id, usuario_id, curso_id, estado, fecha_inscripcion, fecha_expiracion, notas " +
            "FROM inscripciones");
        var count = 0;
        foreach (var row in rows)
        {
            try
            {
                var id = Guid.Parse(row.id.ToString());
                if (await _target.Enrollments.FindAsync(new object[] { id }, ct) is not null) continue;

                var enrollment = new Enrollment(
                    userId: Guid.Parse(row.usuario_id.ToString()),
                    courseId: Guid.Parse(row.curso_id.ToString()),
                    status: MapEnrollmentStatus(row.estado?.ToString()),
                    expiresAt: row.fecha_expiracion is not null
                        ? (DateTime?)Convert.ToDateTime(row.fecha_expiracion) : null,
                    notes: row.notas?.ToString());
                SetId(enrollment, id);
                _target.Enrollments.Add(enrollment);
                count++;
            }
            catch (Exception ex) { errors.Add($"Enrollment {row.id}: {ex.Message}"); }
        }
        await _target.SaveChangesAsync(ct);
        _logger.LogInformation("Migrated {Count} enrollments", count);
        return count;
    }

    private async Task<int> MigrateProgressAsync(
        NpgsqlConnection source, List<string> errors, CancellationToken ct)
    {
        var rows = await source.QueryAsync(
            "SELECT id, usuario_id, leccion_id, estado, video_posicion, audio_posicion, " +
            "porcentaje_avance, completado_en FROM progreso_usuario");
        var count = 0;
        foreach (var row in rows)
        {
            try
            {
                var id = Guid.Parse(row.id.ToString());
                if (await _target.UserProgress.FindAsync(new object[] { id }, ct) is not null) continue;

                var progress = new UserProgress(
                    Guid.Parse(row.usuario_id.ToString()),
                    Guid.Parse(row.leccion_id.ToString()));

                if (row.estado?.ToString() == "completado")
                    progress.MarkCompleted();
                else
                    progress.UpdatePosition(
                        row.video_posicion is not null ? (int?)Convert.ToInt32(row.video_posicion) : null,
                        row.audio_posicion is not null ? (int?)Convert.ToInt32(row.audio_posicion) : null,
                        Convert.ToDecimal(row.porcentaje_avance ?? 0));

                SetId(progress, id);
                _target.UserProgress.Add(progress);
                count++;
            }
            catch (Exception ex) { errors.Add($"Progress {row.id}: {ex.Message}"); }
        }
        await _target.SaveChangesAsync(ct);
        _logger.LogInformation("Migrated {Count} progress records", count);
        return count;
    }

    private async Task<int> MigrateExamAnswersAsync(
        NpgsqlConnection source, List<string> errors, CancellationToken ct)
    {
        var rows = await source.QueryAsync(
            "SELECT id, usuario_id, examen_id, pregunta_id, respuesta, es_correcta, " +
            "puntos_obtenidos, intento_numero FROM respuestas_examen");
        var count = 0;
        foreach (var row in rows)
        {
            try
            {
                var id = Guid.Parse(row.id.ToString());
                if (await _target.ExamAnswers.FindAsync(new object[] { id }, ct) is not null) continue;

                var answer = new ExamAnswer(
                    userId: Guid.Parse(row.usuario_id.ToString()),
                    examId: Guid.Parse(row.examen_id.ToString()),
                    questionId: Guid.Parse(row.pregunta_id.ToString()),
                    answer: row.respuesta?.ToString(),
                    isCorrect: row.es_correcta is not null ? (bool?)Convert.ToBoolean(row.es_correcta) : null,
                    pointsEarned: Convert.ToDecimal(row.puntos_obtenidos ?? 0),
                    attemptNumber: Convert.ToInt32(row.intento_numero ?? 1));
                SetId(answer, id);
                _target.ExamAnswers.Add(answer);
                count++;
            }
            catch (Exception ex) { errors.Add($"ExamAnswer {row.id}: {ex.Message}"); }
        }
        await _target.SaveChangesAsync(ct);
        _logger.LogInformation("Migrated {Count} exam answers", count);
        return count;
    }

    private async Task<int> MigrateGradesAsync(
        NpgsqlConnection source, List<string> errors, CancellationToken ct)
    {
        var rows = await source.QueryAsync(
            "SELECT id, usuario_id, examen_id, intento_numero, nota_total, aprobado, estado, " +
            "feedback_maestro, maestro_id, calificado_en FROM calificaciones");
        var count = 0;
        foreach (var row in rows)
        {
            try
            {
                var id = Guid.Parse(row.id.ToString());
                if (await _target.Grades.FindAsync(new object[] { id }, ct) is not null) continue;

                var grade = new Grade(
                    userId: Guid.Parse(row.usuario_id.ToString()),
                    examId: Guid.Parse(row.examen_id.ToString()),
                    attemptNumber: Convert.ToInt32(row.intento_numero ?? 1),
                    totalScore: Convert.ToDecimal(row.nota_total ?? 0),
                    isPassed: Convert.ToBoolean(row.aprobado ?? false),
                    status: MapGradingStatus(row.estado?.ToString()));

                if (row.feedback_maestro is not null || row.maestro_id is not null)
                    grade.ApplyManualGrade(
                        Convert.ToDecimal(row.nota_total ?? 0),
                        Convert.ToBoolean(row.aprobado ?? false),
                        row.feedback_maestro?.ToString(),
                        row.maestro_id is not null ? Guid.Parse(row.maestro_id.ToString()) : Guid.Empty);

                SetId(grade, id);
                _target.Grades.Add(grade);
                count++;
            }
            catch (Exception ex) { errors.Add($"Grade {row.id}: {ex.Message}"); }
        }
        await _target.SaveChangesAsync(ct);
        _logger.LogInformation("Migrated {Count} grades", count);
        return count;
    }

    private async Task<int> MigrateNotificationsAsync(
        NpgsqlConnection source, List<string> errors, CancellationToken ct)
    {
        var rows = await source.QueryAsync(
            "SELECT id, usuario_id, tipo, titulo, mensaje, leida FROM notificaciones");
        var count = 0;
        foreach (var row in rows)
        {
            try
            {
                var id = Guid.Parse(row.id.ToString());
                if (await _target.Notifications.FindAsync(new object[] { id }, ct) is not null) continue;

                var notif = new Notification(
                    Guid.Parse(row.usuario_id.ToString()),
                    row.tipo?.ToString() ?? "info",
                    row.titulo?.ToString() ?? "Notification",
                    row.mensaje?.ToString() ?? "");

                if (Convert.ToBoolean(row.leida ?? false))
                    notif.MarkAsRead();

                SetId(notif, id);
                _target.Notifications.Add(notif);
                count++;
            }
            catch (Exception ex) { errors.Add($"Notification {row.id}: {ex.Message}"); }
        }
        await _target.SaveChangesAsync(ct);
        _logger.LogInformation("Migrated {Count} notifications", count);
        return count;
    }

    // ─── Helpers ───────────────────────────────────────────────────────────���──

    private static void SetId<T>(T entity, Guid id) where T : class
    {
        var prop = typeof(T).GetProperty("Id");
        prop?.SetValue(entity, id);
    }

    private static CourseStatus MapStatus(string? s) => s switch
    {
        "publicado" => CourseStatus.Published,
        "archivado" => CourseStatus.Archived,
        _ => CourseStatus.Draft
    };

    private static AccessType MapAccessType(string? s) => s switch
    {
        "pago" => AccessType.Paid,
        "membresia" => AccessType.Membership,
        _ => AccessType.Free
    };

    private static LessonType MapLessonType(string? s) => s switch
    {
        "audio" => LessonType.Audio,
        "texto" => LessonType.Text,
        "pdf" => LessonType.Pdf,
        "mixto" => LessonType.Mixed,
        _ => LessonType.Video
    };

    private static QuestionType MapQuestionType(string? s) => s switch
    {
        "verdadero_falso" => QuestionType.TrueFalse,
        "respuesta_corta" => QuestionType.ShortAnswer,
        "ensayo" => QuestionType.Essay,
        _ => QuestionType.MultipleChoice
    };

    private static EnrollmentStatus MapEnrollmentStatus(string? s) => s switch
    {
        "activa" => EnrollmentStatus.Active,
        "expirada" => EnrollmentStatus.Expired,
        "cancelada" => EnrollmentStatus.Cancelled,
        _ => EnrollmentStatus.Pending
    };

    private static GradingStatus MapGradingStatus(string? s) => s switch
    {
        "auto" => GradingStatus.Auto,
        "maestro" => GradingStatus.Manual,
        _ => GradingStatus.Pending
    };
}
