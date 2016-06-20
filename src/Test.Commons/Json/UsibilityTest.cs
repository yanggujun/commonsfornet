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
using System.Collections.Generic;
using Xunit;

namespace Test.Commons.Json
{
    public class UsibilityTest
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
        public void TestLarge01()
        {
            var rand = new Random((int)(0x0000ffff & DateTime.Now.Ticks));
            for (var i = 0; i < 10; i++)
            {
                var n = rand.Next();
                TestBook(n);
            }
        }

        private void TestBook(int n)
        {
            JsonMapper.For<Note>().ConstructWith(x =>
            {
                var jsonObj = x as JObject;
                Note note;
                if (jsonObj.ContainsKey("index"))
                {
                    var foot = new Footnote();
                    foot.note = jsonObj.GetString("note");
                    foot.writtenBy = jsonObj.GetString("writtenBy");
                    foot.index = jsonObj.GetInt64("index");
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
            });
            var large = Book.Factory<Book, Genre, Page, Headnote, Footnote>(n, x => (Genre)x);
            var json = JsonMapper.ToJson(large);
            var newLarge = JsonMapper.To<Book>(json);
            Assert.True(large.Equals(newLarge));
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
