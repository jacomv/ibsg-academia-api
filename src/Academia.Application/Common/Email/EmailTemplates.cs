namespace Academia.Application.Common.Email;

/// <summary>HTML email templates for all platform notifications.</summary>
public static class EmailTemplates
{
    private const string BaseUrl = "https://academia.ibsg.app";

    private static string Wrap(string title, string content)
    {
        return "<html><head><meta charset='UTF-8'><title>" + title + "</title></head>" +
               "<body style='font-family:-apple-system,sans-serif;background:#f4f4f5;margin:0;padding:20px'>" +
               "<div style='max-width:560px;margin:0 auto;background:#fff;border-radius:12px;overflow:hidden;box-shadow:0 2px 8px rgba(0,0,0,.08)'>" +
               "<div style='background:linear-gradient(135deg,#1e40af,#3b82f6);padding:32px 40px;text-align:center'>" +
               "<h1 style='color:#fff;margin:0;font-size:22px'>✝ IBSG Academia</h1>" +
               "<p style='color:#bfdbfe;margin:8px 0 0;font-size:14px'>Plataforma de formación cristiana</p>" +
               "</div>" +
               "<div style='padding:32px 40px;color:#374151'>" + content + "</div>" +
               "<div style='padding:20px 40px;text-align:center;font-size:12px;color:#9ca3af;border-top:1px solid #f3f4f6'>" +
               "<p>IBSG Academia &middot; <a href='" + BaseUrl + "' style='color:#6b7280'>" + BaseUrl + "</a></p>" +
               "<p>Si no solicitaste este email, puedes ignorarlo.</p>" +
               "</div></div></body></html>";
    }

    private static string Btn(string href, string text) =>
        "<a href='" + href + "' style='display:inline-block;background:#2563eb;color:#fff;text-decoration:none;" +
        "padding:12px 28px;border-radius:8px;font-weight:600;font-size:15px;margin:16px 0'>" + text + "</a>";

    private static string Highlight(string html) =>
        "<div style='background:#eff6ff;border-left:4px solid #3b82f6;padding:12px 16px;border-radius:0 8px 8px 0;margin:16px 0'>" +
        html + "</div>";

    public static string Welcome(string firstName, string email) => Wrap(
        "Bienvenido a IBSG Academia",
        "<h2 style='margin-top:0;color:#111827'>¡Bienvenido, " + firstName + "! 🙏</h2>" +
        "<p>Tu cuenta ha sido creada exitosamente en IBSG Academia.</p>" +
        Highlight("<strong>Tu correo de acceso:</strong> " + email) +
        "<p>Ya puedes ingresar a la plataforma y comenzar tu camino de formación espiritual.</p>" +
        Btn(BaseUrl + "/login", "Ingresar a la plataforma"));

    public static string PasswordReset(string firstName, string resetLink) => Wrap(
        "Recuperar contraseña",
        "<h2 style='margin-top:0;color:#111827'>Recuperar contraseña</h2>" +
        "<p>Hola " + firstName + ", recibimos una solicitud para restablecer la contraseña de tu cuenta.</p>" +
        "<p>Haz clic para crear una nueva contraseña. <strong>Este enlace expira en 1 hora.</strong></p>" +
        Btn(resetLink, "Restablecer contraseña") +
        Highlight("<strong>¿No solicitaste esto?</strong><br>Si no pediste restablecer tu contraseña, ignora este mensaje.") +
        "<p style='font-size:12px;color:#9ca3af'>Si el botón no funciona, copia este enlace:<br>" + resetLink + "</p>");

    public static string ExamPassed(string firstName, string examTitle, decimal score) => Wrap(
        "¡Aprobaste el examen! — " + examTitle,
        "<h2 style='margin-top:0;color:#111827'>🎉 ¡Felicidades, " + firstName + "!</h2>" +
        "<p>Has aprobado el examen <strong>\"" + examTitle + "\"</strong> con una calificación de:</p>" +
        Highlight("<strong style='font-size:24px;color:#16a34a'>" + score + "%</strong>") +
        "<p>¡Sigue adelante con tu formación. Cada paso cuenta!</p>" +
        Btn(BaseUrl + "/estudiante", "Ver mis cursos"));

    public static string EnrollmentActivated(string firstName, string courseTitle) => Wrap(
        "Inscripción activada — " + courseTitle,
        "<h2 style='margin-top:0;color:#111827'>Tu inscripción está activa ✅</h2>" +
        "<p>Hola " + firstName + ", tu inscripción en el curso <strong>\"" + courseTitle + "\"</strong> ha sido activada.</p>" +
        "<p>Ya puedes acceder a todo el contenido del curso.</p>" +
        Btn(BaseUrl + "/estudiante/cursos", "Ir al curso"));

    public static string CourseCompleted(string firstName, string courseTitle) => Wrap(
        "¡Completaste el curso! — " + courseTitle,
        "<h2 style='margin-top:0;color:#111827'>🏆 ¡Curso completado!</h2>" +
        "<p>Hola " + firstName + ", has completado el curso <strong>\"" + courseTitle + "\"</strong>.</p>" +
        "<p>Tu certificado de completado está disponible para descargar.</p>" +
        Btn(BaseUrl + "/estudiante/certificados", "Descargar mi certificado") +
        "<p>¡Gracias por tu dedicación a la formación espiritual!</p>");

    public static string Certificate(string firstName, string courseTitle, string certificateNumber) => Wrap(
        "Tu certificado — " + courseTitle,
        "<h2 style='margin-top:0;color:#111827'>Certificado de completado 🎓</h2>" +
        "<p>Hola " + firstName + ", adjunto encontrarás tu certificado por completar el curso <strong>\"" + courseTitle + "\"</strong>.</p>" +
        Highlight("<strong>Número de certificado:</strong> " + certificateNumber) +
        "<p>También puedes descargarlo desde la plataforma en cualquier momento.</p>" +
        Btn(BaseUrl + "/estudiante/certificados", "Ver mis certificados"));

    public static string ExamGraded(string firstName, string examTitle, decimal score, bool isPassed) => Wrap(
        "Tu examen fue calificado — " + examTitle,
        "<h2 style='margin-top:0;color:#111827'>Tu examen fue revisado</h2>" +
        "<p>Hola " + firstName + ", un maestro ha revisado tu examen <strong>\"" + examTitle + "\"</strong>.</p>" +
        Highlight("<strong>Calificación final: " + score + "%</strong><br>Resultado: " + (isPassed ? "✅ Aprobado" : "❌ No aprobado")) +
        Btn(BaseUrl + "/estudiante", "Ver resultado completo"));
}
