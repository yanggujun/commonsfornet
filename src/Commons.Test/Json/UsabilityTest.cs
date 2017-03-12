// Copyright CommonsForNET.
// Licensed to the Apache Software Foundation (ASF) under one or more
// contributor license agreements. See the NOTICE file distributed with
// this work for additional information regarding copyright ownership.
// The ASF licenses this file to You under the Apache License, Version 2.0
// (the "License"); you may not use this file except in compliance with
// the License. You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Commons.Json;
using Commons.Json.Mapper;
using System;
using System.Linq;
using Xunit;
using System.Threading.Tasks;
using Commons.Utils;

namespace Commons.Test.Json
{
	public class UsabilityTest
    {
        [Fact]
        public void TestSmall01()
        {
            var small = SmallPost.Create(125234);
            var json = JsonMapper.ToJson(small);
            var newSmall = JsonMapper.To<SmallPost>(json);
            Assert.Equal(small.active, newSmall.active);
            Assert.Equal(small.created, newSmall.created);
            Assert.Equal(small.ID, newSmall.ID);
            Assert.Equal(small.title, newSmall.title);
            Assert.True(small.Equals(newSmall));
        }

        [Fact]
        public void TestSmall02()
        {
            var message = Message.Create(21255);
            var json = JsonMapper.ToJson(message);
            var newMsg = JsonMapper.To<Message>(json);
            Assert.Equal(message.message, newMsg.message);
            Assert.Equal(message.version, newMsg.version);
            Assert.True(message.Equals(newMsg));
        }

        [Fact]
        public void TestSmall03()
        {
            var complex = Complex.Create(1231);
            var json = JsonMapper.ToJson(complex);
            var newCmp = JsonMapper.To<Complex>(json);
            Assert.Equal(complex.x, newCmp.x);
            Assert.Equal(complex.y, newCmp.y);
            Assert.Equal(complex.z, newCmp.z);
        }

        [Fact]
        public void TestSmall04()
        {
            var context = JsonMapper.NewContext().For<CompletePrimitiveObject>(y =>
                    y.MapProperty(x => x.F1).With("K1")
                    .MapProperty(x => x.F2).With("K2")
                    .MapProperty(x => x.F3).With("K3")
                    .MapProperty(x => x.F4).With("K4")
                    .MapProperty(x => x.F5).With("K5")
                    .MapProperty(x => x.F6).With("K6")
                    .MapProperty(x => x.F7).With("K7")
                    .MapProperty(x => x.F8).With("K8")
                    .MapProperty(x => x.F9).With("K9")
                    .MapProperty(x => x.F10).With("K10")
                    .MapProperty(x => x.F11).With("K11")
                    .MapProperty(x => x.F12).With("K12")
                    .MapProperty(x => x.F13).With("K13")
                    .MapProperty(x => x.F14).With("K14")
                    .MapProperty(x => x.F15).With("K15")
                    .MapProperty(x => x.F16).With("K16")
                    .MapProperty(x => x.F17).With("K17"));

            var id = Guid.NewGuid();
            var cpo = new CompletePrimitiveObject
            {
                F1 = "booo",
                F2 = 20000,
                F3 = 3.09769,
                F4 = 100.2f,
                F5 = 100,
                F6 = true,
                F7 = 899999212312,
                F8 = 35,
                F9 = 80,
                F10 = 347312,
                F11 = 98876868687897876,
                F12 = 300,
                F13 = 9.4134123175123m,
                F14 = 't',
                F15 = new DateTime(1995, 10, 21),
                F16 = new Simple
                {
                    FieldA = "F1",
                    FieldB = 3412,
                    FieldC = 60098.1234124,
                    FieldD = false,
                },
                F17 = id
            };

            var json = context.ToJson(cpo);

            var dj = JsonMapper.Parse(json);
            Assert.Equal("booo", (string)dj.K1);
            Assert.Equal(20000, (int)dj.K2);
            Assert.Equal(3.09769, (double)dj.K3);
            Assert.Equal(100.2f, (float)dj.K4);
            Assert.Equal(100, (short)dj.K5);
            Assert.True((bool)dj.K6);
            Assert.Equal(899999212312, (long)dj.K7);
            Assert.Equal(35, (byte)dj.K8);
            Assert.Equal(80, (sbyte)dj.K9);
            Assert.Equal((uint)347312, (uint)dj.K10);
            Assert.Equal((ulong)98876868687897876, (ulong)dj.K11);
            Assert.Equal(300, (ushort)dj.K12);
            Assert.Equal(9.4134123175123m, (decimal)dj.K13);
            Assert.Equal("t", (string)dj.K14);
            Assert.True(TestHelper.DateTimeEqual(new DateTime(1995, 10, 21), DateTime.Parse((string)dj.K15)));
            Assert.Equal("F1", (string)dj.K16.FieldA);
            Assert.Equal(3412, (int)dj.K16.FieldB);
            Assert.Equal(60098.1234124, (double)dj.K16.FieldC);
            Assert.False((bool)dj.K16.FieldD);
            Assert.Equal(id, Guid.Parse((string)dj.K17));

            var newCpo = context.To<CompletePrimitiveObject>(json);
            Assert.Equal("booo", newCpo.F1);
            Assert.Equal(20000, newCpo.F2);
            Assert.Equal(3.09769, newCpo.F3);
            Assert.Equal(100.2f, newCpo.F4);
            Assert.Equal(100, newCpo.F5);
            Assert.True(newCpo.F6);
            Assert.Equal(899999212312, newCpo.F7);
            Assert.Equal(35, newCpo.F8);
            Assert.Equal(80, newCpo.F9);
            Assert.Equal((uint)347312, newCpo.F10);
            Assert.Equal((ulong)98876868687897876, newCpo.F11);
            Assert.Equal(300, newCpo.F12);
            Assert.Equal(9.4134123175123m, newCpo.F13);
            Assert.Equal('t', newCpo.F14);
            Assert.True(TestHelper.DateTimeEqual(new DateTime(1995, 10, 21), newCpo.F15));
            Assert.Equal("F1", newCpo.F16.FieldA);
            Assert.Equal(3412, newCpo.F16.FieldB);
            Assert.Equal(60098.1234124, newCpo.F16.FieldC);
            Assert.False(newCpo.F16.FieldD);
            Assert.Equal(id, newCpo.F17);
        }

