using Academia.Application.Common.Exceptions;
using Academia.Application.Common.Interfaces;
using Academia.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academia.Application.Notes.Commands.DeleteNote;

public class DeleteNoteCommandHandler : IRequestHandler<DeleteNoteCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public DeleteNoteCommandHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteNoteCommand request, CancellationToken cancellationToken)
    {
        var note = await _context.StudentNotes
            .FirstOrDefaultAsync(n => n.Id == request.NoteId && n.UserId == _currentUser.Id,
                cancellationToken);

        if (note is null)
            throw new NotFoundException("Note", request.NoteId);

        _context.StudentNotes.Remove(note);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
