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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Commons.Test.Json
{
    public class Book : IEquatable<Book>
    {
        private static DateTime NOW = DateTime.UtcNow;
        private static List<byte[]> ILLUSTRATIONS = new List<byte[]>();
        static Book()
        {
            var rnd = new Random(1);
            for (int i = 0; i < 10; i++)
            {
                var buf = new byte[256 * i * i * i];
                rnd.NextBytes(buf);
                ILLUSTRATIONS.Add(buf);
            }
        }

        public Book()
        {
            pages = new LinkedList<Page>();
            changes = new HashSet<DateTime>();
            metadata = new Dictionary<string, string>();
            genres = new Genre[0];
            cover = new byte[0];//otherwise buggy ProtoBuf and some other libs
        }
        public int ID { get; set; }
        public string title { get; set; }
        public int authorId { get; set; }
        public LinkedList<Page> pages { get; set; }
        public DateTime? published { get; set; }
        public byte[] cover { get; set; }
        public HashSet<DateTime> changes { get; set; }
        public Dictionary<string, string> metadata { get; set; }
        public Genre[] genres { get; set; }
        public override int GetHashCode() { return ID; }
        public override bool Equals(object obj) { return Equals(obj as Book); }
        public bool Equals(Book other)
        {
            var otherChanges = other != null ? other.changes.ToList() : null;
            var thisChanges = changes.ToList();
            if (otherChanges != null) otherChanges.Sort();
            thisChanges.Sort();
            var otherKeys = other != null ? other.metadata.Keys.ToList() : null;
            var thisKeys = this.metadata.Keys.ToList();
            if (otherKeys != null) otherKeys.Sort();
            thisKeys.Sort();
            var result = other != null && other.ID == this.ID && other.title == this.title;
            result = result && other.authorId == this.authorId;
            result = result && other.pages != null && Enumerable.SequenceEqual(other.pages, this.pages);
            result = result && other.published == this.published;
            result = result && other.cover != null && Enumerable.SequenceEqual(other.cover, this.cover);
            result = result && otherChanges != null && Enumerable.SequenceEqual(otherChanges, thisChanges);
            result = result && otherKeys != null && Enumerable.SequenceEqual(otherKeys, thisKeys);
            result = result && otherKeys.All(it => other.metadata[it] == this.metadata[it]);
            result = result && other.genres != null && Enumerable.SequenceEqual(other.genres, this.genres);
            return result;
        }
        public static TB Factory<TB, TG, TP, TH, TF>(int i, Func<int, TG> cast)
            where TB : new()
            where TG : struct
            where TP : new()
            where TH : new()
            where TF : new()
        {
            dynamic book = new TB();
            book.ID = -i;
            book.authorId = i / 100;
            book.published = i % 3 == 0 ? null : (DateTime?)NOW.AddMinutes(i).Date;
            book.title = "book title " + i;
            var genres = new TG[i % 2];
            for (int j = 0; j < i % 2; j++)
                genres[j] = cast((i + j) % 4);
            book.genres = genres;
            for (int j = 0; j < i % 20; j++)
                book.changes.Add(NOW.AddMinutes(i).Date);
            for (int j = 0; j < i % 50; j++)
                book.metadata["key " + i + j] = "value " + i + j;
            if (i % 3 == 0 || i % 7 == 0) book.cover = ILLUSTRATIONS[i % ILLUSTRATIONS.Count];
            var sb = new StringBuilder();
            for (int j = 0; j < i % 1000; j++)
            {
                sb.Append("some text on page " + j);
                sb.Append("more text for " + i);
                dynamic page = new TP();
                page.text = sb.ToString();
                for (int z = 0; z < i % 100; z++)
                {
                    dynamic note;
                    if (z % 3 == 0)
                    {
                        note = new TH();
                        note.modifiedAt = NOW.AddSeconds(i);
                        note.note = "headnote " + j + " at " + z;
                    }
                    else
                    {
                        note = new TF();
                        note.createadAt = NOW.AddSeconds(i);
                        note.note = "footnote " + j + " at " + z;
                        note.index = i;
                    }
                    if (z % 3 == 0)
                        note.writtenBy = "author " + j + " " + z;
                    page.notes.Add(note);
                }
                book.pages.AddLast(page);
            }
            return book;
        }
    }
    public enum Genre
    {
        Action,
        Romance,
        Comedy,
        SciFi
    }
    public class Page : IEquatable<Page>
    {
        public Page()
        {
            notes = new List<Note>();
            identity = Guid.NewGuid();
        }
        public string text { get; set; }
        public List<Note> notes { get; set; }
        public Guid identity { get; set; }
        public override int GetHashCode() { return identity.GetHashCode(); }
        public override bool Equals(object obj) { return Equals(obj as Page); }
        public bool Equals(Page other)
        {
            var result = other != null;
            result = result && other.text == this.text;
            result = result && Enumerable.SequenceEqual(other.notes, this.notes);
            result = result && other.identity == this.identity;
            return result;
        }
    }
    public class Footnote : Note, IEquatable<Footnote>
    {
        public string note { get; set; }
        public string writtenBy { get; set; }
        public DateTime createadAt { get; set; }
        public long index { get; set; }
        public override int GetHashCode() { return (int)index; }
        public override bool Equals(object obj) { return Equals(obj as Footnote); }
        public bool Equals(Footnote other)
        {
            var result = other != null;
            result = result && other.note == this.note;
            result = result && other.writtenBy == this.writtenBy;
            result = result && TestHelper.DateTimeEqual(other.createadAt, createadAt);
            result = result && other.index == this.index;
            return result;
        }
    }
    public class Headnote : Note, IEquatable<Headnote>
    {
        public string note { get; set; }
        public string writtenBy { get; set; }
        public DateTime? modifiedAt { get; set; }
        public override int GetHashCode() { return (modifiedAt ?? DateTime.MinValue).GetHashCode(); }
        public override bool Equals(object obj) { return Equals(obj as Headnote); }
        public bool Equals(Headnote other)
        {
            var result = other != null;
            result = result && other.note == this.note;
            result = result && other.writtenBy == this.writtenBy;
            result = result && TestHelper.DateTimeEqual(other.modifiedAt.Value, modifiedAt.Value);
            return result;
        }
    }
    public interface Note
    {
        string note { get; set; }
        string writtenBy { get; set; }
    }
}
