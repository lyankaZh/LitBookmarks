using System;
using Domain_Logic.Concrete;
using Domain_Logic.Entities;

namespace Domain_Logic.Abstract
{
    public interface IUnitOfWork:IDisposable
    {
        Repository<Bookmark> BookmarkRepository { get; }
        Repository<User> UserRepository { get; }
        Repository<Comment> CommentRepository { get; }
        //Repository<Like> LikeRepository { get; }
        Repository<Genre> GenreRepository { get; }
        void Save();
    }
}
