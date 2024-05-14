using System.Collections;

namespace MoonshotNet.ChatCompletions
{
    public class MoonshotChatHistory : IList<MoonshotMessage>, IReadOnlyList<MoonshotMessage>
    {
        private readonly List<MoonshotMessage> messages;

        public MoonshotChatHistory()
        {
            messages = new();
        }

        public MoonshotChatHistory(IEnumerable<MoonshotMessage> messages)
        {
            ArgumentNullException.ThrowIfNull(messages, nameof(messages));
            this.messages = new(messages);
        }

        public MoonshotChatHistory(string systemMessage)
        {
            messages = new();
            AddSystemMessage(systemMessage);
        }

        private void AddMessage(MoonshotChatRole role, string content) => messages.Add(new MoonshotMessage(role, content));

        public void AddUserMessage(string content) => AddMessage(MoonshotChatRole.User, content);

        public void AddAssistantMessage(string content) => AddMessage(MoonshotChatRole.Assistant, content);

        private void AddSystemMessage(string content) => AddMessage(MoonshotChatRole.System, content);

        public int Count => messages.Count;

        IEnumerator IEnumerable.GetEnumerator() => messages.GetEnumerator();

        MoonshotMessage IReadOnlyList<MoonshotMessage>.this[int index] => messages[index];

        public IEnumerator<MoonshotMessage> GetEnumerator()
        {
            return messages.GetEnumerator();
        }

        MoonshotMessage IList<MoonshotMessage>.this[int index]
        {
            get => messages[index];
            set
            {
                ArgumentNullException.ThrowIfNull(value, nameof(value));
                messages[index] = value;
            }
        }

        public bool IsReadOnly => false;

        public void Add(MoonshotMessage item)
        {
            ArgumentNullException.ThrowIfNull(item, nameof(item));
            messages.Add(item);
        }

        public void AddRange(IEnumerable<MoonshotMessage> items)
        {
            ArgumentNullException.ThrowIfNull(items, nameof(items));
            messages.AddRange(items);
        }

        public bool Contains(MoonshotMessage item)
        {
            ArgumentNullException.ThrowIfNull(item, nameof(item));
            return messages.Contains(item);
        }

        public void CopyTo(MoonshotMessage[] array, int arrayIndex)
        {
            messages.CopyTo(array, arrayIndex);
        }

        public int IndexOf(MoonshotMessage item)
        {
            ArgumentNullException.ThrowIfNull(item, nameof(item));
            return messages.IndexOf(item);
        }

        public void Insert(int index, MoonshotMessage item)
        {
            ArgumentNullException.ThrowIfNull(item, nameof(item));
            messages.Insert(index, item);
        }

        public bool Remove(MoonshotMessage item)
        {
            ArgumentNullException.ThrowIfNull(item, nameof(item));
            return messages.Remove(item);
        }

        public void RemoveAt(int index) => messages.RemoveAt(index);

        public void RemoveRange(int index, int count)
        {
            messages.RemoveRange(index, count);
        }

        public void Clear() => messages.Clear();
    }
}
