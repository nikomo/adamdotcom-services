﻿using System;
using AdamDotCom.OpenSource.Service;
using NUnit.Framework;

namespace Unit.Tests
{
    [TestFixture]
    public class ProjectsExtensionsTests
    {
        [Test]
        public void VerifyFilterEmptyRepositories()
        {
            var projects = new Projects
                               {
                                   new Project { Name = "adamdotcom-services", LastMessage = null, LastModified = null, Url = "project1" },
                                   new Project { Name = "services", LastMessage = null, LastModified = null, Url = "project2" },
                                   new Project { Name = "Project3", LastMessage = "committed", LastModified = DateTime.Now.AddDays(-2).ToString(), Url = "project3" }
                               };

            var results = projects.FilterEmptyRepositories();
            Assert.AreEqual(1, results.Count);
            Assert.IsTrue(results[0].Name == "Project3");
        }

        [Test]
        public void VerifyFilterDuplicateProjectsByLastModified()
        {
            var projects = new Projects
                               {
                                   new Project { Name = "Project1", LastModified = DateTime.Now.ToString(), Url = "project1-url" },
                                   new Project { Name = "Project1", LastModified = DateTime.Now.AddHours(-2).ToString(), Url = "project2" }
                               };

            var results = projects.FilterDuplicateProjectsByLastModified();
            Assert.AreEqual(1, results.Count);
            Assert.IsTrue(results[0].Url == "project1-url");
        }

        [Test]
        public void VerifyFilters()
        {
            var projects = new Projects
                               {
                                   new Project { Name = "adamdotcom-services", LastModified = DateTime.Now.AddHours(-1).ToString(), Url = "project1" },
                                   new Project { Name = "services", LastModified = DateTime.Now.ToString(), LastMessage = "commit", Url = "project2" },
                                   new Project { Name = "-services-", LastModified = DateTime.Now.AddHours(-2).ToString(), Url = "project3" },
                                   new Project { Name = "Empty-Repository", LastModified = null, LastMessage = null, Url = "project4" }
                               };

            foreach (var project in projects.Filter("remove:adamdotcom"))
            {
                Assert.IsTrue(project.Name.IndexOf("adamdotcom") == -1);
            }

            foreach (var project in projects.Filter("remove:-"))
            {
                Assert.IsTrue(project.Name.IndexOf("-") == -1);
                Assert.IsFalse(project.Name == "adamdotcomservices");
                Assert.IsFalse(project.Name == " services ");
            }

            var result = projects.Filter("remove:duplicate-items");
            
            Console.WriteLine("");
            foreach (var item in result)
            {
                Console.WriteLine(item.Name + " " + item.LastModified);
            }
            
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(result[1].LastModified, projects[1].LastModified);
            Assert.AreEqual(result[1].Url, projects[1].Url);

            result = projects.Filter("remove:empty-items");
            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result[1].LastModified == projects[1].LastModified);
            Assert.IsTrue(result[1].Url == projects[1].Url);
        }

        [Test]
        public void VerifyNewFilterGrammar()
        {
            var projects = new Projects
                               {
                                   new Project { Name = "adamdotcom-services", LastModified = DateTime.Now.AddHours(-1).ToString(), Url = "project1" },
                                   new Project { Name = "services", LastModified = DateTime.Now.ToString(), LastMessage = "commit", Url = "project2" },
                                   new Project { Name = "-services-", LastModified = DateTime.Now.AddHours(-2).ToString(), Url = "project3" },
                                   new Project { Name = "WebSite", LastModified = DateTime.Now.AddHours(-10).ToString(), LastMessage = "Updated fizzle my jizzle", Url = "project4" },
                                   new Project { Name = "new repo", LastModified = null, LastMessage = null, Url = "project5" }
                               };
            var result = projects.Filter("remove:adamdotcom,remove:-,remove:duplicate-items,remove:empty-items");
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result[0].Url == "project2");
            Assert.IsTrue(result[1].Url == "project4");

            result = projects.Filter("remove:adamdotcom,-,duplicate-items,empty-items");
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result[0].Url == "project2");
            Assert.IsTrue(result[1].Url == "project4");
        }
    }
}