using System;
using System.Collections.Generic;
using System.Linq;
using MemeStation.Database.Enums;
using Microsoft.EntityFrameworkCore;

namespace MemeStation.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Meme> Memes { get; set; }
        public DbSet<MemeNews> News { get; set; }
        public DbSet<MemeReaction> Reactions { get; set; }
        public DbSet<Invite> Invites { get; set; }
        public DbSet<TransferOwnership> TransferOwnerships { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("user");
            modelBuilder.Entity<Meme>().ToTable("meme");
            modelBuilder.Entity<Invite>().ToTable("invite");
            modelBuilder.Entity<MemeNews>().ToTable("meme_news");
            modelBuilder.Entity<MemeReaction>().ToTable("reaction");
            modelBuilder.Entity<TransferOwnership>().ToTable("transfer_ownership");

            modelBuilder.Entity<MemeReaction>().HasOne(x => x.Shared).WithOne(s => s.Reaction)
                .HasForeignKey<Shared>(k => k.MemeReactionId);

            modelBuilder.Entity<Invite>().HasMany(x => x.Referrals).WithOne(x => x.Invite)
                .HasForeignKey(k => k.ReferralCode).IsRequired();


            modelBuilder.Entity<Meme>(meme =>
            {
                meme.HasOne(m => m.Creator).WithMany(u => u.CreatedMemes).HasForeignKey(m => m.CreatorAddress)
                    .IsRequired();
                meme.HasMany(m => m.TransferOwnerships).WithOne(t => t.Meme);
            });
            modelBuilder.Entity<User>(user =>
            {
                user.HasMany(u => u.CreatedMemes).WithOne(x => x.Creator).HasForeignKey(x => x.CreatorAddress)
                    .IsRequired();
                user.HasMany(u => u.TransferOwnerships).WithOne(x => x.Recipient);
                //user.HasMany(u => u.OwnedMemes).WithMany(m => m.Owners);
                //user.HasMany(t => t.TransferOwnerships).WithOne(u => u.Recipient);
            });

            modelBuilder.Entity<TransferOwnership>(x =>
            {
                x.HasKey(t => t.Id);
                x.HasOne(t => t.Recipient).WithMany(u => u.TransferOwnerships).HasForeignKey(k => k.RecipientId).IsRequired();
                x.HasOne(t => t.Meme).WithMany(u => u.TransferOwnerships).HasForeignKey(k => k.MemeId).IsRequired();
            });
        }
    }

    public static class DbInitializer
    {
        public static void Initialize(DatabaseContext ctx)
        {
            bool addedStuff = false;

            if (!ctx.Users.Any())
            {
                var users = new User[]
                {
                    new User()
                    {
                        // Id = "L2gwSai3",
                        Auth0Id = "Auth0Id1",
                        UserName = "PepeLorDTradEr - J1",
                        Picture = "/images/1344 - PePeLordTrader.jpg",
                        Address = "address1",
                        WalletDescription =
                            "Most of you must know me from previous threads, or simply by my legendary reputation. I offer accurate, worldwide recognized evaluation of your Rare PePes. (I can also give authenticity certificates) I am well known here and my worldwide reputation as the best PePe trader in the century and for those to come speaks for itself, which is why you probably already known who I am. Sincerely yours, PepeLorDTradEr-J1",
                        CreatedMemes = new List<Meme>(),
                        OwnedMemes = new List<Meme>()
                    },
                    new User()
                    {
                        // Id = "8ol7kC6i",
                        Auth0Id = "Auth0Id2",
                        UserName = "Last2stand",
                        Picture = "/images/Last2stand.jpg",
                        Address = "0xEa33Bc82BA1188f54f276606fbFa55E48968b861",
                        WalletDescription = "I am passionate about cryptocurrencies, PePe and Doge !"
                    }
                };

                ctx.AddRange(users);
                ctx.SaveChanges();
                addedStuff = true;
            }

            if (!ctx.Memes.Any())
            {
                var memes = new Meme[]
                {
                    new Meme()
                    {
                        Id = "60cbcc8134032462377ed910",
                        FileJsonUrl = "/images/PePes/Watermarked/0903 - JutsuPePe.png",
                        Price = 12000,
                        Title = "PePe and the Imperial Han",
                        Description =
                            "This PePe is estimated to have risen during the second imperial dynasty of China (Han Dynasty;202BC-220AD), which adds a very distinct cachet to the PePe, making it very rare and expensive. Legend says it was one of the most prized possesion of the rebel leader Liu Bang.",
                        ContractAddress = "0xb932a70a57673d89f4acffbe830e8ed7f75fb9e0",
                        TokenId = "11601",
                        CreatorAddress = "address1",
                        CreationTime = new DateTime(2021, 06, 01, 12, 0, 0, 0),
                        Type = FilteType.Png,
                        Origin = "4CHAN",
                        MemeType = "Imperial Han",
                        //FileSize = randomGen.Next(125, 4000),
                        MaxInstance = 3,
                    },
                    new Meme()
                    {
                        Id = "60cbe0147c7bcafb061c92a2",
                        FileJsonUrl = "/images/PePes/Watermarked/1313%20-%20MisPrintPePe.jpg",
                        Price = 18000,
                        Title = "PePe | \"The Pair\"",
                        Description =
                            "This PePe, alongside its black and white misprint edition, are renown as the two most rare and expensive PePe that have ever existed. They are only sold as a pair, for they are both a misprint (the only two that ever happenend) of the same source (creator unknown). This PePe is a perfect balance of pleasing aesthetics and an aggressive, yet absorbing use of abstract colors. \"The Pair\" was stolen from the Louvre museum on the 21st of August, 1911, at the same time as the Mona Lisa by Vincenzo Peruggi. It has then traveled and only been found and returned to the Louvre's in 1914 (January 4th). \"The Pair\" is a 1/1 astronomically rare PePe and the current owner is one of the most renown PePe Collectionneur in the entire world: PepeLorDTradEr-J1.",
                        ContractAddress = "0xb932a70a57673d89f4acffbe830e8ed7f75fb9e0",
                        TokenId = "11602",
                        CreatorAddress = "address1",
                        CreationTime = new DateTime(2021, 06, 01, 12, 0, 0, 0),
                        Type = FilteType.Png,
                        Origin = "4CHAN",
                        MemeType = "The Pair",
                        //FileSize = randomGen.Next(125, 4000),
                        MaxInstance = 1,
                    },
                    new Meme()
                    {
                        Id = "60ca031c31393851cdc4bbfa",
                        FileJsonUrl = "/images/PePes/Watermarked/1314%20-%20SketchPePe.png",
                        Price = 1500,
                        Title = "The SketchPePe",
                        Description =
                            "The author of this PePe is unknown, but after thorough investigation by the FBPI (Federal Bureau of PePe Investigation), it was authorized to be traded as a Semi Rare PePe on the markets. The sketch was made and first used (2015) as a form of payment in a French caf�, which is a an hommage to Pablo Picasso's napkin.",
                        ContractAddress = "0xb932a70a57673d89f4acffbe830e8ed7f75fb9e0",
                        TokenId = "11603",
                        CreatorAddress = "address1",
                        CreationTime = new DateTime(2021, 06, 01, 12, 0, 0, 0),
                        Type = FilteType.Png,
                        Origin = "4CHAN",
                        MemeType = "Sketch",
                        //FileSize = randomGen.Next(125, 4000),
                        MaxInstance = 10,
                    },
                    new Meme()
                    {
                        Id = "60ca031c31393851cdc4bbfb",
                        FileJsonUrl = "/images/PePes/Watermarked/0424%20-%20MartianPePe.png",
                        Price = 500,
                        Title = "The VikingPePe",
                        Description =
                            "This PePe is dated to the Middle Ages, its production is estimated between 793-1066 AD (Viking Age). It represents J�rmungandr, the Midgard Serpent. This PePe is very sought after by Viking descendants or colletionneurs thathave a peculiar interest in the Norse Mythology. Being an original, it tremendously increases the value of this Rare PePe.",
                        ContractAddress = "0xb932a70a57673d89f4acffbe830e8ed7f75fb9e0",
                        TokenId = "11604",
                        CreatorAddress = "0xEa33Bc82BA1188f54f276606fbFa55E48968b861",
                        CreationTime = new DateTime(2021, 06, 01, 12, 0, 0, 0),
                        Type = FilteType.Png,
                        Origin = "4CHAN",
                        MemeType = "Viking",
                        //FileSize = randomGen.Next(125, 4000),
                        MaxInstance = 25,
                    }
                };
                ctx.AddRange(memes);
                ctx.SaveChanges();

                var news = new MemeNews[]
                {
                    new MemeNews()
                    {
                        NewsId = "60f5f33f4545381bf6933a7c",
                        MemeId = "60cbcc8134032462377ed910",
                        Date = new DateTime(2021, 03, 18),
                        SubTitle = "China History",
                        Title = "The Han Dynasty - Longest Imperial Dynasty",
                        Url = "https://www.chinahighlights.com/travelguide/china-history/the-han-dynasty.htm"
                    }
                };

                ctx.AddRange(news);
                ctx.SaveChanges();
            }
        }
    }
}