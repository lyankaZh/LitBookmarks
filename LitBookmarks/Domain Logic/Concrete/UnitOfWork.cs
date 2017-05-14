using System;
using Domain_Logic.Abstract;
using Domain_Logic.Entities;

namespace Domain_Logic.Concrete
{
    public class UnitOfWork:IUnitOfWork
    {
        private readonly BookmarkDbContext _context = new BookmarkDbContext();
        private Repository<Bookmark> _bookmarkRepository;
        private Repository<User> _userRepository;
        private Repository<Comment> _commentRepository;
        //private Repository<Like> _likeRepository;
        private Repository<Genre> _genreRepository;

        public Repository<Bookmark> BookmarkRepository => _bookmarkRepository ?? (_bookmarkRepository = new Repository<Bookmark>(_context));
        public Repository<User> UserRepository => _userRepository ?? (_userRepository = new Repository<User>(_context));
        public Repository<Comment> CommentRepository => _commentRepository ?? (_commentRepository = new Repository<Comment>(_context));
        //public Repository<Like> LikeRepository => _likeRepository ?? (_likeRepository = new Repository<Like>(_context));
        public Repository<Genre> GenreRepository => _genreRepository ?? (_genreRepository = new Repository<Genre>(_context));
        public void Save()
        {
            _context.SaveChanges();
        }

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
