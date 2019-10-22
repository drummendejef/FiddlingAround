using System;
using System.Linq;

namespace EFGetStarted
{
    class Program
    {
        static void Main(string[] args)
        {
            //Gekozen voor EF Core
            // https://docs.microsoft.com/en-us/ef/efcore-and-ef6/index#guidance-for-new-applications

            //Startpagina EF: https://docs.microsoft.com/en-us/ef/

            //EF Core: https://docs.microsoft.com/en-us/ef/core/


            using (var db = new BloggingContext())
            {
                // Create
                Console.WriteLine("Inserting a new blog");
                db.Add(new Blog { Url = "http://blogs.msdn.com/adonet" });
                db.SaveChanges();

                // Read
                Console.WriteLine("Querying for a blog");
                var blog = db.Blogs
                    .OrderBy(b => b.BlogId)
                    .First();

                // Update
                Console.WriteLine("Updating the blog and adding a post");
                blog.Url = "https://devblogs.microsoft.com/dotnet";
                blog.Posts.Add(
                    new Post
                    {
                        Title = "Hello World",
                        Content = "I wrote an app using EF Core!"
                    });
                db.SaveChanges();

                // Delete
                Console.WriteLine("Delete the blog");
                db.Remove(blog);
                db.SaveChanges();

                // Add own Test item
                db.Add(new Test1 { Names = "Naam 1" });
                db.SaveChanges();
            }
        }
    }
}
