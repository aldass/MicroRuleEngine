﻿using EFModeling.Samples.DataSeeding;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroRuleEngine.Core.Tests
{
    [TestClass]
    public class ExpressionToSqlQueryTest
    {
        internal DbContextOptions<BloggingContext> GetDbOptions()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder<BloggingContext>()
                   .UseSqlite(connection)
                   .Options;

            // Create the schema in the database
            using (var context = new BloggingContext(options))
            {
                context.Database.EnsureCreated();
            }
            return options;
        }
        [TestMethod]
        public void BasicEqualityExpression()
        {
           
            using (var context = new BloggingContext(GetDbOptions()))
            {
                context.Blogs.Add(new Blog { Url = "http://test.com" });
                context.SaveChanges();

                var testBlog = context.Blogs.FirstOrDefault(b => b.Url == "http://test.com");

                var fields = Mre.Member.GetFields(typeof(Blog));
                Rule rule = new Rule
                {
                    MemberName = "Url",
                    Operator = MreOperator.Equal.ToString("g"),
                    TargetValue = "http://test.com"
                };

                var blog2 = context.Blogs.Where(Mre.ToExpression<Blog>(rule, false)).FirstOrDefault();

                Assert.IsTrue(testBlog.BlogId == blog2.BlogId);

            }
        }
    }
}
