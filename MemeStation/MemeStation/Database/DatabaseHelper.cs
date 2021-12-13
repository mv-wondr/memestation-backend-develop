using System;
using System.Collections.Generic;
using System.Linq;
using MemeStation.Core;
using MemeStation.Models.Meme;
using MemeStation.Models.News;
using MemeStation.Models.Reaction;
using MemeStation.ResponseBuilder;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace MemeStation.Database
{
    public interface IDatabaseHelper
    {
        IEnumerable<MemeResponse> GetAllMemes(Func<DbSet<Meme>, IQueryable<Meme>> f = null);
        IEnumerable<UserResponse> GetAllUsers(Func<DbSet<User>, IQueryable<User>> f = null);

        IEnumerable<NewsResponse> GetAllNews(Func<DbSet<MemeNews>, IQueryable<MemeNews>> f = null);

        IEnumerable<ReactionResponse> GetAllReactions(Func<IEnumerable<MemeReaction>, IEnumerable<MemeReaction>> f = null);
    }

    public class DatabaseHelper : IDatabaseHelper
    {
        private readonly DatabaseContext _ctx;

        public DatabaseHelper (DatabaseContext ctx)
        {
            _ctx = ctx;
        }

        public IEnumerable<MemeResponse> GetAllMemes(Func<DbSet<Meme>, IQueryable<Meme>> f = null)
        {
            var memes = _ctx.Memes;
            var filteredMemes = f == null ? memes : f(memes);

            return filteredMemes.Select(m => new MemeResponse
            {
                Id = m.Id,
                Creator = new UserResponse
                {
                    UserName = m.Creator.UserName, WalletDescription = m.Creator.WalletDescription,
                    Picture = m.Creator.Picture, Address = m.Creator.Address, Authenticated = m.Creator.Authenticated
                },
                ContractAddress = m.ContractAddress,
                TokenId = m.TokenId,
                Title = m.Title,
                Subtitle = m.Subtitle,
                Description = m.Description,
                FileUrl = m.FileJsonUrl,
                Price = m.Price,
                TypeId = m.Type,
                CreationTime = m.CreationTime,
                Origin = m.Origin,
                MaxInstance = m.MaxInstance,
            });
        }

        public IEnumerable<UserResponse> GetAllUsers(Func<DbSet<User>, IQueryable<User>> f = null)
        {
            var users = _ctx.Users;
            var filteredUsers = f == null ? users : f(users);

            return filteredUsers.Select(u => new UserResponse()
            {
                Address = u.Address,
                Auth0Id = u.Auth0Id,
                UserName = u.UserName,
                Picture = u.Picture,
                WalletDescription = u.WalletDescription,
                Authenticated = u.Authenticated,
                OwnedMemes = u.TransferOwnerships.Where(x => x.Active).Select(m =>  new MemeResponse()
                    {
                        Id = m.Meme.Id,
                        Creator = new UserResponse
                        {
                            UserName = m.Meme.Creator.UserName, WalletDescription = m.Meme.Creator.WalletDescription,
                            Picture = m.Meme.Creator.Picture, Address = m.Meme.Creator.Address
                        },
                        ContractAddress = m.Meme.ContractAddress,
                        TokenId = m.Meme.TokenId,
                        Title = m.Meme.Title,
                        Subtitle = m.Meme.Subtitle,
                        Description = m.Meme.Description,
                        FileUrl = m.Meme.FileJsonUrl,
                        Price = m.Meme.Price,
                        TypeId = m.Meme.Type,
                        CreationTime = m.Meme.CreationTime,
                        Origin = m.Meme.Origin,
                        MaxInstance = m.Meme.MaxInstance,
                    }).ToList(),
                CreatedMemes = u.CreatedMemes.Select(m => new MemeResponse()
                {
                    Id = m.Id,
                    Creator = new UserResponse
                    {
                        UserName = m.Creator.UserName, WalletDescription = m.Creator.WalletDescription,
                        Picture = m.Creator.Picture, Address = m.Creator.Address
                    },
                    ContractAddress = m.ContractAddress,
                    TokenId = m.TokenId,
                    Title = m.Title,
                    Subtitle = m.Subtitle,
                    Description = m.Description,
                    FileUrl = m.FileJsonUrl,
                    Price = m.Price,
                    TypeId = m.Type,
                    CreationTime = m.CreationTime,
                    Origin = m.Origin,
                    MaxInstance = m.MaxInstance,
                }).ToList(),
                
            });
        }

        public IEnumerable<NewsResponse> GetAllNews(Func<DbSet<MemeNews>, IQueryable<MemeNews>> f = null)
        {
            var news = _ctx.News;
            var filteredNews = f == null ? news : f(news);
            return filteredNews.Select(x => new NewsResponse()
            {
                NewsId = x.NewsId,
                MemeId = x.MemeId,
                //TODO: change this to put memeRespone type to not have the creator field to null.
                Meme = x.Meme,
                SubTitle = x.SubTitle,
                Title = x.Title,
                Url = x.Url
            });
        }

        public IEnumerable<ReactionResponse> GetAllReactions(Func<IEnumerable<MemeReaction>, IEnumerable<MemeReaction>> f = null)
        {

            var reactions = _ctx.Reactions.Include(x => x.Comment).Include(i => i.Shared).ToList();
            var filteredReaction = f == null ? reactions : f(reactions);
            return filteredReaction.Select(r => new ReactionResponse()
            {
                ReactionId = r.ReactionId,
                OriginatorAddress = r.OriginatorAddress,
                ReactionType = r.ReactionType.ToString(),
                Date = r.Date,
                MemeId = r.MemeId,
                Comment = r.Comment?.Comment,
                Platform = r.Shared?.Platform.ToString()
            });
        }

    }
}
