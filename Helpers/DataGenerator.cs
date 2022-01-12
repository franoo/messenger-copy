using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Helpers
{
    public class DataGenerator
    {

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MyDBContext(
                serviceProvider.GetRequiredService<DbContextOptions<MyDBContext>>()))
            {
                if (context.Users.Any())
                {
                    return;
                }
                context.Users.AddRange(
                    new User
                    {
                        id = 1,
                        firstname = "Niuniek",
                        lastname = "Teju",
                        username = "niuniek",
                        PasswordHash = "2137"
                        
                    },
                    new User
                    {
                        id = 2,
                        firstname = "Mysia",
                        lastname = "Szynszyla",
                        username = "mysia",
                        PasswordHash = "2137"
                    },
                    new User
                    {
                        id = 3,
                        firstname = "Ludwik",
                        lastname = "Pies",
                        username = "ludwik",
                        PasswordHash = "2137"
                    }
                    );
                context.Messages.AddRange(new Message[]
                {
                    new Message
                    {
                        Id=1,
                        SenderId=1,
                        ReceiverId=2,
                        MessageContent="Witaj Mysiu",
                        Date = DateTime.Now.AddDays(-1),
                        ConversationId=1
                    },
                    new Message
                    {
                        Id=2,
                        SenderId=2,
                        ReceiverId=1,
                        MessageContent="Witaj Niunku",
                        Date = DateTime.Now,
                        ConversationId=1
                    },
                    new Message
                    {
                        Id=3,
                        SenderId=3,
                        ReceiverId=1,
                        MessageContent="Witaj Niunku, tu Ludwik",
                        Date = DateTime.Now.AddHours(5),
                        ConversationId=2
                    }
                });
                //context.Conversations.AddRange(new Conversation[]
                //{
                //    new Conversation
                //    {
                //        Id=1
                //    },
                //    new Conversation
                //    {
                //        Id=2
                //    }
                //});

                context.SaveChanges();

                

            }
        }
    }
}