        [Fact]
        public void TestStandard01()
        {
            for (var i = 0; i < 10; i++)
            {
                TestDeletePost(i);
            }
        }

        [Fact]
        public void TestStandard02()
        {
            var rand = new Random((int)(0x0000ffff & DateTime.Now.Ticks));
            for (var i = 0; i < 20; i++)
            {
                var n = rand.Next() % 1000;
                TestDeletePost(n);
            }
        }

        [Fact]
        public void TestStandard03()
        {
            for (var i = 0; i < 10; i++)
            {
                TestPost(i);
            }
        }

        [Fact]
        public void TestStandard04()
        {
            var rand = new Random((int)(0x0000ffff & DateTime.Now.Ticks));
            for (var i = 0; i < 20; i++)
            {
                var n = rand.Next() % 1000;
                TestPost(n);
            }
        }


        [Fact]
        public void TestStandard05()
        {
            var tasks = new Task[4];
            var counter = AtomicInt32.From(0);
            for (var n = 0; n < 4; n++)
            {
                tasks[n] = Task.Factory.StartNew(() =>
                {
                    for (var i = 0; i < 100; i++)
                    {
                        TestPost(i);
                        counter.Increment();
                    }
                });
            }

            Task.WaitAll(tasks);
            Assert.Equal(400, counter.Value);
        }

        [Fact]
        public void TestLarge01()
        {
            var rand = new Random((int)(0x0000ffff & DateTime.Now.Ticks));
            var context = JsonMapper.NewContext().UseDateFormat("MM/dd/yyyy HH:mm:ss").For<Note>(y => y.ConstructWith(x =>
            {
                var jsonObj = x as JObject;
                Note note;
                if (jsonObj.ContainsKey("index"))
                {
                    var foot = new Footnote();
                    foot.note = jsonObj.GetString("note");
                    foot.writtenBy = jsonObj.GetString("writtenBy");
					foot.index = long.Parse(jsonObj["index"].Value);
                    foot.createadAt = DateTime.Parse(jsonObj.GetString("createadAt"));
                    note = foot;
                }
                else
                {
                    var head = new Headnote();
                    head.note = jsonObj.GetString("note");
                    head.writtenBy = jsonObj.GetString("writtenBy");
                    head.modifiedAt = DateTime.Parse(jsonObj.GetString("modifiedAt"));
                    note = head;
                }
                return note;
            }));
            for (var i = 0; i < 10; i++)
            {
                var n = rand.Next();
                TestBook(context, n);
            }
        }

        private void TestBook(IMapContext context, int n)
        {
            var large = Book.Factory<Book, Genre, Page, Headnote, Footnote>(n, x => (Genre)x);
            var json = context.ToJson(large);
            var newLarge = context.To<Book>(json);
            AssertBook(large, newLarge);
        }

        private void AssertBook(Book book1, Book book2)
        {
			var otherChanges = book2 != null ? book2.changes.ToList() : null;
			var thisChanges = book1.changes.ToList();
			if (otherChanges != null) otherChanges.Sort();
			thisChanges.Sort();
			var otherKeys = book2 != null ? book2.metadata.Keys.ToList() : null;
			var thisKeys = book1.metadata.Keys.ToList();
			if (otherKeys != null) otherKeys.Sort();
			thisKeys.Sort();
            Assert.True(book2 != null && book2.ID == book1.ID && book2.title == book1.title);
            Assert.True(book2.authorId == book1.authorId);
            Assert.True(book2.pages != null && Enumerable.SequenceEqual(book2.pages, book1.pages));
            Assert.True(book2.published == book1.published);
            Assert.True(book2.cover != null && Enumerable.SequenceEqual(book2.cover, book1.cover));
            Assert.True(otherChanges != null && Enumerable.SequenceEqual(otherChanges, thisChanges));
            Assert.True(otherKeys != null && Enumerable.SequenceEqual(otherKeys, thisKeys));
            Assert.True(otherKeys.All(it => book2.metadata[it] == book1.metadata[it]));
            Assert.True(book2.genres != null && Enumerable.SequenceEqual(book2.genres, book1.genres)); 
        }

