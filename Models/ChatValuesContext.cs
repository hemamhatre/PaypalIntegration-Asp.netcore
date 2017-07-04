using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatValues.Web.Models
{
    public class ChatValuesContext : DbContext
    {
        public ChatValuesContext(DbContextOptions<ChatValuesContext> options) : base(options) { }

        public DbSet<JobCard> JobCards { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<Voucher> Vouchers { get; set; }

        public DbSet<VoucherRedeem> VoucerRedeems { get; set; }

        public DbSet<Subscription> Subscriptions { get; set; }

        public DbSet<Answers> AnswersDbSet { get; set; }

        public DbSet<Question> Questions { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Session> Sessions { get; set; }

        public DbSet<SCQMapping> SCQMappings { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<UserSubscription> UserSubscriptions { get; set; }

        public DbSet<UserSCQMapping> UserSCQMappings { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<UserSession> UserSessions { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<JobCardDeals> JobCardDeals { get; set; } 

        public DbSet<Favourite> Favourites { get; set; }

        public DbSet<BlockedUser> BlockedUsers { get; set; }

        public DbSet<Recommend> Recommendations { get; set; }

        public DbSet<Answer> Answers { get; set; }

        public DbSet<Option> Options { get; set; }

        public DbSet<OnlineUser> OnlineUsers { get; set; }

        public DbSet<AudioMessage> AudioMessages { get; set; }

        public DbSet<JobCardViews> JobCardViews { get; set; }

        public DbSet<UserActivation> UserActivation { get; set; }

        public DbSet<GrantedVouchers> GrantVouchers { get; set; }

        public DbSet<GrantedVouchers> GrantedVouchers { get; set; }

        public DbSet<Transaction> Transaction { get; set; }


    }
}
