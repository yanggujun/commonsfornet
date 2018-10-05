using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.Test.Json
{
    public class NewBook : IEquatable<NewBook>
    {
        private static DateTime NOW = DateTime.UtcNow;
        private static List<byte[]> ILLUSTRATIONS = new List<byte[]>();
        static NewBook()
        {
            var rnd = new Random(1);
            for (int i = 0; i < 10; i++)
            {
                var buf = new byte[256 * i * i * i];
                rnd.NextBytes(buf);
                ILLUSTRATIONS.Add(buf);
            }
        }

        public NewBook()
        {
            pages = new LinkedList<BlankPage>();
            changes = new HashSet<DateTime>();
            metadata = new Dictionary<string, string>();
            genres = new Genre[0];
            cover = new byte[0];//otherwise buggy ProtoBuf and some other libs
        }
        public int ID { get; set; }
        public string title { get; set; }
        public int authorId { get; set; }
        public LinkedList<BlankPage> pages { get; set; }
        public DateTime? published { get; set; }
        public byte[] cover { get; set; }
        public HashSet<DateTime> changes { get; set; }
        public Dictionary<string, string> metadata { get; set; }
        public Genre[] genres { get; set; }
        public override int GetHashCode() { return ID; }
        public override bool Equals(object obj) { return Equals(obj as Book); }
        public bool Equals(NewBook other)
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
        public static TB Factory<TB, TG, TP>(int i, Func<int, TG> cast)
            where TB : new()
            where TG : struct
            where TP : new()
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
                book.pages.AddLast(page);
            }
            return book;
        }
    }

    public class BlankPage : IEquatable<Page>
    {
        public BlankPage()
        {
            identity = Guid.NewGuid();
        }
        public string text { get; set; }
        public Guid identity { get; set; }
        public override int GetHashCode() { return identity.GetHashCode(); }
        public override bool Equals(object obj) { return Equals(obj as Page); }
        public bool Equals(Page other)
        {
            var result = other != null;
            result = result && other.text == this.text;
            result = result && other.identity == this.identity;
            return result;
        }
    }
}