        private void AssertDateTime(DateTime v1, DateTime v2)
        {
            Assert.Equal(v1.Year, v2.Year);
            Assert.Equal(v1.Month, v2.Month);
            Assert.Equal(v1.Day, v2.Day);
            Assert.Equal(v1.Hour, v2.Hour);
            Assert.Equal(v1.Minute, v2.Minute);
            Assert.Equal(v1.Second, v2.Second);
        }

        private void TestPost(int i)
        {
            var post = Post.Factory<Post, Vote, PostState, Comment>(i, x => (PostState)x);
            var json = JsonMapper.ToJson(post);
            var newPost = JsonMapper.To<Post>(json);
            if (post.approved.HasValue)
            {
                AssertDateTime(post.approved.Value, newPost.approved.Value);
            }
            Assert.Equal(post.comments?.Count, newPost.comments?.Count);
            if (post.comments != null)
            {
                for (var j = 0; j < post.comments.Count; j++)
                {
                    if (post.comments[j].approved.HasValue)
                    {
                        AssertDateTime(post.comments[j].approved.Value, newPost.comments[j].approved.Value);
                    }
                    else
                    {
                        Assert.False(newPost.comments[j].approved.HasValue);
                    }

                    Assert.Equal(post.comments[j].created, newPost.comments[j].created);
                    Assert.Equal(post.comments[j].message, newPost.comments[j].message);
                    Assert.Equal(post.comments[j].user, newPost.comments[j].user);
                    Assert.Equal(post.comments[j].votes?.downvote, newPost.comments[j].votes?.downvote);
                    Assert.Equal(post.comments[j].votes?.upvote, newPost.comments[j].votes?.upvote);
                }
            }
            AssertDateTime(post.created, newPost.created);
            Assert.Equal(post.ID, newPost.ID);
            Assert.Equal(post.notes?.Count, newPost.notes?.Count);
            if (post.notes != null)
            {
                for (var j = 0; j < post.notes.Count; j++)
                {
                    Assert.Equal(post.notes[j], newPost.notes[j]);
                }
            }
            Assert.Equal(post.state, newPost.state);
            Assert.Equal(post.tags?.Count, newPost.tags?.Count);
            if (post.tags != null)
            {
                foreach (var t in post.tags)
                {
                    newPost.tags.Contains(t);
                }
            }
            Assert.Equal(post.text, newPost.text);
            Assert.Equal(post.title, newPost.title);
            Assert.Equal(post.votes?.downvote, newPost.votes?.downvote);
            Assert.Equal(post.votes?.upvote, newPost.votes?.upvote);
        }

        private void TestDeletePost(int i)
        {
            var dlt = DeletePost.Factory<DeletePost, PostState>(i, x => (PostState)x);
            var json = JsonMapper.ToJson(dlt);
            var newDlt = JsonMapper.To<DeletePost>(json);
            Assert.Equal(dlt.deletedBy, newDlt.deletedBy);
            Assert.Equal(dlt.lastModified.Year, newDlt.lastModified.Year);
            Assert.Equal(dlt.lastModified.Month, newDlt.lastModified.Month);
            Assert.Equal(dlt.lastModified.Day, newDlt.lastModified.Day);
            Assert.Equal(dlt.lastModified.Hour, newDlt.lastModified.Hour);
            Assert.Equal(dlt.lastModified.Minute, newDlt.lastModified.Minute);
            Assert.Equal(dlt.lastModified.Second, newDlt.lastModified.Second);
            Assert.Equal(dlt.postID, newDlt.postID);
            Assert.Equal(dlt.reason, newDlt.reason);
            Assert.Equal(dlt.referenceId, newDlt.referenceId);
            Assert.Equal(dlt.state, newDlt.state);
            Assert.Equal(dlt.versions?.Length, newDlt.versions?.Length);
            Assert.Equal(dlt.votes?.Count, dlt.votes?.Count);
            if (dlt.versions != null)
            {
                for (var j = 0; j < dlt.versions.Length; j++)
                {
                    Assert.Equal(dlt.versions[j], newDlt.versions[j]);
                }
            }
            if (dlt.votes != null)
            {
                for (var j = 0; j < dlt.votes.Count; j++)
                {
                    Assert.Equal(dlt.votes[j], newDlt.votes[j]);
                }
            }
        }
    }
}
